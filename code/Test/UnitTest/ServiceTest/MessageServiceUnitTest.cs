using AutoMapper;
using Infrastructure.Core.Data;
using Innocellence.CA.Entity;
using Innocellence.CA.ModelsView;
using Innocellence.CA.Services;
using Innocellence.CA.Services.Fakes;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UnitTest.ServiceTest
{
    [TestClass]
    public class MessageServiceUnitTest : FakeSetUp
    {
        [TestMethod]
        public void InsertViewTestMethod()
        {
            using (ShimsContext.Create())
            {
                ShimMessageService.Constructor = d =>
                {
                    d.Repository = new Repository<Message, int>(new MockContent());
                };

                MessageView testView = new MessageView();
                testView.AppId = 16;
                testView.Title = "MessageTestFirst";
                testView.Content = "MessageTestContent1";
                testView.CreatedDate = DateTime.Now;
                testView.CreatedUserID = "cwwhy1";
                testView.UpdatedDate = DateTime.Now;
                testView.UpdatedUserID = "cwwhy1";
                testView.Code = Guid.NewGuid();
                testView.ReadCount = 0;
                testView.ThumbsUpCount = 0;
                testView.IsDeleted = false;
                testView.ThumbImageId = 16;
                testView.PublishDate = DateTime.Now.AddDays(1);
                Mapper.CreateMap<MessageView, Message>();
                var messageTestService = new ShimMessageService(new MessageService());
                var instance = messageTestService.Instance;
                int actual = instance.InsertView(testView);
                int expected = 1;
                Assert.AreEqual(expected, actual);
            }
        }

        [TestMethod]
        public void MappingTestMethod()
        {
            MessageView testView = new MessageView();
            testView.AppId = 16;
            testView.Title = "MessageTest";
            testView.Content = "MessageTestContent";
            testView.CreatedDate = DateTime.Now;
            testView.CreatedUserID = "cwwhy1";
            testView.UpdatedDate = DateTime.Now;
            testView.UpdatedUserID = "cwwhy1";
            testView.Code = Guid.NewGuid();
            testView.ReadCount = 0;
            testView.ThumbsUpCount = 0;
            testView.IsDeleted = false;
            testView.ThumbImageId = 16;
            testView.PublishDate = DateTime.Now.AddDays(1);
            Mapper.CreateMap<MessageView, Message>();
            //暂时用无参构造器
            var messageTestService = new MessageService();
            Message result = new Message();
            result = messageTestService.Mapping(testView, result);
            var actual = result.Code;
            var expected = testView.Code;
            Assert.AreEqual(expected, actual);
        }
    }

}
