using System;
using UnityEngine;

namespace fireMCG.PathOfLayouts.LayoutBrowser.Ui
{
    public class GraphCard : MonoBehaviour
    {
        private Action<string> _selectedCallback;
        private Action<string> _playCallback;
        private string _graphId;

        public void Initialize(Action<string> selectedCallback, Action<string> playCallback, string graphId)
        {
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