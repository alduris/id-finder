using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FinderMod.Search;
using Menu.Remix;
using Menu.Remix.MixedUI;
using Menu.Remix.MixedUI.ValueTypes;
using RWCustom;
using UnityEngine;
using static FinderMod.OpUtil;

namespace FinderMod.Tabs
{
    internal class SearchTab : FinderTab
    {
        public SearchTab(OptionInterface owner) : base(owner, "Search")
        {
            queries = new List<PreQuery>();
        }
        
        private OpScrollBox cont_queries;
        private OpScrollBox cont_results;
        private OpLabel label_progress;
        private List<PreQuery> queries;
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
                new List<ListItem>(SearchOptions.Groups.Keys.ToArray()
                    .Where(s => ModManager.MSC || !SearchOptions.Groups[s].MSC)
                    .Select(s => new ListItem(s))
                )
            );
            var button_add = new OpSimpleButton(new(280f, 520f), new(80f, 24f), "ADD") { description = "Add an item to search for" };

            cont_queries = new OpScrollBox(new(10f, 270f), new(580f, 240f), 0f, false, true, true) { contentSize = 100f };

            var input_min = new OpTextBox(CosmeticBind(0), new(50f, 236f), 100f) { description = "Start of search range" };
            var input_max = new OpTextBox(CosmeticBind(100000), new(185f, 236f), 100f) { description = "End of search range" };
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

