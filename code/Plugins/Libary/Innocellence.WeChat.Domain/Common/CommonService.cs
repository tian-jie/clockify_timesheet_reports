using Infrastructure.Core.Data;
using Infrastructure.Core;
using Innocellence.CA.Contracts;
using Innocellence.CA.Entity;
using Innocellence.CA.ModelsView;
using Innocellence.CA.Service;
using Innocellence.CA.Service.Common;

using System.Collections.Generic;
using System.Threading.Tasks;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Xml;
using System;

using System.Linq.Expressions;
using Infrastructure.Web.Net.Mail;
using Infrastructure.Utility.Logging;
using Infrastructure.Core.Caching;




namespace Innocellence.CA.Service
{
    public class CommonService : BaseService<WechatUser>, ICommonService
    {
        // public ILogger Logger { get; set; }

        private static object objLock = new object();

        private static object objLockSys = new object();

        private static object objLockWeChat = new object();

        public static List<Category> lstCategory
        {
            get
            {
                var lst = CacheManager.GetCacher("Default").Get<List<Category>>("Category");
                if (lst == null)
                {
                    lock (objLock)
                    {
                        if (lst == null)
                        {
                            BaseService<Category> ser = new BaseService<Category>();
                            lst = ser.Entities.ToList();

                            //暂时不缓存
                          //  CacheManager.GetCacher("Default").Set("Category", lst,new TimeSpan(1,0,0));
                        }
                    }
                }

                return lst;
            }
        }

        public static List<SysConfig> lstSysConfig {
            get
            {
                var lst = CacheManager.GetCacher("Default").Get<List<SysConfig>>("SysConfig");
                if (lst == null)
                {
                    lock (objLock)
                    {
                        if (lst == null)
                        {
                            BaseService<SysConfig> ser = new BaseService<SysConfig>();
                            lst = ser.Entities.ToList();

                            CacheManager.GetCacher("Default").Set("SysConfig", lst,new TimeSpan(1,0,0));
                        }
                    }
                }

                return lst;
            }
        }

        public static List<SysWechatConfig> lstWeChatConfig
        {
            get
            {
                var lst = CacheManager.GetCacher("Default").Get<List<SysWechatConfig>>("WeChatConfig");
                if (lst == null)
                {
                    lock (objLock)
                    {
                        if (lst == null)
                        {
                            BaseService<SysWechatConfig> ser = new BaseService<SysWechatConfig>();
                            lst = ser.Entities.ToList();

                            CacheManager.GetCacher("Default").Set("SysWechatConfig", lst, new TimeSpan(1, 0, 0));
                        }
                    }
                }
                return lst;
            }
        }

        private static ILogger Logger = LogManager.GetLogger(typeof(CommonService));


        public CommonService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {

        }

        public static void ClearCache(int iType)
        {
            if(iType==1){
                CacheManager.GetCacher("Default").Remove("Category");
            }
            else if (iType == 2)
            {
                CacheManager.GetCacher("Default").Remove("SysConfig");
            }
            
        }

        public static List<Category> GetCategory(CategoryType iCode, string lan)
        {
            var category = GetCategory(iCode);
            return category.FindAll(a => a.LanguageCode.Equals(lan, StringComparison.CurrentCultureIgnoreCase));
        }

        public static List<Category> GetCategory(CategoryType iCode, string lan,bool isDelete)
        {
            var category = GetCategory(iCode);
            return category.FindAll(a =>a.IsDeleted==isDelete &&  a.LanguageCode.Equals(lan, StringComparison.CurrentCultureIgnoreCase));
        }

        /// <summary>
        /// 获取分类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="iCode"></param>
        /// <returns></returns>
        public static List<Category> GetCategory(CategoryType iCode)
        {


            return GetCategory(iCode,(bool?)null);


        }

        public static List<Category> GetCategory(CategoryType iCode,bool? isDelete)
        {
            //if (lstCategory == null)
            //{
            //    lock (objLock)
            //    {
            //        if (lstCategory == null)
            //        {
            //            BaseService<Category> ser = new BaseService<Category>();
            //            lstCategory = ser.Entities.ToList();


            //        }
            //    }
            //}

            if (isDelete.HasValue) { 
            return lstCategory.FindAll(a =>a.IsDeleted==isDelete && a.CategoryTypeCode == (int)iCode);
            }
            else
            {
                return lstCategory.FindAll(a =>  a.CategoryTypeCode == (int)iCode);
            }

        }

