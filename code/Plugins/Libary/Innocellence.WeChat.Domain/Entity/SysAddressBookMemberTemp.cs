using Infrastructure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innocellence.WeChat.Domain.Entity
{
    public class SysAddressBookMemberTemp : EntityBase<int> 
    {
        public string Avatar { get; set; }
        public string Department { get; set; }
        public string FishflowDepartment { get; set; }
        //public List<string> deptLvs { get; set; }
        public string Email { get; set; }
        public int? Gender { get; set; }
        public string Mobile { get; set; }
        public string UserName { get; set; }
        public string EnName { get; set; }
        public string Position { get; set; }
        public int? Status { get; set; }
        public string UserId { get; set; }
        public string WeiXinId { get; set; }
        public string EmployeeStatus { get; set; }
        public string DirectManagerID { get; set; }
        public string City { get; set; }
        public string Birthday { get; set; }
        public string EmployeeNo { get; set; }
        public string GradeCode { get; set; }
        public string CompanyID { get; set; }
        public DateTime? CreateTime { get; set; }
        public int? DeleteFlag { get; set; }
        public int? AccountManageId { get; set; }
        public string TagList { get; set; }
        public string Extend1 { get; set; }

        public SysAddressBookMember ConvertToMember()
        {
            SysAddressBookMember member = new SysAddressBookMember();
            typeof(SysAddressBookMemberTemp).GetProperties(System.Reflection.BindingFlags.Public |
            System.Reflection.BindingFlags.GetProperty |
            System.Reflection.BindingFlags.SetProperty |
            System.Reflection.BindingFlags.Instance |
            System.Reflection.BindingFlags.DeclaredOnly).ToList().ForEach(a =>
            {
                member.GetType().GetProperty(a.Name).SetValue(member, a.GetValue(this));
            });
            return member;
        }
    }
}
