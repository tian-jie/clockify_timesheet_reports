using Infrastructure.Core.Data;
using Innocellence.CA.Entity;
using Innocellence.CA.ModelsView;
using Innocellence.CA.Services;
using Innocellence.CA.Services.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Linq;
using System.Linq.Expressions;
using Infrastructure.Utility.Data;
using AutoMapper;

namespace UnitTest.ServiceTest
{
    [TestClass]
    public class QuestionManageServiceUnitTest : FakeSetUp
    {
        [TestMethod]
        public void GetByIdTestMethod()
        {
            ShimQuestionManageService.Constructor = d =>
            {
                d.Repository = new Repository<QuestionManage, int>(new MockContent());
            };
            var questionManageService = new ShimQuestionManageService(new QuestionManageService());

            DateTime systime = DateTime.Now;

            QuestionManage questionManage1 = new QuestionManage();
            questionManage1.Id = 2;
            questionManage1.AppId = 16;
            questionManage1.CreatedUserId = "cwwhy1";
            questionManage1.UpdatedUserId = "cwwhy1";
            questionManage1.Question = "我什么时候才会有钱";
            questionManage1.Answer = "洗洗睡吧";
            questionManage1.CreatedDate = systime;
            questionManage1.UpdatedDate = systime.AddMinutes(1);
            questionManage1.Status = "已回答";
            questionManage1.ReadCount = 21;
            questionManage1.IsDeleted = false;

            questionManageService.Instance.Repository.Insert(questionManage1);

            QuestionManage questionManage2 = new QuestionManage();
            questionManage2.Id = 5;
            questionManage2.AppId = 16;
            questionManage2.CreatedUserId = "cwwhy1";
            questionManage2.Question = "来盘昆特牌吧";
            questionManage2.CreatedDate = systime.AddMinutes(10);
            questionManage2.Status = "未回答";
            questionManage2.ReadCount = 150;
            questionManage2.IsDeleted = false;

            questionManageService.Instance.Repository.Insert(questionManage2);

            var actual1 = questionManageService.Instance.GetById<QuestionManageView>(2);
            Assert.IsNotNull(actual1);

            var actual2 = questionManageService.Instance.GetById<QuestionManageView>(4);
            Assert.IsNull(actual2);
           
        }

        [TestMethod]
        public void GetStatusListTestMethod()
        {
            ShimQuestionManageService.Constructor = d =>
            {
                d.Repository = new Repository<QuestionManage, int>(new MockContent());
            };
            var questionManageService = new ShimQuestionManageService(new QuestionManageService());

            QuestionManage questionManage1 = new QuestionManage();
            questionManage1.Id = 2;
            questionManage1.AppId = 16;
            questionManage1.Status = "已回答";
            questionManage1.IsDeleted = false;

            questionManageService.Instance.Repository.Insert(questionManage1);

            QuestionManage questionManage2 = new QuestionManage();
            questionManage2.Id = 4;
            questionManage2.AppId = 16;
            questionManage2.Status = "已回答";
            questionManage2.IsDeleted = false;

            questionManageService.Instance.Repository.Insert(questionManage2);

            QuestionManage questionManage3 = new QuestionManage();
            questionManage3.Id = 5;
            questionManage3.AppId = 16;
            questionManage3.Status = "未回答";
            questionManage3.IsDeleted = false;

            questionManageService.Instance.Repository.Insert(questionManage3);

            QuestionManage questionManage4 = new QuestionManage();
            questionManage4.Id = 7;
            questionManage4.AppId = 16;
            questionManage4.Status = "未回答";
            questionManage4.IsDeleted = false;

            questionManageService.Instance.Repository.Insert(questionManage4);

            List<string> actual = questionManageService.Instance.GetStatusList(16);
            int expected = 2;
            Assert.AreEqual(expected, actual.Count);

            Assert.IsTrue(actual.Contains("已回答"));

            Assert.IsTrue(actual.Contains("未回答"));
        }

