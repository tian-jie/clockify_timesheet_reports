using Infrastructure.Core.Data;
using Infrastructure.Core.Logging;
using Infrastructure.Utility.Data;
using Infrastructure.Utility.IO;
using Infrastructure.Web.Domain.Entity;
using Infrastructure.Web.Domain.Service;
using Infrastructure.Web.Domain.Service.Common;
using Infrastructure.Web.UI;
using Innocellence.Authentication.Authentication;
using Innocellence.WeChatMain.Common;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.Contracts.ViewModel;
using Innocellence.WeChat.Domain.ModelsView;
using Innocellence.WeChat.Domain.Common;
using Innocellence.WeChatMain.Services;
using Innocellence.Weixin.QY.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using Innocellence.WeChat.Domain.Services;
using Innocellence.WeChat.Domain.Service;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.IO;
using Innocellence.WeChat.Domain.ViewModelFront;
using Innocellence.Weixin.MP.AdvancedAPIs.GroupMessage;
using System.Threading;
using Innocellence.Weixin.Entities;
using Innocellence.WeChat.Domain;
using Infrastructure.Core.Infrastructure;
using System.Drawing;
using Innocellence.Weixin.MP.AdvancedAPIs;

namespace Innocellence.WeChatMain.Controllers
{
    public partial class ArticleInfoController : BaseController<ArticleInfo, ArticleInfoView>
    {
        protected IArticleThumbsUpService _articleThumbsUpService;//= new BaseService<ArticleThumbsUp>("CAAdmin");
        private static readonly IDataPermissionCheck permission = new ArticleInfoDataPermissionService();
        private static string wechatBaseUrl = CommonService.GetSysConfig("WeChatUrl", "");
        private readonly IArticleInfoService _articleInfoService;
        private IWechatMPUserService _WechatMPUserService = new WechatMPUserService();
        //protected BaseService<AttachmentsItem> _IDXYImagesRootService = new BaseService<AttachmentsItem>();
        //protected BaseService<AttachmentsFilesItem> _IDXYImagesRootFilesService = new BaseService<AttachmentsFilesItem>();
        //protected BaseService<DXYAutoReplyRoot> _DXYAutoReplyRootService = new BaseService<DXYAutoReplyRoot>();
        //protected BaseService<DXYAutoReplyKeyWordItem> _IDXYAutoReplyKeyWordItemFilesService = new BaseService<DXYAutoReplyKeyWordItem>();
        //protected BaseService<DXYAutoReplyItem> _IDXYAutoReplyItemService = new BaseService<DXYAutoReplyItem>();
        //protected BaseService<DXYDetail> _DXYDetailService = new BaseService<DXYDetail>("CAAdmin");
        protected BaseService<QrCodeMPItem> _IDXYDXYQRCodeItemService = new BaseService<QrCodeMPItem>("CAAdmin");

        ILogger _log = LogManager.GetLogger("ArticleInfoController");

        public ArticleInfoController(IArticleInfoService objService, IArticleThumbsUpService articleThumbsUpService, int appId)
            : base(objService)
        {
            _articleInfoService = objService;
            _articleThumbsUpService = articleThumbsUpService;
            AppId = appId;
            ViewBag.AppId = AppId;
        }

        public ArticleInfoController(IArticleInfoService objService, IArticleThumbsUpService articleThumbsUpService)
            : base(objService)
        {
            _articleInfoService = objService;
            _articleThumbsUpService = articleThumbsUpService;

            // AppId = (int)CategoryType.Undefined;
        }

        [HttpGet, DataSecurityFilter]
        public virtual ActionResult Edit(string id, int? appId)
        {
            AppId = appId.GetValueOrDefault();

            ViewBag.AppId = AppId;
            var appInfo = WeChatCommonService.lstSysWeChatConfig.FirstOrDefault(a => a.Id == AppId);
            ViewBag.IsCorp = (appInfo.IsCorp == null || appInfo.IsCorp == false) ? 0 : 1;
            CreateCategoryListAction((CategoryType)(AppId));

            var obj = GetObject(id);

            ViewBag.cateId = -1;
            if (!string.IsNullOrEmpty(id) && obj.CategoryId.HasValue)
            {
                var cateInfo = CommonService.lstCategory.FirstOrDefault(a =>
                    obj.CategoryId == a.Id && a.AppId == AppId && a.IsDeleted == false);

                ViewBag.cateId = obj.CategoryId;
                ViewBag.cateName = cateInfo != null ? cateInfo.CategoryName.ToString() : string.Empty;

                //data permission
                base.AppDataPermissionCheck(permission, appId.GetValueOrDefault(), obj.AppId.GetValueOrDefault());
            }

            return View("../ArticleInfo/Edit", obj);
        }

        #region 抓取数据用

        public virtual void DIATest()
        {

            for (int i = 0; i < 5; i++)
            {
                Thread t = new Thread(new ThreadStart(DIAGET));//注意ThreadStart委托的定义形式
                t.Start();//线程开始，控制权返回Main线程
                //while (t.IsAlive == true) ;
                Thread.Sleep(100);
                t.Abort();
                t.Join();//阻塞Main线程,直到t终止
            }
        }

        private static void DIAGET()
        {
            HttpClient httpClient = new HttpClient();

            HttpResponseMessage response = httpClient.GetAsync("http://10.0.0.157/Diabetes/diabetes/api/uploadalldata").Result;


            string loginResult = response.Content.ReadAsStringAsync().Result;
        }

        [HttpGet]
        public virtual ActionResult ImportFromDXY()
        {

            HttpClient httpClient = new HttpClient();
            List<KeyValuePair<String, String>> paramList = new List<KeyValuePair<String, String>>();

            paramList.Add(new KeyValuePair<string, string>("account", "111"));
            HttpResponseMessage response = httpClient.PostAsync(new Uri("http://10.0.0.157/Diabetes/diabetes/api/uploadalldata"), new FormUrlEncodedContent(paramList)).Result;


            string loginResult = response.Content.ReadAsStringAsync().Result;
            return View();
        }

        public virtual ActionResult ImportFromDXY(string username, string password)
        {
            HttpClientHandler handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };
            handler.UseCookies = true;
            HttpClient httpClient = new HttpClient(handler);
            httpClient.MaxResponseContentBufferSize = 256000;
            httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
            httpClient.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.8");
            httpClient.DefaultRequestHeaders.Add("Host", "sim.dxy.cn");
            httpClient.DefaultRequestHeaders.Add("Referer", "https://sim.dxy.cn/user/login");
            httpClient.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.73 Safari/537.36");
            string loginurl = "https://sim.dxy.cn/japi/login";
            List<KeyValuePair<String, String>> paramList = new List<KeyValuePair<String, String>>();

            paramList.Add(new KeyValuePair<string, string>("account", username));
            paramList.Add(new KeyValuePair<string, string>("password", password));
            paramList.Add(new KeyValuePair<string, string>("postURL", "/japi/login"));

            HttpResponseMessage response = httpClient.PostAsync(new Uri(loginurl), new FormUrlEncodedContent(paramList)).Result;


            string loginResult = response.Content.ReadAsStringAsync().Result;
            //httpClient.DefaultRequestHeaders.Remove("Referer");
            httpClient.DefaultRequestHeaders.Remove("Accept-Encoding");
            //httpClient.DefaultRequestHeaders.Remove("Referer");
            httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, sdch");
            httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("GZIP"));

            handler.CookieContainer.Add(AddCookiesMP());

            //CookieCollection ddd;
            //ddd = handler.CookieContainer.GetCookies(loginurl);



            //List<DXYAutoReplyRootListView> lstDXYItems = new List<DXYAutoReplyRootListView>();
            //int errorCount = 0;
            //for (int i = 1; i <= pagecout; i++)
            //{
            //    string listUrl = "https://sim.dxy.cn/japi/weixin/autoreply/rule/list?applicationId=" + url + "&page=" + i + "&size=" + pagesize;
            //    response = httpClient.GetAsync(new Uri(listUrl)).Result;

            //    string listResult = response.Content.ReadAsStringAsync().Result;
            //    DXYAutoReplyRootListView DXYItems = JsonConvert.DeserializeObject<DXYAutoReplyRootListView>(listResult);

            //    if (DXYItems.success == "True")
            //    {
            //        lstDXYItems.Add(DXYItems);
            //    }
            //    else {
            //        errorCount++;
            //    }
            //}


