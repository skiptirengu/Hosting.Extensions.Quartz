using Quartz;
using System;
using System.Threading.Tasks;

namespace Hosting.Extensions.Quartz.Example
{
    public class ConsolePrintJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            Console.WriteLine("Executing job");
            return Task.CompletedTask;
        }
    }
}
