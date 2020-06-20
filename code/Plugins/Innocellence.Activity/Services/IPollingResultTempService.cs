using Infrastructure.Core;
using Innocellence.Activity.Contracts.Entity;
using Innocellence.Activity.Contracts.ViewModel;

namespace Innocellence.Activity.Services
{
    public interface IPollingResultTempService : IDependency, IBaseService<PollingResultTempEntity>
    {
        int InsertView(PollingResultTempView objModalSrc);
        int UpdateView(PollingResultTempView objModalSrc);
        int GetTempCountByLillyID(int id, string lillyid);
        int Delete(int id,string Lillyid);
    }
}
