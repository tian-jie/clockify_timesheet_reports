using EntityFramework.BulkInsert.Extensions;
using Infrastructure.Core.Data;
using Kevin.T.Timesheet.Common;
using Kevin.T.Timesheet.Entities;
using Kevin.T.Timesheet.Interfaces;
using Kevin.T.Timesheet.ModelsView;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;

namespace Kevin.T.Timesheet.Services
{
    public class ResourcePlanService : BaseService<ResourcePlan>, IResourcePlanService
    {
        IEmployeeService _employeeService;
        IRoleTitleService _roleTitleService;
        public ResourcePlanService(IEmployeeService employeeService, IRoleTitleService roleTitleService)
            : base("Timesheet")
        {
            _employeeService = employeeService;
            _roleTitleService = roleTitleService;
        }

        public void UploadResourcePlan(string excelFilename, string projectGid)
        {
            // 先处理excel文件到data list
            IWorkbook wk = null;
            string extension = System.IO.Path.GetExtension(excelFilename);//GetExtension获取Excel的扩展名

            List<YearWeekStructure> yearWeekStructures = new List<YearWeekStructure>();
            try
            {
                FileStream fs = File.OpenRead(excelFilename);
                if (extension.Equals(".xls"))
                {
                    wk = new HSSFWorkbook(fs); //把xls文件中的数据写入wk中
                }
                else
                {
                    wk = new XSSFWorkbook(fs);//把xlsx文件中的数据写入wk中
                }
                fs.Close();
                var sheet = wk.GetSheet("Hours"); //读取当前表数据   20            GetIndexRow();//获取【指标、科目、数据】的行数列数

                // 计算有哪些week
                IRow row = sheet.GetRow(2);  // 读取日期行的数据
                for (var i = 6; true; i++)
                {
                    var cell = row.GetCell(i);
                    if (cell == null)
                    {
                        break;
                    }
                    var date = cell.DateCellValue;

                    // 根据date计算当前的周
                    var week = date.WeekOfYear();
                    var year = date.YearOfWeekOfYear();

                    yearWeekStructures.Add(new YearWeekStructure { Column = i, Year = year, Week = week });
                }

                // 遍历员工，开始处理Resource Plan数据
                var allEmployees = _employeeService.AllEmployeesWithRole();
                var resourcePlans = new List<ResourcePlan>();
                for (var rowNum = 3; rowNum < 32; rowNum++)
                {
                    row = sheet.GetRow(rowNum);
                    var cell = row.GetCell(1, MissingCellPolicy.RETURN_BLANK_AS_NULL);
                    if (cell == null)
                    {
                        continue;
                    }
                    // 根据员工姓名，反查员工id
                    var employeeName = cell.StringCellValue;

                    if (string.IsNullOrEmpty(employeeName))
                    {
                        continue;
                    }
                    var employeeInfo = allEmployees.FirstOrDefault(a => a.Name.Equals(employeeName, StringComparison.OrdinalIgnoreCase));

                    if (employeeInfo == null)
                    {
                        throw new Exception("Invalid Employee Name - " + employeeName);
                    }

                    // 如果查到了，就从头开始计算数据，0就不存了，以减少数据量
                    foreach (var yw in yearWeekStructures)
                    {
                        // 取数据
                        var cell0 = row.GetCell(yw.Column, MissingCellPolicy.RETURN_BLANK_AS_NULL);
                        double effort = 0;

                        if (cell0 == null)
                        {
                            continue;
                        }

                        effort = cell0.NumericCellValue;
                        if (effort == 0)
                        {
                            continue;
                        }

                        var reroucePlan = new ResourcePlan()
                        {
                            Amount = (decimal)effort,
                            EmployeeGid = employeeInfo.Gid,
                            ProjectGid = projectGid,
                            Year = yw.Year,
                            Week = yw.Week,
                            IsDeleted = false
                        };

                        resourcePlans.Add(reroucePlan);
                    }
                }

                // 然后清空这个project的所有plan数据
                DeleteByProject(projectGid);

                // 最后批量导入这个project的plan数据
                ((DbContext)Repository.UnitOfWork).BulkInsert<ResourcePlan>(resourcePlans, 1000);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message); //只在Debug模式下才输出
            }

        }

        public List<ResourcePlan> GetResourcePlanByProject(string projectGid)
        {
            return Repository.Entities.Where(a => a.ProjectGid == projectGid && a.IsDeleted != true).ToList();
        }

        public void DeleteByProject(string projectGid)
        {
            string sql = $"Update ResourcePlan set isDeleted=1, UpdatedDate=GETDATE() where projectGid='{projectGid}'";
            Repository.SqlExcute(sql);
        }

        public List<TimeEntriesGroupByEmployeeView> GetTimeEntriesByProjectGroupByEmployee(string projectGid)
        {
            // 获取项目相关的员工
            var allUsers = _employeeService.AllEmployeesWithRole();
            var roleTitles = _roleTitleService.AllInternal();


            // 看这个projectGid，如果属于taskgid，就从taskgid过滤
            //var task = _projectTaskService.GetProjectTaskById(projectGid);

            // 再根据每个员工统计timeentry
            //var timeEntry = Repository.Entities.Where(a => a.IsDeleted != true);
            //if (task != null && task.Count > 0)
            //{
            //    timeEntry = timeEntry.Where(a => a.TaskId == projectGid);
            //}
            //else
            //{
            //    timeEntry = timeEntry.Where(a => a.ProjectId == projectGid);
            //}


            var resourcePlans = GetResourcePlanByProject(projectGid);
            var timeEntryByUsers = resourcePlans.GroupBy(a => a.EmployeeGid);

            var resorcePlansByEmployeesView = new List<TimeEntriesGroupByEmployeeView>();

            foreach (var a in timeEntryByUsers)
            {
                var employee = allUsers.FirstOrDefault(b => b.Gid == a.Key);

                var tv = new TimeEntriesGroupByEmployeeView()
                {
                    UserId = a.Key,
                    TotalHours = a.Sum(b => b.Amount),
                    EmployeeRate = employee.RoleRate,
                    EmployeeRole = employee.RoleName,
                    TotalHoursRate = a.Sum(b => b.Amount) * employee.RoleRate,
                    EmployeeName = employee.Name
                };

                tv.TotalEffortByWeek = new List<TotalEffortByWeek>();

                // 计算周数
                foreach (var v in a)
                {
                    // 计算这个日期属于第几周
                    var week = v.Week;
                    var year = v.Year;

                    var totalEffortByWeek = tv.TotalEffortByWeek.FirstOrDefault(b => b.WeekNumber == week);
                    tv.TotalEffortByWeek.Add(new TotalEffortByWeek()
                    {
                        Year = year,
                        WeekNumber = week,
                        TotalHours = v.Amount,
                        TotalHoursRate = v.Amount * employee.RoleRate
                    });

                }

                resorcePlansByEmployeesView.Add(tv);
            }
            return resorcePlansByEmployeesView;
        }

    }

    class YearWeekStructure
    {
        public int Column { get; set; }
        public int Year { get; set; }
        public int Week { get; set; }
    }
}
