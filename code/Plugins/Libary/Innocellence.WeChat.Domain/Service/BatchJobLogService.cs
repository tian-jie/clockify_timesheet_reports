using System.Linq;
using Infrastructure.Core.Data;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Entity;

namespace Innocellence.WeChat.Domain.Services 
{
    public class BatchJobLogService : BaseService<BatchJobLog> , IBatchJobLogService
    {
        public BatchJobLog GetLogByJobId(string jobId)
        {
            return Repository.Entities.FirstOrDefault(x => x.JobID.Equals(jobId.Trim()));
        }
    }
}
