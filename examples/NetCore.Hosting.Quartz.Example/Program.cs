using Microsoft.Extensions.Hosting;
using Quartz;
using Serilog;
using System.Threading.Tasks;

namespace NetCore.Hosting.Quartz.Example
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration().WriteTo.ColoredConsole().CreateLogger();

            var host = new HostBuilder()
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
                .UseQuartz()
                .UseConsoleLifetime()
                .UseSerilog()
                .Build();

            await host.RunAsync();
        }
    }
}
