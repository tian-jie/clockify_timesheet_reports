using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
namespace Innocellence.Web.Entity
{
	//[Table("ValidUser")]
    public partial class SysMenuModel : EntityBase<int>
	{

        /// <summary>
        ///     UserId for the user that is in the role
        /// </summary>
        public override int Id
        {
            get;
            set;
        }
        /// <summary>
        ///     RoleId for the role
        /// </summary>
        public  string  MenuName
        {
            get;
            set;
        }
 
	}
}
