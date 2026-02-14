using fireMCG.PathOfLayouts.Common;
using fireMCG.PathOfLayouts.Core;
using fireMCG.PathOfLayouts.IO;
using fireMCG.PathOfLayouts.Manifest;
using fireMCG.PathOfLayouts.Messaging;
using System.Collections.Generic;
using UnityEngine;

namespace fireMCG.PathOfLayouts.Layouts
{
    public class LayoutLoader : MonoBehaviour
    {
        public enum LayoutLoadingMethod { RandomAct, RandomArea, RandomGraph, RandomLayout, TargetLayout }

        private void OnEnable()
        {
            RegisterMessageListeners();
        }

        private void OnDisable()
        {
            UnregisterMessageListeners();
        }

        private void RegisterMessageListeners()
        {
            MessageBusManager.Instance.Subscribe<LoadRandomActMessage>(PlayRandomAct);
            MessageBusManager.Instance.Subscribe<LoadRandomAreaMessage>(PlayRandomArea);
            MessageBusManager.Instance.Subscribe<LoadRandomGraphMessage>(PlayRandomGraph);
            MessageBusManager.Instance.Subscribe<LoadRandomLayoutMessage>(PlayRandomLayout);
            MessageBusManager.Instance.Subscribe<LoadTargetLayoutMessage>(PlayTargetLayout);
        }

        private void UnregisterMessageListeners()
        {
            MessageBusManager.Instance.Unsubscribe<LoadRandomActMessage>(PlayRandomAct);
            MessageBusManager.Instance.Unsubscribe<LoadRandomAreaMessage>(PlayRandomArea);
            MessageBusManager.Instance.Unsubscribe<LoadRandomGraphMessage>(PlayRandomGraph);
            MessageBusManager.Instance.Unsubscribe<LoadRandomLayoutMessage>(PlayRandomLayout);
            MessageBusManager.Instance.Unsubscribe<LoadTargetLayoutMessage>(PlayTargetLayout);
        }

        private void PlayRandomAct(LoadRandomActMessage message)
        {
            IReadOnlyList<ActEntry> acts = Bootstrap.Instance.ManifestService.Manifest.acts;
            string actId = acts[Random.Range(0, acts.Count)].actId;

            PlayRandomArea(actId, LayoutLoadingMethod.RandomAct);
        }

        private void PlayRandomArea(LoadRandomAreaMessage message)
        {
            IReadOnlyList<AreaEntry> areas = Bootstrap.Instance.ManifestService.Manifest.GetAreas(message.ActId);
            string areaId = areas[Random.Range(0, areas.Count)].areaId;

            PlayRandomGraph(message.ActId, areaId, LayoutLoadingMethod.RandomArea);
        }

        private void PlayRandomArea(string actId, LayoutLoadingMethod loadingMethod)
        {
            IReadOnlyList<AreaEntry> areas = Bootstrap.Instance.ManifestService.Manifest.GetAreas(actId);
            string areaId = areas[Random.Range(0, areas.Count)].areaId;

            PlayRandomGraph(actId, areaId, LayoutLoadingMethod.RandomArea);
        }

        private void PlayRandomGraph(LoadRandomGraphMessage message)
        {
            IReadOnlyList<GraphEntry> graphs = Bootstrap.Instance.ManifestService.Manifest.GetGraphs(message.ActId, message.AreaId);
            string graphId = graphs[Random.Range(0, graphs.Count)].graphId;

            PlayRandomLayout(message.ActId, message.AreaId, graphId, LayoutLoadingMethod.RandomGraph);
        }

        private void PlayRandomGraph(string actId, string areaId, LayoutLoadingMethod loadingMethod)
        {
            IReadOnlyList<GraphEntry> graphs = Bootstrap.Instance.ManifestService.Manifest.GetGraphs(actId, areaId);
            string graphId = graphs[Random.Range(0, graphs.Count)].graphId;

            PlayRandomLayout(actId, areaId, graphId, LayoutLoadingMethod.RandomGraph);
        }

        private void PlayRandomLayout(LoadRandomLayoutMessage message)
        {
            IReadOnlyList<string> layouts = Bootstrap.Instance.ManifestService.Manifest.GetLayoutIds(message.ActId, message.AreaId, message.GraphId);
            string layoutId = layouts[Random.Range(0, layouts.Count)];

            TryLoadLayout(message.ActId, message.AreaId, message.GraphId, layoutId, LayoutLoadingMethod.RandomLayout);
        }

        private void PlayRandomLayout(string actId, string areaId, string graphId, LayoutLoadingMethod loadingMethod)
        {
            IReadOnlyList<string> layouts = Bootstrap.Instance.ManifestService.Manifest.GetLayoutIds(actId, areaId, graphId);
            string layoutId = layouts[Random.Range(0, layouts.Count)];

            TryLoadLayout(actId, areaId, graphId, layoutId, loadingMethod);
        }

        private void PlayTargetLayout(LoadTargetLayoutMessage message)
        {
            TryLoadLayout(message.ActId, message.AreaId, message.GraphId, message.LayoutId, LayoutLoadingMethod.TargetLayout);
        }

        private void TryLoadLayout(string actId, string areaId, string graphId, string layoutId, LayoutLoadingMethod loadingMethod)
        {
            Texture2D layoutMap = null;
            Texture2D collisionMap = null;
            string layoutPath = StreamingPathResolver.GetLayoutFilePath(actId, areaId, graphId, layoutId);
            string collisionPath = StreamingPathResolver.GetCollisionMapFilePath(actId, areaId, graphId, layoutId);

            try
            {
                layoutMap = TextureFileLoader.LoadPng(layoutPath, FilterMode.Bilinear);
                collisionMap = TextureFileLoader.LoadPng(collisionPath, FilterMode.Point);

                if(layoutMap is null || collisionMap is null)
                {
                    throw new System.Exception("Layout and collision maps can't be null.");
                }
            }
            catch(System.Exception e)
            {
                throw new System.Exception($"LayoutLoader.TryLoadLayout failed to load textures.", e);
            }

            MessageBusManager.Instance.Publish(new OnAppStateChangeRequest(StateController.AppState.Gameplay));
            MessageBusManager.Instance.Publish(new OnLayoutLoadedMessage(
                actId,
                areaId,
                graphId,
                layoutId,
                layoutMap,
                collisionMap,
                loadingMethod));
        }
    }
}