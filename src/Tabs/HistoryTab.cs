using System;
using System.Linq;
using FinderMod.Search;
using Menu.Remix;
using Menu.Remix.MixedUI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using static Menu.Menu;
using HistoryItem = FinderMod.Search.HistoryManager.HistoryItem;

namespace FinderMod.Tabs
{
    internal class HistoryTab : BaseTab
    {
        private bool dirty;
        private OpScrollBox box = null!;
        private HistoryItem? activeItem = null;
        private bool shownTooLargeWarning = false;

        public HistoryTab(Options options) : base(options, Translate("History"))
        {
            HistoryManager.UpdateHistory += MarkDirty;
        }

        private void MarkDirty()
        {
            dirty = true;
        }

        public override void Initialize()
        {
            dirty = true;
            box = new OpScrollBox(this, 0f, false, true);
        }

        public override void Update()
        {
            if (dirty && box != null)
            {
                dirty = false;
                var oldScrollOffset = box.scrollOffset;

                // Remove old elements
                foreach (UIelement element in box.items)
                {
                    element.Deactivate();
                    _RemoveItem(element);
                }
                box.items.Clear();
                box.SetContentSize(0f, true);

                // Header
                box.AddItems(
                    new OpLabel(10f, 560f, Translate("HISTORY"), true),
                    new OpLabel(10f, 530f, Translate("Manage past searches here. Hover over a button to see its description at the bottom."), false),
                    new OpImage(new Vector2(10f, 522f), "pixel") { scale = new Vector2(580f, 2f), color = MenuRGB(MenuColors.MediumGrey) } // 6px margin surrounding
                );

                // Items
                float y = 518f;

                var history = HistoryManager.GetHistory();
                if (history.Count > 0)
                {
                    foreach (var item in history)
                    {
                        bool active = activeItem.HasValue && activeItem.Value == item;

                        // Normal UI stuff
                        y -= 30f;
                        var detailsButton = new OpSimpleImageButton(new Vector2(10f, y), new Vector2(24, 24f), "Menu_InfoI") { description = Translate(!active ? "Expand results" : "Retract results") };
                        var nameInput = new OpTextBox(OpUtil.CosmeticBind(item.name), new Vector2(40f, y), 450f) { description = Translate("Rename search") };
                        var copyButton = new OpSimpleImageButton(new Vector2(496f, y), new Vector2(24f, 24f), "keyShiftB") { description = Translate("Copy") };
                        var restoreButton = new OpSimpleImageButton(new Vector2(526f, y), new Vector2(24f, 24f), "Menu_Symbol_Repeats") { description = Translate("Restore") };
                        var deleteButtom = new OpSimpleImageButton(new Vector2(556f, y), new Vector2(24f, 24f), "Menu_Symbol_Clear_All") { description = Translate("Delete"), colorEdge = OpUtil.RedColor };

                        box.AddItems(detailsButton, nameInput, copyButton, restoreButton, deleteButtom);

                        detailsButton.OnClick += (_) => DetailsButton_OnClick(item);
                        nameInput.OnValueChangedFix += (_, _, old) => NameInput_OnValueChanged(nameInput, old, item);
                        copyButton.OnClick += (_) => CopyButton_OnClick(item);
                        restoreButton.OnClick += (_) => RestoreButton_OnClick(item);
                        deleteButtom.OnClick += (_) => DeleteButtom_OnClick(item);

                        // Values if active
                        if (active)
                        {
                            y -= 30f;
                            box.AddItems(new OpLabel(40f, y, $"Searched at {item.date.ToLocalTime():G}, from {item.min} to {item.max}"));
                            y -= 10f;
                            try
                            {
                                var options = item.GetOptions().ToList();
                                for (int i = 0; i < item.results.Length; i++)
                                {
                                    if (item.results.Length > 1)
                                    {
                                        y -= 30f;
                                        box.AddItems(new OpLabel(40f, y, $"Result {i + 1}:", true));
                                        y -= 10f;
                                    }

                                    // Option values
                                    float y1 = y;
                                    bool first = true;
                                    for (int j = 0, k = 0; j < options.Count; j++)
                                    {
                                        if (j > 0 && !options[j].linked) k++;
                                        if (k == i)
                                        {
                                            if (!first) y1 -= 15f;
                                            else first = false;
                                            y1 -= 15f;
                                            box.AddItems(new OpLabel(40f, y1, options[j].name, false));
                                            foreach (var str in options[j].ToString().Split('\n'))
                                            {
                                                if (str == "") continue;
                                                y1 -= 15f;
                                                box.AddItems(new OpLabel(40f, y1, str, false));
                                            }
                                        }
                                    }

                                    // Results
                                    float y2 = y;
                                    var results = item.results[i];
                                    foreach (var result in results)
                                    {
                                        y2 -= 15f;
                                        box.AddItems(new OpLabel(310f, y2, $"{result.id} (distance: {result.dist})", false));
                                    }

                                    // Set y
                                    y = Mathf.Min(y1, y2);
                                }

                                // Vertical sidebar to show range
                                box.AddItems(new OpImage(new Vector2(22f, y), "pixel") { scale = new Vector2(2f, detailsButton.PosY - y), color = MenuRGB(MenuColors.MediumGrey) });
                            }
                            catch (Exception e)
                            {
                                // Now, theoretically if my code works the way it should, we should never get here. Alas I am too nervous about slip ups or whoopsies to be too sure.
                                Plugin.logger.LogError(e);
                                box.PlaySound(SoundID.HUD_Karma_Reinforce_Bump);
                            }
                        }
                    }
                }
                else
                {
                    y -= 30f;
                    box.AddItems(new OpLabel(10f, y, "Nothing in history yet!", false));
                }

                // Resize yippee
                box.SetContentSize(600f - y, true);

                if (600f - y > 10000f && !shownTooLargeWarning)
                {
                    shownTooLargeWarning = true;
                    ConfigContainer.instance.CfgMenu.ShowAlert(Translate("Scroll box exceeded size limit! Content may be cut off."));
                }

                // Also prevent flickering issue and scrolling weirdly
                foreach (var item in box.items)
                {
                    item.lastScreenPos = box.pos;
                }
                box.scrollOffset = oldScrollOffset; // and scroll back to where we were
            }
        }

        private void DetailsButton_OnClick(HistoryItem item)
        {
            if (activeItem.HasValue && activeItem.Value == item)
            {
                activeItem = null;
            }
            else
            {
                activeItem = item;
            }
            MarkDirty();
        }

        private void NameInput_OnValueChanged(OpTextBox self, string oldValue, HistoryItem item)
        {
            if (self.value.Trim() == "")
            {
                self.value = oldValue;
            }
            else if (self.value != oldValue && item.name != self.value)
            {
                HistoryManager.RenameHistoryItem(item, self.value);
            }
        }

        private void CopyButton_OnClick(HistoryItem item)
        {
            UniClipboard.SetText(JsonConvert.SerializeObject(item));
            ConfigContainer.instance.CfgMenu.ShowAlert(OptionalText.GetText(OptionalText.ID.ConfigContainer_AlertCopyCosmetic).Replace("<Text>", "history item"));
        }

        private void RestoreButton_OnClick(HistoryItem item)
        {
            item.RestoreSearch();
            ConfigContainer._ChangeActiveTab(0); // index of SearchTab
        }

        private void DeleteButtom_OnClick(HistoryItem item)
        {
            HistoryManager.RemoveHistoryItem(item);
            MarkDirty();
        }
    }
}
