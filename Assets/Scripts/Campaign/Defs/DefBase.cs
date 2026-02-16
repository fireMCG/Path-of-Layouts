using UnityEditor;
using UnityEngine;

namespace fireMCG.PathOfLayouts.Campaign
{
    public abstract class DefBase : ScriptableObject
    {
        public string id;

        public string displayName;

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                string path = AssetDatabase.GetAssetPath(this);
                if (!string.IsNullOrWhiteSpace(path))
                {
                    id = AssetDatabase.AssetPathToGUID(path);
                    EditorUtility.SetDirty(this);
                }
            }
        }
#endif
    }
}