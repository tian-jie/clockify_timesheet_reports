using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Innocellence.WeChat.Domain.Contracts;

namespace Innocellence.WeChat.Domain.Service.job
{
    public sealed class ManuallyJobManager
    {
        private static readonly ConcurrentDictionary<JobName, ICustomerJob> _customerJobs = new ConcurrentDictionary<JobName, ICustomerJob>();
        private static readonly List<ICustomerJob> jobInstances = new List<ICustomerJob>();

        public ICustomerJob GetJob(JobName jobName)
        {
            return GetJobFromDictionary(jobName);
        }

        private static ICustomerJob GetJobFromDictionary(JobName jobName)
        {
            return _customerJobs.GetOrAdd(jobName, x =>
               {
                   if (!jobInstances.Any())
                   {
                       Assembly.GetAssembly(typeof(ICustomerJob))
                           .GetTypes()
                           .Where(type => !type.IsInterface && typeof(ICustomerJob).IsAssignableFrom(type)).ToList().ForEach(type =>
                           {
                               var jobInstance = (ICustomerJob)Activator.CreateInstance(type);
                               jobInstances.Add(jobInstance);
                           });
                   }
                   var instance = jobInstances.FirstOrDefault(job => job.JobName == jobName);

                   return instance;
               });
        }
    }
}
