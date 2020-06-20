using System.Collections.Generic;
using System.ComponentModel;
using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;

namespace Innocellence.WeChat.Domain.Contracts
{
    public interface IManualStartJobService : IDependency
    {
        /// <summary>
        /// 手动启动选择的job，运行成功后返回，返回最近成功的信息
        /// </summary>
        /// <param name="runJobs"></param>
        /// <returns></returns>
        IList<ReportJobLog> ManualStartJob(IList<JobName> runJobs);
    }


    public enum JobName
    {
        [Description("菜单报表job")]
        MenuReportJob,

        [Description("App访问量报表job")]
        AppAccessReportJob,

        [Description("网页访问量报表job")]
        ArticleReportJob,

        [Description("其他网页访问量报表job")]
        PageReportJob,
    }
}
