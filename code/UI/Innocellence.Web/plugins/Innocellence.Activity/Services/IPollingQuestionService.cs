using Infrastructure.Core;
using Innocellence.Activity.Contracts.Entity;
using Innocellence.Activity.Contracts.ViewModel;
using System.Collections.Generic;

namespace Innocellence.Activity.Services
{
    public interface IPollingQuestionService : IDependency, IBaseService<PollingQuestionEntity>
    {
        List<PollingQuestionView> GetPollingQuestion(int pollingId);
        List<PollingQuestionEntity> GetPollingQuestionEntity(int pollingId);
    }

}
