using System;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace fireMCG.PathOfLayouts.Srs
{
    public class SrsEntryButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private TMP_Text _label;

        private void Awake()
        {
            Assert.IsNotNull(_button);
            Assert.IsNotNull(_label);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveAllListeners();
        }

        public void SetLabel(string label)
        {
            _label.text = label;
        }

        public void SetOnClickListener(Action<string> callback, string srsEntryKey)
        {
            _button.onClick.AddListener(() => callback?.Invoke(srsEntryKey));
        }
    }
}