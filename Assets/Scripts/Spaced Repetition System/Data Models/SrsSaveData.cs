using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace fireMCG.PathOfLayouts.Srs
{
    [Serializable]
    public sealed class SrsSaveData
    {
        [JsonProperty("schemaVersion")]
        public int schemaVersion = 1;

        // Key = actId-areaId-graphId-layoutId
        [JsonProperty("layouts")]
        public Dictionary<string, SrsLayoutData> layouts = new();

        public static SrsSaveData CreateDefault()
        {
            return new SrsSaveData();
        }
    }
}