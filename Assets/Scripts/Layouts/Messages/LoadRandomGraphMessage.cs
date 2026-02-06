using fireMCG.PathOfLayouts.Messaging;

namespace fireMCG.PathOfLayouts.Layouts
{
    public sealed class LoadRandomGraphMessage : IMessage
    {
        public readonly string ActId;
        public readonly string AreaId;

        public LoadRandomGraphMessage(string actId, string areaId)
        {
            ActId = actId;
            AreaId = areaId;
        }
    }
}