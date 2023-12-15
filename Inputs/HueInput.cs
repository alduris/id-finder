using System;
using System.Collections.Generic;
using Menu.Remix.MixedUI;
using Menu.Remix.MixedUI.ValueTypes;
using RWCustom;

namespace FinderMod.Inputs
{
    public class HueInput : FloatInput
    {
        public HueInput(string name) : base(name, 0f, 1f)
        {
            Wrap = true;
        }

        public override void AddUI(float x, ref float y, List<UIelement> inputs, Action UpdateQueryBox)
        {
            base.AddUI(x, ref y, inputs, UpdateQueryBox);
            if (Enabled)
            {
                OpFloatSlider input = inputs.GetLastObject() as OpFloatSlider;
                input.colorEdge = Custom.HSL2RGB(input.GetValueFloat(), 1f, 0.625f);
                input.colorFill = Custom.HSL2RGB(input.GetValueFloat(), 1f, 0.625f);
                input.OnValueChanged += (_, _, _) =>
                {
                    input.colorEdge = Custom.HSL2RGB(input.GetValueFloat(), 1f, 0.625f);
                    input.colorFill = Custom.HSL2RGB(input.GetValueFloat(), 1f, 0.625f);
                };
            }
        }
    }
}
