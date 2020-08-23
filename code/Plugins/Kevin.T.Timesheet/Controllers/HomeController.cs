using Infrastructure.Core.Logging;
using Innocellence.WeChat.Domain;
using Kevin.T.Timesheet.Entities;
using Kevin.T.Timesheet.Interfaces;
using Kevin.T.Timesheet.ModelsView;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Kevin.T.Timesheet.Controllers
{
    public class HomeController : BaseController<TimeEntry, TimeEntryView>
    {
        private ILogger Logger;
        //ITimeEntryService _timeEntryService;
        ITimesheetService _timesheetService;
        IClockifyService _clockifyService;

        public HomeController(ITimeEntryService timeEntryService, ITimesheetService timesheetService, IClockifyService clockifyService)
            : base(timeEntryService)
        {
            _timesheetService = timesheetService;
            _clockifyService = clockifyService;
            Logger = LogManager.GetLogger("Banner Management");
        }

        public override ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> SyncUsers()
        {
            var login = await _clockifyService.Login("jie.tian@innocellence.com", "Welcome1!");
            var workspaceId = login.membership.FirstOrDefault(a => a.membershipType == "WORKSPACE").targetId;

            var cnt = await _timesheetService.SyncUsers(workspaceId, login.token);
            return SuccessNotification($"Success Synced: {cnt}");
        }

        public async Task<ActionResult> SyncProjects()
        {
            var login = await _clockifyService.Login("jie.tian@innocellence.com", "Welcome1!");
            var workspaceId = login.membership.FirstOrDefault(a => a.membershipType == "WORKSPACE").targetId;
            var cnt = await _timesheetService.SyncProjects(workspaceId, login.token);
            return SuccessNotification($"Success Synced: {cnt}");
        }

        public async Task<ActionResult> SyncGroups()
        {
            var login = await _clockifyService.Login("jie.tian@innocellence.com", "Welcome1!");
            var workspaceId = login.membership.FirstOrDefault(a => a.membershipType == "WORKSPACE").targetId;
            var cnt = await _timesheetService.SyncGroups(workspaceId, login.token);
            return SuccessNotification($"Success Synced: {cnt}");
        }

        public async Task<ActionResult> SyncClients()
        {
            var login = await _clockifyService.Login("jie.tian@innocellence.com", "Welcome1!");
            var workspaceId = login.membership.FirstOrDefault(a => a.membershipType == "WORKSPACE").targetId;
            var cnt = await _timesheetService.SyncClients(workspaceId, login.token);
            return SuccessNotification($"Success Synced: {cnt}");
        }

        public async Task<ActionResult> SyncTimeEntry(string startTime, string endTime)
        {
            var login = await _clockifyService.Login("jie.tian@innocellence.com", "Welcome1!");
            var workspaceId = login.membership.FirstOrDefault(a => a.membershipType == "WORKSPACE").targetId;

            var sdt = DateTime.ParseExact(startTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var edt = DateTime.ParseExact(endTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

            var cnt = await _timesheetService.SyncTimeRecordsV3(workspaceId, login.token, sdt, edt);
            return SuccessNotification($"Success Synced: {cnt}");
        }

    }
}