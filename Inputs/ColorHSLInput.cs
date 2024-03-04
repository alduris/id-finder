using System;
using System.Collections.Generic;
using Menu.Remix.MixedUI;
using RWCustom;
using UnityEngine;

namespace FinderMod.Inputs
{
    public class ColorHSLInput : InputGroup
    {
        private readonly BaseInput h;
        private readonly BaseInput s;
        private readonly BaseInput l;
        private readonly float[] defaults = new float[3];
        private Color display;

        public ColorHSLInput(string name) : base(name, [ new HueInput("H"), new FloatInput("S"), new FloatInput("L") ])
        {
            h = inputs[0];
            s = inputs[1];
            l = inputs[2];
            display = new(0f, 0f, 0f);
        }

        public ColorHSLInput(string name, float? hv, float? sv, float? lv) : base(name, [
            hv == null ? new HueInput("H") : new Label($"H: {hv.Value}"),
            sv == null ? new HueInput("S") : new Label($"S: {sv.Value}"),
            lv == null ? new HueInput("L") : new Label($"L: {lv.Value}")
        ])
        {
            if (hv is null)
                h = inputs[0];
            else
                defaults[0] = hv.Value;

            if (sv is null)
                s = inputs[1];
            else
                defaults[1] = sv.Value;

            if (lv is null)
                l = inputs[2];
            else
                defaults[2] = lv.Value;

            display = new(h == null ? 0f : hv.Value, s == null ? 0f : sv.Value, l == null ? 0f : lv.Value);
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

        public override float? GetValue(int index)
        {
            List<float?> list = [];
            if (h is not Label) list.Add(h.GetValue(0));
            if (s is not Label) list.Add(s.GetValue(0));
            if (l is not Label) list.Add(l.GetValue(0));

            return list[index];
        }

        public override string ToString()
        {
            string hs = h is Label ? h.GetValue(0).ToString() ?? "??" : defaults[0].ToString();
            string ss = s is Label ? s.GetValue(0).ToString() ?? "??" : defaults[1].ToString();
            string ls = l is Label ? l.GetValue(0).ToString() ?? "??" : defaults[2].ToString();
            return $"hsl({hs}, {ss}, {ls})";
        }

        private void UpdateColor()
        {
            if (!h.Enabled && !s.Enabled && !l.Enabled)
            {
                display = Color.black;
            }
            else
            {
                float hf = h is Label ? (h.GetValue(0) % 1f) ?? 0f : defaults[0];
                float sf = h is Label ? (s.GetValue(0) % 1f) ?? 1f : defaults[1];
                float lf = h is Label ? (l.GetValue(0) % 1f) ?? 0.5f : defaults[2];
                display = Custom.HSL2RGB(hf, sf, lf);
            }
        }
    }
}
