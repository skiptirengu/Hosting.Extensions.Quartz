using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Impl.Matchers;
using Quartz.Listener;
using Quartz.Spi;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Hosting.Extensions.Quartz.Tests
{
    public class QuartzTest
    {
        [Fact]
        public void HostBuilderExtension()
        {
            var dictionary = new Dictionary<string, string>()
            {
                { "Quartz:quartz.threadPool.threadCount", "1" },
                { "Quartz:quartz.threadPool.threadPriority", "Normal" }
            };

            var host = new HostBuilder()
                .ConfigureAppConfiguration(x => x.AddInMemoryCollection(dictionary))
                .UseQuartz(
                    (ctx, c) =>
                    {
                        c.Set("quartz.threadPool.threadCount", "4");
                    },
                    (context, provider, sched) =>
                    {
                        sched.ListenerManager.AddJobListener(new JobChainingJobListener("TestListener"), KeyMatcher<JobKey>.KeyEquals(new JobKey("DummyJob")));
                    }
                )
                .ConfigureServices((context, services) =>
                {
                    services
                        .AddJobService<IDummyJob, DummyJob>((job, trigger) =>
                        {
                            job.WithIdentity("DummyJob").WithDescription("IDummyJob_Job");
                            trigger.WithDescription("IDummyJob_Trigger");
                        })
                        .AddJobService<EvenDummierJob>((job, trigger) =>
                        {
                            job.WithDescription("EvenDummierJob_Job");
                            trigger.WithDescription("EvenDummierJob_Trigger");
                        });
                })
                .Build();

            var s = host.Services;
            var scheduler = s.GetRequiredService<IScheduler>();
            Assert.IsAssignableFrom<IScheduler>(scheduler);

            var config = s.GetRequiredService<QuartzConfigCollection>();
            Assert.Equal("4", config["quartz.threadPool.threadCount"]);
            Assert.Equal("Normal", config["quartz.threadPool.threadPriority"]);

            Assert.IsType<JobFactory>(s.GetRequiredService<IJobFactory>());
            Assert.IsType<QuartzHostedService>(s.GetService<IHostedService>());

            var jobs = s.GetRequiredService<IEnumerable<IJobSchedule>>().ToArray();
            Assert.True(jobs[0].JobDetail.JobType.IsAssignableFrom(typeof(IDummyJob)));
            Assert.Equal("IDummyJob_Job", jobs[0].JobDetail.Description);
            Assert.Equal("IDummyJob_Trigger", jobs[0].Trigger.Description);

            Assert.True(jobs[1].JobDetail.JobType.IsAssignableFrom(typeof(EvenDummierJob)));
            Assert.Equal("EvenDummierJob_Job", jobs[1].JobDetail.Description);
            Assert.Equal("EvenDummierJob_Trigger", jobs[1].Trigger.Description);

            var matchers = scheduler.ListenerManager.GetJobListeners();
            Assert.Single(matchers);
            Assert.Equal("TestListener", matchers.First().Name);
        }
    }
}
