using Quartz;

namespace NetCore.Hosting.Quartz
{
    public interface IJobSchedule
    {
        IJobDetail JobDetail { get; }
        ITrigger Trigger { get; }
    }
}
