using Infrastructure.Core;
using Kevin.T.Timesheet.Entities;
using System.Collections.Generic;

namespace Kevin.T.Timesheet.Interfaces
{
    public interface IProjectUserService : IDependency, IBaseService<ProjectUser>
    {
        List<ProjectUser> GetAllProjectUsers();

        List<ProjectUser> GetUserByProject(string projectId, string taskId);

    }
}
