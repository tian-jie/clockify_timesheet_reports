using System;
using System.Globalization;
using System.Threading.Tasks;
using Infrastructure.Core.Contracts;
using Infrastructure.Utility.Data;
using Infrastructure.Utility.Extensions;
using Innocellence.WeChat.Domain.Services;
using WebBackgrounder;
using System.Collections.Generic;
using System.Linq;
using Binbin.Linq;
using Infrastructure.Core.Data;
using Infrastructure.Core.Logging;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.Entity;
using Infrastructure.Web.Domain.Service;


namespace Innocellence.WeChat.Domain.Service.job
{
    public class ArticleReportJob : ICustomerJob
    {
        private readonly TimeSpan _interval;
        private readonly IReportJobLogService _jobLogService = new ReportJobLogService();
        private readonly IUserBehaviorService _userBehaviorService = new UserBehaviorService(new CodeFirstDbContext());
        private readonly IArticleReportService _articleReportService = new ArticleReportService();
        private readonly IArticleInfoService _articleInfoService = new ArticleInfoService();
        private readonly IQuestionManageService _questionManageService = new QuestionManageService(new CodeFirstDbContext());
      //  private readonly IMessageService _messageService = new MessageService();

        private static readonly ILogger log = LogManager.GetLogger(typeof(ArticleReportJob));
        private readonly string jobName = JobName.ArticleReportJob.ToString();//"ArticleReportJob";
        private bool _success = true;

        public ArticleReportJob()
        {
            _interval = TimeSpan.FromHours(24);
        }

        public Task Execute()
        {
            return new Task(RunJob);
        }

        private void RunJob()
        {
            ReportJobLog jobLog = null;

            try
            {
                jobLog = _jobLogService.CreateJobLog(jobName);
                ImportDataByTime(jobLog.DateFrom, jobLog.DateTo);
                jobLog.JobStatus = JobStatus.Success.ToString();
            }
            catch (Exception e)
            {
                if (jobLog != null) jobLog.ErrorMessage = e.InnerException == null ? e.Message : e.InnerException.Message;

                log.Error<string>(string.Format("article report job error:{0}" + Environment.NewLine + "{1}", e.Message,
                    e.StackTrace));
            }
            finally
            {
                if (jobLog != null)
                {
                    jobLog.UpdatedDate = DateTime.Now;
                    _jobLogService.Repository.Update(jobLog);
                }
            }

        }

        /// <summary>
        /// contentType 是  1,2,3   Description: 1. URL中带News 2. URL中带Message 3. URL中带 QuestionDetail [removed]  
        /// LED ContentType 51.News 52.Courses 53.MicroCourses
        /// </summary>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        private void ImportDataByTime(DateTime fromDateTime, DateTime toDateTime)
        {
            var articleReportLogs = new List<ArticleReport>();
            var predicate = PredicateBuilder.True<UserBehavior>();

            predicate = PredicateBuilder.And(predicate, x => x.CreatedTime <= toDateTime && x.CreatedTime > fromDateTime);

            predicate = PredicateBuilder.And(predicate, x => x.ContentType == 1 || x.ContentType == 2 || x.ContentType == 51 || x.ContentType == 52 || x.ContentType == 53);//|| x.ContentType == 3 remove by andrew.

            predicate = PredicateBuilder.And(predicate, x => x.Content != null || x.Content != "");

            //第一层过滤 按ContentType
            var logs = _userBehaviorService.Repository.Entities.Where(predicate).ToList().Where(x => !string.IsNullOrEmpty(x.Content)).ToList();

            //去多表取数据 
            logs.GroupBy(x => x.ContentType).ToList().ForEach(x =>
            {
                //根据ContentType去相应的表去拿最终集合
                switch (x.Key)
                {
                    case 1:
                        articleReportLogs.AddRange(GetNewsListByIds(x.ToList()));
                        break;
                    case 2:
                        articleReportLogs.AddRange(GetMessageListByIds(x.ToList()));
                        break;
                    //case 3:
                    //    articleReportLogs.AddRange(GetQuestionListByIds(x.ToList()));
                    //    break;
                    case 51:
                    case 52:
                    case 53:
                        articleReportLogs.AddRange(GetArticleListByIds(x.ToList()));
                        break;
                }
            });

            //先按AppId分组--再按Title分--再按创建时间分--就能得到某个APP下某标题下某天的数据访问量
            //lstExtentions.GroupBy(x => x.AppId).ToList().
            //ForEach(x => x.GroupBy(y => y.Title).ToList().
            //ForEach(z => z.GroupBy(w => w.CreatedTime).ToList().
            //ForEach(group => articleReportLogs.Add(new ArticleReport
            //{
            //    ArticleId = int.Parse(z.First().Content),
            //    ArticleTitle = z.First().Title,
            //    AppId = z.First().AppId,
            //    VisitTimes = z.Count(),
            //    AccessDate = z.First().CreatedTime,
            //    VisitorCount = z.GroupBy(y => y.UserId).Count()
            //}))));

            _articleReportService.Repository.Insert(articleReportLogs.AsEnumerable());
        }


