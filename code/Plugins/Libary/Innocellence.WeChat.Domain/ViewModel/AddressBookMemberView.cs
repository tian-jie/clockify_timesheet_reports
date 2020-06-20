using Infrastructure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Innocellence.Weixin.QY.AdvancedAPIs.MailList;
using Innocellence.WeChat.Domain.Entity;

namespace Innocellence.WeChat.Domain.ModelsView 
{
    public class AddressBookMemberView : IViewModel
    {
        public int Id { get; set; }
        public string Avatar { get; set; }
        public int[] DepartmentIds { get; set; }
        public string DisplayDepartmentNames { get; set; }
        //public List<string> deptLvs { get; set; }
        public string Email { get; set; }
        public int? Gender { get; set; }
        public string Mobile { get; set; }
        public string UserName { get; set; }
        public string EnName { get; set; }
        public string Position { get; set; }
        public int? Status { get; set; }
        public string WeiXinId { get; set; }
        public int? EmployeeStatus { get; set; }
        public string LabourUnionStatus { get; set; }
        public string EmployeeNo { get; set; }
        public string GradeCode { get; set; }
        public string City { get; set; }
        public string DirectManagerID { get; set; }
        public string CompanyID { get; set; }
        public string Birthday { get; set; }
        public string UserId { get; set; }
        public int[] TagList { get; set; }
        public IDictionary<string, string> DepartmentTagList { get; set; }

        public DateTime? CreateTime { get; set; }

        public int? AccountManageId { get; set; }

        [JsonIgnore]
        public string Department
        {
            set
            {
                DepartmentIds = JsonConvert.DeserializeObject<int[]>(value);
            }
            get
            {
                if (this.DepartmentIds != null)
                {
                    return JsonConvert.SerializeObject(this.DepartmentIds);
                }
                else
                {
                    return null;
                }
            }
        }


        public IViewModel ConvertAPIModel(object obj)
        {

            if (obj == null) { return this; }
            var value = (SysAddressBookMember)obj;
        
            this.Id = value.Id;
            this.Avatar = value.Avatar;
            this.Department = value.Department;
            this.Mobile = value.Mobile;
            this.Position = value.Position;
            this.UserId = value.UserId;
            this.UserName = value.UserName;
            this.Gender = value.Gender;
            this.Status = value.Status;
            this.Email = value.Email;
            this.WeiXinId = value.WeiXinId;
            if (!string.IsNullOrEmpty(value.EmployeeStatus))
            {
                this.EmployeeStatus = (int)Enum.Parse(typeof(EmployeeStatusEnum), value.EmployeeStatus);
            }
          
            this.City = value.City;
            this.EnName = value.EnName;
            this.EmployeeNo = value.EmployeeNo;
            this.DirectManagerID = value.DirectManagerID;
            this.Birthday = value.Birthday;
            this.CompanyID = value.CompanyID;
            this.GradeCode = value.GradeCode;
            this.TagList = JsonConvert.DeserializeObject<int[]>(value.TagList ?? "[]");

            this.DepartmentIds = JsonConvert.DeserializeObject<int[]>(value.Department ?? "[]");

            this.LabourUnionStatus = value.LabourUnionStatus;

            CreateTime = value.CreateTime;
            AccountManageId = value.AccountManageId;

            return this;
        }

        [JsonIgnore]
        public GetMemberResult ConvertApiModel
        {
            get
            {
                return new GetMemberResult()
                {
                    avatar = this.Avatar,
                    status = this.Status.Value,
                    department = this.DepartmentIds,
                    email = this.Email,
                    gender = this.Gender ?? 0,
                    userid = this.UserId,
                    mobile = this.Mobile,
                    name = this.UserName,
                    position = this.Position,
                    weixinid = this.WeiXinId
                };
            }
            set
            {
                this.Avatar = value.avatar;
                this.Status = value.status;
                this.DepartmentIds = value.department;
                this.Email = value.email;
                this.Gender = value.gender;
                this.UserId = value.userid;
                this.Mobile = value.mobile;
                this.UserName = value.name;
                this.Position = value.position;
                this.WeiXinId = value.weixinid;
            }
        }

