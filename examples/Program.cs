using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Impl.Matchers;
using Quartz.Listener;
using Serilog;
using System;
using System.Threading.Tasks;

namespace Hosting.Extensions.Quartz.Example
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration().WriteTo.ColoredConsole().CreateLogger();

            var host = new HostBuilder()
                .ConfigureAppConfiguration(x => x.AddJsonFile("appsettings.json"))
                .ConfigureServices((services) =>
                {
                    services.AddJobService<ConsolePrintJob>((job, trigger) =>
                    {
                        job.WithIdentity("ConsolePrintJob").WithDescription("Simple job");
                        trigger.StartNow().WithSimpleSchedule((x) => x.WithIntervalInSeconds(5).RepeatForever());
                    });
                    services.AddJobService<LogWriteJob>((job, trigger) =>
                    {
                        job.WithIdentity("LogWriteJob").WithDescription("Simple job");
                        trigger.StartNow().WithSimpleSchedule((x) => x.WithIntervalInSeconds(2).RepeatForever());
                    });
                    services.AddJobService<Job1>((job) =>
                    {
                        job.WithIdentity("Job1");
                    });
                })
                // You can use the other `UserQuartz` 2 methods
                // if you only want to configure either the scheduler factory
                // or the scheduler instance
                .UseQuartz(
                    (context, config) =>
                    {
                        // Here you can further customize options passed down to the StdSchedulerFactory
                        config.Set("quartz.threadPool.threadCount", Environment.ProcessorCount.ToString());
                    },
                    (context, provider, scheduler) =>
                    {
                        // You can further configure the scheduler instance here, like 
                        // add job listeners, trigger listeners, etc.
                        // DO NOT call the Start method here as it will be automatically
                        // invoked by the hosted service once it is started.
                        var listener = new JobChainingJobListener("Chain");
                        var firstJob = new JobKey("ConsolePrintJob");
                        listener.AddJobChainLink(firstJob, new JobKey("Job1"));
                        scheduler.ListenerManager.AddJobListener(listener, KeyMatcher<JobKey>.KeyEquals(firstJob));
                    }
                )
                .UseConsoleLifetime()
                .UseSerilog()
                .Build();

            await host.RunAsync();
        }
    }
}
