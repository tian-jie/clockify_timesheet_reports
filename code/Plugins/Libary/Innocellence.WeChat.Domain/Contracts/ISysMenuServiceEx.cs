using Infrastructure.Core;
using Infrastructure.Web.Domain.Entity;

namespace Innocellence.WeChat.Domain.Contracts
{
    public interface ISysMenuServiceEx : IDependency, IBaseService<SysMenu>
    {
        void InitAppSysMenu(int rootMenuParentId, int appId, string appName, int accountManageId);
    }
}
