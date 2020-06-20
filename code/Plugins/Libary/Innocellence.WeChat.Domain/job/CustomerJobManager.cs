using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebBackgrounder;

namespace Innocellence.WeChat.Domain.Service.job
{
    public class CustomerJobManager
    {
        private readonly IJobCoordinator _coordinator;
        private Action<Exception> _failHandler;
        private readonly IJobHost _host;
        private readonly IEnumerable<IJob> _jobs;
        private readonly Scheduler _scheduler;
        private readonly Timer _timer;
        private readonly TimeSpan _starTimeSpan;

        public CustomerJobManager(IList<IJob> jobs, IJobCoordinator coordinator, TimeSpan startTimeSpan)
            : this(jobs, new JobHost(), coordinator, startTimeSpan)
        {

        }

        public CustomerJobManager(IList<IJob> jobs, IJobHost host, TimeSpan startTimeSpan)
            : this(jobs, host, new SingleServerJobCoordinator(), startTimeSpan)
        {

        }

        public CustomerJobManager(IList<IJob> jobs, IJobHost host, IJobCoordinator coordinator, TimeSpan startTimeSpan)
        {
            if (jobs == null)
            {
                throw new ArgumentNullException("jobs");
            }
            if (host == null)
            {
                throw new ArgumentNullException("host");
            }
            if (coordinator == null)
            {
                throw new ArgumentNullException("coordinator");
            }
            _jobs = jobs;
            _scheduler = new Scheduler(jobs);
            _host = host;
            _coordinator = coordinator;
            _timer = new Timer(OnTimerElapsed);

            _starTimeSpan = startTimeSpan;
        }

        public void Dispose()
        {
            Stop();
            foreach (IJob job in _jobs)
            {
                // ReSharper disable once SuspiciousTypeConversion.Global
                var disposable = job as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
            _timer.Dispose();
            _coordinator.Dispose();
        }

        private void DoNextJob()
        {
            using (Schedule schedule = _scheduler.Next())
            {
                Task work = _coordinator.GetWork(schedule.Job);
                if (work != null)
                {
                    _host.DoWork(work);
                }
            }
        }

        public void Fail(Action<Exception> failHandler)
        {
            _failHandler = failHandler;
        }

        private void OnException(Exception e)
        {
            Action<Exception> action = _failHandler;
            if (action != null)
            {
                action(e);
            }
        }

        private void OnTimerElapsed(object sender)
        {
            try
            {
                _timer.Stop();
                DoNextJob();
                _timer.Next(_scheduler.Next().GetIntervalToNextRun());
            }
            catch (Exception exception)
            {
                OnException(exception);
                if (RestartSchedulerOnFailure)
                {
                    _timer.Next(_scheduler.Next().GetIntervalToNextRun());
                }
            }
        }

        public void Start()
        {
            _timer.Next(_starTimeSpan);
        }

        public void Stop()
        {
            _timer.Stop();
        }

        public bool RestartSchedulerOnFailure { get; set; }
    }
}
