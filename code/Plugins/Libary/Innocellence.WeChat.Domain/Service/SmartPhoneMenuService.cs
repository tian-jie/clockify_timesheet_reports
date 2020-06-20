using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Transactions;
using Infrastructure.Core.Data;
using Infrastructure.Utility;
using Infrastructure.Utility.Data;
using Infrastructure.Utility.Extensions;
using Infrastructure.Web.Domain.Entity;
using Infrastructure.Web.Domain.ModelsView;
using Infrastructure.Web.Domain.Service;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Contracts.ViewModel;
using Innocellence.WeChat.Domain.Common;
using Innocellence.Weixin.Entities;
using Innocellence.Weixin.QY;
using Innocellence.Weixin.QY.AdvancedAPIs.MailList;
using Innocellence.Weixin.QY.CommonAPIs;
using Innocellence.Weixin;
using Innocellence.WeChat.Domain.Services;
using Innocellence.Weixin.MP.Entities;
using System;
using Innocellence.Weixin.Entities.Menu;
using System.Web.Script.Serialization;

namespace Innocellence.WeChat.Domain.Service
{
    public class SmartPhoneMenuService : BaseService<Category>, ISmartPhoneMenuService
    {
        private const string _endFix = "ENTRY_PUSH";
        private AutoReplyService _autoReplyService = new AutoReplyService();

        public QyJsonResult Push(int appId)
        {
            var categories = CommonService.lstCategory.Where(x => x.AppId == appId && (x.CategoryCode == null || !x.CategoryCode.EndsWith(_endFix, true, CultureInfo.CurrentCulture)) && (x.IsDeleted == null || x.IsDeleted == false)).OrderBy(x => x.CategoryOrder).Select(x =>
            {
                var function = x.Function.IsNullOrEmpty() ? null : JsonHelper.FromJson<ButtonReturnType>(x.Function);
                return new CategroyChild
                {
                    Id = x.Id,
                    key = x.CategoryCode,
                    name = function == null ? x.CategoryName : function.Button.name,
                    sub_button = new List<MenuFull_RootButton>(),
                    //跳转到新闻列表本身也是view, 因此在同步时将其type改为view.
                    type = function == null ? null : "view-news-list".Equals(function.Button.type) ? "view" : function.Button.type,
                    ParentId = x.ParentCode.GetValueOrDefault(),
                    order = x.CategoryOrder.GetValueOrDefault(),
                    url = function == null ? null : function.Button.url,
                };
            }).ToList();

            foreach (var category in categories)
            {
                if ("click".Equals(category.type))
                {
                    var key = category.key.Split(':')[0];
                    int keyId = default(int);
                    if (int.TryParse(key, out keyId))
                    {
                        var autoReply = _autoReplyService.GetDetail(keyId);
                        if (autoReply == null)
                        {
                            throw new System.Exception(string.Format("{0} 的口令不存在, 请重新选择.", category.name));
                        }
                        if ((int)AutoReplyKeywordEnum.MENU == autoReply.KeywordType
                            && null != autoReply.Keywords && autoReply.Keywords.Count > 0)
                        {
                            if ((int)AutoReplyMenuEnum.SCAN_PUSH_EVENT == autoReply.Keywords[0].SecondaryType)
                            {
                                category.type = "scancode_push";
                            }
                            else if ((int)AutoReplyMenuEnum.SCAN_WITH_PROMPT == autoReply.Keywords[0].SecondaryType)
                            {
                                category.type = "scancode_waitmsg";
                            }
                        }
                    }
                    else
                    {
                        throw new System.Exception(string.Format("请重新选择 {0} 的口令.", category.key));
                    }
                }
            }

            var buttons = new List<MenuFull_RootButton>();

            categories.Where(x => x.ParentId == 0).ToList().ForEach(x => GenerateMenuHierarchy(categories, x, buttons));

            var config = WeChatCommonService.GetWeChatConfigByID(appId);
            //修改
            if (categories != null && categories.Count > 0)
            {
                var menu = new GetMenuResultFull { menu = new MenuFull_ButtonGroup { button = buttons } };
                var btnMenus = CommonApi.GetMenuFromJsonResult(menu).menu;
                if (config.IsCorp.HasValue && !config.IsCorp.Value)
                {
                    var result = Innocellence.Weixin.MP.CommonAPIs.CommonApi.CreateMenu(config.WeixinCorpId, config.WeixinCorpSecret, btnMenus);
                    return new QyJsonResult() { errcode = (ReturnCode_QY)result.errcode, errmsg = result.errmsg };
                }
                else
                {
                    var result = CommonApi.CreateMenu(WeChatCommonService.GetWeiXinToken(appId), int.Parse(config.WeixinAppId), btnMenus);
                    return result;
                }
            }
            //删除
            else
            {
                if (config.IsCorp.HasValue && !config.IsCorp.Value)
                {
                    var result = Innocellence.Weixin.MP.CommonAPIs.CommonApi.DeleteMenu(config.WeixinCorpId, config.WeixinCorpSecret);
                    return new QyJsonResult() { errcode = (ReturnCode_QY)result.errcode, errmsg = result.errmsg };
                }
                else
                {
                    var result = CommonApi.DeleteMenu(WeChatCommonService.GetWeiXinToken(appId), int.Parse(config.WeixinAppId));
                    return result;
                }
            }
        }

