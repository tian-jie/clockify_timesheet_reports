using Infrastructure.Core.Data;
using Kevin.T.Timesheet.Interfaces;
using Kevin.T.Timesheet.Entities;
using System.Collections;
using Kevin.T.Timesheet.ModelsView;
using System.Collections.Generic;
using System.Linq;

namespace Kevin.T.Timesheet.Services
{
    public class EmployeeService : BaseService<Employee>, IEmployeeService
    {
        public EmployeeService()
            : base("Timesheet")
        {


        }

        public IList<EmployeeView> AllEmployeesWithRole()
        {
            var sql = @"select E.*, R.Id RoleId, R.Title RoleName, R.Rate RoleRate
from Employee E, RoleTitle R, EmployeeRate ER
where E.GID = ER.EmployeeGid and R.ID = ER.RoleId and E.isdeleted<>1 and ER.isdeleted<>1 and R.isdeleted<>1
";
            return Repository.UnitOfWork.SqlQuery<EmployeeView>(sql).ToList();
        }
    }
}
