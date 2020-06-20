using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innocellence.WeChat.Domain.Contracts
{
    public interface ISystemUserTagMappingService : IDependency, IBaseService<SystemUserTagMapping>
    {
        int Create(SystemUserTagMapping tag);
        void Create(List<SystemUserTagMapping> tags);
        int DeleteByTag(int tagId);
        int Delete(int id);
        void DeleteByOpenId(string openId);
        int Update(SystemUserTagMapping tag);
        List<SystemUserTagMapping> GetMappingByTagIds(List<int> tagIds);
        List<SystemUserTagMapping> GetUserTagByOpenId(string openId);
    }
}
