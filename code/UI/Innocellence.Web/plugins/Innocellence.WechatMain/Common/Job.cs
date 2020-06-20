using Infrastructure.Core.Logging;
using Infrastructure.Web.Tasks;
using Innocellence.WeChat.Domain.Common;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.ModelsView;
using Innocellence.WeChat.Domain.Service;
using Innocellence.WeChat.Domain.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using WebBackgrounder;

//[assembly: WebActivatorEx.PostApplicationStartMethod(typeof(Innocellence.WeChatMain.Common.WebBackgrounderSetup), "Start")]
//[assembly: WebActivatorEx.ApplicationShutdownMethod(typeof(Innocellence.WeChatMain.Common.WebBackgrounderSetup), "Shutdown")]

namespace Innocellence.WeChatMain.Common
{
    public class GetCucumberProjectTreeJob : Job
    {
        ILogger log = LogManager.GetLogger("GetCucumberProjectTreeJob");

        public GetCucumberProjectTreeJob(TimeSpan interval, TimeSpan timeout)
            : base("GetCucumberProjectTree Job", interval, timeout)
        {
        }

        public override Task Execute()
        {
            return new Task(() =>
            {
                //Logger.Debug("Refreshing Project Tree...");
                // Utils.Projects = Utils.GetTreeFromDisk();
                try
                {
                    log.Warn("SendMsg Start");

                    // WechatCommon.SendMsg("text", "auto Send Msg：" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), null);

                    //自动发送
                    WechatCommon.AutoSendMsg();

                    log.Warn("SendMsg End");
                }
                catch (Exception ex)
                {
                    log.Error("SendMsg Error", ex);
                }
            });
        }
    }




    public static class WebBackgrounderSetup
    {
        static readonly JobManager _jobManager = CreateJobWorkersManager();

        public static void Start()
        {
            LogManager.GetLogger("WebBackgrounderSetup").Warn("job Start");
            if (_jobManager != null)
            {
                _jobManager.Start();
            }
        }

        public static void Shutdown()
        {
            LogManager.GetLogger("WebBackgrounderSetup").Warn("job Shutdown");
            // _jobManager.Dispose();
        }

        private static JobManager CreateJobWorkersManager()
        {
            var jobs = new IJob[]
        {
            new GetCucumberProjectTreeJob(TimeSpan.FromSeconds(5 * 60), TimeSpan.FromSeconds(20)),
        };

            var coordinator = new SingleServerJobCoordinator();
            var manager = new JobManager(jobs, coordinator);
            manager.Fail(ex => LogManager.GetLogger(typeof(WebBackgrounderSetup)).Error("Web Job Blow Up.", ex));
            return manager;
        }
    }

    /// <summary>
    /// 定时消息的自动事务
    /// </summary>
    public class TimerMessageJob : ITask
    {

        IArticleInfoService articleInfoService;
        ILogger log = LogManager.GetLogger("myJob");

        static List<int> lstSent = new List<int>();

        public TimerMessageJob(IArticleInfoService _articleInfoService)
        {
            articleInfoService = _articleInfoService;

        }
        // private IArticleInfoService _articleInfoService = EngineContext.Current.Resolve<IArticleInfoService>();

        public void Execute()
        {
            log.Debug("TimerMessageJob Execute Start");

            //查询7分钟之内的待发消息
            DateTime dtStart = DateTime.Now.AddMinutes(-2);
            DateTime dtEnd = DateTime.Now.AddSeconds(30);

            Expression<Func<ArticleInfo, bool>> predicate = (a) => a.IsDeleted == false && a.ArticleStatus != "Published" && a.ScheduleSendTime > dtStart && a.ScheduleSendTime < dtEnd;
            var list = articleInfoService.GetListWithContent<ArticleInfoView>(predicate, new Infrastructure.Utility.Data.PageCondition() { PageSize = 100, PageIndex = 1 });

            log.Debug("TimerMessageJob Execute:{0}", list.Count);
            System.Net.IPAddress myIPAddress = (System.Net.IPAddress)System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName()).Where(aa => aa.AddressFamily.ToString() == "InterNetwork").ToList().FirstOrDefault();

            //已经发送成功的信息不会重新发布
            list.RemoveAll(a => lstSent.Exists(b => b == a.Id));

            //保存到已经发送列表
            lstSent.AddRange(list.Select(aa => aa.Id).ToArray());

            //消息分组
            var listArticle = list.GroupBy(a => a.ArticleCode);
            foreach (var a in listArticle)
            {
                //消息排序
                var lstTemp = a.ToList().OrderBy(c => c.OrderID).ToList();
                var content = JsonConvert.DeserializeObject<List<NewsInfoView>>(lstTemp[0].ArticleContentEdit);
                lstTemp.ForEach(b => b.NewsInfo = content.Find(c => c.Id == b.OrderID));

                // System.Net.IPAddress myIPAddress = (System.Net.IPAddress)System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName()).Where(aa => aa.AddressFamily.ToString() == "InterNetwork");

                //消息发送
                using (var transactionScope = new TransactionScope(TransactionScopeOption.Suppress,
                    new TransactionOptions { IsolationLevel = IsolationLevel.RepeatableRead }))
                {

                    Weixin.QY.AdvancedAPIs.Mass.MassResult result = null;
                    try
                    {
                        result = WechatCommon.SendMsgToUser(lstTemp[0].AppId.Value, ((WechatMessageLogType)lstTemp[0].ContentType.Value).ToString(), "", lstTemp);

                        //如果发送失败，把已经发送列表中数据去除，这样可以重新发送。
                        if (result.errcode != Weixin.ReturnCode_QY.请求成功)
                        {
                            lstSent.RemoveAll(bb => lstTemp.Exists(aa => aa.Id == bb));
                        }

                        foreach (var b in lstTemp)
                        {
                            b.PublishDate = DateTime.Now;
                            b.ArticleStatus = "Published";

                            //记录IP
                            if (myIPAddress != null)
                            {
                                b.Previewers = myIPAddress.ToString();
                            }
                            articleInfoService.UpdateView(b, new List<string>() { "PublishDate", "ArticleStatus" });
                        }

                        if (result != null && result.errcode == Weixin.ReturnCode_QY.请求成功)
                        {
                            transactionScope.Complete();
                        }

                    }
                    catch (Exception ex)
                    {
                        log.Error(ex, "Auto SendMsg error");

                        //如果发送失败，把已经发送列表中数据去除，这样可以重新发送。
                        if (result == null || result.errcode != Weixin.ReturnCode_QY.请求成功)
                        {
                            lstSent.RemoveAll(bb => lstTemp.Exists(aa => aa.Id == bb));
                        }
                    }

                }

            }

            log.Debug("TimerMessageJob Execute");
        }
    }

    public class SyncUserJob : ITask
    {
        ILogger log = LogManager.GetLogger("myJob");
        IAddressBookService abService;
        public SyncUserJob(IAddressBookService _abService)
        {
            abService = _abService;
        }
        public void Execute()
        {
            var lst = WeChatCommonService.lstAccountManage.FindAll(a => a.AccountType == 0);
            foreach (var a in lst)
            {
                log.Debug("Start Sync User:{0}", a.Id);
                abService.SyncMember(a.Id);
                log.Debug("End Sync User:{0}", a.Id);
            }
            log.Debug("SyncUserJob Executed");
        }
    }
}