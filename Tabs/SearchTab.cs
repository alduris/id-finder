using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FinderMod.Inputs;
using FinderMod.Search;
using Menu.Remix;
using Menu.Remix.MixedUI;
using Menu.Remix.MixedUI.ValueTypes;
using RWCustom;
using UnityEngine;
using static FinderMod.OpUtil;

namespace FinderMod.Tabs
{
    internal class SearchTab(OptionInterface owner) : BaseTab(owner, "Search")
    {
        private OpScrollBox cont_queries;
        private OpScrollBox cont_results;
        private OpLabel label_progress;
        private List<NewQuery> queries = [];
        private DateTime startTime;

        // private Query[] currRequest;
        private bool waitingForResults = false;

        public override void Initialize()
        {
            queries.Clear();

            // Get max number of threads we can use
            ThreadPool.GetMaxThreads(out int maxThreads, out _);

            // Initialize elements we need
            var combo_allOpts = new OpComboBox(
                CosmeticBind(""), new(10f, 520f), 250f,
                new List<ListItem>(SearchOptions.Options.Keys.ToArray()
                    .Where(s => ModManager.MSC || !SearchOptions.Options[s].MSC)
                    .Select(s => new ListItem(s))
                )
            );
            var button_add = new OpSimpleButton(new(280f, 520f), new(80f, 24f), "ADD") { description = "Add an item to search for" };

            cont_queries = new OpScrollBox(new(10f, 270f), new(580f, 240f), 0f, false, true, true) { contentSize = 100f };

            var input_min = new OpTextBox(CosmeticBind(0), new(50f, 236f), 100f) { description = "Start of search range", allowSpace = true };
            var input_max = new OpTextBox(CosmeticBind(100000), new(185f, 236f), 100f) { description = "End of search range", allowSpace = true };
            var input_find = new OpTextBox(CosmeticBind(1), new(60f, 206f), 60f) { description = "Number of ids to find per result (1-20)" };
            var input_threads = new OpTextBox(CosmeticBind(2), new(190f, 206f), 60f) { description = "Number of threads to use" };

            var button_run = new OpSimpleButton(new(510f, 216f), new(80f, 24f), "SEARCH") { description = "Start the search!", colorEdge = color_start };

            cont_results = new OpScrollBox(new(10f, 10f), new(580f, 160f), 0f, false, true, true);

            // Set up event listeners
            button_add.OnClick += _ =>
            {
                string value = combo_allOpts.value;
                combo_allOpts.value = null;

                if (waitingForResults) return;

                if (SearchOptions.Options.TryGetValue(value, out var option))
                {
                    int count = option.NumOutputs;
                    float[] biasArr = new float[count];
                    for (int i = 0; i < biasArr.Length; i++) biasArr[i] = 1;
                    queries.Add(new NewQuery
                    {
                        Name = value,
                        Requests = new float?[count],
                        Biases = biasArr,
                        Linked = false
                    });
                    UpdateQueryBox();
                }
            };

            button_run.OnClick += _ =>
            {
                if (waitingForResults || queries.Count == 0) return;

                // Set up and validate search request
                (int, int) range = (input_min.valueInt, input_max.valueInt);
                int threads = input_threads.valueInt;
                int resultsPer = input_find.valueInt;

                // Deactivate all the items and stuff
                waitingForResults = true;
                foreach (var item in cont_queries.items)
                {
                    if (item is UIfocusable)
                    {
                        (item as UIfocusable).greyedOut = true;
                    }
                }

                // Add searching text into results scroll box
                foreach (UIelement element in cont_results.items)
                {
                    element.Deactivate();
                    element.tab.items.Remove(element);
                }
                cont_results.items.Clear();
                cont_results.SetContentSize(0f, true);

                var label_searching = new OpLabel(10f, cont_results.size.y - 40f, "SEARCHING...", true);
                label_progress = new OpLabel(10f, cont_results.size.y - 70f, "0.00% complete", false);
                var button_abort = new OpSimpleButton(new(10f, cont_results.size.y - 100f), new(80f, 24f), "ABORT")
                { description = "Aborts the search and return the current results", colorEdge = color_del, colorFill = color_del };
                button_abort.OnClick += _ =>
                {
                    SearchUtil.Abort("user");
                };
                cont_results.AddItems(label_searching, label_progress, button_abort);
                cont_results.SetContentSize(80f, true);

                startTime = DateTime.Now;
                SearchUtil.Search([.. queries], range, threads, resultsPer);
            };

            input_threads.OnValueChanged += (_, _, old) =>
            {
                try
                {
                    // Correct value if invalid (less than 1)
                    int val = input_threads.valueInt;
                    if (val < 1) input_threads.valueInt = 1;
                    else if (val > maxThreads) input_threads.valueInt = maxThreads;
                }
                catch
                {
                    // Called if input is left blank
                    input_threads.value = old;
                }
            };
            input_find.OnValueChanged += (_, _, old) =>
            {
                try
                {
                    // Correct value if invalid (less than 1, more than 20)
                    int val = input_find.valueInt;
                    if (val < 1) input_find.valueInt = 1;
                    else if (val > 20) input_find.valueInt = 20;
                }
                catch
                {
                    // Called if input is left blank
                    input_find.value = old;
                }
            };

            // Add stuff to tab
            var UIArrPlayerOptions = new UIelement[]
            {
                new OpLabel(10f, 570f, "Input", true),
                new OpLabel(10f, 550f, "WARNING: do not leave this tab while searching for ids.", false) { color = color_warn },
                cont_queries,
                new OpLabel(10f, 236f, "From:"),
                input_min,
                new OpLabel(160f, 236f, "To:"),
                input_max,
                new OpLabel(10f, 206f, "Results:"),
                input_find,
                new OpLabel(135f, 206f, "Threads:"),
                input_threads,
                button_run,
                new OpLabel(10f, 176f, "Output", true),
                cont_results,
                combo_allOpts, button_add // For z-index ordering (I hope that's how this works at least)
            };
            AddItems(UIArrPlayerOptions);
        }

