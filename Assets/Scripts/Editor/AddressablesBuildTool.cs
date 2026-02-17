#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace fireMCG.PathOfLayouts.EditorTools
{
    public static class AddressablesBuildTools
    {
        [MenuItem("Path of Layouts/Addressables/Configure/Configure Content for Platform: PC")]
        public static void ConfigurePcLocal()
        {
            AddressablesPlatformConfigurator.ConfigureFor(BuildTarget.StandaloneWindows64);
            Debug.Log("[Addressables] Configured for PC Local.");
        }

        [MenuItem("Path of Layouts/Addressables/Configure/Configure Content for Platform: Linux")]
        public static void ConfigureLinuxLocal()
        {
            AddressablesPlatformConfigurator.ConfigureFor(BuildTarget.StandaloneLinux64);
            Debug.Log("[Addressables] Configured for Linux Local.");
        }

        [MenuItem("Path of Layouts/Addressables/Configure/Configure Content for Platform: Android")]
        public static void ConfigureAndroidCcd()
        {
            AddressablesPlatformConfigurator.ConfigureFor(BuildTarget.Android);
            Debug.Log("[Addressables] Configured for Android CCD.");
        }

        [MenuItem("Path of Layouts/Addressables/Build/Build Content for Active Platform")]
        public static void BuildContentForCurrentTargetPlatform()
        {
            BuildTarget target = EditorUserBuildSettings.activeBuildTarget;
            AddressablesPlatformConfigurator.ConfigureFor(target);

            ClearCache();

            AddressableAssetSettings.BuildPlayerContent();
            Debug.Log($"[Addressables] BuildPlayerContent complete for {target}.");
        }

        [MenuItem("Path of Layouts/Addressables/Build/Build Content for Platform: Android")]
        public static void BuildAndroidCcdContent()
        {
            SwitchTarget(BuildTargetGroup.Android, BuildTarget.Android);

            AddressablesPlatformConfigurator.ConfigureFor(BuildTarget.Android);

            ClearCache();

            AddressableAssetSettings.BuildPlayerContent();
            Debug.Log("[Addressables] Built Android CCD content. Upload the ServerData/Android output to CCD.");
        }

        [MenuItem("Path of Layouts/Addressables/Build/Build Content for Platform: Windows")]
        public static void BuildWindowsLocalContent()
        {
            SwitchTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);

            AddressablesPlatformConfigurator.ConfigureFor(BuildTarget.StandaloneWindows64);

            ClearCache();

            AddressableAssetSettings.BuildPlayerContent();
            Debug.Log("[Addressables] Built Windows local content.");
        }

        [MenuItem("Path of Layouts/Addressables/Build/Build Content for Platform: Linux")]
        public static void BuildLinuxLocalContent()
        {
            SwitchTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneLinux64);

            AddressablesPlatformConfigurator.ConfigureFor(BuildTarget.StandaloneLinux64);

            ClearCache();

            AddressableAssetSettings.BuildPlayerContent();
            Debug.Log("[Addressables] Built Linux local content.");
        }

        private static void SwitchTarget(BuildTargetGroup group, BuildTarget target)
        {
            if (EditorUserBuildSettings.activeBuildTarget == target)
            {
                return;
            }

            bool ok = EditorUserBuildSettings.SwitchActiveBuildTarget(group, target);
            if (!ok)
            {
                throw new System.InvalidOperationException($"Failed to switch build target to {target}.");
            }
        }

        private static void ClearCache()
        {
            AddressableAssetSettings.CleanPlayerContent();
            Caching.ClearCache();
        }
    }
}
#endif