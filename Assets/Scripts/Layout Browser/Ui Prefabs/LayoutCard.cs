using UnityEngine.Assertions;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using fireMCG.PathOfLayouts.Core;
using fireMCG.PathOfLayouts.Srs;

namespace fireMCG.PathOfLayouts.LayoutBrowser.Ui
{
    public sealed class LayoutCard : MonoBehaviour
    {
        [SerializeField] private TMP_Text _label;
        [SerializeField] private RectTransform _thumbnailBackground;
        [SerializeField] private RectTransform _thumbnailContainer;
        [SerializeField] private RawImage _thumbnailImage;
        [SerializeField] private GameObject _addToLearningButton;
        [SerializeField] private GameObject _removeFromLearningButton;

        private Action<string> _settingsCallback;
        private Action<string> _playCallback;
        private string _actId;
        private string _areaId;
        private string _graphId;
        private string _layoutId;
        private string _srsEntryKey;

        private bool _isLearning;

        private void Awake()
        {
            Assert.IsNotNull(_label);
            Assert.IsNotNull(_thumbnailBackground);
            Assert.IsNotNull(_thumbnailContainer);
            Assert.IsNotNull(_thumbnailImage);
            Assert.IsNotNull(_addToLearningButton);
            Assert.IsNotNull(_removeFromLearningButton);
        }

        public void Initialize(
            Action<string> settingsCallback,
            Action<string> playCallback,
            Texture2D thumbnail,
            string actId,
            string areaId,
            string graphId,
            string layoutId)
        {
            _actId = actId;
            _areaId = areaId;
            _graphId = graphId;
            _layoutId = layoutId;
            _srsEntryKey = SrsService.GetSrsEntryKey(_actId, _areaId, _graphId, _layoutId);

            _label.text = layoutId;

            float scaleX = _thumbnailBackground.sizeDelta.x / thumbnail.width;
            float scaleY = _thumbnailBackground.sizeDelta.y / thumbnail.height;
            float scale = Mathf.Min(scaleX, scaleY);
            _thumbnailContainer.sizeDelta = new Vector2(thumbnail.width * scale, thumbnail.height * scale);

            _thumbnailImage.texture = thumbnail;
            _settingsCallback = settingsCallback;
            _playCallback = playCallback;

            _isLearning = Bootstrap.Instance.SrsService.IsLearning(_srsEntryKey);
            SetSrsButtonStates();
        }

        public void Play()
        {
            _playCallback?.Invoke(_layoutId);
        }

        public void OpenSettings()
        {
            _settingsCallback?.Invoke(_layoutId);
        }

        public void AddToLearning()
        {
            if (!Bootstrap.Instance.SrsService.AddToLearning(_actId, _areaId, _graphId, _layoutId))
            {
                return;
            }

            _isLearning = true;

            SetSrsButtonStates();
        }

        public void RemoveFromLearning()
        {
            if (!Bootstrap.Instance.SrsService.RemoveFromLearning(_srsEntryKey))
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