using Newtonsoft.Json;
using System;

namespace fireMCG.PathOfLayouts.Manifest
{
    [Serializable]
    public class GraphEntry
    {
        [JsonProperty("graphId")]
        public string graphId;

        [JsonProperty("fileExtension")]
        public string fileExtension;
    }
}