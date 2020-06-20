/*----------------------------------------------------------------
    Copyright (C) 2015 Innocellence
    
    文件名：MenuController.cs
    文件功能描述：自定义菜单设置工具Controller
    
    
    创建标识：Innocellence - 20150312
----------------------------------------------------------------*/

using Autofac;
using Infrastructure.Core.Caching;
using Infrastructure.Core.Infrastructure;
using Infrastructure.Web.Domain.Contracts;
using Infrastructure.Web.Domain.Service;
using Infrastructure.Web.UI;
using Innocellence.WeChat.Domain.Common;
using Innocellence.Weixin;
using Innocellence.Weixin.Entities;
using Innocellence.Weixin.Exceptions;
using Innocellence.Weixin.QY.AdvancedAPIs;
using Innocellence.Weixin.QY.AdvancedAPIs.MailList;
using Innocellence.Weixin.QY.CommonAPIs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Innocellence.WeChatMain.Controllers
{
    public class DepartmentController : WinxinBaseController
    {

        private static ICacheManager _cacheManager = EngineContext.Current.Resolve<ICacheManager>(new TypedParameter(typeof(Type), typeof(CommonService)));

        public DepartmentController(ICategoryService newsService)
            : base(newsService)
        {
            // _newsService = newsService;
        }

        //
        // GET: /Menu/

        public override ActionResult Index()
        {

            return View();
        }


        private string GetToken()
        {
            var config = WeChatCommonService.lstSysWeChatConfig.Find(a => a.AccountManageId == AccountManageID);
            return AccessTokenContainer.TryGetToken(config.WeixinCorpId, config.WeixinCorpSecret);
        }




        public override ActionResult GetList()
        {

            PageParameter param = new PageParameter();

            TryUpdateModel(param);

            //实现对用户和多条件的分页的查询，rows表示一共多少条，page表示当前第几页
            param.length = param.length == 0 ? 10 : param.length;
            int iTotal = param.iRecordsTotal;

            var list = GetListData((int)Math.Floor(param.start / param.length * 1.0d) + 1, param.length, ref iTotal);// _newsService.GetList(null, null, 10, 10);
            if (list == null) { list = new List<UserList_Simple>(); }
            var t = from a in list
                    select new { Id = a.userid, UserName = a.name };

            return Json(new
            {
                sEcho = param.draw,
                iTotalRecords = iTotal,
                iTotalDisplayRecords = iTotal,
                aaData = t
            }, JsonRequestBehavior.AllowGet);
        }

        public List<UserList_Simple> GetListData(int iPage, int iCount, ref int iTotal)
        {

            string strID = Request["DeptId"];
            if (!string.IsNullOrEmpty(strID))
            {
                return MailListApi.GetDepartmentMember(GetToken(), int.Parse(strID), 0, 0).userlist;
            }

            return null;
            //o => o.Asc(f => f.Name, f => f.Id)

        }


        public override ActionResult Get(string sID)
        {
            string strID = Request["Id"];
            if (!string.IsNullOrEmpty(strID))
            {
                int Id = int.Parse(strID);

                var strToken = GetToken();
                var result = MailListApi.GetDepartmentList(strToken);

                return Json(result.department.Find(a => a.id == Id), JsonRequestBehavior.AllowGet);
            }

            return Json(null, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetListTree()
        {


            //  string strID = Request["DeptId"];
            // if (!string.IsNullOrEmpty(strID))
            // {

            var lst = WeChatCommonService.lstDepartment(AccountManageID);

            var allIds = lst.Select(a=>a.id).ToList();
            foreach (var item in lst)
            {
                if (!allIds.Contains(item.parentid))
                {
                    item.parentid = 0;
                }
            }
            return Json(EasyUITreeData.GetTreeData(new List<DepartmentList>(lst.ToArray()), "id", "name", "parentid"), JsonRequestBehavior.AllowGet);

        }


        public bool BeforeAddOrUpdate(DepartmentList objModal, int? ID)
        {
            if (objModal.parentid <= 0)
            {
                ModelState.AddModelError("Invalid Operation", "不可修改最上级部门名称。");
                return false;
            }
            return true;
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult PostDept(DepartmentList objModal, int? Id)
        {
            //验证错误
            if (!BeforeAddOrUpdate(objModal, Id))
            {
                return Json(GetErrorJson(), JsonRequestBehavior.AllowGet);
            }

            //objModal.id = int.Parse(Request["deptid"]);
            try
            {
                if (Id.HasValue && Id.Value > 0)
                {
                    MailListApi.UpdateDepartment(GetToken(), objModal);
                }
                else
                {
                    MailListApi.CreateDepartment(GetToken(), objModal);
                }
            }
            catch (ErrorJsonResultException ex)
            {
                WxJsonResult jsResult = ex.JsonResult;
                string errorMsg = ((ReturnCode_QY)jsResult.errcode).ToString();
                return ErrorNotification(errorMsg);
            }
            
            _cacheManager.Remove("DepartmentList");

            return Json(doJson(null), JsonRequestBehavior.AllowGet);
        }



        public override JsonResult Delete(string sIds)
        {
            if (!string.IsNullOrEmpty(sIds))
            {
                try
                {
                    string[] arrID = sIds.Split(',');
                    var departmentID = int.Parse(CommonService.GetSysConfig("DimissionDepartment", ""));
                    var departments = WeChatCommonService.lstDepartment(AccountManageID);
                    var dismissionDepartment = departments.Where(d => d.id == departmentID).FirstOrDefault();
                    if (dismissionDepartment != null && arrID.Contains(dismissionDepartment.id.ToString()))
                    {
                        return ErrorNotification("离职人员部门不可以被删除。");
                    }
                    foreach (string strID in arrID)
                    {
                        if (string.IsNullOrEmpty(strID)) { continue; }                        
                        MailListApi.DeleteDepartment(GetToken(), strID);
                    }
                }
                catch (ErrorJsonResultException ex)
                {
                    WxJsonResult jsResult = ex.JsonResult;
                    string errorMsg = ((ReturnCode_QY)jsResult.errcode).ToString();
                    return ErrorNotification(errorMsg);
                }     
            }
            else
            {
                //_BaseService.Delete(objModal);

            }

            _cacheManager.Remove("DepartmentList");
            return Json(doJson(null), JsonRequestBehavior.AllowGet);
        }
    }
}
