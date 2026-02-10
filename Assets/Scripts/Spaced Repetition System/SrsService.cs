using fireMCG.PathOfLayouts.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace fireMCG.PathOfLayouts.Srs
{
    public class SrsService
    {
        public SrsSaveData Data { get; private set; }

        public async Task<SrsSaveData> LoadSrsSaveDataAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            SrsSaveData data;

            try
            {
                data = await JsonFileStore.LoadOrCreateAsync(
                    PersistentPathResolver.GetSrsFilePath(),
                    SrsSaveData.CreateDefault,
                    token);
            }
            catch(System.Exception e)
            {
                Debug.LogError($"SrsService.LoadSrsSaveDataAsync error, e={e}");

                return null;
            }

            return data;
        }

        // Includes all ids since layoutIds are file names attributed by users which aren't forced to be unique
        public static string GetSrsKey(string actId, string areaId, string graphId, string layoutId)
        {
            return $"{actId}-{areaId}-{graphId}-{layoutId}";
        }
    }
}