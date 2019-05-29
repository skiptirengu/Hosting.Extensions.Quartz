using Quartz;
using System;
using System.Threading.Tasks;

namespace Hosting.Extensions.Quartz.Tests
{
    public class DummyJob : IDummyJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            throw new NotImplementedException();
        }
    }
}
