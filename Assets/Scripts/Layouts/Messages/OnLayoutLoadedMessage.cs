using fireMCG.PathOfLayouts.Messaging;
using UnityEngine;

namespace fireMCG.PathOfLayouts.Layouts
{
    public class OnLayoutLoadedMessage : IMessage
    {
        public readonly string ActId;
        public readonly string AreaId;
        public readonly string GraphId;
        public readonly string LayoutId;
        public readonly Texture2D LayoutMap;
        public readonly Texture2D CollisionMap;

        public OnLayoutLoadedMessage(string actId, string areaId, string graphId, string layoutId, Texture2D layoutMap, Texture2D collisionMap)
        {
            ActId = actId;
            AreaId = areaId;
            GraphId = graphId;
            LayoutId = layoutId;
            LayoutMap = layoutMap;
            CollisionMap = collisionMap;
        }
    }
}