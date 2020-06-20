using Infrastructure.Core;
using Kevin.T.Timesheet.Entities;

namespace Kevin.T.Timesheet.Interfaces
{
    public interface IEmployeeService : IDependency, IBaseService<Employee>
    {
    }
}
