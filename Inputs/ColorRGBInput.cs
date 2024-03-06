using System;
using System.Collections.Generic;
using Menu.Remix.MixedUI;
using Menu.Remix.MixedUI.ValueTypes;
using UnityEngine;
using static FinderMod.OpUtil;

namespace FinderMod.Inputs
{
    public class ColorRGBInput(string name) : BaseInput(name, 3)
    {
        private Color value;

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
            x += LABEL_OFFSET;
            inputs.Add(cb);
            inputs.Add(label);

            // Make input
            if (Enabled)
            {
                var input = new OpColorPicker(CosmeticBind(value), new Vector2(x, y));
                y -= input.size.y;
                input.pos = new(input.pos.x, input.pos.y - input.size.y);
                input.OnValueChanged += (_, _, _) => { value = input.valueColor; };

                inputs.Add(input);
            }

            y -= LINE_HEIGHT;
        }


        public override float? GetValue(int index)
        {
            if (!Enabled) return null;

            if      (index == 0) return value.r;
            else if (index == 1) return value.g;
            else                 return value.b;
        }

        public override void SetValues(bool enabled, List<float> values)
        {
            base.SetValues(enabled, values);
            if (enabled)
            {
                value = new Color(values[0], values[1], values[2]);
            }
        }

        public override string ToString()
        {
            return $"{Name}: rgb({value.r}, {value.g}, {value.b})";
        }
    }
}
