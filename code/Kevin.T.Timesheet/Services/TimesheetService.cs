using EntityFramework.BulkInsert.Extensions;
using Infrastructure.Core.Logging;
using Kevin.T.Timesheet.Entities;
using Kevin.T.Timesheet.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Kevin.T.Timesheet.Services
{
    public class TimesheetService : ITimesheetService
    {
        IClientService _clientService;
        IEmployeeService _employeeService;
        IGroupService _groupService;
        IProjectService _projectService;
        ITimeEntryService _timeEntryService;
        IClockifyService _clockifyService;
        IUserGroupService _userGroupService;
        IProjectTaskService _projectTaskService;
        ILogger _logger = LogManager.GetLogger("Banner Management");

        public TimesheetService(IClientService clientService,
            IEmployeeService employeeService,
            IGroupService groupService,
            IProjectService projectService,
            ITimeEntryService timeEntryService,
            IClockifyService clockifyService,
            IUserGroupService userGroupService,
            IProjectTaskService projectTaskService)
        {
            _clientService = clientService;
            _employeeService = employeeService;
            _groupService = groupService;
            _projectService = projectService;
            _timeEntryService = timeEntryService;
            _clockifyService = clockifyService;
            _userGroupService = userGroupService;
            _projectTaskService = projectTaskService;
        }

        public async Task<int> SyncUsers(string userid, string token)
        {
            var currentUser = HttpContext.Current.User.Identity.Name;

            // 先到clockify获取数据
            var ls = await _clockifyService.GetUsers(userid, token);
            var list = new List<Employee>();
            foreach (var l in ls)
            {
                list.Add(new Employee()
                {
                    Gid = l.id,
                    Name = l.name,
                    Email = l.email,
                    ProfilePicture = l.profilePicture,
                    Status = l.status,
                    CreatedDate = DateTime.Now,
                    CreatedUserID = currentUser,
                    CreatedUserName = currentUser,
                    IsDeleted = false,
                    UpdatedDate = DateTime.Now,
                    UpdatedUserID = currentUser,
                    UpdatedUserName = currentUser
                });
            }

            // 然后在本地插入或者同步数据
            var affectedRecordCount = _employeeService.Repository.SqlExcute("truncate table employee");

            affectedRecordCount = await _employeeService.Repository.InsertAsync(list);

            return affectedRecordCount;
        }

        public async Task<int> SyncGroups(string userid, string token)
        {
            var currentUser = HttpContext.Current.User.Identity.Name;

            // 先到clockify获取数据
            var ls = await _clockifyService.GetUserGroups(userid, token);
            var list = new List<Entities.Group>();

            var userGroups = new List<UserGroup>();
            foreach (var l in ls)
            {
                list.Add(new Entities.Group()
                {
                    Gid = l.id,
                    Name = l.name,
                    CompanyId = "",
                    WorkspaceId = l.workspaceId,
                    CreatedDate = DateTime.Now,
                    CreatedUserID = currentUser,
                    CreatedUserName = currentUser,
                    IsDeleted = false,
                    UpdatedDate = DateTime.Now,
                    UpdatedUserID = currentUser,
                    UpdatedUserName = currentUser
                });

                foreach (var uid in l.userIds)
                {
                    userGroups.Add(new UserGroup()
                    {
                        GroupId = l.id,
                        UserId = uid,
                        CreatedDate = DateTime.Now,
                        CreatedUserID = currentUser,
                        CreatedUserName = currentUser,
                        IsDeleted = false,
                        UpdatedDate = DateTime.Now,
                        UpdatedUserID = currentUser,
                        UpdatedUserName = currentUser
                    });
                }
            }

            // 然后在本地插入或者同步数据
            int affectedRecordCount = _groupService.Repository.SqlExcute("truncate table [Group]");
            affectedRecordCount = await _groupService.Repository.InsertAsync(list);

            int affectedRecordCount1 = _userGroupService.Repository.SqlExcute("truncate table [UserGroup]");
            affectedRecordCount1 = await _userGroupService.Repository.InsertAsync(userGroups);

            return affectedRecordCount;
        }

        public async Task<int> SyncProjects(string userid, string token)
        {
            var currentUser = HttpContext.Current.User.Identity.Name;

            // 先到clockify获取数据
            var ls = await _clockifyService.GetProjects(userid, token);
            var list = new List<Project>();
            foreach (var l in ls)
            {
                list.Add(new Project()
                {
                    Gid = l.id,
                    Name = l.name,
                    Archived = l.archived,
                    Billable = l.billable,
                    ClientId = l.clientId,
                    Color = l.color,
                    Duration = l.duration,
                    IsPublic = l.Public,
                    Note = l.note,
                    WorkspaceId = l.workspaceId,
                    CreatedDate = DateTime.Now,
                    CreatedUserID = currentUser,
                    CreatedUserName = currentUser,
                    IsDeleted = false,
                    UpdatedDate = DateTime.Now,
                    UpdatedUserID = currentUser,
                    UpdatedUserName = currentUser
                });
            }

            // 然后在本地插入或者同步数据
            var affectedRecordCount = _projectService.Repository.SqlExcute("truncate table [Project]");

            affectedRecordCount = await _projectService.Repository.InsertAsync(list);

            return affectedRecordCount;
        }

        public async Task<int> SyncClients(string userid, string token)
        {
            var currentUser = HttpContext.Current.User.Identity.Name;

            // 先到clockify获取数据
            var ls = await _clockifyService.GetClients(userid, token);
            var list = new List<Client>();
            foreach (var l in ls)
            {
                list.Add(new Client()
                {
                    Gid = l.id,
                    Name = l.name,
                    Note = l.note,
                    CreatedDate = DateTime.Now,
                    CreatedUserID = currentUser,
                    CreatedUserName = currentUser,
                    IsDeleted = false,
                    UpdatedDate = DateTime.Now,
                    UpdatedUserID = currentUser,
                    UpdatedUserName = currentUser
                });
            }

            // 然后在本地插入或者同步数据
            var affectedRecordCount = _clientService.Repository.SqlExcute("truncate table [Client]");

            affectedRecordCount = await _clientService.Repository.InsertAsync(list);

            return affectedRecordCount;
        }

        public async Task<int> SyncTimeRecords(string userid, string token, DateTime startDate, DateTime endDate)
        {
            var currentUser = HttpContext.Current.User.Identity.Name;

            // 先到clockify获取数据
            var ls = await _clockifyService.GetTimeEntries(userid, token, startDate, endDate);
            var list = new List<TimeEntry>();

            var projectIdList = _projectService.Repository.Entities.Select(a => a.Gid).ToList();
            var taskIdList = _projectTaskService.Repository.Entities.Select(a => a.Gid).ToList();

            foreach (var l in ls)
            {
                try
                {
                    var clockifyTime = l.timeInterval.duration;
                    Regex regex = new Regex(@"PT((?<Hour>\d*)H)?((?<Minute>\d*)M)?((?<Second>\d*)S)?");
                    var match = regex.Match(clockifyTime);

                    int hour = 0;
                    int minute = 0;
                    int second = 0;
                    var val = match.Groups["Hour"].Value;
                    if (!string.IsNullOrEmpty(val))
                    {
                        hour = int.Parse(val);
                    }

                    val = match.Groups["Minute"].Value;
                    if (!string.IsNullOrEmpty(val))
                    {
                        minute = int.Parse(val);
                    }

                    val = match.Groups["Second"].Value;
                    if (!string.IsNullOrEmpty(val))
                    {
                        second = int.Parse(val);
                    }


                    list.Add(new TimeEntry()
                    {
                        Gid = l.id,
                        UserId = l.userId,
                        WorkspaceId = l.workspaceId,
                        Description = l.description,
                        //IsBillable = 
                        IsLocked = l.isLocked,
                        ProjectId = l.project.id,
                        TaskId = l.task == null ? "" : l.task.id,
                        TotalHours = (decimal)((hour * 3600 + minute * 60 + second) / 3600.0f),
                        Date = DateTime.ParseExact(l.timeInterval.start, "yyyy-MM-ddTHH:mm:ssZ", System.Globalization.CultureInfo.InvariantCulture).Date,
                        CreatedDate = DateTime.Now,
                        CreatedUserID = currentUser,
                        CreatedUserName = currentUser,
                        IsDeleted = false,
                        UpdatedDate = DateTime.Now,
                        UpdatedUserID = currentUser,
                        UpdatedUserName = currentUser
                    });

                    if (!projectIdList.Contains(l.project.id))
                    {
                        // 插入project信息
                        await _projectService.Repository.InsertAsync(new Project()
                        {
                            Gid = l.project.id,
                            Billable = l.project.billable,
                            ClientId = l.project.clientId,
                            Archived = l.project.archived,
                            Color = l.project.color,
                            Duration = l.project.duration,
                            IsPublic = l.project.Public,
                            Name = l.project.name,
                            Note = l.project.note,
                            WorkspaceId = l.project.workspaceId,
                            CreatedDate = DateTime.Now,
                            CreatedUserID = currentUser,
                            CreatedUserName = currentUser,
                            IsDeleted = false,
                            UpdatedDate = DateTime.Now,
                            UpdatedUserID = currentUser,
                            UpdatedUserName = currentUser
                        });

                        projectIdList.Add(l.project.id);
                    }

                    if (!taskIdList.Contains(l.task.id))
                    {
                        // 插入project信息
                        await _projectTaskService.Repository.InsertAsync(new ProjectTask()
                        {
                            Gid = l.task.id,
                            Duration = l.task.duration,
                            Name = l.task.name,
                            AssigneeId = l.task.assigneeId,
                            Estimate = l.task.estimate,
                            Status = l.task.status,
                            CreatedDate = DateTime.Now,
                            CreatedUserID = currentUser,
                            CreatedUserName = currentUser,
                            IsDeleted = false,
                            UpdatedDate = DateTime.Now,
                            UpdatedUserID = currentUser,
                            UpdatedUserName = currentUser,
                            ProjectGid = l.projectId
                        });

                        taskIdList.Add(l.task.id);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message);
                    _logger.Error(JsonConvert.SerializeObject(l));
                }
            }

            // 然后在本地插入或者同步数据
            var affectedRecordCount = _timeEntryService.Repository.SqlExcute($"delete from timeentry where [date]>='{startDate.ToString("yyyy-MM-dd")}' and [date]<'{endDate.AddDays(1).ToString("yyyy-MM-dd")}'");


            var ctx = (DbContext)(_timeEntryService.Repository.UnitOfWork);
            var options = new BulkInsertOptions
            {
                EnableStreaming = true,
            };
            ctx.BulkInsert(list, options);
            ctx.SaveChanges();

            affectedRecordCount = list.Count;

            return affectedRecordCount;
        }

    }
}
