using Infrastructure.Core.Data;
using Kevin.T.Timesheet.Interfaces;
using Kevin.T.Timesheet.Entities;

namespace Kevin.T.Timesheet.Services
{
    public class RoleTitleService : BaseService<RoleTitle>, IRoleTitleService
    {
        public RoleTitleService()
            : base("Timesheet")
        {

        }
    }
}
