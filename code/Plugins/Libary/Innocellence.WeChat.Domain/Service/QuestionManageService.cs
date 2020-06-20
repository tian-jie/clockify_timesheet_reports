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
using Infrastructure.Core.Logging;
using Infrastructure.Web.Domain.Service;
using Innocellence.WeChat.Domain.Contracts.ViewModel;


namespace Innocellence.WeChat.Domain.Services
{
    /// <summary>
    /// 业务实现——在线问答
    /// </summary>
    public partial class QuestionManageService : BaseService<QuestionManage>, IQuestionManageService
    {
        public QuestionManageService(IUnitOfWork unitOfWork)
            : base("CAAdmin")
        {


        }

        public QuestionManageService()
            : base()
        {

        }
        //根据id显示列表
        public T GetById<T>(int id) where T : IViewModel, new()
        {
            Expression<Func<QuestionManage, bool>> predicate = a => a.IsDeleted == false && a.Id == id;

            var t = Repository.Entities.Where(predicate).ToList().Select(n => (T)(new T().ConvertAPIModel(n))).FirstOrDefault();

            return t;
        }
        //获取状态列表
        public List<string> GetStatusList(int appid)
        {
           
            Expression<Func<QuestionManage, bool>> predicate = a => a.IsDeleted == false && a.AppId == appid;
            var t = Repository.Entities.Where(predicate).Select(a => a.Status).Distinct().ToList();
            return t;
        }
        //根据状态显示列表
        public List<T> GetListByStatus<T>(int appid, string Status) where T : IViewModel, new()
        {

            Expression<Func<QuestionManage, bool>> predicate = a => a.AppId == appid && a.IsDeleted == false && a.Status == Status;

            var lst = Repository.Entities.Where(predicate).ToList().Select(n => (T)(new T().ConvertAPIModel(n))).ToList();

            return lst;
        }
        //根据QUserId显示列表
        public List<T> GetListByQUserId<T>(int appid, string QUserId,string category=null) where T : IViewModel, new()
        {
            Expression<Func<QuestionManage, bool>> predicate;
            if (category == null)
            {
                predicate =
                    a => a.AppId == appid && a.IsDeleted == false && a.CreatedUserId == QUserId;
            }
            else
            {
              predicate = a => a.AppId == appid && a.IsDeleted == false && a.CreatedUserId == QUserId && a.Category == category;
            }

            var lst = Repository.Entities.Where(predicate).OrderByDescending(m => m.CreatedDate).ToList().Select(n => (T)(new T().ConvertAPIModel(n))).ToList();

            return lst;
        }

        //public List<T> GetList<T>(Expression<Func<QuestionManage, bool>> predicate) where T : IViewModel, new()
        //{

        //    var lst = Repository.Entities.Where(predicate).ToList().Select(n => (T)(new T().ConvertAPIModel(n))).ToList();


        //    //更新CategoryCode
        //    var lstCate = CommonService.GetCategory(CategoryType.ArticleInfoCate);

        //    lst.ForEach(d =>
        //    {
        //        dynamic a = d;
        //        var cate = lstCate.FirstOrDefault(b => b.CategoryCode == a.ArticleCateSub);
        //        if (cate != null)
        //        {
        //            a.NewsCateName = cate.CategoryName;
        //        }
        //    });


        //    return lst;
        //}
        //排序
        public override List<T> GetList<T>(Expression<Func<QuestionManage, bool>> predicate,
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
                IOrderedQueryable<QuestionManage> orderSource = null;
                foreach (SortCondition sortCondition in sortConditions)
                {
                    orderSource = count == 0
                        ? CollectionPropertySorter<QuestionManage>.OrderBy(source, sortCondition.SortField, sortCondition.ListSortDirection)
                        : CollectionPropertySorter<QuestionManage>.ThenBy(orderSource, sortCondition.SortField, sortCondition.ListSortDirection);
                    count++;
                }
                source = orderSource;
            }
            var lst = source != null
                ? source.Skip((pageIndex - 1) * pageSize).Take(pageSize)
                : Enumerable.Empty<QuestionManage>();

            return lst.ToList().Select(n => (T)(new T().ConvertAPIModel(n))).ToList();
        }

        public override int InsertView<T>(T objModalSrc)
        {
            return AddOrUpdate(objModalSrc, true);

        }

        public override int UpdateView<T>(T objModalSrc)
        {

            return AddOrUpdate(objModalSrc, false);

        }

        public List<T> GetQuestionList<T>(Expression<Func<QuestionManage, bool>> predicate) where T : IViewModel, new()
        {
            var lst = Repository.Entities.Where(predicate).OrderByDescending(m => m.CreatedDate).ToList().Select(n => (T)(new T().ConvertAPIModel(n))).ToList();
            return lst;
        }
        private int AddOrUpdate<T>(T objModalSrc, bool bolAdd)
        {
            QuestionManageView objView = objModalSrc as QuestionManageView;
            if (objView == null)
            {
                return -1;
            }
            int iRet;

            var Question = objView.MapTo<QuestionManage>();

            if (bolAdd)
            {
                Question.ReadCount = 0;
                iRet = Repository.Insert(Question);
                objView.Id = Question.Id;
            }
            else
            {
                iRet = Repository.Update(Question);
            }
            BaseService<QuestionImages> ser = new BaseService<QuestionImages>("CAAdmin");
            if (!string.IsNullOrEmpty(objView.ImageIdList))
            {
                foreach (var imageid in objView.ImageIdList.Split(','))
                {
                    if (!string.IsNullOrEmpty(imageid))
                    {
                        QuestionImages qt = new QuestionImages();
                        qt.Id = int.Parse(imageid);
                        qt.QuestionID = objView.Id;
                        ser.Repository.Update(qt, new List<string>() { "QuestionID" });
                    }

                }
            }
            return iRet;

        }

        public  int Update(QuestionManage entity)
        {
            LogManager.GetLogger(this.GetType()).Debug("QuestionManageService.Update - QuestionStatus="
                + entity.Status);
            return Repository.Update(entity);
        }

    }

}