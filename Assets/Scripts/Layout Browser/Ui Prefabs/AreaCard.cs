using System;
using System.Globalization;
using TMPro;
using UnityEngine;

namespace fireMCG.PathOfLayouts.LayoutBrowser.Ui
{
    public sealed class AreaCard : MonoBehaviour
    {
        [SerializeField] private TMP_Text _label;

        private Action<string> _selectedCallback;
        private Action<string> _playCallback;
        private string _areaId;

        public void Initialize(Action<string> selectedCallback, Action<string> playCallback, string areaId)
        {
            _label.text = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(areaId);

            _selectedCallback = selectedCallback;
            _playCallback = playCallback;
            _areaId = areaId;
        }

        public void Select()
        {
            _selectedCallback?.Invoke(_areaId);
        }

        public void Play()
        {
            _playCallback?.Invoke(_areaId);
        }
    }
}