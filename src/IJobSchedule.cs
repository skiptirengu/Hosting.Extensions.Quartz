using Quartz;

namespace Hosting.Extensions.Quartz
{
    public interface IJobSchedule
    {
        IJobDetail JobDetail { get; }
        ITrigger Trigger { get; }
    }
}