        [TestMethod]
        public void GetListByStatusTestMethod()
        {
            ShimQuestionManageService.Constructor = d =>
            {
                d.Repository = new Repository<QuestionManage, int>(new MockContent());
            };
            var questionManageService = new ShimQuestionManageService(new QuestionManageService());

            QuestionManage questionManage1 = new QuestionManage();
            questionManage1.Id = 2;
            questionManage1.AppId = 16;
            questionManage1.Status = "已回答";
            questionManage1.IsDeleted = false;

            questionManageService.Instance.Repository.Insert(questionManage1);

            QuestionManage questionManage2 = new QuestionManage();
            questionManage2.Id = 4;
            questionManage2.AppId = 17;
            questionManage2.Status = "已回答";
            questionManage2.IsDeleted = false;

            questionManageService.Instance.Repository.Insert(questionManage2);

            QuestionManage questionManage3 = new QuestionManage();
            questionManage3.Id = 5;
            questionManage3.AppId = 16;
            questionManage3.Status = "已回答";
            questionManage3.IsDeleted = true;

            questionManageService.Instance.Repository.Insert(questionManage3);

            QuestionManage questionManage4 = new QuestionManage();
            questionManage4.Id = 7;
            questionManage4.AppId = 16;
            questionManage4.Status = "未回答";
            questionManage4.IsDeleted = false;

            questionManageService.Instance.Repository.Insert(questionManage4);

            QuestionManage questionManage5 = new QuestionManage();
            questionManage5.Id = 8;
            questionManage5.AppId = 17;
            questionManage5.Status = "未回答";
            questionManage5.IsDeleted = false;

            questionManageService.Instance.Repository.Insert(questionManage5);

            QuestionManage questionManage6 = new QuestionManage();
            questionManage6.Id = 10;
            questionManage6.AppId = 16;
            questionManage6.Status = "未回答";
            questionManage6.IsDeleted = false;

            questionManageService.Instance.Repository.Insert(questionManage6);

            var actual1 = questionManageService.Instance.GetListByStatus<QuestionManageView>(
                16, "已回答");
            int expected1 = 2;
            //数据新规时，IsDeleted自动设置为false
            Assert.AreEqual(expected1, actual1.Count);

            var actual2 = questionManageService.Instance.GetListByStatus<QuestionManageView>(
                    16, "未回答");
            int expected2 = 2;
            Assert.AreEqual(expected2, actual2.Count);
        }

        [TestMethod]
        public void GetListByQUserIdTestMethod()
        {
            ShimQuestionManageService.Constructor = d =>
            {
                d.Repository = new Repository<QuestionManage, int>(new MockContent());
            };
            var questionManageService = new ShimQuestionManageService(new QuestionManageService());

            QuestionManage questionManage1 = new QuestionManage();
            questionManage1.Id = 2;
            questionManage1.AppId = 16;
            questionManage1.Status = "已回答";
            questionManage1.CreatedUserId = "SuperMan";
            questionManage1.IsDeleted = false;

            questionManageService.Instance.Repository.Insert(questionManage1);

            QuestionManage questionManage2 = new QuestionManage();
            questionManage2.Id = 4;
            questionManage2.AppId = 17;
            questionManage2.Status = "已回答";
            questionManage2.CreatedUserId = "SuperMan";
            questionManage2.IsDeleted = false;

            questionManageService.Instance.Repository.Insert(questionManage2);

            QuestionManage questionManage3 = new QuestionManage();
            questionManage3.Id = 5;
            questionManage3.AppId = 16;
            questionManage3.Status = "已回答";
            questionManage3.CreatedUserId = "SuperMan";
            questionManage3.IsDeleted = true;

            questionManageService.Instance.Repository.Insert(questionManage3);

            QuestionManage questionManage4 = new QuestionManage();
            questionManage4.Id = 7;
            questionManage4.AppId = 16;
            questionManage4.Status = "未回答";
            questionManage4.CreatedUserId = "SuperWoman";
            questionManage4.IsDeleted = false;

            questionManageService.Instance.Repository.Insert(questionManage4);

            var actual1 = questionManageService.Instance.GetListByQUserId<QuestionManageView>(
                16, "SuperMan");
            int expected1 = 0;
            Assert.AreEqual(expected1, actual1.Count);

            var claim = ClaimsPrincipal.Current.Claims.FirstOrDefault(a => a.Type == ClaimTypes.Name);
            string userId = claim == null ? string.Empty : claim.Value;
            var actual2 = questionManageService.Instance.GetListByQUserId<QuestionManageView>(
                16, userId);
            int expected2 = 3;
            Assert.AreEqual(expected2, actual2.Count);
        
        }