            // Images&Voice&Video
            //string imagesBaseUrl = "https://sim.dxy.cn/japi/media/list?applicationId=" + appid;


            //List<string> lstApps = new List<string> { "142", "148", "149", "150", "156", "164", "179", "190", "192", "217", "238", "250", "258", "265", "284", "294", "296", "384" };
            //List<DXYImagesRootView> allList = new List<DXYImagesRootView>();
            //foreach (string strappid in lstApps)
            //{
            //    // Files
            //    string filesBaseUrl = "https://sim.dxy.cn/japi/file/list?applicationId=" + strappid;
            //    SaveDXYFiles(GetFileList(filesBaseUrl, response, httpClient), strappid);             
            //}


            //ViewBag.ImgMessage = SaveDXYImage(GetRootList(appid, response, httpClient), appid);
            //ViewBag.ImgMessage = SaveAutoReply(url,lstDXYItems, response, httpClient);
            //ViewBag.ImgMessage = SaveDXYImage(lstDXYImages);
            //ViewBag.ImgMessage = SaveDXYText(lstDXYImages);
            //ViewBag.ImgMessage = SaveDXYFiles(lstDXYImages);
            ViewBag.ImgMessage = SaveQRCode(response, httpClient);

            //CheckImage();

            //httpClient.Dispose();
            return View();
        }
        private string SaveQRCode(HttpResponseMessage response, HttpClient httpClient)
        {
            string strValue = string.Empty;

            string listUrl = "https://sim.dxy.cn/japi/weixin/qrcode/list?current=1&page=1&total=1&count=0&size=10&page_url=";
            response = httpClient.GetAsync(new Uri(listUrl)).Result;

            DXYQRCodeRootView listResultKeyWords = JsonConvert.DeserializeObject<DXYQRCodeRootView>(response.Content.ReadAsStringAsync().Result);

            int intTotal = listResultKeyWords.pageBean.total;

            List<QrCodeMPItem> Models = new List<QrCodeMPItem>();

            for (int i = intTotal; i > 0; i--)
            {
                listUrl = "https://sim.dxy.cn/japi/weixin/qrcode/list?current=1&page=" + i + "&total=1&count=0&size=10&page_url=";
                response = httpClient.GetAsync(new Uri(listUrl)).Result;

                DXYQRCodeRootView listResultKeyWordsSub = JsonConvert.DeserializeObject<DXYQRCodeRootView>(response.Content.ReadAsStringAsync().Result);

                Models.AddRange(listResultKeyWordsSub.items.OrderBy(p => p.SceneId));
            }

            WebClient mClient = new WebClient();
            foreach (QrCodeMPItem item in Models)
            {

                //if (!string.IsNullOrEmpty(item.url))
                //{
                //    string savePath = Server.MapPath("/content/QRCodeMP" + item.url);

                //    string saveFolderPath = savePath.Replace(savePath.Split('\\').Last(), "");

                //    if (!Directory.Exists(saveFolderPath))//判断文件夹是否存在 
                //    {
                //        Directory.CreateDirectory(saveFolderPath);//不存在则创建文件夹 
                //    }
                //    mClient = new WebClient();
                //    mClient.DownloadFile("https://sim.dxy.cn" + item.url, savePath);
                //}

                _IDXYDXYQRCodeItemService.Repository.Insert(item);
            }


            return strValue;
        }

        //private void CheckImage()
        //{
        //    int okCount = 0;
        //    int errCount = 0;
        //    List<DXYDetail> lstAttachmentsItem = _DXYDetailService.Repository.SqlQuery("select * from DXYDetail where body is not null").ToList();
        //    WebClient mClient = new WebClient();
        //    foreach (DXYDetail item in lstAttachmentsItem)
        //    {
        //        Regex imgsrc = new Regex("< *[img][^\\>]*[src] *= *[\"\']{0,1}([^\"\'\\>]*)", RegexOptions.IgnoreCase);
        //        MatchCollection matches = imgsrc.Matches(item.body);

        //        foreach (Match mc in matches)
        //        {
        //            try
        //            {
        //                string strPath = mc.Groups[0].Value;
        //                strPath = strPath.Substring(strPath.IndexOf("src=\"")+5);
        //                string ourPath = "/content/WeChatMaterial" + strPath.Substring(strPath.IndexOf("/imgqn/"));
        //                string savePath = Server.MapPath(ourPath);
        //                if (!System.IO.File.Exists(savePath))//判断文件是否存在 
        //                {
        //                    string saveFolderPath = savePath.Replace(savePath.Split('\\').Last(), "");

        //                    if (!Directory.Exists(saveFolderPath))//判断文件夹是否存在 
        //                    {
        //                        Directory.CreateDirectory(saveFolderPath);//不存在则创建文件夹 
        //                    }
        //                    mClient = new WebClient();
        //                    mClient.DownloadFile(strPath, savePath);
        //                }
        //                okCount++;
        //            }
        //            catch (Exception ex)
        //            {
        //                errCount++;
        //                _log.Error("失败ID" + item.Id + "|" + ex.Message);
        //            }
        //        }
        //    }
        //    mClient.Dispose();
        //}

        //private string SaveAutoReply(string appId, List<DXYAutoReplyRootListView> lstDXYItems, HttpResponseMessage response, HttpClient httpClient)
        //{
        //    int sCount = 0;
        //    int ErrorCount = 0;

        //    foreach (DXYAutoReplyRootListView item in lstDXYItems)
        //    {
        //            foreach (DXYAutoReplyRoot subItem in item.rules)
        //            {
        //                try
        //                {

        //                    subItem.AppId = appId;
        //                    _DXYAutoReplyRootService.Repository.Insert(subItem);

        //                    string listUrlKeyWords = "https://sim.dxy.cn/japi/weixin/autoreply/keywords/list?ruleId=" + subItem.ruleId + "&applicationId=" + appId;
        //                    response = httpClient.GetAsync(new Uri(listUrlKeyWords)).Result;
        //                    string listResultKeyWords = response.Content.ReadAsStringAsync().Result;
        //                    DXYAutoReplyKeyWordsView keyWords = JsonConvert.DeserializeObject<DXYAutoReplyKeyWordsView>(listResultKeyWords);
        //                    foreach (DXYAutoReplyKeyWordItem keyItem in keyWords.keywords)
        //                    {
        //                        keyItem.DXYid = keyItem.Id.ToString();
        //                        keyItem.AppId = appId;
        //                        _IDXYAutoReplyKeyWordItemFilesService.Repository.Insert(keyItem);
        //                    }

        //                    string listUrl = "https://sim.dxy.cn/japi/weixin/autoreply/routes/list?ruleId=" + subItem.ruleId + "&applicationId=" + appId;
        //                    response = httpClient.GetAsync(new Uri(listUrl)).Result;
        //                    string listResult = response.Content.ReadAsStringAsync().Result;
        //                    DXYAutoReplyRootView rList = JsonConvert.DeserializeObject<DXYAutoReplyRootView>(listResult);
        //                    foreach (RulesItems rItem in rList.routes)
        //                    {
        //                        foreach (DXYAutoReplyItem cItem in rItem.repliers)
        //                        {
        //                            cItem.routeId = rItem.routeId;
        //                            cItem.ruleId = rItem.ruleId;
        //                            cItem.AppId = appId;
        //                            _IDXYAutoReplyItemService.Repository.Insert(cItem);
        //                        }
        //                    }
        //                    sCount++;
        //                }
        //                catch (Exception ex)
        //                {
        //                    ErrorCount++;
        //                    _log.Error("失败previousId=" + subItem.previousId + "|" + ex.Message);

        //                }
        //            }             
        //    }


        //    return "成功" + sCount + "次,失败" + ErrorCount + "次";
        //}

        //private List<DXYImagesRootView> GetRootList(string url, HttpResponseMessage response, HttpClient httpClient)
        //{
        //    List<DXYImagesRootView> lstDXYImages = new List<DXYImagesRootView>();

        //    List<string> types = new List<string> { "image", "voice", "video" };

        //    foreach (string type in types)
        //    {
        //        string listUrl = url + "&type=" + type;
        //        response = httpClient.GetAsync(new Uri(listUrl)).Result;
        //        string listResult = response.Content.ReadAsStringAsync().Result;
        //        DXYImagesRootView DXYImages = JsonConvert.DeserializeObject<DXYImagesRootView>(listResult);        

        //        for (int i = 1; i <= DXYImages.pageBean.total; i++)
        //        {
        //            string nowUrl = listUrl + "&page=" + i + "&size=" + DXYImages.pageBean.size;
        //            response = httpClient.GetAsync(new Uri(nowUrl)).Result;
        //            string result = response.Content.ReadAsStringAsync().Result;
        //            DXYImagesRootView DXYResults = JsonConvert.DeserializeObject<DXYImagesRootView>(result);
        //            lstDXYImages.Add(DXYResults);
        //        }
        //    }           
        //    return lstDXYImages;
        //}

        //private List<DXYImagesRootView> GetFileList(string url, HttpResponseMessage response, HttpClient httpClient)
        //{
        //    List<DXYImagesRootView> lstDXYImages = new List<DXYImagesRootView>();

        //    response = httpClient.GetAsync(new Uri(url)).Result;
        //    string listResult = response.Content.ReadAsStringAsync().Result;
        //    DXYImagesRootView DXYImages = JsonConvert.DeserializeObject<DXYImagesRootView>(listResult);

        //    for (int i = 1; i <= DXYImages.pageBean.total; i++)
        //    {
        //        string nowUrl = url + "&page=" + i + "&size=" + DXYImages.pageBean.size;
        //        response = httpClient.GetAsync(new Uri(nowUrl)).Result;
        //        string result = response.Content.ReadAsStringAsync().Result;
        //        DXYImagesRootView DXYResults = JsonConvert.DeserializeObject<DXYImagesRootView>(result);
        //        lstDXYImages.Add(DXYResults);
        //    }

        //    return lstDXYImages;
        //}

        //private string SaveDXYImage(List<DXYImagesRootView> models, string appid)
        //{
        //    List<AttachmentsItem> lstAttachmentsItem = _IDXYImagesRootService.Repository.SqlQuery("select * from AttachmentsItem").ToList();


        //    WebClient mClient = new WebClient();
        //    int OkCount = 0;
        //    int newCount = 0;
        //    int ErrorCount = 0;
        //    foreach (DXYImagesRootView item in models)
        //    {
        //        foreach (AttachmentsItem attachItem in item.attachments)
        //        {
        //            try
        //            {
        //                List<AttachmentsItem> lstItems = lstAttachmentsItem.Where(p => p.attachmentUrl == attachItem.attachmentUrl).ToList();
        //                if (lstItems.Count() > 0)
        //                {
        //                    AttachmentsItem updateItem = lstItems.Last();
        //                    updateItem.AppId = appid;
        //                    _IDXYImagesRootService.Repository.Update(updateItem);
        //                    OkCount++;
        //                }
        //                else
        //                {
        //                    attachItem.AppId = appid;

        //                    if (!string.IsNullOrEmpty(attachItem.attachmentUrl))
        //                    {
        //                        string savePath = Server.MapPath("/content/WeChatMaterial" + new Uri(attachItem.attachmentUrl).LocalPath);

        //                        string saveFolderPath = savePath.Replace(savePath.Split('\\').Last(), "");

        //                        if (!Directory.Exists(saveFolderPath))//判断文件夹是否存在 
        //                        {
        //                            Directory.CreateDirectory(saveFolderPath);//不存在则创建文件夹 
        //                        }
        //                        mClient = new WebClient();
        //                        mClient.DownloadFile(attachItem.attachmentUrl, savePath);
        //                    }
        //                    if (!string.IsNullOrEmpty(attachItem.thumbUrl))
        //                    {
        //                        string savePath = Server.MapPath("/content/WeChatMaterial" + attachItem.thumbUrl);

        //                        string saveFolderPath = savePath.Replace(savePath.Split('\\').Last(), "");

        //                        if (!Directory.Exists(saveFolderPath))//判断文件夹是否存在 
        //                        {
        //                            Directory.CreateDirectory(saveFolderPath);//不存在则创建文件夹 
        //                        }
        //                        mClient = new WebClient();
        //                        mClient.DownloadFile("https://sim.dxy.cn" + attachItem.thumbUrl, savePath);
        //                    }
        //                    _IDXYImagesRootService.Repository.Insert(attachItem);
        //                    newCount++;
        //                    _log.Error("新插attachItem.attachmentUrl=" + attachItem.attachmentUrl);
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                ErrorCount++;
        //                _log.Error("失败attachItem.attachmentUrl=" + attachItem.attachmentUrl + "|" + ex.Message);
        //            }
        //        }
        //    }
        //    mClient.Dispose();

        //    return "成功" + OkCount + "次，失败" + ErrorCount + "次，新插入次数" + newCount;
        //}

        //private string SaveDXYFiles(List<DXYImagesRootView> models, string appid)
        //{
        //    List<AttachmentsFilesItem> lstAttachmentsItem = _IDXYImagesRootFilesService.Repository.SqlQuery("select * from AttachmentsFilesItem").ToList();


        //    WebClient mClient = new WebClient();
        //    int OkCount = 0;
        //    int newCount = 0;
        //    int ErrorCount = 0;
        //    foreach (DXYImagesRootView item in models)
        //    {
        //        foreach (AttachmentsFilesItem fileItem in item.files)
        //        {
        //            try
        //            {
        //                List<AttachmentsFilesItem> lstItems = lstAttachmentsItem.Where(p => p.fileId == fileItem.fileId).ToList();
        //                if (lstItems.Count() > 0)
        //                {
        //                    AttachmentsFilesItem updateItem = lstItems.Last();
        //                    updateItem.AppId = appid;
        //                    _IDXYImagesRootFilesService.Repository.Update(updateItem);
        //                    OkCount++;
        //                }
        //                else
        //                {
        //                    fileItem.AppId = appid;
        //                    DateTime tempDt = Convert.ToDateTime(fileItem.uploadTime);

        //                    string realUrl = "/japi/download/" + tempDt.Year + "/" + (tempDt.Month + 100).ToString().Substring(1) + "/" + (tempDt.Day + 100).ToString().Substring(1) + "/" + fileItem.uid + "." + fileItem.ext;
        //                    string savePath = Server.MapPath("/content/WeChatMaterial" + realUrl);
        //                    fileItem.thumbUrl = realUrl;

        //                    string saveFolderPath = savePath.Replace(savePath.Split('\\').Last(), "");
        //                    if (!Directory.Exists(saveFolderPath))//判断文件夹是否存在 
        //                    {
        //                        Directory.CreateDirectory(saveFolderPath);//不存在则创建文件夹 
        //                    }
        //                    mClient = new WebClient();
        //                    mClient.DownloadFile("https://sim.dxy.cn" + realUrl, savePath);
        //                    _IDXYImagesRootFilesService.Repository.Insert(fileItem);
        //                    newCount++;
        //                    _log.Error("新插fileId=" + fileItem.fileId);
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                ErrorCount++;
        //                _log.Error("失败path=" + fileItem.path + "|" + ex.Message);

        //            }
        //        }
        //    }
        //    mClient.Dispose();

        //    return "成功" + OkCount + "次，失败" + ErrorCount + "次";
        //}

        private CookieCollection AddCookies()
        {
            CookieCollection addCookie = new CookieCollection();

            Cookie model = new Cookie("_gat", "1");
            model.Domain = "sim.dxy.cn";
            addCookie.Add(model);
            model = new Cookie("kdtnote_fans_id", "1468981768");
            model.Domain = "sim.dxy.cn";
            addCookie.Add(model);
            model = new Cookie("JSESSIONID", "2EFF3960F431461BC45773B9AA6EB19F");
            model.Domain = "sim.dxy.cn";
            addCookie.Add(model);
            model = new Cookie("cookieTabset", "multiple");
            model.Domain = "sim.dxy.cn";
            addCookie.Add(model);
            model = new Cookie("kdt_id", "121");
            model.Domain = "sim.dxy.cn";
            addCookie.Add(model);
            model = new Cookie("_ga", "GA1.2.972817531.1468201346");
            model.Domain = "sim.dxy.cn";
            addCookie.Add(model);
            model = new Cookie("team_auth_key", "f21522473c622a9df61cade7d23e3792");
            model.Domain = "sim.dxy.cn";
            addCookie.Add(model);
            model = new Cookie("cookieTabset", "file");
            model.Domain = "sim.dxy.cn";
            addCookie.Add(model);
            return addCookie;
        }

        private CookieCollection AddCookiesMP()
        {
            CookieCollection addCookie = new CookieCollection();

            Cookie model = new Cookie("_gat", "1");
            model.Domain = "sim.dxy.cn";
            addCookie.Add(model);
            model = new Cookie("kdtnote_fans_id", "1468296135");
            model.Domain = "sim.dxy.cn";
            addCookie.Add(model);
            model = new Cookie("JSESSIONID", "086765324F9242B70B9508944610FB30");
            model.Domain = "sim.dxy.cn";
            addCookie.Add(model);
            model = new Cookie("sysTheme", "theme_b");
            model.Domain = "sim.dxy.cn";
            addCookie.Add(model);
            model = new Cookie("kdt_id", "84");
            model.Domain = "sim.dxy.cn";
            addCookie.Add(model);
            model = new Cookie("_ga", "GA1.2.972817531.1468201346");
            model.Domain = "sim.dxy.cn";
            addCookie.Add(model);
            //model = new Cookie("team_auth_key", "fcbab277afc8d01bec5c454c2b20da55");
            //model.Domain = "sim.dxy.cn";
            //addCookie.Add(model);
            //model = new Cookie("user_id", "151");
            //model.Domain = "sim.dxy.cn";
            //addCookie.Add(model);
            //model = new Cookie("user_weixin", "15016943392");
            //model.Domain = "sim.dxy.cn";
            //addCookie.Add(model);
            //model = new Cookie("user_nickname", "15016943392");
            //model.Domain = "sim.dxy.cn";
            //addCookie.Add(model);
            //model = new Cookie("login_auth_key", "b080fa429103dccd2a7cc86ce58658a1");
            //model.Domain = "sim.dxy.cn";
            //addCookie.Add(model);
            //model = new Cookie("weixin_auth_key", "cb450c3bfc36674c6be833e470147e21");
            //model.Domain = "sim.dxy.cn";
            //addCookie.Add(model);

            return addCookie;
        }

        //private string OldSaveDXYImage(List<DXYImagesRootView> models)
        //{
        //    WebClient mClient = new WebClient();
        //    int OkCount = 0;
        //    int ImgCount = 0;
        //    int ErrorCount = 0;
        //    foreach (DXYImagesRootView item in models)
        //    {
        //        foreach (AttachmentsItem attachItem in item.attachments)
        //        {
        //            try
        //            {
        //                if (!string.IsNullOrEmpty(attachItem.attachmentUrl))
        //                {
        //                    string savePath = Server.MapPath("/content/WeChatMaterial" + new Uri(attachItem.attachmentUrl).LocalPath);

        //                    string saveFolderPath = savePath.Replace(savePath.Split('\\').Last(), "");

        //                    if (!Directory.Exists(saveFolderPath))//判断文件夹是否存在 
        //                    {
        //                        Directory.CreateDirectory(saveFolderPath);//不存在则创建文件夹 
        //                    }
        //                    mClient = new WebClient();
        //                    mClient.DownloadFile(attachItem.attachmentUrl, savePath);
        //                    ImgCount++;

        //                }
        //                if (!string.IsNullOrEmpty(attachItem.thumbUrl))
        //                {
        //                    string savePath = Server.MapPath("/content/WeChatMaterial" + attachItem.thumbUrl);

        //                    string saveFolderPath = savePath.Replace(savePath.Split('\\').Last(), "");

        //                    if (!Directory.Exists(saveFolderPath))//判断文件夹是否存在 
        //                    {
        //                        Directory.CreateDirectory(saveFolderPath);//不存在则创建文件夹 
        //                    }
        //                    mClient = new WebClient();
        //                    mClient.DownloadFile("https://sim.dxy.cn" + attachItem.thumbUrl, savePath);
        //                    ImgCount++;
        //                }
        //                OkCount++;
        //                _IDXYImagesRootService.Repository.Insert(attachItem);
        //            }
        //            catch (Exception ex)
        //            {
        //                ErrorCount++;
        //                _log.Error("失败attachItem.attachmentUrl=" + attachItem.attachmentUrl + "|" + ex.Message);

        //            }
        //        }
        //    }
        //    mClient.Dispose();

        //    return "成功" + OkCount + "次，失败" + ErrorCount + "次，共抓取图片" + ImgCount + "个";
        //}
        #endregion 抓取数据用
        protected virtual void CreateCategoryListAction(CategoryType category)
        {
            List<Category> lstCate = CommonService.GetCategory((int)category, false).ToList();

            ViewBag.lstCate = lstCate;
        }

        [HttpGet, DataSecurityFilter]
        public virtual ActionResult Index(int? appId)
        {
            int AppId = appId==58 ? 10 : appId.GetValueOrDefault();

            CreateCategoryListAction((CategoryType)AppId);
            ViewBag.AppId = AppId;
            var appName = WeChatCommonService.lstSysWeChatConfig.Find(a => a.Id == appId);
            if (appName != null)
            {
                ViewBag.APPName = appName.AppName;
            }
            else
            {
                ViewBag.APPName = string.Empty;

            }
            return View("../ArticleInfo/Index");
        }

        public ActionResult GetNewsList(int appId, int PageIndex = 1, int PageSize = 20)
        {
            GridRequest req = new GridRequest(this.Request);
            Expression<Func<ArticleInfo, bool>> predicate = _ => true;
            predicate = predicate.AndAlso(a => a.IsDeleted == false && a.ArticleType == 0 && a.AppId == appId);
            if (!string.IsNullOrEmpty(Request["searchKeyword"]))
            {
                string searchKeyword = Request["searchKeyword"];
                predicate = predicate.AndAlso(a => a.ArticleTitle.ToUpper().Contains(searchKeyword.ToUpper()));
            }
            PageCondition ConPage = new PageCondition()
            {
                PageIndex = PageIndex,
                PageSize = PageSize,
                RowCount = 0,
            };
            var q = _articleInfoService.GetListWithContent<ArticleInfoView>(predicate, ConPage);

            // CategoryCode转变换CategoryName
            var lstCate = CommonService.GetCategory((appId), false);
            q = ChangeCategoryCodeToCategoryName(q, lstCate);
            req.PageCondition.RowCount = ConPage.RowCount;
            return this.GetPageResult(q, req);
        }

        public override List<ArticleInfoView> GetListEx(Expression<Func<ArticleInfo, bool>> predicate, PageCondition ConPage)
        {
            string strTitle = Request["txtArticleTitle"];
            string txtDate = Request["txtDate"];
            string strSubCate = Request["txtSubCate"];

            var q = GetListPrivate(ref predicate, ConPage, strTitle, string.Empty, txtDate, strSubCate);

            return q.ToList();
        }

        protected List<ArticleInfoView> GetListPrivate(ref Expression<Func<ArticleInfo, bool>> predicate, PageCondition ConPage, string strTitle = "", string strNewsCate = "", string txtDate = "", string strSubCate = "", bool isAdmin = false)
        {
            predicate = predicate.AndAlso(a => a.IsDeleted == false && a.ArticleType == 0);

            if (!string.IsNullOrEmpty(strTitle))
            {
                predicate = predicate.AndAlso(a => a.ArticleTitle.Contains(strTitle));
            }

            if (!string.IsNullOrEmpty(txtDate))
            {
                DateTime dateTime = Convert.ToDateTime(txtDate);
                DateTime dateAdd = dateTime.AddDays(1);
                predicate = predicate.AndAlso(a => a.PublishDate >= dateTime && a.PublishDate <= dateAdd);
            }

            if (!string.IsNullOrEmpty(strSubCate))
            {
                // ID变更为CategoryCode
                //var cateInfo = CommonService.lstCategory.FirstOrDefault(a => a.Id == Convert.ToInt32(strSubCate)
                //    && a.IsDeleted == false);

                //string categoryCode = cateInfo == null ? string.Empty : cateInfo.CategoryCode;
                //predicate = predicate.AndAlso(a => a.ArticleCateSub == strSubCate);
                //predicate = predicate.AndAlso(a => a.ArticleCateSub == categoryCode);
                var cateId = Convert.ToInt32(strSubCate);
                int? id = null;
                if (cateId != -1)
                {
                    id = cateId;
                }
                predicate = predicate.AndAlso(a => a.CategoryId == id);
            }

            //if (!isAdmin)
            //{
            //    var codes = (from item in CommonService.GetCategory(false).Where(a => !a.IsAdmin.Value).ToList() select item.CategoryCode).ToList();
            //    predicate = predicate.AndAlso(a => codes.Contains(a.ArticleCateSub) && !string.IsNullOrEmpty(a.ArticleCateSub));
            //}


            string appId = Request["AppId"];

            //TODO:remove
            if (!string.IsNullOrEmpty(appId))
            {
                int appid = int.Parse(appId);
                predicate = predicate.AndAlso(a => a.AppId == appid);
            }

            //if (!isAdmin)
            //{
            //    // 非系统管理员方式，过滤appid，并且isadmin要为false
            //    predicate = predicate.AndAlso(a => a.AppId == AppId);

            //    predicate = predicate.And(a => a.ArticleCateSub == CommonService.lstCategory.FirstOrDefault(b => b.CategoryCode == a.ArticleCateSub && b.IsAdmin == false).CategoryCode);
            //}
            //else
            //{
            //    // 系统管理员方式，不过滤appid，并且isadmin要为true
            //    predicate = predicate.AndAlso(a => a.ArticleCateSub == CommonService.lstCategory.FirstOrDefault(b => b.CategoryCode == a.ArticleCateSub && b.IsAdmin == true).CategoryCode);
            //}

            //  ConPage.SortConditions.Add(new SortCondition("CreatedDate", System.ComponentModel.ListSortDirection.Descending));

            //var q = _BaseService.GetList<ArticleInfoView>(predicate, ConPage);
            var q = _articleInfoService.GetList<ArticleInfoView>(predicate, ConPage);

            //TODO:


            if (!string.IsNullOrEmpty(appId) && int.TryParse(appId, out AppId))
            {
                // CategoryCode转变换CategoryName
                var lstCate = CommonService.GetCategory((AppId), false);

                q = ChangeCategoryCodeToCategoryName(q, lstCate);


                string wxDetailUrlPrefix = string.Empty;
                var accountManagement = WeChatCommonService.GetAccountManageByWeChatID(AppId);
                if (accountManagement != null)
                {
                    bool isCrop = accountManagement.AccountType == 0;
                    var wechatBaseUrl = isCrop ? CommonService.GetSysConfig("WeChatUrl", "") : CommonService.GetSysConfig("Content Server", "");
                    if (!string.IsNullOrWhiteSpace(wechatBaseUrl))
                    {
                        if (wechatBaseUrl.EndsWith("/"))
                        {
                            wechatBaseUrl = wechatBaseUrl.Substring(0, wechatBaseUrl.Length - 1);
                        }
                        wxDetailUrlPrefix = wechatBaseUrl + (isCrop ? "/News/ArticleInfo/WxDetail/{0}?wechatid={1}&isPreview=false" : "/MPNews/ArticleInfo/WxDetail/{0}?wechatid={1}&isPreview=false");
                    }
                }
                if (!string.IsNullOrEmpty(wxDetailUrlPrefix))
                {
                    q.ForEach(a => a.WxDetailUrl = string.Format(wxDetailUrlPrefix, a.Id, a.AppId));
                }
            }

            return q;
        }

        /// <summary>
        /// CategoryCode转变换CategoryName
        /// </summary>
        /// <param name="articleList">待修正的图文消息列表</param>
        /// <param name="cateList">标签列表</param>
        /// <returns>修正后的图文消息列表</returns>
        private List<ArticleInfoView> ChangeCategoryCodeToCategoryName(List<ArticleInfoView> articleList, List<Category> cateList)
        {
            List<ArticleInfoView> list = new List<ArticleInfoView>();

            if (articleList == null || articleList.Count == 0 ||
                cateList == null || cateList.Count == 0)
            {
                return articleList;
            }

            foreach (ArticleInfoView article in articleList)
            {
                Category category = cateList.Where(x => article.CategoryId.HasValue &&
                    x.Id == article.CategoryId.Value).Distinct().FirstOrDefault();
                article.CategoryName = category == null ? "其他" : category.CategoryName;

                list.Add(article);
            }

            return list;
        }

        public static bool IsAdminCategory(string categoryCode)
        {
            return CommonService.lstCategory.FirstOrDefault(a => a.CategoryCode == categoryCode && !a.IsAdmin.Value) == null;
        }

        //BackEnd校验
        public override bool BeforeAddOrUpdate(ArticleInfoView objModal, string Id)
        {
            bool validate = true;
            StringBuilder errMsg = new StringBuilder();

            if (string.IsNullOrEmpty(objModal.ArticleURL))
            {
                if (string.IsNullOrEmpty(objModal.ArticleContent))
                {
                    validate = false;
                    errMsg.Append(T("请输入新闻内容或者原文链接<br/>"));
                }
            }
            else
            {
                Regex reg = new Regex(@"^((https|http)?:\/\/)[^\s]+");
                if (!reg.IsMatch(objModal.ArticleURL))
                {
                    validate = false;
                    errMsg.Append(T("请输入完整的原文链接, 以http://或https://开头。<br/>"));
                }
            }

            if (string.IsNullOrEmpty(objModal.ArticleTitle))
            {
                validate = false;
                errMsg.Append(T("请输入标题。<br/>"));
            }

            if (string.IsNullOrEmpty(objModal.ArticleComment))
            {
                //validate = false;
                //errMsg.Append(T("请输入描述。<br/>"));
            }

            if (!string.IsNullOrEmpty(objModal.ArticleComment) && objModal.ArticleComment.Length > 120)
            {
                validate = false;
                errMsg.Append(T("描述内容不可超过120个字。<br/>"));
            }

            if (objModal.ImageCoverUrl == null)
            {
                validate = false;
                errMsg.Append(T("请上传图片<br/>"));
            }

            //if (string.IsNullOrEmpty(objModal.ArticleCateSub))
            //{
            //    validate = false;
            //    errMsg.Append(T("Please input Category.<br/>"));
            //}

            if (!validate)
            {
                ModelState.AddModelError("无效输入", errMsg.ToString());
            }

            return validate;
        }

        //Post方法
        [HttpPost]
        [ValidateInput(false)]
        public override JsonResult Post(ArticleInfoView objModal, string Id)
        {
            //trim.防止用户出现操作失误
            //if (objModal.Previewers != null)
            //{
            //    objModal.Previewers = objModal.Previewers.Trim();
            //}

            if (!BeforeAddOrUpdate(objModal, Id) || !ModelState.IsValid)
            {
                return Json(GetErrorJson(), JsonRequestBehavior.AllowGet);
            }

            // ID变更为CategoryCode
            //        if (objModal.CategoryId.HasValue)
            //        {
            //            var cateInfo = CommonService.lstCategory.FirstOrDefault(a => a.Id == Convert.ToInt32(objModal.CategoryId)
            //&& a.IsDeleted == false);
            //            objModal.ArticleCateSub = cateInfo.CategoryCode;
            //        }
            //        else
            //        {
            //            //objModal.ArticleCateSub = string.Empty;
            //            objModal.CategoryId = null;
            //        }
            //if (cateInfo == null)
            //{
            //    return Json(GetErrorJson(), JsonRequestBehavior.AllowGet);
            //}



            //string strPath = "/Content/file/" + DateTime.Now.ToString("yyyyMMdd") + "/";


            //if (!string.IsNullOrEmpty(objModal.ImageCoverUrl))
            //{
            //    string strSrc = objModal.ImageCoverUrl.Trim();
            //    if (strSrc.IndexOf("wechat.innoprise.cn") < 0)
            //    {
            //        var pad = strSrc.Split('.');
            //        string str = pad[pad.Length - 1];
            //        if (!System.IO.Directory.Exists(Request.PhysicalApplicationPath + strPath))
            //        {
            //            System.IO.Directory.CreateDirectory(Request.PhysicalApplicationPath + strPath);
            //        }

            //        string strFilePath = strPath + "C" + DateTime.Now.ToString("yyyyMMddHHmmss") + "." + str;

            //        FileStream m = new System.IO.FileStream(Request.PhysicalApplicationPath + strFilePath, System.IO.FileMode.Create);

            //        Innocellence.Weixin.HttpUtility.Get.Download(strSrc, m);
            //        m.Close();

            //        objModal.ImageCoverUrl = (CommonService.GetSysConfig("WeChatUrl", "") + strFilePath).Replace("//C", "/C");
            //    }
            //}



            //if (!string.IsNullOrEmpty(objModal.ArticleContent))
            //{
            //    var mcs = Regex.Matches(objModal.ArticleContent, "<img src=\"(.*?)\".*?/>");

            //    if (mcs.Count > 0)
            //    {

            //        int iIndex = 0;
            //        foreach(Match a in mcs)
            //        {

            //            string strSrc = a.Groups[1].Value;

            //            if (strSrc.IndexOf("wechat.innoprise.cn")>=0)
            //            {
            //                continue;
            //            }

            //            iIndex++;

            //            var pad = strSrc.Split('.');
            //            string str = pad[pad.Length - 1];
            //            if (!System.IO.Directory.Exists(Request.PhysicalApplicationPath + strPath))
            //            {
            //                System.IO.Directory.CreateDirectory(Request.PhysicalApplicationPath + strPath);
            //            }

            //            string strFilePath = strPath + DateTime.Now.ToString("yyyyMMddHHmmss") + iIndex.ToString() + "." + str;

            //            FileStream m = new System.IO.FileStream(Request.PhysicalApplicationPath + strFilePath, System.IO.FileMode.Create);

            //            Innocellence.Weixin.HttpUtility.Get.Download(strSrc, m);

            //            m.Close();


            //            objModal.ArticleContent = objModal.ArticleContent.Replace(strSrc, CommonService.GetSysConfig("WeChatUrl", "") + strFilePath).Replace("//C","/C");

            //        }

            //    }


            //   // Regex.Replace(objModal.ArticleContent, "<img src=\"(.*?)\".*?/>", "<img src=\"" + CommonService.GetSysConfig("WeChatUrl", "") + "\"$2/>");

            //}


            objModal.PreviewStartDate = DateTime.Now;

            InsertOrUpdate(objModal, Id);

            return Json(doJson(null), JsonRequestBehavior.AllowGet);
        }

        protected void InsertOrUpdate(ArticleInfoView objModal, string Id)
        {

            objModal.ArticleType = 0; //新闻类型
            if (objModal.CategoryId == -1)
            {
                objModal.CategoryId = null;
            }
            if (string.IsNullOrEmpty(Id) || Id == "0")
            {
                _BaseService.InsertView(objModal);
            }
            else
            {
                var lst = new List<string>() {
                    "ArticleTitle","ArticleComment",
                     "ArticleURL", "CategoryId",
                    "ArticleContent", "Previewers","AppId", "IsLike","IsTransmit","IsPassingWeChatUserID","ImageCoverUrl",
                    "SecurityLevel","ShowComment","ShowReadCount","IsWatermark","NoShare","PreviewStartDate" };
                _BaseService.UpdateView(objModal, lst);
            }
        }

        protected void doFile(ArticleInfoView objModal, ArticleContentView obj, int i)
        {
            if (!string.IsNullOrEmpty(objModal.ImgUrl[i]))
            {

                if (objModal.ImgUrl[i] != objModal.ImgUrl_Old[i])
                {
                    var objImg = new ArticleImages();

                    string strPath = Request.PhysicalApplicationPath + objModal.ImgUrl[i];
                    objImg.ImageContent = System.IO.File.ReadAllBytes(strPath);
                    //objModal.ImageContent_T = System.IO.File.ReadAllBytes(Request.PhysicalApplicationPath + strImg.Replace(".", "_T."));
                    objImg.ImageName = System.IO.Path.GetFileName(strPath);

                    obj.ImageUrl = objImg.ImageName;
                    obj.objImage = objImg;
                }
                else if (!string.IsNullOrEmpty(objModal.ImgID[i]))
                {
                    string strPath = Request.PhysicalApplicationPath + objModal.ImgUrl[i];
                    obj.ImageUrl = System.IO.Path.GetFileName(strPath);
                    obj.ImageID = int.Parse(objModal.ImgID[i]);
                }

            }

        }

        protected virtual void PublishByRole(int appid, Article article, ArticleInfoView articleInfoView)
        {
            var category = CommonService.lstCategory.Find(r => r.Id == articleInfoView.CategoryId);//.Role.Replace(',', '|');

            if (category == null || string.IsNullOrEmpty(category.Role) || category.Role == "@all")
            {
                // 如果role里为空或者为@all，就是发给所有人的
                WechatCommon.PublishMessage(appid, new List<ArticleInfoView>() { articleInfoView }, "@all", "", "");
            }
            else
            {
                //WeChatCommonService.lstTag
                var roles = category.Role.Split(',');

                var tagIds = WeChatCommonService.lstTag(AccountManageID).Where(x => roles.Any(y => y == x.tagname)).Select(x => x.tagid);
                // 如果role里不是@all，就是发给指定人的，特指标签组
                WechatCommon.PublishMessage(appid, new List<ArticleInfoView>() { articleInfoView }, "", "", string.Join("|", tagIds));
            }
        }

        /// <summary>
        /// 发布文章
        /// </summary>
        /// <param name="Id">文章ID</param>
        /// <param name="appid">当前应用ID</param>
        /// <param name="ispush">是否推送到前台</param>
        /// <returns></returns>
        public JsonResult ChangeStatus(string Id, int appid, bool ispush)
        {
            int iRet = 0;

            var objModel = _BaseService.Repository.GetByKey(int.Parse(Id));

            if (objModel.ArticleStatus == null || objModel.ArticleStatus == ConstData.STATUS_NEW)
            {
                objModel.ArticleStatus = ConstData.STATUS_PUBLISH;
                objModel.PublishDate = DateTime.Now;

                //向前台推送提醒信息
                ArticleInfoView aiv = (ArticleInfoView)new ArticleInfoView().ConvertAPIModel(objModel);
                LogManager.GetLogger(this.GetType()).Debug(string.Format("ImageUrl = '{0}', ArticleUrl='{1}'", aiv.ThumbImageUrl, aiv.ArticleURL));

                if (ispush)
                {
                    // 新版本下，推送图片放在ImageUrl里
                    var article = new Article()
                    {
                        Title = objModel.ArticleTitle,
                        Description = objModel.ArticleComment,
                        // PicUrl = aiv.ThumbImageId == null ? wechatBaseUrl+"/Content/img/LogoRed.png" : string.Format("{0}/Common/PushFile?id={1}&FileName={2}", wechatBaseUrl, aiv.ThumbImageId, aiv.ThumbImageUrl),
                        PicUrl = aiv.ImageCoverUrl == null ? wechatBaseUrl + "Content/img/LogoRed.png" : string.Format("{0}{1}", wechatBaseUrl, aiv.ImageCoverUrl),
                        Url = string.Format("{0}/News/ArticleInfo/WxDetail/{1}?wechatid={2}", wechatBaseUrl, objModel.Id, objModel.AppId)
                    };

                    PublishByRole(appid, article, aiv);
                }
            }
            else
            {
                objModel.ArticleStatus = ConstData.STATUS_NEW;
                objModel.PublishDate = null;
            }

            iRet = _BaseService.Repository.Update(objModel, new List<string>() { "ArticleStatus", "PublishDate" });

            if (iRet > 0)
            {
                return SuccessNotification("Success");
            }
            else
            {
                return ErrorNotification("Change Status Falid");
            }

        }

        /// <summary>
        /// 批量发布文章
        /// </summary>
        /// <param name="Ids">文章的ID-数组</param>
        /// <param name="appid">当前应用ID</param>
        /// <returns></returns>
        public JsonResult ChangeStatusBatch(string[] Ids, bool isNotification)
        {
            int iRet = 0;
            List<ArticleInfoView> articleList = new List<ArticleInfoView>();
            var dateNow = DateTime.Now;
            var appid = 0;

            for (int i = 0; i < Ids.Length; i++)
            {
                var objModel = _BaseService.Repository.GetByKey(int.Parse(Ids[i]));
                appid = objModel.AppId.GetValueOrDefault();

                if (objModel.ArticleStatus == null || objModel.ArticleStatus == ConstData.STATUS_NEW)
                {
                    objModel.ArticleStatus = ConstData.STATUS_PUBLISH;
                    objModel.PublishDate = dateNow.AddSeconds(Ids.Length - i);//更新时间越加越少，这样倒序排才能与后台顺序一致

                    //向前台推送提醒信息
                    ArticleInfoView aiv = (ArticleInfoView)new ArticleInfoView().ConvertAPIModel(objModel);
                    LogManager.GetLogger(this.GetType()).Debug(string.Format("ImageUrl = '{0}', ArticleUrl='{1}'", aiv.ThumbImageUrl, aiv.ArticleURL));

                    //var article = new Article()
                    //{
                    //    Title = objModel.ArticleTitle,
                    //    Description = objModel.ArticleComment,
                    //   // PicUrl = aiv.ThumbImageId == null ? "http://wechatcms.XXXadmin.cn/Content/img/XXXLogoRed.png" : string.Format("{0}/Common/PushFile?id={1}&FileName={2}", wechatBaseUrl, aiv.ThumbImageId, aiv.ThumbImageUrl),
                    //    PicUrl = aiv.ImageCoverUrl == null ? wechatBaseUrl+"Content/img/LogoRed.png" : string.Format("{0}{1}", wechatBaseUrl, aiv.ImageCoverUrl),
                    //   Url = string.Format("{0}/News/ArticleInfo/WxDetail/{1}?wechatid={2}", wechatBaseUrl, objModel.Id,objModel.AppId)
                    //};
                    //添加到文章列表中去
                    articleList.Add(aiv);
                }
                else
                {
                    objModel.ArticleStatus = ConstData.STATUS_NEW;
                    objModel.PublishDate = null;
                }

                iRet = _BaseService.Repository.Update(objModel, new List<string>() { "ArticleStatus", "PublishDate" });
            }

            if (articleList.Any() && isNotification)
            {
                //最后将文章列表推送--批量默认推送给全部人
                WechatCommon.PublishMessage(appid, articleList, "@all", "", "");
            }

            if (iRet > 0)
            {
                return SuccessNotification("Success");
            }
            else
            {
                return ErrorNotification("Change Status Falid");
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public virtual JsonResult WxPreview(ArticleInfoView objModal)
        {
            //从post过来的form拿数据
            string Id = Request.Form["ID"];

            //验证错误
            if (!BeforeAddOrUpdate(objModal, Id) || !ModelState.IsValid)
            {
                return Json(GetErrorJson(), JsonRequestBehavior.AllowGet);
            }
            objModal.ArticleStatus = "Saved";
            objModal.PreviewStartDate = DateTime.Now;
            InsertOrUpdate(objModal, Id);

            //向前台推送提醒信息
            //var article = new Article()
            //{
            //    Title = "[预览]" + objModal.ArticleTitle,
            //    Description = objModal.ArticleComment,
            // //   PicUrl = string.Format("{0}/Common/PushFile?id={1}&FileName={2}", wechatBaseUrl, objModal.ThumbImageId, objModal.ThumbImageUrl),
            //    PicUrl = objModal.ImageCoverUrl == null ? wechatBaseUrl + "Content/img/LogoRed.png" : string.Format("{0}{1}", wechatBaseUrl, objModal.ImageCoverUrl),
            //    Url = string.Format("{0}/News/ArticleInfo/WxPreview/{1}?wechatid={2}", wechatBaseUrl, objModal.Id,objModal.AppId)
            //};


            

            //为了fix掉新建文章预览 微信接口报错后 再save 出现2条数据
            try
            {
                if (objModal.IsCorp == 1) //企业号
                {

                    //var Users = objModal.ToUser.Split(',').ToList();

                    //IAddressBookService uService = EngineContext.Current.Resolve<IAddressBookService>();
                    //var list = uService.Repository.Entities.Where(a => Users.Contains(a.EmployeeNo) && a.Status==0).ToList();
                    //if (list == null || list.Count==0)
                    //{
                    //    return ErrorNotification("用户不存在或者未关注！");
                    //}

                    string personStr = objModal.PersonList != null && objModal.PersonList.Count > 0 ? string.Join("|", objModal.PersonList) : string.Empty;
                    string groupStr = objModal.GroupList != null && objModal.GroupList.Count > 0 ? string.Join("|", objModal.GroupList) : string.Empty;
                    string tagStr = objModal.TagList != null && objModal.TagList.Count > 0 ? string.Join("|", objModal.TagList) : string.Empty;
                    objModal.ToUser = personStr.Replace("|", ",");
                    objModal.ToDepartment = groupStr.Replace("|", ",");
                    objModal.ToTag = tagStr.Replace("|", ",");
                    _BaseService.UpdateView(objModal, new List<string> { "ToUser", "ToDepartment", "ToTag" });
                    WechatCommon.PublishMessage(objModal.AppId.GetValueOrDefault(), new List<ArticleInfoView>() { objModal }, personStr, groupStr, tagStr, true);
                }
                else //服务号
                {
                    var wechat = WeChatCommonService.GetWeChatConfigByID(objModal.AppId.GetValueOrDefault());
                    string strSendType = string.Empty;
                    SearchUserMPView searchCondition = new SearchUserMPView
                    {
                        Group = objModal.Group,
                        Province = objModal.Province,
                        City = objModal.City,
                        Sex = objModal.Sex
                    };

                    if (objModal.Group == null)
                    {
                        objModal.Group = -2;
                    }

                    if (objModal.Sex < 0 && objModal.Province == "-1")
                    {
                        if (objModal.Group == -2)
                        {
                            // ToAll
                            strSendType = "ToAll";
                        }
                        else
                        {
                            // Send By Tag
                            strSendType = "ByTag";
                        }
                    }
                    else
                    {
                        // Send By OpenId
                        strSendType = "ByOpenId";
                    }
                    var ret0 = Innocellence.Weixin.MP.AdvancedAPIs.MediaApi.UploadTemporaryMedia(wechat.WeixinAppId, wechat.WeixinCorpSecret, Weixin.MP.UploadMediaFileType.thumb, System.Web.HttpContext.Current.Server.MapPath("/" + objModal.ImageCoverUrl.Insert(objModal.ImageCoverUrl.LastIndexOf('.'), "_T")));
                    //  var configs = Infrastructure.Web.Domain.Service.CommonService.lstSysConfig;
                    //  var config = configs.Where(a => a.ConfigName.Equals("Content Server", StringComparison.CurrentCultureIgnoreCase)).First();
                    string host = Infrastructure.Web.Domain.Service.CommonService.GetSysConfig("Content Server", "");// config.ConfigValue;
                    if (host.EndsWith("/"))
                    {
                        host = host.Substring(0, host.Length - 1);
                    }
                    var article = new NewsModel()
                    {
                        content = objModal.ArticleContent,

                        title = objModal.ArticleTitle,
                        digest = objModal.ArticleComment,
                        thumb_media_id = ret0.thumb_media_id,
                        thumb_url = host + objModal.ImageCoverUrl,
                        content_source_url = string.Format("{0}/MPNews/ArticleInfo/WxDetail/{1}?wechatid={2}&isPreview={3}", wechatBaseUrl, objModal.Id, objModal.AppId, true)

                    };


                    var openIds = objModal.PersonList;

                    //IWechatMPUserService uService = EngineContext.Current.Resolve<IWechatMPUserService>();
                    //var list= uService.Repository.Entities.Where(a => Users.Contains(a.Id)).ToList();
                    //if (list == null || list.Count == 0)
                    //{
                    //    return ErrorNotification("用户不存在或者未关注！");
                    //}

                    var ret = Innocellence.Weixin.MP.AdvancedAPIs.MediaApi.UploadTemporaryNews(wechat.WeixinAppId, wechat.WeixinCorpSecret, 10000, new NewsModel[] { article });
                    // MPSendMessage(strSendType, wechat, objModal.Group.Value.ToString(), ret.media_id, searchCondition, Weixin.MP.GroupMessageType.mpnews);

                    string[] userOpenIds = _WechatMPUserService.GetUserBySearchCondition(searchCondition, AccountManageID).Select(u => u.OpenId).ToArray();

                    foreach (var openId in openIds)
                    {
                        Innocellence.Weixin.MP.AdvancedAPIs.GroupMessageApi.SendGroupMessagePreview(wechat.WeixinAppId,
                       wechat.WeixinCorpSecret, Weixin.MP.GroupMessageType.mpnews, ret.media_id, openId);
                    }



                }
            }
            catch (Exception ex)
            {
                return SuccessNotification(string.Format("{0};{1}", objModal.Id, ex.Message));
            }

            return SuccessNotification(string.Format("{0}", objModal.Id));
        }

        private SendResult MPSendMessage(string SendType, SysWechatConfig wechat, string groupId, string value, SearchUserMPView searchCondition, Weixin.MP.GroupMessageType type)
        {
            SendResult returnResult = null;
            switch (SendType)
            {
                case "ToAll":
                    returnResult = Innocellence.Weixin.MP.AdvancedAPIs.GroupMessageApi.SendGroupMessageByGroupId(wechat.WeixinAppId, wechat.WeixinCorpSecret, groupId, value, type, true, false, 1000);
                    break;
                case "ByTag":
                    returnResult = Innocellence.Weixin.MP.AdvancedAPIs.GroupMessageApi.SendGroupMessageByGroupId(wechat.WeixinAppId, wechat.WeixinCorpSecret, groupId, value, type, false, false, 1000);
                    break;
                case "ByOpenId":
                    string[] userOpenIds = _WechatMPUserService.GetUserBySearchCondition(searchCondition, AccountManageID).Select(u => u.OpenId).ToArray();
                    returnResult = Innocellence.Weixin.MP.AdvancedAPIs.GroupMessageApi.SendGroupMessageByOpenId(wechat.WeixinAppId, wechat.WeixinCorpSecret, type, value, 1000, userOpenIds);
                    break;
            }
            return returnResult;

        }

        [HttpGet]
        public virtual ActionResult getTreeData(int? appId)
        {
            var list = CommonService.GetCategory(appId.Value, false).OrderBy(a => a.CategoryOrder).Where(a => a.IsAdmin == null || !a.IsAdmin.Value).ToList();
            list.Add(new Category() { AppId = appId.Value, Id = -1, CategoryName = "其他", ParentCode = 0 });
            var listReturn = EasyUITreeData.GetTreeData(list, "Id", "CategoryName", "ParentCode");

            return Json(listReturn, JsonRequestBehavior.AllowGet);

        }

        public override ActionResult Export()
        {
            string strTitle = Request["txtArticleTitle"];
            string txtDate = Request["txtDate"];
            string strSubCate = Request["txtSubCate"];

            if ((CategoryType)AppId != CategoryType.Undefined) return ExportCSV(strTitle, txtDate, strSubCate);

            int appid;
            if (int.TryParse(Request["appId"], out appid))
            {
                AppId = appid;
            }

            return ExportCSV(strTitle, txtDate, strSubCate);
        }

        /// <summary>
        /// CSV文件出力
        /// </summary>
        /// <param name="title">新闻标题</param>
        /// <param name="pulishDate">发布时间</param>
        /// <param name="categoryId">标签ID</param>
        /// <returns></returns>
        public ActionResult ExportCSV(string title, string pulishDate, string categoryId)
        {
            Expression<Func<ArticleInfo, bool>> predicate;
            if ((CategoryType)AppId == CategoryType.Undefined)
            {
                predicate = x => x.IsDeleted == false;
            }
            else
            {
                predicate = x => x.AppId == AppId && x.IsDeleted == false;
            }

            if (!string.IsNullOrEmpty(title))
            {
                title = title.Trim().ToLower();
                predicate = predicate.AndAlso(a => a.ArticleTitle.ToLower().Contains(title));
            }

            if (!string.IsNullOrEmpty(pulishDate))
            {
                DateTime condDate = Convert.ToDateTime(pulishDate);
                DateTime condDateEnd = condDate.AddDays(1);
                predicate = predicate.AndAlso(x => x.PublishDate >= condDate && x.PublishDate <= condDateEnd);
            }

            if (!string.IsNullOrEmpty(categoryId))
            {
                // ID变更为CategoryCode
                //var cateInfo = CommonService.lstCategory.FirstOrDefault(a => a.Id == cate
                //    && a.IsDeleted == false);

                //string categoryCode = cateInfo == null ? string.Empty : cateInfo.CategoryCode;
                int? cate = null;
                if (Convert.ToInt32(categoryId) != -1)
                {
                    cate = Convert.ToInt32(categoryId);
                }
                predicate = predicate.AndAlso(a => a.CategoryId == cate);

            }
            //过滤掉isadmin的数据
            //var codes = (from item in CommonService.GetCategory(false).Where(a => !a.IsAdmin.Value).ToList() select item.CategoryCode).ToList();
            //predicate = predicate.AndAlso(a => codes.Contains(a.ArticleCateSub) && !string.IsNullOrEmpty(a.ArticleCateSub));
            // APP列表
            var appInfo = WeChatCommonService.lstSysWeChatConfig.FirstOrDefault(a => a.Id == AppId);
            // 当前APP下Category列表
            //var cateList = CommonService.lstCategory.Where(a => a.AppId == AppId
            //        && a.IsDeleted == false);


            predicate = predicate.AndAlso(a => a.ArticleType == 0);

            List<ArticleInfoView> reportList = new List<ArticleInfoView>();

            reportList = _articleInfoService.GetList<ArticleInfoView>(predicate).OrderByDescending(x => x.PublishDate).ThenByDescending(x => x.Id).ToList();

            //var ids = reportList.Select(x => x.Id).ToList();
            //var lst=_articleThumbsUpService.Repository.Entities.Where(x => ids.Contains(x.ArticleID)
            //                                                       && x.IsDeleted != true &&
            //                                                       x.Type==ThumbupType.Article.ToString()).Select(x=>x.ArticleID).ToList();
            //reportList.ForEach(x =>
            //{
            //    x.ThumbsUpCount = lst.Count(z => z == x.Id);
            //});

            if (appInfo != null)
            {
                //foreach (ArticleInfoView item in reportList)
                //{
                //    item.APPName = appInfo.AppName;

                //}
                reportList.ForEach(item =>
                {
                    item.APPName = appInfo.AppName;
                });
            }

            return exportToCSV(reportList);
        }

        private ActionResult exportToCSV(List<ArticleInfoView> wechatFollowReportList)
        {
            string[] headLine = { "Id", "ArticleTitle", "APPName", "ArticleCateName", "ReadCount", "ThumbsUpCount", "PublishDate" };
            CsvSerializer<ArticleInfoView> csv = new CsvSerializer<ArticleInfoView>();
            csv.UseLineNumbers = false;
            var sRet = csv.SerializeStream(wechatFollowReportList, headLine);

            string fileHeadName = ((CategoryType)AppId).ToString();

            string fileName = fileHeadName + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".csv";
            return File(sRet, "text/comma-separated-values", fileName);
        }

        #region 图文编辑页面 canvas获取视频base64截图转图片存储
        [HttpPost]
        public JsonResult GetPosterUrlList(List<PosterUrl> posterUrlList)
        {
            try
            {
                if (posterUrlList.Any())
                {
                    foreach (var poter in posterUrlList)
                    {
                        poter.filePath = SaveBase64ImageToServer(poter.posterBase);
                        poter.posterBase = "";
                    }

                }
            }
            catch (Exception e)
            {
                _Logger.Debug(e);
                throw;
            }

            return Json(posterUrlList);
        }

        private string SaveBase64ImageToServer(string imageContent)
        {
            string filePath = string.Empty;
            try
            {
                if (!string.IsNullOrWhiteSpace(imageContent))
                {
                    string startStr = "data:image/png;base64,";
                    if (imageContent.StartsWith(startStr))
                    {
                        string temp;
                        filePath = WeChatUserRequestMessageLogHandler.GetFilePath(".png", out temp);
                        byte[] arr = Convert.FromBase64String(imageContent.Replace(startStr, ""));
                        MemoryStream ms = new MemoryStream(arr);
                        Bitmap bmp = new Bitmap(ms);
                        bmp.Save(filePath);
                        filePath = temp.Trim('/');
                    }
                    else
                    {
                        filePath = imageContent;
                    }
                }
                _Logger.Debug("base64 image file path :{0}", filePath);
                return filePath;
            }
            catch (Exception ex)
            {
                _Logger.Error("Base64StringToImage 转换失败\nException：" + ex.Message);
            }
            return filePath;
        }
        public class PosterUrl
        {
            public string fileId { get; set; }
            public string posterBase { get; set; }
            public string filePath { get; set; }
        }
        #endregion
    }
}
