using System.Collections.Generic;
using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.Weixin.QY.AdvancedAPIs.App;

namespace Innocellence.WeChat.Domain.Contracts
{
    public interface ISysWechatConfigService : IDependency, IBaseService<SysWechatConfig>
    {
        void SyncWechatApps(int accountManageId, string accessToken, List<GetAppList_AppInfo> wxAppList, List<SysWechatConfig> appList,bool isSyncMenu);
    }
}
