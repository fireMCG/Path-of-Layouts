using fireMCG.PathOfLayouts.Campaign;
using fireMCG.PathOfLayouts.Campaign.Common;
using fireMCG.PathOfLayouts.Core;
using fireMCG.PathOfLayouts.Messaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace fireMCG.PathOfLayouts.NodeEditor
{
    public sealed class NodeEditorUiController : MonoBehaviour, IPointerClickHandler
    {
        [Header("Layout")]
        [SerializeField] private RawImage _layoutImage;
        [SerializeField] private RectTransform _layoutRect;
        [SerializeField] private RectTransform _markerRoot;

        [Header("Viewport Clamp")]
        [SerializeField] private float _maxLayoutWidth = 1024f;
        [SerializeField] private float _maxLayoutHeight = 1024f;

        [Header("Marker Prefab")]
        [SerializeField] private NodeMarkerPrefab _markerPrefab;
        [SerializeField] private float _markerSize = 32f;

        [Header("Selected Node UI")]
        [SerializeField] private TMP_InputField _nodeNameInput;
        [SerializeField] private TMP_Dropdown _nodeTypeDropdown;
        [SerializeField] private TMP_InputField _linkedAreaIdInput;

        private readonly List<NodeMarkerPrefab> _markers = new();
        private readonly Dictionary<NavigationNode, NodeMarkerPrefab> _nodeToMarker = new();

        private CancellationTokenSource _cancellationTokenSource;

        private LayoutDef _layout;
        private NavigationDataAsset _navData;
        private Texture2D _layoutTexture;

        private NavigationNode _selectedNode;

        private Vector2 _displaySize;
        private Canvas _rootCanvas;
        private Camera _uiCamera;

        private void Awake()
        {
            RegisterMessageListeners();

            List<string> options = new();

            foreach(NodeType nodeType in Enum.GetValues(typeof(NodeType)))
            {
                options.Add(nodeType.ToString());
            }

            _nodeTypeDropdown.ClearOptions();
            _nodeTypeDropdown.AddOptions(options);
        }

        private void OnDestroy()
        {
            ClearTokenSource();

            UnregisterMessageListeners();
        }

        private void ClearTokenSource()
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;
            }
        }

        private void RegisterMessageListeners()
        {
            UnregisterMessageListeners();

            MessageBusManager.Instance.Subscribe<NodeEditorOpenMessage>(OnNodeEditorOpenMessage);
            MessageBusManager.Instance.Subscribe<OnAppStateChanged>(OnAppStateChanged);
        }

        private void UnregisterMessageListeners()
        {
            MessageBusManager.Instance.Unsubscribe<NodeEditorOpenMessage>(OnNodeEditorOpenMessage);
            MessageBusManager.Instance.Unsubscribe<OnAppStateChanged>(OnAppStateChanged);
        }
        
        private async void OnNodeEditorOpenMessage(NodeEditorOpenMessage message)
        {
            ClearTokenSource();

            _cancellationTokenSource = new();

            await LoadAsync(message.LayoutId, _cancellationTokenSource.Token);

            MessageBusManager.Instance.Publish(new OnAppStateChangeRequest(StateController.AppState.NodeEditor));
        }

        private void OnAppStateChanged(OnAppStateChanged message)
        {
            if(message.NewState != StateController.AppState.NodeEditor)
            {
                return;
            }

            RebuildMarkers();
            RefreshSelectedUi();
        }

        private async Task LoadAsync(string layoutId, CancellationToken cancellationToken)
        {
            _layout = Bootstrap.Instance.CampaignDatabase.GetLayout(layoutId);

            cancellationToken.ThrowIfCancellationRequested();

            _layoutTexture = await Bootstrap.Instance.ContentService.LoadLayoutImageAsync(layoutId, cancellationToken);
            _layoutImage.texture = _layoutTexture;

            _layoutRect.sizeDelta = new Vector2(_layoutTexture.width, _layoutTexture.height);
            _markerRoot.sizeDelta = _layoutRect.sizeDelta;

            cancellationToken.ThrowIfCancellationRequested();

            ApplyClampedDisplaySize(_layoutTexture.width, _layoutTexture.height);

            LayoutRebuilder.ForceRebuildLayoutImmediate(_layoutRect);

            EnsureNavData();

            _selectedNode = null;

            cancellationToken.ThrowIfCancellationRequested();
        }

        private void ApplyClampedDisplaySize(int textureWidth, int textureHeight)
        {
            float w = (float)textureWidth;
            float h = (float)textureHeight;

            float scaleX = _maxLayoutWidth / w;
            float scaleY = _maxLayoutHeight / h;
            float scale = Mathf.Min(scaleX, scaleY, 1f);

            float displayW = w * scale;
            float displayH = h * scale;

            _displaySize = new Vector2(displayW, displayH);

            _layoutRect.sizeDelta = _displaySize;
            _markerRoot.sizeDelta = _displaySize;
        }

        public void Save()
        {
#if UNITY_EDITOR
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif
        }

        private void MarkDirty()
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(_navData);
            EditorUtility.SetDirty(_layout);