        // TESTING REMOVE
        /*private readonly BaseInput[] testInputs =
        [
            new Label("label"),
            new FloatInput("float", 0f, 2f),
            new IntInput("int", 0, 5),
            new BoolInput("bool"),
            new ColorRGBInput("rgb"),
            new Whitespace(),
            new InputGroup("group",
            [
                new FloatInput("inp1"),
                new FloatInput("inp2"),
                new FloatInput("inp3")
            ]),
            new Label("label 2"),
            new ColorHSLInput("hsl"),
        ];*/

        private void UpdateQueryBox()
        {
            const int SLIDER_WIDTH = 160;
            const float SHORT_INPUT_WIDTH = 60f;
            const float LINE_HEIGHT = 30f; // must be at minimum 24f
            const float WHITESPACE_AMOUNT = 10f;
            const float BIAS_INPUT_WIDTH = 60f;
            const float BIAS_TEXT_WIDTH = 40f;
            const float LEFT_MARGIN = 25f;
            const float LABEL_OFFSET = LEFT_MARGIN + 30f;
            float maxWidth = cont_queries.size.x - 25f; // wrap 10f from slider + 15f slider width. also a constant but I can't make it const

            float y = cont_queries.size.y; // start from top of scroll box
            // float height = 10f; // 10f bottom padding accounting
            List<UIelement> items = new();

            foreach (UIelement element in cont_queries.items)
            {
                element.Deactivate();
                element.tab.items.Remove(element);
            }
            cont_queries.items.Clear();
            cont_queries.SetContentSize(0);

            foreach (NewQuery query in queries)
            {
                bool first = ReferenceEquals(queries[0], query);

                // Move to next line
                // height += LINE_HEIGHT + 10f; // 10f top padding
                y -= LINE_HEIGHT + 10f;      // ditto

                // Create delete button and label
                // Delete button doesn't show if linked
                if (!query.Linked)
                {
                    var button_delete = new OpSimpleButton(new(10f, y), new(24, 24), "\xd7")
                    {
                        colorEdge = color_del,
                        colorFill = color_del,
                        description = "Delete search query"
                    };
                    button_delete.OnClick += _ => {
                        if (waitingForResults) return;
                        int i = queries.IndexOf(query);
                        queries.Remove(query);
                        if (queries.Count > i && queries[i].Linked)
                        {
                            queries[i].Linked = false;
                        }
                        UpdateQueryBox();
                    };
                    items.Add(button_delete);
                }

                // Create link button
                var button_link = new OpSimpleButton(new(40f, y), new(24, 24), query.Linked ? "\u2212" : "+")
                {
                    colorEdge = color_link,
                    colorFill = color_link,
                    description = (query.Linked ? "Unlink" : "Link") + " search query with above",
                    greyedOut = first
                };
                button_link.OnClick += _ => {
                    if (first || waitingForResults) return;
                    query.Linked = !query.Linked;
                    UpdateQueryBox();
                };
                items.Add(button_link);

                var label_name = new OpLabel(70f, y, query.Name, true); // 70f = 10f (left padding) + 2 * 24f (button width) + 2 * 6f (margin)
                items.Add(label_name);

                // Create input row
                var inputs = SearchOptions.Options[query.Name].CreateInputs();
                y -= LINE_HEIGHT;
                int ptr = 0;

                foreach (var input in inputs)
                {
                    List<float> floats = [];
                    if (query.Requests[ptr] != null)
                    {
                        for (int i = 0; i < input.ValueCount; i++)
                        {
                            floats.Add(query.Requests[ptr].GetValueOrDefault(0f));
                        }
                    }
                    input.SetValues(query.Requests[ptr] != null, floats); // todo: better way for checking enabled

                    input.AddUI(LEFT_MARGIN, ref y, items, UpdateQueryBox);
                }

                /*foreach (var input in testInputs)
                {
                    input.AddUI(LEFT_MARGIN, ref y, items, UpdateQueryBox);
                }*/

                // Add items
                items.Reverse();
                cont_queries.AddItems([.. items]);
            }

            float height = cont_queries.size.y - y + 10f * queries.Count;
            cont_queries.SetContentSize(height, true);
        }

