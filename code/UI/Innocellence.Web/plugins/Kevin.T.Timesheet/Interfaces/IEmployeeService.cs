using Infrastructure.Core;
using Kevin.T.Timesheet.Entities;
using Kevin.T.Timesheet.ModelsView;
using System.Collections.Generic;

namespace Kevin.T.Timesheet.Interfaces
{
    public interface IEmployeeService : IDependency, IBaseService<Employee>
    {
        IList<EmployeeView> AllEmployeesWithRole();
    }
}
