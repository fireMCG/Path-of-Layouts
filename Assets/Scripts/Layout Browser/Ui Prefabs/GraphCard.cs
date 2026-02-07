using NUnit.Framework;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace fireMCG.PathOfLayouts.LayoutBrowser.Ui
{
    public sealed class GraphCard : MonoBehaviour
    {
        [SerializeField] private TMP_Text _label;
        [SerializeField] private RectTransform _thumbnailBackground;
        [SerializeField] private RectTransform _thumbnailContainer;
        [SerializeField] private RawImage _thumbnailImage;

        private Action<string> _selectedCallback;
        private Action<string> _playCallback;
        private string _graphId;

        private void Awake()
        {
            Assert.IsNotNull(_label);
            Assert.IsNotNull(_thumbnailBackground);
            Assert.IsNotNull(_thumbnailContainer);
            Assert.IsNotNull(_thumbnailImage);
        }

        public void Initialize(Action<string> selectedCallback, Action<string> playCallback, string graphId, Texture2D thumbnail)
        {
            _label.text = graphId;

            float scaleX = _thumbnailBackground.sizeDelta.x / thumbnail.width;
            float scaleY = _thumbnailBackground.sizeDelta.y / thumbnail.height;
            float scale = Mathf.Min(scaleX, scaleY);
            _thumbnailContainer.sizeDelta = new Vector2(thumbnail.width * scale, thumbnail.height * scale);

            _thumbnailImage.texture = thumbnail;
            _selectedCallback = selectedCallback;
            _playCallback = playCallback;
            _graphId = graphId;
        }

        public void Select()
        {
            _selectedCallback?.Invoke(_graphId);
        }

        public void Play()
        {
            _playCallback?.Invoke(_graphId);
        }

        public void ToggleGraphSrsState()
        {

        }
    }
}