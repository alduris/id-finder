using System;
using System.Collections.Generic;
using System.Linq;
using FinderMod.Search;
using FinderMod.Search.Options;
using Menu.Remix;
using Menu.Remix.MixedUI;
using Menu.Remix.MixedUI.ValueTypes;
using Newtonsoft.Json;
using UnityEngine;
using static FinderMod.OpUtil;
using static FinderMod.Search.HistoryManager;

namespace FinderMod.Tabs
{
    internal class SearchTab(OptionInterface owner) : BaseTab(owner, "Search")
    {
        internal static SearchTab instance = null!;
        
        private OpScrollBox cont_queries = null!;
        private OpScrollBox cont_results = null!;
        private OpLabel label_progress = null!;
        internal OpTextBox input_min = null!;
        internal OpTextBox input_max = null!;
        internal OpDragger input_find = null!;
        internal readonly List<Option> options = [];
        internal Threadmaster? threadmaster = null;
        private DateTime startTime;

        private bool waitingForResults = false;

        public override void Initialize()
        {
            instance = this;
            options.Clear();

            // Get max number of threads we can use
            var maxThreads = Environment.ProcessorCount * 2;

            // Initialize elements we need
            var combo_allOpts = new OpComboBox2(
                CosmeticBind(""), new(10f, 520f), 250f,
                [.. OptionRegistry.ListOptions()
                    .Select(s => new ListItem(s))]
            )
            { listHeight = 24 };
            var button_add = new OpSimpleButton(new(266f, 520f), new(80f, 24f), "ADD") { description = "Add an item to search for" };
            var button_copy = new OpSimpleButton(new(464f, 520f), new(60f, 24f), "COPY") { description = "Copy to clipboard" };
            var button_paste = new OpSimpleButton(new(530f, 520f), new(60f, 24f), "PASTE") { description = "Paste from clipboard" };
            
            cont_queries = new OpScrollBox(new(10f, 240f), new(580f, 270f), 0f, false, true, true);

            input_min = new OpTextBox(CosmeticBind(0), new(50f, 206f), 100f) { description = "Start of search range", allowSpace = true };
            input_max = new OpTextBox(CosmeticBind(100000), new(185f, 206f), 100f) { description = "End of search range", allowSpace = true };
            input_find = new OpDragger(CosmeticRange(6, 1, 100), 360f, 206f) { description = "Number of ids to find per result (1-100)" };
            var input_threads = new OpDragger(CosmeticRange(maxThreads / 4, 1, maxThreads), 455f, 206f) { description = "Number of threads to use" };

            var button_run = new OpSimpleButton(new(510f, 206f), new(80f, 24f), "SEARCH") { description = "Start the search!", colorEdge = BlueColor };

            cont_results = new OpScrollBox(new(10f, 10f), new(580f, 160f), 0f, false, true, true);

            // Set up event listeners
            // button_save.OnClick += (_) => HistoryManager.SaveHistory(options, [], (input_min.valueInt, input_max.valueInt));
            button_copy.OnClick += (_) =>
            {
                UniClipboard.SetText(HistoryManager.CreateCopyString(options, (input_min.valueInt, input_max.valueInt)));
                ConfigContainer.instance.CfgMenu.ShowAlert(OptionalText.GetText(OptionalText.ID.ConfigContainer_AlertCopyCosmetic).Replace("<Text>", "search"));
            };
            button_paste.OnClick += (_) =>
            {
                try
                {
                    var data = JsonConvert.DeserializeObject<HistoryItem>(UniClipboard.GetText());
                    data.RestoreSearch();
                }
                catch (Exception ex)
                {
                    button_paste.PlaySound(SoundID.MENU_Error_Ping);
                    ConfigContainer.instance.CfgMenu.ShowAlert("Clipboard does not contain valid search!");
                    Plugin.logger.LogError(ex);
                }
            };

            button_add.OnClick += _ =>
            {
                string value = combo_allOpts.value;
                combo_allOpts.value = null;

                if (waitingForResults) return;
                if (OptionRegistry.TryGetOption(value, out var option))
                {
                    AddOption(option, true);
                }
            };

            button_run.OnClick += _ =>
            {
                if (waitingForResults || options.Count == 0) return;

                // Set up and validate search request
                (int, int) range = (input_min.valueInt, input_max.valueInt);
                int threads = input_threads.GetValueInt();
                int resultsPer = input_find.GetValueInt();

                // Deactivate all the items and stuff
                waitingForResults = true;
                foreach (var item in cont_queries.items)
                {
                    if (item is UIfocusable foc)
                    {
                        foc.greyedOut = true;
                    }
                }

                // Add searching text into results scroll box
                foreach (UIelement element in cont_results.items)
                {
                    element.Deactivate();
                    _RemoveItem(element);
                }
                cont_results.items.Clear();
                cont_results.SetContentSize(0f, true);

                var label_searching = new OpLabel(10f, cont_results.size.y - 40f, "SEARCHING...", true);
                label_progress = new OpLabel(10f, cont_results.size.y - 70f, "0.00% complete", false);
                var button_abort = new OpSimpleButton(new(10f, cont_results.size.y - 100f), new(80f, 24f), "ABORT")
                { description = "Aborts the search and return the current results", colorEdge = RedColor, colorFill = RedColor };
                button_abort.OnClick += _ =>
                {
                    threadmaster?.Abort("user");
                };
                cont_results.AddItems(label_searching, label_progress, button_abort);
                cont_results.SetContentSize(80f, true);

                startTime = DateTime.Now;
                threadmaster = new Threadmaster(options, threads, resultsPer, range, false);
                threadmaster.Run();
            };

            // Add stuff to tab
            var UIArrPlayerOptions = new UIelement[]
            {
                new OpLabel(10f, 570f, "Input", true),
                new OpLabel(10f, 550f, "WARNING: do not leave this tab while searching for ids.", false) { color = YellowColor },
                combo_allOpts, button_add,
                button_copy, button_paste,
                cont_queries,
                new OpLabel(10f, 206f, "From:"),
                input_min,
                new OpLabel(160f, 206f, "To:"),
                input_max,
                new OpLabel(310f, 206f, "Results:"),
                input_find,
                new OpLabel(400f, 206f, "Threads:"),
                input_threads,
                button_run,
                new OpLabel(10f, 176f, "Output", true),
                cont_results,
            };
            AddItems(UIArrPlayerOptions);
        }