        private void GenerateMenuHierarchy(IEnumerable<CategroyChild> list, CategroyChild parent, IList<MenuFull_RootButton> buttons)
        {
            var categroyChildren = list as IList<CategroyChild> ?? list.ToList();
            var currentLayerChilds = categroyChildren.Where(x => x.ParentId == parent.Id).ToList();

            parent.sub_button.AddRange(currentLayerChilds.OrderBy(x => x.order).ToList());

            if (parent.ParentId == 0)
            {
                buttons.Add(parent);
            }

            currentLayerChilds.ForEach(x => GenerateMenuHierarchy(categroyChildren, x, buttons));
        }

        public AppMenuView QueryMenuViewById(int menuId, int AccountMangageID)
        {
            var category = CommonService.lstCategory.FirstOrDefault(x => x.Id == menuId) ?? Repository.GetByKey(menuId);
            if (category == null)
            {
                return null;
            }

            category.IsAdmin = category.IsAdmin.GetValueOrDefault();
            var view = (AppMenuView)(new AppMenuView().ConvertAPIModel(category));
            view.TagItems = WeChatCommonService.lstTag(AccountMangageID);
            view.SelecTagItems = category.Role.IsNullOrEmpty() ? null : category.Role.Split(',').ToList().ConvertAll(x => new TagItem { tagname = x });
            view.ButtonReturnType = category.Function.IsNullOrEmpty() ? null : JsonHelper.FromJson<ButtonReturnType>(category.Function);

            return view;
        }

        public int UpdateOrAdd(AppMenuView menu)
        {
            menu.Function = menu.ButtonReturnType == null ? null : JsonHelper.ToJson(menu.ButtonReturnType);
            //menu.Role = menu.SelecTagItems == null || menu.SelecTagItems.Count == 0
            //    ? null
            //    : menu.SelecTagItems.Select(x => x.tagname).Join(",");

            var count = 0;

            if (!menu.ParentCode.HasValue)
            {
                menu.ParentCode = 0;
            }

            if (menu.Id == 0)
            {
                count = base.InsertView((CategoryView)menu);
            }
            else
            {
                //删除菜单
                if (menu.IsDeleted.HasValue && menu.IsDeleted.Value)
                {
                    var menuGroup =
                           CommonService.lstCategory.Where(x => x.ParentCode == menu.Id || x.Id == menu.Id).ToList();


                    using (
                        var transactionscope = new TransactionScope(TransactionScopeOption.Required,
                            new TransactionOptions { IsolationLevel = IsolationLevel.RepeatableRead }))
                    {
                        menuGroup.ForEach(x =>
                                             {
                                                 x.IsDeleted = true;
                                                 Repository.Update(x);
                                             });
                        transactionscope.Complete();
                    }
                }
                else
                {
                    count = base.UpdateView((CategoryView)menu, new List<string>
                    {
                        "CategoryCode","AppId","CategoryName","CategoryDesc",
                        "ParentCode","IsAdmin","CategoryOrder","Function","NoRoleMessage","Role"
                    });
                }
            }

            CommonService.RemoveCategoryFromCache();

            return count;
        }

