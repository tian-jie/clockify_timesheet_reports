using Infrastructure.Core.Data;
using Innocellence.Activity.Entity;
using Innocellence.Activity.Services;

namespace Innocellence.CA.Service
{
    public partial class BarrageSummaryService : BaseService<BarrageSummary>, IBarrageSummaryService
    {
        public BarrageSummaryService()
            : base("CAAdmin")
        {
             

        }
       
      
    }
}