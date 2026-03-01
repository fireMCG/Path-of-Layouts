using fireMCG.PathOfLayouts.Core;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace fireMCG.PathOfLayouts.LayoutBrowser.Ui
{
    public sealed class GraphCard : MonoBehaviour
    {
        [SerializeField] private TMP_Text _label;
        [SerializeField] private RectTransform _thumbnailBackground;
        [SerializeField] private RectTransform _thumbnailContainer;
        [SerializeField] private RawImage _thumbnailImage;
        [SerializeField] private GameObject _addToLearningButton;
        [SerializeField] private GameObject _removeFromLearningButton;

        private Action<string> _selectedCallback;
        private Action<string> _playCallback;
        private string _graphId;

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

        public void Initialize(Action<string> selectedCallback, Action<string> playCallback, string graphId, string displayName)
        {
            _label.text = displayName;

            _thumbnailImage.texture = null;

            _selectedCallback = selectedCallback;
            _playCallback = playCallback;
            _graphId = graphId;

            _isLearning = Bootstrap.Instance.SrsService.IsLearning(_graphId);

            SetSrsButtonStates();
        }

        public void SetThumbnail(Texture2D thumbnail)
        {
            if(thumbnail == null)
            {
                _thumbnailImage.texture = null;

                return;
            }

            float scaleX = _thumbnailBackground.sizeDelta.x / thumbnail.width;
            float scaleY = _thumbnailBackground.sizeDelta.y / thumbnail.height;
            float scale = Mathf.Min(scaleX, scaleY);

            _thumbnailContainer.sizeDelta = new Vector2(thumbnail.width * scale, thumbnail.height * scale);
            _thumbnailImage.texture = thumbnail;
        }

        public void Select()
        {
            _selectedCallback?.Invoke(_graphId);
        }

        public void Play()
        {
            _playCallback?.Invoke(_graphId);
        }

        public void AddToLearning()
        {
            if (!Bootstrap.Instance.SrsService.AddToLearning(_graphId, Srs.SrsDataType.Graph))
            {
                return;
            }

            _isLearning = true;

            SetSrsButtonStates();
        }

        public void RemoveFromLearning()
        {
            if (!Bootstrap.Instance.SrsService.RemoveFromLearning(_graphId))
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