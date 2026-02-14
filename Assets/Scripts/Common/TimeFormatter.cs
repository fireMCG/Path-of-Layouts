using UnityEngine;

namespace fireMCG.PathOfLayouts.Common
{
    public static class TimeFormatter
    {
        public static string FormatTime(float timeInSeconds)
        {
            int totalSeconds = Mathf.FloorToInt(timeInSeconds);

            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;

            return $"{minutes}:{seconds:00}";
        }

        public static string FormatTimeExplicit(float timeInSeconds)
        {
            int totalSeconds = Mathf.FloorToInt(timeInSeconds);

            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;

            return $"{minutes}m{seconds:00}s";
        }
    }
}