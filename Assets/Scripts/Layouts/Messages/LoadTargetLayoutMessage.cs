using fireMCG.PathOfLayouts.Messaging;

namespace fireMCG.PathOfLayouts.Layouts
{
    public sealed class LoadTargetLayoutMessage : IMessage
    {
        public readonly string ActId;
        public readonly string AreaId;
        public readonly string GraphId;
        public readonly string LayoutId;

        public LoadTargetLayoutMessage(string actId, string areaId, string graphId, string layoutId)
        {
            ActId = actId;
            AreaId = areaId;
            GraphId = graphId;
            LayoutId = layoutId;
        }
    }
}