using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;
using Microsoft.AspNet.Identity;
using Infrastructure.Web.Domain.Entity;
namespace Innocellence.WeChat.Domain.Contracts
{
    public interface ISysUserService : IDependency,IBaseService<SysUser>
    {
         UserManager<SysUser, int> UserContext { get; set; }
         SysUser UserLoginAsync(string strUser, string strPassword);
    }

    public interface ISysRoleService : IDependency, IBaseService<SysRole>
    {
        RoleManager<SysRole, int> RoleContext { set; get; }
        
    }
}
