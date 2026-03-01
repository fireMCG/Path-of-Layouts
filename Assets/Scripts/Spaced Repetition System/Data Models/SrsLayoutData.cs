using Newtonsoft.Json;
using System;

namespace fireMCG.PathOfLayouts.Srs
{
    public enum SrsDataType { Area, Graph, Layout }

    [Serializable]
    public sealed class SrsEntryData
    {
        [JsonProperty("id")]
        public string id;

        [JsonProperty("dataType")]
        public int dataType;

        [JsonProperty("isLearning")]
        public bool isLearning;

        [JsonProperty("masteryLevel")]
        public int masteryLevel;

        [JsonProperty("timesPracticed")]
        public int timesPracticed;

        [JsonProperty("timesSucceeded")]
        public int timesSucceeded;

        [JsonProperty("timesFailed")]
        public int timesFailed;

        [JsonProperty("lastPracticedUtc")]
        public string lastPracticedUtc;

        [JsonProperty("lastResult")]
        public string lastResult;

        [JsonProperty("streak")]
        public int streak;

        [JsonProperty("bestTimeSeconds")]
        public float bestTimeSeconds;

        [JsonProperty("averageTimeSeconds")]
        public float averageTimeSeconds;

        public SrsEntryData(string entryId, SrsDataType entryType)
        {
            id = entryId;
            dataType = (int)entryType;
            isLearning = false;
            masteryLevel = 0;
            timesPracticed = 0;
            timesSucceeded = 0;
            timesFailed = 0;
            lastPracticedUtc = null;
            lastResult = null;
            streak = 0;
            bestTimeSeconds = 0;
            averageTimeSeconds = 0;
        }
    }
}