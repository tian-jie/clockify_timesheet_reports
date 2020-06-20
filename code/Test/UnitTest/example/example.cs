using System;
using System.Collections.Generic;
using Infrastructure.Utility;
using Infrastructure.Utility.Data;
using Innocellence.CA.Entity;
using Innocellence.CA.ModelsView;
using Innocellence.CA.Service;
using Innocellence.CA.Services;
using Innocellence.Weixin.CommonService.QyMessageHandlers;
using NUnit.Framework;

namespace UnitTest.example
{
    public class example : SetUp
    {
        [Test]
        public void DB()
        {
            var articleInfoService = new ArticleInfoService();

            //添加测试数据
            articleInfoService.Repository.Insert(new ArticleInfo
            {
                Id = 1,
                ArticleTitle = "test",
                ArticleContent = "content",
                ArticleCateSub = "1",
                //ArticleCate = 1,
                ArticleCode = Guid.NewGuid(),
                ArticleComment = "comment",
                //ArticleContentEdit = "edit",
                ArticleStatus = "new",
                ArticleURL = "abc/123.html",
                CreatedDate = DateTime.Now,
                CreatedUserID = "me",
                IsDeleted = false,
                LanguageCode = "zh",
                ReadCount = 0,
                UpdatedDate = DateTime.Now
            });

            var article = articleInfoService.GetList<ArticleInfoView>(x => x.Id > 0);

            Assert.IsTrue(article.Count == 1);
        }


        [Test]
        public void PinYin()
        {
            var py = PinYinConverter.Get("john.xie");

            Assert.IsTrue(py.Length > 0);
        }
    }

    public class UserServiceCase : DbSetUp
    {
        [Test]
        public void UserService()
        {
            var userService = new AppUserService();

            var list = userService.QueryUser("张", new PageCondition());

            Assert.IsTrue(list.Count > 3);
        }

        [Test]
        public void JsonTest()
        {
            var list = JsonHelper.FromJson<List<MenuGroup>>(@"[{AppId: 15,MenuGroups: [{GroupName: 'SALES',Menus: ['SM_SALES_SPP', 'SM_SALES_BONUS', 'SM_SALES_VEEVA', 'SM_SALES_RAPID', 'SM_SALES_OTHER'],NeedTags: ['SF_DM', 'SF_NSD', 'SF_RSD', 'SF_REP', 'SPP_OPERATION'],		ErrorMessage: '对不起，您没有访问该菜单的权限!'	}, {		GroupName: 'MARKET',		Menus: ['SM_MARKET_HCP', 'SM_MARKET_SPEAKER', 'SM_MARKET_RAPID', 'SM_MARKET_VEEVA', 'SM_MARKET_OTHER'],		NeedTags: ['SPP_MKT'],		ErrorMessage: '对不起，您没有访问该菜单的权限!'	}]}, {	AppId: 16,	MenuGroups: [{		GroupName: 'representation',		Menus: ['ST_RCOUR_VIEW', 'ST_RTRA_BAA', 'ST_RMICRO_FORUM', 'ST_RMICRO_SHARE', 'ST_RLEARN_TYRANT'],		NeedTags: ['SF_RSD', 'SF_DM', 'SF_REP', 'SF_NSD', 'SF_Trainer'],		ErrorMessage: '您如果需要了解代表的培训信息,请联系培训部!'	}, {		GroupName: 'manger',		Menus: ['ST_ECOUR_VIEW', 'ST_ETRA_BAA', 'ST_EMICRO_FORUM', 'ST_EMICRO_SHARE'],		NeedTags: ['SF_DM', 'SF_RSD', 'SF_NSD', 'SF_Trainer'],		ErrorMessage: '如果您需要了解地区经理的相关信息,请联系您的主管!'	}]}]");

            Assert.IsTrue(list.Count > 0);
        }

        [Test]
        public void PerssionCheck()
        {
            cache.Remove("Category");
            string message;
            SmartphoneMenuPermissionManager.PermissionCheck("ST_RCOUR_VIEW", "WF81892", 16, out message);

            Assert.IsTrue(message == null);
        }
    }
}
