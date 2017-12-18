using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Xmu.Crms.Shared.Scheduling
{
    //Stolen from https://blog.maartenballiauw.be/post/2017/08/01/building-a-scheduled-cache-updater-in-aspnet-core-2.html
    public class SchedulerHostedService : HostedService
    {
        private readonly List<SchedulerTaskWrapper> _scheduledTasks = new List<SchedulerTaskWrapper>();

        public SchedulerHostedService(IEnumerable<IScheduledTask> scheduledTasks)
        {
            var referenceTime = DateTime.UtcNow;
            foreach (var scheduledTask in scheduledTasks)
            {
                AddTask(scheduledTask, referenceTime);
            }
        }

        public void AddTask(IScheduledTask scheduledTask, DateTime? nextRunTime = null)
        {
            if (scheduledTask.Interval < TimeSpan.FromMinutes(1))
            {
                throw new NotSupportedException();
            }
            _scheduledTasks.Add(new SchedulerTaskWrapper
            {
                Interval = scheduledTask.Interval,
                Task = scheduledTask,
                NextRunTime = nextRunTime ?? DateTime.UtcNow
            });
        }

        public event EventHandler<UnobservedTaskExceptionEventArgs> UnobservedTaskException;

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await ExecuteOnceAsync(cancellationToken);

                await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
            }
        }

        private async Task ExecuteOnceAsync(CancellationToken cancellationToken)
        {
            var taskFactory = new TaskFactory(TaskScheduler.Current);
            var referenceTime = DateTime.UtcNow;

            var tasksThatShouldRun = _scheduledTasks.Where(t => t.ShouldRun(referenceTime)).ToList();

            foreach (var taskThatShouldRun in tasksThatShouldRun)
            {
                taskThatShouldRun.Increment();

                await taskFactory.StartNew(
                    async () =>
                    {
                        try
                        {
                            await taskThatShouldRun.Task.ExecuteAsync(cancellationToken);
                        }
                        catch (Exception ex)
                        {
                            var args = new UnobservedTaskExceptionEventArgs(
                                ex as AggregateException ?? new AggregateException(ex));

                            UnobservedTaskException?.Invoke(this, args);

                            if (!args.Observed)
                            {
                                throw;
                            }
                        }
                    },
                    cancellationToken);
            }
        }

        private class SchedulerTaskWrapper
        {
            public TimeSpan Interval { private get; set; }
            public IScheduledTask Task { get; set; }

            private DateTime LastRunTime { get; set; }
            public DateTime NextRunTime { private get; set; }

            public void Increment()
            {
                LastRunTime = NextRunTime;
                NextRunTime = LastRunTime + Interval;
            }

            public bool ShouldRun(DateTime currentTime) => NextRunTime < currentTime && LastRunTime != NextRunTime;
        }
    }
}