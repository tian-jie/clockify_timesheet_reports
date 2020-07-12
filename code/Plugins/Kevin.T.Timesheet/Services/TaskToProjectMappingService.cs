using Infrastructure.Core.Data;
using Kevin.T.Timesheet.Interfaces;
using Kevin.T.Timesheet.Entities;
using System.Collections.Generic;
using System.Linq;
using Kevin.T.Timesheet.ModelsView;

namespace Kevin.T.Timesheet.Services
{
    public class TaskToProjectMappingService : BaseService<TaskToProjectMapping>, ITaskToProjectMappingService
    {
        public TaskToProjectMappingService()
            : base("Timesheet")
        {

        }

        public List<TaskToProjectMapping> All()
        {
            return Repository.Entities.Where(a => a.IsDeleted != true).ToList();
        }

        public List<Project> AllProjects()
        {
            var mapping = Repository.Entities.Where(a => a.IsDeleted != true).GroupBy(a => new { a.ProjectGid, a.ProjectName });
            var projects = new List<Project>();
            foreach (var p in mapping)
            {
                projects.Add(new Project()
                {
                    Id = 0,
                    Gid = p.Key.ProjectGid,
                    Name = p.Key.ProjectName
                });
            }

            return projects;
        }

    }
}