        internal void AddOption(Option option, bool update)
        {
            options.Add(option);
            option.OnLink += UpdateQueryBox;
            option.OnDelete += () =>
            {
                options.Remove(option);
                UpdateQueryBox();
            };
            if (update) UpdateQueryBox();
        }

        internal void UpdateQueryBox()
        {
            const float PADDING = 10f;
            float oldHeight = cont_queries.contentSize;
            float oldScroll = cont_queries.scrollOffset;

            // Remove old
            foreach (UIelement element in cont_queries.items)
            {
                element.Deactivate();
                _RemoveItem(element);
            }
            cont_queries.items.Clear();
            cont_queries.SetContentSize(0);

            // Add new
            float y = cont_queries.size.y - PADDING; // start from top of scroll box
            List<UIelement> items = [];
            bool first = true;

            foreach (Option option in options)
            {
                option.firstOption = first;
                first = false;
                option.CreateOptions(ref y, items);
                y -= PADDING;
            }
            cont_queries.AddItems([.. items]);

            cont_queries.SetContentSize(cont_queries.size.y - y + PADDING, true);
            cont_queries.ScrollOffset = oldScroll + (oldHeight - cont_queries.contentSize);
        }

        public override void Update()
        {
            if (threadmaster != null && waitingForResults)
            {
                if (!threadmaster.Running)
                {
                    waitingForResults = false;

                    // Remove old elements
                    foreach (UIelement element in cont_results.items)
                    {
                        element.Deactivate();
                        _RemoveItem(element);
                    }
                    cont_results.items.Clear();
                    cont_results.SetContentSize(0f, true);

                    // Init label
                    var label = new OpLabelLong(new(10f, cont_results.size.y - 10f), new(cont_results.size.x - 20f, 0f), "", true, FLabelAlignment.Left) { verticalAlignment = OpLabel.LabelVAlignment.Bottom };

                    // Did we abort?
                    if (threadmaster.AbortReason != null)
                    {
                        label.text += $"OPERATION ABORTED EARLY ({threadmaster.AbortReason})\n";
                    }

                    var results = threadmaster.GetResults();
                    for (int i = 0; i < results.Length; i++)
                    {
                        label.text += $"Request {i + 1}:\n";
                        for (int j = 0; j < results[i].Length; j++)
                        {
                            var res = results[i][j];
                            label.text += $"        {res.id} (distance: {res.dist})\n";
                        }
                    }

                    var labelSize = (label.GetLineCount() - 1) * label.LineHeight;
                    label.text = label.text.Substring(0, label.text.Length - 1); // get rid of last \n
                    label.PosY -= labelSize;

                    // Copy results button
                    var button_copy = new OpSimpleButton(new(10f, cont_results.size.y - labelSize - 40f), new(48f, 24f), "COPY") { description = "Copy results" };
                    button_copy.OnClick += (_) =>
                    {
                        UniClipboard.SetText(label.text);
                        ConfigContainer.instance.CfgMenu.ShowAlert(OptionalText.GetText(OptionalText.ID.ConfigContainer_AlertCopyCosmetic).Replace("<Text>", "results"));
                    };

                    // Set the scroll box size
                    cont_results.AddItems(label, button_copy);
                    cont_results.SetContentSize(labelSize + 50f, true);

                    // Save history
                    if (threadmaster.AbortReason == null)
                    {
                        bool saved = false;
                        var optionsLocalClone = options.ToList();
                        var range = (input_min.valueInt, input_max.valueInt);
                        var button_save = new OpSimpleButton(new(64f, cont_results.size.y - labelSize - 40f), new(48f, 24f), "SAVE") { description = "Save results to history" };
                        button_save.OnClick += (_) =>
                        {
                            if (saved) return;
                            saved = true;
                            HistoryManager.SaveHistory(optionsLocalClone, results, range);
                            button_save.Deactivate();
                            RemoveItems(button_save);
                        };
                        cont_results.AddItems(button_save);

                        // Temporary history gets removed after it is actually saved, or when the game closes
                        HistoryManager.SaveTemporaryHistory(options, results, range);

                        // Also print to dev console
                        Commands.TryPrint("ID FINDER RESULTS", Color.white);
                        Commands.TryPrint(string.Join(", ", [.. options]), Color.white);
                        Commands.TryPrint(label.text);
                    }

                    // Reupdate query box to reenable everything
                    UpdateQueryBox();
                }
                else
                {
                    // Update progress thingy
                    double min = threadmaster.Progress;
                    long difference = DateTime.Now.Ticks - startTime.Ticks;
                    TimeSpan startTicks = min == 0 ? TimeSpan.MaxValue : new(difference);
                    TimeSpan endTicks = min == 0 ? TimeSpan.MaxValue : new((long)(difference / min));
                    string startStr = startTicks.ToString(endTicks.TotalHours >= 1 ? "hh':'mm':'ss" : "mm':'ss");
                    string endStr = endTicks.ToString(endTicks.TotalHours >= 1 ? "hh':'mm':'ss" : "mm':'ss");
                    label_progress.text = $"{min * 100.0:N2}% completed, {startStr}/{endStr} (elapsed/estimated)";
                }
            }
        }

        public override void ClearMemory()
        {
            options.Clear();
            waitingForResults = false;
            instance = null!;
            threadmaster?.Abort("clearing memory");
        }
    }
}
