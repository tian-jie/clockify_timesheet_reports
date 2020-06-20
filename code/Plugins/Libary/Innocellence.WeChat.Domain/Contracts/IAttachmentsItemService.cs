using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Innocellence.WeChat.Domain.Contracts
{
    public interface IAttachmentsItemService : IDependency, IBaseService<SysAttachmentsItem>
    {
        int UpdateMediaId(int fileId, string mediaId, DateTime MediaCreateTime);
        T GetById<T>(int id) where T : IViewModel, new();
        T GetByMediaId<T>(string mediaId) where T : IViewModel, new();
        List<T> GetList<T>(Expression<Func<SysAttachmentsItem, bool>> predicate) where T : IViewModel, new();
        SysAttachmentsItem ThumbImageAndInsertIntoDB(AttachmentsItemPostProperty p);
    }
}
