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
using System.Text;
using System.Threading.Tasks;

using Infrastructure.Core;
using Infrastructure.Core.Data;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.Contracts;
using System.Linq.Expressions;
using Innocellence.WeChat.Domain.Service;
using Innocellence.WeChat.Domain.Common;
using Innocellence.WeChat.Domain.ModelsView;
using System.IO;
using Infrastructure.Utility.Data;
using Infrastructure.Utility.Filter;


namespace Innocellence.WeChat.Domain.Services
{
    /// <summary>
    /// 业务实现——新闻
    /// </summary>
    public partial class ImageInfoService : BaseService<ImageInfo>, IImageInfoService
    {
        public ImageInfoService()
            : base("CAAdmin")
        {

        }

        public List<T> GetListByParent<T>(int ownerId) where T : IViewModel, new()
        {
            Expression<Func<ImageInfo, bool>> predicate = a => a.OwnerId == ownerId;
            var ens = Repository.Entities.ToList() ;

            var lst = ens.Select(n => (T)(new T().ConvertAPIModel(n))).ToList();            

            return lst;
        }


        public override List<T> GetList<T>(Expression<Func<ImageInfo, bool>> predicate,
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
                IOrderedQueryable<ImageInfo> orderSource = null;
                foreach (SortCondition sortCondition in sortConditions)
                {
                    orderSource = count == 0
                        ? CollectionPropertySorter<ImageInfo>.OrderBy(source, sortCondition.SortField, sortCondition.ListSortDirection)
                        : CollectionPropertySorter<ImageInfo>.ThenBy(orderSource, sortCondition.SortField, sortCondition.ListSortDirection);
                    count++;
                }
                source = orderSource;
            }
            var lst = source != null
                ? source.Skip((pageIndex - 1) * pageSize).Take(pageSize)
                : Enumerable.Empty<ImageInfo>();

            return lst.ToList().Select(n => (T)(new T().ConvertAPIModel(n))).ToList();

            //  var lst = this.Entities.Where(predicate).Take(iTop).ToList().Select(n => (T)(new T().ConvertAPIModel(n))).ToList();

            // return lst;
        }

        public override int InsertView<T>(T objModalSrc)
        {
            ArticleInfoView objView = objModalSrc as ArticleInfoView;
            if (objView == null)
            {
                return -1;
            }
            int iRet;

            var article = new ImageInfo();

            article = objView.MapTo<ImageInfo>();

            iRet = Repository.Insert(article);
            return iRet;
        }

        public override int UpdateView<T>(T objModalSrc)
        {
            int iRet = 0;
            ArticleInfoView objView = objModalSrc as ArticleInfoView;
            if (objView == null)
            {
                return -1;
            }

            var image = new ImageInfo();

            image = objView.MapTo<ImageInfo>();

            iRet = Repository.Update(image);
            return iRet;
        }


    }
}