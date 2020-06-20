using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;

namespace Innocellence.WeChat.Domain.Contracts
{
    public interface IFileManageService : IDependency, IBaseService<FileManage>
    {
        int UpdateMediaId(int fileId, string mediaId);
        T GetById<T>(int id) where T : IViewModel, new();
        T GetByMediaId<T>(string mediaId) where T : IViewModel, new();
    }
}