        [TestMethod]
        public void GetListTestMethod()
        {
            ShimQuestionManageService.Constructor = d =>
            {
                d.Repository = new Repository<QuestionManage, int>(new MockContent());
            };
            var questionManageService = new ShimQuestionManageService(new QuestionManageService());

            DateTime systime = DateTime.Now;

            QuestionManage questionManage1 = new QuestionManage();
            questionManage1.Id = 2;
            questionManage1.AppId = 16;
            questionManage1.CreatedUserId = "cwwhy1";
            questionManage1.UpdatedUserId = "cwwhy1";
            questionManage1.Question = "我什么时候才会有钱";
            questionManage1.Answer = "洗洗睡吧";
            questionManage1.CreatedDate = systime;
            questionManage1.UpdatedDate = systime.AddMinutes(1);
            questionManage1.Status = "已回答";
            questionManage1.ReadCount = 21;
            questionManage1.IsDeleted = false;

            questionManageService.Instance.Repository.Insert(questionManage1);

            QuestionManage questionManage2 = new QuestionManage();
            questionManage2.Id = 5;
            questionManage2.AppId = 16;
            questionManage2.CreatedUserId = "cwwhy1";
            questionManage2.Question = "来盘昆特牌吧";
            questionManage2.CreatedDate = systime.AddMinutes(10);
            questionManage2.Status = "未回答";
            questionManage2.ReadCount = 150;
            questionManage2.IsDeleted = false;

            questionManageService.Instance.Repository.Insert(questionManage2);

            Expression<Func<QuestionManage, bool>> predicate = a => a.AppId == 16 && a.ReadCount > 100;

            var actual1 = questionManageService.Instance.GetList<QuestionManageView>(predicate);
            var expected1_1 = 1;
            var expected1_2 = 5;
            Assert.AreEqual(expected1_1, actual1.Count);
            Assert.AreEqual(expected1_2, actual1[0].Id);
            
        }

        [TestMethod]
        public void GetListOverrideTestMethod()
        {
            ShimQuestionManageService.Constructor = d =>
            {
                d.Repository = new Repository<QuestionManage, int>(new MockContent());
            };
            var questionManageService = new ShimQuestionManageService(new QuestionManageService());

            QuestionManage questionManage1 = new QuestionManage();
            questionManage1.Id = 2;
            questionManage1.AppId = 16;
            questionManage1.Status = "已回答";
            questionManage1.ReadCount = 12;
            questionManage1.IsDeleted = false;

            questionManageService.Instance.Repository.Insert(questionManage1);

            QuestionManage questionManage2 = new QuestionManage();
            questionManage2.Id = 4;
            questionManage2.AppId = 17;
            questionManage2.Status = "已回答";
            questionManage2.ReadCount = 50;
            questionManage2.IsDeleted = false;

            questionManageService.Instance.Repository.Insert(questionManage2);

            QuestionManage questionManage3 = new QuestionManage();
            questionManage3.Id = 5;
            questionManage3.AppId = 16;
            questionManage3.Status = "已回答";
            questionManage3.ReadCount = 152;
            questionManage3.IsDeleted = false;

            questionManageService.Instance.Repository.Insert(questionManage3);

            QuestionManage questionManage4 = new QuestionManage();
            questionManage4.Id = 7;
            questionManage4.AppId = 16;
            questionManage4.Status = "未回答";
            questionManage4.ReadCount = 500;
            questionManage4.IsDeleted = false;

            questionManageService.Instance.Repository.Insert(questionManage4);

            QuestionManage questionManage5 = new QuestionManage();
            questionManage5.Id = 8;
            questionManage5.AppId = 17;
            questionManage5.Status = "未回答";
            questionManage5.ReadCount = 54;
            questionManage5.IsDeleted = false;

            questionManageService.Instance.Repository.Insert(questionManage5);

            QuestionManage questionManage6 = new QuestionManage();
            questionManage6.Id = 10;
            questionManage6.AppId = 16;
            questionManage6.Status = "未回答";
            questionManage6.ReadCount = 48;
            questionManage6.IsDeleted = false;

            questionManageService.Instance.Repository.Insert(questionManage6);

            Expression<Func<QuestionManage, bool>> predicate = a => a.AppId == 16;
            int total = 0;
            SortCondition sort = new SortCondition("ReadCount", System.ComponentModel.ListSortDirection.Descending);
            List<SortCondition> sortCondition = new List<SortCondition>();
            sortCondition.Add(sort);

            var questionManageList = questionManageService.Instance.GetList<QuestionManageView>(
                predicate, 2, 2, ref total, null);
            // total的期待值
            int expected1 = 4;
            // 目标数据的Id期待值
            int expected2 = 5;
            Assert.AreEqual(expected1, total);
            Assert.AreEqual(expected2, questionManageList[0].Id);
            //Case1: 没有排序条件默认按ID降序排

            predicate = a => a.ReadCount >= 30;
            total = 50;
            questionManageList = questionManageService.Instance.GetList<QuestionManageView>(
                predicate, 2, 3, ref total, sortCondition);

            expected1 = 50;
            expected2 = 4;
            // 返回数据条数的期待值
            int expected3 = 2;
            // 目标数据的ReadCount值
            int expected4 = 50;
            Assert.AreEqual(expected1, total);
            Assert.AreEqual(expected2, questionManageList[0].Id);
            Assert.AreEqual(expected3, questionManageList.Count);
            Assert.AreEqual(expected4, questionManageList[0].ReadCount);
            //Case2: total的意义
            
        }

