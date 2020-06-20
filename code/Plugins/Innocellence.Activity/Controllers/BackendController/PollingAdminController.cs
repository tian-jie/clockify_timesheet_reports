using EntityFramework.Extensions;
using Infrastructure.Utility.Data;
using Infrastructure.Utility.Filter;
using Infrastructure.Utility.IO;
using Infrastructure.Web.Domain.Service;
using Infrastructure.Web.UI;

using Innocellence.Activity.Contracts.Entity;
using Innocellence.Activity.Contracts.ViewModel;
using Innocellence.Activity.ModelsView;
using Innocellence.Activity.Service;
using Innocellence.Activity.Services;
using Innocellence.Authentication.Authentication;
using Innocellence.WeChat.Domain;
using Innocellence.WeChat.Domain.Contracts;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Innocellence.Activity.Admin.Controllers
{

    public partial class PollingAdminController : BaseController<PollingEntity, PollingView>
    {
        private readonly IPollingService _pollingService;
        private readonly PollingResultService _pollingResultService = new PollingResultService();
        private static readonly IDataPermissionCheck permission = new PollingPermissionService();
        private readonly IPollingAnswerService _pollingAnswerService;
        const string templateExcelFilename = "/plugins/Innocellence.Activity/content/PollingAcountingTemplate-Export.xlsx";

        public PollingAdminController(IPollingService pollingService, int appId, IPollingAnswerService pollingAnswerService)
            : base(pollingService)
        {
            _pollingService = pollingService;
            AppId = appId;
            ViewBag.AppId = AppId;
            _pollingAnswerService = pollingAnswerService;
        }

        public PollingAdminController(IPollingService pollingService, IPollingAnswerService pollingAnswerService)
            : base(pollingService)
        {
            _pollingService = pollingService;
            AppId = (int)CategoryType.Undefined;
            ViewBag.AppId = AppId;
            _pollingAnswerService = pollingAnswerService;
        }

        [HttpGet, DataSecurityFilter]
        public ActionResult Index(int appId)
        {
            ViewBag.AppId = appId;
            return View();
        }

        public ActionResult Create(int appId)
        {
            ViewBag.AppId = appId;
            AppId = appId;
            var objView = new PollingView();
            return View(objView);
        }

        [HttpGet, DataSecurityFilter]
        public ActionResult Edit(string id, int appId)
        {
            ViewBag.AppId = appId;
            AppId = appId;

            var objView = new PollingView();

            if (!string.IsNullOrEmpty(id) && id != "0")
            {
                var Id = int.Parse(id);
                objView = _pollingService.GetPollingView(Id);
            }

            if (!string.IsNullOrEmpty(id))
            {
                base.AppDataPermissionCheck(permission, appId, int.Parse(objView.AppId));
            }

            return View(objView);
        }

        public ActionResult Result(string id, int appId)
        {
            ViewBag.AppId = appId;
            AppId = appId;

            var objView = new PollingView();

            if (!string.IsNullOrEmpty(id) && id != "0")
            {
                var Id = int.Parse(id);

                objView = _pollingService.GetPersonCount(Id);
            }

            if (!string.IsNullOrEmpty(id))
            {
                base.AppDataPermissionCheck(permission, appId, int.Parse(objView.AppId));
            }

            return View(objView);
        }

        public ActionResult Screen(string id, int appId)
        {
            ViewBag.AppId = appId;
            AppId = appId;

            var objView = new PollingView();

            if (!string.IsNullOrEmpty(id) && id != "0")
            {
                var Id = int.Parse(id);
                objView = _pollingService.GetPollingView(Id);
            }

            if (!string.IsNullOrEmpty(id))
            {
                base.AppDataPermissionCheck(permission, appId, int.Parse(objView.AppId));
            }

            return View(objView);
        }

        public JsonResult GetSeriesData(string id, int appId, string questionid)
        {
            var screenView = new PollingScreenView();

            //根据qId去结果表里面算数据
            if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(questionid))
            {
                int pollingId = int.Parse(id);
                int quesId = int.Parse(questionid);
                screenView = _pollingService.GetPollingScreenData(pollingId, quesId);
            }

            return Json(screenView, JsonRequestBehavior.AllowGet);
        }


        //用户答卷
        public ActionResult CustomAnswer(string Id, int appId)
        {
            ViewBag.AppId = appId;
            ViewBag.pollingId = Id;
            AppId = appId;

            return View();
        }
        //用户答卷
        public ActionResult CustomScore(string Id, int appId)
        {
            ViewBag.AppId = appId;
            ViewBag.pollingId = Id;
            AppId = appId;

            return View();
        }

        public override ActionResult GetList()
        {

            var type = Request["Type"];
            var id = Request["Id"];
            var req = new GridRequest(Request);

            if (type == "Result")
            {
                var predicate = FilterHelper.GetExpression<PollingAnswerEntity>(req.FilterGroup);
                var profileList = GetListPrivate(predicate, req.PageCondition);

                return GetPageResult(profileList, req);
            }
            else if (type == "Score")
            {
                var predicate = FilterHelper.GetExpression<PollingAnswerEntity>(req.FilterGroup);
                var profileList = GetListPrivate1(predicate, req.PageCondition);

                return GetPageResult(profileList, req);
            }
            var predicate1 = FilterHelper.GetExpression<PollingEntity>(req.FilterGroup);
            return GetPageResult(GetListEx(predicate1, req.PageCondition), req);
        }

        protected List<PollingResultCustomView> GetListPrivate(Expression<Func<PollingAnswerEntity, bool>> predicate, PageCondition pageCondition)
        {
            var objView = new List<PollingResultCustomView>();
            predicate = rtnPredicate();
            objView = _pollingAnswerService.GetList<PollingResultCustomView>(predicate, pageCondition);

            return objView;
        }
        private Expression<Func<PollingAnswerEntity, bool>> rtnPredicate()
        {
            string pollInfo = Request["txtSearch"];
            var id = Request["Id"];
            var req = new GridRequest(Request);
            Expression<Func<PollingAnswerEntity, bool>> predicate = FilterHelper.GetExpression<PollingAnswerEntity>(req.FilterGroup);

            if (!string.IsNullOrEmpty(id) && id != "0")
            {
                int ID = int.Parse(id);
                predicate = predicate.AndAlso(a => a.PollingId == ID);

            }
            if (!string.IsNullOrEmpty(pollInfo))
            {
                predicate = predicate.AndAlso(x => x.Name.Contains(pollInfo) || x.LillyId.Contains(pollInfo));
            }
            return predicate.AndAlso(a => a.IsDeleted != true);
        }

        protected List<PollingResultCustomView> GetListPrivate1(Expression<Func<PollingAnswerEntity, bool>> predicate, PageCondition pageCondition)
        {
            var objView = new List<PollingResultCustomView>();
            predicate = rtnPredicate();
            objView = _pollingAnswerService.Query(predicate, pageCondition);
            return objView;

        }

        public override List<PollingView> GetListEx(Expression<Func<PollingEntity, bool>> predicate, PageCondition ConPage)
        {
            predicate = predicate.AndAlso(a => a.IsDeleted == false);
            string strTitle = Request["pollingName"];
            string startDate = Request["startDate"];
            string endDate = Request["endDate"];

            if (!string.IsNullOrEmpty(strTitle))
            {
                predicate = predicate.AndAlso(x => x.Name.Contains(strTitle));
            }

            if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
            {
                var startDateTime = DateTime.Parse(startDate);
                var endDateTime = DateTime.Parse(endDate);
                predicate = predicate.AndAlso(x => x.StartDateTime >= startDateTime && x.EndDateTime <= endDateTime);
            }

            return base.GetListEx(predicate, ConPage);
        }

        //BackEnd校验
        public override bool BeforeAddOrUpdate(PollingView objModal, string Id)
        {
            bool validate = true;
            StringBuilder errMsg = new StringBuilder();
            var startDate = objModal.StartDateTime.ToString();
            var endDate = objModal.EndDateTime.ToString();
            var now = DateTime.Now;

            if (objModal.Name == null)
            {
                validate = false;
                errMsg.Append(T("请输入名称.<br/>"));
            }

            if (string.IsNullOrWhiteSpace(objModal.Type.ToString()))
            {
                validate = false;
                errMsg.Append(T("请选择分类.<br/>"));
            }

            if (string.IsNullOrEmpty(startDate))
            {
                validate = false;
                errMsg.Append(T("请输入当前投票的开始时间.<br/>"));
            }

            if (string.IsNullOrEmpty(endDate))
            {
                validate = false;
                errMsg.Append(T("请输入当前投票的结束时间.<br/>"));
            }

            if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
            {
                if (objModal.StartDateTime > objModal.EndDateTime)
                {
                    validate = false;
                    errMsg.Append(T("开始时间不能大于结束时间.<br/>"));
                }

                if (objModal.StartDateTime >= objModal.EndDateTime.AddHours(-1))
                {
                    validate = false;
                    errMsg.Append(T("投票持续时间不能低于1小时.<br/>"));
                }

                if (objModal.EndDateTime <= now.AddHours(1))
                {
                    validate = false;
                    errMsg.Append(T("截止时间必须大于当前时间加1个小时.<br/>"));
                }
            }

            if (!validate)
            {
                ModelState.AddModelError("不正确的输入", errMsg.ToString());
            }

            return validate;
        }

        [HttpPost]
        public JsonResult UpdateTime(string Id, string endTime)
        {
            var objView = new PollingView();

            if (!string.IsNullOrEmpty(Id) && Id != "0")
            {
                var id = int.Parse(Id);
                objView = _pollingService.GetPollingView(id);
            }

            if (!string.IsNullOrEmpty(endTime))
            {
                objView.EndDateTime = DateTime.Parse(endTime);
            }

            //验证错误
            if (!BeforeAddOrUpdate(objView, Id) || !ModelState.IsValid)
            {
                return Json(GetErrorJson(), JsonRequestBehavior.AllowGet);
            }

            InsertOrUpdate(objView, Id);

            return Json(doJson(null), JsonRequestBehavior.AllowGet);
        }

        //Post方法
        [HttpPost]
        [ValidateInput(false)]
        public override JsonResult Post(PollingView objModal, string Id)
        {
            AppId = int.Parse(objModal.AppId);

            if (string.IsNullOrEmpty(Id) && objModal.Id != 0)
            {
                Id = objModal.Id.ToString();
            }

            //验证错误
            if (!BeforeAddOrUpdate(objModal, Id) || !ModelState.IsValid)
            {
                return Json(GetErrorJson(), JsonRequestBehavior.AllowGet);
            }

            InsertOrUpdate(objModal, Id);

            return Json(doJson(null), JsonRequestBehavior.AllowGet);
        }

        public virtual void InsertOrUpdate(PollingView objModal, string Id)
        {
            //objModal.AppId = AppId;
            if (string.IsNullOrEmpty(Id) || Id == "0")
            {
                _pollingService.InsertView(objModal);
            }
            else
            {
                _BaseService.UpdateView(objModal);
            }
        }

        [HttpGet]
        public JsonResult GetPollingList(string appId)
        {
            string type = Request["Type"];
            Expression<Func<PollingEntity, bool>> predicate = a => a.IsDeleted != true;
            DateTime current = DateTime.Now;

            predicate = predicate.AndAlso(x => x.EndDateTime > current);

            predicate = predicate.AndAlso(x => x.AppId == appId);

            if (!string.IsNullOrEmpty(type))
            {
                var typeId = int.Parse(type);
                predicate = predicate.AndAlso(x => x.Type == typeId);
            }

            var lst = _pollingService.QueryList(predicate);

            return Json(new { data = lst }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExportToExcel()
        {
            string id = Request["Id"];

            if (string.IsNullOrEmpty(id) || id == "0")
            {
                throw new InvalidOperationException("ID参数错误！");
            }

            var Id = int.Parse(id);
            var pollingView = _pollingService.GetPollingView(Id);


            var pollName = pollingView.Name ?? "";
            string fileName = pollName + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xlsx";

            string templateFilename = Server.MapPath(templateExcelFilename);
            using (FileStream file = new FileStream(templateFilename, FileMode.Open, FileAccess.Read))
            {
                var workbook = new XSSFWorkbook(file);
                var sheet1 = workbook.GetSheet("答卷");
                var sheet2 = workbook.GetSheet("正确答案");
                var sheet3 = workbook.GetSheet("得分");

                // 导出 答卷 
                //var answer = _pollingResultService.GetList(Id);
                var reportList1 = _pollingAnswerService.GetIsRight(rtnPredicate());
                int i = 1;
                foreach (var v in reportList1)
                {
                    int j = 0;
                    var row = sheet1.CreateRow(i++);
                    row.CreateCell(j++).SetCellValue(v.UserId);
                    row.CreateCell(j++).SetCellValue(v.UserName);
                    row.CreateCell(j++).SetCellValue(v.UserDeptLv1);
                    row.CreateCell(j++).SetCellValue(v.UserDeptLv2);
                    row.CreateCell(j++).SetCellValue(v.UserDeptLv3);
                    row.CreateCell(j++).SetCellValue(v.QuestionId);
                    row.CreateCell(j++).SetCellValue(v.QuestionTitle);
                    row.CreateCell(j++).SetCellValue(v.CreatedDate.ToString());
                    row.CreateCell(j++).SetCellValue(v.CustomAnswer);
                    row.CreateCell(j++).SetCellValue(v.CustomStatus);
                   
                }

                // 导出 正确答案 

                //var reportList2 = pollingView.PollingQuestions;
                var pol = _pollingService.GetPersonCount(Id);
                var reportList2 = pol.PollingQuestions;
                i = 1;
                foreach (var v in reportList2)
                {
                    int j = 0;
                    var row = sheet2.CreateRow(i++);
                    row.CreateCell(j++).SetCellValue(j);
                    row.CreateCell(j++).SetCellValue(v.Id);
                    row.CreateCell(j++).SetCellValue(v.Title);
                    row.CreateCell(j++).SetCellValue(v.RightAnswers);
                    row.CreateCell(j++).SetCellValue((double?)v.Score??0);
                    row.CreateCell(j++).SetCellValue(string.Format("{0}/{1}", v.rightPersons, v.answerPersons));
                }

                // 导出 得分 
                var reportList3 = _pollingAnswerService.GetPersonScore(rtnPredicate());
                i = 3;
                foreach (var v in reportList3)
                {
                    int j = 0;
                    var row = sheet3.CreateRow(i++);
                    row.CreateCell(j++).SetCellValue(v.UserId);
                    row.CreateCell(j++).SetCellValue(v.UserName);
                    row.CreateCell(j++).SetCellValue(v.UserDeptLv1);
                    row.CreateCell(j++).SetCellValue(v.UserDeptLv2);
                    row.CreateCell(j++).SetCellValue(v.UserDeptLv3);
                    row.CreateCell(j++).SetCellValue((double)v.CustomScore);
                }


                using (MemoryStream ms = new MemoryStream())
                {
                    workbook.Write(ms);
                    ms.Flush();
                    ms.Position = 0;
                    //sheet = null;
                    //workbook = null;
                    //workbook.Close();//一般只用写这一个就OK了，他会遍历并释放所有资源，但当前版本有问题所以只释放sheet
                    return File(ms.ToArray(), "application/vnd-excel", fileName);
                }

            }

        }


        public ActionResult ExportPollingResult()
        {
            string id = Request["Id"];
            var objView = new PollingView();
            Expression<Func<PollingResultEntity, bool>> predicate = x => x.IsDeleted == false;

            if (!string.IsNullOrEmpty(id) && id != "0")
            {
                var Id = int.Parse(id);
                predicate = predicate.AndAlso(x => x.PollingId == Id);
                objView = _pollingService.GetPollingView(Id);
            }

            List<PollingResultExportView> reportList = _pollingResultService.GetListExport<PollingResultExportView>(predicate).OrderByDescending(x => x.Id).ToList();
            _pollingResultService.GetUserList(reportList);
            return ExportToCsv(reportList, objView);
        }

        private ActionResult ExportToCsv(List<PollingResultExportView> lst, PollingView objView)
        {
            string[] headLine = { "Id", "UserId", "UserName", "QuestionName", "Answer", "AnswerText", "CreatedDate" };
            var csv = new CsvSerializer<PollingResultExportView> { UseLineNumbers = false };
            var sRet = csv.SerializeStream(lst, headLine);
            var pollName = objView.Name ?? "";
            string fileName = pollName + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".csv";
            return File(sRet, "text/comma-separated-values", fileName);
        }

        //public override ActionResult Export()
        //{
        //    string id = Request["Id"];

        //    var reportList = new List<PollingResultScoreView>();
        //    var objView = new PollingView();
        //    if (!string.IsNullOrEmpty(id) && id != "0")
        //    {
        //        var Id = int.Parse(id);
        //        var polling = _pollingService.GetPollingById(Id);
        //        reportList = _pollingService.PollingResultScore(Id,polling);
        //        objView = _pollingService.GetPollingView(x => x.Id == Id && x.IsDeleted == false);
        //    }

        //    return ExportToScoreCsv(reportList, objView);
        //}

        //private ActionResult ExportToScoreCsv(List<PollingResultScoreView> lst, PollingView objView)
        //{
        //    string[] headLine = { "UserId", "UserName", "UserDeptLv1", "UserDeptLv2", "UserDeptLv3", "CustomScore" };
        //    var csv = new CsvSerializer<PollingResultScoreView> { UseLineNumbers = false };
        //    var sRet = csv.SerializeStream(lst, headLine);
        //    var pollName = objView.Name ?? "";
        //    string fileName = pollName + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".csv";
        //    return File(sRet, "text/comma-separated-values", fileName);
        //}

        //public ActionResult ExportAnswer()
        //{
        //    string id = Request["Id"];
        //    var reportList = new List<PollingResultCustomView>();
        //    var objView = new PollingView();
        //    if (!string.IsNullOrEmpty(id) && id != "0")
        //    {
        //        var Id = int.Parse(id);
        //        var polling = _pollingService.GetPollingById(Id);
        //        reportList = _pollingService.PollingResult(Id,polling);
        //        objView = _pollingService.GetPollingView(x => x.Id == Id && x.IsDeleted == false);
        //    }

        //    return ExportToAnswerCsv(reportList, objView);
        //}

        //private ActionResult ExportToAnswerCsv(List<PollingResultCustomView> lst, PollingView objView)
        //{
        //    string[] headLine = { "UserId", "UserName", "UserDeptLv1", "UserDeptLv2", "UserDeptLv3", "QuestionId", "QuestionTitle", "CustomAnswer", "RightAnswers", "CustomStatus" };
        //    var csv = new CsvSerializer<PollingResultCustomView> { UseLineNumbers = false };
        //    var sRet = csv.SerializeStream(lst, headLine);
        //    var pollName = objView.Name ?? "";
        //    string fileName = pollName + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".csv";
        //    return File(sRet, "text/comma-separated-values", fileName);
        //}
        public ActionResult ProcessOldPollingData()
        {
            // 1. 读取所有的polling;
            // 1.1 读取所有的question，缓存
            // 2.1 每个polling跑一遍第一页的表格
            // 2.1.1 每行到question里查一下这个人多少分
            // 2.1.2 每行保存到csv里。


            var pollingQuestionService = new PollingQuestionService();



            var pageCondition = new PageCondition();
            pageCondition.PageSize = 100000000;

            var pollingService = _pollingService;

            var pollings = pollingService.GetList<PollingView>(a => a.Type == 1 && a.IsDeleted != true, pageCondition);

            int i = 0;
            var workbook = new XSSFWorkbook();
            var sheet = workbook.CreateSheet("答卷");
            // 1. 读取所有的polling;
            foreach (var polling in pollings)
            {

                var pv = pollingService.GetPollingView(polling.Id);

                // 1.1 读取所有的question
                var questions = pollingQuestionService.GetPollingQuestion(pv.Id);
                var results = _pollingResultService.GetList(pv.Id);
                var answersByQuestion = pollingService.PollingResult(pv.Id, results, pv);
                foreach (var answerbyQuestion in answersByQuestion)
                {
                    var question = questions.FirstOrDefault(a => a.Id == answerbyQuestion.QuestionId);
                    if (question == null)
                    {
                        continue;
                    }
                    answerbyQuestion.CustomScore = (answerbyQuestion.IsRight) ? (decimal)question.Score : 0;


                    // 导出 答卷 

                    var v = answerbyQuestion;
                    int j = 0;
                    var row = sheet.CreateRow(i++);
                    row.CreateCell(j++).SetCellValue(v.UserId);
                    row.CreateCell(j++).SetCellValue(v.UserName);
                    row.CreateCell(j++).SetCellValue(v.UserDeptLv1);
                    row.CreateCell(j++).SetCellValue(v.UserDeptLv2);
                    row.CreateCell(j++).SetCellValue(v.UserDeptLv3);
                    row.CreateCell(j++).SetCellValue(v.QuestionId);
                    row.CreateCell(j++).SetCellValue(v.QuestionTitle);
                    row.CreateCell(j++).SetCellValue(v.CustomAnswer);
                    row.CreateCell(j++).SetCellValue(v.CustomStatus);
                   
                }

                //using (FileStream fs = new FileStream("d:\\oldpolling\\"+pv.Id+".xlsx", FileMode.Create))
                //{
                //    workbook.Write(fs);
                //}

                //workbook.Close();
            }

            using (MemoryStream ms1 = new MemoryStream())
            {
                workbook.Write(ms1);
                ms1.Flush();
                ms1.Position = 0;
                //sheet = null;
                //workbook = null;
                //workbook.Close();//一般只用写这一个就OK了，他会遍历并释放所有资源，但当前版本有问题所以只释放sheet
                return File(ms1.ToArray(), "application/vnd-excel", "ProcessOldPolling.xlsx");
            }

        }

        public ActionResult RankingList(string code)
        {
            var cList = Ranking();
            if (cList == null)
            {
                return ErrorNotification("RankList字段值为空");
            }

            var ranks = cList.FirstOrDefault(x => x.Code == code);

            if (ranks == null)
            {
                return ErrorNotification("RankList中没有符合要求的Code数据");
            }
            var ids = ranks.Ids;
            var pollingIds = ids.Replace("，", ",").Split(',').ToList().ConvertAll(int.Parse);
            ViewBag.Title = ranks.Title;

            var data = _pollingService.GetSorting(pollingIds);

            string columnsIds = string.Empty;
            if (data != null && data.Count > 0)
            {
                int sumAll = 0;
                var columns = data[0].Sorts.Count;
                for (int i = 0; i < columns; i++)
                {
                    int sum = 0;

                    for (int j = 0; j < data.Count; j++)
                    {
                        sum += data[j].Sorts[i].GetValueOrDefault();
                        if (i == columns - 1)
                        {
                            sumAll += data[j].SortSum.GetValueOrDefault();
                        }
                    }
                    if (sum == 0)
                    {
                        columnsIds += i + ",";
                    }
                }
                if (sumAll == 0)
                {
                    for (int j = 0; j < data.Count; j++)
                    {
                        data[j].SortSum = null;
                    }
                }
                if (columnsIds.Length > 0)
                {
                    columnsIds = columnsIds.Substring(0, columnsIds.LastIndexOf(",")).Replace("\"", "");
                    var columns1 = columnsIds.Split(',').ToList().ConvertAll(int.Parse);
                    for (int k = 0; k < columns1.Count; k++)
                    {
                        var col = columns1[k];
                        for (int j = 0; j < data.Count; j++)
                        {
                            data[j].Sorts[col] = null;
                        }
                    }
                }
            }

            return View(data);
        }

        public JsonResult DisablePolling(int pollingId)
        {
            var currentDate = DateTime.Now;
            var counts = _pollingService.Repository.Entities.Where(x => x.Id == pollingId).Update(x => new PollingEntity { EndDateTime = currentDate });

            object result;
            if (counts > 0)
            {
                result = new { Status = 200, Message = "操作成功" };
            }
            else
            {
                result = new { Status = 99, Message = "您操作的数据不存在" };
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 从sysconfig中取Ranklist值
        /// </summary>
        /// <returns></returns>
        public List<RankList> Ranking()
        {
            var js = new JavaScriptSerializer();
            var thisList = js.Deserialize<List<RankList>>(CommonService.GetSysConfig("RankList", ""));
            return thisList;
        }
        public class RankList
        {
            public string Code { get; set; }
            public string Title { get; set; }
            public string Ids { get; set; }
        }

        [DataSecurityFilter,HttpPost]
        public JsonResult ClonePolling(int pollingId, int appId)
        {
            if (pollingId == 0)
            {
                return Json(new { Status = 99, Message = "传入的 pollingid 不能为0!" });
            }

            var newId = _pollingService.ClonePolling(pollingId);

            if (newId == 0)
            {
                return Json(new { Status = 99, Message = "操作的polling 不存在，或已被删除!" });
            }

            return Json(new { Status = 200, Data = newId });
        }
    }
}