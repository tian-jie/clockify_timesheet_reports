using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.Weixin.MP.AdvancedAPIs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innocellence.WeChat.Domain.ViewModel
{
    public class WechatMPUserView : IViewModel
    {
        public int Id { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public int GroupId { get; set; }
        public string HeadImgUrl { get; set; }
        public string Language { get; set; }
        public string NickName { get; set; }
        public string OpenId { get; set; }
        public string Province { get; set; }
        public string Remark { get; set; }
        public int Sex { get; set; }
        public int SubScribe { get; set; }
        public long SubScribeTime { get; set; }
        public string UnSubScribeTime { get; set; }
        public bool IsCanceled { get; set; }
        public string TagIdList { get; set; }
        public string UnionId { get; set; }
        public int? AccountManageId { get; set; }
        public List<GroupTagView> GroupList { get; set; }
        public string TagName { get; set; }
        public string UserTagStr { get; set; }
        public string LocationDisplayStr { get; set; }
        public string CustomerNO { get; set; }
        public DateTime? CustomerRegisteredTime { get; set; }

        public string SimUID { get; set; }

        public IViewModel ConvertAPIModel(object model)
        {
            var e = (WechatMPUser)model;
            Id = e.Id;
            City = e.City;
            Country = e.Country;
            GroupId = e.GroupId;
            HeadImgUrl = e.HeadImgUrl;
            Language = e.Language;
            NickName = e.NickName;
            OpenId = e.OpenId;
            Province = e.Province;
            Remark = e.Remark;
            Sex = e.Sex;
            SubScribe = e.SubScribe;
            SubScribeTime = e.SubScribeTime;
            UnSubScribeTime = e.UnSubScribeTime;
            IsCanceled = e.IsCanceled;
            TagIdList = e.TagIdList;
            UnionId = e.UnionId;
            AccountManageId = e.AccountManageId;
            CustomerNO = e.CustomerNO;
            SimUID = e.SimUID;
            CustomerRegisteredTime = e.CustomerRegisteredTime;
            return this;
        }


        public static WechatMPUser ConvertWeChatUserToMpUser(UserInfoJson model, int AccountManageId, int iAPPID)
        {
            return new WechatMPUser
            {
                City = model.city,
                Country = model.country,
                Province = model.province,
                GroupId = model.groupid,
                HeadImgUrl = model.headimgurl,
                Language = model.language,
                NickName = model.nickname,
                OpenId = model.openid,
                Remark = model.remark,
                Sex = model.sex,
                SubScribe = model.subscribe,
                SubScribeTime = model.subscribe_time,
                TagIdList = model.tagid_list != null ? string.Join(",", model.tagid_list) : "",
                UnionId = model.unionid,
                AccountManageId = AccountManageId,
            };

        }
    }
}
