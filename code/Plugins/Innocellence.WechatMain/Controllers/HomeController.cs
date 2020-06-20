using Autofac;
using Infrastructure.Core.Caching;
using Infrastructure.Core.Infrastructure;
using Infrastructure.Utility.IO;
using Infrastructure.Web.MVC.Attribute;
using Innocellence.WeChat.Domain;
using Innocellence.WeChat.Domain.Common;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Contracts.ViewModel;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.ModelsView;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace Innocellence.WeChatMain.Controllers
{
     [AllowLoginUserAttribute]
    public class HomeController :  BaseController<QrCodeMPItem, QrCodeView>
    {
        private static ICacheManager cacheManager = EngineContext.Current.Resolve<ICacheManager>(new TypedParameter(typeof(Type), typeof(WeChatCommonService)));
        public HomeController(IQrCodeService objectService)
            : base(objectService)
        {
        }

       
        public override ActionResult Index()
        {
            int AccountManageid = int.Parse(Request["AccountManageid"]);
            var model = Innocellence.WeChat.Domain.Common.WeChatCommonService.lstSysWeChatConfig.Where(a => a.AccountManageId == AccountManageid).FirstOrDefault();
            ViewBag.IsCorp = model.IsCorp.Value;
            ViewBag.Appid = model.Id;
            return  View();
        }

        //public ActionResult About()
        //{
        //    ViewBag.Message = "Your application description page.";

        //    return View();
        //}

        //public ActionResult Contact()
        //{
        //    ViewBag.Message = "Your contact page.";

        //    return View();
        //}
        #region MP subscribe report

        [AllowAnonymous]
        public JsonResult GetMpReportList(int appid, DateTime? BeginDate,DateTime? EndDate)
        {

            
            DateTime thisEnd = DateTime.Now;
            DateTime thisStart = DateTime.Now.AddDays(-30);
            try
            {
                if (BeginDate != null && EndDate != null) {
                     thisStart = (DateTime)BeginDate;
                     thisEnd = (DateTime)EndDate;       
                }
                int totalSub = 0;    
                var dataTemp = GetSqlResultCount(appid, thisStart, thisEnd, out totalSub);
                return Json(new {data = dataTemp,total = totalSub }, "200", JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                _Logger.Error(e, "An error occurred while sending news.");
                //throw;
                return Json(new { results = new { Data = 500 } });
            }
            
        }
        
        //private static List<ReportContent> CacheReportList(int appid)
        //{

        //        var lst = cacheManager.Get<List<ReportContent>>("ReportMonthList" + appid, () =>
        //        {
        //            DateTime thisEnd = DateTime.Now;
        //            DateTime thisStart = DateTime.Now.AddDays(-30);
        //            return GetSqlResultCount(appid,thisStart,thisEnd);
        //        });

        //        return lst;
        //}

        private List<ReportContent> GetSqlResultCount (int appid, DateTime start, DateTime end, out int totalSubscribe)
        {
            List<ReportContent> totalCountTableData = new List<ReportContent>();
            using (SqlConnection sqlconn = new SqlConnection(WebConfigurationManager.ConnectionStrings["CAAdmin"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand();

                cmd.Connection = sqlconn;
                cmd.CommandText = "SubscribeReport";
                cmd.CommandType = CommandType.StoredProcedure;


                IDataParameter[] parameters =
                    {
                        new SqlParameter("@AppId", SqlDbType.VarChar,100) ,
                        new SqlParameter("@StartDate", SqlDbType.VarChar,100),
                        new SqlParameter("@EndDate", SqlDbType.VarChar,100),
                     };
                parameters[0].Value = System.Convert.ToString(appid);
                parameters[1].Value = start;
                parameters[2].Value = end;
                cmd.Parameters.Add(parameters[0]);
                cmd.Parameters.Add(parameters[1]);
                cmd.Parameters.Add(parameters[2]);

                using (DataSet ds = new DataSet())
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                    adapter.Fill(ds);

                    //<MpFollowReport>
                    totalCountTableData = TableToList(ds.Tables[0]);
                    totalSubscribe = (int)ds.Tables[1].Rows[0].ItemArray[0] - (int)ds.Tables[2].Rows[0].ItemArray[0];

                }

            }
            return totalCountTableData;
        }

        private List<ReportContent> TableToList(DataTable table)
        {
            List<ReportContent> list = new List<ReportContent>();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                list.Add(RowToModel(table.Rows[i]));
            }
            return list;
        }
        private ReportContent RowToModel(DataRow row)
        {
            ReportContent model = new ReportContent();
            model.TimeInReport = row["CountDayTime"].ToString();
            model.Subscribe = row["SubscribeNum"].ToString();
            model.SubscribeWithScan = row["SubscribeWithScanNum"].ToString();            
            model.UnSubscribe = row["UnSubscribeNum"].ToString();

            return model;
        }
        

        /// <summary>
        /// 导出报表
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="appCate"></param>
        /// <returns></returns>
        public ActionResult MpExportCSV(int appid, DateTime? BeginDate, DateTime? EndDate)
        {
            DateTime thisEnd = DateTime.Now;
            DateTime thisStart = DateTime.Now.AddDays(-30);
            try
            {
                if (BeginDate != null && EndDate != null)
                {
                    thisStart = (DateTime)BeginDate;
                    thisEnd = (DateTime)EndDate;
                }
                int totalSub = 0;
                var reportList = GetSqlResultCount(appid, thisStart, thisEnd, out totalSub);

                return mpExportToCSV(reportList);
            }
            catch (Exception e)
            {
                _Logger.Error(e, "An error occurred while sending news.");
                //throw;
                return Json(new { results = new { Data = 500 } });
            }

            

        }
        private ActionResult mpExportToCSV(List<ReportContent> mpReportList)
        {
            string[] headLine = { "TimeInReport", "Subscribe", "SubscribeWithScan", "UnSubscribe" };
            var lst = mpReportList;
            CsvSerializer<ReportContent> csv = new CsvSerializer<ReportContent>();
            csv.UseLineNumbers = false;
            var ms = csv.SerializeStream(lst, headLine);

            return File(ms, "text/plain", string.Format("{0}.csv", "appAccessReport_" + DateTime.Now.ToString("yyyMMddHHmmss")));

        }
        #endregion

        #region QY subscribe report

        public JsonResult GetQyReportList()
        {
           
            return Json(new { data = GetQyHomeReport() }, "200", JsonRequestBehavior.AllowGet); 

        }

        private QyReportContent GetQyHomeReport() {
            QyReportContent QyIndexReport = new QyReportContent();
            using (SqlConnection sqlconn = new SqlConnection(WebConfigurationManager.ConnectionStrings["CAAdmin"].ConnectionString))
            {
                DateTime start = new DateTime(System.DateTime.Now.Year, System.DateTime.Now.Month, 1);
                DateTime end = System.DateTime.Now;
                SqlCommand cmd = new SqlCommand();

                cmd.Connection = sqlconn;
                cmd.CommandText = "CropIndexReport";
                cmd.CommandType = CommandType.StoredProcedure;


                IDataParameter[] parameters =
                    {
                        new SqlParameter("@StartDate", SqlDbType.VarChar,100),
                        new SqlParameter("@EndDate", SqlDbType.VarChar,100),
                     };
                parameters[0].Value = start;
                parameters[1].Value = end.AddDays(1);
                cmd.Parameters.Add(parameters[0]);
                cmd.Parameters.Add(parameters[1]);

                using (DataSet ds = new DataSet())
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                    adapter.Fill(ds);

                    //<MpFollowReport>
                    QyIndexReport.TotalMember = (int)ds.Tables[0].Rows[0].ItemArray[0];
                    QyIndexReport.SubMember = (int)ds.Tables[0].Rows[0].ItemArray[1];
                    QyIndexReport.SubMemberThisMonth = (int)ds.Tables[0].Rows[0].ItemArray[2];
                    QyIndexReport.UnSubMember = (int)ds.Tables[0].Rows[0].ItemArray[3];
                    QyIndexReport.AddThisMonth = (int)ds.Tables[1].Rows[0].ItemArray[0];
                    QyIndexReport.AddAppThisMonth = (int)ds.Tables[2].Rows[0].ItemArray[0];
                    QyIndexReport.TotalApp = (int)ds.Tables[2].Rows[0].ItemArray[1];
                    QyIndexReport.MsgThisMonth = (int)ds.Tables[3].Rows[0].ItemArray[0];
                    QyIndexReport.TotalMsg = (int)ds.Tables[3].Rows[0].ItemArray[1];
                    QyIndexReport.InteractThisMonth = (int)ds.Tables[3].Rows[0].ItemArray[2];
                    QyIndexReport.TotalInteract = (int)ds.Tables[3].Rows[0].ItemArray[3];
                    QyIndexReport.ReadCountThisMonth = (int)ds.Tables[4].Rows[0].ItemArray[0];
                    //QyIndexReport.AverageMonth = (int)ds.Tables[4].Rows[0].ItemArray[0] / (int)ds.Tables[4].Rows[0].ItemArray[1];
                    QyIndexReport.ReadCountTotal = (int)ds.Tables[4].Rows[0].ItemArray[2];

                }

            }
            return QyIndexReport;

        }  

        #endregion

    }


    public class ReportContent
    {
        [Description("日期")]
        public string TimeInReport { get; set; }
        [Description("普通关注")]
        public string Subscribe { get; set; }
        [Description("扫码关注")]
        public string SubscribeWithScan { get; set; }
        [Description("取消关注")]
        public string UnSubscribe { get; set; }
    }

    public class QyReportContent
    {
        [Description("总员工数")]
        public int TotalMember { get; set; }
        [Description("累计关注人数")]
        public int SubMember { get; set; }
        [Description("本月新增关注")]
        public int SubMemberThisMonth { get; set; }
        [Description("未关注")]
        public int UnSubMember { get; set; }
        [Description("本月新增")]
        public int AddThisMonth { get; set; }
        [Description("本月新增应用")]
        public int AddAppThisMonth { get; set; }
        [Description("总应用数")]
        public int TotalApp { get; set; }
        [Description("本月条数")]
        public int MsgThisMonth { get; set; }
        [Description("总条数")]
        public int TotalMsg { get; set; }
        [Description("本月互动")]
        public int InteractThisMonth { get; set; }
        [Description("累计互动")]
        public int TotalInteract { get; set; }
        [Description("本月阅读人数")]
        public int ReadCountThisMonth { get; set; }
        [Description("平均月阅读人数")]
        public int AverageMonth { get; set; }
        [Description("总阅读人数")]
        public int ReadCountTotal { get; set; }
    }
}