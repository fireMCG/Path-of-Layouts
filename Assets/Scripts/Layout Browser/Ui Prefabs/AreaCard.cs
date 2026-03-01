using fireMCG.PathOfLayouts.Core;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

namespace fireMCG.PathOfLayouts.LayoutBrowser.Ui
{
    public sealed class AreaCard : MonoBehaviour
    {
        [SerializeField] private TMP_Text _label;
        [SerializeField] private GameObject _addToLearningButton;
        [SerializeField] private GameObject _removeFromLearningButton;

        private Action<string> _selectedCallback;
        private Action<string> _playCallback;
        private string _areaId;

        private bool _isLearning;

        private void Awake()
        {
            Assert.IsNotNull(_label);
            Assert.IsNotNull(_addToLearningButton);
            Assert.IsNotNull(_removeFromLearningButton);
        }

        public void Initialize(Action<string> selectedCallback, Action<string> playCallback, string areaId, string displayName)
        {
            _label.text = displayName;

            _selectedCallback = selectedCallback;
            _playCallback = playCallback;
            _areaId = areaId;

            _isLearning = Bootstrap.Instance.SrsService.IsLearning(_areaId);

            SetSrsButtonStates();
        }

        public void Select()
        {
            _selectedCallback?.Invoke(_areaId);
        }

        public void Play()
        {
            _playCallback?.Invoke(_areaId);
        }

        public void AddToLearning()
        {
            if (!Bootstrap.Instance.SrsService.AddToLearning(_areaId, Srs.SrsDataType.Area))
            {
                return;
            }

            _isLearning = true;

            SetSrsButtonStates();
        }

        public void RemoveFromLearning()
        {
            if (!Bootstrap.Instance.SrsService.RemoveFromLearning(_areaId))
            {
                return;
            }

            _isLearning = false;

            SetSrsButtonStates();
        }

        private void SetSrsButtonStates()
        {
            _addToLearningButton.SetActive(!_isLearning);
            _removeFromLearningButton.SetActive(_isLearning);
        }
    }
}