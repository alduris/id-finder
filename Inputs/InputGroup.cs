using System.Collections.Generic;
using System.Linq;
using Menu.Remix.MixedUI;
using Menu.Remix.MixedUI.ValueTypes;
using UnityEngine;
using static FinderMod.OpUtil;

namespace FinderMod.Inputs
{
    internal class InputGroup : BaseInput
    {
        private const float PADDING = 8f;

        private readonly BaseInput[] inputs;
        public OpScrollBox parent;
        
        public InputGroup(string name, BaseInput[] inputs) : base(name, inputs.Aggregate(0, (val, inp) => val + inp.ValueCount))
        {
            this.inputs = inputs;
        }

        private OpScrollBox UpdateContainer(OpScrollBox container)
        {
            // Clear out existing children if there are any
            if (container != null)
            {
                foreach (UIelement el in container.items)
                {
                    el.Deactivate();
                    el.tab.items.Remove(el);
                }
            }

            // Create the new children
            List<UIelement> children = new List<UIelement>();
            List<(BaseInput, OpCheckBox)> cbs = new List<(BaseInput, OpCheckBox)>();
            float y = 0f;
            foreach (var inp in inputs)
            {
                float temp = y;
                var cb = new OpCheckBox(CosmeticBind(inp.Enabled), new Vector2(PADDING, y));
                var label = new OpLabel(30f, y, inp.Name);
                var el = inp.Enabled ? inp.GetUI(PADDING + 30f, PADDING + 36f + label.GetDisplaySize().x, ref y) : null;

                y += 30f;
                children.Add(cb);
                cbs.Add((inp, cb));
                children.Add(label);
                if (el != null) children.Add(el);
            }

            // Create scrollbox if it doesn't exist
            if (container == null)
            {
                container = new OpScrollBox(new Vector2(0f, 0f), new Vector2(600f, y + 2 * PADDING), y + 2 * PADDING, false, true, false);
                parent.AddItems(container);
            }

            // Add event listeners to checkboxes now that container is guaranteed to exist
            foreach (var (inp, cb) in cbs)
            {

                cb.OnValueChanged += (_, _, _) => {
                    inp.Enabled = cb.GetValueBool();
                    UpdateContainer(container);
                };
            }

            // Add the children
            y += PADDING;
            children.ForEach(el => { el.pos = new Vector2(el.pos.x, y - el.pos.y); });
            container.AddItems(children.ToArray());
            return container;
        }

        public override UIelement GetUI(float tx, float x, ref float y)
        {
            var container = UpdateContainer(null);
            container.pos = new Vector2(tx, y - container.size.y);
            container.size = new Vector2(600f - tx - PADDING, container.size.y);

            return container;
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
