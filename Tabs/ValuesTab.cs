using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FinderMod.Search;
using Menu.Remix.MixedUI;
using static FinderMod.OpUtil;

namespace FinderMod.Tabs
{
    internal class ValuesTab : FinderTab
    {
        public ValuesTab(OptionInterface option) : base(option, "Values")
        {
        }

        private OpComboBox searchItems;
        private OpTextBox inputId;
        private OpScrollBox outputBox;

        public override void Initialize()
        {
            searchItems = new OpComboBox(
                CosmeticBind(""), new(10f, 510f), 250f,
                new List<ListItem>(SearchOptions.Groups.Keys.ToArray()
                    .Where(s => ModManager.MSC || !SearchOptions.Groups[s].MSC)
                    .Select(s => new ListItem(s))
                )
            );
            inputId = new OpTextBox(CosmeticBind<int>(0), new(10f + searchItems.size.x + 40f, searchItems.pos.y), 100f);
            outputBox = new OpScrollBox(new(10f, 10f), new(580f, 480f), 30f, false, true, true);

            AddItems(
                new OpLabel(10f, 560f, "Values", true),
                new OpLabel(10f + searchItems.size.x + 20f, searchItems.pos.y, "ID:"),
                inputId,
                outputBox,
                searchItems
            );
            outputBox.AddItems(new OpLabel(10f, outputBox.size.y - 30f, "Select an item from the dropdown"));

            searchItems.OnValueChanged += (_, _, _) => UpdateOutputBox();
            inputId.OnValueUpdate += (_, _, _) => UpdateOutputBox();
        }

        private void UpdateOutputBox()
        {
            const float LINE_HEIGHT = 15f; // line height of OpLabelLong when bigText is false
            const float WHITESPACE_HEIGHT = 10f;

            foreach (var element in outputBox.items)
            {
                element.Deactivate();
                element.tab.items.Remove(element);
            }
            outputBox.items.Clear();

            if (!SearchOptions.Groups.ContainsKey(searchItems.value) || inputId.value == "")
            {
                return;
            }

            // Get values
            Setup setup = SearchOptions.Groups[searchItems.value];
            int seed = inputId.valueInt;
            float y = outputBox.size.y - 10f - LINE_HEIGHT;
            float height = 20f;

            // Count how many random values we need to get (guaranteed at least 9 floats, personality requires it)
            int floatCount = Math.Max(9, setup.MinFloats);
            int rangeCount = setup.FloatRanges?.GetLength(0) ?? 0;
            int intRangeCount = setup.IntRanges?.GetLength(0) ?? 0;
            float[] floats = new float[floatCount + rangeCount + intRangeCount];
            float[] personality = new float[6];
            int frStart = floatCount;
            int irStart = floatCount + rangeCount;

            // Get the random values
            SearchUtil.GetRandomFloats(seed, floatCount, floats);
            SearchUtil.FillPersonality(floats, personality);
            if (rangeCount > 0)
            {
                SearchUtil.GetRandomRanges(seed, setup.FloatRanges, floats, floatCount);
            }
            if (intRangeCount > 0)
            {
                SearchUtil.GetRandomRanges(seed, setup.IntRanges, floats, floatCount + rangeCount);
            }
            float[] values = setup.Apply(floats, personality, seed, frStart, irStart);

            // Build the output string
            int valuePtr = 0;
            for (int i = 0; i < setup.Inputs.Length; i++)
            {
                InputType type = setup.Inputs[i].Type;
                string s;

                // Deal with whitespace stuff
                if (type == InputType.Whitespace || setup.Inputs[i].Name is null)
                {
                    y -= WHITESPACE_HEIGHT;
                    height += WHITESPACE_HEIGHT;
                    continue;
                }

                // Deal with labels too I guess
                if (type == InputType.Label)
                {
                    s = setup.Inputs[i].Name;
                }
                else
                {
                    // Add value(s)
                    s = setup.Inputs[i].Name + ": " + type switch
                    {
                        InputType.MultiChoice => 1 + (int)(values[valuePtr++] - setup.Inputs[i].Range.Item1),
                        // InputType.Integer => (int)values[valuePtr++],
                        InputType.Boolean => values[valuePtr++] == 1f ? "Yes" : "No",
                        InputType.ColorRGB => "rgb(" + values[valuePtr++] + ", " + values[valuePtr++] + ", " + values[valuePtr++] + ")",
                        _ => values[valuePtr++],
                    };
                }

                // Append items
                var label = new OpLabel(10f, y, s);
                if (setup.Inputs[i].Description is not null) label.description = setup.Inputs[i].Description;
                outputBox.AddItems(label);
                y -= LINE_HEIGHT;
                height += LINE_HEIGHT;
            }

            outputBox.SetContentSize(height, true);
        }

        public override void Update()
        {
            // throw new NotImplementedException();
        }
    }
}
