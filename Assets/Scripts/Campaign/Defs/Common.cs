using System;
using UnityEngine;

namespace fireMCG.PathOfLayouts.Campaign.Common
{
    public enum NodeType
    {
        Waypoint,
        Checkpoint,
        Entrance,
        Exit,
        Other
    }

    [Serializable]
    public sealed class TagSet
    {
        public string[] tags = Array.Empty<string>();
    }

    [Serializable]
    public sealed class NavigationNode
    {
        public string displayName = string.Empty;
        public Vector2 normalizedPosition = new Vector2(0.5f, 0.5f);
        public NodeType nodeType = NodeType.Other;

        public string linkedAreaId = string.Empty;

        public NavigationNode(Vector2 normalizedPosition)
        {
            this.normalizedPosition = normalizedPosition;
        }
    }
}