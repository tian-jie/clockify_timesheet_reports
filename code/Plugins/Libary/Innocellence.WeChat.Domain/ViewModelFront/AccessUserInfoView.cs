using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.Services;
using Innocellence.WeChat.Domain.ViewModelFront;
using Innocellence.Weixin.Annotations;
using Innocellence.Weixin.HttpUtility;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Innocellence.WeChat.Domain.ViewModel
{
    public class AccessUserInfoView: IViewModel
    {

        public AccessUserInfoView()
        {
            departmentList = new List<AccessDepartmentView>();
            tagList = new List<TagEntity>();
        }

        // TODO: 还有几个像key是随机数的属性不知道

        public Int32 Id { get; set; }

        // 职位
        public string position { get; set; }

        // 创建时间
        public long createTime { get; set; }

        // 生日
        public string birthday { get; set; }

        // 联系方式
        public string contactNumber { get; set; }

        // 微信ID
        public string wxid { get; set; }

        // 所属部门List
        public List<AccessDepartmentView> departmentList { get; set; }

        // 城市
        public string cityName { get; set; }

        // 注册地址
        public string registerAddress { get; set; }

        // TEAM ID
        public int? teamId { get; set; }

        // 城市编码？
        public string city { get; set; }

        // 合同签订日（入职时间？）
        public string contactSignTime { get; set; }

        public string trainingAgreement { get; set; }

        public string name { get; set; }

        public string userId { get; set; }

        public string gender { get; set; }

        public string major { get; set; }

        public string university { get; set; }

        public string citizenshipNumber { get; set; }

        public int? status { get; set; }

        public string avatar { get; set; }

        public string englishName { get; set; }

        public string confidentialityAgreement { get; set; }

        public string degree { get; set; }

        public string email { get; set; }

        public string address { get; set; }

        // TODO: TagEntity的属性也不全，需要id，name，applicationId
        public List<TagEntity> tagList { get; set; }

        public string publicAvatarPath { get; set; }

        public string hrcode { get; set; }

        [JsonProperty("7a615e3cbd6585440936dd0a907ff035")]
        public string hrcodeNew { get; set; }

        public string mobile { get; set; }

        public IViewModel ConvertAPIModel(object model)
        {
            ////TODO, DB中现在没有的字段暂时赋为空置
            //var obj = (SysAddressBookMember)model;
            //var userView = new AccessUserInfoView();
            //userView.id = obj.Id;
            //userView.position = obj.Position;
            //userView.createTime = "";
            //userView.birthday = "";
            //userView.contactNumber = obj.Mobile; // TODO: 还有个属性Mobile
            //userView.wxid = obj.WeiXinId;
            ////userView.departmentList
            //// TODO: 如何将string 类型的user.Department转化为List类型

            //userView.cityName = "";
            //userView.registerAddress = "";
            //userView.teamId = "";
            //userView.city = "";
            //userView.contactSignTime = "";
            //userView.trainingAgreement = "";
            //userView.name = "";
            //userView.userId = "";
            //userView.gender = "";
            //userView.major = "";
            //userView.university = "";
            //userView.citizenshipNumber = "";
            //userView.status = "";
            //userView.avatar = "";
            //userView.englishName = "";
            //userView.confidentialityAgreement = "";
            //userView.degree = "";
            //userView.email = "";
            //userView.address = "";
            //// TODO tag list
            ////userView.tagList = null;

            //userView.publicAvatarPath = "";
            //userView.hrcode = "";
            //userView.mobile = "";

            return this;
        }
    }
}
