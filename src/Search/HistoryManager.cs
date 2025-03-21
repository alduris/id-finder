using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FinderMod.Search.Options;
using FinderMod.Tabs;
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
            public JArray options;
            public Result[][] results;
            public DateTime date;

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
                        tab.options.Add(option);
                    }
                    tab.UpdateQueryBox();
                }
            }
        }

        private static string SaveFile => Path.Combine(Application.persistentDataPath, "idfinder.txt");
        private static readonly List<HistoryItem> historyItems = [];
        private static bool loadedHistory = false;

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
        }

        private static void WriteToFile()
        {
            File.WriteAllLines(SaveFile, historyItems.Select(x => JsonConvert.SerializeObject(x)));
        }

        public static void SaveHistory(List<Option> options, Result[][] results)
        {
            LoadHistory();

            // Create
            var array = new JArray();
            foreach (var item in options)
            {
                array.Add(item.ToJson());
            }

            var history = new HistoryItem()
            {
                name = "Test",
                options = array,
                results = results,
                date = DateTime.Now,
            };
            historyItems.Add(history);
            WriteToFile();
        }
    }
}
