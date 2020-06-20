using EntityFramework.Extensions;
using Infrastructure.Core.Data;
using Innocellence.Activity.Contracts.Entity;
using Innocellence.Activity.Contracts.ViewModel;
using Innocellence.Activity.Services;
using System.Collections.Generic;
using System.Linq;

namespace Innocellence.Activity.Service
{
    public class PollingResultTempService : BaseService<PollingResultTempEntity>, IPollingResultTempService
    {
        public PollingResultTempService()
            : base("CAAdmin")
        {
        }

        public new int InsertView(PollingResultTempView objModalSrc)
        {
            return AddOrUpdate(objModalSrc, true);

        }

        public new int UpdateView(PollingResultTempView objModalSrc)
        {

            return AddOrUpdate(objModalSrc, false);

        }
        public new int Delete(int id,string Lillyid)
        {
            return Repository.Entities.Where(x => x.PollingId == id && x.UserId == Lillyid).Delete();
        }
        private int AddOrUpdate(PollingResultTempView objModalSrc, bool bolAdd)
        {
            PollingResultTempView objView = objModalSrc;
            if (objView == null)
            {
                return -1;
            }
            int iRet;
            var pollings = new List<PollingResultTempEntity>();
            PollingResultTempEntity polling = new PollingResultTempEntity();
            objView.AnswerResults.ToList().ForEach(x =>
            {
                polling = new PollingResultTempEntity
                {
                    PollingId = objModalSrc.PollingId,
                    QuestionId = x.QuestionId,
                    QuestionName = x.QuestionName,
                    Answer = x.Answer,
                    AnswerText = x.AnswerText,
                    UserId = objModalSrc.UserId
                };
                pollings.Add(polling);

                if (!bolAdd)
                {
                    // 应为没有批量更新
                    iRet = Repository.Update(polling);
                }

            });

            if (bolAdd)
            {
                iRet = Repository.Insert(pollings.AsEnumerable());
            }

            return 1;

        }
        public int GetTempCountByLillyID(int id, string lillyid)
        {
            return Repository.Entities.Where(x => x.PollingId == id && x.UserId == lillyid && x.IsDeleted == false).Select(x => x.UserId).Count();
        }
       
    }

}

