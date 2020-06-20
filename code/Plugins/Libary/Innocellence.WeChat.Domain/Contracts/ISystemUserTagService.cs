using Infrastructure.Core;
using Infrastructure.Web.Domain.Entity;
using Innocellence.WeChat.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innocellence.WeChat.Domain.Contracts
{
   public interface ISystemUserTagService: IDependency, IBaseService<SystemUserTag>
    {
        List<SystemUserTag> GetFirstLevelTag();
        List<SystemUserTag> GetTagByParentId(int parentId);
        int Create(SystemUserTag tag);
        int Delete(int tagId);
        int Update(SystemUserTag tag);
        List<SystemUserTag> GetAllTags();
        int EditTagName(string tagName, int tagId);
    }
}
