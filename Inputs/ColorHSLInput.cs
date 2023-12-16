using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Menu.Remix.MixedUI;
using RWCustom;
using UnityEngine;

namespace FinderMod.Inputs
{
    public class ColorHSLInput : InputGroup
    {
        private BaseInput h;
        private BaseInput s;
        private BaseInput l;
        private Color display;

        public ColorHSLInput(string name) : base(name, new BaseInput[]
        {
            new HueInput("H"),
            new FloatInput("S"),
            new FloatInput("L")
        })
        {
            h = inputs[0];
            s = inputs[1];
            l = inputs[2];
            display = new(0f, 0f, 0f);
        }

        public override void AddUI(float x, ref float y, List<UIelement> elements, Action UpdateQueryBox)
        {
            base.AddUI(x, ref y, elements, UpdateQueryBox);

            if (Enabled)
            {
                // Get the containing rect
                OpRect container = elements.Pop() as OpRect;

                // Create a new rect for the color
                float size = container.size.y - PADDING * 2;
                var rect = new OpRect(new Vector2(container.pos.x + container.size.x - PADDING - size, container.pos.y + PADDING), new Vector2(size, size), 1f);
                UpdateColor();
                rect.colorFill = display;

                // Add event listeners
                int index = elements.Count - 1;
                if (l.Enabled)
                {
                    (elements[index] as OpFloatSlider).OnValueUpdate += (_, _, _) => {
                        UpdateColor();
                        rect.colorFill = display;
                    };
                    index--;
                }
                index -= 2;
                if (s.Enabled)
                {
                    (elements[index] as OpFloatSlider).OnValueUpdate += (_, _, _) => {
                        UpdateColor();
                        rect.colorFill = display;
                    };
                    index--;
                }
                index -= 2;
                if (h.Enabled)
                {
                    (elements[index] as OpFloatSlider).OnValueUpdate += (_, _, _) => {
                        UpdateColor();
                        rect.colorFill = display;
                    };
                }
            
                // Add rect and re-add container
                elements.Add(rect);
                elements.Add(container);
            }
        }

        private void UpdateColor()
        {
            if (!h.Enabled && !s.Enabled && !l.Enabled)
            {
                display = Color.black;
            }
            else
            {
                display = Custom.HSL2RGB((h.GetValue(0) % 1f) ?? 0f, s.GetValue(0) ?? 1f, l.GetValue(0) ?? 0.5f, 1f);
            }
        }
    }
}
