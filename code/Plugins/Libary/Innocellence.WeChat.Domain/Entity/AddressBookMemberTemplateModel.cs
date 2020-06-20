using Infrastructure.Utility.IO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innocellence.WeChat.Domain.Entity
{
    public class AddressBookMemberTemplateModel
    {
        [Description("CName")]
        [UploadColumn("姓名")]
        public string UserName { get; set; }
        [UploadColumn("帐号")]
        public string UserId { get; set; }
        [UploadColumn("微信号")]
        public string WeiXinId { get; set; }
        [Description("Phone")]
        [UploadColumn("手机号")]
        public string Mobile { get; set; }
        [Description("Email")]
        [UploadColumn("邮箱")]
        public string Email { get; set; }
        public int[] DepartmentIds { get; set; }
        public int? Status { get; set; }
        [UploadColumn("所在部门")]
        public string Department 
        { 
            get 
            {
                if (this.DepartmentIds != null && this.DepartmentIds.Length > 0)
                {
                    return string.Join(";", this.DepartmentIds);
                }
                else
                {
                    return string.Empty;
                }
            } 
        }
        [Description("Position")]
        [UploadColumn("职位")]
        public string Position { get; set; }
        [Description("EmployeeNo")]
        public string EmployeeNo { get; set; }
        [Description("OrgName")]
        public string OrgName { get; set; }
        [Description("OrgFullName")]
        public string OrgFullName { get; set; }
        [Description("GradeCode")]
        public string GradeCode { get; set; }
        [Description("City")]
        public string City { get; set; }
        [Description("Birthday")]
        public string Birthday { get; set; }
        [Description("DirectManagerID")]
        public string DirectManagerID { get; set; }
        [Description("EName")]
        public string EnName { get; set; }
        [Description("Gender")]
        [UploadColumn("性别")]
        public string Gender { get; set; }
        [Description("EmployeeStatus")]
        public string EmployeeStatus { get; set; }
        [Description("CompanyID")]
        public string CompanyID { get; set; }
        [Description("LabourUnion")]
        public string LabourUnion { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class UploadColumnAttribute : Attribute
    {
        public string ColumnName { get; set; }
        public UploadColumnAttribute(string columnName)
        {
            ColumnName = columnName;
        }
    }
}
