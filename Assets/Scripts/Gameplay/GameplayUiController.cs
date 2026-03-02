using fireMCG.PathOfLayouts.Core;
using fireMCG.PathOfLayouts.Layouts;
using fireMCG.PathOfLayouts.Messaging;
using fireMCG.PathOfLayouts.Srs;
using PinePie.SimpleJoystick;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace fireMCG.PathOfLayouts.Gameplay
{
    public class GameplayUiController : MonoBehaviour
    {
        [SerializeField] private GameObject _gameplaySettings;
        [SerializeField] private TMP_InputField _movementSpeedField;
        [SerializeField] private TMP_InputField _lightRadiusField;
        [SerializeField] private Button _successButton;
        [SerializeField] private Button _failureButton;
        [SerializeField] private Button _randomReplayButton;
        [SerializeField] private TMP_Text _areaName;

        [SerializeField] private RectTransform _joystickContainer;
        [SerializeField] private JoystickController _joyController;
        [SerializeField] private Toggle _sprintToggle;
        [SerializeField] private GameObject _toggleTimerButton;
        [SerializeField] private GameObject _restartTimerButton;

        private void Awake()
        {
            Assert.IsNotNull(_gameplaySettings);
            Assert.IsNotNull(_movementSpeedField);
            Assert.IsNotNull(_lightRadiusField);
            Assert.IsNotNull(_successButton);
            Assert.IsNotNull(_failureButton);
            Assert.IsNotNull(_randomReplayButton);

            Assert.IsNotNull(_joyController);
            Assert.IsNotNull(_sprintToggle);
            Assert.IsNotNull(_toggleTimerButton);
            Assert.IsNotNull(_restartTimerButton);

#if UNITY_ANDROID
            _joyController.gameObject.SetActive(true);
            _sprintToggle.gameObject.SetActive(true);
            _toggleTimerButton.SetActive(true);
            _restartTimerButton.SetActive(true);
#else
            _joyController.gameObject.SetActive(false);
            _sprintToggle.gameObject.SetActive(false);
            _toggleTimerButton.SetActive(false);
            _restartTimerButton.SetActive(false);
#endif

            _gameplaySettings.SetActive(false);
        }

        private void OnEnable()
        {
            RegisterMessageListeners();

            int radius = PlayerPrefs.GetInt("joystickRadius");
            float handleRadius = radius * 0.75f;
            int diameter = radius * 2;
            int touchRegion = diameter * 2;

            int marginRight = PlayerPrefs.GetInt("joystickRightMargin");
            marginRight += touchRegion;

            int marginBottom = PlayerPrefs.GetInt("joystickBottomMargin");

            RectTransform joyStickTransform = _joyController.transform as RectTransform;

            _joyController.joystickBase.sizeDelta = new Vector2(diameter, diameter);
            _joyController.handle.sizeDelta = new Vector2(handleRadius, handleRadius);
            joyStickTransform.sizeDelta = new Vector2(touchRegion, touchRegion);

            joyStickTransform.anchoredPosition = new Vector2(0f, marginBottom);
            _joystickContainer.sizeDelta = new Vector2(marginRight, 0f);

            int range = PlayerPrefs.GetInt("joystickRange");
            _joyController.joystickRange = range;

            float deadzone = PlayerPrefs.GetFloat("joystickDeadzone");
            _joyController.deadZone = deadzone;
        }

        private void Start()
        {
            _movementSpeedField.text = PlayerPrefs.GetInt("movementSpeed").ToString();
            _lightRadiusField.text = PlayerPrefs.GetInt("lightRadius").ToString();
        }

        private void OnDisable()
        {
            UnregisterMessageListeners();

#if UNITY_ANDROID
            _sprintToggle.SetIsOnWithoutNotify(false);
#endif
        }

        private void RegisterMessageListeners()
        {
            MessageBusManager.Instance.Subscribe<OnLayoutLoadedMessage>(OnLayoutLoaded);
        }

        private void UnregisterMessageListeners()
        {
            MessageBusManager.Instance.Unsubscribe<OnLayoutLoadedMessage>(OnLayoutLoaded);
        }

        // To do : Update to allow using Area and Graph Srs
        private void OnLayoutLoaded(OnLayoutLoadedMessage message)
        {
            SetSrsState(Bootstrap.Instance.SrsService.IsEntryDue(message.RootId));

            _randomReplayButton.interactable = message.LayoutLoadingMethod != LayoutLoader.LayoutLoadingMethod.TargetLayout;

            _areaName.text = Bootstrap.Instance.CampaignDatabase.GetParentAreaFromLayout(message.LayoutId).displayName;
        }

        public void SetSrsState(bool enabled)
        {
            _successButton.interactable = enabled;
            _failureButton.interactable = enabled;
        }

        public void Replay()
        {
            MessageBusManager.Instance.Publish(new OnReplayLayoutMessage(false));
        }

        public void RandomReplay()
        {
            MessageBusManager.Instance.Publish(new OnReplayLayoutMessage(true));
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

            PlayerPrefs.SetInt("movementSpeed", movementSpeedPercent);
            PlayerPrefs.SetInt("lightRadius", lightRadiusPercent);
            MessageBusManager.Instance.Publish(new OnGameplaySettingsChangedMessage(movementSpeedPercent, lightRadiusPercent));
        }

        public void Quit()
        {
            MessageBusManager.Instance.Publish(new OnAppStateChangeRequest(StateController.PreviousState));
        }

        public void RecordSrsSuccess()
        {
            MessageBusManager.Instance.Publish(new RecordSrsResultMessage(SrsPracticeResult.Success));

            SetSrsState(false);
        }

        public void RecordSrsFailure()
        {
            MessageBusManager.Instance.Publish(new RecordSrsResultMessage(SrsPracticeResult.Failure));

            SetSrsState(false);
        }
    }
}