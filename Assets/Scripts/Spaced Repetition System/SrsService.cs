using fireMCG.PathOfLayouts.Common;
using fireMCG.PathOfLayouts.IO;
using fireMCG.PathOfLayouts.Messaging;
using fireMCG.PathOfLayouts.Prompt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace fireMCG.PathOfLayouts.Srs
{
    public class SrsService : IPersistable
    {
        public const float LOW_SUCCESS_RATE_RATIO = 0.65f;

        public SrsSaveData SaveData { get; private set; }

        public bool IsDirty { get; private set; }

        public string Name => "Srs";

        public void MarkClean() => IsDirty = false;

        public void MarkDirty()
        {
            IsDirty = true;

            MessageBusManager.Instance.Publish(new OnPersistableSetDirtyMessage());
        }

        public async Task LoadSrsSaveDataAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            SaveData = await JsonFileStore.LoadOrCreateAsync(
                PersistentPathResolver.GetSrsFilePath(),
                SrsSaveData.CreateDefault,
                token);

            if (SaveData is null)
            {
                throw new InvalidOperationException($"SrsService.LoadSrsSaveDataAsync error, Srs load returned null data");
            }
        }

        public async Task SaveAsync(CancellationToken token)
        {
            if (!IsDirty)
            {
                return;
            }

            token.ThrowIfCancellationRequested();

            if(SaveData is null)
            {
                throw new InvalidOperationException("SrsService.SaveSaveDataAsync error, Srs data is null");
            }

            await JsonFileStore.SaveAsync(
                PersistentPathResolver.GetSrsFilePath(),
                SaveData,
                token);

            MarkClean();
        }

        public void SetDefaultData()
        {
            SaveData = SrsSaveData.CreateDefault();
        }

        public bool AddToLearning(string entryId, SrsDataType entryType)
        {
            if (!TryValidateId(entryId, "SrsService.AddToLearning", "Error adding srs entry to the learning queue"))
            {
                return false;
            }

            if (SaveData.entries.TryGetValue(entryId, out SrsEntryData entryData))
            {
                entryData.isLearning = true;
            }
            else
            {
                entryData = new(entryId, entryType)
                {
                    isLearning = true
                };

                SaveData.entries.Add(entryId, entryData);
            }

            MarkDirty();

            // To do: Srs Layout Added/Enabled Message(s)

            return true;
        }

        public bool RemoveFromLearning(string entryId)
        {
            if (!TryValidateId(entryId, "SrsService.RemoveFromLearning", "Error removing srs entry from the learning queue"))
            {
                return false;
            }

            if (!TryGetIdValue(entryId, "RemoveFromLearning", "Error removing srs entry from the learning queue", out SrsEntryData entryData))
            {
                return false;
            }

            entryData.isLearning = false;

            MarkDirty();

            // To do: Srs Layout Removed/Disabled Message(s)

            return true;
        }

        public bool ToggleLearningState(string entryId)
        {
            if (!TryValidateId(entryId, "SrsService.ToggleLearningState", "Error toggling srs entry learning queue state"))
            {
                return false;
            }

            if (!TryGetIdValue(entryId, "ToggleLearningState", "Error toggling srs entry learning queue state", out SrsEntryData entryData))
            {
                return false;
            }

            entryData.isLearning = !entryData.isLearning;

            MarkDirty();

            // To do: Srs Layout Toggled Message(s)

            return true;
        }

        public void RecordPractice(string entryId, SrsPracticeResult result, float time)
        {
            if (!TryValidateId(entryId, "SrsService.RecordPractice", "Error recording practice results"))
            {
                return;
            }

            if(!TryGetIdValue(entryId, "RecordPractice", "Error recording practice results", out SrsEntryData data))
            {
                return;
            }

            data.masteryLevel = SrsScheduler.ClampMastery(data.masteryLevel + (result == SrsPracticeResult.Success ? 1 : -1));
            data.averageTimeSeconds = data.GetRunningAverageTime(time);

            if(data.bestTimeSeconds < time)
            {
                data.bestTimeSeconds = time;
            }

            data.timesPracticed++;
            data.timesSucceeded += result == SrsPracticeResult.Success ? 1 : 0;
            data.timesFailed += result == SrsPracticeResult.Failure ? 1 : 0;
            
            if(data.lastResult == result.ToString())
            {
                data.streak++;
            }
            else
            {
                data.streak = 1;
            }
            data.lastResult = result.ToString();

            data.lastPracticedUtc = DateTime.UtcNow.ToIsoUtc();

            MarkDirty();

            // To do: Srs Layout Practice Recorded Message
        }

        public IReadOnlyList<SrsEntryData> GetDueEntries(int limit) => GetDueEntries(null, limit);

        public IReadOnlyList<SrsEntryData> GetDueEntries(DateTime? nowUtc = null, int? limit = null)
        {
            DateTime now = nowUtc ?? DateTime.UtcNow;

            IEnumerable<SrsEntryData> query = SaveData.entries.Values
                .Where(l => l is not null && l.isLearning)
                .Select(l => (Layout: l, Due: l.GetDueDateTime(), IsNew: l.timesPracticed < 1))
                .Where(t => t.IsNew || now >= t.Due)
                .OrderBy(t => t.IsNew)
                .ThenBy(t => t.Due)
                .ThenBy(t => t.Layout.masteryLevel)
                .Select(t => t.Layout);

            return ApplyLimit(query, limit).ToList();
        }

        public IReadOnlyList<SrsEntryData> GetNextDueEntries(int limit) => GetNextDueEntries(null, limit);

        public IReadOnlyList<SrsEntryData> GetNextDueEntries(DateTime? nowUtc = null, int? limit = null)
        {
            DateTime now = nowUtc ?? DateTime.UtcNow;

            IEnumerable<SrsEntryData> query = SaveData.entries.Values
                .Where(l => l is not null && l.isLearning && l.timesPracticed > 0)
                .Select(l => (Layout: l, Due: l.GetDueDateTime()))
                .Where(t => now < t.Due)
                .OrderBy(t => t.Due)
                .ThenBy(t => t.Layout.masteryLevel)
                .Select(t => t.Layout);

            return ApplyLimit(query, limit).ToList();
        }

        public IReadOnlyList<SrsEntryData> GetBurntEntries(int? limit = null)
        {
            IEnumerable<SrsEntryData> query = SaveData.entries.Values
                .Where(l => l.masteryLevel == SrsScheduler.MasteryIntervals.Length - 1)
                .OrderByDescending(l => l.lastPracticedUtc);

            return ApplyLimit(query, limit).ToList();
        }

        public IReadOnlyList<SrsEntryData> GetDisabledEntries(int? limit = null)
        {
            IEnumerable<SrsEntryData> query = SaveData.entries.Values
                .Where(l => !l.isLearning)
                .OrderBy(l => l.masteryLevel)
                .ThenBy(l => l.lastPracticedUtc);

            return ApplyLimit(query, limit).ToList();
        }

        public IReadOnlyList<SrsEntryData> GetLowSuccessEntries(int? limit = null)
        {
            IEnumerable<SrsEntryData> query = SaveData.entries.Values
                .Where(l => l.timesPracticed > 0 && (float)l.timesSucceeded / l.timesPracticed <= LOW_SUCCESS_RATE_RATIO)
                .OrderBy(l => (float)l.timesSucceeded / l.timesPracticed);

            return ApplyLimit(query, limit).ToList();
        }

        public int GetEntriesDueWithin(DateTime dueAfter, TimeSpan timeSpan)
        {
            DateTime dueBefore = DateTime.UtcNow.Add(timeSpan);

            return SaveData.entries.Values
                .Where(l => l.isLearning && (l.GetDueDateTime() >= dueAfter && l.GetDueDateTime() < dueBefore))
                .Count();
        }

        public bool IsEntryDue(string entryId, DateTime? nowUtc = null)
        {
            if (string.IsNullOrWhiteSpace(entryId) || !SaveData.entries.ContainsKey(entryId))
            {
                return false;
            }

            return IsEntryDue(SaveData.entries[entryId], nowUtc);
        }

        public bool IsEntryDue(SrsEntryData entryData, DateTime? nowUtc = null)
        {
            if (entryData is null || !entryData.isLearning)
            {
                return false;
            }

            DateTime now = nowUtc ?? DateTime.UtcNow;

            return now >= entryData.GetDueDateTime() || entryData.timesPracticed < 1;
        }

        public bool IsLearning(string entryId)
        {
            if (string.IsNullOrWhiteSpace(entryId))
            {
                return false;
            }

            if(!SaveData.entries.TryGetValue(entryId, out SrsEntryData data))
            {
                return false;
            }

            if(data is null || !data.isLearning)
            {
                return false;
            }

            return true;
        }

        private static IEnumerable<T> ApplyLimit<T>(IEnumerable<T> query, int? limit)
        {
            return (limit is > 0) ? query.Take(limit.Value) : query;
        }

        private bool TryValidateId(string entryId, string methodName, string userFacingHeader)
        {
            if (string.IsNullOrWhiteSpace(entryId))
            {
                string details =
                    $"{userFacingHeader}\n" +
                    $"Srs entry id is invalid" +
                    $"id={entryId}";

                LogAndPublishError(methodName, "id is invalid", entryId, details);

                return false;
            }

            return true;
        }

        private bool TryGetIdValue(string entryId, string methodName, string userFacingHeader, out SrsEntryData srsData)
        {
            srsData = null;

            if(SaveData is null)
            {
                string details =
                    $"{userFacingHeader}\n" +
                    $"Srs data is null\n" +
                    $"id={entryId}";

                LogAndPublishError(methodName, "Srs data is null", entryId, details);

                return false;
            }

            if (!SaveData.entries.TryGetValue(entryId, out srsData))
            {
                string details =
                    $"{userFacingHeader}\n" +
                    "Srs entry can't be found\n" +
                    $"id={entryId}";

                LogAndPublishError(methodName, "id can't be found", entryId, details);

                return false;
            }

            return true;
        }

        private static void LogAndPublishError(string methodName, string error, string entryId, string errorMessage)
        {
            Debug.LogError($"{methodName} error, {error}. id={entryId}");
            MessageBusManager.Instance.Publish(new OnErrorMessage(errorMessage));
        }
    }
}