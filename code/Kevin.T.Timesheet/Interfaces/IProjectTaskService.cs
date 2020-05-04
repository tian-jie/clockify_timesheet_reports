using Infrastructure.Core;
using Kevin.T.Timesheet.Entities;
using Kevin.T.Timesheet.ModelsView;
using System.Collections.Generic;

namespace Kevin.T.Timesheet.Interfaces
{
    public interface IProjectTaskService : IDependency, IBaseService<ProjectTask>
    {
        List<ProjectTask> GetAllProjectTasks();

        List<ProjectTask> GetProjectTaskById(string Gid);

    }
}
