using Infrastructure.Core;
using Innocellence.Activity.Contracts.Entity;
using Innocellence.Activity.Contracts.ViewModel;
using System.Collections.Generic;

namespace Innocellence.Activity.Services
{
    public interface IPollingOptionService : IDependency, IBaseService<PollingOptionEntity>
    {
        List<PollingOptionView> GetPollingOptions(int questionId);
    }

}
