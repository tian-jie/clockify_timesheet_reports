using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;

namespace Innocellence.WeChat.Domain.ModelsView
{
	//[Table("Push")]
    public partial class SysUserView : IViewModel
	{
	
		public Int32 Id { get;set; }

        public String LillyId { get; set; }

        public String UserName { get; set; }
        public String Email { get; set; }
        public DateTime? CreatedDate { get; set; }
        public String CreatedUserID { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public String UpdatedUserID { get; set; }

        public String UserTrueName { get; set; }

        public String PhoneNumber { get; set; }

        public String PasswordHash { get; set; }
        
        
        public IViewModel ConvertAPIModel(object obj){
        var entity = (SysUser)obj;
	    Id =entity.Id;
        LillyId = entity.LillyId;
        UserName = entity.UserName;
        Email = entity.Email;
        PhoneNumber = entity.PhoneNumber;
        UserTrueName = entity.UserTrueName;
 
        return this;
        }
	}
}
