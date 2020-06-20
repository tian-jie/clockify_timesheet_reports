using Infrastructure.Core.Data;
using Kevin.T.Timesheet.Interfaces;
using Kevin.T.Timesheet.Entities;

namespace Kevin.T.Timesheet.Services
{
    public class EmployeeService : BaseService<Employee>, IEmployeeService
    {
        public EmployeeService()
            : base("Timesheet")
        {

        }

    }
}
