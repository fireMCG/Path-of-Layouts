using fireMCG.PathOfLayouts.Manifest;
using fireMCG.PathOfLayouts.Messaging;
using System.Threading.Tasks;
using UnityEngine;

namespace fireMCG.PathOfLayouts.Core
{
    public sealed class Bootstrap : MonoBehaviour
    {
        public static Bootstrap Instance { get; private set; }
        public CampaignManifestService ManifestService { get; private set; }
        public bool IsReady { get; private set; } = false;

        private async void Awake()
        {
            if(Instance is not null)
            {
                Destroy(this);

                return;
            }

            Instance = this;

            await InitializeManifestServiceAsync();
        }

        private async Task InitializeManifestServiceAsync()
        {
            ManifestService = new CampaignManifestService();

            try
            {
                await ManifestService.LoadManifestAsync();
                IsReady = true;

                MessageBusManager.Resolve.Publish(new OnBootstrapReadyMessage());
            }
            catch(System.Exception e)
            {
                Debug.LogError($"Bootstrap.InitializeManifestServiceAsync error, failed to load manifest. e={e}");
            }
        }
    }
}