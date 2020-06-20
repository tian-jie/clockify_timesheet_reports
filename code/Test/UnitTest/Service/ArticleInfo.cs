using System.Collections.Generic;
using System.Linq;
using Infrastructure.Utility.Data;
using Infrastructure.Web.Domain.Service;
using Innocellence.WeChat.Domain.ModelsView;
using Innocellence.WeChat.Domain.Common;
using Innocellence.WeChat.Domain.Services;
using NUnit.Framework;

namespace UnitTest.Service
{
    public class ArticleService : DbSetUp
    {
        [Test]
        public void GetArticleInfoThumbsup()
        {
            var service = new ArticleInfoService();
            var list = new List<int> { 1, 2 };
            //var r = service.Repository.Entities.Where(x => list.Contains(x.Id)).Select(x => new
            //{
            //    article = x,
            //    ArticleThumbsUps = x.ArticleThumbsUps.Where(y => y.ArticleID
            //        == x.Id && y.IsDeleted != true).ToList()
            //}).ToList();

            //Assert.IsTrue(r.Count(x => x.ArticleThumbsUps.Count == 2) == 2);
        }

        [Test]
        public void Like()
        {
            var service = new ArticleInfoService();
            service.UpdateArticleThumbsUp(1, "abc", "Article");
        }

        [Test]
        public void Encryption()
        {
            //var service = new ArticleInfoService();
            //const string id = "abdeind0393";

            //var encryptionStr = service.EncryptorXXXid(id);

            //var orginStr = EncryptionHelper.Decrypt(encryptionStr, CommonService.lstSysConfig.First(x => x.ConfigName == "UrlEncryptionKey").ConfigValue);

            //Assert.IsTrue(id == orginStr);
        }

        [Test]
        public void GetArticleThumbup()
        {
            var service = new ArticleInfoService();

            var list = service.GetList<ArticleInfoView>(x => x.Id == 68, new PageCondition());

            Assert.IsTrue(list.Count == 1);
        }
    }
}
