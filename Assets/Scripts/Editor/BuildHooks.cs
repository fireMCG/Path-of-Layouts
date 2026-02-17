#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace fireMCG.PathOfLayouts.EditorTools
{
    public sealed class BuildHooks : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            if (EditorUserBuildSettings.activeBuildTarget != report.summary.platform)
            {
                UnityEngine.Debug.LogWarning(
                    $"Active build target ({EditorUserBuildSettings.activeBuildTarget}).\n" +
                    $"Mismatch with report target ({report.summary.platform}).\n" +
                    $"Continuing anyway.");
            }

            AddressablesPlatformConfigurator.ConfigureFor(report.summary.platform);
        }
    }
}
#endif