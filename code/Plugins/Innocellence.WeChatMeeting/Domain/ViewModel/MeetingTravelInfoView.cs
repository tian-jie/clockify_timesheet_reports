using Infrastructure.Core;
using Innocellence.WeChatMeeting.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Innocellence.WeChatMeeting.Domain.ViewModel
{
    public class MeetingTravelInfoView : IViewModel
    {
        public MeetingTravelInfoView() {
            MeetingTravelInfoViews = new List<MeetingTravelInfoView>();
            List = new List<MeetingTravelInfoView>();
        }

       

        public  Int32 Id { get; set; }
        public int? MeetingId { get; set; }
        public String UserId { get; set; }
        public String UserName { get; set; }
        public String Mode { get; set; }
        public String Flight_Train { get; set; }
        public DateTime? DepartureTime { get; set; }
        public DateTime? ArrivalTime { get; set; }
        public String ExpectArrivalTime { get; set; }
        public DateTime? CreatedDate { get; set; }
        public String Remarks { get; set; }
        public Boolean? IsDeleted { get; set; }
        public String Type { get; set; }


        public IList<MeetingTravelInfoView> List { get; set; }
        public List<MeetingTravelInfoView> MeetingTravelInfoViews { get; set; }




        public IViewModel ConvertAPIModel(object obj)
        {
            var entity = (MeetingTravelInfo)obj;
            Id = entity.Id;
            MeetingId = entity.MeetingId;
            UserId = entity.UserId;
            UserName = entity.UserName;
            Mode = entity.Mode;
            Flight_Train = entity.Flight_Train;
            DepartureTime = entity.DepartureTime;
            ArrivalTime = entity.ArrivalTime;
            ExpectArrivalTime = entity.ExpectArrivalTime;
            Type = entity.Type;
            Remarks = entity.Remarks;
            CreatedDate = entity.CreatedDate;
            IsDeleted = entity.IsDeleted;

            return this;
        }


    }
}