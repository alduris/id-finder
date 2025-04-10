﻿using System.Collections.Generic;
using Menu.Remix.MixedUI;
using Newtonsoft.Json.Linq;
using RWCustom;
using UnityEngine;

namespace FinderMod.Inputs
{
    public class ColorHSLInput : Group
    {
        public FloatInput HueInput { get; private set; } = null!;
        public FloatInput SatInput { get; private set; } = null!;
        public FloatInput LightInput { get; private set; } = null!;

        private float defaultHue, defaultSat, defaultLight;
        public bool fixColors = false;

        // private OpRect colorRect = null!;

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

        public ColorHSLInput(string name, bool h, float hMin, float hMax, bool s, float sMin, float sMax, bool l, float lMin, float lMax) : base(null!, name)
        {
            children = [new Label(name, false)];
            if (h) children.Add(HueInput = new HueInput("H", hMin, hMax));
            if (s) children.Add(SatInput = new FloatInput("S", sMin, sMax));
            if (l) children.Add(LightInput = new FloatInput("L", lMin, lMax));

            defaultHue = hMin;
            defaultSat = sMax;
            defaultLight = (lMin + lMax) / 2;

            if (HueInput != null) HueInput.OnValueChanged += UpdateColor;
            if (SatInput != null) SatInput.OnValueChanged += UpdateColor;
            if (LightInput != null) LightInput.OnValueChanged += UpdateColor;
        }

        public override void Create(float x, ref float y, List<UIelement> elements)
        {
            base.Create(x, ref y, elements);
            // float height = (HueInput?.Height + MARGIN ?? 0) + (SatInput?.Height + MARGIN ?? 0) + (LightInput?.Height + MARGIN ?? 0) - MARGIN;
            // elements.Add(colorRect = new OpRect(new Vector2(300f, y + height / 2f), Vector2.one * Mathf.Max(30f, height), 1f));
            UpdateColor(null!, 0, 0);
        }

        private void UpdateColor(Input<float> _, float __, float ___)
        {
            float h = HueInput?.value ?? defaultHue;
            float s = SatInput?.value ?? defaultSat;
            float l = LightInput?.value ?? defaultLight;
            Color color = Custom.HSL2RGB(fixColors ? h - Mathf.Floor(h) : h, s, l);
            ColorEdge = color;
            ColorFill = color;
            // if (colorRect != null) colorRect.colorFill = color;
        }

        public override void FromSaveData(JObject data)
        {
            base.FromSaveData(data);
            UpdateColor(null!, 0, 0);
        }

        public override IEnumerable<string> GetHistoryLines()
        {
            if (HueInput is not null) yield return internalName + " H: " + HueInput.value;
            if (SatInput is not null) yield return internalName + " S: " + SatInput.value;
            if (LightInput is not null) yield return internalName + " L: " + LightInput.value;
        }
    }
}
