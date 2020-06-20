// -----------------------------------------------------------------------
//  <copyright file="IdentityService.cs" company="Innocellence">
//      Copyright (c) 2014-2015 Innocellence. All rights reserved.
//  </copyright>
//  <last-editor>@Innocellence</last-editor>
//  <last-date>2015-04-22 17:21</last-date>
// -----------------------------------------------------------------------

using System.Threading.Tasks;
using Infrastructure.Core;
using Infrastructure.Core.Data;
using Infrastructure.Utility.Data;
using Infrastructure.Web.Domain.Service.Common;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.ModelsView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Innocellence.WeChat.Domain.Common;
using Innocellence.Weixin.QY.AdvancedAPIs.App;
using Innocellence.Weixin.QY.AdvancedAPIs.MailList;

namespace Innocellence.WeChat.Domain.Services
{
    /// <summary>
    /// 业务实现——问卷调查模块
    /// </summary>
    public partial class MessageService : BaseService<Message>, IMessageService
    {
        private readonly Dictionary<int, GetTagMemberResult> tagMemberdDictionary = new Dictionary<int, GetTagMemberResult>();
        private readonly IArticleThumbsUpService _articleThumbsUpService;

        public MessageService(IUnitOfWork unitOfWork, 
            IArticleThumbsUpService articleThumbsUpService)
            : base("CAAdmin")
        {
            _articleThumbsUpService = articleThumbsUpService;
        }

        public MessageService()
            : base("CAAdmin")
        {
        }

        /// <summary>
        /// InsertView
        /// </summary>
        /// <param name="objModal"></param>
        /// <returns></returns>
        public override int InsertView<T>(T objModalSrc)
        {
            int iRet;
            var objModal = (MessageView)(IViewModel)objModalSrc;
            Guid myCode = Guid.NewGuid();

            Message obj = new Message();
            obj = Mapping(objModal, obj);
           // obj.Code = myCode;
            obj.Status = ConstData.STATUS_NEW;
            obj.ReadCount = 0;
            iRet = Repository.Insert(obj);
            objModal.Id = obj.Id;

            return iRet;
        }

        #region Map Method.
        public Message Mapping(MessageView objModal, Message obj)
        {
            obj = objModal.MapTo<Message>();

           // obj.Code = objModal.Code;

            return obj;
        }
        #endregion

        public List<T> GetList<T>(Expression<Func<Message, bool>> predicate) where T : IViewModel, new()
        {
            var lst = Repository.Entities.Where(predicate).ToList().Select(a => new Message
            {
                Id = a.Id,
                AppId = a.AppId,
                Title = a.Title,
                PublishDate = a.PublishDate,
                ReadCount = a.ReadCount
            }).Select(n => (T)(new T().ConvertAPIModel(n))).ToList();

            return lst;
        }

        public override List<T> GetList<T>(Expression<Func<Message, bool>> func, PageCondition page)
        {
            var total = 0;

            var list = GetList<MessageView>(func, page.PageIndex, page.PageSize, ref  total, page.SortConditions);
            var ids = list.AsParallel().Select(x => x.Id).ToList();
            var thumbsups = _articleThumbsUpService.Repository.Entities.Where(x => ids.Contains(x.ArticleID) &&
                x.IsDeleted == false && x.Type == ThumbupType.Message.ToString()).Select(x => new { x.ArticleID }).ToList().AsParallel();
            page.RowCount = total;

            Parallel.ForEach(list, x =>
            {
                x.ThumbsUpCount = thumbsups.Count(y => y.ArticleID == x.Id);
            });

            return list.Select(x => (T)(IViewModel)x).ToList();
        }

