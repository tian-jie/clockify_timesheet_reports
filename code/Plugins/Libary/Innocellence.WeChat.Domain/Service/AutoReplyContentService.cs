// -----------------------------------------------------------------------
//  <copyright file="AutoReplyService.cs" company="Innocellence">
//      Copyright (c) 2014-2015 Innocellence. All rights reserved.
//  </copyright>
//  <last-editor>@Innocellence</last-editor>
//  <last-date>2016-07-13 17:21</last-date>
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
using System.Text;
using Innocellence.WeChat.Domain.ViewModel;
using Infrastructure.Core.Logging;

namespace Innocellence.WeChat.Domain.Services
{
    /// <summary>
    /// 业务实现——口令管理模块
    /// </summary>
    public partial class AutoReplyContentService : BaseService<AutoReplyContent>, IAutoReplyContentService
    {

        public AutoReplyContentService()
            : base("CAAdmin")
        {
        }



        public List<T> GetList<T>(Expression<Func<AutoReplyContent, bool>> predicate) where T : IViewModel, new()
        {
            var list = Repository.Entities.Where(predicate).ToList().Select(n => (T)(new T().ConvertAPIModel(n))).ToList();

            return list;
        }

        public List<T> GetList<T>(int appId, string inputStr, int type) where T : IViewModel, new()
        {
            inputStr = CheckInputString(inputStr);
            string query = GetQueryString(appId, inputStr, type);
            LogManager.GetLogger(this.GetType()).Debug(query);
            string allPick = string.Format("select * from AutoReplyContent c where c.AutoReplyId in (select top 1 r.id from AutoReply r where r.PrimaryType = {0} and r.AppId = {1} and r.IsDeleted = 0 Order by r.UpdatedDate desc)", (int)AutoReplyKeywordEnum.ALL, appId);
            AutoReplyContent allPickContent = Repository.SqlQuery(allPick).FirstOrDefault();
            List<AutoReplyContent> allContent = Repository.SqlQuery(query).ToList();
            List<T> list = allContent.Select(n => (T)(new T().ConvertAPIModel(n))).ToList();
            //if (allContent.Any(c => c.PrimaryType != (int)AutoReplyKeywordEnum.ALL))
            //{
            //    list = allContent.Where(c => c.PrimaryType != (int)AutoReplyKeywordEnum.ALL).ToList().Select(n => (T)(new T().ConvertAPIModel(n))).ToList();
            //}
            List<int> needAllPickContentList = new List<int>()
            {
                 (int)AutoReplyKeywordEnum.TEXT,
                 (int)AutoReplyKeywordEnum.AUDIO,
                 (int)AutoReplyKeywordEnum.VIDEO,
                 (int)AutoReplyKeywordEnum.IMAGE,
                 (int)AutoReplyKeywordEnum.ALL,
            };
            if (list.Count == 0 && needAllPickContentList.Contains(type) && allPickContent != null)
            {
                list.Add((T)(new T().ConvertAPIModel(allPickContent)));
            }
            LogManager.GetLogger(this.GetType()).Debug("{0} in {1} has {2} auto replay contents", inputStr, appId, list.Count);
            return list;
        }

        private string CheckInputString(string inputStr)
        {
            if (!string.IsNullOrWhiteSpace(inputStr))
            {
                inputStr = inputStr.Replace("'", "\"");
                return inputStr;
            }
            return string.Empty;
        }

        private string GetQueryString(int appId, string inputStr, int type)
        {
            AutoReplyMenuEnum eventEnum;
            if (type == (int)AutoReplyKeywordEnum.MENU && !Enum.TryParse<AutoReplyMenuEnum>(inputStr, true, out eventEnum))
            {
                //菜单类型的keyword格式为 AutoReplyId:::菜单名
                string sql = string.Format("select top 1 r.Id from AutoReply r where r.IsDeleted = 0 AND r.Id = {0} Order by r.UpdatedDate desc", int.Parse(inputStr.Split(':')[0]));
                return string.Format("select * from AutoReplyContent c where c.AutoReplyId in ({0})", sql);
            }
            else if (type == (int)AutoReplyKeywordEnum.MENU
                || type == (int)AutoReplyMPKeywordEnum.SCAN
                || type == (int)AutoReplyMPKeywordEnum.SubscribeWithScan
                || type == (int)AutoReplyKeywordEnum.TEXT)
            {
                string query = new StringBuilder()
              .Append("select * from AutoReplyContent c where c.AutoReplyId in")
              .Append(" (")
              .Append(" select top 1 k.AutoReplyId from AutoReplyKeyword k")
              .Append(" join AutoReply r on k.AutoReplyId = r.Id")
              .Append(" where r.IsDeleted = 0 ")
              .AppendFormat(" and r.AppId = {0}", appId)
              .AppendFormat(" and (r.PrimaryType = {0})", type)
              .AppendFormat(" and ({0})", GetFilterQuertString(type, inputStr))
              .AppendFormat(" Order by r.UpdatedDate desc")
              .Append(" )")
              .ToString();
                return query;
            }
            else
            {
                string query = new StringBuilder()
              .Append("select * from AutoReplyContent c where c.AutoReplyId in")
              .Append(" (")
              .AppendFormat("select top 1 r.id from AutoReply r where r.IsDeleted = 0 and r.PrimaryType = {0} and r.AppId = {1} Order by r.UpdatedDate desc", type, appId)
              .Append(") ")
              .ToString();
                return query;
            }
        }

        private string GetFilterQuertString(int type, string inputStr)
        {
            switch (type)
            {
                case (int)AutoReplyKeywordEnum.TEXT:
                    return new StringBuilder()
                      .AppendFormat(" (k.SecondaryType = {0} and N'{1}' like k.Keyword)", (int)AutoReplyTextMatchEnum.EQUAL, inputStr)
                      .AppendFormat(" or (k.SecondaryType = {0} and N'{1}' like k.Keyword+'%')", (int)AutoReplyTextMatchEnum.START_WITH, inputStr)
                      .AppendFormat(" or (k.SecondaryType = {0} and N'{1}' like '%'+k.Keyword)", (int)AutoReplyTextMatchEnum.END_WITH, inputStr)
                      .AppendFormat(" or (k.SecondaryType = {0} and N'{1}' like '%'+k.Keyword+'%')", (int)AutoReplyTextMatchEnum.CONTAIN, inputStr)
                      .ToString();
                case (int)AutoReplyMPKeywordEnum.SCAN:
                case (int)AutoReplyMPKeywordEnum.SubscribeWithScan:
                    return new StringBuilder()
                    .AppendFormat(" k.Keyword like '{0}'", inputStr)
                    .ToString();
                case (int)AutoReplyMPKeywordEnum.MENU:
                    AutoReplyMenuEnum eventEnum;
                    Enum.TryParse<AutoReplyMenuEnum>(inputStr, true, out eventEnum);
                    return new StringBuilder()
                    .AppendFormat(" k.SecondaryType = '{0}'", (int)eventEnum)
                    .ToString();
                default:
                    return string.Format(" k.Keyword like N'{0}'", inputStr);
            }
        }
    }
}