using Microsoft.Extensions.Logging;
using Quartz;
using System.Threading.Tasks;

namespace GenericHost.Extensions.Quartz.Example
{
    public class LogWriteJob : IJob
    {
        public LogWriteJob(ILogger<LogWriteJob> logger)
        {
            Logger = logger;
        }

        public ILogger<LogWriteJob> Logger { get; }

        public Task Execute(IJobExecutionContext context)
        {
            Logger.LogInformation("Logging to console");
            return Task.CompletedTask;
        }
    }
}
