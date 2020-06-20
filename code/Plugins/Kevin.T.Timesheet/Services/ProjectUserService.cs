using Infrastructure.Core.Data;
using Kevin.T.Timesheet.Entities;
using Kevin.T.Timesheet.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Kevin.T.Timesheet.Services
{
    public class ProjectUserService : BaseService<ProjectUser>, IProjectUserService
    {
        ITimeEntryService _timeEntryService;

        public ProjectUserService()
            : base("Timesheet")
        {
        }


        public List<ProjectUser> GetAllProjectUsers()
        {
            return Repository.Entities.Where(a => a.IsDeleted != true).ToList();
        }

        public List<ProjectUser> GetUserByProject(string projectId, string taskId)
        {
            return Repository.Entities.Where(a => a.IsDeleted != true && a.ProjectGid == projectId).ToList();
        }
    }
}
