using fireMCG.PathOfLayouts.IO;
using System.Threading;
using System.Threading.Tasks;

namespace fireMCG.PathOfLayouts.Srs
{
    public class SrsService
    {
        public SrsSaveData SrsData { get; private set; }

        public async Task LoadSrsSaveDataAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            SrsSaveData data = null;

            data = await JsonFileStore.LoadOrCreateAsync(
                PersistentPathResolver.GetSrsFilePath(),
                SrsSaveData.CreateDefault,
                token);

            if (data is null)
            {
                throw new System.InvalidOperationException($"SrsService.LoadSrsSaveDataAsync error, Srs load returned null data.");
            }

            SrsData = data;
        }

        public async Task SaveSrsDataAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            await JsonFileStore.SaveAsync(
                PersistentPathResolver.GetSrsFilePath(),
                SrsData,
                token);
        }

        public void SetDefaultData()
        {
            SrsData = SrsSaveData.CreateDefault();
        }

        // Includes all ids since layoutIds are file names attributed by users which aren't forced to be unique
        public static string GetSrsLayoutKey(string actId, string areaId, string graphId, string layoutId)
        {
            return $"{actId}-{areaId}-{graphId}-{layoutId}";
        }
    }
}