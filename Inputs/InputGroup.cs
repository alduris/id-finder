using System;
using System.Collections.Generic;
using System.Linq;
using Menu.Remix.MixedUI;
using Menu.Remix.MixedUI.ValueTypes;
using UnityEngine;
using static FinderMod.OpUtil;

namespace FinderMod.Inputs
{
    public class InputGroup : BaseInput
    {
        protected const float PADDING = 8f;
        protected const float HEIGHT_OFFSET = LINE_HEIGHT - 24f;

        protected readonly BaseInput[] inputs;
        public OpScrollBox parent;
        
        public InputGroup(string name, BaseInput[] inputs) : base(name, inputs.Aggregate(0, (val, inp) => val + inp.ValueCount))
        {
            this.inputs = inputs;
        }

        public override void AddUI(float x, ref float y, List<UIelement> elements, Action UpdateQueryBox)
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
            elements.Add(cb);
            elements.Add(label);

            // Make inputs
            y -= LINE_HEIGHT;
            if (Enabled)
            {
                // Add items
                float temp = y;
                y -= PADDING;
                foreach (var inp in inputs)
                {
                    inp.AddUI(x + PADDING, ref y, elements, UpdateQueryBox);
                }
                y -= PADDING;

                // Make bounding box
                var container = new OpRect(new Vector2(x, y + LINE_HEIGHT), new Vector2(580f - x * 2, temp - y - HEIGHT_OFFSET));
                elements.Add(container);
            }
        }

        public override float? GetValue(int index)
        {
            if (!Enabled) return null;

            int i = 0;
            return inputs.First(inp =>
            {
                if (i + inp.ValueCount >= index)
                {
                    return true;
                }
                else
                {
                    i += inp.ValueCount;
                    return false;
                }
            }).GetValue(index - i);
        }

        public override string ToString()
        {
            List<string> strs = inputs.Select(inp => inp.ToString()).ToList();
            strs.Prepend(Name + ":");
            return string.Join("\n  ", strs);
        }
    }
}
