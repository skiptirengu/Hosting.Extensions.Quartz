using Quartz;
using System.Threading.Tasks;

namespace Hosting.Extensions.Quartz.Tests
{
    public class EvenDummierJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            return Task.CompletedTask;
        }
    }
}