        public override void Update()
        {

            if (waitingForResults)
            {
                if (SearchUtil.done)
                {
                    waitingForResults = false;

                    // Remove old elements
                    foreach (UIelement element in cont_results.items)
                    {
                        element.Deactivate();
                        element.tab.items.Remove(element);
                    }
                    cont_results.items.Clear();
                    cont_results.SetContentSize(0f, true);

                    // Get some info
                    bool aborted = SearchUtil.abort;
                    int resultsPer = SearchUtil.finalResults.GetLength(1);

                    // Add label
                    OpLabelLong label = new(new(10f, 10f), new(cont_results.size.x - 20f, 0), "", true, FLabelAlignment.Left);
                    float labelSize;
                    if (resultsPer > 1)
                    {
                        // Label size accounts for:     every single id plus extra lines for "request #"s          extra line if aborted
                        labelSize = label.LineHeight * (SearchUtil.finalResults.GetLength(0) * (resultsPer + 1) + (aborted ? 1f : 0f) + 1);
                    }
                    else
                    {
                        // Label size accounts for:     number of queries                       extra line if aborted
                        labelSize = label.LineHeight * (SearchUtil.finalResults.GetLength(0) + (aborted ? 1f : 0f) + 1);
                    }
                    if (aborted) label.text += "OPERATION ABORTED EARLY" + (SearchUtil.abortReason is not null ? $" ({SearchUtil.abortReason})" : "") + "\n";
                    for (int i = 0; i < SearchUtil.finalResults.GetLength(0); i++)
                    {
                        if (resultsPer > 1)
                        {
                            label.text += $"Request {i + 1}:\n";
                            // Get every result and put it in a line
                            for (int j = 0; j < resultsPer; j++)
                            {
                                var res = SearchUtil.finalResults[i, j];
                                label.text += $"        {res.Item1} (distance: {res.Item2})\n";
                            }
                        }
                        else
                        {
                            // Just get the single result and put it in the same line
                            var res = SearchUtil.finalResults[i, 0];
                            label.text += $"Request {i + 1}: {res.Item1} (distance: {res.Item2})\n";
                        }
                    }
                    label.text = label.text.Substring(0, label.text.Length - 1); // get rid of last \n
                    label.size = new(cont_results.size.x - 20f, labelSize);
                    label.pos = new(10f, cont_results.size.y - 10f - labelSize);

                    // Copy results button
                    var button_copy = new OpSimpleButton(new(10f, cont_results.size.y - labelSize - 30f), new(48f, 24f), "COPY") { description = "Copy results" };
                    button_copy.OnClick += (_) =>
                    {
                        UniClipboard.SetText(label.text);
                        //string shortText = label.text.Substring(0, Math.Min(label.text.Length, 20));
                        ConfigContainer.instance.CfgMenu.ShowAlert(OptionalText.GetText(OptionalText.ID.ConfigContainer_AlertCopyCosmetic).Replace("<Text>", "results"));
                    };

                    // Set the scroll box size
                    cont_results.AddItems(label, button_copy);
                    cont_results.SetContentSize(labelSize + 40f, true);

                    // Reupdate query box to reenable everything
                    UpdateQueryBox();
                }
                else
                {
                    // Update progress thingy
                    double min = SearchUtil.progress.Length > 0 ? SearchUtil.progress.Min() : 0;
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
            queries.Clear();
            waitingForResults = false;
            SearchUtil.Abort("clearing memory");
        }
    }
}