                if (SearchOptions.Groups.TryGetValue(value, out var setup))
                {
                    int count = SearchOptions.GetNumOutputs(setup.Inputs);
                    int[] biasArr = new int[setup.Inputs.Length];
                    for (int i = 0; i < biasArr.Length; i++) biasArr[i] = 1;
                    queries.Add(new PreQuery
                    {
                        Name = value,
                        Requests = new float?[count],
                        Biases = biasArr,
                        Setup = setup,
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

                List<Query> arrQueries = new();
                foreach (PreQuery prequery in queries)
                {
                    if (prequery.Linked)
                    {
                        Query query = arrQueries.Last();

                        // Add setup
                        Setup[] newSetups = new Setup[query.Setups.Length + 1];
                        for (int i = 0; i < query.Setups.Length; i++)
                        {
                            newSetups[i] = query.Setups[i];
                        }
                        newSetups[newSetups.Length - 1] = prequery.Setup;

                        query.Setups = newSetups;

                        // Add request
                        float?[][] newRequests = new float?[query.Requests.Length + 1][];
                        int[][] newBiases = new int[query.Biases.Length + 1][];
                        for (int i = 0; i < query.Requests.Length; i++)
                        {
                            newRequests[i] = query.Requests[i];
                        }
                        for (int i = 0; i < query.Biases.Length; i++)
                        {
                            newBiases[i] = query.Biases[i];
                        }
                        newRequests[newRequests.Length - 1] = prequery.Requests;
                        newBiases[newBiases.Length - 1] = prequery.Biases;

                        query.Requests = newRequests;
                        query.Biases = newBiases;
                    }
                    else
                    {
                        arrQueries.Add(new Query
                        {
                            Name = prequery.Name,
                            Setups = new Setup[] { prequery.Setup },
                            Requests = new float?[][] { prequery.Requests },
                            Biases = new int[][] { prequery.Biases }
                        });
                    }
                }
                startTime = DateTime.Now;
                SearchUtil.Search(arrQueries.ToArray(), range, threads, resultsPer);
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

        private void UpdateQueryBox()
        {
            const int SLIDER_WIDTH = 160;
            const float SHORT_INPUT_WIDTH = 60f;
            const float LINE_HEIGHT = 30f; // must be at minimum 24f
            const float WHITESPACE_AMOUNT = 10f;
            const float BIAS_INPUT_WIDTH = 60f;
            const float BIAS_TEXT_WIDTH = 40f;
            float maxWidth = cont_queries.size.x - 25f; // wrap 10f from slider + 15f slider width. also a constant but I can't make it const

            float y = cont_queries.size.y; // start from top of scroll box
            float height = 10f; // 10f bottom padding accounting
            List<UIelement> items = new();

            foreach (UIelement element in cont_queries.items)
            {
                element.Deactivate();
                element.tab.items.Remove(element);
            }
            cont_queries.items.Clear();
            cont_queries.SetContentSize(0);

            foreach (PreQuery query in queries)
            {
                bool first = ReferenceEquals(queries[0], query);

                // Move to next line
                height += LINE_HEIGHT + 10f; // 10f top padding
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
                int currIndex = 0;
                for (int i = 0; i < query.Setup.Inputs.Count(); i++)
                {
                    SearchInput item = query.Setup.Inputs[i];
                    if (item.Type == InputType.Whitespace)
                    {
                        y -= WHITESPACE_AMOUNT;
                        height += WHITESPACE_AMOUNT;
                        continue;
                    }
                    else if (item.Type == InputType.Label)
                    {
                        y -= LINE_HEIGHT;
                        height += LINE_HEIGHT;
                        items.Add(new OpLabel(25f, y, item.Name));
                        continue;
                    }

                    // Adjust index for future reference (rgb color has 3 inputs)
                    int setIndex = currIndex;
                    if (item.Type == InputType.ColorRGB)
                    {
                        currIndex += 3;
                    }
                    else
                    {
                        currIndex++;
                    }

                    // Make room for new row
                    height += LINE_HEIGHT;
                    y -= LINE_HEIGHT;

                    // Create use checkbox
                    var checkbox_enabled = new OpCheckBox(CosmeticBind(query.Requests[setIndex] is not null), new(25f, y));
                    checkbox_enabled.OnValueUpdate += (_, t, f) =>
                    {
                        query.Requests[setIndex] = (checkbox_enabled.GetValueBool() ? 0f : null);
                        UpdateQueryBox();
                    };

                    // Create label
                    var label = new OpLabel(55, y, query.Setup.Inputs[i].Name);
                    var labelWidth = label.size.x;

                    // Add items
                    items.Add(checkbox_enabled);
                    items.Add(label);

                    // Skip creating input if it will be unused to prevent confusion
                    if (query.Requests[setIndex] is null)
                    {
                        continue;
                    }

                    // Create the input
                    float biasStartX = 0f;
                    if (item.Type == InputType.Boolean)
                    {
                        // Special case for booleans (a button that changes)
                        bool enabled = (query.Requests[setIndex] == 1f);
                        OpSimpleButton button = new(new(label.pos.x + labelWidth + 6f, y), new(40f, 24f), enabled ? "Yes" : "No")
                        {
                            description = item.Name + (item.Description != null ? " (" + item.Description + ")" : "")
                        };
                        button.OnClick += _ =>
                        {
                            enabled = !enabled;
                            query.Requests[setIndex] = (enabled ? 1f : 0f);
                            button.text = (enabled ? "Yes" : "No");
                        };

                        biasStartX = button.pos.x + button.size.x + 16f;

                        items.Add(button);
                    }
                    else
                    {
                        // Create input like normal
                        UIconfig input;
                        switch (item.Type)
                        {
                            default:
                            case InputType.Float:
                            case InputType.Hue:
                                {
                                    // input = new OpFloatSlider(CosmeticFloat(query.Requests[setIndex] ?? 0f, item.Range.Item1, item.Range.Item2), new(label.pos.x + labelWidth + 6f, y), SLIDER_WIDTH, 4);
                                    input = new OpFloatSlider(CosmeticBind(query.Requests[setIndex] ?? 0f), new(label.pos.x + labelWidth + 6f, y - 4f), SLIDER_WIDTH, 4)
                                    { min = item.Range.Item1, max = item.Range.Item2, description = item.Name };

                                    if (item.Type == InputType.Hue)
                                    {
                                        (input as OpFloatSlider).colorEdge = Custom.HSL2RGB((input as OpFloatSlider).GetValueFloat(), 1f, 0.625f);
                                        (input as OpFloatSlider).colorFill = Custom.HSL2RGB((input as OpFloatSlider).GetValueFloat(), 1f, 0.625f);
                                        input.OnValueUpdate += (_, _, _) =>
                                        {
                                            (input as OpFloatSlider).colorEdge = Custom.HSL2RGB((input as OpFloatSlider).GetValueFloat(), 1f, 0.5f);
                                            (input as OpFloatSlider).colorFill = Custom.HSL2RGB((input as OpFloatSlider).GetValueFloat(), 1f, 0.5f);
                                        };
                                    }
                                    break;
                                }
                            case InputType.Integer:
                                {
                                    input = new OpSliderTick(
                                        CosmeticInt((int)(query.Requests[setIndex] ?? 0),
                                        (int)item.Range.Item1, (int)item.Range.Item2),
                                        new(label.pos.x + labelWidth + 6f, y - 4f),
                                        SLIDER_WIDTH);
                                    //input = new OpSliderTick(CosmeticBind((int)(query.Requests[setIndex] ?? 0)), new(label.pos.x + labelWidth + 6f, y - 4f), SLIDER_WIDTH)
                                    //    { min = (int)item.Range.Item1, max = (int)item.Range.Item2, description = item.Name };
                                    break;
                                }
                            case InputType.Boolean:
                                {
                                    // Technically this is unreachable code but keeping it for reference
                                    input = new OpCheckBox(CosmeticBind(query.Requests[setIndex] == 1f), new(label.pos.x + labelWidth + 6f, y));
                                    break;
                                }
                            case InputType.ColorRGB:
                                {
                                    // Figure out color to put in there (RGB vs HSL)
                                    Color color;
                                    color = new Color(query.Requests[setIndex] ?? 0.5f, query.Requests[setIndex + 1] ?? 0.5f, query.Requests[setIndex + 2] ?? 0.5f);
                                    input = new OpColorPicker(CosmeticBind(color), new(55f, y));

                                    // Make room for the color input
                                    y -= input.size.y;
                                    height += input.size.y;
                                    input.pos = new(input.pos.x, input.pos.y - input.size.y);
                                    break;
                                }
                            case InputType.MultiChoice:
                                {
                                    // Add options
                                    List<string> choices = new();
                                    for(int k = 1; k <= (int)(item.Range.Item2 - item.Range.Item1) + 1; k++)
                                    {
                                        choices.Add(k.ToString());
                                    }
                                    
                                    // Make sure value is within bounds (initially it might not be)
                                    if (query.Requests[setIndex] < item.Range.Item1)
                                        query.Requests[setIndex] = item.Range.Item1;

                                    // Continue!
                                    input = new OpComboBox(
                                        CosmeticBind(query.Requests[setIndex].ToString()),
                                        new(label.pos.x + labelWidth + 6f, y),
                                        SHORT_INPUT_WIDTH,
                                        choices.ToArray()
                                    );
                                    break;
                                }
                        }
                        input.description = item.Name + (item.Description != null ? " (" + item.Description + ")" : "");

                        //var input = new OpTextBox(OpUtil.CosmeticBind(query.Requests[i]?.ToString() ?? ""), new(x, y), inputWidth) { description = query.Setup.Names[i].Name };
                        input.OnValueChanged += (_, value, oldValue) =>
                        {
                            if (query.Requests[setIndex] is null || input.greyedOut)
                            {
                                return;
                            }

                            // Turn into an actual number
                            float num = 0f;
                            if (input is OpFloatSlider)
                            {
                                num = (input as OpFloatSlider).GetValueFloat();
                            }
                            else if (input is OpSliderTick)
                            {
                                num = (input as OpSliderTick).GetValueFloat();
                            }
                            else if (input is OpCheckBox)
                            {
                                // Again, unreachable code but oh well :P (was for boolean inputs)
                                num = (input as OpCheckBox).GetValueBool() ? 1f : 0f;
                            }
                            else if (input is OpColorPicker picker)
                            {
                                if (item.Type == InputType.ColorRGB)
                                {
                                    query.Requests[setIndex] = picker.valueColor.r;
                                    query.Requests[setIndex + 1] = picker.valueColor.g;
                                    query.Requests[setIndex + 2] = picker.valueColor.b;
                                }
                                else
                                {
                                    Vector3 hsl = Custom.RGB2HSL(picker.valueColor);
                                    query.Requests[setIndex] = hsl.x;
                                    query.Requests[setIndex + 1] = hsl.y;
                                    query.Requests[setIndex + 2] = hsl.z;
                                }
                                return;
                            }
                            else if (input is OpComboBox)
                            {
                                if (int.TryParse(input.value, out int n))
                                {
                                    if (n < item.Range.Item1)
                                    {
                                        num = item.Range.Item1;
                                    }
                                    else if (n > item.Range.Item2)
                                    {
                                        num = item.Range.Item2;
                                    }
                                    else
                                    {
                                        num = n;
                                    }
                                }
                                else
                                {
                                    input.value = query.Requests[setIndex].ToString();
                                    return;
                                }
                            }

                            // Update value
                            query.Requests[setIndex] = num;
                        };

                        // Add input to box
                        items.Add(input);
                        biasStartX = input.pos.x + input.size.x + 16f;
                    }

                    // Slap a multiplier input onto that thing
                    // biasStartX = 590f - BIAS_TEXT_WIDTH - BIAS_INPUT_WIDTH;
                    OpUpdown input_bias = new(CosmeticInt(query.Biases[i], 1, 9999), new(biasStartX + BIAS_TEXT_WIDTH, y - 4f), BIAS_INPUT_WIDTH) { description = "Bias" };
                    int j = i;
                    input_bias.OnValueChanged += (_, _, _) =>
                    {
                        try
                        {
                            if (input_bias.valueInt < 1) input_bias.valueInt = 1;
                        }
                        catch
                        {
                            input_bias.valueInt = 1;
                        }
                        finally
                        {
                            query.Biases[j] = input_bias.valueInt;
                        }
                    };

                    items.Add(new OpLabel(biasStartX, y, "Bias:"));
                    items.Add(input_bias);
                }

                // Add items
                cont_queries.AddItems(items.ToArray());
            }

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
