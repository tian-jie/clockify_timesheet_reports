using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Infrastructure.Core.Data;
using Infrastructure.Web.Domain.Contracts;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.ModelsView;

namespace Innocellence.WeChat.Domain.Service
{
    /// <summary>
    /// 业务实现——问卷调查模块
    /// </summary>
    public class FeedBackService : BaseService<FeedBackEntity>, IFeedbackService
    {
        private readonly ICategoryService _categoryService;
        public FeedBackService(ICategoryService categoryService)
            : base("CAAdmin")
        {
            _categoryService = categoryService;
        }

        public IList<FeedBackView> QueryList(int appId, string menuCode = null)
        {
            Expression<Func<FeedBackEntity, bool>> where;
            if (menuCode == null)
            {
                where = x => x.AppID == appId;
            }
            else
            {
                where = x => x.AppID == appId && x.MenuCode == menuCode;
            }

            var list = Repository.Entities.Where(where).ToList();

            var codes = list.Select(y => y.MenuCode).ToList();

            //var categories = _categoryService.Repository.Entities.Where(x => codes.Contains(x.CategoryCode)).Select(x => new { code = x.CategoryCode, name = x.CategoryName }).ToList();

            var viewList = list.Select(x =>
                 {
                     //var category = categories.Find(y => y.code == x.MenuCode);
                     var view = new FeedBackView();
                     view.ConvertAPIModel(x);
                     //view.MenuName = category.name;
                     return view;
                 }).ToList();


            return viewList;
        }
    }
}