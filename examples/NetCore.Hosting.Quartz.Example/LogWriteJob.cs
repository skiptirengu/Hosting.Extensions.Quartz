using Quartz;
using Serilog;
using System.Threading.Tasks;

namespace NetCore.Hosting.Quartz.Example
{
    public class LogWriteJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            Log.Information("Logging to console");
            return Task.CompletedTask;
        }
    }
}