        /// <summary>
        /// 在lst中查找指定语言、指定Code的category的ID
        /// </summary>
        /// <param name="lst"></param>
        /// <param name="iCode"></param>
        /// <param name="strLanguage"></param>
        /// <returns></returns>
        public static int? GetCategoryID(ref List<Category> lst, int? iCateCode, CategoryType iType, string strLanguage)
        {
            if (iCateCode == null || iCateCode.Value==0) { return null; }
            var obj = GetCategory(ref lst, iCateCode.Value,iType, strLanguage);
            return obj == null ? null : (int?)obj.Id;
        }

        public static string GetCategoryValue(CategoryType iCode, int iCateCode, CategoryType iType, string strLanguage)
        {
            var lst = GetCategory(iCode);
            var obj = GetCategory(ref lst, iCateCode,iType, strLanguage);
            return obj == null ? null : obj.CategoryName;
        }
        public static string GetCategoryValueByID(ref List<Category> lst, int iCateID, CategoryType iType)
        {
            //var lst = GetCategory(iCode);
            var obj = GetCategoryByID(ref lst, iCateID, iType);
            return obj == null ? null : obj.CategoryName;
        }

        public static Category GetCategoryByID(ref List<Category> lst, int iCateID, CategoryType iType)
        {
            Category objRet=null;

            if (lst != null && lst.Count > 0)
            {
                objRet = lst.Find(a => a.Id == iCateID);
            }

            if (objRet == null)
            {
                ClearCache(1);
                lst = lstCategory.FindAll(a => a.CategoryTypeCode == (int)iType);
                objRet = lst.Find(a => a.Id == iCateID);
            }

            return objRet;
            //var obj = lstThis.Find(a => a.LanguageCode == strLanguage);
            //if (obj == null) { return null; }
           
        }

        public static Category GetCategory(ref List<Category> lst, int iCateCode,CategoryType iType, string strLanguage)
        {
            Category objRet = null;

            if (lst != null && lst.Count > 0)
            {
                var lstThis = lst.FindAll(a => a.CategoryCode == iCateCode);
                if (lstThis != null && lstThis.Count > 0)
                {
                    objRet = lstThis.Find(a => a.LanguageCode == strLanguage);
                }
            }

            if (objRet == null)
            {
                ClearCache(1);
                lst = lstCategory.FindAll(a => a.CategoryTypeCode == (int)iType);
                objRet = lst.Find(a => a.CategoryCode == iCateCode);
            }

            return objRet;
        }

        public static SysConfig GetSysConfig(SysConfigCode strCode)
        {
            //if (lstSysConfig == null)
            //{
            //    lock (objLockSys)
            //    {
            //        if (lstSysConfig == null)
            //        {
            //            BaseService<SysConfig> ser = new BaseService<SysConfig>();
            //            lstSysConfig = ser.Entities.ToList();
            //        }
            //    }
            //}

            return lstSysConfig.Find(a => a.ConfigCode == (int)strCode);
        }

        public static string GetSysConfig(SysConfigCode strCode, string strDefault)
        {
            var obj = GetSysConfig(strCode);

            return obj == null || string.IsNullOrEmpty(obj.ConfigValue) ? strDefault : obj.ConfigValue;
        }


        public static SysWechatConfig GetWeChatConfig(int iAppID)
        {
            var obj = new SysWechatConfig()
            {
                WeixinAppId = ConfigurationManager.AppSettings["WeixinAppId"],
                WeixinCorpId = ConfigurationManager.AppSettings["WeixinCorpId"],
                WeixinCorpSecret = ConfigurationManager.AppSettings["WeixinCorpSecret"],
                WeixinEncodingAESKey = ConfigurationManager.AppSettings["WeixinEncodingAESKey"],
                WeixinToken = ConfigurationManager.AppSettings["WeixinToken"]
            };

            //var obj = lstWeChatConfig.Find(a => a.WeixinAppId == iAppID.ToString());

            //if (obj == null)
            //{
            //    CacheManager.GetCacher("Default").Remove("SysConfig");
            //    return lstWeChatConfig.Find(a => a.WeixinAppId == iAppID.ToString());
            //}

            return obj;

            //return obj == null || string.IsNullOrEmpty(obj.ConfigValue) ? strDefault : obj.ConfigValue;
        }




