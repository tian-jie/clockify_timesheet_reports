using Infrastructure.Core;
using Kevin.T.Clockify.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kevin.T.Timesheet.Interfaces
{
    public interface ITimeService : IDependency
    {
        Task SyncUsers(string userid, string token);

        Task<LoginModel> SyncGroups(string userid, string token);

        Task<LoginModel> SyncTimeRecords(string userid, string token);
    }
}
