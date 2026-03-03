using fireMCG.PathOfLayouts.Common;
using fireMCG.PathOfLayouts.Core;
using fireMCG.PathOfLayouts.Layouts;
using fireMCG.PathOfLayouts.Messaging;
using fireMCG.PathOfLayouts.Ui.Components;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace fireMCG.PathOfLayouts.Srs
{
    /// <summary>
    /// To do: Split views into their own scripts and make this script the navigation controller.
    /// To do: Replace instantiation and destroy logic with an object pool.
    /// To do: Implement limits in the views (other than overview which already has it) to prevent overflow
    ///     and implement pagination.
    /// To do: Change streak label between "Success/Failure Streak" based on the value of data.lastResult.
    /// </summary>
    public sealed class SrsUiController : MonoBehaviour
    {
        private const int MAX_OVERVIEW_CONTAINER_ENTRIES = 13;
        private const string DEFAULT_SELECTION_NAME = "Selection Statistics";

        [SerializeField] private GameObject _overviewView;
        [SerializeField] private RectTransform _overviewDueContainer;
        [SerializeField] private RectTransform _overviewUpcomingContainer;
        [SerializeField] private RectTransform _overviewLowSuccessContainer;
        [SerializeField] private SrsEntryButton _entryButtonPrefab;

        [SerializeField] private GameObject _dueView;
        [SerializeField] private RectTransform _dueContainer;

        [SerializeField] private GameObject _upcomingView;
        [SerializeField] private RectTransform _upcomingContainer;

        [SerializeField] private GameObject _disabledView;
        [SerializeField] private RectTransform _disabledContainer;

        [SerializeField] private Toggle _showSelectionNameToggle;
        [SerializeField] private TMP_Text _selectionName;
        [SerializeField] private TMP_Text _entryTypeText;
        [SerializeField] private TMP_Text _levelText;
        [SerializeField] private TMP_Text _practicedText;
        [SerializeField] private TMP_Text _succeededText;
        [SerializeField] private TMP_Text _failedText;
        [SerializeField] private TMP_Text _successRateText;
        [SerializeField] private TMP_Text _averageTimeText;
        [SerializeField] private TMP_Text _bestTimeText;
        [SerializeField] private TMP_Text _nextPracticeText;
        [SerializeField] private TMP_Text _streakText;

        [SerializeField] private TMP_Text _toggleLearningText;
        [SerializeField] private Image _toggleLearningImage;
        [SerializeField] private Button _toggleLearningButton;

        [SerializeField] private RectTransform _ratioSlidersContainer;
        [SerializeField] private RatioSlider _ratioSliderPrefab;

        private string _selectedEntryId = string.Empty;

        private static (string label, TimeSpan timeSpan, Color color)[] _dueWithinElements =
        {
            ("6 Hours", TimeSpan.FromHours(6), new Color(0.63f, 0f, 0f)),
            ("1 Day", TimeSpan.FromDays(1), new Color(0.7f, 0.35f, 0f)),
            ("3 Days", TimeSpan.FromDays(3), new Color(0.6f, 0.6f, 0f)),
            ("1 Week", TimeSpan.FromDays(7), new Color(0f, 0.63f, 0f)),
            ("2 Weeks", TimeSpan.FromDays(14), new Color(0f, 0.6f, 0.6f)),
            ("1 Month", TimeSpan.FromDays(30), new Color(0f, 0f, 0.63f))
        };

        private void Awake()
        {
            Assert.IsNotNull(_overviewDueContainer);
            Assert.IsNotNull(_overviewUpcomingContainer);
            Assert.IsNotNull(_overviewLowSuccessContainer);
            Assert.IsNotNull(_entryButtonPrefab);

            Assert.IsNotNull(_showSelectionNameToggle);
            Assert.IsNotNull(_selectionName);
            Assert.IsNotNull(_entryTypeText);
            Assert.IsNotNull(_levelText);
            Assert.IsNotNull(_practicedText);
            Assert.IsNotNull(_succeededText);
            Assert.IsNotNull(_failedText);
            Assert.IsNotNull(_successRateText);
            Assert.IsNotNull(_averageTimeText);
            Assert.IsNotNull(_bestTimeText);
            Assert.IsNotNull(_nextPracticeText);
            Assert.IsNotNull(_streakText);

            Assert.IsNotNull(_toggleLearningText);
            Assert.IsNotNull(_toggleLearningImage);
            Assert.IsNotNull(_toggleLearningButton);

            Assert.IsNotNull(_ratioSlidersContainer);
            Assert.IsNotNull(_ratioSliderPrefab);
        }

        private void OnEnable()
        {
            RegisterMessageListeners();
        }

        private void OnDisable()
        {
            UnregisterMessageListeners();
        }

        private void RegisterMessageListeners()
        {
            MessageBusManager.Instance.Subscribe<OnAppStateChanged>(UpdateOverview);
        }

        private void UnregisterMessageListeners()
        {
            MessageBusManager.Instance.Unsubscribe<OnAppStateChanged>(UpdateOverview);
        }

        public void QuitToMainMenu()
        {
            MessageBusManager.Instance.Publish(new OnAppStateChangeRequest(StateController.AppState.MainMenu));
        }

        private void UpdateOverview(OnAppStateChanged message)
        {
            if(message.NewState != StateController.AppState.LearningCenter)
            {
                return;
            }

            ShowOverview();
        }

        public void ShowOverview()
        {
            ClearStatisticsUi();

            _overviewView.SetActive(true);
            _dueView.SetActive(false);
            _upcomingView.SetActive(false);
            _disabledView.SetActive(false);

            FillViewContainer(_overviewDueContainer, Bootstrap.Instance.SrsService.GetDueEntries(MAX_OVERVIEW_CONTAINER_ENTRIES));
            FillViewContainer(_overviewUpcomingContainer, Bootstrap.Instance.SrsService.GetNextDueEntries(MAX_OVERVIEW_CONTAINER_ENTRIES));
            FillViewContainer(_overviewLowSuccessContainer, Bootstrap.Instance.SrsService.GetLowSuccessEntries(MAX_OVERVIEW_CONTAINER_ENTRIES));
            FillDueWithinStatistics();
        }

        public void ShowDue()
        {
            ClearStatisticsUi();

            _overviewView.SetActive(false);
            _dueView.SetActive(true);
            _upcomingView.SetActive(false);
            _disabledView.SetActive(false);

            FillViewContainer(_dueContainer, Bootstrap.Instance.SrsService.GetDueEntries());
        }

        public void ShowUpcoming()
        {
            ClearStatisticsUi();

            _overviewView.SetActive(false);
            _dueView.SetActive(false);
            _upcomingView.SetActive(true);
            _disabledView.SetActive(false);

            FillViewContainer(_upcomingContainer, Bootstrap.Instance.SrsService.GetNextDueEntries());
        }

        public void ShowDisabled()
        {
            ClearStatisticsUi();

            _overviewView.SetActive(false);
            _dueView.SetActive(false);
            _upcomingView.SetActive(false);
            _disabledView.SetActive(true);

            FillViewContainer(_disabledContainer, Bootstrap.Instance.SrsService.GetDisabledEntries());
        }

        private void FillViewContainer(RectTransform container, IReadOnlyList<SrsEntryData> entries)
        {
            foreach(Transform transform in container)
            {
                Destroy(transform.gameObject);
            }

            foreach(SrsEntryData entry in entries)
            {
                SrsEntryButton button = Instantiate(_entryButtonPrefab, container);
                button.Initialize(
                    OnSelectEntry,
                    OnPlayEntry,
                    entry.id,
                    GetEntryShortName(entry.id));
            }
        }

        private void FillDueWithinStatistics()
        {
            foreach (Transform transform in _ratioSlidersContainer)
            {
                Destroy(transform.gameObject);
            }

            int totalDueLayouts = 0;
            int[] dueLayoutsAmount = new int[_dueWithinElements.Length];
            for(int i = 0; i < _dueWithinElements.Length; i++)
            {
                DateTime dueAfter = i > 0 ? DateTime.UtcNow.Add(_dueWithinElements[i - 1].timeSpan) : DateTime.MinValue;
                int tempValue = Bootstrap.Instance.SrsService.GetEntriesDueWithin(dueAfter, _dueWithinElements[i].timeSpan);
                dueLayoutsAmount[i] = tempValue;
                totalDueLayouts += tempValue;
            }

            List<(int dueAmount, Color color)> elements = new();
            for(int i = 0; i < dueLayoutsAmount.Length; i++)
            {
                elements.Add((dueLayoutsAmount[i], _dueWithinElements[i].color));
                RatioSlider ratioSlider = Instantiate(_ratioSliderPrefab, _ratioSlidersContainer);
                ratioSlider.Initialize(_dueWithinElements[i].label, elements.ToArray(), totalDueLayouts);
            }
        }

        private void OnSelectEntry(string entryId)
        {
            _selectedEntryId = entryId;
            SrsEntryData data = Bootstrap.Instance.SrsService.SaveData.entries[_selectedEntryId];

            UpdateSelectionName();

            _entryTypeText.text = Enum.GetName(typeof(SrsDataType), data.dataType);
            _levelText.text = data.masteryLevel.ToString();
            _practicedText.text = data.timesPracticed.ToString();
            _succeededText.text = data.timesSucceeded.ToString();
            _failedText.text = data.timesFailed.ToString();
            _successRateText.text = ((float)data.timesSucceeded / data.timesPracticed * 100).ToString("F0") + "%";

            _averageTimeText.text = TimeFormatter.FormatTimeExplicit(data.averageTimeSeconds);
            _bestTimeText.text = TimeFormatter.FormatTimeExplicit(data.bestTimeSeconds);

            _nextPracticeText.text = data.GetTimeStringUntilDue(DateTime.UtcNow);

            _streakText.text = data.streak.ToString();

            SetEntryLearningStateUi();
        }

        public void UpdateSelectionNameVisibility()
        {
            UpdateSelectionName();
        }

        private void UpdateSelectionName()
        {
            _selectionName.text = GetEntryShortName(_selectedEntryId);
        }

        private string GetEntryShortName(string entryId)
        {
            SrsEntryData data = Bootstrap.Instance.SrsService.SaveData.entries[entryId];
            switch ((SrsDataType)data.dataType)
            {
                case SrsDataType.Area:
                    return Bootstrap.Instance.CampaignDatabase.GetArea(entryId).displayName;
                case SrsDataType.Graph:
                    return _showSelectionNameToggle.isOn ?
                        Bootstrap.Instance.CampaignDatabase.GetGraph(entryId).displayName :
                        Bootstrap.Instance.CampaignDatabase.GetParentArea(entryId).displayName;
                case SrsDataType.Layout:
                    return _showSelectionNameToggle.isOn ?
                        Bootstrap.Instance.CampaignDatabase.GetLayout(entryId).displayName :
                        Bootstrap.Instance.CampaignDatabase.GetParentAreaFromLayout(entryId).displayName;
                default:
                    return "Error";
            }
        }

        public void ToggleEntryLearningState()
        {
            if (!Bootstrap.Instance.SrsService.ToggleLearningState(_selectedEntryId))
            {
                ClearLearningStateUi();

                return;
            }

            
            SetEntryLearningStateUi();
        }

        private void SetEntryLearningStateUi()
        {
            SrsEntryData data = Bootstrap.Instance.SrsService.SaveData.entries[_selectedEntryId];
            if (data is null)
            {
                ClearLearningStateUi();

                return;
            }

            _toggleLearningButton.interactable = true;
            _toggleLearningImage.color = data.isLearning ? new Color(0.63f, 0f, 0f) : new Color(0f, 0.63f, 0f);
            _toggleLearningText.text = data.isLearning ? "Disable" : "Enable";
        }

        private void ClearLearningStateUi()
        {
            _toggleLearningImage.color = Color.white;
            _toggleLearningButton.interactable = false;
            _toggleLearningText.text = "N/A";
        }

        private void OnPlayEntry(string entryId)
        {
            SrsEntryData data = Bootstrap.Instance.SrsService.SaveData.entries[entryId];

            switch ((SrsDataType)data.dataType)
            {
                case SrsDataType.Area:
                    MessageBusManager.Instance.Publish(new LoadRandomGraphMessage(data.id));
                    break;
                case SrsDataType.Graph:
                    MessageBusManager.Instance.Publish(new LoadRandomLayoutMessage(data.id));
                    break;
                case SrsDataType.Layout:
                    MessageBusManager.Instance.Publish(new LoadTargetLayoutMessage(data.id));
                    break;
                default:
                    throw new Exception($"Error, invalid Srs Entry Type, type={(SrsDataType)data.dataType}");
            }
        }

        private void ClearStatisticsUi()
        {
            _selectedEntryId = string.Empty;
            ClearLearningStateUi();

            _selectionName.text = DEFAULT_SELECTION_NAME;
            _entryTypeText.text = "None";
            _levelText.text = "N/A";
            _practicedText.text = "N/A";
            _succeededText.text = "N/A";
            _failedText.text = "N/A";
            _successRateText.text = "N/A";
            _averageTimeText.text = "N/A";
            _bestTimeText.text = "N/A";
            _nextPracticeText.text = "N/A";
            _streakText.text = "N/A";
        }
    }
}