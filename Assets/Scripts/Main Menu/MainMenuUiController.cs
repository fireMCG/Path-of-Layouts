using fireMCG.PathOfLayouts.Core;
using fireMCG.PathOfLayouts.Layouts;
using fireMCG.PathOfLayouts.Messaging;
using UnityEngine;

namespace fireMCG.PathOfLayouts.Ui
{
    public sealed class MainMenuUiController : MonoBehaviour
    {
        public void QuickPlay()
        {
            MessageBusManager.Instance.Publish(new LoadRandomActMessage());
        }

        public void OpenLayoutBrowser()
        {
            OnAppStateChangeRequest message = new OnAppStateChangeRequest(StateController.AppState.LayoutBrowser);
            MessageBusManager.Instance.Publish(message);
        }

        public void OpenLearningCenter()
        {
            OnAppStateChangeRequest message = new OnAppStateChangeRequest(StateController.AppState.LearningCenter);
            MessageBusManager.Instance.Publish(message);
        }

        public void OpenSettings()
        {
            OnAppStateChangeRequest message = new OnAppStateChangeRequest(StateController.AppState.Settings);
            MessageBusManager.Instance.Publish(message);
        }

        public void Quit()
        {
            Application.Quit();
        }
    }
}