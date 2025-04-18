using System;
using System.Collections.Generic;
using FinderMod.Inputs;
using UnityEngine;

namespace FinderMod.Search.Options
{
    public class FrogVarsOption : Option
    {
        private readonly IntInput HornCountInput;
        private readonly FloatInput HornScaleInput;

        private readonly ColorHSLInput BodyColorInput;
        private readonly ColorHSLInput BellyColorInput;

        public FrogVarsOption()
        {
            elements = [
                HornCountInput = new IntInput("Number of horns", 0, 3),
                HornScaleInput = new FloatInput("Horn scale", 0.8f, 1.2f),
                new Whitespace(),
                BodyColorInput = new ColorHSLInput("Body Color", -0.04f, 0.3f, 0.07f, 0.57f, 0f, 0.58f),
                BellyColorInput = new ColorHSLInput("Belly Color", -0.01f, 0.22f, 0f, 0.55f, 0.21f, 0.94f),
                ];
        }

        private FrogResults GetResults(XORShift128 Random)
        {
            HSLColor bodyColor, bellyColor;
            if (Random.Value < 0.05f)
            {
                bodyColor = new HSLColor(0.2f, 0.17f, 0.15f);
                bellyColor = new HSLColor(0.12f, 0.09f, 0.41f);
            }
            else
            {
                bodyColor = new HSLColor(0.06f, 0.47f, 0.38f);
                bellyColor = new HSLColor(0.09f, 0.45f, 0.74f);
            }
            float fac = ClampedRandomVariation(0.5f, 0.1f, 0.2f, Random) - 0.5f;
            bodyColor.hue = Mathf.Repeat(bodyColor.hue + fac, 1f);
            bodyColor.saturation = ClampedRandomVariation(bodyColor.saturation, 0.1f, 0.1f, Random);
            bodyColor.lightness = ClampedRandomVariation(bodyColor.lightness, 0.2f, 0.1f, Random);
            bellyColor.hue = Mathf.Repeat(bellyColor.hue + fac, 1f);
            bellyColor.saturation = ClampedRandomVariation(bellyColor.saturation, 0.1f, 0.1f, Random);
            bellyColor.lightness = ClampedRandomVariation(bellyColor.lightness, 0.2f, 0.1f, Random);


            int horns = Mathf.RoundToInt(3f * Random.Value);
            float hornScale = Random.Range(0.8f, 1.2f);

            return new FrogResults
            {
                bodyColor = bodyColor,
                bellyColor = bellyColor,
                hornCount = horns,
                hornScale = hornScale,
            };
        }

        public override float Execute(XORShift128 Random)
        {
            var results = GetResults(Random);

            float r = 0;
            r += DistanceIf(results.bodyColor, BodyColorInput);
            r += DistanceIf(results.bellyColor, BellyColorInput);
            r += DistanceIf(results.hornCount, HornCountInput);
            r += DistanceIf(results.hornScale, HornScaleInput);
            return r;
        }

        protected override IEnumerable<string> GetValues(XORShift128 Random)
        {
            var results = GetResults(Random);
            yield return $"Horn count: {results.hornCount}";
            yield return $"Horn scale: {results.hornScale}";
            yield return null!;
            yield return $"Body color: hsl({results.bodyColor.hue}, {results.bodyColor.saturation}, {results.bodyColor.lightness})";
            yield return $"Belly color: hsl({results.bellyColor.hue}, {results.bellyColor.saturation}, {results.bellyColor.lightness})";
        }

        private struct FrogResults
        {
            public int hornCount;
            public float hornScale;
            public HSLColor bodyColor;
            public HSLColor bellyColor;
        }
    }
}
