using fireMCG.PathOfLayouts.Messaging;

namespace fireMCG.PathOfLayouts.NodeEditor
{
    public sealed class NodeEditorOpenMessage : IMessage
    {
        public readonly string LayoutId;

        public NodeEditorOpenMessage(string layoutId)
        {
            LayoutId = layoutId;
        }
    }
}