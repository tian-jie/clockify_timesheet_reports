using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innocellence.WeChat.Domain.ViewModel
{
    public class WechatUserRequestMessageTagView : IViewModel
    {

        public int Id { get; set; }

        public int AppID { get; set; }

        public String TagName { get; set; }

        public Boolean IsDeleted { get; set; }

        public String CreatedUserID { get; set; }

        public DateTime? CreatedDate { get; set; }

        public String UpdatedUserID { get; set; }

        public DateTime? UpdatedDate { get; set; }


        public IViewModel ConvertAPIModel(object obj)
        {
            var entity = (WechatUserRequestMessageTag)obj;
            Id = entity.Id;
            AppID = entity.AppID;
            TagName = entity.TagName;
            IsDeleted = entity.IsDeleted;
            CreatedUserID = entity.CreatedUserID;
            CreatedDate = entity.CreatedDate;
            UpdatedUserID = entity.UpdatedUserID;
            UpdatedDate = entity.UpdatedDate;
            return this;
        }

        public WechatUserRequestMessageTag ConvertToEntity()
        {
            var entity = new WechatUserRequestMessageTag();
            entity.Id = Id;
            entity.AppID = AppID;
            entity.TagName = TagName;
            entity.IsDeleted = IsDeleted;
            entity.CreatedUserID = CreatedUserID;
            entity.CreatedDate = CreatedDate;
            entity.UpdatedUserID = UpdatedUserID;
            entity.UpdatedDate = UpdatedDate;
            return entity;
        }
    }
}
