using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Quartz;
using Serilog;
using System;
using System.Threading.Tasks;

namespace GenericHost.Extensions.Quartz.Example
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
                })
                .UseQuartz((context, config) =>
                {
                    config.Set("quartz.threadPool.threadCount", Environment.ProcessorCount.ToString());
                })
                .UseConsoleLifetime()
                .UseSerilog()
                .Build();

            await host.RunAsync();
        }
    }
}
