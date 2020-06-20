using Infrastructure.Utility.Data;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Innocellence.WeChatMain.Controllers
{
    public class UserBehaviorApiController : ApiController
    {
        IUserBehaviorService _objService;
        public UserBehaviorApiController(IUserBehaviorService objService)
        {
            _objService = objService;
        }

        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public object Post(string oid, string url)
        {
            try
            {
                // TODO: 分析URL，获取moduleId信息
                int AppId = 0;
                // TODO: 分析Request，获取IP，设备等信息
                string ip = "";
                string device = "";
                if (_objService.Repository.Insert(new UserBehavior()
                {
                    UserId = oid,
                    Url = url,
                    AppId = AppId,
                    Device = device,
                    ClientIp = ip,
                    CreatedTime = DateTime.Now
                }) == 1)
                {
                    return Json(new OperationResult(OperationResultType.Success));
                }
                else
                {
                    return Json(new OperationResult(OperationResultType.Error, "Insert Data Error."));
                }
            }
            catch (Exception ex)
            {
                return Json(new OperationResult(OperationResultType.Error, ex.Message));
            }
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}