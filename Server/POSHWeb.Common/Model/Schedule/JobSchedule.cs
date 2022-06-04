using POSHWeb.Scheduler.Trigger;

namespace POSHWeb.Model.Schedule
{
    public class JobSchedule
    {
        public int Id { get; set; }
        public PSScript Script { get; set; }
        public ScheduleTrigger Schedule { get; set; }
    }
}
