using System.Linq;
using FinderMod.Search;
using Menu;
using Menu.Remix.MixedUI;
using UnityEngine;
using static FinderMod.OpUtil;

namespace FinderMod.Tabs
{
    internal class ValuesTab : BaseTab
    {
        private readonly OpComboBox2 searchItems;
        private readonly OpTextBox inputId;
        private readonly OpScrollBox outputBox;

        public ValuesTab(FinderProcess owner, int pageIndex) : base(owner, "VALUES", pageIndex)
        {
            // Background rect
            subObjects.Add(new RoundedRect(owner, this, basePos, baseSize, true)
            {
                borderColor = Menu.Menu.MenuColor(Menu.Menu.MenuColors.MediumGrey)
            });
            
            // Normal items
            searchItems = new OpComboBox2(
                    CosmeticBind(""), new(basePos.x + 10f, basePos.y + baseSize.y - 37f), 400f,
                    [.. OptionRegistry.ListOptions().Select(s => new ListItem(s))])
                { listHeight = 24 };
            inputId = new OpTextBox(CosmeticBind(0), new(searchItems.pos.x + searchItems.size.x + 40f, searchItems.pos.y), 100f) { allowSpace = true };
            outputBox = new OpScrollBox(basePos + new Vector2(10f, 10f), baseSize - new Vector2(20f, 60f), 30f, false, true);

            AddItems(
                new OpLabel(searchItems.pos.x + searchItems.size.x + 20f, searchItems.pos.y, "ID:"),
                inputId,
                outputBox,
                searchItems
            );
            outputBox.AddItems(new OpLabel(10f, outputBox.size.y - 30f, "Select an item from the dropdown"));

            searchItems.OnValueChanged += UpdateValues;
            inputId.OnValueUpdate += UpdateValues;
            
            // Default selectable
            defaultSelectable = WrapperFor(inputId);
        }

        private void UpdateValues(UIconfig _, string value, string oldValue)
        {
            if (value != oldValue) UpdateOutputBox();
        }

        private void UpdateOutputBox()
        {
            const float LINE_HEIGHT = 15f; // line height of OpLabelLong when bigText is false
            const float WHITESPACE_HEIGHT = 10f;

            // Remove old
            foreach (UIelement element in outputBox.items)
            {
                element.Deactivate();
                RemoveItems(element);
            }
            outputBox.items.Clear();
            outputBox.SetContentSize(0);

            // Add new
            string optionName = searchItems.value;
            if (OptionRegistry.TryGetOption(optionName, out var option))
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

        protected override void InternalUpdate()
        {
        }
    }
}
