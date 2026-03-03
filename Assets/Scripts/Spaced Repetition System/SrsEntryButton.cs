using System;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace fireMCG.PathOfLayouts.Srs
{
    public class SrsEntryButton : MonoBehaviour
    {
        [SerializeField] private Button _selectButton;
        [SerializeField] private TMP_Text _label;

        private void Awake()
        {
            Assert.IsNotNull(_selectButton);
            Assert.IsNotNull(_label);
        }

        private void OnDestroy()
        {
            _selectButton.onClick.RemoveAllListeners();
        }

        public void Initialize(Action<string> onSelect, string entryId, string displayName)
        {
            _label.text = displayName;
            _selectButton.onClick.AddListener(() => onSelect?.Invoke(entryId));
        }
    }
}