        public bool CheckMessagePushRule(int appId,int AccountMangageID, IList<int> selectedDepartmentIds, IList<int> selectedTagIds, IList<string> selectedWeChatUserIDs, out CheckResult checkResult, bool isToAllUsers)
        {
            checkResult = new CheckResult();
            var appInfo = WeChatCommonService.GetAppInfo(appId);
           
            IList<string> allAssignedUsers;

            if (!CheckVisualRange(AccountMangageID,appInfo, out allAssignedUsers, () => GenerateTagInfoDictionary(appInfo)))
            {
                checkResult.ErrorUsers = selectedWeChatUserIDs;
                checkResult.ErrorDepartments = GetErrorDepartments(selectedDepartmentIds, AccountMangageID);
                //checkResult.ErrorTags = GetErrorDepartments(selectedTagIds);
                return false;
            }

            if (isToAllUsers)
            {
                return CheckUser(AccountMangageID,allAssignedUsers, selectedWeChatUserIDs, checkResult);
            }

            var isDepartmentPass = CheckDepartment(appInfo, selectedDepartmentIds, checkResult, AccountMangageID);

            //var isTagPass = CheckTag(appInfo, selectedTagIds, allAssignedUsers, checkResult);

            var isUserPass = CheckUser(AccountMangageID,allAssignedUsers, selectedWeChatUserIDs, checkResult);

            return isDepartmentPass && isUserPass;
        }

        private bool CheckDepartment(GetAppInfoResult appInfo, IList<int> selectedDepartmentIds, CheckResult checkResult, int AccountMangageID)
        {
            //直接分配给应用
            var assignedDepartmentIds = appInfo.allow_partys.partyid.ToList();

            var underTagDepartmentIDs = tagMemberdDictionary.Select(x => x.Value).SelectMany(x => x.partylist).ToList();

            assignedDepartmentIds.AddRange(underTagDepartmentIDs);

            //TODO:调整
            if (!assignedDepartmentIds.Any())
            {
                if (!selectedDepartmentIds.Any()) return true;

                checkResult.ErrorDepartments = GetErrorDepartments(selectedDepartmentIds, AccountMangageID);

                return false;
            }

            var departments = WeChatCommonService.GetSubDepartments(assignedDepartmentIds.Distinct().ToList(), AccountMangageID).ToList();

            #region hiddlen
            //var allowUsers = WeChatCommonService.lstUser.Where(x => x.department.Any(y => departments.Any(z => z.id == y))).ToList();

            //var needUsers = WeChatCommonService.lstUser.Where(x => x.department.Any(y => selectedDepartmentIds.Any(s => s == y))).ToList();

            //var errorUsers = needUsers.Where(x => allowUsers.All(u => x.userid != u.userid)).ToList();

            //if (!errorUsers.Any())
            //{
            //    return true;
            //}

            //var errorDepartments = WeChatCommonService.lstDepartment.Where(x => errorUsers.SelectMany(y => y.department).Any(z => z == x.id)).ToList();

            //var errorDepartmentIds = selectedDepartmentIds.Where(x => WeChatCommonService.GetSubDepartments(x).Any(y => errorDepartments.Any(z => z.id == y.id))).ToList();

            //if (!errorDepartmentIds.Any()) return true;
            #endregion

            var errorDepartmentIds = selectedDepartmentIds.Where(x => departments.All(y => x != y.id)).ToList();

            if (!errorDepartmentIds.Any())
            {
                return true;
            }

            checkResult.ErrorDepartments = GetErrorDepartments(errorDepartmentIds, AccountMangageID);

            return false;
        }

        private bool CheckTag(GetAppInfoResult appInfo,int AccountMangageID, IList<int> selectedTagIds, IEnumerable<string> allAssignedUsers, CheckResult checkResult)
        {
            if (!selectedTagIds.Any()) return true;

            var needTags = selectedTagIds.Select(selectedTagId => new TagEntity { TagId = selectedTagId, TagMember = WeChatCommonService.GetTagMembers(selectedTagId, int.Parse(appInfo.agentid)) }).ToList();

            //needTags.Select(x => new { TagId = x.Key, TagMember = x.Value }).ToList();

            var needUsers = needTags.SelectMany(x => x.TagMember.userlist.Select(y => y.userid)).ToList();

            var needParties = WeChatCommonService.GetSubDepartments(needTags.SelectMany(x => x.TagMember.partylist).ToList(), AccountMangageID).ToList();

            needUsers.AddRange(WeChatCommonService.lstUser(AccountMangageID).Where(x => needParties.Any(y => x.department.Any(z => z == y.id))).Select(x => x.userid).ToList());

            var errorUsers = needUsers.Where(x => allAssignedUsers.Any(y => x == y)).ToList();

            var errorTags = needTags.Where(x => errorUsers.Any(y => x.TagMember.userlist.Any(z => z.userid == y))).Select(x => x.TagId).ToList();

            if (!errorTags.Any()) return true;

            checkResult.ErrorTags = GetErrorTags(errorTags, AccountMangageID);
            return false;
        }

