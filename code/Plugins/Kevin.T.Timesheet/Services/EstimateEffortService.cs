using Infrastructure.Core.Data;
using Kevin.T.Timesheet.Interfaces;
using Kevin.T.Timesheet.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Kevin.T.Timesheet.Services
{
    public class EstimateEffortService : BaseService<EstimateEffort>, IEstimateEffortService
    {
        public EstimateEffortService()
            : base("Timesheet")
        {

        }

        public IQueryable<EstimateEffort> All()
        {
            return Repository.Entities.Where(a => a.IsDeleted != true);
        }

        public List<EstimateEffort> GetListByProject(string projectGid)
        {
            return All().Where(a => a.ProjectGid == projectGid).ToList();
        }
    }
}
