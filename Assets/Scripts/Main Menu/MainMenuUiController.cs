using fireMCG.PathOfLayouts.Core;
using fireMCG.PathOfLayouts.Layouts;
using fireMCG.PathOfLayouts.Messaging;
using UnityEngine.Assertions;
using UnityEngine;

namespace fireMCG.PathOfLayouts.Ui
{
    public sealed class MainMenuUiController : MonoBehaviour
    {
        [field: SerializeField] private GameObject _settingsWindow;

        private void Awake()
        {
            Assert.IsNotNull(_settingsWindow);

            SetSettingsWindowState(false);
        }

        public void QuickPlay()
        {
            MessageBusManager.Resolve.Publish(new LoadRandomActMessage());
        }

        public void OpenLayoutBrowser()
        {
            OnAppStateChangeRequest message = new OnAppStateChangeRequest(StateController.AppState.LayoutBrowser);
            MessageBusManager.Resolve.Publish(message);
        }

        public void OpenLearningCenter()
        {
            OnAppStateChangeRequest message = new OnAppStateChangeRequest(StateController.AppState.LearningCenter);
            MessageBusManager.Resolve.Publish(message);
        }

        public void ToggleSettingsWindow()
        {
            SetSettingsWindowState(!_settingsWindow.activeSelf);
        }

        public void Quit()
        {
            Application.Quit();
        }

        private void SetSettingsWindowState(bool state)
        {
            _settingsWindow.SetActive(state);
        }
    }
}