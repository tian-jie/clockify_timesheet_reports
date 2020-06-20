using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Linq.Expressions;
using Infrastructure.Utility.Data;
using Infrastructure.Web.UI;
using Infrastructure.Web.Domain.Entity;
using Infrastructure.Web.Domain.ModelsView;
using Infrastructure.Web.Domain.Contracts;
using Infrastructure.Web.Domain.Service.Common;
using Infrastructure.Web.Domain.Service;
using Innocellence.CA.Services;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Configuration;
using System.Text;
using System.Collections.ObjectModel;
using Newtonsoft.Json.Linq;

namespace Innocellence.Web.Controllers
{
    public class MSSqlController : BaseController<Category, CategoryView>
    {


        public MSSqlController(ICategoryService categoryService)
            : base(categoryService)
        {

        }

        public override ActionResult Index()
        {
            return base.Index();
        }

        //初始化list页面
        public override ActionResult GetList()
        {
            GridRequest req = new GridRequest(Request);

            int iCount = req.PageCondition.RowCount;
            string strSql = Request["strSql"];
            //  var mq = System.Text.RegularExpressions.Regex.Matches(strSql, @"select (.*?) from (\w*) (where (.*?))? ");

            string txtConn = Request["txtConn"];

            if (strSql.IndexOf("select") == 0)
            {
                string sql = @"  select top {0} * from 
                (
                select row_number() over(order by id) as rownumber,* from ({2}) _a
                ) A
                where rownumber > {1}";
                sql = string.Format(sql, req.PageCondition.PageSize, Request["start"], strSql);
                //



                string list3;
                string str;

                using (var db = new SqlSugar.SqlSugarClient(ConfigurationManager.ConnectionStrings[txtConn].ConnectionString))
                {
                    // list3 = db.SqlQueryDynamic(sql);

                    if (req.PageCondition.RowCount == 0)
                    {
                        req.PageCondition.RowCount = db.GetInt(string.Format("select count(*) from ({0}) a", strSql));
                    }


                    str = db.SqlQueryJson(sql);

                    list3 = Newtonsoft.Json.JsonConvert.SerializeObject(new
                    {
                        sEcho = Request["draw"],
                        iTotalRecords = req.PageCondition.RowCount,
                        iTotalDisplayRecords = req.PageCondition.RowCount,
                        aaData = "##"
                    });

                    str = list3.Replace("\"##\"", str);



                }


                return Content(str, "application/json");
            }
            else
            {
                //sql = strSql;
                // var rep = _BaseService.Repository.SqlExcute(strSql, 1);


                using (var db = new SqlSugar.SqlSugarClient(ConfigurationManager.ConnectionStrings[txtConn].ConnectionString))
                {
                    db.ExecuteCommand(strSql);

                }
                return SuccessNotification("");

            }

            //实现对用户和多条件的分页的查询，rows表示一共多少条，page表示当前第几页
            //  var list = GetListEx(predicate, req.PageCondition);// _newsService.GetList(null, null, 10, 10);

        }

        public ActionResult GetListFirst()
        {

            string strSql = Request["strSql"];
            //  var mq = System.Text.RegularExpressions.Regex.Matches(strSql, @"select (.*?) from (\w*) (where (.*?))? ");

            string sql = @"select top 1 *  from ( {0}) a";


            sql = string.Format(sql, strSql);
            // dynamic rep = _BaseService.Repository.UnitOfWork.SqlQuery<List<dynamic>>(sql, 1).ToList();

            string txtConn = Request["txtConn"];

            object list3;
            string str;
            List<object> aa;
            using (var db = new SqlSugar.SqlSugarClient(ConfigurationManager.ConnectionStrings[txtConn].ConnectionString))
            {

                str = db.SqlQueryJson(sql);

                // list3 = db.SqlQueryDynamic(sql);
                aa = Newtonsoft.Json.JsonConvert.DeserializeObject<List<object>>(str);

            }

            var dd = (aa[0].GetType()).GetProperties(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly)[0];
            var ddd = (Collection<JToken>)dd.GetValue(aa[0]);

            StringBuilder sb = new StringBuilder();
            foreach (var dddd in ddd)
            {
                dynamic da = dddd;
                sb.AppendFormat(",{0}", da.Name);
            }
            if (!string.IsNullOrEmpty(str))
            {
                return Content(sb.ToString().Substring(1));
            }
            else
            {
                return Content("");
            }




            //using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings[txtConn].ConnectionString))
            //{
            //    connection.Open();
            //     brand = connection.Query<dynamic>(sql).ToList();

            //}

            //return Content(str, "application/json");

            //return Json(list3, JsonRequestBehavior.AllowGet);
            // return Content(str);


            //实现对用户和多条件的分页的查询，rows表示一共多少条，page表示当前第几页
            //  var list = GetListEx(predicate, req.PageCondition);// _newsService.GetList(null, null, 10, 10);

        }

        public override ActionResult Get(string id)
        {
            CategoryView modelView = new CategoryView();

            if (!string.IsNullOrEmpty(Request["id"]) && Request["id"] != "0")
            {
                //根据courseId拿lst
                var model = _objService.Repository.Entities.Where(a => a.CategoryCode == id).FirstOrDefault();
                if (model != null)
                {
                    modelView.Id = model.Id;
                    modelView.CategoryName = model.CategoryName;
                    modelView.CategoryCode = model.CategoryCode;
                    modelView.AppId = model.AppId;
                }
                else
                {
                    ModelState.AddModelError("NotFind", T("This data is not find"));
                }

            }

            return Json(modelView, JsonRequestBehavior.AllowGet);

        }

        //Post方法
        public override JsonResult Post(CategoryView objModal, string Id)
        {
            //验证错误
            if (!BeforeAddOrUpdate(objModal, Id) || !ModelState.IsValid)
            {
                return Json(GetErrorJson(), JsonRequestBehavior.AllowGet);
            }

            if (string.IsNullOrEmpty(Id) || Id == "0")
            {
                _BaseService.InsertView(objModal);
            }
            else
            {
                _BaseService.UpdateView(objModal);
            }

            return Json(doJson(null), JsonRequestBehavior.AllowGet);
        }

        public override bool BeforeAddOrUpdate(CategoryView objModal, string Id)
        {
            bool validate = true;
            if (string.IsNullOrEmpty(objModal.CategoryName))
            {
                ModelState.AddModelError("titleEmpty", "标题不能为空");
                validate = false;
            }

            if (objModal.CategoryCode == null)
            {
                ModelState.AddModelError("codeEmpty", "CategoryCode不能为空");
                validate = false;
            }

            if (objModal.AppId == null)
            {
                ModelState.AddModelError("codeTypeEmpty", "CategoryTypeCode不能为空");
                validate = false;
            }
            return validate;
        }

        public override bool AfterDelete(string sIds)
        {
            // CommonService.ClearCache(1);
            return true;
        }
    }
}
