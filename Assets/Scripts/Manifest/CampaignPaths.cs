using System.IO;
using UnityEngine;

namespace fireMCG.PathOfLayouts.Manifest
{
    public static class CampaignPaths
    {
        public const string MANIFEST_ROOT_FOLDER = "Campaign";
        public const string MANIFEST_FILE_NAME = "campaign_manifest.json";
        public const string SCREENSHOT_SEPARATOR = "_sshot";
        public const string COLLISION_MAP_SUFFIX = ".collision";

        public static string GetManifestRootPath()
        {
            return Path.Combine(Application.streamingAssetsPath, MANIFEST_ROOT_FOLDER);
        }

        public static string GetManifestFilePath()
        {
            return Path.Combine(GetManifestRootPath(), MANIFEST_FILE_NAME);
        }
    }
}