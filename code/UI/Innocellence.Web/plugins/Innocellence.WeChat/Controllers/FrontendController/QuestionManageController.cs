using Infrastructure.Core.Logging;
using Infrastructure.Web.Domain.Service;
using Infrastructure.Web.Net.Mail;
using Innocellence.WeChat.Domain;
using Innocellence.WeChat.Domain.Common;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.ModelsView;
using Innocellence.WeChat.Domain.Service;
using Innocellence.WeChat.Domain.Services;
using Innocellence.Weixin.QY.AdvancedAPIs;
using Innocellence.Weixin.QY.CommonAPIs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Innocellence.WeChat.Controllers
{
    public partial class QuestionManageController : WeChatBaseController<QuestionManage, QuestionManageView>
    {
        public IQuestionManageService _objService;

        public IQuestionImagesService _objImageService = new QuestionImagesService();
        private IUserInfoService _objUserService=new UserInfoService();
        public QuestionManageController(IQuestionManageService objService)
            : base(objService)
        {

            _objService = objService;
            AppId = (int)Infrastructure.Web.Domain.Service.CategoryType.Undefined;
        }


        //我要提问

        public ActionResult RaiseQuestion()
        {
            string strAppId = AppId.ToString();
            string QUserId = ViewBag.WeChatUserID;
            if (string.IsNullOrEmpty(QUserId))
            {
                return Redirect("/notauthed.html");
            }
            if (string.IsNullOrEmpty(strAppId)  ||
             !int.TryParse(strAppId, out AppId))
            {
                return Redirect("/notauthed.html");
            }

            int appId = int.Parse(strAppId);
            //记录用户行为
            ExecuteBehavior(appId, 7, "SERVICE_ONLINE");
            QuestionManageView ResultViewModel = new QuestionManageView();
            var lst = _objService.GetListByQUserId<QuestionManageView>(appId, QUserId, null);
            if (lst != null)
            {
                ResultViewModel.List = lst;
            }
            else
            {
                ResultViewModel.List = new List<QuestionManageView>();
            }
            ViewBag.appid = AppId;
            return View("../QuestionManage/RaiseQuestion", ResultViewModel);
        }


        [HttpPost]
        public ActionResult Create(QuestionManageView objModal, string Id)
        {
            if (!BeforeAddOrUpdate(objModal, Id) || !ModelState.IsValid)
            {
                return Json(GetErrorJson(), JsonRequestBehavior.AllowGet);
            }

            objModal.Status = "未回答";
            objModal.QUserName = ViewBag.WeChatUserID;
            InsertOrUpdate(objModal, Id);
            SendMailForApply(objModal.Id, objModal.Question);

            return Json(new { rtnId = objModal.Id, str = "Insert Success." }, JsonRequestBehavior.AllowGet);

        }

        protected void InsertOrUpdate(QuestionManageView objModal, string Id)
        {
            if (string.IsNullOrEmpty(Id) || Id == "0")
            {
                _BaseService.InsertView(objModal);
            }
            else
            {
                var lst = new List<string>()
                {
                    "Question",
                    "Answer"
                };
                _BaseService.UpdateView(objModal, lst);
            }
        }

        public ActionResult QuestionDetail(int id)
        {
            var question = _objService.Repository.GetByKey(id);
            List<QuestionImagesView> lst = _objImageService.GetListByQuestionID<QuestionImagesView>(id);

            if (question == null)
            {
                throw new FileNotFoundException();
            }

            question.ReadCount++;
            _objService.Repository.Update(question, new List<string>() { "ReadCount" });
            //记录用户行为
            ExecuteBehavior(question.AppId.Value, 3, "", id.ToString());
            var questionView = (QuestionManageView)(new QuestionManageView().ConvertAPIModel(question));
            questionView.QuestionImages = lst;
            return View(questionView);
        }

        public JsonResult Satisfied(int id, int? satisfaction)
        {

            var question = _objService.Repository.GetByKey(id);


            if (question == null)
            {
                throw new FileNotFoundException();
            }
            question.Satisfaction = satisfaction;
            _objService.Repository.Update(question, new List<string>() { "Satisfaction" });
            //记录用户行
            //ExecuteBehavior(question.AppId.Value, 3, "", id.ToString());
            return new JsonResult() { Data = "Success" };
        }
        //重写发邮件方法
        public bool SendMailForApply(int id, string question)
        {
            var baseUrl = CommonService.GetSysConfig("SSO Server", "");
            //            string jsonString = @"{""EmailReceiver"":""summer.zhang@innocellence.com"",""EnableSsl"":""true"",""EmailHost"":""smtp.office365.com"",""EmailPassword"":""Summer861108"",
            //""EmailUserName"":""summer.zhang@innocellence.com"",""EmailPort"":""587"",""EmailSender"":""summer.zhang@innocellence.com"",""EmailEnable"":""true"",
            //""EmailTitle"":""微信提问提醒 Wechat Request Reminder"",""EmailTemplate"":""<br>员工服务专员：  <br>Dear ESR，  <br>微信中有员工已提问，请尽快回复。谢谢！  <br>We have received EE's request in Wechat.Please kindly reply a.s.a.p. Thank you!  <br>Question: #Question#  <br>XXX ID: #WeChatUserID#  <br>Email: #Email#  <br>Attachment:<br>#Attachment#""}";
        
            var jsonString = CommonService.GetSysConfig("HREmail", "");
            var request = string.Format("<a href=\"{0}/CAAdmin/QuestionManage/index?AppId={1}\">问题链接</a>\n", baseUrl, 17);
            return SendMail(id, question, jsonString, request, baseUrl,""); ;
        }

        public ActionResult RaiseTicket(int AppId, string category)
        {
            string strAppId = AppId.ToString();
            string QUserId = ViewBag.WeChatUserID;

            if (string.IsNullOrEmpty(QUserId))
            {
                return Redirect("/notauthed.html");
            }

            if (string.IsNullOrEmpty(strAppId) || string.IsNullOrEmpty(category) ||
                !int.TryParse(strAppId, out AppId))
            {
                return Redirect("/notauthed.html");
            }
            int appId = int.Parse(strAppId);
            //记录用户行为
            ExecuteBehavior(appId, 7, category);
            QuestionManageView ResultViewModel = new QuestionManageView();
            var lst = _objService.GetListByQUserId<QuestionManageView>(appId, QUserId, category);
            if (lst != null)
            {
                ResultViewModel.List = lst;
            }
            else
            {
                ResultViewModel.List = new List<QuestionManageView>();
            }
            var user = _objUserService.GetByWeChatUserID(QUserId);
            if (user != null)
            {
                ViewBag.Tel = user.Tel;
            }
            ViewBag.appid = AppId;
            ViewBag.category = category;
            return View("../QuestionManage/RaiseTicket", ResultViewModel);
        }
        public ActionResult CreateTicket(QuestionManageView objModal, string Id)
        {
            if (!BeforeAddOrUpdate(objModal, Id) || !ModelState.IsValid)
            {
                return Json(GetErrorJson(), JsonRequestBehavior.AllowGet);
            }

            objModal.Status = "未开单";
            objModal.QUserName = ViewBag.WeChatUserID;
            InsertOrUpdate(objModal, Id);
            string QUserId = ViewBag.WeChatUserID;
            var userModel = _objUserService.GetByWeChatUserID(QUserId);
            if (userModel != null)
            {
                userModel.Tel = objModal.Tel;
                _objUserService.Repository.Update(userModel, new List<string>() { "Tel" });
            }
            else
            {
                _objUserService.Repository.Insert(
              new UserInfo { WeChatUserID = QUserId, Tel = objModal.Tel });
            }

            SendTicketMailForApply(objModal.Id, objModal.Question, objModal.AppId.ToString(), objModal.Category,objModal.Id);

            return Json(new { rtnId = objModal.Id, str = "Insert Success." }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult TicketDetail(int id)
        {

            var question = _objService.Repository.GetByKey(id);
            List<QuestionImagesView> lst = _objImageService.GetListByQuestionID<QuestionImagesView>(id);

            if (question == null)
            {
                throw new FileNotFoundException();
            }

            question.ReadCount++;
            _objService.Repository.Update(question, new List<string>() { "ReadCount" });
            //记录用户行为
            ExecuteBehavior(question.AppId.Value, 3, "", id.ToString());
            var questionView = (QuestionManageView)(new QuestionManageView().ConvertAPIModel(question));
            questionView.QuestionImages = lst;
            return View(questionView);
        }
        public bool SendTicketMailForApply(int id, string question, string Appid, string category,int questionId)
        {
            //            string jsonString = @"{""EmailReceiver"":""summer.zhang@innocellence.com"",""EnableSsl"":""true"",""EmailHost"":""smtp.office365.com"",""EmailPassword"":""Summer861108"",
            //""EmailUserName"":""summer.zhang@innocellence.com"",""EmailPort"":""587"",""EmailSender"":""summer.zhang@innocellence.com"",""EmailEnable"":""true"",
            //""EmailTitle"":""微信提问提醒 Wechat Request Reminder"",""EmailTemplate"":""<br>员工服务专员：  <br>Dear ESR，  <br>微信中有员工已提问，请尽快回复。谢谢！  <br>We have received EE's request in Wechat.Please kindly reply a.s.a.p. Thank you!  <br>Question: #Question#  <br>XXX ID: #WeChatUserID#  <br>Email: #Email#  <br>Attachment:<br>#Attachment#""}";
            
            var baseUrl = CommonService.GetSysConfig("SSO Server", "");
            var jsonString = CommonService.GetSysConfig("ITEmail", "");
            var request = string.Format("<a href=\"{0}/CAAdmin/QuestionManage/TicketIndex?AppId={1}&category={2}&QuestionId={3}\">提交问题链接</a>\n", baseUrl, Appid, category, questionId);
           
            return SendMail(id,question,jsonString,request,baseUrl,"EmailTitle");
        }

        public ActionResult RaiseFinance(int AppId, string category)
        {
            string strAppId = AppId.ToString();
            string QUserId = ViewBag.WeChatUserID;

            if (string.IsNullOrEmpty(QUserId))
            {
                return Redirect("/notauthed.html");
            }

            if (string.IsNullOrEmpty(strAppId) || string.IsNullOrEmpty(category) ||
                !int.TryParse(strAppId, out AppId))
            {
                return Redirect("/notauthed.html");
            }
            int appId = int.Parse(strAppId);
            //记录用户行为
            ExecuteBehavior(appId, 7, category);
            QuestionManageView ResultViewModel = new QuestionManageView();
            var lst = _objService.GetListByQUserId<QuestionManageView>(appId, QUserId, category);
            if (lst != null)
            {
                ResultViewModel.List = lst;
            }
            else
            {
                ResultViewModel.List = new List<QuestionManageView>();
            }
           
            ViewBag.appid = AppId;
            ViewBag.category = category;
            return View(ResultViewModel);
        }
        public ActionResult CreateFinance(QuestionManageView objModal, string Id)
        {
            if (!BeforeAddOrUpdate(objModal, Id) || !ModelState.IsValid)
            {
                return Json(GetErrorJson(), JsonRequestBehavior.AllowGet);
            }

            objModal.Status = "未回答";
           objModal.QUserName = ViewBag.WeChatUserID;
            InsertOrUpdate(objModal, Id);

            SendFinanceMailForApply(objModal.Id, objModal.Question, objModal.AppId.ToString(), objModal.Category);

            return Json(new { rtnId = objModal.Id, str = "Insert Success." }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult FinanceDetail(int id)
        {

            var question = _objService.Repository.GetByKey(id);
            List<QuestionImagesView> lst = _objImageService.GetListByQuestionID<QuestionImagesView>(id);

            if (question == null)
            {
                throw new FileNotFoundException();
            }

            question.ReadCount++;
            _objService.Repository.Update(question, new List<string>() { "ReadCount" });
            //记录用户行为
            ExecuteBehavior(question.AppId.Value, 3, "", id.ToString());
            var questionView = (QuestionManageView)(new QuestionManageView().ConvertAPIModel(question));
            questionView.QuestionImages = lst;
            return View(questionView);
        }
        public bool SendFinanceMailForApply(int id, string question, string Appid, string category)
        {
            //            string jsonString = @"{""EmailReceiver"":""summer.zhang@innocellence.com"",""EnableSsl"":""true"",""EmailHost"":""smtp.office365.com"",""EmailPassword"":""Summer861108"",
            //""EmailUserName"":""summer.zhang@innocellence.com"",""EmailPort"":""587"",""EmailSender"":""summer.zhang@innocellence.com"",""EmailEnable"":""true"",
            //""EmailTitle"":""微信提问提醒 Wechat Request Reminder"",""EmailTemplate"":""<br>员工服务专员：  <br>Dear ESR，  <br>微信中有员工已提问，请尽快回复。谢谢！  <br>We have received EE's request in Wechat.Please kindly reply a.s.a.p. Thank you!  <br>Question: #Question#  <br>XXX ID: #WeChatUserID#  <br>Email: #Email#  <br>Attachment:<br>#Attachment#""}";

            var baseUrl = CommonService.GetSysConfig("SSO Server", "");
            var jsonString = CommonService.GetSysConfig("WechatEmail", "");
            var request = string.Format("<a href=\"{0}/CAAdmin/QuestionManage/FinanceIndex?AppId={1}&category={2}\">提交问题链接</a>\n", baseUrl, Appid, category);

            return SendMail(id, question, jsonString, request, baseUrl, "");
        }
        public bool SendMail(int id, string question, string jsonString, string request,string baseUrl,string title)
        {

            //获取收件人信息 
            var receiver = string.Empty; //For local Testing "summer.zhang@innocellence.com"
            var objConfig = WeChatCommonService.GetWeChatConfigByID(AppId);

            string strToken = AccessTokenContainer.TryGetToken(objConfig.WeixinCorpId, objConfig.WeixinCorpSecret);
            var mobile = string.Empty;
#if !DEBUG
            var obj = MailListApi.GetMember(strToken, ViewBag.WeChatUserID);
            if (obj != null)
            {
                receiver = obj.email;
            }
            else
            {
                receiver = "";
            }
#endif
            var EmailReceiver = "";
            var EmailEnableSsl = false;
            var EmailHost = "";
            var EmailPassword = "";
            var EmailUserName = "";
            var EmailPort = "";
            var EmailSender = "";
            var EmailEnable = true;
            var EmailTitle = "";
            var EmailTemplate = "";
           
            object jsonResult = null;
           
            LogManager.GetLogger(this.GetType()).Debug("request" + request);
            JavaScriptSerializer js = new JavaScriptSerializer();
            jsonResult = js.Deserialize<object>(jsonString);
            var fullResult = jsonResult as Dictionary<string, object>;

            if (fullResult != null)
            {
                EmailReceiver = fullResult["EmailReceiver"] as string;
                EmailEnableSsl = bool.Parse(fullResult["EmailEnableSsl"].ToString());
                EmailHost = fullResult["EmailHost"] as string;
                EmailPassword = fullResult["EmailPassword"] as string;
                EmailUserName = fullResult["EmailUserName"] as string;
                EmailPort = fullResult["EmailPort"] as string;
                EmailSender = fullResult["EmailSender"] as string;
                EmailEnable = bool.Parse(fullResult["EmailEnable"].ToString());
                EmailTitle = fullResult["EmailTitle"] as string;
                EmailTemplate = fullResult["EmailTemplate"] as string;
            }

            if (!string.IsNullOrEmpty(EmailTitle))
            {
                //特殊处理EmailTitle
                if (!string.IsNullOrEmpty(title))
                {
                    EmailTitle = string.Format("{0} ({1},{2})", EmailTitle, ViewBag.WeChatUserID, DateTime.Now.ToShortDateString());
                }
                EmailTitle = EmailTitle.Replace("\r", "").Replace("\n", "");
            }
            List<QuestionImagesView> lst = _objImageService.GetListByQuestionID<QuestionImagesView>(id);
            var attachlink = "";

            if (lst != null || lst.Count > 0)
            {
                foreach (var attach in lst)
                {
                    attachlink = string.Format("{0}<a href=\"{1}\">{2}</a>\n", attachlink, string.Format("{0}Common/QuestionFile?id={1}&filename={2}", baseUrl, attach.Id, attach.ImageName), attach.ImageName);
                }
            }
         
            var userModel = _objUserService.GetByWeChatUserID(ViewBag.WeChatUserID);
            if (userModel != null)
            {
                mobile = userModel.Tel;
            }

            var EmailContent = EmailTemplate.Replace("#Url#", request).Replace("#Question#", question).
                Replace("#WeChatUserID#", ViewBag.WeChatUserID).Replace("#Email#", receiver).Replace("#Contact#", mobile)
                .Replace("#Attachment#", attachlink);

            EmailMessageSettingsRecord set = new EmailMessageSettingsRecord()
            {
                Host = EmailHost,
                UserName = EmailUserName,
                Password = EmailPassword,
                Port = int.Parse(EmailPort),
                EnableSsl = EmailEnableSsl,
                Enable = EmailEnable,
                DeliveryMethod = "Network",
                Address = EmailSender,
                RequireCredentials = true
            };

            Task.Run(() =>
            {
                EmailMessageService ser = new EmailMessageService(set);
                ser.SendMessage(EmailSender, EmailReceiver, EmailTitle,
                    EmailContent.Replace("\r\n", "<br/>").Replace("\n", "<br/>").Replace("\r", "<br/>"));
            });

            return true;
        }
    }
}