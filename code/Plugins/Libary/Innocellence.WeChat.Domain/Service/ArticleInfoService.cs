// -----------------------------------------------------------------------
//  <copyright file="IdentityService.cs" company="Innocellence">
//      Copyright (c) 2014-2015 Innocellence. All rights reserved.
//  </copyright>
//  <last-editor>@Innocellence</last-editor>
//  <last-date>2015-04-22 17:21</last-date>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Infrastructure.Core;
using Infrastructure.Core.Data;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Entity;
using System.Linq.Expressions;
using Innocellence.WeChat.Domain.Service;
using Innocellence.WeChat.Domain.Common;
using Innocellence.WeChat.Domain.ModelsView;
using Infrastructure.Utility.Data;
using Infrastructure.Utility.Filter;
using Infrastructure.Web.Domain.Service;
using Infrastructure.Web.Domain.Service.Common;
using System.Text;
using Innocellence.Weixin.QY.AdvancedAPIs.App;
using Innocellence.Weixin.QY.AdvancedAPIs.MailList;
using Innocellence.WeChat.Domain.ViewModel;
using System.Collections;

namespace Innocellence.WeChat.Domain.Services
{
    /// <summary>
    /// 业务实现——新闻
    /// </summary>
    public partial class ArticleInfoService : BaseService<ArticleInfo>, IArticleInfoService
    {
        private IImageInfoService _imageService;
        private const string keyName = "UrlEncryptionKey";

        private readonly IArticleThumbsUpService _articleThumbsUpService;
        private readonly Dictionary<int, GetTagMemberResult> tagMemberdDictionary = new Dictionary<int, GetTagMemberResult>();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="imageService"></param>
        /// <param name="articleThumbsUpService"></param>
        public ArticleInfoService(IUnitOfWork unitOfWork,
            IImageInfoService imageService,
            IArticleThumbsUpService articleThumbsUpService)
            : base("CAAdmin")
        {
            _imageService = imageService;
            _articleThumbsUpService = articleThumbsUpService;
        }

        /// <summary>
        /// 
        /// </summary>
        public ArticleInfoService()
            : base("CAAdmin")
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public T GetById<T>(int id) where T : IViewModel, new()
        {
            Expression<Func<ArticleInfo, bool>> predicate = a => a.Id == id && a.IsDeleted == false;

            var t = Repository.Entities.Where(predicate).ToList().Select(n => (T)(new T().ConvertAPIModel(n))).FirstOrDefault();

            return t;
        }

        //public ArticleInfo GetByCateSub(string articleCateSub)
        //{
        //    Expression<Func<ArticleInfo, bool>> predicate = a => a.IsDeleted == false && a.ArticleCateSub == articleCateSub;

        //    var t = Repository.Entities.FirstOrDefault(predicate);//.ToList().FirstOrDefault();

