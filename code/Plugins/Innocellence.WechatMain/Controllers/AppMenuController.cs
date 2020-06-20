using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Infrastructure.Core.Logging;
using Infrastructure.Utility.Extensions;
using Infrastructure.Web.Domain.Entity;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Contracts.ViewModel;
using Innocellence.Weixin;
using System;
using Innocellence.Weixin.QY.AdvancedAPIs.MailList;
using Infrastructure.Web.Domain.Service;
using Infrastructure.Utility.Data;
using Innocellence.WeChat.Domain.ViewModel;
using Infrastructure.Web.Domain.ModelsView;
using System.Text.RegularExpressions;
using System.Text;
using Innocellence.WeChat.Domain;
using Newtonsoft.Json;

namespace Innocellence.WeChatMain.Controllers
{
    public class AppMenuController : BaseController<Category, AppMenuView>
    {
        private static readonly ILogger _log = LogManager.GetLogger(typeof(AppMenuController));
        private readonly ISmartPhoneMenuService _menuService;
        private readonly IArticleInfoService _articleInfoService;
        public AppMenuController(ISmartPhoneMenuService newsService, IArticleInfoService articleInfoService)
            : base(newsService)
        {
            _menuService = newsService;
            _articleInfoService = articleInfoService;
        }

        public override ActionResult Edit(string id)
        {
            int categoryId;

            if (int.TryParse(id, out categoryId))
            {
                var category = _menuService.QueryMenuViewById(categoryId, AccountManageID);
                return category == null ? ErrorNotification("您操作的数据不存在!") : Json(category, JsonRequestBehavior.AllowGet);
            }

            _log.Debug("CategoryController.Edit: 传入的id不为数字类型!");

            return ErrorNotification("参数应该为自然数!");
        }
        [ValidateInput(false)]
        public override JsonResult Post(AppMenuView objModal, string Id)
        {
            int categoryId;

            if (int.TryParse(Id, out categoryId))
            {
                //暂时写死。
                objModal.IsAdmin = false;

                _menuService.UpdateOrAdd(objModal);

                if (objModal != null && objModal.ButtonReturnType != null && objModal.ButtonReturnType.Button != null)
                {
                    if (objModal.ButtonReturnType.Button.type.Equals("view-news-list"))
                    {
                        objModal.ButtonReturnType.Button.url = CommonService.GetSysConfig("FileManageUrl", "http://localhost:24829").Trim('/') + "/News/ArticleInfo/List?wechatid=" + objModal.AppId + "&strSubCate=" + objModal.Id;

                        var obj = _menuService.Repository.GetByKey(objModal.Id);
                        obj.Function = JsonHelper.ToJson(objModal.ButtonReturnType);
                        _menuService.Repository.Update(obj, new List<string>() { "Function" });
                    }
                }
                return Json(doJson(null), JsonRequestBehavior.AllowGet);
            }

            _log.Debug("CategoryController.Edit: 传入的id不为数字类型!");
            return ErrorNotification("参数应该为自然数!");
        }

