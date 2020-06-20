using Infrastructure.Core;
using Infrastructure.Web.Domain.Entity;
using Innocellence.WeChat.Domain.Contracts.ViewModel;
using Innocellence.Weixin.Entities;
using MPEntities = Innocellence.Weixin.MP.Entities;

namespace Innocellence.WeChat.Domain.Contracts
{
    public interface ISmartPhoneMenuService : IDependency, IBaseService<Category>
    {
        QyJsonResult Push(int appId);

        AppMenuView QueryMenuViewById(int menuId, int AccountMangageID);

        int UpdateOrAdd(AppMenuView view);

        MPEntities.GetMenuResult GetMPMenu(int appId);

        MPEntities.CreateMenuConditionalResult CreateConditionalMenu(int appId, string menuJsonStr, MPEntities.Menu.MenuMatchRule menuMatchRule);
    }
}
