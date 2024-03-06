using System;
using System.Collections.Generic;
using System.Linq;
using Menu.Remix.MixedUI;
using Menu.Remix.MixedUI.ValueTypes;
using UnityEngine;
using static FinderMod.OpUtil;

namespace FinderMod.Inputs
{
    internal class PickerInput : BaseInput
    {
        private readonly Dictionary<string, int> values;
        private string key;

        public PickerInput(string name, List<string> options) : base(name, 1)
        {
            foreach (var option in options)
            {
                int i = 0;
                values[option] = i++;
            }
            key = options[0];
        }

        public override void AddUI(float x, ref float y, List<UIelement> inputs, Action UpdateQueryBox)
        {
            // Make checkbox
            var cb = new OpCheckBox(CosmeticBind(Enabled), new(x, y));
            cb.OnValueUpdate += (_, t, f) =>
            {
                Enabled = cb.GetValueBool();
                UpdateQueryBox();
            };

            // Make label
            var label = new OpLabel(x + LABEL_OFFSET, y, Name);
            x += LABEL_OFFSET + label.GetDisplaySize().x + INPUT_OFFSET * 2;
            inputs.Add(cb);
            inputs.Add(label);

            // Make input
            if (Enabled)
            {
                var input = new OpComboBox(CosmeticBind(key), new Vector2(x, y - 4f), 160, values.Keys.ToArray());
                input.OnValueChanged += (_, _, _) => { key = input.value; };

                inputs.Add(input);
            }

            y -= LINE_HEIGHT;
        }

        public override float? GetValue(int index)
        {
            return Enabled ? values[key] : null;
        }

        public override void SetValues(bool enabled, List<float> thisvalues)
        {
            base.SetValues(enabled, thisvalues);
            if (enabled)
            {
                int val = (int)thisvalues[0];
                foreach (var v in values.AsEnumerable())
                {
                    if (v.Value == val)
                    {
                        key = v.Key;
                        break;
                    }
                }
            }
        }

        public override string ToString()
        {
            return $"{Name}: {key}";
        }
    }
}
