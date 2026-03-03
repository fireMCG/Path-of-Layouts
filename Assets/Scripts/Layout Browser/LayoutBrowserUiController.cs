using fireMCG.PathOfLayouts.Campaign;
using fireMCG.PathOfLayouts.Core;
using fireMCG.PathOfLayouts.LayoutBrowser.Ui;
using fireMCG.PathOfLayouts.Layouts;
using fireMCG.PathOfLayouts.Messaging;
using fireMCG.PathOfLayouts.NodeEditor;
using fireMCG.PathOfLayouts.Prompt;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace fireMCG.PathOfLayouts.Ui
{
    public sealed class LayoutBrowserUiController : MonoBehaviour
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

        [SerializeField] private RectTransform _horizontalGridContainer;
        [SerializeField] private AreaCard _areaCardPrefab;
        [SerializeField] private GraphCard _graphCardPrefab;
        [SerializeField] private LayoutCard _layoutCardPrefab;

        [SerializeField] private Image _compassImage;
        [SerializeField] private Sprite _phaarylCompass;
        [SerializeField] private Sprite _vastiriCompass;
        [SerializeField] private Sprite _utzaalCompass;
        [SerializeField] private Sprite _kingsmarchCompass;

        private View _currentView = View.Acts;
        private string _selectedActId = null;
        private string _selectedAreaId = null;
        private string _selectedGraphId = null;

        private CancellationTokenSource _populateTokenSource;

        private void Awake()
        {
            Assert.IsNotNull(_actsMenuRoot);
            Assert.IsNotNull(_areaMenuRoot);
            Assert.IsNotNull(_graphGridRoot);
            Assert.IsNotNull(_layoutGridRoot);
            Assert.IsNotNull(_backButton);
            Assert.IsNotNull(_areaMenuContent);
            Assert.IsNotNull(_graphGridContent);
            Assert.IsNotNull(_layoutGridContent);
            Assert.IsNotNull(_areaCardPrefab);
            Assert.IsNotNull(_graphCardPrefab);
            Assert.IsNotNull(_layoutCardPrefab);
        }

        private void OnEnable()
        {
            RegisterMessageListeners();
        }

        private void OnDisable()
        {
            UnregisterMessageListeners();

            CancelPopulate();
        }

        private void OnDestroy()
        {
            CancelPopulate();
        }

        private void RegisterMessageListeners()
        {
            MessageBusManager.Instance.Subscribe<OnAppStateChanged>(OnAppStateChanged);
        }

        private void UnregisterMessageListeners()
        {
            MessageBusManager.Instance.Unsubscribe<OnAppStateChanged>(OnAppStateChanged);
        }

        private void OnAppStateChanged(OnAppStateChanged message)
        {
            // Avoid clearing Ui when coming back from gameplay in order to resume at the same position.
            if(message.PreviousState == StateController.AppState.Gameplay || message.PreviousState == StateController.AppState.NodeEditor)
            {
                return;
            }

            if(message.NewState == StateController.AppState.LayoutBrowser)
            {
                ResetUi();
            }
        }

        public void OpenMainMenu()
        {
            MessageBusManager.Instance.Publish(new OnAppStateChangeRequest(StateController.AppState.MainMenu));

            ResetUi();
        }

        public void SelectActByIndex(int index)
        {
            CampaignDatabase database = Bootstrap.Instance.CampaignDatabase;
            if(database.acts == null || database.acts.Length < 1)
            {
                MessageBusManager.Instance.Publish(new OnErrorMessage("Failed to select act. Database acts are null or empty."));

                return;
            }

            if(index < 0 || database.acts.Length - 1 < index)
            {
                throw new System.ArgumentOutOfRangeException();
            }

            SelectId(database.acts[index].id);
        }

        public void SelectId(string id)
        {
            switch (_currentView)
            {
                case View.Acts:
                    _selectedActId = id;
                    PopulateAreaWindow();
                    break;

                case View.Areas:
                    _selectedAreaId = id;
                    _ = PopulateGraphWindowAsync();
                    break;

                case View.Graphs:
                    _selectedGraphId = id;
                    _ = PopulateLayoutWindowAsync();
                    break;

                default:
                    ResetUi();
                    break;
            }
        }

        public void PlayId(string id)
        {
            switch (_currentView)
            {
                case View.Areas:
                    MessageBusManager.Instance.Publish(new LoadRandomGraphMessage(id));
                    break;

                case View.Graphs:
                    MessageBusManager.Instance.Publish(new LoadRandomLayoutMessage(id));
                    break;

                case View.Layouts:
                    MessageBusManager.Instance.Publish(new LoadTargetLayoutMessage(id));
                    break;

                default:
                    ResetUi();
                    break;
            }
        }

        public void Back()
        {
            CancelPopulate();

            switch (_currentView)
            {
                case View.Layouts:
                    _selectedGraphId = null;
                    ClearChildren(_layoutGridContent);
                    Show(View.Graphs);
                    break;

                case View.Graphs:
                    _selectedAreaId = null;
                    ClearChildren(_graphGridContent);
                    Show(View.Areas);
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

        private void PopulateAreaWindow()
        {
            CancelPopulate();

            Show(View.Areas);

            ActDef act = Bootstrap.Instance.CampaignDatabase.GetAct(_selectedActId);
            if (act == null || act.areas is null || act.areas.Length < 1)
            {
                MessageBusManager.Instance.Publish(new OnErrorMessage("Failed to populate areas: act not found or has no areas."));

                return;
            }

            ClearChildren(_areaMenuContent);

            IReadOnlyList<AreaDef> areas = act.areas;

            RectTransform currentContainer = null;
            for(int i = 0; i < areas.Count; i++)
            {
                if(i % 4 == 0)
                {
                    currentContainer = Instantiate(_horizontalGridContainer, _areaMenuContent);
                }

                if (areas[i] == null)
                {
                    continue;
                }

                // To do: Implement custom area thumbnails.
                AreaCard card = Instantiate(_areaCardPrefab, currentContainer);
                card.Initialize(SelectId, PlayId, areas[i].id, areas[i].displayName);
            }
        }

        private async Task PopulateGraphWindowAsync()
        {
            CancelPopulate();

            Show(View.Graphs);

            AreaDef area = Bootstrap.Instance.CampaignDatabase.GetArea(_selectedAreaId);
            if (area == null || area.graphs is null || area.graphs.Length < 1)
            {
                MessageBusManager.Instance.Publish(new OnErrorMessage("Failed to populate graphs: area not found or has no graphs."));

                return;
            }

            ClearChildren(_graphGridContent);

            _populateTokenSource = new CancellationTokenSource();
            CancellationToken token = _populateTokenSource.Token;

            try
            {
                token.ThrowIfCancellationRequested();

                await Bootstrap.Instance.ContentService.PreDownloadAreaGraphRendersAsync(_selectedAreaId, token);

                token.ThrowIfCancellationRequested();

                IReadOnlyList<GraphDef> graphs = area.graphs;


                RectTransform currentContainer = null;
                for (int i = 0; i < graphs.Count; i++)
                {
                    token.ThrowIfCancellationRequested();

                    if (i % 4 == 0)
                    {
                        currentContainer = Instantiate(_horizontalGridContainer, _graphGridContent);
                    }

                    if (graphs[i] == null)
                    {
                        continue;
                    }

                    GraphCard card = Instantiate(_graphCardPrefab, currentContainer);
                    card.Initialize(SelectId, PlayId, graphs[i].id, graphs[i].displayName);

                    Texture2D render = await Bootstrap.Instance.ContentService.LoadGraphRenderAsync(graphs[i].id, token);

                    token.ThrowIfCancellationRequested();

                    card.SetThumbnail(render);
                }
            }
            catch (System.OperationCanceledException) { }
            catch (System.Exception e)
            {
                Debug.LogError(e);

                MessageBusManager.Instance.Publish(new OnErrorMessage("Failed to populate graph renders."));
            }
        }

        private async Task PopulateLayoutWindowAsync()
        {
            CancelPopulate();

            Show(View.Layouts);

            GraphDef graph = Bootstrap.Instance.CampaignDatabase.GetGraph(_selectedGraphId);
            if(graph == null || graph.layouts is null || graph.layouts.Length < 1)
            {
                MessageBusManager.Instance.Publish(new OnErrorMessage("Failed to populate layouts: graph not found or has no layouts."));

                return;
            }

            ClearChildren(_layoutGridContent);

            _populateTokenSource = new CancellationTokenSource();
            CancellationToken token = _populateTokenSource.Token;

            try
            {
                token.ThrowIfCancellationRequested();

                await Bootstrap.Instance.ContentService.PreDownloadGraphLayoutThumbnailsAsync(_selectedGraphId, token);

                token.ThrowIfCancellationRequested();

                IReadOnlyList<LayoutDef> layouts = graph.layouts;

                RectTransform currentContainer = null;
                for (int i = 0; i < layouts.Count; i++)
                {
                    token.ThrowIfCancellationRequested();

                    if (i % 4 == 0)
                    {
                        currentContainer = Instantiate(_horizontalGridContainer, _layoutGridContent);
                    }

                    if (layouts[i] == null)
                    {
                        continue;
                    }

                    LayoutCard card = Instantiate(_layoutCardPrefab, currentContainer);
                    card.Initialize(PlayId, OpenNodeEditor, layouts[i].id, layouts[i].displayName);

                    Texture2D thumbnail = await Bootstrap.Instance.ContentService.LoadLayoutThumbnailAsync(layouts[i].id, token);

                    token.ThrowIfCancellationRequested();

                    card.SetThumbnail(thumbnail);
                }
            }
            catch (System.OperationCanceledException) { }
            catch (System.Exception e)
            {
                Debug.LogError(e);

                MessageBusManager.Instance.Publish(new OnErrorMessage("Failed to populate layout thumbnails."));
            }
        }

        public void OpenNodeEditor(string layoutId)
        {
            MessageBusManager.Instance.Publish(new NodeEditorOpenMessage(layoutId));
        }

        private void ResetUi()
        {
            CancelPopulate();

            _selectedActId = null;
            _selectedAreaId = null;
            _selectedGraphId = null;

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

            SetActCompass();

            _backButton.SetActive(view != View.Acts);
        }

        private void SetActCompass()
        {
            if(_currentView != View.Areas)
            {
                return;
            }

            if(_selectedActId == Bootstrap.Instance.CampaignDatabase.acts[0].id)
            {
                _compassImage.sprite = _phaarylCompass;
            }
            else if(_selectedActId == Bootstrap.Instance.CampaignDatabase.acts[1].id)
            {
                _compassImage.sprite = _vastiriCompass;
            }
            else if (_selectedActId == Bootstrap.Instance.CampaignDatabase.acts[2].id)
            {
                _compassImage.sprite = _utzaalCompass;
            }
            else
            {
                _compassImage.sprite = _kingsmarchCompass;
            }
        }

        private void CancelPopulate()
        {
            if(_populateTokenSource is null)
            {
                return;
            }

            _populateTokenSource.Cancel();
            _populateTokenSource.Dispose();
            _populateTokenSource = null;
        }

        private static void ClearChildren(RectTransform parent)
        {
            if (!parent)
            {
                return;
            }

            for (int i = parent.childCount - 1; i >= 0; i--)
            {
                Destroy(parent.GetChild(i).gameObject);
            }
        }
    }
}