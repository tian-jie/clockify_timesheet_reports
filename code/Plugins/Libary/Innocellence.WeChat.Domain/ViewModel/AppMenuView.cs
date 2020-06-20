using System.Collections.Generic;
using Infrastructure.Core;
using Infrastructure.Web.Domain.Entity;
using Infrastructure.Web.Domain.ModelsView;
using Innocellence.Weixin.QY.AdvancedAPIs.MailList;
using Innocellence.Weixin.Entities.Menu;

namespace Innocellence.WeChat.Domain.Contracts.ViewModel
{
    public class AppMenuView : CategoryView
    {
        public ButtonReturnType ButtonReturnType { get; set; }

        /// <summary>
        /// 当前菜单需要的tag
        /// </summary>
        public IList<TagItem> SelecTagItems { get; set; }

        /// <summary>
        /// 所有的tag
        /// </summary>
        public IList<TagItem> TagItems { get; set; }

        public new IViewModel ConvertAPIModel(object model)
        {
            var entity = (Category)model;
            Id = entity.Id;
            CategoryCode = entity.CategoryCode;
            CategoryName = entity.CategoryName;
            AppId = entity.AppId;
            LanguageCode = entity.LanguageCode;
            CategoryDesc = entity.CategoryDesc;
            ParentCode = entity.ParentCode;
            IsDeleted = entity.IsDeleted;
            CreatedDate = entity.CreatedDate;
            CreatedUserID = entity.CreatedUserID;
            UpdatedDate = entity.UpdatedDate;
            UpdatedUserID = entity.UpdatedUserID;
            NoRoleMessage = entity.NoRoleMessage;
            IsAdmin = entity.IsAdmin;
            Role = entity.Role;
            CategoryOrder = entity.CategoryOrder;

            return this;
        }
    }

    public class ButtonReturnType
    {
        /// <summary>
        /// SingleViewButton或者是SingleClickButton类型的实例
        /// </summary>
        public MenuButton Button { get; set; }

        //[JsonConverter(typeof(StringEnumConverter))]
        public string ResponseMsgType { get; set; }

        /// <summary>
        /// 文本内容
        /// </summary>
        public string Content { get; set; }
    }

    public class MenuButton : SingleButton
    {
        public MenuButton()
            : base(string.Empty)
        {
        }

        public string url { get; set; }

        public string key { get; set; }
    }
}
