using Innocellence.WeChat.Domain.Contracts;

namespace Innocellence.Activity.Service
{
    public class EventPermissionService : IDataPermissionCheck
    {
        public bool AppDataCheck(int targetAppId, int currentAppId)
        {
            return targetAppId == currentAppId;
        }
    }

    public class PollingPermissionService : IDataPermissionCheck
    {
        public bool AppDataCheck(int targetAppId, int currentAppId)
        {
            return targetAppId == currentAppId;
        }
    }

}
