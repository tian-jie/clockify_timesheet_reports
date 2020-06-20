using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;

namespace Innocellence.WeChat.Domain.Contracts
{
    public interface IBatchJobLogService : IDependency, IBaseService<BatchJobLog>
    {
        BatchJobLog GetLogByJobId(string jobId);
    }
}
