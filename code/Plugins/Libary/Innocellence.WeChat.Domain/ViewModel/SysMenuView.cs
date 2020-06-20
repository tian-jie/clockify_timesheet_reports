using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using System.Security.Claims;
namespace Innocellence.WeChat.Domain.Entity
{
	//[Table("ValidUser")]
    public partial class SysMenuView :IViewModel
	{

        /// <summary>
		/// Email
		/// </summary>
		public virtual string Email
		{
			get;
			set;
		}
		/// <summary>
		///     True if the email is confirmed, default is false
		/// </summary>
		public virtual bool EmailConfirmed
		{
			get;
			set;
		}
		/// <summary>
		///     The salted/hashed form of the user password
		/// </summary>
		public virtual string PasswordHash
		{
			get;
			set;
		}
		/// <summary>
		///     A random value that should change whenever a users credentials have changed (password changed, login removed)
		/// </summary>
		public virtual string SecurityStamp
		{
			get;
			set;
		}
		/// <summary>
		///     PhoneNumber for the user
		/// </summary>
		public virtual string PhoneNumber
		{
			get;
			set;
		}
		/// <summary>
		///     True if the phone number is confirmed, default is false
		/// </summary>
		public virtual bool PhoneNumberConfirmed
		{
			get;
			set;
		}
		/// <summary>
		///     Is two factor enabled for the user
		/// </summary>
		public virtual bool TwoFactorEnabled
		{
			get;
			set;
		}
		/// <summary>
		///     DateTime in UTC when lockout ends, any time in the past is considered not locked out.
		/// </summary>
		public virtual DateTime? LockoutEndDateUtc
		{
			get;
			set;
		}
		/// <summary>
		///     Is lockout enabled for this user
		/// </summary>
	
		/// <summary>
		///     User ID (Primary Key)
		/// </summary>
		public  int Id
		{
			get;
			set;
		}
		/// <summary>
		///     User name
		/// </summary>
		public  string MenuName
		{
			get;
			set;
		}
		



        public bool? IsDeleted { get; set; }

        public IViewModel ConvertAPIModel(object obj)
        {
            var entity = (SysMenu)obj;
            Id = entity.Id;
            MenuName = entity.MenuName;


            return this;
        }
 
	}
}
