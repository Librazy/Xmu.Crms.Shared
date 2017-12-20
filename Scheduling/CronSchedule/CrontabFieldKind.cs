using System;

namespace Xmu.Crms.Shared.Scheduling.CronSchedule
{
    [Serializable]
    public enum CrontabFieldKind
    {
        Minute,
        Hour,
        Day,
        Month,
        DayOfWeek
    }
}