        //根据Ids去组织相应的集合
        private IList<ArticleReport> GetNewsListByIds(IList<UserBehavior> userBehaviors)
        {

            var articleIds = getTargetIDs(userBehaviors).ToList();

            var list = _articleInfoService.Repository.Entities.Where(x => articleIds.Contains(x.Id)).Select(x => new { x.ArticleTitle, x.AppId, x.Id, x.CategoryId }).ToList();



            //反查拿出behavior集合
            var logs = userBehaviors.Where(x => list.Any(y => y.Id.ToString(CultureInfo.CurrentCulture) == x.Content)).ToList();

            var outs = logs.Select(x =>
            {
                var news = list.First(y => y.Id.ToString(CultureInfo.CurrentCulture) == x.Content);

                var category = CommonService.lstCategory.FirstOrDefault(z => news.CategoryId != null && z.Id == news.CategoryId);

                return new ReportExtention
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    NewsId = int.Parse(x.Content),
                    CreatedTime = x.CreatedTime.Date,
                    Title = news.ArticleTitle,
                    Content = x.Content,
                    AppId = news.AppId.GetValueOrDefault(),
                    MenuName = category == null ? string.Empty : category.CategoryName,
                    //MenuKey = news.ArticleCateSub
                    MenuKey = category == null ? string.Empty: category.CategoryCode,
                };
            }).ToList();

            return GroupList(outs).ToList();

        }

        private IList<ArticleReport> GetMessageListByIds(IList<UserBehavior> userBehaviors)
        {

            //var messageIds = getTargetIDs(userBehaviors);

            //var list = _messageService.Repository.Entities.Where(x => messageIds.Contains(x.Id)).Select(x => new { x.Title, x.AppId, x.Id }).ToList();

            //var logs = userBehaviors.Where(x => list.Any(y => y.Id.ToString(CultureInfo.CurrentCulture) == x.Content)).ToList();

            //var outs = logs.Select(x =>
            //{
            //    var news = list.First(y => y.Id.ToString(CultureInfo.CurrentCulture) == x.Content);
            //    return new ReportExtention
            //    {
            //        Id = x.Id,
            //        UserId = x.UserId,
            //        NewsId = int.Parse(x.Content),
            //        CreatedTime = x.CreatedTime.Date,
            //        Title = news.Title,
            //        Content = x.Content,
            //        AppId = news.AppId.GetValueOrDefault()
            //    };
            //}).ToList();

            //return GroupList(outs).ToList();

            return null;

        }

