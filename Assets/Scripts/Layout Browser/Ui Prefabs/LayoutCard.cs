using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace fireMCG.PathOfLayouts.LayoutBrowser.Ui
{
    public sealed class LayoutCard : MonoBehaviour
    {
        [SerializeField] private TMP_Text _label;
        [SerializeField] private RectTransform _thumbnailBackground;
        [SerializeField] private RectTransform _thumbnailContainer;
        [SerializeField] private RawImage _thumbnailImage;

        private Action<string> _settingsCallback;
        private Action<string> _playCallback;
        private string _layoutId;

        public void Initialize(Action<string> settingsCallback, Action<string> playCallback, string layoutId, Texture2D thumbnail)
        {
            _label.text = layoutId;

            float scaleX = _thumbnailBackground.sizeDelta.x / thumbnail.width;
            float scaleY = _thumbnailBackground.sizeDelta.y / thumbnail.height;
            float scale = Mathf.Min(scaleX, scaleY);
            _thumbnailContainer.sizeDelta = new Vector2(thumbnail.width * scale, thumbnail.height * scale);

            _thumbnailImage.texture = thumbnail;
            _settingsCallback = settingsCallback;
            _playCallback = playCallback;
            _layoutId = layoutId;
        }

        public void Play()
        {
            _playCallback?.Invoke(_layoutId);
        }

        public void OpenSettings()
        {
            _settingsCallback?.Invoke(_layoutId);
        }

        public void ToggleLayoutSrsState()
        {

        }
    }
}