#endif
        }

        private void EnsureNavData()
        {
            if (_layout.navigationData != null)
            {
                _navData = _layout.navigationData;

                return;
            }

            NavigationDataAsset created = ScriptableObject.CreateInstance<NavigationDataAsset>();
            created.name = "NavData_" + _layout.id;

            created.width = _layoutTexture.width;
            created.height = _layoutTexture.height;

            _layout.navigationData = created;

#if UNITY_EDITOR
            string layoutPath = AssetDatabase.GetAssetPath(_layout);
            string assetName = created.name + ".asset";
            string targetPath = Path.Combine(Path.GetDirectoryName(layoutPath), assetName).Replace("\\", "/");

            AssetDatabase.CreateAsset(created, targetPath);

            EditorUtility.SetDirty(created);
            EditorUtility.SetDirty(_layout);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif

            _navData = created;
        }

        private void RebuildMarkers()
        {
            foreach (Transform marker in _markerRoot)
            {
                Destroy(marker.gameObject);
            }

            _markers.Clear();

            if (_navData == null || _navData.nodes is null)
            {
                return;
            }

            for (int i = 0; i < _navData.nodes.Count; i++)
            {
                NavigationNode node = _navData.nodes[i];

                NodeMarkerPrefab marker = CreateMarker(node);
                _markers.Add(marker);
            }
        }

        #region Pointer Events
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData is null ||
                eventData.pointerPressRaycast.gameObject == null ||
                eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }

            Vector2 normalized = ScreenToNormalized(eventData.position);

            if (normalized.x < 0f || normalized.x > 1f || normalized.y < 0f || normalized.y > 1f)
            {
                return;
            }

            NavigationNode node = new NavigationNode(normalized);
            node.displayName = $"Node {_navData.nodes.Count}";

            _navData.nodes.Add(node);
            MarkDirty();

            NodeMarkerPrefab marker = CreateMarker(node);
            _markers.Add(marker);

            _selectedNode = node;

            RefreshSelectedUi();
        }

        private void OnMarkerClicked(NodeMarkerPrefab marker)
        {
            _selectedNode = marker.Node;

            RefreshSelectedUi();
        }

        private void OnMarkerDragged(NodeMarkerPrefab marker, PointerEventData eventData, DragPhase phase)
        {
            Vector2 normalized = ScreenToNormalized(eventData.position);

            normalized.x = Mathf.Clamp01(normalized.x);
            normalized.y = Mathf.Clamp01(normalized.y);

            marker.Node.normalizedPosition = normalized;
            marker.RectTransform.anchoredPosition = NormalizedToAnchored(normalized);

            if (phase == DragPhase.End)
            {
                MarkDirty();
            }
        }
        #endregion

        #region SelectedUI
        public void OnNodeNameChanged(string value)
        {
            if (_selectedNode is null)
            {
                return;
            }

            _selectedNode.displayName = value;

            MarkDirty();
        }

        public void OnNodeTypeChanged(int value)
        {
            if (_selectedNode is null)
            {
                return;
            }

            _selectedNode.nodeType = (NodeType)value;
            _nodeToMarker[_selectedNode].SetColor(GetColorForNode(_selectedNode.nodeType));

            MarkDirty();

            RefreshSelectedUi();
        }

        public void OnLinkedAreaIdChanged(string value)
        {
            if (_selectedNode is null)
            {
                return;
            }

            MarkDirty();

            _selectedNode.linkedAreaId = value;
        }

        public void RemoveSelectedNode()
        {
            if (_selectedNode is null)
            {
                return;
            }

            _nodeToMarker.Remove(_selectedNode);

            int removeIndex = _navData.nodes.IndexOf(_selectedNode);
            if (removeIndex >= 0)
            {
                _navData.nodes.RemoveAt(removeIndex);
            }

            MarkDirty();

            for (int i = 0; i < _markers.Count; i++)
            {
                NodeMarkerPrefab marker = _markers[i];

                if (marker == null)
                {
                    continue;
                }

                if (marker.Node == _selectedNode)
                {
                    _markers.RemoveAt(i);
                    Destroy(marker.gameObject);

                    break;
                }
            }

            _selectedNode = null;

            RefreshSelectedUi();
        }

        private void RefreshSelectedUi()
        {
            if (_selectedNode is null)
            {
                _nodeNameInput.SetTextWithoutNotify(string.Empty);

                _nodeTypeDropdown.SetValueWithoutNotify(0);

                _linkedAreaIdInput.SetTextWithoutNotify(string.Empty);
                _linkedAreaIdInput.gameObject.SetActive(false);

                return;
            }

            _nodeNameInput.SetTextWithoutNotify(_selectedNode.displayName);

            _nodeTypeDropdown.SetValueWithoutNotify((int)_selectedNode.nodeType);

            bool isEntrance = _selectedNode.nodeType == NodeType.Entrance;
            _linkedAreaIdInput.gameObject.SetActive(isEntrance);
            _linkedAreaIdInput.SetTextWithoutNotify(_selectedNode.linkedAreaId);
        }
        #endregion

        #region Helpers
        private NodeMarkerPrefab CreateMarker(NavigationNode node)
        {
            NodeMarkerPrefab marker = Instantiate(_markerPrefab, _markerRoot);

            marker.RectTransform.sizeDelta = new Vector2(_markerSize, _markerSize);
            marker.RectTransform.anchoredPosition = NormalizedToAnchored(node.normalizedPosition);

            Color color = GetColorForNode(node.nodeType);

            marker.Initialize(node, OnMarkerClicked, OnMarkerDragged, color);

            _nodeToMarker.Add(node, marker);

            return marker;
        }

        private Vector2 ScreenToNormalized(Vector2 screenPosition)
        {
            Vector2 local;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_layoutRect, screenPosition, null, out local);

            Rect rect = _layoutRect.rect;

            float x01 = (local.x - rect.xMin) / rect.width;
            float y01 = (local.y - rect.yMin) / rect.height;

            return new Vector2(x01, y01);
        }

        private Vector2 NormalizedToAnchored(Vector2 normalizedPosition)
        {
            float x = _markerRoot.rect.xMin + (normalizedPosition.x * _markerRoot.rect.width);
            float y = _markerRoot.rect.yMin + (normalizedPosition.y * _markerRoot.rect.height);

            return new Vector2(x, y);
        }

        private static Color GetColorForNode(NodeType nodeType)
        {
            switch (nodeType)
            {
                case NodeType.Waypoint:
                    return Color.darkBlue;

                case NodeType.Checkpoint:
                    return Color.cyan;

                case NodeType.Entrance:
                    return Color.green;

                case NodeType.Exit:
                    return Color.red;

                case NodeType.Other:
                    return Color.darkGreen;

                default:
                    return Color.white;
            }
        }
        #endregion

        public void Close()
        {
            MessageBusManager.Instance.Publish(new OnAppStateChangeRequest(StateController.AppState.LayoutBrowser));
        }
    }
}