        public override JsonResult Delete(string sIds)
        {
            if (!sIds.IsNullOrEmpty())
            {
                sIds.Split(',').Select(x => new AppMenuView { Id = int.Parse(x), IsDeleted = true }).ToList().ForEach(x => _menuService.UpdateOrAdd(x));
            }

            return Json(doJson(null), JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]
        [ValidateInput(false)]
        public JsonResult Push(string id)
        {
            int appId = 0;

            if (!int.TryParse(id, out appId))
            {
                return ErrorNotification("Push falid!");
            }

            var result = _menuService.Push(appId);

            return result.errcode == ReturnCode_QY.请求成功 ? SuccessNotification("Success") : ErrorNotification("Push falid!");
        }

        [HttpPost]
        public ActionResult ModifyCategory(List<CategoryButtonView> buttons, int appId)
        {
            if (ValidateButtons(buttons))
            {
                List<Category> currentAppCategory = CommonService.lstCategory.FindAll(x => x.AppId == appId && x.IsDeleted == false).ToList();
                List<int> firstLevelCategoryIds = currentAppCategory.FindAll(x => x.ParentCode == 0).Select(x => x.Id).ToList();
                List<int> secondLevelCategoryIds = currentAppCategory.FindAll(x => x.ParentCode > 0).Select(x => x.Id).ToList();

                _Logger.Debug("begin to modify category.");
                List<int> needDeleteIds = new List<int>();
                List<int> hasInsertedIds = new List<int>();
                try
                {
                    if (buttons != null && buttons.Count > 0)
                    {
                        //第一层
                        for (int i = 0; i < buttons.Count; i++)
                        {
                            var btn = buttons[i];
                            int categoryId = DoInsertCategory(btn, appId, i, 0);
                            hasInsertedIds.Add(categoryId);
                            if (btn.children != null && btn.children.Count > 0)
                            {
                                for (int j = 0; j < btn.children.Count; j++)
                                {
                                    var subBtn = btn.children[j];
                                    int subCategoryId = DoInsertCategory(subBtn, appId, j, categoryId);
                                    hasInsertedIds.Add(subCategoryId);
                                }
                            }
                        }
                    }
                    needDeleteIds = currentAppCategory.Select(x => x.Id).Except(hasInsertedIds).ToList();
                    DoDeleteCategory(needDeleteIds);
                }
                catch (Exception ex)
                {
                    _Logger.Error(ex);
                    throw;
                }
                finally
                {
                    _Logger.Debug("deleted ids :{0}", string.Join("; ", needDeleteIds.ToArray()));
                    CommonService.RemoveCategoryFromCache();
                }
                return Push(appId.ToString());
            }
            return null;
        }

        private bool ValidateButtons(List<CategoryButtonView> buttons)
        {
            if (null != buttons)
            {
                foreach (var btn in buttons)
                {
                    if (ValidateBtnName(btn.name, false) && ValidateBtnType(btn.type, btn.children != null && btn.children.Count > 0))
                    {
                        if (btn.children != null && btn.children.Count > 0)
                        {
                            foreach (var subBtn in btn.children)
                            {
                                ValidateBtnName(subBtn.name, true);
                                ValidateBtnType(subBtn.type, false);
                            }
                        }
                    }
                }
            }
            return true;
        }

        private bool ValidateBtnName(string btnName, bool isSubBtn)
        {
            if (string.IsNullOrEmpty(btnName))
            {
                throw new Exception("请输入菜单名称");
            }
            //一级菜单5个汉字  15个字母
            //二级菜单10个汉字 30个字母
            int max = isSubBtn ? 30 : 15;
            if (GetStringLength(btnName) > max)
            {
                throw new Exception(isSubBtn ? "二级菜单名称过长, 请重新输入" : "一级菜单名称过长, 请重新输入");
            }
            return true;
        }

        /// <summary>
        /// 获取中英文混排字符串的实际长度(字节数)
        /// 一个中文当成3个英文处理
        /// </summary>
        /// <param name="str">要获取长度的字符串</param>
        /// <returns>字符串的实际长度值（字节数）</returns>
        private int GetStringLength(string str)
        {
            if (str.Equals(string.Empty)) return 0;
            int strlen = 0;
            ASCIIEncoding strData = new ASCIIEncoding();
            //将字符串转换为ASCII编码的字节数字
            byte[] strBytes = strData.GetBytes(str);
            for (int i = 0; i <= strBytes.Length - 1; i++)
            {
                if (strBytes[i] == 63)
                {
                    //中文都将编码为ASCII编码63,即"?"号
                    strlen += 2;
                }
                strlen++;
            }
            return strlen;
        }

        private bool ValidateBtnType(string btnType, bool hasChildren)
        {
            if (!hasChildren)
            {
                if (string.IsNullOrEmpty(btnType))
                {
                    throw new Exception("请选择菜单类型");
                }
            }
            return true;
        }

        private int DoInsertCategory(CategoryButtonView btn, int appId, int order, int parentId = 0)
        {
            bool hasChildren = btn.children != null && btn.children.Count > 0;
            AppMenuView btnView = new AppMenuView();
            if (btn.Id != 0)
            {
                btnView = (AppMenuView)btnView.ConvertAPIModel(_BaseService.Repository.GetByKey(btn.Id));
            }
            btnView.Id = btn.Id;
            btnView.AppId = appId;
            btnView.ParentCode = parentId;
            btnView.CategoryName = btn.name;
            btnView.CategoryOrder = order + 1;
            btnView.CreatedDate = DateTime.Now;
            btnView.CreatedUserID = User.Identity.Name;
            ButtonReturnType btnFunc = new ButtonReturnType()
            {
                Button = new MenuButton()
                {
                    name = btn.name,
                    type = hasChildren ? string.Empty : btn.type,
                    url = hasChildren ? string.Empty : btn.url,
                }
            };
            if ("click".Equals(btnFunc.Button.type))
            {
                var key = btn.key;
                if (!string.IsNullOrEmpty(btn.key))
                {
                    key = btn.key.Split(':')[0];
                }
                btnView.CategoryCode = string.Format("{0}:::{1}", key, btn.name);
                btnView.CategoryDesc = btnView.CategoryCode;
                btnFunc.Button.key = btnView.CategoryCode;
            }
            else
            {
                btnView.CategoryCode = string.Empty;
                btnView.CategoryDesc = btn.url;
            }

            btnView.ButtonReturnType = btnFunc;
            btnView.Function = JsonHelper.ToJson(btnFunc);
            if (btnView.Id != 0)
            {
                _BaseService.UpdateView(btnView);
            }
            else
            {
                _BaseService.InsertView(btnView);
            }

            //跳转到新闻列表需要后台手动创建url,其本质是view,在同步至微信服务器时转换为view
            if ("view-news-list".Equals(btn.type))
            {
                btnView.ButtonReturnType.Button.url = CommonService.GetSysConfig("WeChatUrl", "http://wechat.innoprise.cn/").Trim('/') + "/News/ArticleInfo/List?wechatid=" + appId + "&strSubCate=" + btnView.Id;
                var obj = _menuService.Repository.GetByKey(btnView.Id);
                obj.Function = JsonHelper.ToJson(btnView.ButtonReturnType);
                btnView.CategoryDesc = btnView.ButtonReturnType.Button.url;
                _menuService.Repository.Update(obj, new List<string>() { "Function", "CategoryDesc" });
            }

            return btnView.Id;
        }

        private void DoDeleteCategory(List<int> needDeleteIds, int retryCount = 0)
        {
            try
            {
                if (needDeleteIds != null && needDeleteIds.Count > 0)
                {
                    _BaseService.Repository.Delete(needDeleteIds);
                    string sql = string.Format("Update ArticleInfo set CategoryId = null Where CategoryId in ({0})", string.Join(",", needDeleteIds));
                    _articleInfoService.Repository.SqlExcute(sql);
                }
            }
            catch (Exception)
            {
                retryCount++;
                if (retryCount < 5)
                {
                    DoDeleteCategory(needDeleteIds, retryCount);
                }
                else
                {
                    throw;
                }
            }
        }

        public JsonResult GetMPMenu(int appId)
        {
            try
            {
                var result = _menuService.GetMPMenu(appId);
                if (null != result && null != result.menu)
                {
                    if (result.conditionalmenu != null)
                    {
                        foreach (var item in result.conditionalmenu)
                        {
                            _log.Debug("conditionalmenu id:{0}", item.menuid);
                        }
                    }
                    //暂时不处理conditional menu
                    result.conditionalmenu = null;
                    return Json(new { menu = result }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                return ErrorNotification(ex);
            }
            return null;
        }

        public ActionResult CreateConditionalMPMenu(int appId)
        {
            return View();
        }

        public JsonResult DoCreateCreateConditionalMPMenu(int appId, string group_id, string menuStr)
        {
            group_id = group_id.Trim();
            menuStr = menuStr.Trim();
            if (appId > 0 && !string.IsNullOrEmpty(group_id) && !string.IsNullOrEmpty(menuStr))
            {
                try
                {
                    _log.Debug("{0} create conditional menu for {1}", appId, group_id);
                    var menuMatchRule = new Weixin.MP.Entities.Menu.MenuMatchRule()
                    {
                        tag_id = group_id,
                        //group_id = group_id,
                    };
                    var result = _menuService.CreateConditionalMenu(appId, menuStr, menuMatchRule);
                    if (result.errcode == ReturnCode.请求成功)
                    {
                        return SuccessNotification("操作成功");
                    }
                    else
                    {
                        return ErrorNotification("操作失败: " + result.errmsg);
                    }
                }
                catch (Exception ex)
                {
                    return ErrorNotification(ex);
                }
            }
            return null;
        }
    }

}