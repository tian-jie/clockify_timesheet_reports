/*----------------------------------------------------------------
    Copyright (C) 2015 Innocellence
    
    文件名：MenuController.cs
    文件功能描述：自定义菜单设置工具Controller
    
    
    创建标识：Innocellence - 20150312
----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Autofac;
using Infrastructure.Core.Caching;
using Infrastructure.Core.Infrastructure;
using Infrastructure.Web.Domain.Services;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Service;
using Innocellence.WeChat.Domain.Services;
using Innocellence.Weixin.QY;
using Innocellence.Weixin.QY.CommonAPIs;

using Innocellence.Weixin.QY.AdvancedAPIs.MailList;
using Infrastructure.Web.UI;
using Innocellence.Weixin.QY.AdvancedAPIs.Media;
using System.IO;
using Infrastructure.Web.Domain.Contracts;
//using Innocellence.WeChat.Domain.Common;
using Innocellence.Weixin.QY.AdvancedAPIs.App;
using Infrastructure.Web.Domain.Service;
using Infrastructure.Web.Domain.Entity;
using Infrastructure.Core.Data;
using Infrastructure.Web.Domain.ModelsView;
using System.Text;
using Innocellence.Weixin.QY.AdvancedAPIs;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.Common;
using Infrastructure.Utility.Data;
using Innocellence.WeChat.Domain.Contracts.ViewModel;
using Newtonsoft.Json;
using Innocellence.WeChat.Domain.ViewModel;
using Innocellence.WeChat.Domain;

namespace Innocellence.WeChatMain.Controllers
{
    public class AppManageController : BaseController<Category, CategoryView>
    {

        private static ICacheManager cacheManager = EngineContext.Current.Resolve<ICacheManager>(new TypedParameter(typeof(Type), typeof(WeChatCommonService)));
        BaseService<SysWechatConfig> sys = new BaseService<SysWechatConfig>();
        private ISysMenuService _sysMenuService = new SysMenuService();
        private ISysMenuServiceEx _sysMenuServiceEx = new SysMenuServiceEx();
        private IAccountManageService _accountManageService = new AccountManageService();
        private ISysWechatConfigService _sysWechatConfigService = new SysWechatConfigService();

        public AppManageController(ICategoryService newsService, ISysWechatConfigService sysWechatConfigService)
            : base(newsService)
        {
            _sysWechatConfigService = sysWechatConfigService;
        }

        //
        // GET: /Menu/
        public override ActionResult Index()
        {

            return View();
        }

        public ActionResult GetAppList()
        {
            //获取app的list, 这个是select控件所用
            var lst = WeChatCommonService.lstSysWeChatConfig.Where(a => a.AccountManageId == AccountManageID).ToList();

            var app = new Dictionary<int, string>();
            foreach (var rst in lst)
            {
                // 绑定的微信ID
                app.Add(int.Parse(rst.WeixinAppId), rst.AppName);
            }
            ViewBag.AList = app;
            return View();
        }

        public ActionResult GetAppInfoList( bool? isSyncMenu)
        {
            var param = new PageParameter();
            TryUpdateModel(param);
            //实现对用户和多条件的分页的查询，rows表示一共多少条，page表示当前第几页
            param.length = param.length == 0 ? 10 : param.length;
            var iTotal = param.iRecordsTotal;

            // TODO：从Session中取得AccountManageId，这个值应该是刚已进入管理平台时就选择的

            // 取得改账号下所有的App，并跟管理平台同步
            var allApp = GetAndSyncApps(AccountManageID, isSyncMenu);// _newsService.GetList(null, null, 10, 10);

            // 取当前页
            var list = new List<GetAppInfoResult>();
            var page = (int)Math.Floor(param.start / param.length * 1.0d) + 1;
            if (allApp.Count != 0)
            {
                iTotal = allApp.Count();

                list = allApp.OrderBy(a => int.Parse(a.agentid)).Skip((page - 1) * param.length).Take(param.length).ToList();
            }

            var t = from a in list
                    select new
                    {
                        Id = int.Parse(a.agentid),
                        APPName = a.name,
                        Appsquare_logo = a.square_logo_url,
                        round_logo = a.round_logo_url,
                        Description = a.description,
                        Allow_userinfo = a.allow_userinfos,
                        Allow_partys = a.allow_partys,
                        Allow_tags = a.allow_tags,
                        Close = a.close,
                        Redirect_domain = a.redirect_domain,
                        Location_flag = a.report_location_flag,
                        IgnoreDeviceFilterElementsreportuser = a.isreportuser,
                        Isreportenter = a.isreportenter
                    };

            return Json(new
            {
                sEcho = param.draw,
                iTotalRecords = iTotal,
                iTotalDisplayRecords = iTotal,
                aaData = t
            }, JsonRequestBehavior.AllowGet);

        }


        ///// <summary>
        ///// 获取所有App信息
        ///// </summary>
        ///// <param name="appid"></param>
        ///// <returns></returns>

        /// <summary>
        /// 取得并同步一个微信号（企业号）下所有App信息
        /// </summary>
        /// <param name="accountManageId"></param>
        /// <returns></returns>
        public List<GetAppInfoResult> GetAndSyncApps(int accountManageId,bool? isSyncMenu)
        {
            var result = new List<GetAppInfoResult>();
            var appSelected = Request["APPList"];

            // ------------从微信服务器获取所有的App，以此为基础同步我们的App管理表

            // 取得accessToken

          var config=  WeChatCommonService.lstSysWeChatConfig.FirstOrDefault(a => a.AccountManageId == accountManageId);

            var accessToken = WeChatCommonService.GetWeiXinToken(config.Id);

            // 如果是搜索App
            if (!string.IsNullOrEmpty(appSelected))
            {
                var app = WeChatCommonService.lstSysWeChatConfig.FirstOrDefault(a => a.AccountManageId == accountManageId && a.WeixinAppId == appSelected);

               
                var appInfo = AppApi.GetAppInfo(accessToken, int.Parse(appSelected));
                // 取得平台内AppId
               
                appInfo.agentid = app.Id.ToString();
                result.Add(appInfo);

                return result;
            }
            var wxApps = AppApi.GetAppList(accessToken);
            var applist = WeChatCommonService.lstSysWeChatConfig.Where(a => a.AccountManageId == accountManageId).ToList();

            // 同步App
            // TODO：目前的效率不高，在每次刷新页面或者翻页时都会重新去同步App
            _sysWechatConfigService.SyncWechatApps(accountManageId, accessToken, wxApps.agentlist, applist, isSyncMenu??true);

            // 将Session中的Menu更新
            var loginUser = (SysUser)Session["UserInfo"];
            var newMenus = _sysMenuService.GetMenusByUserID(loginUser, null);
            loginUser.Menus = newMenus;
            Session["UserInfo"] = loginUser;

            // 返回List
            var newAppList = WeChatCommonService.lstSysWeChatConfig.Where(a => a.AccountManageId == accountManageId).ToList();

            //这段代码完全没必要了
            //foreach (var newApp in newAppList)
            //{
            //    try
            //    {
            //        var appInfo = AppApi.GetAppInfo(accessToken, int.Parse(newApp.WeixinAppId));
            //        // 前台需要的是平台的AppID，而不是微信的agentId
            //        // 所以此处借用agentid字段来存储appId（因为类GetAppInfoResult已经打包为dll作为底层了,修改的话势必可能会影响到别的地方）
            //        appInfo.agentid = newApp.Id.ToString();
            //        result.Add(appInfo);
            //    }
            //    catch
            //    {
            //    }
            //}

            return result;
        }

        public override ActionResult Edit(string id)
        {
            ViewBag.AppId = id;
            var Config = WeChatCommonService.GetWeChatConfigByID(int.Parse(id));


            //string strToken = Rtntoken(int.Parse(id));
            var strToken = WeChatCommonService.GetWeiXinToken(Config.Id);
            GetAppInfoResult result = AppApi.GetAppInfo(strToken, int.Parse(Config.WeixinAppId));
            ViewBag.result = result;

            //部门信息
            //List<DepartmentList> subdepartList = MailListApi.GetDepartmentList(GetToken(), 0).department;
            List<DepartmentList> subdepartList = new List<DepartmentList>();
            if (WeChatCommonService.lstDepartment(AccountManageID) != null)
            {
                subdepartList = WeChatCommonService.lstDepartment(AccountManageID);
            }
            String allowpart = string.Join(",", result.allow_partys.partyid);
            List<string> departname = new List<string>();
            foreach (var part in allowpart.Split(','))
            {
                foreach (var allow in subdepartList)
                {
                    if (allow.id.ToString() == part)
                    {
                        departname.Add(allow.name);
                        break;
                    }
                }
            }
            ViewBag.depatlist = departname;

            //标签信息
            //var tagList = MailListApi.GetTagList(strToken).taglist;
            //ViewBag.taglist = tagList;
            List<TagItem> tagList = new List<TagItem>();
            if (WeChatCommonService.lstTag(AccountManageID) != null)
            {
                tagList = WeChatCommonService.lstTag(AccountManageID);
            }
            String allowtag = string.Join(",", result.allow_tags.tagid);
            List<string> tagname = new List<string>();
            foreach (var tag in allowtag.Split(','))
            {
                foreach (var tag_list in tagList)
                {
                    if (tag_list.tagid == tag)
                    {
                        tagname.Add(tag_list.tagname);
                        break;
                    }
                }
            }
            ViewBag.tagname = tagname;

            //用户信息

            List<GetMemberResult> user_List = new List<GetMemberResult>();

            if (WeChatCommonService.lstUser(AccountManageID) != null)
            {
                user_List = WeChatCommonService.lstUser(AccountManageID);
            }
            List<GetAppInfo_AllowUserInfos_User> userList = result.allow_userinfos.user;
            Dictionary<string, string> username = new Dictionary<string, string>();
            foreach (var user in userList)
            {
                foreach (var user1 in user_List)
                {
                    if (user1.userid == user.userid)
                    {
                        username.Add(user1.name, user1.avatar);

                        break;
                    }
                }
            }
            ViewBag.userlist = username;

            //获取corpid与secret

            if (Config != null)
            {
                ViewBag.corpid = Config.WeixinCorpId;
                ViewBag.secret = "******";//Config.WeixinCorpSecret;
                ViewBag.welcomemessage = Config.WelcomeMessage;
                ViewBag.configid = Config.Id;
                ViewBag.token = "******";// Config.WeixinToken;
                ViewBag.encodingAESKey = "******";// Config.WeixinEncodingAESKey;
            }
            return View("../appmanage/Edit", result);

        }
        //上传多媒体文件
        public JsonResult PostImage(string appid)
        {
            UploadForeverResultJson ret = new UploadForeverResultJson();

            var Config = WeChatCommonService.GetWeChatConfigByID(int.Parse(appid));

            if (Request.Files.Count > 0)
            {
                //var strToken = Rtntoken(int.Parse(appid));
                var strToken = WeChatCommonService.GetWeiXinToken(Config.Id);

                Dictionary<string, Stream> dic = new Dictionary<string, Stream>();
                HttpPostedFileBase objFile = Request.Files[0];
                var filename = objFile.FileName;
                var stream = objFile.InputStream;
                dic.Add(filename, stream);
                // ret = MediaApi.UploadForeverMedia(strToken, appid, UploadMediaFileType.image, dic, "");
                // ret = MediaApi.UploadPermanent(strToken, UploadMediaFileType.image, dic, "");
                ret = MediaApi.AddMaterial(strToken, UploadMediaFileType.image, dic, "");

            }
            return Json(ret, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 必填项验证
        /// </summary>
        /// <param name="name"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public bool CheckRequire(string name, string hint, out string errorMessage)
        {
            bool result = true;
            errorMessage = string.Empty;
            if (string.IsNullOrEmpty(name))
            {
                result = false;
                errorMessage = hint;
            }
            return result;
        }
        //验证输入字符数长度
        public bool CheckLength(string name, string hint, int length, out string errorMessage)
        {
            bool result = true;
            errorMessage = string.Empty;
            if (!string.IsNullOrEmpty(name) && name.Length > length)
            {
                result = false;
                errorMessage = hint;
            }
            return result;
        }
        public bool BeforeAddOrUpdate(SetAppPostData objModal, string ID)
        {
            //后台校验 Go here..
            bool validate = true;
            StringBuilder errMsg = new StringBuilder();
            string departId = Request["departmentAll"];
            string errorMessage = string.Empty;

            if (!CheckRequire(objModal.name, "应用名称长度为2-16个字符。<br/>", out errorMessage))
            {
                validate = false;
                errMsg.Append(T(errorMessage));
            }
            if (!CheckLength(objModal.name, "应用名称长度太长。<br/>", 16, out errorMessage))
            {
                validate = false;
                errMsg.Append(T(errorMessage));
            }
            if (!CheckRequire(objModal.description, "功能介绍长度为4-120个字。<br/>", out errorMessage))
            {
                validate = false;
                errMsg.Append(T(errorMessage));
            }
            if (!CheckLength(objModal.description, "功能介绍长度太长。<br/>", 120, out errorMessage))
            {
                validate = false;
                errMsg.Append(T(errorMessage));
            }
            if (!CheckLength(objModal.redirect_domain, "域名长度太长。<br/>", 50, out errorMessage))
            {
                validate = false;
                errMsg.Append(T(errorMessage));
            }

            if (!CheckRequire(Request["CorpID"], "请填写CorpID。<br/>", out errorMessage))
            {
                validate = false;
                errMsg.Append(T(errorMessage));
            }
            if (!CheckLength(Request["CorpID"], "CorpID长度太长。<br/>", 150, out errorMessage))
            {
                validate = false;
                errMsg.Append(T(errorMessage));
            }
            if (!CheckRequire(Request["Secret"], "请填写Secret。<br/>", out errorMessage))
            {
                validate = false;
                errMsg.Append(T(errorMessage));
            }
            if (!CheckLength(Request["Secret"], "Secret长度太长。<br/>", 250, out errorMessage))
            {
                validate = false;
                errMsg.Append(T(errorMessage));
            }
            if (!CheckLength(Request["Token"], "Token长度太长。<br/>", 100, out errorMessage))
            {
                validate = false;
                errMsg.Append(T(errorMessage));
            }
            if (!CheckLength(Request["EncodingAESKey"], "EncodingAESKey长度太长。<br/>", 100, out errorMessage))
            {
                validate = false;
                errMsg.Append(T(errorMessage));
            }
            if (!validate)
            {
                ModelState.AddModelError("Invalid Input", errMsg.ToString());
            }

            return validate;
        }

        /// <summary>
        /// 设置企业号应用
        /// </summary>
        /// <param name="data"></param>
        //[AllowAnonymous]
        //[HttpPost]
        public ActionResult SetAppInfo(SetAppPostData data, string Id)
        {
            //更新SysWechatConfig数据库
            //验证错误
            if (!BeforeAddOrUpdate(data, Id) || !ModelState.IsValid)
            {
                return Json(GetErrorJson(), JsonRequestBehavior.AllowGet);
            }
            #region 更新菜单
            var loginUser = (SysUser)Session["UserInfo"];
            var objModel = sys.Repository.GetByKey(int.Parse(Request["ConfigID"]));
            var menuModel = _sysMenuService.GetMenusByUserID(loginUser, null).Where(a => a.AppId == int.Parse(Request["ConfigID"]) && a.NavigateUrl.Equals("/")).FirstOrDefault();
            if (menuModel != null)
            {
                menuModel.MenuName = data.name;
                menuModel.MenuTitle = data.name;
                _sysMenuService.Repository.Update(menuModel);
            }
            #endregion

            var Config = WeChatCommonService.GetWeChatConfigByID(int.Parse(Request["ConfigID"]));

            #region 同步到微信
            //var strToken = Rtntoken(int.Parse(data.agentid));
            var strToken = WeChatCommonService.GetWeiXinToken(Config.Id);
            if (!string.IsNullOrEmpty(Request["MediaID"]))
            {
                data.logo_mediaid = Request["MediaID"];
            }
            if (string.IsNullOrEmpty(data.redirect_domain))
            {
                data.redirect_domain = string.Empty;
            }

            AppApi.SetApp(strToken, data);
            #endregion

            #region 存DB
            var objNewModel = new SysWechatConfig();
            #region 从微信获取App logo, 存入objNewModel
            try
            {
               
                GetAppInfoResult result = AppApi.GetAppInfo(strToken, int.Parse(Config.WeixinAppId));
                objNewModel.CoverUrl = result.square_logo_url;
            }
            catch (Exception ex)
            {
                _Logger.Error("an error occurd when get app logo :{0}", ex);
            }
            #endregion
            var lst = new List<string>();
            objNewModel.Id = objModel.Id;
            objNewModel.AppName = data.name;

            if (Request["CorpID"] != objModel.WeixinCorpId && !string.IsNullOrEmpty(Request["Secret"]))
            {
                objNewModel.WeixinCorpId = Request["CorpID"];
            }
            if (Request["Secret"] != objModel.WeixinCorpSecret && Request["Secret"] != "******" && !string.IsNullOrEmpty(Request["Secret"]))
            {
                objNewModel.WeixinCorpSecret = Request["Secret"];
            }
            if (Request["Welcome"] != objModel.WelcomeMessage)
            {
                objNewModel.WelcomeMessage = Request["Welcome"];
            }
            if (Request["Token"] != objModel.WeixinToken && Request["Token"] != "******" && !string.IsNullOrEmpty(Request["Token"]))
            {
                objNewModel.WeixinToken = Request["Token"];
            }
            if (Request["EncodingAESKey"] != objModel.WeixinEncodingAESKey && Request["EncodingAESKey"] != "******" && !string.IsNullOrEmpty(Request["EncodingAESKey"]))
            {
                objNewModel.WeixinEncodingAESKey = Request["EncodingAESKey"];
            }

            sys.Repository.Update(objNewModel);
            #endregion

            #region 清理缓存
            //清理缓存
            if (WeChatCommonService.lstDepartment(AccountManageID) != null)
            {
                cacheManager.Remove("DepartmentList");
            }
            if (WeChatCommonService.lstTag(AccountManageID) != null)
            {
                cacheManager.Remove("TagItem");
            }

            if (WeChatCommonService.lstUser(AccountManageID) != null)
            {
                cacheManager.Remove("UserItem" + AccountManageID);
            }
            if (WeChatCommonService.lstSysWeChatConfig != null)
            {
                cacheManager.Remove("SysWeChatConfig");
            }

            var newMenus = _sysMenuService.GetMenusByUserID(loginUser, null);
            loginUser.Menus = newMenus;
            Session["UserInfo"] = loginUser;
            #endregion

            return Json(doJson(null));

        }
        public ActionResult CategoryManageList(string id)
        {
            ViewBag.appid = id;
            ViewBag.taglist = WeChatCommonService.lstTag(AccountManageID);
            return View();
        }


        public ActionResult GetListTree(string appid)
        {

            int appResult = default(int);

            if (int.TryParse(appid, out appResult))
            {
                List<Category> lstCate = CommonService.lstCategory.FindAll(x => x.AppId == appResult && x.IsDeleted == false).ToList();

                List<Category> menuButtons = lstCate.Where(b => b.ParentCode == 0).OrderBy(b => b.CategoryOrder).ToList();

                List<CategoryButtonView> btnList = new List<CategoryButtonView>();

                foreach (var button in menuButtons)
                {
                    var btnFunc = JsonHelper.FromJson<ButtonReturnType>(button.Function);
                    CategoryButtonView btn = new CategoryButtonView()
                    {
                        Id = button.Id,
                        name = button.CategoryName,
                        key = ConvertCategoryCodeToAutpReplyId(button.CategoryCode),
                        type = btnFunc.Button.type,
                        url = btnFunc.Button.url,
                    };
                    List<Category> subButtons = lstCate.Where(b => b.ParentCode == button.Id).OrderBy(b => b.CategoryOrder).ToList();
                    foreach (var subBtn in subButtons)
                    {
                        var subBtnFunc = JsonHelper.FromJson<ButtonReturnType>(subBtn.Function);
                        CategoryButtonView subButton = new CategoryButtonView()
                        {
                            Id = subBtn.Id,
                            name = subBtn.CategoryName,
                            key = ConvertCategoryCodeToAutpReplyId(subBtn.CategoryCode),
                            type = subBtnFunc.Button.type,
                            url = subBtnFunc.Button.url,
                        };
                        btn.children.Add(subButton);
                    }
                    btnList.Add(btn);
                }

                return Json(new
                {
                    menu = btnList
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return ErrorNotification("app id is wrong");
            }
        }

        private string ConvertCategoryCodeToAutpReplyId(string code)
        {
            if (!string.IsNullOrEmpty(code))
            {
                if (code.Contains(":"))
                {
                    return code.Split(':')[0];
                }
                return code;
            }
            return string.Empty;
        }

        public override ActionResult getTreeData()
        {
            string strID = Request["ID"];
            string appRequest = Request["appid"];
            if (string.IsNullOrEmpty(appRequest))
            {
                return ErrorNotification("请求错误！");
            }
            int appid = Convert.ToInt32(appRequest);
            var list = CommonService.GetCategory(appid, false).OrderBy(a => a.CategoryOrder).ToList();
            var listReturn = EasyUITreeData.GetTreeData(list, "Id", "CategoryName", "ParentCode");

            if (!string.IsNullOrEmpty(strID))
            {
                EasyUITreeData.SetChecked(new List<int> { int.Parse(strID) }, listReturn);

            }

            return Json(listReturn, JsonRequestBehavior.AllowGet);

        }

        public override bool BeforeAddOrUpdate(CategoryView objModal, string Id)
        {
            cacheManager.Remove("Category");
            return true;
        }

        public override bool AfterDelete(string sIds)
        {
            cacheManager.Remove("Category");
            return true;
        }

        public JsonResult SynchronousCategorylist()
        {
            string appRequest = Request["appid"];

            if (CommonService.lstCategory != null)
            {
                cacheManager.Remove("Category");
            }

            //TODO等待另一接口实现

            return Json(doJson(null), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 取得token
        /// </summary>
        /// <param name="appId">这个appId不是微信的agengId，而是DB中的appId，也就是表SysWechatConfig的Id</param>
        /// <returns></returns>
        public string Rtntoken(int appId = 0)
        {
            var config = WeChatCommonService.GetWeChatConfigByID(appId);
            var strToken = AccessTokenContainer.TryGetToken(config.WeixinCorpId, config.WeixinCorpSecret);
            if (!string.IsNullOrEmpty(strToken))
            {
                return strToken;
            }

            return null;

        }

        ///// <summary>
        ///// 用默认的App（企业小助手）来获取企业号的accesstoken
        ///// 这个方法主要是为了防止新同步过来的app没有coprId和corpSecret信息
        ///// </summary>
        ///// <returns></returns>
        //private string GetTokenByDefaultApp()
        //{
        //    var wxAccount = _accountManageService.Repository.GetByKey(AccountManageID);
        //    var wxCorpId = wxAccount.CorpId.Trim();
        //    var accessToken = WeChatCommonService.GetWeiXinTokenByAgentId(wxCorpId);
        //    return accessToken;
        //}

        public override JsonResult Delete(string sIds)
        {
            if (!string.IsNullOrEmpty(sIds))
            {
                var cateList = CommonService.lstCategory;
                var arrID = sIds.TrimEnd(',').Split(',').Select(int.Parse).ToArray();

                if (cateList.Any(x => arrID.Any(y => y == x.ParentCode) && x.IsDeleted == false))
                {
                    return ErrorNotification("拥有子菜单的父菜单无法删除！");
                }


            }
            return base.Delete(sIds);
        }

        [HttpGet]
        public ActionResult MenuManagement()
        {
            return View();
        }
    }
}

