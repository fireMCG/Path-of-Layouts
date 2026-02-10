using System;

namespace fireMCG.PathOfLayouts.Srs
{
    public static class SrsLayoutDataExtension
    {
        public static float GetSuccessRate(this SrsLayoutData data)
        {
            return 100f / (data.timesSucceeded + data.timesFailed) * data.timesSucceeded;
        }

        public static DateTime GetDueDateTime(this SrsLayoutData data)
        {
            DateTime.TryParse(data.lastPracticedUtc, out DateTime lastPracticed);

            return lastPracticed.Add(SrsScheduler.MasteryIntervals[data.masteryLevel]);
        }
    }
}