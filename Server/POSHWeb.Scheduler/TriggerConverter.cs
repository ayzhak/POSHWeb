using POSHWeb.Scheduler.Trigger;
using Quartz;

namespace POSHWeb.Scheduler.Converter;

public static class TriggerConverter
{
    public static ITrigger Convert(ScheduleTrigger scheduleTrigger)
    {
        var trigger = TriggerBuilder.Create();
        if (scheduleTrigger.Mode == SchedulerOperationMode.Simple)
        {
            if (scheduleTrigger.Start.HasValue)
            {
                trigger.StartAt(scheduleTrigger.Start.Value);
            }
            else
            {
                trigger.StartNow();
            }

            if (scheduleTrigger.End.HasValue)
            {
                trigger.EndAt(scheduleTrigger.End.Value);
            }

            if (scheduleTrigger.Interval == null) throw new InvalidOperationException();
            trigger.WithSimpleSchedule(builder =>
            {
                switch (scheduleTrigger.IntervalType)
                {
                    case Trigger.IntervalType.Millisecond:
                        builder.WithInterval(TimeSpan.FromMilliseconds(scheduleTrigger.Interval.Value));
                        break;
                    case Trigger.IntervalType.Second:
                        builder.WithInterval(TimeSpan.FromSeconds(scheduleTrigger.Interval.Value));
                        break;
                    case Trigger.IntervalType.Minute:
                        builder.WithInterval(TimeSpan.FromMinutes(scheduleTrigger.Interval.Value));
                        break;
                    case Trigger.IntervalType.Hour:
                        builder.WithInterval(TimeSpan.FromHours(scheduleTrigger.Interval.Value));
                        break;
                    case Trigger.IntervalType.Day:
                        builder.WithInterval(TimeSpan.FromDays(scheduleTrigger.Interval.Value));
                        break;
                    case Trigger.IntervalType.Week:
                        builder.WithInterval(TimeSpan.FromDays(scheduleTrigger.Interval.Value * 7));
                        break;
                    case Trigger.IntervalType.Month:
                        builder.WithInterval(TimeSpan.FromDays(scheduleTrigger.Interval.Value * 30));
                        break;
                    case Trigger.IntervalType.Year:
                        builder.WithInterval(TimeSpan.FromDays(scheduleTrigger.Interval.Value * 365));
                        break;
                }

                if (scheduleTrigger.Count.HasValue)
                {
                    builder.WithRepeatCount(scheduleTrigger.Count.Value);
                }
                else
                {
                    builder.RepeatForever();
                }
            });
        }

        if (scheduleTrigger.Mode == SchedulerOperationMode.Cron)
        {
            if (scheduleTrigger.Cron == null) throw new InvalidOperationException();
            trigger.WithCronSchedule(scheduleTrigger.Cron);
        }

        if (scheduleTrigger.Mode == SchedulerOperationMode.Continues)
        {
            trigger.StartNow()
                .WithSimpleSchedule(builder =>
                {
                    builder.RepeatForever();
                    if (!scheduleTrigger.Delay.HasValue)
                    {
                        builder.WithInterval(TimeSpan.FromMilliseconds(1));
                    }

                    switch (scheduleTrigger.DelayType)
                    {
                        case Trigger.IntervalType.Millisecond:
                            builder.WithInterval(TimeSpan.FromMilliseconds(scheduleTrigger.Delay.Value));
                            break;
                        case Trigger.IntervalType.Second:
                            builder.WithInterval(TimeSpan.FromSeconds(scheduleTrigger.Delay.Value));
                            break;
                        case Trigger.IntervalType.Minute:
                            builder.WithInterval(TimeSpan.FromMinutes(scheduleTrigger.Delay.Value));
                            break;
                        case Trigger.IntervalType.Hour:
                            builder.WithInterval(TimeSpan.FromHours(scheduleTrigger.Delay.Value));
                            break;
                        case Trigger.IntervalType.Day:
                            builder.WithInterval(TimeSpan.FromDays(scheduleTrigger.Delay.Value));
                            break;
                        case Trigger.IntervalType.Week:
                            builder.WithInterval(TimeSpan.FromDays(scheduleTrigger.Delay.Value * 7));
                            break;
                        case Trigger.IntervalType.Month:
                            builder.WithInterval(TimeSpan.FromDays(scheduleTrigger.Delay.Value * 30));
                            break;
                        case Trigger.IntervalType.Year:
                            builder.WithInterval(TimeSpan.FromDays(scheduleTrigger.Delay.Value * 365));
                            break;
                    }
                });
        }

        if (scheduleTrigger.Mode == SchedulerOperationMode.Now)
        {
            trigger.StartNow();
        }

        if (scheduleTrigger.Mode == SchedulerOperationMode.Once)
        {
            if (!scheduleTrigger.Start.HasValue) throw new InvalidOperationException();
            trigger.StartAt(scheduleTrigger.Start.Value);
        }
        
        return trigger.Build();
    }
}