        //    return t;
        //}
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Newscode"></param>
        /// <returns></returns>
        public List<T> GetListByCode<T>(string Newscode) where T : IViewModel, new()
        {
            Guid strCode = Guid.Parse(Newscode);
            Expression<Func<ArticleInfo, bool>> predicate = a => a.IsDeleted == false && a.ArticleCode == strCode;

            var lst = Repository.Entities.Where(predicate).ToList().Select(n => (T)(new ArticleInfoView().ConvertAPIModelList(n))).ToList();

            return lst;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public List<T> GetList<T>(Expression<Func<ArticleInfo, bool>> predicate) where T : IViewModel, new()
        {

            var lst = Repository.Entities.Where(predicate).ToList().Select(n => (T)(new ArticleInfoView().ConvertAPIModelList(n))).ToList();


            //更新CategoryCode
            //var lstCate = CommonService.GetCategory(CategoryType.ArticleInfoCate);

            lst.ForEach(d =>
            {
                ArticleInfoView a = (ArticleInfoView)(IViewModel)d;

                //var cate = lstCate.FirstOrDefault(b => b.CategoryCode == a.ArticleCateSub);
                var cate = CommonService.GetCategory(a.AppId.Value, false).FirstOrDefault(b => b.Id == a.CategoryId);
                if (cate != null)
                {
                    a.ArticleCateName = cate.CategoryName;
                }
                else
                {
                    a.ArticleCateName = "其他";
                }
            });


            return lst;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strIDs"></param>
        /// <returns></returns>
        public List<NewsInfoView> GetListByNewsIDs(string strIDs)
        {
            if (!string.IsNullOrWhiteSpace(strIDs))
            {
                var IDs = Array.ConvertAll<string, int>(strIDs.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries), a => int.Parse(a));
                Dictionary<int, int> indexMap = new Dictionary<int, int>();
                for (int i = 0; i < IDs.Length; i++)
                {
                    indexMap.Add(IDs[i], i);
                }
                var lst = Repository.Entities.Where(a => IDs.Contains(a.Id)).ToList().Select(n => (NewsInfoView)(new ArticleInfoView().ConvertToNewsInfoView(n))).OrderBy(a => a.Id, new NewIdComparer(indexMap)).ToList();
                return lst;
            }
            return new List<NewsInfoView>();
        }
        /// <summary>
        /// 
        /// </summary>
        public class NewIdComparer : IComparer<int>
        {
            private Dictionary<int, int> _indexMap;
            public NewIdComparer(Dictionary<int, int> indexMap)
            {
                _indexMap = indexMap;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public int Compare(int x, int y)
            {
                int result = 0;
                var i1 = _indexMap[x];
                var i2 = _indexMap[y];
                if (i1 > i2)
                {
                    result = 1;
                }
                else if (i1 == i2)
                {
                    result = 0;
                }
                else
                {
                    result = -1;
                }
                return result;
            }
        }

        //public List<T> GetArticleList<T>(Expression<Func<ArticleInfo, bool>> predicate) where T : IViewModel, new()
        //{

        //    var lst =
        //        Repository.Entities.Where(predicate).OrderByDescending(m => m.CreatedDate).ToList().Select(n => (T)(new ArticleInfoView().ConvertAPIModelList(n))).ToList();
        //    return lst;
        //}
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="total"></param>
        /// <param name="sortConditions"></param>
        /// <returns></returns>
        public override List<T> GetList<T>(Expression<Func<ArticleInfo, bool>> predicate,
           int pageIndex,
           int pageSize,
           ref int total,
          List<SortCondition> sortConditions = null)
        {
            if (total <= 0)
            {
                total = Repository.Entities.Count(predicate);
            }
            var source = Repository.Entities.Where(predicate);

            if (sortConditions == null || sortConditions.Count == 0)
            {
                source = source.OrderByDescending(m => m.Id);
            }
            else
            {
                int count = 0;
                IOrderedQueryable<ArticleInfo> orderSource = null;
                foreach (SortCondition sortCondition in sortConditions)
                {
                    orderSource = count == 0
                        ? CollectionPropertySorter<ArticleInfo>.OrderBy(source, sortCondition.SortField, sortCondition.ListSortDirection)
                        : CollectionPropertySorter<ArticleInfo>.ThenBy(orderSource, sortCondition.SortField, sortCondition.ListSortDirection);
                    count++;
                }
                source = orderSource;
            }
            var lst = source != null
                ? source.Skip((pageIndex - 1) * pageSize).Take(pageSize)
                : Enumerable.Empty<ArticleInfo>();

            return lst.ToList().Select(n => (T)(new ArticleInfoView().ConvertAPIModelList(n))).ToList();

            //  var lst = this.Entities.Where(predicate).Take(iTop).ToList().Select(n => (T)(new T().ConvertAPIModel(n))).ToList();

            // return lst;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="pageCon"></param>
        /// <returns></returns>
        public List<T> GetListWithContent<T>(Expression<Func<ArticleInfo, bool>> predicate, PageCondition pageCon)
        {
            var pageIndex = pageCon.PageIndex;
            var pageSize = pageCon.PageSize;
            var total = pageCon.RowCount;
            var sortConditions = pageCon.SortConditions;
            if (total <= 0)
            {
                total = Repository.Entities.Count(predicate);
                pageCon.RowCount = total;
            }
            var source = Repository.Entities.Where(predicate);

            if (sortConditions == null || sortConditions.Count == 0)
            {
                source = source.OrderByDescending(m => m.Id);
            }
            else
            {
                int count = 0;
                IOrderedQueryable<ArticleInfo> orderSource = null;
                foreach (SortCondition sortCondition in sortConditions)
                {
                    orderSource = count == 0
                        ? CollectionPropertySorter<ArticleInfo>.OrderBy(source, sortCondition.SortField, sortCondition.ListSortDirection)
                        : CollectionPropertySorter<ArticleInfo>.ThenBy(orderSource, sortCondition.SortField, sortCondition.ListSortDirection);
                    count++;
                }
                source = orderSource;
            }
            var lst = source != null
                ? source.Skip((pageIndex - 1) * pageSize).Take(pageSize)
                : Enumerable.Empty<ArticleInfo>();

            return lst.ToList().Select(n => (T)(new ArticleInfoView().ConvertAPIModelListWithContent(n))).ToList();

            //  var lst = this.Entities.Where(predicate).Take(iTop).ToList().Select(n => (T)(new T().ConvertAPIModel(n))).ToList();

            // return lst;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objModalSrc"></param>
        /// <returns></returns>
        public override int InsertView<T>(T objModalSrc)
        {

            return AddOrUpdate(objModalSrc, true, default(List<string>));

        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objModalSrc"></param>
        /// <returns></returns>
        public override int UpdateView<T>(T objModalSrc)
        {

            return AddOrUpdate(objModalSrc, false, null);

        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objModalSrc"></param>
        /// <param name="lst"></param>
        /// <returns></returns>
        public override int UpdateView<T>(T objModalSrc, List<string> lst)
        {

            return AddOrUpdate(objModalSrc, false, lst);

        }

        private int AddOrUpdate<T>(T objModalSrc, bool bolAdd, List<string> lst)
        {
            ArticleInfoView objView = objModalSrc as ArticleInfoView;
            if (objView == null)
            {
                return -1;
            }
            int iRet;

            BaseService<ArticleImages> ser = new BaseService<ArticleImages>("CAAdmin");

            foreach (var a in objView.ArticleContentViews)
            {
                if (a.objImage != null)
                {
                    ser.Repository.Insert(a.objImage);
                    a.ImageID = a.objImage.Id;
                }
            }

            var article = objView.MapTo<ArticleInfo>();


            if (objView.ArticleContentViews != null && objView.ArticleContentViews.Count > 0)
            {
                // 处理article.content
                article.ArticleContentEdit = JsonHelper.ToJson(objView.ArticleContentViews);
            }


            if (bolAdd)
            {
                article.ArticleStatus = (string.IsNullOrEmpty(article.ArticleStatus) ? ConstData.STATUS_NEW : article.ArticleStatus);

                article.ReadCount = 0;
                article.ThumbsUpCount = 0;
                article.IsDeleted = false;
                iRet = Repository.Insert(article);
                objView.Id = article.Id;
            }
            else
            {
                if (lst == null || lst.Count == 0)
                {
                    iRet = Repository.Update(article);
                }
                else
                {
                    iRet = Repository.Update(article, lst);
                }
            }

            if (objView.ArticleContentViews != null)
            {
                foreach (var a in objView.ArticleContentViews)
                {
                    if (a.objImage != null)
                    {
                        a.objImage.ArticleID = article.Id;
                        ser.Repository.Update(a.objImage, new List<string>() { "ArticleID" });
                    }
                }
            }

            return iRet;
        }

        public int UpdateArticleThumbsUp(int articleId, string userId, string type)
        {

            bool isDelete = false;

            var likedUsers = _articleThumbsUpService.Repository.Entities.Where(x => x.ArticleID == articleId && x.Type == type).Select(x => new { UserID = x.UserID, Id = x.Id, IsDelete = x.IsDeleted }).ToList();

            var currentUser = likedUsers.FirstOrDefault(x => x.UserID == userId);

            var likedCount = likedUsers.Count(x => x.IsDelete != true);

            if (currentUser != null)
            {
                if (currentUser.IsDelete == true)
                {
                    _articleThumbsUpService.Repository.Update(new ArticleThumbsUp { Id = currentUser.Id, IsDeleted = false }, new List<string> { "IsDeleted" });
                    likedCount++;
                }
                else
                {
                    _articleThumbsUpService.Repository.Update(new ArticleThumbsUp { Id = currentUser.Id, IsDeleted = true }, new List<string> { "IsDeleted" });
                    likedCount--;
                    isDelete = true;
                }

            }
            else
            {
                _articleThumbsUpService.Repository.Insert(new ArticleThumbsUp
                {
                    ArticleID = articleId,
                    UserID = userId,
                    Type = type
                });
                likedCount++;
            }

            //更新文章表的点赞总数。
            this.Repository.SqlExcute(string.Format("update ArticleInfo set ThumbsUpCount=ThumbsUpCount{1} where id={0}", articleId, isDelete ? "-1" : "+1"));

            return likedCount;
        }

        //public override List<T> GetList<T>(Expression<Func<ArticleInfo, bool>> func, PageCondition page)
        //{
        //    var total = 0;

        //    var list = GetList<ArticleInfoView>(func, page.PageIndex, page.PageSize, ref total, page.SortConditions);
        //    var ids = list.AsParallel().Select(x => x.Id).ToList();
        //    var thumbsups = _articleThumbsUpService.Repository.Entities.Where(x => ids.Contains(x.ArticleID) &&
        //        x.IsDeleted != true && x.Type == ThumbupType.Article.ToString()).Select(x => new { x.ArticleID }).ToList().AsParallel();
        //    page.RowCount = total;


        //    //点赞的时候应该直接存储到文章表，不能循环查询，循环中禁止查询sql，影响会很大。实在不行可以关联查询
        //    //Parallel.ForEach(list, x =>
        //    //{
        //    //    x.ThumbsUpCount = thumbsups.Count(y => y.ArticleID == x.Id);
        //    //});

        //    return list.Select(x => (T)(IViewModel)x).ToList();
        //}

        //private static IQueryable<ArticleLikeEntity> GetArticleInfosWithThumbsup(IQueryable<ArticleInfo> queryable)
        //{
        //    // return queryable.Select(x => new
        //    //ArticleLikeEntity
        //    //{
        //    //    ArticleInfo = x,
        //    //    ArticleThumbsUps = x.ArticleThumbsUps.Where(y => y.ArticleID
        //    //        == x.Id && y.IsDeleted != true).ToList()
        //    //});

        //    throw new NotImplementedException();
        //}

        public string EncryptorWeChatUserID(string WeChatUserID)
        {
            return EncryptionHelper.Encrypt(WeChatUserID, CommonService.lstSysConfig.First(x => x.ConfigName == keyName).ConfigValue);
        }

        public string DecryptWeChatUserID(string encryWeChatUserID)
        {
            return EncryptionHelper.Decrypt(encryWeChatUserID, CommonService.lstSysConfig.First(x => x.ConfigName == keyName).ConfigValue);
        }

        //public List<T> GetArticleListByFilter<T>(string appId, string strCateSub, string keywords, PageCondition con) where T : IViewModel, new()
        //{
        //    StringBuilder sb = new StringBuilder();
        //    sb.AppendFormat("SELECT * FROM ArticleInfo WHERE 1=1 AND ArticleStatus Like '{0}'", ConstData.STATUS_PUBLISH);
        //    if (!string.IsNullOrEmpty(appId))
        //    {
        //        int appid;
        //        if (int.TryParse(appId, out appid))
        //        {
        //            sb.AppendFormat(" AND AppId={0}", appid);
        //        }
        //    }

        //    if (!string.IsNullOrEmpty(strCateSub))
        //    {
        //        int cate;
        //        if (int.TryParse(strCateSub, out cate))
        //        {
        //            sb.AppendFormat(" AND CategoryId={0}", cate);
        //        }
        //    }

        //    if (!string.IsNullOrEmpty(keywords))
        //    {
        //        keywords = keywords.Trim();
        //        var keys = keywords.Split(' ');
        //        if (keys.Length > 0)
        //        {
        //            string filter = string.Empty;
        //            foreach (var key in keys)
        //            {
        //                filter += string.Format(" ArticleTitle LIKE N'%{0}%' OR ArticleContent LIKE N'%{0}%' OR", key);
        //            }
        //            sb.AppendFormat(" AND ({0})", filter.TrimEnd('O', 'R'));
        //        }
        //    }

        //    sb.Append(" ORDER BY CreatedDate DESC, Id DESC");

        //    var source = this.Repository.SqlQuery(sb.ToString());

        //    if (con.SortConditions == null || con.SortConditions.Count == 0)
        //    {
        //        source = source.OrderByDescending(a => a.Id);
        //    }
        //    else
        //    {
        //        int count = 0;
        //        dynamic orderSource = null;
        //        foreach (SortCondition sortCondition in con.SortConditions)
        //        {
        //            orderSource = count == 0
        //                ? CollectionPropertySorter<ArticleInfo>.OrderBy(source, sortCondition.SortField, sortCondition.ListSortDirection)
        //                : CollectionPropertySorter<ArticleInfo>.ThenBy(orderSource, sortCondition.SortField, sortCondition.ListSortDirection);
        //            count++;
        //        }
        //        source = orderSource;
        //    }
        //    con.RowCount = source.ToList().Count;
        //    var lst = source != null
        //       ? source.Skip((con.PageIndex - 1) * con.PageSize).Take(con.PageSize)
        //       : Enumerable.Empty<ArticleInfo>();

        //    return lst.ToList().Select(n => (T)(new T().ConvertAPIModel(n))).ToList();
        //}








        public bool CheckMessagePushRule(int appId, int AccountMangageID, IList<int> selectedDepartmentIds, IList<int> selectedTagIds, IList<string> selectedWeChatUserIDs, out CheckResult checkResult, bool isToAllUsers)
        {
            checkResult = new CheckResult();
            var appInfo = WeChatCommonService.GetAppInfo(appId);

            IList<string> allAssignedUsers;

            if (!CheckVisualRange(AccountMangageID, appInfo, out allAssignedUsers, () => GenerateTagInfoDictionary(appInfo)))
            {
                checkResult.ErrorUsers = selectedWeChatUserIDs;
                checkResult.ErrorDepartments = GetErrorDepartments(selectedDepartmentIds, AccountMangageID);
                //checkResult.ErrorTags = GetErrorDepartments(selectedTagIds);
                return false;
            }

            if (isToAllUsers)
            {
                return CheckUser(AccountMangageID, allAssignedUsers, selectedWeChatUserIDs, checkResult);
            }

            var isDepartmentPass = CheckDepartment(appInfo, selectedDepartmentIds, checkResult, AccountMangageID);

            //var isTagPass = CheckTag(appInfo, selectedTagIds, allAssignedUsers, checkResult);

            var isUserPass = CheckUser(AccountMangageID, allAssignedUsers, selectedWeChatUserIDs, checkResult);

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

        private bool CheckTag(GetAppInfoResult appInfo, int AccountMangageID, IList<int> selectedTagIds, IEnumerable<string> allAssignedUsers, CheckResult checkResult)
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

        private static bool CheckUser(int AccountMangageID, IEnumerable<string> allAssignedUsers, IList<string> selectedWeChatUserIDs, CheckResult checkResult)
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

        private static IList<ErrorDesc> GetErrorDepartments(IEnumerable<int> errorDepartments, int iAccountManageID)
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

        private bool CheckVisualRange(int AccountMangageID, GetAppInfoResult appInfo, out IList<string> allAssignedUsers, Action func = null)
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

    public class ArticleLikeEntity
    {
        public ArticleInfo ArticleInfo { get; set; }

        public IList<ArticleThumbsUp> ArticleThumbsUps { get; set; }
    }

    public enum ThumbupType
    {
        Article,
        Message
    }
}