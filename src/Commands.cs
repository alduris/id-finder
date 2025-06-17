using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using DevConsole;
using DevConsole.Commands;
using FinderMod.Search;
using FinderMod.Search.Options;
using UnityEngine;

namespace FinderMod
{
    internal static class Commands
    {
        private static bool registered = false;

        public static void Register()
        {
            registered = true;
            new CommandBuilder("id_finder")
                .Help("id_finder [history|values]")
                .AutoComplete(FinderAutocomplete)
                .Run(FinderRun)
                .Register();
        }

        //

        private static IEnumerable<string> FinderAutocomplete(string[] args)
        {
            if (args.Length == 0)
            {
                yield return "history";
                yield return "values";
            }
            else
            {
                switch (args[0])
                {
                    case "history":
                        if (args.Length == 1)
                        {
                            yield return "help-index: int";
                        }
                        break;
                    case "values":
                        if (args.Length == 1)
                        {
                            foreach (string option in OptionRegistry.ListOptions())
                            {
                                yield return $"{option}";
                            }
                        }
                        else if (args.Length == 2)
                        {
                            yield return "help-id: int";
                        }
                        break;
                }
            }
        }

        private static void FinderRun(string[] args)
        {
            if (args.Length == 0)
            {
                GameConsole.WriteLine("Too few arguments!", Color.red);
                return;
            }

            switch (args[0])
            {
                case "history":
                    {
                        var history = HistoryManager.GetHistory();
                        var tempHistory = HistoryManager.GetTempHistory();
                        if (args.Length < 2)
                        {
                            int counter = 0;
                            int counterMaxLen = (history.Count + tempHistory.Count).ToString().Length;
                            int nameMaxLen = NameLengthSafe();
                            int resultsMaxLen = ResultsLengthSafe();

                            GameConsole.WriteLine("SAVED HISTORY", Color.white);
                            if (history.Count > 0)
                            {
                                foreach (var item in history)
                                {
                                    counter++;
                                    GameConsole.WriteLine(HistoryItemPreview(item));
                                    Plugin.logger.LogDebug(HistoryItemPreview(item));
                                }
                            }
                            else
                            {
                                GameConsole.WriteLine("  No saved history");
                            }
                            GameConsole.WriteLine("TEMPORARY HISTORY", Color.white);
                            if (tempHistory.Count > 0)
                            {
                                foreach (var item in tempHistory)
                                {
                                    counter++;
                                    GameConsole.WriteLine(HistoryItemPreview(item));
                                }
                            }
                            else
                            {
                                GameConsole.WriteLine("  No temporary history");
                            }

                            [MethodImpl(MethodImplOptions.AggressiveInlining)]
                            int NameLengthSafe()
                            {
                                int histLen = history.Count > 0 ? history.Max(x => x.name.Length) : 0;
                                int tempLen = tempHistory.Count > 0 ? tempHistory.Max(x => x.name.Length) : 0;
                                return Math.Max(histLen, tempLen);
                            }
                            [MethodImpl(MethodImplOptions.AggressiveInlining)]
                            int ResultsLengthSafe()
                            {
                                int histLen = history.Count > 0 ? history.Max(x => x.results?.Length > 0 ? x.results[0].Length : 0) : 0;
                                int tempLen = tempHistory.Count > 0 ? tempHistory.Max(x => x.results?.Length > 0 ? x.results[0].Length : 0) : 0;
                                return Math.Max(histLen.ToString().Length, tempLen.ToString().Length);
                            }
                            string HistoryItemPreview(HistoryManager.HistoryItem item)
                            {
                                return string.Format(
                                    "  {0}  {1}  {2} results  (searched {3:G})",
                                    counter.ToString().PadLeft(counterMaxLen),
                                    item.name.PadRight(nameMaxLen),
                                    (item.results?.Length > 0 ? item.results[0].Length * item.results.Length : 0).ToString().PadLeft(resultsMaxLen),
                                    item.date.ToLocalTime()
                                    );
                            }
                        }
                        else if (int.TryParse(args[1], out int index) && index >= 1 && index <= history.Count + tempHistory.Count)
                        {
                            index--;
                            HistoryManager.HistoryItem historyItem;
                            if (index < history.Count)
                            {
                                // Use saved history
                                historyItem = history[index];
                            }
                            else
                            {
                                // Use temporary history
                                historyItem = tempHistory[index - history.Count];
                            }
                            
                            for (int i = 0; i < historyItem.results.Length; i++)
                            {
                                if (historyItem.results.Length == 1)
                                    GameConsole.WriteLine("RESULTS", Color.white);
                                else
                                    GameConsole.WriteLine("RESULT " + i, Color.white);

                                int padLength = historyItem.results[i].Max(x => x.id.ToString().Length);

                                // Auto space them so we take up as little space as possible
                                List<string> outputs = [];
                                for (int j = 0; j < historyItem.results[i].Length; j++)
                                {
                                    outputs.Add($"{historyItem.results[i][j].id.ToString().PadLeft(padLength)}  (distance: {historyItem.results[i][j].dist})");
                                }
                            }
                        }
                        else
                        {
                            GameConsole.WriteLine("Invalid history index!", Color.red);
                            return;
                        }
                    }
                    break;
                case "values":
                    {
                        int id;
                        if (args.Length < 3)
                        {
                            GameConsole.WriteLine("Too few arguments!", Color.red);
                            return;
                        }
                        else if (!int.TryParse(args[2], out id))
                        {
                            GameConsole.WriteLine("ID is not an integer!", Color.red);
                            return;
                        }

                        if (OptionRegistry.TryGetOption(args[1], out Option option))
                        {
                            foreach (string value in option.GetValues(id))
                            {
                                if (value is not null)
                                    GameConsole.WriteLine(value);
                                else
                                    GameConsole.WriteLine("");
                            }
                        }
                        else
                        {
                            GameConsole.WriteLine($"\"{args[0]}\" is not a valid option!");
                            return;
                        }
                    }
                    break;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Helper methods for places not in this class where we can guarantee if the assembly is loaded
        
        public static void TryPrint(string message)
        {
            if (registered) ActuallyPrint(message);
        }
        public static void TryPrint(string message, Color color)
        {
            if (registered) ActuallyPrint(message, color);
        }
        private static void ActuallyPrint(string message, Color? color = null)
        {
            if (color.HasValue) GameConsole.WriteLine(message, color.Value);
            else GameConsole.WriteLine(message);
        }
    }
}
