using Infrastructure.Core.Logging;
using Infrastructure.Web.UI;
using Innocellence.Web.Controllers;
using Kevin.T.Timesheet.Entities;
using Kevin.T.Timesheet.Interfaces;
using Kevin.T.Timesheet.ModelsView;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Kevin.T.Timesheet.Controllers
{
    public class ProjectController : BaseController<Project, ProjectView>
    {
        private readonly IProjectService _projectService;

        public ProjectController(IProjectService projectService)
            : base(projectService)
        {
            _projectService = projectService;
        }

        [HttpPost]
        public override ActionResult GetList()
        {
            var req = new GridRequest(Request);

            var projects = _projectService.GetAllProjects();

            return GetPageResult(projects, req);
        }

    }
}