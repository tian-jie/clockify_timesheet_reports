using Infrastructure.Core.Data;
using Innocellence.Activity.Model;
namespace Innocellence.Activity.Services
{
    public class BarrageExtService : BaseService<BarrageExt>, IBarrageExtService
    {
        public BarrageExtService()
            : base("CAAdmin")
        {

        }
       
    }
}