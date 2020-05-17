using Infrastructure.Core.Data;
using Kevin.T.Timesheet.Interfaces;
using Kevin.T.Timesheet.Entities;
using System.Collections.Generic;
using System.Linq;
using Kevin.T.Timesheet.ModelsView;

namespace Kevin.T.Timesheet.Services
{
    public class ProjectService : BaseService<Project>, IProjectService
    {
        IEstimateEffortService _estimateEffortService;

        public ProjectService(IEstimateEffortService estimateEffortService)
            : base("Timesheet")
        {
            _estimateEffortService = estimateEffortService;
        }


        public List<Project> GetAllProjects()
        {
            return Repository.Entities.Where(a => a.IsDeleted != true).ToList();
        }

        public Project GetProjectById(string Gid)
        {
            return Repository.Entities.FirstOrDefault(a => a.IsDeleted != true && a.Gid == Gid);
        }

        public ProjectAccountingView GetProjectEstimatedEffortById(string Gid)
        {
            var efforts = _estimateEffortService.Repository.Entities.Where(a => a.ProjectGid == Gid);
            var estimatedSpentManHour = efforts.Sum(a => a.Effort);
            var estimatedSpentManHourRate = efforts.Sum(a => (a.Effort * a.RoleRate));

            return new ProjectAccountingView()
            {
                ProjectGid = Gid,
                EstimatedSpentManHour = estimatedSpentManHour,
                EstimatedSpentManHourRate = estimatedSpentManHourRate
            };
        }

        public ProjectAccountingView AccountProject(string Gid)
        {
            // 第一步，获取这个项目的所有clockify信息
            //_timeEntryService

            return null;
        }
    }
}
