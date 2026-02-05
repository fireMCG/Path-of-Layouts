using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace fireMCG.PathOfLayouts.Manifest
{
    [Serializable]
    public class AreaEntry
    {
        [JsonProperty("areaId")]
        public string areaId;

        [JsonProperty("graphs")]
        public List<GraphEntry> graphs = new();
    }
}