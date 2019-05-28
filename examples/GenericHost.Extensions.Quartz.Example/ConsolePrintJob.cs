using Quartz;
using System;
using System.Threading.Tasks;

namespace GenericHost.Extensions.Quartz.Example
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
