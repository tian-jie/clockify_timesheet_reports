using Infrastructure.Core;
using Kevin.T.Timesheet.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Kevin.T.Timesheet.Interfaces
{
    public interface IEstimateEffortService : IDependency, IBaseService<EstimateEffort>
    {
        List<EstimateEffort> GetListByProject(string projectGid);

        IQueryable<EstimateEffort> All();
    }
}
