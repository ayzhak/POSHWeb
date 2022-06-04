namespace POSHWeb.Scheduler.Trigger;

public class ScheduleTrigger
{
    public int Id { get; set; }
    public string Name { get; set; }
    public SchedulerOperationMode Mode { get; set; }
    public DateTimeOffset? Start { get; set; }
    public DateTimeOffset? End { get; set; }
    public double? Interval { get; set; }
    public IntervalType? IntervalType { get; set; }
    public int? Count { get; set; }
    public string? Cron { get; set; }
    public double? Delay { get; set; }
    public IntervalType? DelayType { get; set; }


    public bool Validate()
    {
        if (Mode == SchedulerOperationMode.Simple)
        {
            return Interval.HasValue && IntervalType.HasValue && Interval.Value > 0;
        }

        if (Mode == SchedulerOperationMode.Cron)
        {
            return Cron != null;
        }

        if (Mode == SchedulerOperationMode.Continues)
        {
            return true;
        }

        if (Mode == SchedulerOperationMode.Now)
        {
            return true;
        }

        if (Mode == SchedulerOperationMode.Once)
        {
            return Start.HasValue;
        }

        throw new NotSupportedException("Specify the Type");
    }
}