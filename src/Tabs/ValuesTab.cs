using System;
using System.Collections.Generic;
using System.Linq;
using FinderMod.Search;
using Menu.Remix.MixedUI;
using static FinderMod.OpUtil;

namespace FinderMod.Tabs
{
    internal class ValuesTab(OptionInterface option) : BaseTab(option, "Values")
    {
        private OpComboBox2 searchItems = null!;
        private OpTextBox inputId = null!;
        private OpScrollBox outputBox = null!;

        public override void Initialize()
        {
            searchItems = new OpComboBox2(
                CosmeticBind(""), new(10f, 520f), 250f,
                new List<ListItem>(
                    OptionRegistry.ListOptions()
                    .Select(s => new ListItem(s))
                )
            )
            { listHeight = 20 };
            inputId = new OpTextBox(CosmeticBind(0), new(10f + searchItems.size.x + 40f, searchItems.pos.y), 100f) { allowSpace = true };
            outputBox = new OpScrollBox(new(10f, 10f), new(580f, 480f), 30f, false, true, true);

            AddItems(
                new OpLabel(10f, 560f, "Values", true),
                new OpLabel(10f + searchItems.size.x + 20f, searchItems.pos.y, "ID:"),
                inputId,
                outputBox,
                searchItems
            );
            outputBox.AddItems(new OpLabel(10f, outputBox.size.y - 30f, "Select an item from the dropdown"));

            searchItems.OnValueChanged += UpdateValues;
            inputId.OnValueUpdate += UpdateValues;
        }

        private void UpdateValues(UIconfig _, string value, string oldValue)
        {
            if (value != oldValue) UpdateOutputBox();
        }

        private void UpdateOutputBox()
        {
            const float LINE_HEIGHT = 15f; // line height of OpLabelLong when bigText is false
            const float WHITESPACE_HEIGHT = 10f;

            RemoveItems([.. outputBox.items]);

            var name = searchItems.value;
            if (OptionRegistry.TryGetOption(name, out var option))
            {
                int seed = inputId.valueInt;
                float y = outputBox.size.y - 10f;
                foreach (var str in option.GetValues(seed))
                {
                    if (str != null)
                    {
                        y -= LINE_HEIGHT;
                        var label = new OpLabel(10f, y, str);
                        outputBox.AddItems(label);
                        label.lastScreenPos = label.pos;
                    }
                    else
                    {
                        y -= WHITESPACE_HEIGHT;
                    }
                }
                outputBox.SetContentSize(outputBox.size.y - y + 10f, true);
            }
        }

        public override void Update()
        {
        }
    }
}
