using AutoMapper;
using Kevin.T.Timesheet.Entities;
using Kevin.T.Timesheet.ModelsView;

namespace Kevin.T.Timesheet
{

    public class ModelMappers
    {
        public static void MapperRegister()
        {
            Mapper.CreateMap<EstimateEffortView, EstimateEffort>();
            Mapper.CreateMap<ProjectUserView, ProjectUser>();
            Mapper.CreateMap<ProjectView, Project>();
            Mapper.CreateMap<TimeEntryView, TimeEntry>();
           
        }

    }
}