        public static bool SendMail(string strUserName,String strContent)
        {
           // Logger.Info("SendMail start");

           //// strContent = strContent

           // var EmailFeedback = GetSysConfig(SysConfigCode.EmailFeedback, "");
           // var EmailEnableSsl = GetSysConfig(SysConfigCode.EmailEnableSsl, "true");
           // var EmailHost = GetSysConfig(SysConfigCode.EmailHost, "");
           // var EmailPassword = GetSysConfig(SysConfigCode.EmailPassword, "");
           // var EmailUserName = GetSysConfig(SysConfigCode.EmailUserName, "");
           // var EmailPort = GetSysConfig(SysConfigCode.EmailPort, "0");
           // var EmailSender = GetSysConfig(SysConfigCode.EmailSender, "");
           // var EmailEnable = GetSysConfig(SysConfigCode.EmailEnable, "true");
           // var EmailTitle = GetSysConfig(SysConfigCode.EmailTitle, "");

           // var EmailTemplate = GetSysConfig(SysConfigCode.EmailTemplate, "#UserName# Feedback Content:\r\n #Content#");

           // var EmailContent = EmailTemplate.Replace("#UserName#", strUserName).Replace("#Content#", strContent);

           // EmailMessageSettingsRecord set = new EmailMessageSettingsRecord()
           // {
           //     Host = EmailHost,
           //     UserName = EmailUserName,
           //     Password = EmailPassword,
           //     Port = int.Parse(EmailPort),
           //     EnableSsl = bool.Parse(EmailEnableSsl),
           //     Enable = bool.Parse(EmailEnable),
           //     DeliveryMethod = "Network",
           //     Address = EmailSender,
           //     RequireCredentials = true
           // };


           // Task.Run(() =>
           // {
           //     EmailMessageService ser = new EmailMessageService(set);
           //     ser.SendMessage(EmailTitle, EmailFeedback, "Feedback", EmailContent.Replace("\r\n", "<br/>").Replace("\n", "<br/>").Replace("\r", "<br/>"));
           // });




            return true;
        }

        #region 获取XML中节点信息
        public XmlNodeList GetXmlNodes(string tagName)
        {
            XmlNodeList nodes = null;
            try
            {
                // 系统目录
                string atr = System.AppDomain.CurrentDomain.BaseDirectory;
                // WebConfig中配置的XML文件路径
                string strPath = ConfigurationManager.AppSettings["XmlPath"];

                XmlDocument doc = new XmlDocument();

                doc.Load(atr + strPath);

                nodes = doc.GetElementsByTagName(tagName);
            }
            catch (Exception ex)
            {
                // Logger.Error(ex, "获取XML中节点信息错误-" + tagName, ex.ToString());
            }
            return nodes;
        }
        #endregion

        #region 发送邮件
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="strto">收件人</param>
        /// <param name="subj">邮件主题</param>
        /// <param name="bodys">邮件内容</param>
        /// <returns>是否发送成功</returns>
        public bool SendMail(string strto, string subj, string bodys)
        {
            XmlNode mailserver = GetXmlNodes("MailServer")[0];

            // SMTP服务器
            string smtpserver = mailserver.Attributes["smtpserver"].Value;
            // 发件人
            string strfrom = mailserver.Attributes["strfrom"].Value;
            // 发件人用户名
            string userName = mailserver.Attributes["userName"].Value;
            // 发件人密码
            string pwd = mailserver.Attributes["pwd"].Value;
            // 端口
            int sendPort = int.Parse(mailserver.Attributes["sendPort"].Value);
            // 是否SSL加密
            bool ssl = bool.Parse(mailserver.Attributes["ssl"].Value);

            try
            {
                System.Net.Mail.SmtpClient _smtpClient = new System.Net.Mail.SmtpClient();
                _smtpClient.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                _smtpClient.Host = smtpserver;  //SMTP服务器
                _smtpClient.Port = sendPort;    //端口
                _smtpClient.EnableSsl = ssl;    //是否SSL加密
                _smtpClient.Credentials = new System.Net.NetworkCredential(userName, pwd);
                System.Net.Mail.MailMessage _mailMessage = new System.Net.Mail.MailMessage(strfrom, strto);
                _mailMessage.Subject = subj;
                _mailMessage.SubjectEncoding = System.Text.Encoding.UTF8;//正文编码
                _mailMessage.Body = bodys;
                _mailMessage.BodyEncoding = System.Text.Encoding.UTF8;//正文编码
                _mailMessage.IsBodyHtml = true;//设置为HTML格式
                _mailMessage.Priority = System.Net.Mail.MailPriority.High;//优先级
                _smtpClient.Send(_mailMessage);
                return true;
            }
            catch
            {
                throw;
            }
        }


        #endregion

    }


}
