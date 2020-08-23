using Infrastructure.Core;
using Kevin.T.Clockify.Data.Models;
using Kevin.T.Timesheet.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kevin.T.Timesheet.Interfaces
{
    public interface IClockifyService : IDependency
    {
        Task<LoginModel> Login(string username, string password);
        Task<List<UserGroupModel>> GetUserGroups(string userid, string token);
        Task<List<UserModel>> GetUsers(string userid, string token);
        Task<List<TimeEntryModel>> GetTimeEntries(string userid, string token, DateTime startDate, DateTime endDate);
        Task<List<TimeEntryModelV2>> GetTimeEntriesV2(string userid, string token, DateTime startDate, DateTime endDate);
        Task<List<TimeEntry>> GetTimeEntriesV3(string userid, string token, DateTime startDate, DateTime endDate);
        Task<List<ClientModel>> GetClients(string userid, string token);
        Task<List<ProjectModel>> GetProjects(string userid, string token);

    }
}
