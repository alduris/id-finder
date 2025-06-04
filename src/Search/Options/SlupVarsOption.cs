using System.Collections.Generic;
using FinderMod.Inputs;
using FinderMod.Tabs;
using RWCustom;
using UnityEngine;

namespace FinderMod.Search.Options
{
    public class SlupVarsOption : Option
    {
        private readonly FloatInput SizeInput, WidenessInput, EyeInput;
        private readonly ColorHSLInput ColorInp;
        private readonly BoolInput DarkInput;

        public SlupVarsOption() : base()
        {
            elements = [
                SizeInput = new FloatInput("Size"),
                WidenessInput = new FloatInput("Wideness"),
                new Whitespace(),
                DarkInput = new BoolInput("Is dark?"),
                ColorInp = new ColorHSLInput("Color", true, 0f, 1f, true, 0f, 1f, true, 0.649f, 1f),
                EyeInput = new FloatInput("Eye (L)") { description = "Only affects dark slugpups", enabled = false }
            ];

            DarkInput.OnValueChanged += DarkInp_OnValueChanged;
        }

        private void DarkInp_OnValueChanged(Input<bool> input, bool value, bool oldValue)
        {
            if (value != oldValue)
            {
                if (value)
                {
                    ColorInp.LightInput.min = 0.01f;
                    ColorInp.LightInput.max = 0.146f;
                    ColorInp.LightInput.value = Custom.LerpMap(ColorInp.LightInput.value, 0.649f, 1f, 0.146f, 0.01f);
                }
                else
                {
                    ColorInp.LightInput.min = 0.649f;
                    ColorInp.LightInput.max = 1f;
                    ColorInp.LightInput.value = Custom.LerpMap(ColorInp.LightInput.value, 0.01f, 0.146f, 1f, 0.649f);
                    EyeInput.enabled = false;
                }

                UpdateQueryBox(); // needs to be updated so input updates
            }
        }

        private Results GetResults(XORShift128 Random)
        {
            uint seed = Random.x;

            float bal, met, stl, siz, wde, eye, h, s, l;
            bool drk;

            // Physical attributes
            bal = Mathf.Pow(Random.Range(0f, 1f), 1.5f);
            met = Mathf.Pow(Random.Range(0f, 1f), 1.5f);
            stl = Mathf.Pow(Random.Range(0f, 1f), 1.5f);
            siz = Mathf.Pow(Random.Range(0f, 1f), 1.5f);
            wde = Mathf.Pow(Random.Range(0f, 1f), 1.5f);

            // Color
            h = Mathf.Lerp(Random.Range(0.15f, 0.58f), Random.Value, Mathf.Pow(Random.Value, 1.5f - met));
            s = Mathf.Pow(Random.Range(0f, 1f), 0.3f + stl * 0.3f);
            drk = Random.Range(0f, 1f) <= 0.3f + stl * 0.2f;
            l = Mathf.Pow(Random.Range(drk ? 0.9f : 0.75f, 1f), 1.5f - stl);
            eye = Mathf.Pow(Random.Range(0f, 1f), 2f - stl * 1.5f);
            // eye L actually more complicated than just lightness, lerps between room dark or white and inverse of some other color based on 0.25x this
            // except it's not even the inverse, this is the calculation: new Color(1f - color.r, 1f - color.b, 1f - color.g)


            // Special color cases
            switch (seed)
            {
                case 1000:
                    // Custom.RGB2HSL(new Color(.6f, .7f, .9f))
                    h = 0.6111111f;
                    s = 0.6f;
                    l = 0.75f;
                    eye = 0f;
                    break;
                case 1001:
                    // Custom.RGB2HSL(new Color(.48f, .87f, .81f))
                    drk = false;
                    h = 0.4743589f;
                    s = 0.6f;
                    l = 0.675f;
                    eye = 0f;
                    break;
                case 1002:
                    // Custom.RGB2HSL(new Color(.43922f, .13725f, .23529f))
                    drk = true;
                    h = 0.9458887f;
                    s = 0.5238261f;
                    l = 0.288235f;
                    break;
            }

            if (drk) l = Mathf.Clamp(1 - l, 0.01f, 1f);

            return new Results
            {
                size = siz,
                wideness = wde,
                h = h,
                s = s,
                l = l,
                dark = drk,
                eye = eye,
            };
        }

        public override float Execute(XORShift128 Random)
        {
            var results = GetResults(Random);

            // Compute result
            float r = 0f;
            r += DistanceIf(results.size, SizeInput);
            r += DistanceIf(results.wideness, WidenessInput);
            r += WrapDistanceIf(results.h, ColorInp.HueInput);
            r += DistanceIf(results.s, ColorInp.SatInput);
            r += DistanceIf(results.l, ColorInp.LightInput);
            r += DistanceIf(results.dark, DarkInput);
            r += DistanceIf(results.eye, EyeInput);
            return r;
        }

        protected override IEnumerable<string> GetValues(XORShift128 Random)
        {
            var results = GetResults(Random);

            // Print stuff
            yield return $"Size: {results.size}";
            yield return $"Wideness: {results.wideness}";
            yield return $"Color: hsl({results.h}, {results.s}, {results.l})";
            yield return $"Is dark: {(results.dark ? "Yes" : "No")}";
            yield return $"Eye (lightness): {results.eye}";
        }

        private struct Results
        {
            public float size;
            public float wideness;
            public float h, s, l;
            public bool dark;
            public float eye;
        }
    }
}
