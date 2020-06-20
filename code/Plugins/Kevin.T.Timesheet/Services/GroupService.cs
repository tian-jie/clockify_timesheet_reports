using Infrastructure.Core.Data;
using Kevin.T.Timesheet.Interfaces;
using Kevin.T.Timesheet.Entities;

namespace Kevin.T.Timesheet.Services
{
    public class GroupService : BaseService<Group>, IGroupService
    {
        public GroupService()
            : base("Timesheet")
        {

        }
    }
}
