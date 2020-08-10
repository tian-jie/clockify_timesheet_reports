using Infrastructure.Core.Logging;
using Infrastructure.Utility.Data;
using Infrastructure.Web.UI;
using Innocellence.WeChat.Domain;
using Kevin.T.Timesheet.Entities;
using Kevin.T.Timesheet.Interfaces;
using Kevin.T.Timesheet.ModelsView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace Kevin.T.Timesheet.Controllers
{
    public class ResourcePlanController : BaseController<ResourcePlan, ResourcePlanView>
    {
        private ILogger Logger = LogManager.GetLogger("Timesheet");
        private IResourcePlanService _resourcePlanService;

        public ResourcePlanController(IResourcePlanService resourcePlanService)
            : base(resourcePlanService)
        {
            _resourcePlanService = resourcePlanService;
        }

        public override ActionResult Index()
        {
            return View();
        }

        public ActionResult UploadResourcePlanExcel(string projectGid)
        {
            try
            {
                // 保存excel文件
                HttpPostedFileBase objFile = Request.Files[0];
                string strPathFrom = string.Format("{0}\\Content\\UploadFiles\\images\\{1}\\{2}", Request.PhysicalApplicationPath, DateTime.Now.ToString("yyyy"), DateTime.Now.ToString("MMdd"));
                if (!System.IO.Directory.Exists(strPathFrom))
                {
                    System.IO.Directory.CreateDirectory(strPathFrom);
                }

                var strPath = string.Format("{0}\\{1}**{2}", strPathFrom,
                    DateTime.Now.Ticks, System.IO.Path.GetExtension(objFile.FileName));
                //  dic.Add(objFile.FileName, objFile.InputStream);
                strPath = strPath.Replace("**", "");
                objFile.SaveAs(strPath);

                // 保存数据
                _resourcePlanService.UploadResourcePlan(strPath, projectGid);

                return Json(new { status = 200, Message = "成功" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = 500, ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }

        public override ActionResult GetList()
        {
            string strProjectGid = Request["ProjectGid"];
            var q = _resourcePlanService.GetTimeEntriesByProjectGroupByEmployee(strProjectGid);
            return Json(q, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetData()
        {
            Expression<Func<ResourcePlan, bool>> predicate = m => true;
            PageCondition pageCondition = new PageCondition(1, 1000);

            var list = GetListEx(predicate, pageCondition);

            var result = new List<string>();

            return Json(list, JsonRequestBehavior.AllowGet);
        }

    }
}