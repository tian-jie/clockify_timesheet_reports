using Infrastructure.Utility.Data;
using Infrastructure.Web.UI;
using Innocellence.WeChat.Domain;
using Kevin.T.Timesheet.Entities;
using Kevin.T.Timesheet.Interfaces;
using Kevin.T.Timesheet.ModelsView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Razor;

namespace Kevin.T.Timesheet.Controllers
{
    public class ProjectUserController : BaseController<ProjectUser, ProjectUserView>
    {
        private readonly IProjectUserService _projectUserService;
        private readonly IProjectService _projectService;
        private readonly IEmployeeService _employeeService;
        private readonly IRoleTitleService _roleTitleService;
        private readonly ITimeEntryService _timeEntryService;

        public ProjectUserController(IProjectUserService projectUserService
            , IProjectService projectService
            , IEmployeeService employeeService
            , IRoleTitleService roleTitleService
            , ITimeEntryService timeEntryService)
            : base(projectUserService)
        {
            _projectUserService = projectUserService;
            _projectService = projectService;
            _employeeService = employeeService;
            _roleTitleService = roleTitleService;
            _timeEntryService = timeEntryService;
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
            var q = _objService.GetList<ProjectUserView>(predicate.AndAlso(x => x.IsDeleted != true), ConPage);

            return q.ToList();
        }

        public JsonResult GetData(string projectGid)
        {
            Expression<Func<ProjectUser, bool>> predicate = m => m.ProjectGid == projectGid;
            PageCondition pageCondition = new PageCondition(1, 50);

            var list = GetListEx(predicate, pageCondition);

            // 上面取标准的，下面再加上项目里填过但没配置进来的
            var extraEmployees = _timeEntryService.GetEmployeeByProjectTimeEntry(projectGid);
            foreach (var emp in extraEmployees)
            {
                if(list.FirstOrDefault(a=>a.UserGid == emp.Gid) != null)
                {
                    continue;
                }
                list.Add(new ProjectUserView()
                {
                    EmployeeName = emp.Name,
                    Id = 0,
                    ProjectGid = projectGid,
                    Rate = 1,
                    UserGid = emp.Gid,
                    UserRoleTitle = "N/A",
                    UserRoleTitleId = 0
                });
            }

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveData(string projectGid, List<ProjectUserView> projectUserViews)
        {
            foreach (var projectUserView in projectUserViews)
            {
                projectUserView.ProjectGid = projectGid;

                if (projectUserView.Id == 0)
                {
                    _projectUserService.InsertView(projectUserView);
                }
                else
                {
                    _projectUserService.UpdateView(projectUserView);
                }
            }

            return Json(new { status = 200 }, JsonRequestBehavior.AllowGet);
        }

    }
}