using fireMCG.PathOfLayouts.LayoutBrowser.Ui;
using fireMCG.PathOfLayouts.Manifest;
using fireMCG.PathOfLayouts.Messaging;
using fireMCG.PathOfLayouts.System;
using System.Collections.Generic;
using UnityEngine;

namespace fireMCG.PathOfLayouts.Ui
{
    public class LayoutBrowserUiController : MonoBehaviour
    {
        private enum View
        {
            Acts,
            Areas,
            Graphs,
            Layouts
        }

        [SerializeField] private GameObject _actsMenuRoot;
        [SerializeField] private GameObject _areaMenuRoot;
        [SerializeField] private GameObject _graphGridRoot;
        [SerializeField] private GameObject _layoutGridRoot;
        [SerializeField] private GameObject _backButton;

        [SerializeField] private RectTransform _areaMenuContent;
        [SerializeField] private RectTransform _graphGridContent;
        [SerializeField] private RectTransform _layoutGridContent;

        [SerializeField] private AreaCard _areaCardPrefab;
        [SerializeField] private GraphCard _graphCardPrefab;
        [SerializeField] private LayoutCard _layoutCardPrefab;

        private View _currentView = View.Acts;
        private string _selectedActId = null;
        private string _selectedAreaId = null;
        private string _selectedGraphId = null;
        private string _selectedLayoutId = null;

        private void Awake()
        {
            ResetUi();
        }

        public void OpenMainMenu()
        {
            OnAppStateChangeRequest message = new OnAppStateChangeRequest(StateController.AppState.MainMenu);
            MessageBusManager.Resolve.Publish(message);

            ResetUi();
        }

        public void SelectAct(string actId)
        {
            _selectedActId = actId;

            OpenAreaWindow();
        }

        public void SelectArea(string areaId)
        {
            _selectedAreaId = areaId;

            OpenGraphWindow();
        }

        public void SelectGraph(string graphId)
        {
            _selectedGraphId= graphId;

            OpenLayoutWindow();
        }

        public void SelectLayout(string layoutId)
        {
            _selectedLayoutId = layoutId;
        }

        public void PlayGraph(string graphId)
        {

        }

        public void PlayLayout(string layoutId)
        {

        }

        public void Back()
        {
            switch (_currentView)
            {
                case View.Layouts:
                    _selectedGraphId = null;
                    _selectedLayoutId = null;
                    ClearChildren(_layoutGridContent);
                    OpenGraphWindow();
                    break;

                case View.Graphs:
                    _selectedAreaId = null;
                    ClearChildren(_graphGridContent);
                    OpenAreaWindow();
                    break;

                case View.Areas:
                    _selectedActId = null;
                    ClearChildren(_areaMenuContent);
                    Show(View.Acts);
                    break;

                case View.Acts:
                default:
                    break;
            }
        }

        private void OpenAreaWindow()
        {
            Show(View.Areas);

            IReadOnlyList<AreaEntry> areas = Bootstrap.Instance.ManifestService.Manifest.GetAreas(_selectedActId);

            foreach(AreaEntry area in areas)
            {
                AreaCard card = Instantiate(_areaCardPrefab, _areaMenuContent);
                card.Initialize(SelectArea, area.areaId);
            }
        }

        private void OpenGraphWindow()
        {
            Show(View.Graphs);

            IReadOnlyList<GraphEntry> graphs = Bootstrap.Instance.ManifestService.Manifest
                .GetGraphs(_selectedActId, _selectedAreaId);

            foreach (GraphEntry graph in graphs)
            {
                GraphCard card = Instantiate(_graphCardPrefab, _graphGridContent);
                card.Initialize(SelectGraph, PlayGraph, graph.graphId);
            }
        }

        private void OpenLayoutWindow()
        {
            Show(View.Layouts);
        }

        private void ResetUi()
        {
            _selectedActId = null;
            _selectedAreaId = null;
            _selectedGraphId = null;
            _selectedLayoutId = null;

            ClearChildren(_areaMenuContent);
            ClearChildren(_graphGridContent);
            ClearChildren(_layoutGridContent);

            Show(View.Acts);
        }

        private void Show(View view)
        {
            _currentView = view;

            _actsMenuRoot.SetActive(view == View.Acts);
            _areaMenuRoot.SetActive(view == View.Areas);
            _graphGridRoot.SetActive(view == View.Graphs);
            _layoutGridRoot.SetActive(view == View.Layouts);

            _backButton.SetActive(view != View.Acts);
        }

        private static void ClearChildren(RectTransform parent)
        {
            if (!parent) return;

            for (int i = parent.childCount - 1; i >= 0; i--)
            {
                Destroy(parent.GetChild(i).gameObject);
            }
        }
    }
}