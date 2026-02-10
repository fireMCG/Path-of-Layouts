using System;

namespace fireMCG.PathOfLayouts.Srs
{
    public static class SrsScheduler
    {
        public static readonly TimeSpan[] MasteryIntervals =
        {
            TimeSpan.Zero,              // 0
            TimeSpan.FromMinutes(30),   // 1
            TimeSpan.FromHours(3),      // 2
            TimeSpan.FromHours(12),     // 3
            TimeSpan.FromDays(1),       // 4
            TimeSpan.FromDays(3),       // 5
            TimeSpan.FromDays(7),       // 6
            TimeSpan.FromDays(14),      // 7
            TimeSpan.FromDays(30),      // 8
            TimeSpan.FromDays(90)       // 9
        };

        public static int ClampMastery(int masteryLevel)
        {
            if(masteryLevel < 0)
            {
                return 0;
            }

            int max = MasteryIntervals.Length - 1;
            if(masteryLevel > max)
            {
                return max;
            }

            return masteryLevel;
        }

        public static DateTime ComputeNextDueDate(DateTime utcNow, int newMasteryLevel)
        {
            newMasteryLevel = ClampMastery(newMasteryLevel);
            TimeSpan interval = MasteryIntervals[newMasteryLevel];

            return utcNow.Add(interval);
        }
    }
}