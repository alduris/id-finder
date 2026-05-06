using System;
using System.Collections.Generic;
using FinderMod.Inputs;
using RWCustom;
using UnityEngine;

namespace FinderMod.Search.Options
{
    internal class LilypuckVarsOption : Option
    {
        private readonly IntInput LeavesInput;
        private readonly ColorRGBInput ColorInput;
        private readonly FloatInput LightRadInput;

        public LilypuckVarsOption()
        {
            RepresentedObject = DLCSharedEnums.AbstractObjectType.LillyPuck;
            elements = [
                LeavesInput = new IntInput("Leaf count", 8, 10),
                ColorInput = new ColorRGBInput("Flower color"),
                LightRadInput = new FloatInput("Light radius", 190f, 260f)
                ];
        }

        public override float Execute(XORShift128 Random)
        {
            var vars = new Variations(Random);

            float r = 0f;
            r += DistanceIf(vars.leaves, LeavesInput);
            r += DistanceIf(vars.flowerColor, ColorInput);
            r += DistanceIf(vars.lightRadius, LightRadInput);

            return r;
        }

        protected override IEnumerable<string> GetValues(XORShift128 Random)
        {
            var vars = new Variations(Random);

            yield return $"Leaf count: {vars.leaves}";
            yield return $"Flower color: rgb({vars.flowerColor.r}, {vars.flowerColor.g}, {vars.flowerColor.b})";
            yield return $"Light radius: {vars.lightRadius}";
        }

        private struct Variations
        {
            public int leaves;
            public Color flowerColor;
            public float lightRadius;

            public Variations(XORShift128 Random)
            {
                leaves = 6 + Random.Range(2, 5);
                float hue = Random.Value;
                hue = Mathf.Lerp(hue, 0.4f, 0.1f);
                var hsl = new HSLColor(hue, 1f, 0.5f);
                flowerColor = Custom.HSL2RGB(hsl.hue, hsl.saturation, hsl.lightness);
                flowerColor.b = Mathf.Lerp(flowerColor.b, flowerColor.g, flowerColor.g);
                flowerColor.g = Custom.LerpMap(flowerColor.g, 0f, 0.3f, 1f, 0.3f);
                float gray = Mathf.Clamp(flowerColor.r + flowerColor.g / 2f + flowerColor.b / 3f, 0f, 1f) * 0.4f;
                gray = Mathf.Lerp(gray, 0.6f, flowerColor.b);
                if (Random.Value < 0.2f)
                {
                    flowerColor = Color.Lerp(flowerColor, new Color(0.7f, 0.9f, 0.9f), 0.7f + gray / 10f);
                }
                else
                {
                    flowerColor = Color.Lerp(flowerColor, new Color(0.7f, 0.7f, 0.7f), gray);
                }
                lightRadius = Random.Range(190f, 260f);
            }
        }
    }
}
