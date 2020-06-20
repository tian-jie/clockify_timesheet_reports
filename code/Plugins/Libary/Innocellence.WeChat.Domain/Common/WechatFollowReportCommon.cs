using Autofac;
using Infrastructure.Core.Caching;
using Infrastructure.Core.Data;
using Infrastructure.Core.Infrastructure;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.Common;
using Innocellence.Weixin.QY.AdvancedAPIs;
using Innocellence.Weixin.QY.AdvancedAPIs.MailList;
using Innocellence.Weixin.QY.CommonAPIs;
using System;
using System.Linq;

namespace Innocellence.WeChat.Domain.Common
{
    public class WechatFollowReportCommon
    {
        
        /// <summary>
        /// 微信关注数据追加
        /// </summary>
        private static void InsertWechatFollowReport(int AccountManageID)
        {
            var objConfig = WeChatCommonService.GetWeChatConfigByID(AccountManageID);
            
            string strToken = AccessTokenContainer.TryGetToken(objConfig.WeixinCorpId, objConfig.WeixinCorpSecret);

            var departmentList =  MailListApi.GetDepartmentMemberInfo(strToken, 1, 1, 0);

            //关注用户列表
            var followerList = departmentList.userlist.Where(a => a.status == 1).ToList();

            //为关注用户列表
            var unFollowerList = departmentList.userlist.Where(a => a.status == 4).ToList();

            DateTime dtNow = DateTime.Now;

            WechatFollowReport wechatFollowReport = new WechatFollowReport() 
            {
                StatisticsDate = Convert.ToDateTime(dtNow.AddDays(-1).ToString("yyyy-MM-dd")),
                FollowCount = followerList.Count,
                UnFollowCount = unFollowerList.Count,
                CreatedDate = dtNow
            };

            BaseService<WechatFollowReport> ser = new BaseService<WechatFollowReport>();
            ser.Repository.Insert(wechatFollowReport);
        }

        /// <summary>
        /// 执行微信关注数据相关操作
        /// </summary>
        public static WechatFollowReportType WechatFollowReportWork(int AccountManageID)
        {
            WechatFollowReportType result = WechatFollowReportType.Error;

            //取得最新微信关注数据
            BaseService<WechatFollowReport> ser = new BaseService<WechatFollowReport>();
            WechatFollowReport wechatFollowReport = ser.Repository.Entities.OrderByDescending(
                a => a.StatisticsDate).FirstOrDefault();

            DateTime dtCheckDate = Convert.ToDateTime(DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"));

            //如果当天未进行过统计，向微信关注表追加数据
            if (wechatFollowReport == null || dtCheckDate > wechatFollowReport.StatisticsDate)
            {
                InsertWechatFollowReport(AccountManageID);

                result = WechatFollowReportType.Success;
            }
            else
            {
                result = WechatFollowReportType.AlreadyCreated;
            }

            return result;
        }
    }

    public enum WechatFollowReportType
    {
        Success = 0,
        AlreadyCreated = 1,
        Error = 9 
    }

}