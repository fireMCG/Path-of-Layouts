#if UNITY_EDITOR
using fireMCG.PathOfLayouts.Content;
using System;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;

namespace fireMCG.PathOfLayouts.EditorTools
{
    public static class AddressablesPlatformConfigurator
    {
        // Windows & Linux
        private const string LocalBuildPath = "Local.BuildPath";
        private const string LocalLoadPath = "Local.LoadPath";

        // Android
        private const string RemoteBuildPath = "Remote.BuildPath";
        private const string RemoteLoadPath = "Remote.LoadPath";

        public static void ConfigureFor(BuildTarget target)
        {
            AddressableAssetSettings settings = GetSettingsOrThrow();

            switch (target)
            {
                case BuildTarget.Android:
                    ConfigureAndroidRemote(settings);
                    EnsureRemoteLoadPathIsValid(settings);
                    break;

                case BuildTarget.StandaloneWindows64:
                case BuildTarget.StandaloneLinux64:
                    ConfigurePCLocal(settings);
                    break;

                default:
                    ConfigurePCLocal(settings);
                    break;
            }

            MarkSettingsDirty(settings);
        }

        private static void ConfigurePCLocal(AddressableAssetSettings settings)
        {
            SwitchActiveProfile(settings, AddressablesKeys.Profiles.PC_LOCAL);

            SetGroupBuildLoad(settings, AddressablesKeys.Groups.GRAPH_RENDERS, LocalBuildPath, LocalLoadPath);
            SetGroupBuildLoad(settings, AddressablesKeys.Groups.LAYOUT_IMAGES, LocalBuildPath, LocalLoadPath);
            SetGroupBuildLoad(settings, AddressablesKeys.Groups.LAYOUT_NAVIGATION_DATA, LocalBuildPath, LocalLoadPath);

            MarkSettingsDirty(settings);
        }

        private static void ConfigureAndroidRemote(AddressableAssetSettings settings)
        {
            SwitchActiveProfile(settings, AddressablesKeys.Profiles.ANDROID_CCD);

            SetGroupBuildLoad(settings, AddressablesKeys.Groups.GRAPH_RENDERS, RemoteBuildPath, RemoteLoadPath);
            SetGroupBuildLoad(settings, AddressablesKeys.Groups.LAYOUT_IMAGES, RemoteBuildPath, RemoteLoadPath);
            SetGroupBuildLoad(settings, AddressablesKeys.Groups.LAYOUT_NAVIGATION_DATA, RemoteBuildPath, RemoteLoadPath);

            MarkSettingsDirty(settings);
        }

        private static AddressableAssetSettings GetSettingsOrThrow()
        {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                throw new InvalidOperationException("Addressables Settings not found. Create Addressables settings first.");
            }

            return settings;
        }

        private static void SwitchActiveProfile(AddressableAssetSettings settings, string profileName)
        {
            AddressableAssetProfileSettings profileSettings = settings.profileSettings;
            if (profileSettings == null)
            {
                throw new InvalidOperationException("Addressables profileSettings is null.");
            }

            string profileId = profileSettings.GetAllProfileNames()
                .Select(n => (Name: n, Id: profileSettings.GetProfileId(n)))
                .Where(x => string.Equals(x.Name, profileName, StringComparison.Ordinal))
                .Select(x => x.Id)
                .FirstOrDefault();

            if (string.IsNullOrWhiteSpace(profileId))
            {
                throw new InvalidOperationException($"Addressables profile not found: '{profileName}'. Create it in Addressables Profiles.");
            }

            settings.activeProfileId = profileId;
        }

        private static void SetGroupBuildLoad(AddressableAssetSettings settings, string groupKey, string buildPath, string loadPath)
        {
            AddressableAssetGroup group = settings.FindGroup(groupKey);
            if (group == null)
            {
                throw new InvalidOperationException($"Addressables group not found: '{groupKey}'.");
            }

            BundledAssetGroupSchema schema = group.GetSchema<BundledAssetGroupSchema>();
            if (schema == null)
            {
                throw new InvalidOperationException($"Group '{groupKey}' is missing BundledAssetGroupSchema.");
            }

            schema.BuildPath.SetVariableByName(settings, buildPath);
            schema.LoadPath.SetVariableByName(settings, loadPath);

            EditorUtility.SetDirty(group);
        }

        private static void EnsureRemoteLoadPathIsValid(AddressableAssetSettings settings)
        {
            string val = settings.profileSettings.GetValueByName(settings.activeProfileId, RemoteLoadPath);
            if (string.IsNullOrWhiteSpace(val) || val.Contains("<undefined>", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    $"Profile '{AddressablesKeys.Profiles.ANDROID_CCD}' has invalid '{RemoteLoadPath}'. Set it to your CCD Badge URL.");
            }
        }

        private static void MarkSettingsDirty(AddressableAssetSettings settings)
        {
            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
#endif