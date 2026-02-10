using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace fireMCG.PathOfLayouts.IO
{
    public static class JsonFileStore
    {
        private static readonly string TEMPORARY_FILE_EXTENSION = ".tmp";
        private static readonly string BACKUP_FILE_EXTENSION = ".bak";

        private static readonly JsonSerializerSettings JsonSettings = new()
        {
            Formatting = Formatting.Indented,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore
        };

        public static async Task<T> LoadOrCreateAsync<T>(string path, Func<T> defaultFactory, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            if (defaultFactory == null)
            {
                throw new ArgumentNullException(nameof(defaultFactory));
            }

            if (!File.Exists(path))
            {
                T created = defaultFactory();
                await SaveAsync(path, created, token);

                return created;
            }

            string json = await File.ReadAllTextAsync(path, Encoding.UTF8, token);

            T data = JsonConvert.DeserializeObject<T>(json, JsonSettings);

            if(data == null)
            {
                token.ThrowIfCancellationRequested();

                T created = defaultFactory();
                await SaveAsync(path, created, token);

                return created;
            }

            return data;
        }

        public static async Task SaveAsync<T>(string path, T data, CancellationToken token)
        {
            string directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            token.ThrowIfCancellationRequested();

            string json = JsonConvert.SerializeObject(data, JsonSettings);

            string tempPath = path + TEMPORARY_FILE_EXTENSION;

            try
            {
                token.ThrowIfCancellationRequested();

                await File.WriteAllTextAsync(tempPath, json, Encoding.UTF8, token);

                if (File.Exists(path))
                {
                    string bakPath = path + BACKUP_FILE_EXTENSION;
                    File.Replace(tempPath, path, bakPath, ignoreMetadataErrors: true);

                    return;
                }
                else
                {
                    File.Move(tempPath, path);
                }
            }
            catch
            {
                try
                {
                    if (File.Exists(tempPath))
                    {
                        File.Delete(tempPath);
                    }
                }
                catch { }

                throw;
            }
        }
    }
}