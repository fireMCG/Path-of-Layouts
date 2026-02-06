using fireMCG.PathOfLayouts.Messaging;

namespace fireMCG.PathOfLayouts.Core
{
    public sealed class OnAppStateChangeRequest : IMessage
    {
        public readonly StateController.AppState TargetState;

        public OnAppStateChangeRequest(StateController.AppState targetState)
        {
            TargetState = targetState;
        }
    }
}