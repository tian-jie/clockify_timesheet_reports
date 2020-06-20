using Infrastructure.Core.Data;
using Kevin.T.Timesheet.Entities;
using Kevin.T.Timesheet.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Kevin.T.Timesheet.Services
{
    public class ProjectTaskService : BaseService<ProjectTask>, IProjectTaskService
    {
        public ProjectTaskService()
            : base("Timesheet")
        {
        }


        public List<ProjectTask> GetAllProjectTasks()
        {
            return Repository.Entities.Where(a => a.IsDeleted != true).ToList();
        }

        public List<ProjectTask> GetProjectTaskById(string Gid)
        {
            return Repository.Entities.Where(a => a.IsDeleted != true && a.Gid == Gid).ToList();
        }

    }
}
