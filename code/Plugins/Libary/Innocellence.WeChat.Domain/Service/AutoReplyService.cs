// -----------------------------------------------------------------------
//  <copyright file="AutoReplyService.cs" company="Innocellence">
//      Copyright (c) 2014-2015 Innocellence. All rights reserved.
//  </copyright>
//  <last-editor>@Innocellence</last-editor>
//  <last-date>2016-07-13 17:21</last-date>
// -----------------------------------------------------------------------

using Infrastructure.Core;
using Infrastructure.Core.Data;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.ModelsView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Innocellence.WeChat.Domain.Services
{
    /// <summary>
    /// 业务实现——口令管理模块
    /// </summary>
    public partial class AutoReplyService : BaseService<AutoReply>, IAutoReplyService
    {
        private readonly IAutoReplyKeywordService _autoReplyKeywordService = new AutoReplyKeywordService();

        private readonly IAutoReplyContentService _autoReplyContentService = new AutoReplyContentService();

        public AutoReplyService()
            : base("CAAdmin")
        {
        }



        public List<T> GetList<T>(Expression<Func<AutoReply, bool>> predicate) where T : IViewModel, new()
        {
            var list = Repository.Entities.Where(predicate).ToList().Select(n => (T)(new T().ConvertAPIModel(n))).ToList();

            return list;
        }

        /// <summary>
        /// 取得一个口令的所有详细信息
        /// </summary>
        /// <param name="autoReplyId"></param>
        /// <returns></returns>
        public AutoReplyView GetDetail(int autoReplyId)
        {
            // 主表
            var autoReply = Repository.Entities.FirstOrDefault(x => x.Id == autoReplyId);

            if (autoReply == null)
            {
                return null;
            }

            var view = AutoReplyView.ConvertFromEntity(autoReply);

            // 口令
            view.Keywords = _autoReplyKeywordService.GetList<AutoReplyKeywordView>(x => x.AutoReplyId == autoReplyId);

            // 回复
            view.Contents = _autoReplyContentService.GetList<AutoReplyContentView>(x => x.AutoReplyId == autoReplyId);

            return view;
        }

        /// <summary>
        /// 添加自动回复记录
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        public int Add(AutoReplyView view)
        {
            var entity = view.ConvertToEntity();
            var result = Repository.Insert(entity);
            if (result > 0 && entity.Id > 0)
            {
                foreach (var keyword in view.Keywords)
                {
                    var keywordEntity = keyword.ConvertToEntity();
                    keywordEntity.AutoReplyId = entity.Id;
                    _autoReplyKeywordService.Repository.Insert(keywordEntity);
                }

                foreach (var content in view.Contents)
                {
                    var contentEntity = content.ConvertToEntity();
                    contentEntity.AutoReplyId = entity.Id;
                    _autoReplyContentService.Repository.Insert(contentEntity);
                }

            }

            return result;
        }

        /// <summary>
        /// 更新自动回复记录
        /// 注：先删除全部再插入数据，这样做会使主键值增的比较快，但考虑到业务上口令不会经常变更，故采用此较为简单的方法
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        public int Update(AutoReplyView view)
        {
            var entity = view.ConvertToEntity();
            // 口令管理表
            var result = Repository.Update(entity);
            if (result > 0 && entity.Id > 0)
            {

                // 口令表
                // 删除所有
                _autoReplyKeywordService.Repository.Delete(x => x.AutoReplyId == entity.Id);
                foreach (var keyword in view.Keywords)
                {
                    var keywordEntity = keyword.ConvertToEntity();
                    // 添加
                    _autoReplyKeywordService.Repository.Insert(keywordEntity);
                }

                // 口令回复表
                // 删除所有
                _autoReplyContentService.Repository.Delete(x => x.AutoReplyId == entity.Id);
                foreach (var content in view.Contents)
                {
                    var contentEntity = content.ConvertToEntity();
                    _autoReplyContentService.Repository.Insert(contentEntity);
                }
            }

            return result;
        }

        // THIS IS NOT NEEDED, BECAUSE BASIC SERVICE HAS IMPLEMENTED THIS SEARCH
        //public List<T> GetList<T>(Expression<Func<AutoReply, bool>> predicate, PageCondition page) where T : IViewModel, new()
        //{
        //    // filter
        //    var data = Repository.Entities.Where(predicate).OrderByDescending(x => x.Id);

        //    // paging
        //    var total = data.Count();

        //    page.RowCount = total;
        //    page.PageIndex = page.PageIndex == 0 ? 1 : page.PageIndex;

        //    var list = data.Skip((page.PageIndex - 1) * page.PageSize).Take(page.PageSize).ToList().Select(n => (T)(new T().ConvertAPIModel(n))).ToList(); ;

        //    return list;
        //}


    }
}