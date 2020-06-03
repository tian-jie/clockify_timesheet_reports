using Infrastructure.Core;
using Kevin.T.Timesheet.Entities;
using System.Linq;

namespace Kevin.T.Timesheet.Interfaces
{
    public interface IRoleTitleService : IDependency, IBaseService<RoleTitle>
    {
        IQueryable<RoleTitle> All();
    }
}
