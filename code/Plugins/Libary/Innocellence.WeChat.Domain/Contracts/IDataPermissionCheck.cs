using Infrastructure.Core;

namespace Innocellence.WeChat.Domain.Contracts
{
    public interface IDataPermissionCheck : IDependency
    {
        bool AppDataCheck(int targetAppId, int currentAppId);
    }
}
