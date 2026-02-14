using fireMCG.PathOfLayouts.Messaging;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

namespace fireMCG.PathOfLayouts.Gameplay
{
    public class TimerUiController : MonoBehaviour
    {
        [SerializeField] private TMP_Text _timerText;

        private Timer _timer;

        private void Awake()
        {
            Assert.IsNotNull(_timerText);

            _timer = new Timer();
        }

        private void OnEnable()
        {
            RegisterMessageListeners();
        }

        private void OnDisable()
        {
            UnregisterMessageListeners();
        }

        private void Update()
        {
            // To do: Replace with new Input System.

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                _timer.Toggle();
            }

            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                _timer.Restart();
            }

            _timer.Tick(Time.deltaTime);
            _timerText.text = _timer.ToString();
        }

        private void RegisterMessageListeners()
        {
            MessageBusManager.Instance.Subscribe<StartTimerMessage>(OnStartTimerMessage);
            MessageBusManager.Instance.Subscribe<PauseTimerMessage>(OnPauseTimerMessage);
            MessageBusManager.Instance.Subscribe<RestartTimerMessage>(OnRestartTimerMessage);
        }

        private void UnregisterMessageListeners()
        {
            MessageBusManager.Instance.Unsubscribe<StartTimerMessage>(OnStartTimerMessage);
            MessageBusManager.Instance.Unsubscribe<PauseTimerMessage>(OnPauseTimerMessage);
            MessageBusManager.Instance.Unsubscribe<RestartTimerMessage>(OnRestartTimerMessage);
        }

        private void OnStartTimerMessage(StartTimerMessage message) => _timer.Start();

        private void OnPauseTimerMessage(PauseTimerMessage message) => _timer.Pause();

        private void OnRestartTimerMessage(RestartTimerMessage message) => _timer.Restart();
    }
}