using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;

namespace Innocellence.WeChat.Domain.Contracts
{
    public interface IImageInfoService : IDependency, IBaseService<ImageInfo>
    {
        List<T> GetListByParent<T>(int ownerId) where T : IViewModel, new();
    }
}
