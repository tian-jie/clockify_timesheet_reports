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

namespace Innocellence.WeChat.Domain.Services
{
    /// <summary>
    /// 业务实现——口令管理模块
    /// </summary>
    public partial class AutoReplyKeywordService : BaseService<AutoReplyKeyword>, IAutoReplyKeywordService
    {
        public AutoReplyKeywordService()
            : base("CAAdmin")
        {
        }



        public List<T> GetList<T>(Expression<Func<AutoReplyKeyword, bool>> predicate) where T : IViewModel, new()
        {
            var list = Repository.Entities.Where(predicate).ToList().Select(n => (T)(new T().ConvertAPIModel(n))).ToList();

            return list;
        }
      
    }
}