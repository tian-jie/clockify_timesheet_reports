using Infrastructure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innocellence.WeChat.Domain.Entity
{
    public class WechatMPUser : EntityBase<int>
    {
        public override int Id { get; set; }
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
        public string SimUID { get; set; }
        public int? AccountManageId { get; set; }
        public string CustomerNO { get; set; }
        public DateTime? CustomerRegisteredTime { get; set; }
    }
}
