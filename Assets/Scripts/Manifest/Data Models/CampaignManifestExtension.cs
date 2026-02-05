using System.Collections.Generic;
using System.Linq;

namespace fireMCG.PathOfLayouts.Manifest
{
    public static class CampaignManifestExtension
    {
        public static IReadOnlyList<AreaEntry> GetAreas(this CampaignManifest manifest, string actId)
        {
            return manifest.acts
                .FirstOrDefault(act => act.actId == actId).areas;
        }

        public static IReadOnlyList<GraphEntry> GetGraphs(this CampaignManifest manifest, string actId, string areaId)
        {
            return manifest.acts
                .FirstOrDefault(act => act.actId == actId).areas
                .FirstOrDefault(area =>  area.areaId == areaId).graphs;
        }
    }
}