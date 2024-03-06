using System;
using System.Collections.Generic;
using Menu.Remix.MixedUI;
using Menu.Remix.MixedUI.ValueTypes;
using UnityEngine;
using static FinderMod.OpUtil;

namespace FinderMod.Inputs
{
    public class BoolInput(string name) : BaseInput(name, 1)
    {
        private bool value;

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
            x += LABEL_OFFSET + label.GetDisplaySize().x + INPUT_OFFSET;
            inputs.Add(cb);
            inputs.Add(label);

            // Make input
            if (Enabled)
            {
                var input = new OpSimpleButton(new Vector2(x, y), new Vector2(40f, 24f), value ? "Yes" : "No");
                input.OnClick += (_) =>
                {
                    value = !value;
                    input.text = value ? "Yes" : "No";
                };
                inputs.Add(input);
            }

            // Add items and prepare for next
            y -= LINE_HEIGHT;
        }

        public override float? GetValue(int index)
        {
            return Enabled ? (value ? 1f : 0f) : null;
        }

        public override void SetValues(bool enabled, List<float> values)
        {
            base.SetValues(enabled, values);
            if (enabled)
            {
                value = values[0] == 1f;
            }
        }

        public override string ToString()
        {
            return Name + ": " + (value ? "yes" : "no");
        }
    }
}
