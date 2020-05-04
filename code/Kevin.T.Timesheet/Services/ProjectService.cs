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
        ITimeEntryService _timeEntryService;

        public ProjectService(ITimeEntryService timeEntryService)
            : base("Timesheet")
        {
            _timeEntryService = timeEntryService;
        }


        public List<Project> GetAllProjects()
        {
            return Repository.Entities.Where(a => a.IsDeleted != true).ToList();
        }

        public Project GetProjectById(string Gid)
        {
            return Repository.Entities.FirstOrDefault(a => a.IsDeleted != true && a.Gid == Gid);
        }

        public ProjectAccountingView AccountProject(string Gid)
        {
            // 第一步，获取这个项目的所有clockify信息
            //_timeEntryService

            return null;
        }
    }
}
