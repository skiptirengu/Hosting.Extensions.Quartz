using Quartz;

namespace Hosting.Extensions.Quartz
{
    public class JobSchedule : IJobSchedule
    {
        public IJobDetail JobDetail { get; }
        public ITrigger Trigger { get; }

        public JobSchedule(IJobDetail detail, ITrigger trigger)
        {
            JobDetail = detail;
            Trigger = trigger;
        }
    }
}
