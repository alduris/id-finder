using System.Collections.Generic;
using FinderMod.Inputs;
using UnityEngine;

namespace FinderMod.Search.Options
{
    public class SlupVarsOption : Option
    {
        private readonly FloatInput szInp, wdInp, eyeInp;
        private readonly ColorHSLInput colorInp;
        private readonly BoolInput darkInp;

        public SlupVarsOption() : base()
        {
            elements = [
                szInp = new FloatInput("Size"),
                wdInp = new FloatInput("Wideness"),
                new Whitespace(),
                colorInp = new ColorHSLInput("Color", true, 0f, 1f, true, 0f, 1f, true, 0.65f, 1f),
                darkInp = new BoolInput("Is dark?"),
                eyeInp = new FloatInput("Eye (L)") { description = "Inverse if dark" }
            ];
            colorInp.LightInput.description = "Inverse if dark";
        }

        public override float Execute(XORShift128 Random)
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

            // Compute result
            float r = 0f;
            r += DistanceIf(siz, szInp);
            r += DistanceIf(wde, wdInp);
            r += WrapDistanceIf(h, colorInp.HueInput);
            r += DistanceIf(s, colorInp.SatInput);
            r += DistanceIf(l, colorInp.LightInput);
            if (darkInp.enabled) r += (drk ^ darkInp.value ? 0f : 1f);
            r += DistanceIf(eye, eyeInp);
            return r;
        }

        protected override IEnumerable<string> GetValues(XORShift128 Random)
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

            // Print stuff
            yield return $"Size: {siz}";
            yield return $"Wide: {wde}";
            yield return $"Color: hsl({h}, {s}, {l})";
            yield return $"Is dark: {(drk ? "Yes" : "No")}";
            yield return $"Eye (lightness): {eye}";
        }
    }
}
