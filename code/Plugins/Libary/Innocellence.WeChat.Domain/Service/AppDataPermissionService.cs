using Innocellence.WeChat.Domain.Contracts;

namespace Innocellence.WeChat.Domain.Service
{
    public class ArticleInfoDataPermissionService : IDataPermissionCheck
    {
        public bool AppDataCheck(int targetAppId, int currentAppId)
        {
            return targetAppId == currentAppId;
        }
    }

    public class MessageDataPermissionService : IDataPermissionCheck
    {
        public bool AppDataCheck(int targetAppId, int currentAppId)
        {
            return targetAppId == currentAppId;
        }
    }

    public class QuestionDataPermissionService : IDataPermissionCheck
    {
        public bool AppDataCheck(int targetAppId, int currentAppId)
        {
            return targetAppId == currentAppId;
        }
    }

    public class EventPermissionService : IDataPermissionCheck
    {
        public bool AppDataCheck(int targetAppId, int currentAppId)
        {
            return targetAppId == currentAppId;
        }
    }
}
