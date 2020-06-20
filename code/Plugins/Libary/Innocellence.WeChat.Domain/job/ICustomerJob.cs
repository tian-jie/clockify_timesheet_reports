using Innocellence.WeChat.Domain.Contracts;
using WebBackgrounder;

namespace Innocellence.WeChat.Domain.Service.job
{
    public interface ICustomerJob : IJob
    {
        void ManuallyRunJob();

        JobName JobName { get;  }

        bool Success { get; }
    }
}
