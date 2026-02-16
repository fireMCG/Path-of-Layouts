using System;
using UnityEngine;

namespace fireMCG.PathOfLayouts.Campaign
{
    public enum NodeType
    {
        Waypoint,
        Checkpoint,
        Entrance,
        Other
    }

    [Serializable]
    public sealed class TagSet
    {
        public string[] tags = Array.Empty<string>();
    }

    [Serializable]
    public struct NavigationNode
    {
        public NodeType nodeType;

        public string linkedAreaId;

        public string displayName;

        public Vector2 normalizedPosition;
    }
}