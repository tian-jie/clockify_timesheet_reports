using Infrastructure.Core;
using Kevin.T.Clockify.Data.Models;
using System;
using System.Threading.Tasks;

namespace Kevin.T.Timesheet.Interfaces
{
    public interface ITimesheetService : IDependency
    {
        Task<int> SyncUsers(string userid, string token);

        Task<int> SyncGroups(string userid, string token);

        Task<int> SyncTimeRecords(string userid, string token, DateTime startTime, DateTime endTime);

        Task<int> SyncProjects(string userid, string token);

        Task<int> SyncClients(string userid, string token);


    }
}