        private static bool CheckUser(int AccountMangageID,IEnumerable<string> allAssignedUsers, IList<string> selectedWeChatUserIDs, CheckResult checkResult)
        {
            if (!selectedWeChatUserIDs.Any())
            {
                return true;
            }

            var errorUsers = selectedWeChatUserIDs.Where(id => allAssignedUsers.All(assignedUserId => string.Compare(id, assignedUserId, StringComparison.CurrentCultureIgnoreCase) != 0) || WeChatCommonService.lstUser(AccountMangageID).First(x => string.Compare(x.userid, id, StringComparison.CurrentCultureIgnoreCase) == 0).status == 4).ToList();

            if (!errorUsers.Any()) return true;
            checkResult.ErrorUsers = errorUsers;
            return false;
        }

        private static IList<ErrorDesc> GetErrorDepartments(IEnumerable<int> errorDepartments,int iAccountManageID)
        {
            return errorDepartments.Select(x =>
                {
                    var department = WeChatCommonService.lstDepartment(iAccountManageID).FirstOrDefault(y => x == y.id);
                    if (department == null)
                    {
                        throw new InnocellenceException(string.Format("the department id {0} have not been find!", x));
                    }
                    return new ErrorDesc { Id = department.id, Name = department.name };
                }).ToList();
        }

        private static IList<ErrorDesc> GetErrorTags(IEnumerable<int> errorTags, int AccountMangageID)
        {
            return errorTags.Select(x =>
            {
                var tag = WeChatCommonService.lstTag(AccountMangageID).FirstOrDefault(y => int.Parse(y.tagid) == x);
                if (tag == null)
                {
                    throw new InnocellenceException(string.Format("the tag id {0} have not been find!", x));
                }
                return new ErrorDesc { Id = int.Parse(tag.tagid), Name = tag.tagname };
            }).ToList();
        }

        private bool CheckVisualRange(int AccountMangageID,GetAppInfoResult appInfo, out  IList<string> allAssignedUsers, Action func = null)
        {
            allAssignedUsers = null;

            var isConfig = appInfo.allow_partys.partyid.Any() || appInfo.allow_tags.tagid.Any() || appInfo.allow_userinfos.user.Any();
            if (!isConfig)
            {
                return false;
            }

            if (func != null)
            {
                func();
            }

            //TODO:获取直接配置的用户信息
            var assignedUsers = appInfo.allow_userinfos.user.Select(x => x.userid).ToList();

            var departments = appInfo.allow_partys.partyid.ToList();

            foreach (var tagInfo in tagMemberdDictionary.Values)
            {
                //user under tag
                assignedUsers.AddRange(tagInfo.userlist.Select(x => x.userid).ToList());

                departments.AddRange(tagInfo.partylist);
            }

            var subDepartments = WeChatCommonService.GetSubDepartments(departments.Distinct().ToList(), AccountMangageID).ToList();

            //TODO:获取部门下的人
            assignedUsers.AddRange(WeChatCommonService.lstUser(AccountMangageID).Where(x => x.department.Any(y => subDepartments.Any(d => d.id == y))).Select(x => x.userid).ToList());

            allAssignedUsers = assignedUsers.Distinct().ToList();

            return true;
        }

        private void GenerateTagInfoDictionary(GetAppInfoResult appInfo)
        {
            foreach (var allowTag in appInfo.allow_tags.tagid)
            {
                tagMemberdDictionary.Add(allowTag, WeChatCommonService.GetTagMembers(allowTag, int.Parse(appInfo.agentid)));
            }
        }
    }

    public class TagEntity
    {
        public int TagId { get; set; }

        public GetTagMemberResult TagMember { get; set; }
    }

}