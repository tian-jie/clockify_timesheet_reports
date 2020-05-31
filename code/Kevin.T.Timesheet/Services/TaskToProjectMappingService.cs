using Infrastructure.Core.Data;
using Kevin.T.Timesheet.Interfaces;
using Kevin.T.Timesheet.Entities;
using System.Collections.Generic;
using System.Linq;

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
    }
}
