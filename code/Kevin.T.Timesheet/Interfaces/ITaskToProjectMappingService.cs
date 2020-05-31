using Infrastructure.Core;
using Kevin.T.Timesheet.Entities;
using System.Collections.Generic;

namespace Kevin.T.Timesheet.Interfaces
{
    public interface ITaskToProjectMappingService : IDependency, IBaseService<TaskToProjectMapping>
    {
        List<TaskToProjectMapping> All();
    }
}
