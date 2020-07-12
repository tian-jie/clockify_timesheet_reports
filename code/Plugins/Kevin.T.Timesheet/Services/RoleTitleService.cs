using Infrastructure.Core.Data;
using Kevin.T.Timesheet.Entities;
using Kevin.T.Timesheet.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Kevin.T.Timesheet.Services
{
    public class RoleTitleService : BaseService<RoleTitle>, IRoleTitleService
    {
        public RoleTitleService()
            : base("Timesheet")
        {

        }

        public IList<RoleTitle> AllInternal()
        {
            return Repository.Entities.Where(a => a.IsDeleted != true && a.Type == "internal").ToList();
        }

        public IList<RoleTitle> AllExternal()
        {
            return Repository.Entities.Where(a => a.IsDeleted != true && a.Type == "external").ToList();
        }
    }
}
