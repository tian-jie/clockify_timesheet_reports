using Infrastructure.Core.Logging;
using Infrastructure.Web.Domain.Service;
using Infrastructure.Web.Tasks;
//using Innocellence.WeChat.Domain.Common;
using System;
using System.Threading.Tasks;
using WebBackgrounder;

namespace Innocellence.WeChatMain.Services
{
    public class WechatFollowReportJob : ITask
    {
        private static string jobExecuteTime = CommonService.GetSysConfig("JobExecuteTime", "08:00:00");
        public void Execute()
        {
            try
            {
                
                string dateTimeFormat = "HH:mm:ss";

                DateTime dtNow =Convert.ToDateTime(DateTime.Now.ToString(dateTimeFormat));

                DateTime dtStandard = Convert.ToDateTime(jobExecuteTime);

                if (dtNow < dtStandard)
                {
                    return;
                }
                
                //WechatFollowReportType result = WechatFollowReportCommon.WechatFollowReportWork();
                
            }
            catch (Exception ex)
            {
                LogManager.GetLogger(this.GetType()).Error("WechatFollowReport Error", ex);
            }
            
        }
        
    }
}
