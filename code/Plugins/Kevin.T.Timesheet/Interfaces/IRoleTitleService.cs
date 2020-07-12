using Infrastructure.Core;
using Kevin.T.Timesheet.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Kevin.T.Timesheet.Interfaces
{
    public interface IRoleTitleService : IDependency, IBaseService<RoleTitle>
    {
        IList<RoleTitle> AllInternal();

        IList<RoleTitle> AllExternal();
    }
}
