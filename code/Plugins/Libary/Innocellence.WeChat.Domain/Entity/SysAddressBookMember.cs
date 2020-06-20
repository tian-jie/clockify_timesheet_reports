using AutoMapper;
using Infrastructure.Core;
using Innocellence.WeChat.Domain.ModelsView;
using Innocellence.Weixin.QY.AdvancedAPIs.MailList;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innocellence.WeChat.Domain.Entity
{
    public partial class SysAddressBookMember : EntityBase<int> 
    {
        //public override int Id { get; set; }
        //腾讯后台头像
        public string Avatar { get; set; }
        /// <summary>
        /// 总部门信息，等于丁香园部门和fishflow部门的合集
        /// </summary>
        public string Department { get; set; }
        /// <summary>
        /// 丁香园部门
        /// </summary>
        public string DXYDepartment { get; set; }
        /// <summary>
        /// fishflow部门
        /// </summary>
        public string FishflowDepartment { get; set; }
        //public List<string> deptLvs { get; set; }
        public string Email { get; set; }
        public int? Gender { get; set; }
        public string Mobile { get; set; }
        public string UserName { get; set; }
        public string EnName { get; set; }
        public string Position { get; set; }
        /// <summary>
        /// 关注状态
        /// </summary>
        public int? Status { get; set; }
        public string UserId { get; set; }
        public string WeiXinId { get; set; }
        /// <summary>
        /// 在职离职状态
        /// </summary>
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
        //是否是工会成员 "Y" ,"N"
        public string LabourUnionStatus { get; set; }
        //fishflow传来的ADAccount
        public string Extend1 { get; set; }
        public DateTime? SubscribeTime { get; set; }
    }
}
