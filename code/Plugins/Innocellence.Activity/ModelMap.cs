using AutoMapper;
using Innocellence.Activity.Contracts.Entity;
using Innocellence.Activity.Contracts.ViewModel;
using Innocellence.Activity.ModelsView;

namespace Innocellence.Activity.Entity
{

    public class ModelMappers
    {
        public static void MapperRegister()
        {
            //Identity

            //vote
            Mapper.CreateMap<PollingView, PollingEntity>();
            Mapper.CreateMap<PollingQuestionView, PollingQuestionEntity>();
            Mapper.CreateMap<PollingOptionView, PollingOptionEntity>();
            Mapper.CreateMap<PollingResultView, PollingResultEntity>();

            Mapper.CreateMap<AwardView, AwardEntity>();
            Mapper.CreateMap<BarrageView, Barrage>();
            Mapper.CreateMap<BarrageSummaryView, BarrageSummary>();

            Mapper.CreateMap<EventEntityView,EventEntity>();
            Mapper.CreateMap<EventProfileEntityView, EventProfileEntity>();

           
        }

    }
}
