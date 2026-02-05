using Newtonsoft.Json;
using System;

namespace fireMCG.PathOfLayouts.Manifest
{
    [Serializable]
    public class LayoutEntry
    {
        [JsonProperty("layoutId")]
        public string layoutId;

        [JsonProperty("displayName")]
        public string displayName;

        [JsonProperty("flags")]
        public string[] flags;
    }
}