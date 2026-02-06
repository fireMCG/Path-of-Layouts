using fireMCG.PathOfLayouts.Messaging;

namespace fireMCG.PathOfLayouts.Layouts
{
    public sealed class LoadRandomAreaMessage : IMessage
    {
        public readonly string ActId;

        public LoadRandomAreaMessage(string actId)
        {
            ActId = actId;
        }
    }
}