using fireMCG.PathOfLayouts.Common;
using System;

namespace fireMCG.PathOfLayouts.Srs
{
    public static class SrsEntryDataExtension
    {
        public static float GetSuccessRate(this SrsEntryData data)
        {
            return 100f / (data.timesSucceeded + data.timesFailed) * data.timesSucceeded;
        }

        public static DateTime GetDueDateTime(this SrsEntryData data)
        {
            if(!DateTimeExtension.TryParseIsoUtc(data.lastPracticedUtc, out DateTime lastPracticed))
            {
                lastPracticed = DateTime.MinValue;
            }

            return lastPracticed.Add(SrsScheduler.MasteryIntervals[data.masteryLevel]);
        }

        public static string GetTimeStringUntilDue(this SrsEntryData data, DateTime? utcNow = null)
        {
            utcNow = utcNow ?? DateTime.UtcNow;

            DateTime dueDateTime = data.GetDueDateTime();

            if(dueDateTime <= utcNow)
            {
                return "Due";
            }

            TimeSpan dueTimeSpan = (dueDateTime - utcNow).Value;

            if(dueTimeSpan.Ticks > 0 && dueTimeSpan.Seconds > 0)
            {
                dueTimeSpan.Add(TimeSpan.FromMinutes(1));
            }

            TimeSpan roundedTimeSpan = new TimeSpan(dueTimeSpan.Days, dueTimeSpan.Hours, dueTimeSpan.Minutes, 0);

            int days = roundedTimeSpan.Days;
            int hours = roundedTimeSpan.Hours;
            int minutes = roundedTimeSpan.Minutes;

            if(days > 0)
            {
                return $"{days}d{hours:D2}h{minutes:D2}m";
            }

            if(hours > 0)
            {
                return $"{hours}h{minutes:D2}m";
            }

            return $"{minutes}m";
        }

        public static float GetRunningAverageTime(this SrsEntryData data, float newTimeSeconds)
        {
            if (data.timesPracticed == 0)
            {
                return newTimeSeconds;
            }

            return ((data.averageTimeSeconds * data.timesPracticed) + newTimeSeconds) / (data.timesPracticed + 1);
        }
    }
}