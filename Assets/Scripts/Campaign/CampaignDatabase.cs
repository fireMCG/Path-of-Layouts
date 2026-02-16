using System;
using System.Collections.Generic;
using UnityEngine;

namespace fireMCG.PathOfLayouts.Campaign
{
    [CreateAssetMenu(menuName = "Path of Layouts/Campaign/Campaign Database")]
    public sealed class CampaignDatabase : ScriptableObject
    {
        public ActDef[] acts = Array.Empty<ActDef>();

        public AreaDef[] allAreas = Array.Empty<AreaDef>();
        public GraphDef[] allGraphs = Array.Empty<GraphDef>();
        public LayoutDef[] allLayouts = Array.Empty<LayoutDef>();
        public NavigationDataAsset[] allNavigationData = Array.Empty<NavigationDataAsset>();

        private Dictionary<string, ActDef> _actById;
        private Dictionary<string, AreaDef> _areaById;
        private Dictionary<string, GraphDef> _graphById;
        private Dictionary<string, LayoutDef> _layoutById;

        public bool IsIndexed => _layoutById != null;

        private void OnEnable()
        {
            BuildRuntimeIndex();
        }

        public void BuildRuntimeIndex()
        {
            _actById = new Dictionary<string, ActDef>(StringComparer.Ordinal);
            _areaById = new Dictionary<string, AreaDef>(StringComparer.Ordinal);
            _graphById = new Dictionary<string, GraphDef>(StringComparer.Ordinal);
            _layoutById = new Dictionary<string, LayoutDef>(StringComparer.Ordinal);

            IndexById(_actById, acts);
            IndexById(_areaById, allAreas);
            IndexById(_graphById, allGraphs);
            IndexById(_layoutById, allLayouts);
        }

        public void IndexById<T>(Dictionary<string, T> dictionary, IEnumerable<T> items) where T : DefBase
        {
            if(items is null)
            {
                return;
            }

            foreach(var item in items)
            {
                if(item is null)
                {
                    continue;
                }

                if (string.IsNullOrWhiteSpace(item.id))
                {
                    continue;
                }

                dictionary.TryAdd(item.id, item);
            }
        }

        public ActDef GetAct(string id) => _actById[id];
        public AreaDef GetArea(string id) => _areaById[id];
        public GraphDef GetGraph(string id) => _graphById[id];
        public LayoutDef GetLayout(string id) => _layoutById[id];

        public bool TryGetLayout(string id, out LayoutDef layout)
        {
            layout = null;
            return _layoutById is not null && _layoutById.TryGetValue(id, out layout);
        }

        public int GetAreaCount(string actId) => GetAct(actId).areaCount;
        public int GetGraphCount(string areaId) => GetArea(areaId).graphCount;
        public int GetLayoutCount(string graphId) => GetGraph(graphId).layoutCount;
    }
}