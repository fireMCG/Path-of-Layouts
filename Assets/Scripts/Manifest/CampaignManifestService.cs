using fireMCG.PathOfLayouts.IO;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace fireMCG.PathOfLayouts.Manifest
{
    public sealed class CampaignManifestService
    {
        public CampaignManifest Manifest { get; private set; }

        public async Task LoadManifestAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            string json = await ReadStreamingAssetsTextAsync(StreamingPathResolver.GetManifestFilePath(), token);

            token.ThrowIfCancellationRequested();

            CampaignManifest manifest = JsonConvert.DeserializeObject<CampaignManifest>(json);

            if(manifest is null)
            {
                throw new Exception($"CampaignManifestService.LoadManifestAsync error,JSON deserialized to null.");
            }

            Manifest = manifest;
        }

        private static async Task<string> ReadStreamingAssetsTextAsync(string path, CancellationToken token)
        {
            if(Application.platform == RuntimePlatform.Android)
            {
                using UnityWebRequest request = UnityWebRequest.Get(path);

                using CancellationTokenRegistration registration = token.Register(() =>
                {
                    request.Abort();
                });

                token.ThrowIfCancellationRequested();

                UnityWebRequestAsyncOperation operation = request.SendWebRequest();

                while (!operation.isDone)
                {
                    token.ThrowIfCancellationRequested();

                    await Task.Yield();
                }

                token.ThrowIfCancellationRequested();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    throw new Exception(
                        $"CampaignManifestService.ReadStreamingAssetsTextAsync error");
                }

                return request.downloadHandler.text;
            }

            if (!File.Exists(path))
            {
                throw new FileNotFoundException(
                    $"CampaignManifestService.ReadStreamingAssetsTextAsync error, manifest not found at {path}");
            }

            token.ThrowIfCancellationRequested();

            return await File.ReadAllTextAsync(path, token);
        }
    }
}