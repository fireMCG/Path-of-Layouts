using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace fireMCG.PathOfLayouts.Srs
{
    [Serializable]
    public sealed class SrsSaveData
    {
        [JsonProperty("schemaVersion")]
        public int schemaVersion = 2;

        [JsonProperty("entries")]
        public Dictionary<string, SrsEntryData> entries = new();

        public static SrsSaveData CreateDefault()
        {
            return new SrsSaveData();
        }
    }
}