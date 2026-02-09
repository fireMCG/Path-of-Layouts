using fireMCG.PathOfLayouts.Core;
using fireMCG.PathOfLayouts.Messaging;
using UnityEngine;

namespace fireMCG.PathOfLayouts.Gameplay
{
    public class GameplayUiController : MonoBehaviour
    {
        [SerializeField] private GameplayController _gameplayController;

        public void Replay()
        {
            _gameplayController.Replay();
        }

        public void Quit()
        {
            MessageBusManager.Resolve.Publish(new OnAppStateChangeRequest(StateController.AppState.LayoutBrowser));
        }
    }
}