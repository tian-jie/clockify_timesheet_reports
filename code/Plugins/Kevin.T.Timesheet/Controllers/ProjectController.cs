using Infrastructure.Web.UI;
using Innocellence.WeChat.Domain;
using Kevin.T.Timesheet.Entities;
using Kevin.T.Timesheet.Interfaces;
using Kevin.T.Timesheet.ModelsView;
using System.Web.Mvc;

namespace Kevin.T.Timesheet.Controllers
{
    public class ProjectController : BaseController<Project, ProjectView>
    {
        private readonly IProjectService _projectService;

        public ProjectController(IProjectService projectService, ITaskToProjectMappingService taskToProjectMappingService)
            : base(projectService)
        {
            _projectService = projectService;
        }

        [HttpPost]
        public override ActionResult GetList()
        {
            var req = new GridRequest(Request);

            // 普通项目
            var projects = _projectService.GetAllActiveProjects();

            return GetPageResult(projects, req);
        }

    }
}