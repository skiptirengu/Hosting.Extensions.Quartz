using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Hosting.Extensions.Quartz
{
    public class QuartzHostedService : IHostedService
    {
        private readonly IScheduler _scheduler;
        private readonly IServiceProvider _collection;
        private readonly IEnumerable<IJobSchedule> _jobs;
        private readonly ILogger<QuartzHostedService> _logger;

        public QuartzHostedService(
            IScheduler scheduler,
            IEnumerable<IJobSchedule> jobs,
            IServiceProvider collection,
            ILogger<QuartzHostedService> logger)
        {
            _scheduler = scheduler;
            _collection = collection;
            _jobs = jobs;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Scheduling {count} job(s)", _jobs.Count());
            foreach (var item in _jobs)
            {
                if (item.Trigger == null)
                {
                    await _scheduler.AddJob(item.JobDetail, true, cancellationToken);
                }
                else
                {
                    await _scheduler.ScheduleJob(item.JobDetail, item.Trigger, cancellationToken);
                }
            }
            _logger.LogInformation("Starting quartz scheduler");
            await _scheduler.Start(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stoping quartz scheduler");
            return _scheduler?.Shutdown(cancellationToken);
        }
    }
}
