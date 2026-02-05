using System;
using UnityEngine;

namespace fireMCG.PathOfLayouts.LayoutBrowser.Ui
{
    public class LayoutCard : MonoBehaviour
    {
        private Action<string> _playCallback;
        private Action<string> _settingsCallback;
        private string _layoutId;

        public void Initialize(Action<string> playCallback, Action<string> settingsCallback, string layoutId)
        {
            _playCallback = playCallback;
            _settingsCallback = settingsCallback;
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