        [TestMethod]
        public void AddOrUpdateTestMethod()
        {
            ShimQuestionManageService.Constructor = d =>
            {
                d.Repository = new Repository<QuestionManage, int>(new MockContent());
            };

            ShimQuestionImagesService.Constructor = d =>
            {
                d.Repository = new Repository<QuestionImages, int>(new MockContent());
            };

            ShimQuestionImagesService.Constructor = d =>
            {
                d.Repository = new Repository<QuestionImages, int>(new MockContent());
            };

            var questionManageService = new ShimQuestionManageService(new QuestionManageService());

            QuestionManageView questionManage = new QuestionManageView();
            questionManage.Id = 2;
            questionManage.AppId = 16;
            questionManage.Question = "来盘昆特牌吧";
            questionManage.Answer = "村里还没有人能胜得了我";
            questionManage.Status = "已回答";
            questionManage.ReadCount = 120;
            questionManage.IsDeleted = false;
            questionManage.ImageIdList = "24,25,26";

            int actual = questionManageService.Instance.InsertView<QuestionManageView>(null);
            int expected = -1;

            Assert.AreEqual(expected, actual);

            actual = questionManageService.Instance.UpdateView<QuestionManageView>(null);
            expected = -1;
            Assert.AreEqual(expected, actual);

            Mapper.CreateMap<QuestionManageView, QuestionManage>();

            // 目标方法由于有其他service进行Insert操作,测试中断
            //actual = questionManageService.Instance.InsertView<QuestionManageView>(questionManage);
            //expected = 1;

            //Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UpdateTestMethod()
        {
            ShimQuestionManageService.ConstructorIUnitOfWork = (d1, d2) =>
            {
                d1.Repository = new Repository<QuestionManage, int>(new MockContent());
            };

            var questionManageService = new ShimQuestionManageService(new QuestionManageService(new MockContent()));

            QuestionManage questionManage = new QuestionManage();
            questionManage.Id = 2;
            questionManage.AppId = 16;
            questionManage.Question = "来盘昆特牌吧";
            questionManage.Answer = "村里还没有人能胜得了我";
            questionManage.Status = "已回答";
            questionManage.ReadCount = 120;
            questionManage.IsDeleted = false;

            questionManageService.Instance.Repository.Insert(questionManage);

            questionManage.Id = 1;

            //进行中由于ObjectStateManager无法模拟，测试中断
            //int actual = questionManageService.Instance.Update(questionManage);
            //int expected = 0;

            //Assert.AreEqual(expected, actual);

            //questionManage.Id = 2;

            //actual = questionManageService.Instance.Update(questionManage);
            //expected = 1;

            //Assert.AreEqual(expected, actual);

        }

    }
}