        [JsonIgnore]
        public SysAddressBookMember ConvertEntity
        {
            get
            {
                return new SysAddressBookMember()
                {
                    Id = this.Id,
                    Avatar = this.Avatar,
                    Department = this.Department,
                    Mobile = this.Mobile,
                    Position = this.Position,
                    UserId = this.UserId,
                    UserName = this.UserName,
                    Gender = this.Gender,
                    Status = this.Status,
                    Email = this.Email,
                    WeiXinId = this.WeiXinId,
                    CompanyID = this.CompanyID,
                    EnName = this.EnName,
                    EmployeeNo = this.EmployeeNo,
                    Birthday = this.Birthday,
                    GradeCode = this.GradeCode,
                    DirectManagerID = this.DirectManagerID,
                    City = this.City,
                    TagList = JsonConvert.SerializeObject(this.TagList ?? new int[0]),
                    EmployeeStatus = ((EmployeeStatusEnum)(this.EmployeeStatus ?? 0)).ToString(),
                    LabourUnionStatus = LabourUnionStatus,
                };
            }
            set
            {
                this.Id = value.Id;
                this.Avatar = value.Avatar;
                this.Department = value.Department;
                this.Mobile = value.Mobile;
                this.Position = value.Position;
                this.UserId = value.UserId;
                this.UserName = value.UserName;
                this.Gender = value.Gender;
                this.Status = value.Status;
                this.Email = value.Email;
                this.WeiXinId = value.WeiXinId;
                this.EmployeeStatus = (value.EmployeeStatus==null?(int?)null:(int)Enum.Parse(typeof(EmployeeStatusEnum), value.EmployeeStatus));
                this.City = value.City;
                this.EnName = value.EnName;
                this.EmployeeNo = value.EmployeeNo;
                this.DirectManagerID = value.DirectManagerID;
                this.Birthday = value.Birthday;
                this.CompanyID = value.CompanyID;
                this.GradeCode = value.GradeCode;
                this.TagList = JsonConvert.DeserializeObject<int[]>(value.TagList ?? "[]");
                this.LabourUnionStatus = value.LabourUnionStatus;
            }
        }

        [JsonIgnore]
        public AddressBookMemberTemplateModel ConvertTemplateModel
        {
            set
            {
                this.UserId = value.UserId;
                //this.WeiXinId = value.WeiXinId;
                this.UserName = value.UserName;
                this.GradeCode = value.GradeCode;
                this.CompanyID = value.CompanyID;
                this.City = value.City;
                this.Birthday = value.Birthday;
                this.EnName = value.EnName;
                this.DirectManagerID = value.DirectManagerID;
                this.EmployeeNo = value.EmployeeNo;
                this.EmployeeStatus = (int)Enum.Parse(typeof(EmployeeStatusEnum), value.EmployeeStatus ?? "U");
                this.Mobile = value.Mobile;
                this.Email = value.Email;
                this.Position = value.Position;
                if (value.Status == null || value.Status == 0)
                {
                    this.Status = 4; //未关注
                    
                }
                else
                {
                    this.Status = value.Status;
                }
                this.DepartmentIds = value.DepartmentIds;
                this.LabourUnionStatus = value.LabourUnion == null ? null : value.LabourUnion.ToUpper();
                
                if (string.IsNullOrWhiteSpace(value.Gender))
                {
                    this.Gender = 0;
                }
                else if (value.Gender.Trim().Equals("男", StringComparison.InvariantCultureIgnoreCase)
                    || value.Gender.Trim().Equals("Male", StringComparison.InvariantCultureIgnoreCase))
                {
                    this.Gender = 1;
                }
                else if (value.Gender.Trim().Equals("女", StringComparison.InvariantCultureIgnoreCase)
                    || value.Gender.Trim().Equals("Female", StringComparison.InvariantCultureIgnoreCase))
                {
                    this.Gender = 2;
                }
            }
            get
            {
                var result = new AddressBookMemberTemplateModel()
                {
                    UserId = this.UserId,
                    //WeiXinId = this.WeiXinId,
                    EmployeeStatus = this.EmployeeStatus == null ? null : ((EmployeeStatusEnum)this.EmployeeStatus).ToString(),
                    EmployeeNo = this.EmployeeNo,
                    EnName = this.EnName,
                    Birthday = this.Birthday,
                    City = this.City,
                    CompanyID = this.CompanyID,
                    DirectManagerID = this.DirectManagerID,
                    //Gender = this.Gender,
                    GradeCode = this.GradeCode,
                    Position = this.Position,
                    UserName = this.UserName,
                    Mobile = this.Mobile,
                    Email = this.Email,
                };
                result.DepartmentIds = this.DepartmentIds;
                switch (this.Gender)
                {
                    case 1:
                        result.Gender = "Male";
                        break;
                    case 2:
                        result.Gender = "Female";
                        break;
                    default:
                        break;
                }

                return result;
            }
        }

        public enum EmployeeStatusEnum
        {
            U = 0, // 未知
            A = 1, // 在职
            D = 2, // 离职
            T = 3  // 实习
        }
    }
}
