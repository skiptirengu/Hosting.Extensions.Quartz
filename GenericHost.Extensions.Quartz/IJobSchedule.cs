using Quartz;

namespace GenericHost.Extensions.Quartz
{
    public interface IJobSchedule
    {
        IJobDetail JobDetail { get; }
        ITrigger Trigger { get; }
    }
}