        private IList<ArticleReport> GetQuestionListByIds(IList<UserBehavior> userBehaviors)
        {

            var questionIds = getTargetIDs(userBehaviors);

            var list = _questionManageService.Repository.Entities.Where(x => questionIds.Contains(x.Id)).Select(x => new { x.Question, x.AppId, x.Id }).ToList();

            var logs = userBehaviors.Where(x => list.Any(y => y.Id.ToString(CultureInfo.CurrentCulture) == x.Content)).ToList();

            var outs = logs.Select(x =>
            {
                var news = list.First(y => y.Id.ToString(CultureInfo.CurrentCulture) == x.Content);
                return new ReportExtention
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    NewsId = int.Parse(x.Content),
                    CreatedTime = x.CreatedTime.Date,
                    Title = news.Question,
                    Content = x.Content,
                    AppId = news.AppId.GetValueOrDefault()
                };
            }).ToList();

            return GroupList(outs).ToList();

        }

        //LED的内容类型接入ArticleReport
        private IList<ArticleReport> GetArticleListByIds(IList<UserBehavior> userBehaviors)
        {
            var objContent=new LEDContent();
            var outs = userBehaviors.Select(x =>
            {
                //反序列化LEDContent对象，拿出需要的字段
                if (!string.IsNullOrEmpty(x.Content))
                {
                    objContent = JsonHelper.FromJson<LEDContent>(x.Content);
                }

                var category = CommonService.lstCategory.FirstOrDefault(z => z.CategoryCode == objContent.MeunKey);

                return new ReportExtention
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    NewsId = objContent.ArticleId,
                    CreatedTime = x.CreatedTime.Date,
                    Title = objContent.ArticleTitle,
                    Content = x.Content,
                    AppId = (int)CategoryType.LEDCate,
                    MenuName = category == null ? string.Empty : category.CategoryName,
                    MenuKey = objContent.MeunKey
                };
            }).ToList();

            return GroupList(outs).ToList();
        }


        /// <summary>
        /// 组织我们最终要的集合--某个APP下某篇文章下某天的访问人数，访问次数
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private IEnumerable<ArticleReport> GroupList(IEnumerable<ReportExtention> source)
        {
            var list = new List<ArticleReport>();
            source.GroupBy(x => x.AppId).ToList().
            ForEach(x => x.GroupBy(y => y.NewsId).ToList().
            ForEach(z => z.GroupBy(w => w.CreatedTime).ToList().
            ForEach(group => list.Add(new ArticleReport
            {
                ArticleId = z.First().NewsId,//modify by andrew 3/18
                ArticleTitle = z.First().Title,
                AppId = z.First().AppId,
                MenuKey = z.First().MenuKey,
                MenuName = z.First().MenuName,
                AppName = GetAppNameFromCach(z.First().AppId),
                VisitTimes = group.Count(),
                AccessDate = group.Key,
                VisitorCount = group.GroupBy(y => y.UserId).Count()
            }))));

            return list;
        }

        private string GetAppNameFromCach(int appId)
        {
          //  string app = null;// CommonService.lstSysWeChatConfig.FirstOrDefault(x => x.WeixinAppId == appId.ToString(CultureInfo.CurrentCulture));
            var appName = string.Empty;
            //if (app == null)
            //{
            //    log.Info("没有找到app id 为{0}的app!", appId);
            //}
            //else
            //{
            //    appName = app.AppName;
            //}

            return appName;
        }

        private static IEnumerable<int> getTargetIDs(IEnumerable<UserBehavior> behaviors)
        {
            return behaviors.Select(x =>
             {
                 int id;
                 if (int.TryParse(x.Content, out id))
                 {
                     return id;
                 }

                 log.Debug("article job data exception: the content column type of behavior is not int ,the value is {0}. The primary key value is {1}", x.Content, x.Id);
                 return 0;
             }).Distinct().ToList();

        }

        public TimeSpan Interval
        {
            get { return _interval; }
        }

        public string Name
        {
            get { return "Article Report Job"; }
        }

        public TimeSpan Timeout
        {
            get { return TimeSpan.MaxValue; }
        }

        public void ManuallyRunJob()
        {
            RunJob();
        }

        public JobName JobName
        {
            get { return JobName.ArticleReportJob; }
        }

        public bool Success
        {
            get { return _success; }
        }
    }
}
