using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Infrastructure.Web.Domain.Entity;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.Contracts.ViewModel;
using Innocellence.WeChat.Domain.ModelsView;
using Innocellence.WeChatTalk.Domain.ViewModel;
using Innocellence.WeChatTalk.Domain.Entity;




namespace Innocellence.WeChatTalk.Domain
{

    public class ModelMappers
    {
        public static void MapperRegister()
        {
            
            Mapper.CreateMap<MultiTalkView, MultiTalk>();
            //Mapper.CreateMap<MeetingInvitationView,MeetingInvitation>();
            //Mapper.CreateMap<MeetingFileView,MeetingFile>();

           
        }

    }
}
