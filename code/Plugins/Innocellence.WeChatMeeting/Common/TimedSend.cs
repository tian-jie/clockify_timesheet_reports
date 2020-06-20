using Infrastructure.Core.Logging;
using Infrastructure.Web.Tasks;
using Innocellence.WeChat.Domain.Common;
using Innocellence.WeChatMeeting.Domain.Entity;
using Innocellence.WeChatMeeting.Domain.Service;
using Innocellence.WeChatMeeting.Domain.ViewModel;
using Innocellence.Weixin.QY.AdvancedAPIs;
using Innocellence.Weixin.QY.AdvancedAPIs.Mass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using WebBackgrounder;

namespace Innocellence.WeChatMeeting.Common
{
    public class TimedSend
    {
        private static readonly ILogger log = LogManager.GetLogger("Meeting_TimedSend");

        #region  会议提醒（提醒分三次：提前三天，提前一天，提前半小时各提醒一次）
        /// <summary>
        ///会议提醒（提醒分三次：提前三天，提前一天，提前半小时各提醒一次）
        /// </summary>
        /// <returns></returns>
        public static void TimedSendMessage()
        {
            //当前时间
            DateTime current_time = DateTime.Now;
            IMeetingService _meetingService = new MeetingService();
            IMeetingInvitationService _meetingInvitationService = new MeetingInvitationService();

            var meetlist = _meetingService.Repository.Entities.Where(m => m.IsDeleted == false && m.StartDateTime > current_time).ToList();//未开始的会议

            if (meetlist.Count > 0)
            {
                foreach (var item in meetlist)
                {

                    var invitationlist = _meetingInvitationService.Repository.Entities.Where(i => i.MeetingId == item.Id && i.IsDeleted == false && i.State == 1).Take(100).ToList();//邀请的参会人员并已确认报名（state 为1报名成功）

                    if (invitationlist.Count > 0)
                    {
                        foreach (var invitation in invitationlist)
                        {

                            TimeSpan ts = Convert.ToDateTime(invitation.StartDateTime) - current_time;//时间差


                            if (ts.TotalMinutes <= 4320 && ts.TotalMinutes > 4319.7) //三天
                            {
                                SendTextMessage(item, invitationlist, "三天后");

                            }
                            if (ts.TotalMinutes <= 1440 && ts.TotalMinutes > 1439.7)//一天
                            {
                                SendTextMessage(item, invitationlist, "一天后");
                            }
                            if (ts.TotalMinutes <= 30 && ts.TotalMinutes > 29.7)//半个小时
                            {
                                SendTextMessage(item, invitationlist, "半小时后");
                            }
                        }
                    }

                }
            }

        }

        #endregion


        #region 发送文本消息
        public static bool SendTextMessage(Meeting meet, List<MeetingInvitation> invitationlist, string flag)
        {

            log.Debug("flag:{0}", flag);

            try
            {
                string meetMes = string.Empty;

                foreach (var invitation in invitationlist)
                {
                    if (flag == "半小时后")
                    {
                        meetMes = "您好，您受邀参加的\"" + meet.Title + "\"将于" + flag + "开始。";

                    }
                    else if (flag == "三天后" || flag == "一天后")
                    {
                        meetMes = "您好，您受邀参加的\"" + meet.Title + "\"将于" + flag + " - " + meet.StartDateTime.Value.ToString("yyyy年MM月dd日 HH:mm") + "开始。";

                    }

                    log.Debug("meetMes:{0}", meetMes);


                    var objConfig = WeChatCommonService.GetWeChatConfigByID(int.Parse(meet.EnterpriseAppId));
                    string strToken = (objConfig.IsCorp != null && !objConfig.IsCorp.Value) ? Innocellence.Weixin.MP.CommonAPIs.AccessTokenContainer.GetToken(objConfig.WeixinCorpId, objConfig.WeixinCorpSecret) : Innocellence.Weixin.QY.CommonAPIs.AccessTokenContainer.TryGetToken(objConfig.WeixinCorpId, objConfig.WeixinCorpSecret);
                    MassApi.SendText(strToken, invitation.UserId, "", "", objConfig.WeixinAppId.ToString(), meetMes, 0);

                }
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;

        }
        #endregion

    }
}