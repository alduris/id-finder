using RWCustom;
using UnityEngine;

namespace FinderMod.Inputs
{
    public class ColorHSLInput : Group
    {
        public FloatInput HueInput => children[0] as FloatInput;
        public FloatInput SatInput => children[1] as FloatInput;
        public FloatInput LightInput => children[2] as FloatInput;

        public ColorHSLInput() : base([new FloatInput("H", 0f, 1f, 0.5f), new FloatInput("S", 0f, 1f, 0.5f), new FloatInput("L", 0f, 1f, 0.5f)])
        {
            HueInput.OnValueChanged += UpdateColor;
            SatInput.OnValueChanged += UpdateColor;
            LightInput.OnValueChanged += UpdateColor;
        }

        private void UpdateColor(Input<float> _, float __, float ___)
        {
            Color color = Custom.HSL2RGB(HueInput.Value, SatInput.Value, LightInput.Value);
            ColorEdge = color;
            ColorFill = color;
        }
    }
}
