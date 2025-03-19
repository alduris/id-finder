using Menu.Remix.MixedUI;
using RWCustom;
using UnityEngine;

namespace FinderMod.Inputs
{
    public class HueInput(string name, float min, float max) : FloatInput(name, min, max)
    {
        public HueInput(string name) : this(name, 0f, 1f) { }

        protected override UIconfig GetElement(Vector2 pos)
        {
            var el = base.GetElement(pos) as OpFloatSlider;
            SetColor(el);
            el.OnValueUpdate += (_, _, _) =>
            {
                SetColor(el);
            };
            return el;
        }

        private void SetColor(OpFloatSlider input)
        {
            var value = GetValue(input);
            input.colorEdge = Custom.HSL2RGB(value, 1f, 0.625f);
            input.colorFill = Custom.HSL2RGB(value, 1f, 0.625f);
        }
    }
}
