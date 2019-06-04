using System.Threading.Tasks;
using Quartz;
using Serilog;

namespace Hosting.Extensions.Quartz.Example
{
    public class Job1 : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            Log.Information("Job1");
            return Task.CompletedTask;
        }
    }
}