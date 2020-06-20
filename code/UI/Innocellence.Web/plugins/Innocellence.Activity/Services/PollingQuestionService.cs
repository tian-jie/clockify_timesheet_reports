using Infrastructure.Core.Data;
using Innocellence.Activity.Contracts.Entity;
using Innocellence.Activity.Contracts.ViewModel;
using Innocellence.Activity.Services;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Innocellence.Activity.Service
{
    public class PollingQuestionService : BaseService<PollingQuestionEntity>, IPollingQuestionService
    {

        public PollingQuestionService()
            : base("CAAdmin")
        {

        }

        public List<PollingQuestionView> GetPollingQuestion(int pollingId)
        {
            var pollingQuestion = Repository.Entities.AsNoTracking().Where(x => x.PollingId == pollingId && x.IsDeleted != true).ToList().Select(n => (PollingQuestionView)(new PollingQuestionView().ConvertAPIModel(n))).ToList();
            return pollingQuestion;
        }
        public List<PollingQuestionEntity> GetPollingQuestionEntity(int pollingId)
        {
            var pollingQuestion = Repository.Entities.AsNoTracking().Where(x => x.PollingId == pollingId && x.IsDeleted != true).ToList().ToList();
            return pollingQuestion;
        }
    }
}
