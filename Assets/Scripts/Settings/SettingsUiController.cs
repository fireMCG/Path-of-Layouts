using fireMCG.PathOfLayouts.Core;
using fireMCG.PathOfLayouts.Messaging;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

namespace fireMCG.PathOfLayouts.Settings
{
    public sealed class SettingsUiController : MonoBehaviour
    {
        [SerializeField] private GameObject _mobileControlsContainer;
        [SerializeField] private TMP_InputField _joystickRadiusField;
        [SerializeField] private TMP_InputField _joystickRangeField;
        [SerializeField] private TMP_InputField _joystickDeadzoneField;
        [SerializeField] private TMP_InputField _joystickRightMarginField;
        [SerializeField] private TMP_InputField _joystickBottomMarginField;

        private void Awake()
        {
#if UNITY_ANDROID
            Assert.IsNotNull(_mobileControlsContainer);
            Assert.IsNotNull(_joystickRadiusField);
            Assert.IsNotNull(_joystickRangeField);
            Assert.IsNotNull(_joystickDeadzoneField);
            Assert.IsNotNull(_joystickRightMarginField);
            Assert.IsNotNull(_joystickBottomMarginField);

            _mobileControlsContainer.SetActive(true);
#else
            Assert.IsNotNull(_mobileControlsContainer);
            _mobileControlsContainer.SetActive(false);
#endif
        }

        private void OnEnable()
        {
#if UNITY_ANDROID
            _joystickRadiusField.SetTextWithoutNotify(PlayerPrefs.GetInt("joystickRadius").ToString());
            _joystickRangeField.SetTextWithoutNotify(PlayerPrefs.GetInt("joystickRange").ToString());
            _joystickDeadzoneField.SetTextWithoutNotify(PlayerPrefs.GetFloat("joystickDeadzone").ToString());
            _joystickRightMarginField.SetTextWithoutNotify(PlayerPrefs.GetInt("joystickRightMargin").ToString());
            _joystickBottomMarginField.SetTextWithoutNotify(PlayerPrefs.GetInt("joystickBottomMargin").ToString());
#endif
        }

        public void Apply()
        {
#if UNITY_ANDROID
            if (int.TryParse(_joystickRadiusField.text, out int radiusInt))
            {
                radiusInt = Mathf.Clamp(radiusInt, 32, 512);
                _joystickRadiusField.SetTextWithoutNotify(radiusInt.ToString());

                PlayerPrefs.SetInt("joystickRadius", radiusInt);
            }

            if (int.TryParse(_joystickRangeField.text, out int rangeInt))
            {
                rangeInt = Mathf.Clamp(rangeInt, 32, 512);
                _joystickRangeField.SetTextWithoutNotify(rangeInt.ToString());

                PlayerPrefs.SetInt("joystickRange", rangeInt);
            }

            if (float.TryParse(_joystickDeadzoneField.text, out float deadzoneFloat))
            {
                deadzoneFloat = Mathf.Clamp(deadzoneFloat, 0f, 0.8f);
                _joystickDeadzoneField.SetTextWithoutNotify(deadzoneFloat.ToString());

                PlayerPrefs.SetFloat("joystickDeadzone", deadzoneFloat);
            }

            if (int.TryParse(_joystickRightMarginField.text, out int marginRightInt))
            {
                marginRightInt = Mathf.Clamp(marginRightInt, 128, 1024);
                _joystickRightMarginField.SetTextWithoutNotify(marginRightInt.ToString());

                PlayerPrefs.SetInt("joystickRightMargin", marginRightInt);
            }

            if (int.TryParse(_joystickBottomMarginField.text, out int marginBottomInt))
            {
                marginBottomInt = Mathf.Clamp(marginBottomInt, 32, 1024);
                _joystickBottomMarginField.SetTextWithoutNotify(marginBottomInt.ToString());

                PlayerPrefs.SetInt("joystickBottomMargin", marginBottomInt);
            }
#endif
        }

        public void ReturnToMainMenu()
        {
            MessageBusManager.Instance.Publish(new OnAppStateChangeRequest(StateController.AppState.MainMenu));
        }
    }
}