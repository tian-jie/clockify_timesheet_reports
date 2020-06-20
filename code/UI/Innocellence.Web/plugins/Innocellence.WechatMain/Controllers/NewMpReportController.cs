using System;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Data;
using System.Web.Configuration;
using Innocellence.WeChat.Domain.Entity;
using System.Collections;
using System.Collections.Generic;
using Innocellence.WeChat.Domain.Common;
using Newtonsoft.Json;

namespace Innocellence.WeChatMain.Controllers
{
    public class NewMpReportController : Controller
    {

        private Dictionary<int, string> dic = new Dictionary<int, string> { };
        public NewMpReportController()
        {

        }

        [AllowAnonymous]
        public ActionResult MpReport(int appid)
        {
            ViewBag.AppId = appid;
            return View();
        }

        /// <summary>
        /// ajax 获取报表信息
        /// </summary>
        /// <param name="appid"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageCount"></param>
        /// <returns></returns>
        public JsonResult GetMpReportList(int appid, DateTime? BeginDate, DateTime? EndDate, int start = 0, int length = 10)
        {
            int totalCount = 0;
            DateTime thisEnd = DateTime.Now;
            DateTime thisStart = DateTime.Now.AddDays(-30);
            if (BeginDate != null && EndDate != null)
            {
                thisStart = (DateTime)BeginDate;
                thisEnd = (DateTime)EndDate;
            }
            var data = GetSqlResult(appid, thisStart, thisEnd, start, length, out totalCount);

            return Json(new
            {
                sEcho = Request["draw"],
                iTotalRecords = totalCount,
                iTotalDisplayRecords = totalCount,
                aaData = data
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 调用存储过程
        /// </summary>
        /// <param name="appid"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageCount"></param>
        /// <returns></returns>
        private List<MpFollowReport> GetSqlResult(int appid, DateTime start, DateTime end, int pageIndex, int pageCount,out int totalCount)
        {
            List<MpFollowReport> tableData = new List<MpFollowReport>();

            using (SqlConnection sqlconn = new SqlConnection(WebConfigurationManager.ConnectionStrings["CAAdmin"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand();

                cmd.Connection = sqlconn;
                cmd.CommandText = "MpActivityReport";
                cmd.CommandType = CommandType.StoredProcedure;


                IDataParameter[] parameters =
                    {
                        new SqlParameter("@AppId", SqlDbType.Int) ,
                        new SqlParameter("@ContentId", SqlDbType.VarChar,100),
                        new SqlParameter("@StartDate", SqlDbType.Date),
                        new SqlParameter("@EndDate", SqlDbType.Date),
                        new SqlParameter("@StartCount", SqlDbType.Int),
                        new SqlParameter("@PageCount", SqlDbType.Int)
                     };
                parameters[0].Value = appid;
                parameters[1].Value = "17,18";
                parameters[2].Value = start;
                parameters[3].Value = end;
                parameters[4].Value = pageIndex;
                parameters[5].Value = pageCount;
                cmd.Parameters.Add(parameters[0]);
                cmd.Parameters.Add(parameters[1]);
                cmd.Parameters.Add(parameters[2]);
                cmd.Parameters.Add(parameters[3]);
                cmd.Parameters.Add(parameters[4]);
                cmd.Parameters.Add(parameters[5]);

                using (DataSet ds = new DataSet())
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                    adapter.Fill(ds);

                    //<MpFollowReport>

                    tableData = TableToList(ds.Tables[0]);
                    totalCount = (int)ds.Tables[1].Rows[0].ItemArray[0];
                    
                }

            }
            return tableData;
        }
        private List<MpFollowReport> TableToList(DataTable table)
        {
            List<MpFollowReport> list = new List<MpFollowReport>();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                var newModel = RowToModel(table.Rows[i]);
                newModel.Id = i + 1;
                list.Add(newModel);
            }
            return list;
        }
        private MpFollowReport RowToModel(DataRow row)
        {
            MpFollowReport model = new MpFollowReport();
            //model.Id = row["UserId"].ToString();
            model.UserName = row["UserName"].ToString();

            model.Content = row["Content"].ToString();
            WechatUserMessageLogContentType newtype;
            if (Enum.TryParse(row["ContentType"].ToString(), out newtype))
            {
                switch (newtype)
                {
                    //扫码关注
                    case WechatUserMessageLogContentType.Request_Event_Subscribe_With_Scan:
                        model.ContentType = WechatUserMessageLogContentType.Request_Event_Subscribe_With_Scan.GetDescriptionByName();
                        model.ExtendObject = JsonConvert.DeserializeObject(row["Content"].ToString());

                        break;
                    //取消关注
                    case WechatUserMessageLogContentType.Request_Event_UnSubscribe:
                        model.ContentType = WechatUserMessageLogContentType.Request_Event_UnSubscribe.GetDescriptionByName();
                        break;
                    default:
                        break;
                }
            }
            model.CreatedTime = row["CreatedTime"].ToString();
            model.CustomerNO = row["CustomerNO"].ToString();
            model.CustomerRegisteredTime = row["CustomerRegisteredTime"].ToString();

            return model;
        }
        private JsonResult GetResultJason(int status, object data, string message)
        {
            return new JsonResult { Data = new { Status = status, Data = data, Message = message }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }


    }
}
