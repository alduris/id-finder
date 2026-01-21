using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using DevConsole;
using DevConsole.Commands;
using FinderMod.Search;
using FinderMod.Search.Options;
using Menu.Remix.MixedUI;
using RWCustom;
using UnityEngine;

namespace FinderMod
{
    internal static class Commands
    {
        internal static bool registered = false;

        public static void Register()
        {
            registered = true;
            new CommandBuilder("id_finder")
                .Help("id_finder [history|values|spawn]")
                .AutoComplete(FinderAutocomplete)
                .Run(FinderRun)
                .Register();
        }

        private static IEnumerable<string> FinderAutocomplete(string[] args)
        {
            if (args.Length == 0)
            {
                yield return "history";
                yield return "values";
                yield return "spawn";
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
                                yield return $"\"{option}\"";
                            }
                        }
                        else if (args.Length == 2)
                        {
                            yield return "help-id: int";
                        }
                        break;
                    case "spawn":
                        if (args.Length == 1)
                        {
                            yield return "help-index: int";
                        }
                        else if (args.Length == 2)
                        {
                            yield return "null";
                            foreach (var creature in CreatureTemplate.Type.values.entries)
                            {
                                yield return creature;
                            }
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
                            int counterMaxLen = NumDigits(history.Count + tempHistory.Count);
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

                            int NameLengthSafe()
                            {
                                int histLen = history.Count > 0 ? history.Max(x => x.name.Length) : 0;
                                int tempLen = tempHistory.Count > 0 ? tempHistory.Max(x => x.name.Length) : 0;
                                return Math.Max(histLen, tempLen);
                            }
                            int ResultsLengthSafe()
                            {
                                int histLen = history.Count > 0 ? history.Max(x => x.results?.Length > 0 ? x.results[0].Length : 0) : 0;
                                int tempLen = tempHistory.Count > 0 ? tempHistory.Max(x => x.results?.Length > 0 ? x.results[0].Length : 0) : 0;
                                return Math.Max(NumDigits(histLen), NumDigits(tempLen));
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

                            ActuallyPrintResults(historyItem.results);
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
                case "spawn":
                    {
                        if (GameConsole.TargetPos.Room == null)
                        {
                            GameConsole.WriteLine("`id_finder spawn` must be run while in a room!");
                            break;
                        }
                        if (args.Length > 1 && int.TryParse(args[1], out int index))
                        {
                            index--;
                            var history = HistoryManager.GetHistory();
                            var tempHistory = HistoryManager.GetTempHistory();

                            HistoryManager.HistoryItem? historyItem = null;
                            if (index < 0 || index >= history.Count + tempHistory.Count)
                            {
                                GameConsole.WriteLine($"Index out of bounds! Must be between 1 and {history.Count + tempHistory.Count}", Color.red);
                                return;
                            }
                            else if (index < history.Count)
                            {
                                historyItem = history[index];
                            }
                            else
                            {
                                historyItem = tempHistory[index - history.Count];
                            }

                            if (historyItem == null)
                            {
                                GameConsole.WriteLine("Something went wrong! (historyItem was null)", Color.red);
                                return;
                            }
                            
                            var options = historyItem.Value.GetOptions().ToList();
                            int resultsIndex = 0;
                            var target = GameConsole.TargetPos;
                            var wc = Custom.MakeWorldCoordinate(Room.StaticGetTilePosition(target.Pos), target.Room.index);
                            for (int i = 0; i < options.Count; i++)
                            {
                                if (i != 0 && !options[i].linked)
                                {
                                    resultsIndex++;
                                }
                                CreatureTemplate.Type? templateType = null;
                                if (args.Length > 2 + i && args[2 + i] != "null")
                                {
                                    templateType = new CreatureTemplate.Type(args[2 + i], false);
                                }
                                else if (options[i].RepresentedCreature != null)
                                {
                                    templateType = options[i].RepresentedCreature;
                                }

                                if (templateType != null && templateType.Index != -1)
                                {
                                    try
                                    {
                                        var results = historyItem.Value.results[resultsIndex];
                                        foreach (var result in results)
                                        {
                                            var ac = new AbstractCreature(target.Room.world, StaticWorld.GetCreatureTemplate(templateType), null, wc, target.Room.world.game.GetNewID());
                                            target.Room.AddEntity(ac);
                                            if (target.Room.realizedRoom != null)
                                            {
                                                ac.RealizeInRoom();
                                                ac.realizedObject.firstChunk.HardSetPosition(target.Pos);
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Debug.LogException(ex);
                                        GameConsole.WriteLine(ex.ToString(), Color.red);
                                    }
                                }
                                else
                                {
                                    GameConsole.WriteLine($"WARNING: Invalid/missing creature for option {i + 1}! Skipping", Color.yellow);
                                }
                            }
                        }
                        else
                        {
                            GameConsole.WriteLine("Could not parse integer for history index!", Color.red);
                        }
                    }
                    break;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Helper methods for places not in this class where we can guarantee if the assembly is loaded
        
        internal static void TryPrint(string message)
        {
            if (registered) ActuallyPrint(message);
        }
        internal static void TryPrint(string message, Color color)
        {
            if (registered) ActuallyPrint(message, color);
        }

        private static void ActuallyPrint(string message, Color? color = null)
        {
            if (color.HasValue) GameConsole.WriteLine(message, color.Value);
            else GameConsole.WriteLine(message);
        }

        internal static void TryPrintResults(Threadmaster.Result[][] results)
        {
            if (registered) ActuallyPrintResults(results);
        }
        private static void ActuallyPrintResults(Threadmaster.Result[][] results)
        {
            const float maxWidth = 980f; // based on constants defined in https://github.com/SlimeCubed/DevConsole/blob/remix/DevConsole/GameConsole.cs
            FFont font = Futile.atlasManager.GetFontWithName(GameConsole.CurrentFont); // assumed to be a monospace font but we want to check width for wrapping anyways
            StringBuilder sb = new();
            for (int i = 0; i < results.Length; i++)
            {
                if (results.Length > 1) sb.AppendLine($"RESULT {i}");

                int maxIDLength = NumDigits(results[i].Max(x => x.id));
                List<string> toPrint = [.. results[i].Select(x => {
                    return string.Format("{0} (distance: {1})",
                        x.id.ToString().PadLeft(maxIDLength),
                        x.dist);
                })];

                int maxColLength = toPrint.Max(x => x.Length);

                float doubleSpaceLength = WidthOf("  ", font);
                float width = 0f;
                foreach (string s in toPrint)
                {
                    string realStr = s.PadRight(maxColLength);
                    float strWidth = WidthOf(realStr, font);
                    if (width + doubleSpaceLength + strWidth > maxWidth)
                    {
                        sb.AppendLine();
                        width = 0f;
                    }
                    sb.Append("  ");
                    sb.Append(realStr);
                    width += doubleSpaceLength + strWidth;
                }
            }

            GameConsole.WriteLine(sb.ToString());
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // General misc helper methods

        private static float WidthOf(string text, FFont font)
        {
            if (string.IsNullOrEmpty(text)) return 0f;

            float width = 0f;
            foreach (FLetterQuadLine fletterQuadLine in font.GetQuadInfoForText(text, new FTextParams()))
            {
                float oldWidth = width;
                Rect bounds = fletterQuadLine.bounds;
                width = Mathf.Max(oldWidth, bounds.width);
            }
            return width;
        }

        private static int NumDigits(int i)
        {
            return i switch
            {
                -2147483648 => 11,
                < 0 => (int)Math.Log10(-i) + 2,
                0 => 1,
                > 0 => (int)Math.Log10(i) + 1
            };
        }
    }
}
