using fireMCG.PathOfLayouts.Messaging;
using UnityEngine;
using UnityEngine.Assertions;

namespace fireMCG.PathOfLayouts.Core
{
    public sealed class StateController : MonoBehaviour
    {
        public enum AppState
        {
            Initializing,
            MainMenu,
            LayoutBrowser,
            LearningCenter,
            Gameplay
        }

        public static AppState PreviousState { get; private set; } = AppState.Initializing;
        public static AppState CurrentState { get; private set; } = AppState.Initializing;

        [field: SerializeField] private GameObject _mainMenuUiContainer;
        [field: SerializeField] private GameObject _layoutBrowserUiContainer;
        [field: SerializeField] private GameObject _learningCenterUiContainer;
        [field: SerializeField] private GameObject _gameplayUiContainer;

        private void Awake()
        {
            Assert.IsNotNull(_mainMenuUiContainer);
            Assert.IsNotNull(_layoutBrowserUiContainer);
            Assert.IsNotNull(_learningCenterUiContainer);
            Assert.IsNotNull(_gameplayUiContainer);

            RegisterMessageListeners();
        }

        private void OnDestroy()
        {
            UnregisterMessageListeners();
        }

        private void RegisterMessageListeners()
        {
            MessageBusManager.Instance.Subscribe<OnBootstrapReadyMessage>(OnBootstrapReady);
            MessageBusManager.Instance.Subscribe<OnAppStateChangeRequest>(OnAppStateChangeRequest);
        }

        private void UnregisterMessageListeners()
        {
            MessageBusManager.Instance.Unsubscribe<OnBootstrapReadyMessage>(OnBootstrapReady);
            MessageBusManager.Instance.Unsubscribe<OnAppStateChangeRequest>(OnAppStateChangeRequest);
        }

        private void OnBootstrapReady(OnBootstrapReadyMessage message)
        {
            SetState(AppState.MainMenu);
        }

        private void OnAppStateChangeRequest(OnAppStateChangeRequest message)
        {
            SetState(message.TargetState);
        }

        private void SetState(AppState newState)
        {
            if(newState == CurrentState)
            {
                return;
            }

            PreviousState = CurrentState;
            CurrentState = newState;

            _mainMenuUiContainer.SetActive(newState == AppState.MainMenu);
            _layoutBrowserUiContainer.SetActive(newState == AppState.LayoutBrowser);
            _learningCenterUiContainer.SetActive(newState == AppState.LearningCenter);
            _gameplayUiContainer.SetActive(newState == AppState.Gameplay);

            MessageBusManager.Instance.Publish(new OnAppStateChanged(PreviousState, CurrentState));
        }
    }
}