using Infrastructure.Core.Data;
using Innocellence.Activity.Contracts.Entity;
using Innocellence.Activity.Contracts.ViewModel;
using Innocellence.Activity.Services;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Innocellence.Activity.Service
{
    public class PollingOptionService : BaseService<PollingOptionEntity>, IPollingOptionService
    {
        public PollingOptionService()
            : base("CAAdmin")
        {
        }
        public List<PollingOptionView> GetPollingOptions(int questionId)
        {
            var pollingQuestion = Repository.Entities.AsNoTracking().Where(x => x.QuestionId == questionId && x.IsDeleted != true).ToList().Select(n => (PollingOptionView)(new PollingOptionView().ConvertAPIModel(n))).ToList();
            return pollingQuestion;
        }
    }
}
