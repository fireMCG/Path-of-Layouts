using fireMCG.PathOfLayouts.Core;
using fireMCG.PathOfLayouts.Messaging;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

namespace fireMCG.PathOfLayouts.Gameplay
{
    public class GameplayUiController : MonoBehaviour
    {
        [SerializeField] private GameplayController _gameplayController;
        [SerializeField] private GameObject _gameplaySettings;

        [SerializeField] private TMP_InputField _movementSpeedField;
        [SerializeField] private TMP_InputField _lightRadiusField;

        private void Awake()
        {
            Assert.IsNotNull(_gameplayController);
            Assert.IsNotNull(_gameplaySettings);
            Assert.IsNotNull(_movementSpeedField);
            Assert.IsNotNull(_lightRadiusField);

            _gameplaySettings.SetActive(false);
        }

        private void Start()
        {
            // Load gameplay settings save file

            // _movementSpeedField.text = "";
            // _lightRadiusField.text = "";
        }

        public void Replay()
        {
            _gameplayController.Replay();
        }

        public void ToggleGameplaySettings()
        {
            _gameplaySettings.SetActive(!_gameplaySettings.activeSelf);
        }

        public void ApplySettings()
        {
            if(!int.TryParse(_movementSpeedField.text, out int movementSpeedPercent))
            {
                Debug.LogError("GameplayUiContrller.ApplySettings error, parsing failed.");
            }

            if (!int.TryParse(_lightRadiusField.text, out int lightRadiusPercent))
            {
                Debug.LogError("GameplayUiContrller.ApplySettings error, parsing failed.");
            }

            // Save to file

            _gameplayController.SetSettings(movementSpeedPercent, lightRadiusPercent);
        }

        public void Quit()
        {
            MessageBusManager.Resolve.Publish(new OnAppStateChangeRequest(StateController.AppState.LayoutBrowser));
        }
    }
}