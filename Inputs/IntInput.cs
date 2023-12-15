using System;
using System.Collections.Generic;
using Menu.Remix.MixedUI;
using Menu.Remix.MixedUI.ValueTypes;
using UnityEngine;
using static FinderMod.OpUtil;

namespace FinderMod.Inputs
{
    public class IntInput : BaseInput
    {
        private readonly int min;
        private readonly int max;
        private int value;

        public IntInput(string name, int min, int max) : base(name, 1)
        {
            this.min = min;
            this.max = max;
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
            x += LABEL_OFFSET + label.GetDisplaySize().x + INPUT_OFFSET;
            inputs.Add(cb);
            inputs.Add(label);

            // Make input
            if (Enabled)
            {
                var input = new OpSlider(CosmeticBind(value), new Vector2(x, y - 4f), 160) { min = min, max = max };
                input.OnValueChanged += (_, _, _) => { value = input.GetValueInt(); };

                inputs.Add(input);
            }

            y -= LINE_HEIGHT;
        }

        public override float? GetValue(int index)
        {
            return Enabled ? value : null;
        }

        public override string ToString()
        {
            return Name + ": " + value;
        }
    }
}
