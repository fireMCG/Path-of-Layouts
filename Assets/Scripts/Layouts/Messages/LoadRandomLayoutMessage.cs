using fireMCG.PathOfLayouts.Messaging;

namespace fireMCG.PathOfLayouts.Layouts
{
    public sealed class LoadRandomLayoutMessage : IMessage
    {
        public readonly string ActId;
        public readonly string AreaId;
        public readonly string GraphId;

        public LoadRandomLayoutMessage(string actId, string areaId, string graphId)
        {
            ActId = actId;
            AreaId = areaId;
            GraphId = graphId;
        }
    }
}