using Infrastructure.Core.Data;
using Innocellence.Activity.Contracts.Entity;
using Innocellence.Activity.Entity;
using Innocellence.Activity.Model;

namespace Innocellence.Activity.Configuration
{
   
    public class BarrageExtConfiguration : EntityConfigurationBase<BarrageExt, int>
    {
        public BarrageExtConfiguration()
        {
            ToTable("BarrageExt");
        }
    }
   
    public class WXScreenConfiguration : EntityConfigurationBase<WXScreen, int>
    {
        public WXScreenConfiguration()
        {
            ToTable("WXScreen");
        }
    }

    public class EventConfiguration : EntityConfigurationBase<EventEntity, int>
    {
        public EventConfiguration()
        {
            ToTable("Event");
        }
    }

    public class EventProfileConfigration : EntityConfigurationBase<EventProfileEntity, int>
    {
        public EventProfileConfigration()
        {
            ToTable("EventProfile");
        }
    }

    public class PollingConfiguration : EntityConfigurationBase<PollingEntity, int>
    {
        public PollingConfiguration()
        {
            ToTable("Polling");
        }
    }

    public class PollingOptionConfiguration : EntityConfigurationBase<PollingOptionEntity, int>
    {
        public PollingOptionConfiguration()
        {
            ToTable("PollingOption");
        }
    }

    public class PollingQuestionConfiguration : EntityConfigurationBase<PollingQuestionEntity, int>
    {
        public PollingQuestionConfiguration()
        {
            ToTable("PollingQuestion");
        }
    }

    public class PollingResultConfiguration : EntityConfigurationBase<PollingResultEntity, int>
    {
        public PollingResultConfiguration()
        {
            ToTable("PollingResult");
        }
    }
    public class PollingAnswerConfiguration : EntityConfigurationBase<PollingAnswerEntity, int>
    {
        public PollingAnswerConfiguration()
        {
            ToTable("PollingAnswer");
        }
    }
    public class PollingResultTempConfiguration : EntityConfigurationBase<PollingResultTempEntity, int>
    {
        public PollingResultTempConfiguration()
        {
            ToTable("PollingResultTemp");
        }
    }
    public class AnnualCheckinConfiguration : EntityConfigurationBase<AnnualCheckinEntity, int>
    {
        public AnnualCheckinConfiguration()
        {
            ToTable("AnnualCheckin");
        }
    }
}

