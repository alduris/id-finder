using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FinderMod.Search.Options;
using FinderMod.Tabs;
using Menu.Remix.MixedUI.ValueTypes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Result = FinderMod.Search.Threadmaster.Result;

namespace FinderMod.Search
{
    internal static class HistoryManager
    {
        public struct HistoryItem
        {
            public string name;
            public int min;
            public int max;
            public JArray options;
            public Result[][] results;
            public DateTime date;
            public string version;

            public readonly IEnumerable<Option> GetOptions()
            {
                foreach (var item in options)
                {
                    if (OptionRegistry.TryGetOption((string)item["name"]!, out var option))
                    {
                        option.FromJson((item as JObject)!);
                        yield return option;
                    }
                }
            }

            public readonly void RestoreSearch()
            {
                var tab = SearchTab.instance;
                if (tab != null && (tab.threadmaster == null || !tab.threadmaster.Running))
                {
                    tab.options.Clear();
                    foreach (var option in GetOptions())
                    {
                        tab.AddOption(option, false);
                    }
                    tab.UpdateQueryBox();
                    tab.input_min.valueInt = min;
                    tab.input_max.valueInt = max;
                    if (results != null && results.Length > 0 && results[0] != null) tab.input_find.SetValueInt(results[0].Length);
                }
            }

            public readonly override bool Equals(object obj)
            {
                if (obj is HistoryItem other) return (other.name == name && other.date == date) || options.SequenceEqual(other.options);
                return false;
            }

            public readonly override int GetHashCode()
            {
                // this only exists to remove the warning
                return base.GetHashCode();
            }

            public readonly override string ToString()
            {
                return JsonConvert.SerializeObject(this);
            }

            public static bool operator ==(HistoryItem x, HistoryItem y) => x.Equals(y);
            public static bool operator !=(HistoryItem x, HistoryItem y) => !(x == y);
        }

        private static string SaveFile => Path.Combine(Application.persistentDataPath, "idfinder.txt");
        private static readonly List<HistoryItem> historyItems = [];
        private static readonly List<HistoryItem> temporaryHistory = [];
        private static bool loadedHistory = false;

        public static event Action? UpdateHistory;

        private static void LoadHistory()
        {
            if (loadedHistory) return;
            loadedHistory = true;

            if (File.Exists(SaveFile))
            {
                foreach (var line in File.ReadAllLines(SaveFile))
                {
                    try
                    {
                        var data = JsonConvert.DeserializeObject<HistoryItem>(line);
                        historyItems.Add(data);
                    }
                    catch (Exception e)
                    {
                        Plugin.logger.LogError(e);
                    }
                }
            }

            UpdateHistory?.Invoke();
        }

        private static void WriteToFile()
        {
            File.WriteAllLines(SaveFile, historyItems.Select(x => JsonConvert.SerializeObject(x)));
        }

        public static string CreateCopyString(List<Option> options, (int min, int max) range, Result[][] results = null!)
        {
            var history = GetAsHistoryItem(options, results, range);
            return JsonConvert.SerializeObject(history);
        }


        private static HistoryItem GetAsHistoryItem(List<Option> options, Result[][] results, (int min, int max) range)
        {
            results ??= [];

            var array = new JArray();
            foreach (var item in options)
            {
                array.Add(item.ToJson());
            }

            var name = string.Join(", ", options.Select(x => x.name));
            if (name.Trim().Length == 0) name = "Search";
            var history = new HistoryItem()
            {
                name = name,
                min = range.min,
                max = range.max,
                options = array,
                results = results,
                date = DateTime.UtcNow,
                version = Plugin.VERSION
            };

            return history;
        }

        public static void SaveTemporaryHistory(List<Option> options, Result[][] results, (int min, int max) range)
        {
            temporaryHistory.Add(GetAsHistoryItem(options, results, range));
        }

        public static void SaveHistory(List<Option> options, Result[][] results, (int min, int max) range)
        {
            LoadHistory();

            // Create
            var history = GetAsHistoryItem(options, results, range);
            historyItems.Add(history);

            // Remove from temporary history
            temporaryHistory.Remove(history);

            // Save
            WriteToFile();
            UpdateHistory?.Invoke();
        }

        public static void RemoveHistoryItem(HistoryItem item)
        {
            if (historyItems.Remove(item))
            {
                WriteToFile();
                UpdateHistory?.Invoke();
            }
            else
            {
                Plugin.logger.LogDebug("Could not find item!");
            }
        }

        public static void RenameHistoryItem(HistoryItem item, string name)
        {
            for (int i = 0; i < historyItems.Count; i++)
            {
                if (item == historyItems[i])
                {
                    var hi = historyItems[i];
                    hi.name = name;
                    historyItems[i] = hi;
                    WriteToFile();
                    break;
                }
            }
        }

        public static void ClearHistory()
        {
            if (historyItems.Count > 0)
            {
                historyItems.Clear();
                WriteToFile();
            }
        }

        public static List<HistoryItem> GetHistory()
        {
            LoadHistory();
            return [.. historyItems];
        }
        public static List<HistoryItem> GetTempHistory()
        {
            return [.. temporaryHistory];
        }
    }
}
