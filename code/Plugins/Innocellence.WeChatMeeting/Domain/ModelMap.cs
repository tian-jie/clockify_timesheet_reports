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
using Innocellence.WeChatMeeting.Domain.ViewModel;
using Innocellence.WeChatMeeting.Domain.Entity;




namespace Innocellence.WeChatMeeting.Domain
{

    public class ModelMappers
    {
        public static void MapperRegister()
        {
            
            Mapper.CreateMap<MeetingView,Meeting>();
            Mapper.CreateMap<MeetingInvitationView,MeetingInvitation>();
            Mapper.CreateMap<MeetingFileView,MeetingFile>();
            Mapper.CreateMap<MeetingPerInfoView,MeetingPerInfo>();
            Mapper.CreateMap<MeetingTravelInfoView,MeetingTravelInfo>();
           
        }

    }
}
