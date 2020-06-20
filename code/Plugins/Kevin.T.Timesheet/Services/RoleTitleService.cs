using Infrastructure.Core.Data;
using Kevin.T.Timesheet.Entities;
using Kevin.T.Timesheet.Interfaces;
using System.Linq;

namespace Kevin.T.Timesheet.Services
{
    public class RoleTitleService : BaseService<RoleTitle>, IRoleTitleService
    {
        public RoleTitleService()
            : base("Timesheet")
        {

        }

        public IQueryable<RoleTitle> All()
        {
            return Repository.Entities.Where(a => a.IsDeleted != true);
        }
    }
}
