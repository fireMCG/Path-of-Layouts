using fireMCG.PathOfLayouts.Messaging;

public class OnAppStateChanged : IMessage
{
    public readonly StateController.AppState PreviousState;
    public readonly StateController.AppState NewState;

    public OnAppStateChanged(StateController.AppState previousState, StateController.AppState newState)
    {
        PreviousState = previousState;
        NewState = newState;
    }
}