        public GetMenuResult GetMPMenu(int appId)
        {
            var config = WeChatCommonService.GetWeChatConfigByID(appId);
            if (config.IsCorp.HasValue && !config.IsCorp.Value)
            {
                var result = Innocellence.Weixin.MP.CommonAPIs.CommonApi.GetMenu(config.WeixinCorpId, config.WeixinCorpSecret);
                return result;
            }
            return null;
        }

        public CreateMenuConditionalResult CreateConditionalMenu(int appId, string menuJsonStr, Weixin.MP.Entities.Menu.MenuMatchRule menuMatchRule)
        {
            var config = WeChatCommonService.GetWeChatConfigByID(appId);
            if (config != null && config.IsCorp.HasValue && !config.IsCorp.Value)
            {
                var menu = Innocellence.Weixin.MP.CommonAPIs.CommonApi.GetMenuFromJson(menuJsonStr);
                if (menu != null)
                {
                    Weixin.MP.Entities.Menu.ConditionalButtonGroup conditonalBtnGroup = new Weixin.MP.Entities.Menu.ConditionalButtonGroup()
                    {
                        button = menu.menu.button,
                        matchrule = menuMatchRule,
                    };
                    var result = Innocellence.Weixin.MP.CommonAPIs.CommonApi.CreateMenuConditional(config.WeixinCorpId, config.WeixinCorpSecret, conditonalBtnGroup);
                    return result;
                }
            }
            throw new Exception(string.Format("{0} 的配置错误, 请联系管理员.", appId));
        }

        /// <summary>
        /// GetSingleButtonFromJsonObject需要更新到框架中
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public GetMenuResult GetMenuFromJson(string jsonString)
        {
            GetMenuResult getMenuResult = new GetMenuResult(new ButtonGroup());
            try
            {
                JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                object obj = javaScriptSerializer.Deserialize<object>(jsonString);
                Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
                bool flag = dictionary != null && dictionary.ContainsKey("menu");
                if (flag)
                {
                    object obj2 = dictionary["menu"];
                    object[] array = (obj2 as Dictionary<string, object>)["button"] as object[];
                    object[] array2 = array;
                    for (int i = 0; i < array2.Length; i++)
                    {
                        object obj3 = array2[i];
                        Dictionary<string, object> dictionary2 = obj3 as Dictionary<string, object>;
                        bool flag2 = dictionary2.ContainsKey("key") && !string.IsNullOrEmpty(dictionary2["key"] as string);
                        if (flag2)
                        {
                            getMenuResult.menu.button.Add(GetSingleButtonFromJsonObject(dictionary2));
                        }
                        else
                        {
                            SubButton subButton = new SubButton(dictionary2["name"] as string);
                            getMenuResult.menu.button.Add(subButton);
                            object[] array3 = dictionary2["sub_button"] as object[];
                            for (int j = 0; j < array3.Length; j++)
                            {
                                object obj4 = array3[j];
                                subButton.sub_button.Add(GetSingleButtonFromJsonObject(obj4 as Dictionary<string, object>));
                            }
                        }
                    }
                }
                else
                {
                    bool flag3 = dictionary != null && dictionary.ContainsKey("errmsg");
                    if (flag3)
                    {
                        throw new Weixin.Exceptions.ErrorJsonResultException(dictionary["errmsg"] as string, null, null, null);
                    }
                }
            }
            catch (Exception var_18_195)
            {
                getMenuResult = null;
            }
            return getMenuResult;
        }

        /// <summary>
        /// 该方法需要更新到框架中
        /// </summary>
        /// <param name="objs"></param>
        /// <returns></returns>
        private SingleButton GetSingleButtonFromJsonObject(Dictionary<string, object> objs)
        {
            string type = objs["type"] as string;
            if ("view".Equals(type, StringComparison.OrdinalIgnoreCase))
            {
                return new SingleViewButton
                {
                    type = type,
                    url = (objs["url"] as string),
                    name = (objs["name"] as string),
                };
            }
            else
            {
                return new SingleClickButton
                {
                    key = (objs["key"] as string),
                    name = (objs["name"] as string),
                    type = type
                };

            }
        }
    }

    public class CategroyChild : MenuFull_RootButton
    {
        public int Id { get; set; }

        public int ParentId { get; set; }

        public int order { get; set; }
    }
}
