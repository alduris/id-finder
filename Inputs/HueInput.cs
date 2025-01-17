using Menu.Remix.MixedUI;
using RWCustom;
using UnityEngine;

namespace FinderMod.Inputs
{
    public class HueInput(string name, float min, float max, float init) : FloatInput(name, Mathf.Clamp01(min), Mathf.Clamp01(max), Mathf.Clamp01(init))
    {
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
