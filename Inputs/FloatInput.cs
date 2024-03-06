using System;
using System.Collections.Generic;
using Menu.Remix.MixedUI;
using Menu.Remix.MixedUI.ValueTypes;
using UnityEngine;
using static FinderMod.OpUtil;

namespace FinderMod.Inputs
{
    public class FloatInput : BaseInput
    {
        private readonly float min;
        private readonly float max;
        private float value;

        public FloatInput(string name, float min, float max) : base(name, 1)
        {
            this.min = min;
            this.max = max;
            value = Math.Min(max, Math.Max(min, 0f));
        }
        public FloatInput(string name) : this(name, 0f, 1f) { }

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
                var input = new OpFloatSlider(CosmeticBind(value), new Vector2(x, y - 4f), 160, 4) { min = min, max = max };
                input.OnValueUpdate += (_, _, _) => { value = input.GetValueFloat(); };

                inputs.Add(input);
            }

            y -= LINE_HEIGHT;
        }

        public override float? GetValue(int index)
        {
            return Enabled ? value : null;
        }

        public override void SetValues(bool enabled, List<float> values)
        {
            base.SetValues(enabled, values);
            if (Enabled)
            {
                value = values[0];
            }
        }

        public override string ToString()
        {
            return Name + ": " + value;
        }
    }
}
