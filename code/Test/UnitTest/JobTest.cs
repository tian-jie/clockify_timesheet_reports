using System;
using System.Data.Entity;
using System.Linq;
using Innocellence.WeChat.Domain.Service;
using Innocellence.WeChat.Domain.Service.job;
using LinqKit;
using NUnit.Framework;

namespace UnitTest
{
    public class JobTest : DbSetUp
    {
        //private static IList<JobName> jobLogs = new List<JobName> { JobName.AppAccessReportJob, JobName.ArticleReportJob, JobName.MenuReportJob };

        [Test]
        public void JobLogTest()
        {
            var jobLogService = new ReportJobLogService();

            var entity = jobLogService.CreateJobLog("MenuReportJob");

            Assert.IsTrue(entity.Id > 0);
        }

        [Test]
        public void MenuJob()
        {
            var job = new MenuReportJob();
            var task = job.Execute();
            task.Start();
            task.Wait();

        }

        [Test]
        public async void ManualStartJob()
        {
            //var service = new ManualStartJobService();
            //var list = service.ManualStartJob(new[] { JobName.MenuReportJob });

            var service = new ReportJobLogService().Repository.Entities;
           
            //var list =await (from entity in service where SqlFunctions.CharIndex("Menu",entity.JobName)>0 select entity).ToListAsync();
            var list = await service.Where(x => DbFunctions.DiffDays(x.CreatedDate, DateTime.Now) < 1).ToListAsync();

            ////Expression<Func<ReportJobLog, bool>> criteria1 = x => x.JobName =="";
            //var predicate = PredicateBuilder.False<ReportJobLog>();

            //foreach (var jobName in jobLogs)
            //{
            //    var name = jobName.ToString();
            //    predicate = predicate.Or(x => x.JobName == name);
            //}

            //predicate = predicate.And(x => x.JobStatus == "Success");

            //var list = service.Repository.Entities.AsExpandable().Where(predicate);

            Assert.IsTrue(list.Count > 0);
        }
    }
}
