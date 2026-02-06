using fireMCG.PathOfLayouts.Messaging;

namespace fireMCG.PathOfLayouts.Core
{
    public sealed class OnAppStateChanged : IMessage
    {
        public readonly StateController.AppState PreviousState;
        public readonly StateController.AppState NewState;

        public OnAppStateChanged(StateController.AppState previousState, StateController.AppState newState)
        {
            PreviousState = previousState;
            NewState = newState;
        }
    }
}