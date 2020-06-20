using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Core.Data;
using Infrastructure.Utility.Data;
using Infrastructure.Utility.Extensions;
using Infrastructure.Web.Domain.Entity;
using Infrastructure.Web.Domain.Services;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.Contracts.ViewModel;
using Innocellence.WeChat.Domain.Service;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace UnitTest.Service
{
    public class MenuServiceTest : DbSetUp
    {
        [SetUp]
        public void Start()
        {
            cache.Remove("Category");
        }

        [Test]
        public void Query()
        {
            ISmartPhoneMenuService service = new SmartPhoneMenuService();

            //var view = service.QueryMenuViewById(79);

            //Assert.IsTrue(view.Function != null);
        }

        [Test]
        public void Edit()
        {
            ISmartPhoneMenuService service = new SmartPhoneMenuService();

            service.UpdateOrAdd(new AppMenuView
            {
                AppId = 18,
                ButtonReturnType = new ButtonReturnType
                {
                    Button = new MenuButton { key = "holder_tesst", type = "click" },
                    Content = "test",
                    ResponseMsgType = "Text",
                },
                CategoryCode = "news",
                CategoryName = "",
                Id = 94
            });
        }

        [Test]
        public void Push()
        {
            ISmartPhoneMenuService service = new SmartPhoneMenuService();

            service.Push(12);
        }

        [Test]
        public void Update()
        {
            var service = new CategoryService();

            var categories = service.Repository.Entities.AsEnumerable().Where(x => x.Id != 57 && !x.Function.IsNullOrEmpty() && x.AppId != 18).ToList();

            categories.ForEach(category =>
            {
                var btn = JsonHelper.FromJson<MenuButton>(category.Function);
                var buttonReturnType = new ButtonReturnType { Button = btn, Content = null, ResponseMsgType = null };

                category.Function = JsonConvert.SerializeObject(buttonReturnType, Formatting.Indented);

                service.Repository.Update(category);
            });

            Assert.IsTrue(categories.Count > 0);

        }

        [Test]
        public void Delete()
        {
            var service = new SmartPhoneMenuService();

            //var categories = service.Repository.Entities.Where(x => x.AppId == 17).ToList();

            service.UpdateOrAdd(new AppMenuView { Id = 91, IsDeleted = true });
        }

        [Test]
        public void CreateJobLog()
        {
            var service = new BaseService<ReportJobLog>();

            service.Repository.Insert(new ReportJobLog
            {
                CreatedDate = DateTime.Now,
                DateFrom = DateTime.Now.AddDays(-2),
                DateTo = DateTime.Now,
                JobName = "MenuReportJob",
                JobStatus = "Running"
            });

            var logs = service.Repository.Entities.ToList();

            Assert.IsTrue(logs.Count >= 1);

        }

        [Test]
        public void ChangeMenuName()
        {
            var service = new CategoryService();

            var list = service.Repository.Entities.AsNoTracking().ToList();

            var errorList = new List<Category>();

            Parallel.ForEach(list, item =>
            {
                try
                {
                    var function = item.Function.IsNullOrEmpty() ? null : JsonHelper.FromJson<ButtonReturnType>(item.Function);

                    if (function == null) return;
                    if (item.CategoryName == function.Button.name) return;

                    function.Button.name = item.CategoryName;
                    item.Function = JsonHelper.ToJson(function);
                    service.Repository.Update(item);
                }
                catch (Exception e)
                {
                    errorList.Add(item);
                }
            });


        }
    }

    public class KeysJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var t = JToken.FromObject(value);

            if (t.Type != JTokenType.Object && value != null)
            {
                t.WriteTo(writer);
            }
        }
    }
}
