using Infrastructure.Web.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Innocellence.WeChatMeeting.Common
{
    public class TimedSendJob : ITask
    {
        public void Execute()
        {
            TimedSend.TimedSendMessage();//会议提醒
        }
    }
}