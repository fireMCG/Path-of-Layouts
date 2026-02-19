using fireMCG.PathOfLayouts.Campaign.Common;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace fireMCG.PathOfLayouts.NodeEditor
{
    public enum DragPhase
    {
        Begin,
        Drag,
        End
    }

    public sealed class NodeMarkerPrefab : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private Button _button;
        [SerializeField] private Image _image;

        public NavigationNode Node { get; private set; }
        public RectTransform RectTransform { get; private set; }

        private Action<NodeMarkerPrefab, PointerEventData, DragPhase> _dragged;

        private void Awake()
        {
            RectTransform = GetComponent<RectTransform>();
        }

        public void Initialize(
            NavigationNode node,
            Action<NodeMarkerPrefab> clickCallback,
            Action<NodeMarkerPrefab, PointerEventData, DragPhase> dragCallback,
            Color colour)
        {
            Node = node;

            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(() => clickCallback?.Invoke(this));

            _dragged = dragCallback;

            _image.color = colour;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {

        }

        public void OnDrag(PointerEventData eventData)
        {
            _dragged?.Invoke(this, eventData, DragPhase.Drag);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _dragged?.Invoke(this, eventData, DragPhase.End);
        }

        public void SetColor(Color colour)
        {
            _image.color = colour;
        }
    }
}