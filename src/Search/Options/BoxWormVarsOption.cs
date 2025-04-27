using System;
using System.Collections.Generic;
using System.Text;
using FinderMod.Inputs;
using FinderMod.Tabs;
using Menu.Remix.MixedUI;
using Menu.Remix.MixedUI.ValueTypes;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace FinderMod.Search.Options
{
    public class BoxWormVarsOption : Option
    {
        private readonly IntInput NumLarvaeInput;
        private readonly FloatInput SizeVarInput;

        public BoxWormVarsOption()
        {
            elements = [
                SizeVarInput = new FloatInput("Size", 1f, 1.2f),
                NumLarvaeInput = new IntInput("Number of larvae", 1, 5) {description = "Note: box worms with 5 larvae are extremely rare, with a one in 8 million chance."},
                ];
        }

        private BoxWormResults GetResults(XORShift128 Random)
        {
            int numLarvae = Mathf.Max(1, (int)(Random.Value * 5)); // ongomato why did you do it like this
            float sizeVar = 1f + Random.Value * 0.2f;

            return new BoxWormResults
            {
                numLarvae = numLarvae,
                size = sizeVar,
                larvaeFlags = 0
            };
        }

        public override float Execute(XORShift128 Random)
        {
            var results = GetResults(Random);
            return DistanceIf(results.numLarvae, NumLarvaeInput) + DistanceIf(results.size, SizeVarInput);
        }

        protected override IEnumerable<string> GetValues(XORShift128 Random)
        {
            var results = GetResults(Random);
            yield return $"Size: {results.size}";
            yield return $"Number of larvae: {results.numLarvae}";
        }

        private struct BoxWormResults
        {
            public float size;
            public int numLarvae;
            public int larvaeFlags;
        }

        private class BoxWormLarvaeInput : IElement, ISaveInHistory
        {
            private const int LARVAE = 10;

            private bool enabled = false;
            private int bias = 1;
            private int larvae = 0;
            public float Height => (enabled ? 66f : 24f);

            public string SaveKey => "Larvae";

            public void Create(float x, ref float y, List<UIelement> elements)
            {
                y -= 24f;
                var cb = new OpCheckBox(OpUtil.CosmeticBind(enabled), new(x, y));
                cb.OnValueUpdate += ToggleEnable;
                elements.Add(cb);

                if (enabled)
                {
                    // Bias
                    float edge = 600f - 20f - 20f - Mathf.Floor(x / 10f) * 10f;
                    var biasTicker = new OpDragger(OpUtil.CosmeticRange(bias, 1, 999), new Vector2(edge - 24f, y));
                    var biasLabel = new OpLabel(edge - 30f - LabelTest.GetWidth("Bias:"), y, "Bias:") { verticalAlignment = OpLabel.LabelVAlignment.Center };
                    biasTicker.OnValueUpdate += (_, _, _) => bias = biasTicker.GetValueInt();
                    elements.Add(biasTicker);
                    elements.Add(biasLabel);

                    // Checkboxes
                    y -= 42f;
                    float width = 600f - 20f - 20f - Mathf.Floor(x / 10f) * 20f;
                    elements.Add(new OpRect(new Vector2(x, y), new Vector2(width, 36f)));
                    for (int i = 0; i < LARVAE; i++)
                    {
                        elements.Add(LarvaeCheckbox(i, x, y));
                    }
                }
            }

            private void ToggleEnable(UIconfig config, string value, string oldValue)
            {
                if (value != oldValue)
                {
                    enabled = (config as OpCheckBox)!.GetValueBool();
                    SearchTab.instance.UpdateQueryBox();
                }
            }

            private UIelement LarvaeCheckbox(int index, float x, float y)
            {
                int flag = 1 << index;
                bool active = (larvae & flag) != 0;
                var cb = new OpCheckBox(OpUtil.CosmeticBind(active), new Vector2(x + 6f + 30f * index, y + 6f));
                cb.OnValueUpdate += OnCheckboxChange;
                return cb;

                void OnCheckboxChange(UIconfig _, string __, string ___)
                {
                    int value = (cb.GetValueBool()) ? flag : 0;
                    larvae &= ~flag;
                    larvae |= value;
                }
            }

            public float Distance(int value)
            {
                if (!enabled) return 0f;
                int distance = 0;
                for (int i = 0; i < LARVAE; i++)
                {
                    int flag = 1 << i;
                    if ((value & flag) != (larvae & flag))
                    {
                        distance++;
                    }
                }
                return distance * 0.5f * bias; // 0.5 so that two switched equals 1
            }

            public void FromSaveData(JObject data)
            {
                enabled = (bool)data["enabled"]!;
                larvae = (int)data["value"]!;
                bias = (int)data["bias"]!;
            }

            public JObject ToSaveData()
            {
                return new JObject
                {
                    ["enabled"] = enabled,
                    ["value"] = larvae,
                    ["bias"] = bias
                };
            }

            public IEnumerable<string> GetHistoryLines()
            {
                if (enabled)
                {
                    var sb = new StringBuilder("Larvae positions: ");
                    for (int i = 0; i < LARVAE; i++)
                    {
                        if ((larvae & (1 << i)) != 0)
                        {
                            sb.Append('O');
                        }
                        else
                        {
                            sb.Append('_');
                        }
                        sb.Append(' ');
                    }
                    yield return sb.ToString();
                }
                yield break;
            }
        }
    }
}
