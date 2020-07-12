using Infrastructure.Core;
using Kevin.T.Timesheet.Entities;
using Kevin.T.Timesheet.ModelsView;
using System.Collections.Generic;

namespace Kevin.T.Timesheet.Interfaces
{
    public interface IProjectService : IDependency, IBaseService<Project>
    {
        List<Project> GetAllProjects();

        List<Project> GetAllActiveProjects();

        Project GetProjectById(string Gid);

        ProjectAccountingView AccountProject(string Gid);

        ProjectAccountingView GetProjectEstimatedEffortById(string Gid);
    }
}
