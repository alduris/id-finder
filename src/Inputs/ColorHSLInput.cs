using RWCustom;
using UnityEngine;

namespace FinderMod.Inputs
{
    public class ColorHSLInput : Group
    {
        public FloatInput HueInput { get; private set; }
        public FloatInput SatInput { get; private set; }
        public FloatInput LightInput { get; private set; }

        public string descripion
        {
            set
            {
                HueInput.description = value;
                SatInput.description = value;
                LightInput.description = value;
            }
        }

        public ColorHSLInput(string name) : this(name, true, true, true) { }

        public ColorHSLInput(string name, bool h, bool s, bool l) : this(name, h, 0f, 1f, s, 0f, 1f, l, 0f, 1f) { }

        public ColorHSLInput(string name, bool h, float hMin, float hMax, bool s, float sMin, float sMax, bool l, float lMin, float lMax) : base(null)
        {
            children = [new Label(name, false)];
            if (h) children.Add(HueInput = new FloatInput("H", hMin, hMax));
            if (s) children.Add(SatInput = new FloatInput("S", sMin, sMax));
            if (l) children.Add(LightInput = new FloatInput("L", lMin, lMax));

            if (HueInput != null) HueInput.OnValueChanged += UpdateColor;
            if (SatInput != null) SatInput.OnValueChanged += UpdateColor;
            if (LightInput != null) LightInput.OnValueChanged += UpdateColor;
        }

        private void UpdateColor(Input<float> _, float __, float ___)
        {
            Color color = Custom.HSL2RGB(HueInput.value, SatInput.value, LightInput.value);
            ColorEdge = color;
            ColorFill = color;
        }
    }
}
