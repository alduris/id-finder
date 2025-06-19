using System;
using System.Collections.Generic;
using Menu.Remix.MixedUI;
using Newtonsoft.Json.Linq;
using RWCustom;
using UnityEngine;

namespace FinderMod.Inputs
{
    /// <summary>
    /// Group containing inputs representing an HSL color
    /// </summary>
    public class ColorHSLInput : Group
    {
        /// <summary>Hue input</summary>
        public FloatInput HueInput { get; private set; } = null!;
        /// <summary>Saturation input</summary>
        public FloatInput SatInput { get; private set; } = null!;
        /// <summary>Lightness input</summary>
        public FloatInput LightInput { get; private set; } = null!;

        /// <summary>Default value if input is missing</summary>
        public float defaultHue, defaultSat, defaultLight;
        /// <summary>Whether to simulate the bugged calculations done by <see cref="Custom.HSL2RGB(float, float, float)"/> in display preview</summary>
        public bool fixColors = false;

        private HSLColor lastColor;


        /// <summary>
        /// Sets the descriptions of all inputs
        /// </summary>
        public string description
        {
            set
            {
                HueInput.description = value;
                SatInput.description = value;
                LightInput.description = value;
            }
        }

        /// <summary>
        /// Creates an HSL input group with all sliders present and all values possible
        /// </summary>
        /// <param name="name">Name of group</param>
        public ColorHSLInput(string name) : this(name, true, true, true) { }

        /// <summary>
        /// Creates an HSL input group with specific sliders present and all values possible within the present sliders.
        /// </summary>
        /// <param name="name">Name of group</param>
        /// <param name="h">Whether the hue slider is present</param>
        /// <param name="s">Whether the saturation slider is present</param>
        /// <param name="l">Whether the lightness slider is present</param>
        public ColorHSLInput(string name, bool h, bool s, bool l) : this(name, h, 0f, 1f, s, 0f, 1f, l, 0f, 1f) { }

        /// <summary>
        /// Creates an HSL input group with all sliders present and specific ranges
        /// </summary>
        /// <param name="name">Name of group</param>
        /// <param name="hMin">Minimum value of hue slider</param>
        /// <param name="hMax">Maximum value of hue slider</param>
        /// <param name="sMin">Minimum value of saturation slider</param>
        /// <param name="sMax">Maximum value of saturation slider</param>
        /// <param name="lMin">Minimum value of lightness slider</param>
        /// <param name="lMax">Maximum value of lightness slider</param>
        public ColorHSLInput(string name, float hMin, float hMax, float sMin, float sMax, float lMin, float lMax) : this(name, true, hMin, hMax, true, sMin, sMax, true, lMin, lMax) { }

        /// <summary>
        /// Creates an HSL input group with specific sliders present as well as their ranges.
        /// If a slider is not present, it is still recommended to put the min and maximum values as some replacement value for the display color.
        /// </summary>
        /// <param name="name">Name of group</param>
        /// <param name="h">Whether the hue slider is present</param>
        /// <param name="hMin">Minimum value of hue slider, or default value if not present</param>
        /// <param name="hMax">Maximum value of hue slider, or default value if not present</param>
        /// <param name="s">Whether the saturation slider is present</param>
        /// <param name="sMin">Minimum value of saturation slider, or default value if not present</param>
        /// <param name="sMax">Maximum value of saturation slider, or default value if not present</param>
        /// <param name="l">Whether the lightness slider is present</param>
        /// <param name="lMin">Minimum value of lightness slider, or default value if not present</param>
        /// <param name="lMax">Maximum value of lightness slider, or default value if not present</param>
        public ColorHSLInput(string name, bool h, float hMin, float hMax, bool s, float sMin, float sMax, bool l, float lMin, float lMax) : base(null!, name)
        {
            children = [new Label(name, false)];
            if (h) children.Add(HueInput = new HueInput("H", hMin, hMax));
            if (s) children.Add(SatInput = new FloatInput("S", sMin, sMax));
            if (l) children.Add(LightInput = new FloatInput("L", lMin, lMax));

            defaultHue = hMin;
            defaultSat = sMax;
            defaultLight = (lMin + lMax) / 2;

            if (HueInput != null) HueInput.OnValueChanged += OnUpdateColor;
            if (SatInput != null) SatInput.OnValueChanged += OnUpdateColor;
            if (LightInput != null) LightInput.OnValueChanged += OnUpdateColor;

            lastColor = new HSLColor(HueInput?.value ?? defaultHue, SatInput?.value ?? defaultSat, LightInput?.value ?? defaultLight);
        }

        /// <summary>
        /// Creates the element.
        /// </summary>
        /// <param name="x">What x to start from</param>
        /// <param name="y">What y to start from</param>
        /// <param name="elements">The list of elements to add to</param>
        public override void Create(float x, ref float y, List<UIelement> elements)
        {
            base.Create(x, ref y, elements);
            ForceUpdateColor();
        }

        private void OnUpdateColor(Input<float> inp, float __, float ___)
        {
            float h = HueInput?.value ?? defaultHue;
            float s = SatInput?.value ?? defaultSat;
            float l = LightInput?.value ?? defaultLight;
            var newColor = new HSLColor(h, s, l);
            OnValueChanged?.Invoke(this, newColor, lastColor);
            lastColor = newColor;
            ForceUpdateColor();
        }

        /// <summary>
        /// Forces a color update
        /// </summary>
        public void ForceUpdateColor()
        {
            float h = HueInput?.value ?? defaultHue;
            float s = SatInput?.value ?? defaultSat;
            float l = LightInput?.value ?? defaultLight;
            Color color = Custom.HSL2RGB(fixColors ? h - Mathf.Floor(h) : h, s, l);
            ColorEdge = color;
            ColorFill = color;
        }

        /// <summary>
        /// Returns the element to the state it was given the save state data.
        /// </summary>
        /// <param name="data">The save data to restore data to</param>
        public override void FromSaveData(JObject data)
        {
            base.FromSaveData(data);
            ForceUpdateColor();
        }

        /// <summary>Returns the string representation for the input on the history tab.</summary>
        /// <returns>The strings to represent the input on the history tab.</returns>
        public override IEnumerable<string> GetHistoryLines()
        {
            if (HueInput is not null) yield return internalName + " H: " + HueInput.value;
            if (SatInput is not null) yield return internalName + " S: " + SatInput.value;
            if (LightInput is not null) yield return internalName + " L: " + LightInput.value;
        }


        /// <summary>
        /// Event delegate for when the value of the sliders change
        /// </summary>
        /// <param name="input"></param>
        /// <param name="value"></param>
        /// <param name="oldValue"></param>
        public delegate void ValueChanged(ColorHSLInput input, HSLColor value, HSLColor oldValue);
        /// <summary>
        /// Event called when the value of any internal slider changes
        /// </summary>
        public event ValueChanged OnValueChanged = null!;
    }
}
