using Infrastructure.Core.Logging;
using Infrastructure.Utility.Data;
using Infrastructure.Web.UI;
using Innocellence.Web.Controllers;
using Kevin.T.Timesheet.Entities;
using Kevin.T.Timesheet.Interfaces;
using Kevin.T.Timesheet.ModelsView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace Kevin.T.Timesheet.Controllers
{
    public class ProjectUserController : BaseController<ProjectUser, ProjectUserView>
    {
        private readonly IProjectUserService _projectUserService;
        private readonly IProjectService _projectService;
        private readonly IEmployeeService _employeeService;
        private readonly IRoleTitleService _roleTitleService;

        public ProjectUserController(IProjectUserService projectUserService
            , IProjectService projectService
            , IEmployeeService employeeService
            , IRoleTitleService roleTitleService)
            : base(projectUserService)
        {
            _projectUserService = projectUserService;
            _projectService = projectService;
            _employeeService = employeeService;
            _roleTitleService = roleTitleService;
        }

        public override ActionResult Index()
        {
            ViewBag.Projects = _projectService.Repository.Entities.Where(a => a.IsDeleted == false).ToList();
            ViewBag.Employees = _employeeService.Repository.Entities.Where(a => a.IsDeleted == false).ToList();
            ViewBag.RoleTitle = _roleTitleService.Repository.Entities.Where(a => a.IsDeleted == false).ToList();
            return base.Index();
        }

        //初始化list页面
        public override List<ProjectUserView> GetListEx(Expression<Func<ProjectUser, bool>> predicate, PageCondition ConPage)
        {
            string strProjectGid = Request["ProjectName"];

            if (!string.IsNullOrEmpty(strProjectGid))
            {
                predicate = predicate.AndAlso(x => x.ProjectGid.Contains(strProjectGid));
            }

            var q = _objService.GetList<ProjectUserView>(predicate.AndAlso(x => x.IsDeleted == false), ConPage);

            return q.ToList();
        }

    }
}