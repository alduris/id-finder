using System;
using System.Collections.Generic;
using FinderMod.Inputs;
using UnityEngine;

namespace FinderMod.Search.Options
{
    public class MirosVultureVarsOption : Option
    {
        private readonly ColorHSLInput ColorAInput;
        private readonly ColorHSLInput ColorBInput;
        private readonly IntInput FeatherCountInput;
        private readonly FloatInput BeakFatnessInput;
        private readonly IntInput TopTeethCountInput;
        private readonly IntInput BottomTeethCountInput;
        private readonly ColorLerpInput EyeColorInput;

        public MirosVultureVarsOption()
        {
            elements = [
                ColorAInput = new ColorHSLInput("Wing color A", true, 0.0025f - 0.02f, 0.0025f + 0.02f, false, 1f, 1f, true, 0.35f, 0.65f) { fixColors = true },
                ColorBInput = new ColorHSLInput("Wing color B", true, 0.0025f - 0.02f - 0.25f, 0.0025f + 0.02f + 0.25f, true, 0.8f, 1f, true, 0.45f, 1f) { fixColors = true },
                FeatherCountInput = new IntInput("Feathers per wing", 6, 7),
                new Whitespace(),
                BeakFatnessInput = new FloatInput("Beak fatness"),
                TopTeethCountInput = new IntInput("Beak top teeth count", 10, 19),
                BottomTeethCountInput = new IntInput("Beak bottom teeth count", 10, 19),
                new Whitespace(),
                EyeColorInput = new ColorLerpInput("Eye color", new HSLColor(0.08f, 1f, 0.5f).rgb, new HSLColor(0.17f, 1f, 0.5f).rgb)
                ];
        }

        private MirosVultureResults GetResults(XORShift128 Random)
        {
            // Wing colors
            var ColorA = new HSLColor(WrappedRandomVariation(0.0025f, 0.02f, 0.6f, Random), 1f, ClampedRandomVariation(0.5f, 0.15f, 0.1f, Random));
            var ColorB = new HSLColor(ColorA.hue + Mathf.Lerp(-0.25f, 0.25f, Random.Value), Mathf.Lerp(0.8f, 1f, 1f - Random.Value * Random.Value), Mathf.Lerp(0.45f, 1f, Random.Value * Random.Value));

            // Feathers
            int featherCount = Random.Range(6, 8);
            float loseFeathers = ((Random.Value < 0.5f) ? 40f : Mathf.Lerp(8f, 15f, Random.Value));
            float missingFeathers = ((Random.Value < 0.5f) ? 40f : Mathf.Lerp(8f, 15f, Random.Value));
            float brokenFeathers = ((Random.Value < 0.5f) ? 20f : Mathf.Lerp(3f, 6f, Random.Value));

            // Beak
            float beakFatness = Random.Value;
            int topTeethCount = Random.Range(10, 20);
            Random.Shift(topTeethCount * 3);
            int btmTeethCount = Random.Range(10, 20);
            Random.Shift(btmTeethCount * 3);

            // Feathers again because the code just loves being in a consistent order
            for (int i = 0; i < 4 * featherCount; i++)
            {
                Random.Shift(2);
                bool multiBroken = Random.Value < 0.025f;
                if (Random.Value < 1f / loseFeathers || (multiBroken && Random.Value < 0.5f))
                {
                    Random.Shift(3);
                    if (Random.Value < 0.4f)
                    {
                        Random.Shift(2);
                    }
                }
                Random.Shift();
                if (Random.Value >= 0.025f && multiBroken) Random.Shift(); // if (Random.Value < 0.025f || (multiBroken && Random.Value < 0.5f)) { }
                if (Random.Value < 1f / brokenFeathers || (multiBroken && Random.Value < 0.5f))
                {
                    if (Random.Value >= 0.5f) Random.Shift(); // this.wings[j, k].brokenColor = ((Random.Value < 0.5f) ? 1f : Random.Value);
                }
            }

            // Appendages. Whatever that means.
            for (int i = 0; i < 2; i++)
            {
                int segmens = Random.Range(2, 12);
                bool changedRad = false;
                for (int j = 0; j < segmens; j++)
                {
                    if (j != 0) Random.Shift();
                    if (!changedRad && Random.Value < 0.5f)
                    {
                        changedRad = true;
                    }
                    else
                    {
                        changedRad = false;
                    }
                    Random.Shift();
                }
            }

            // Albino check even though it sets it to false anyway
            if (ModManager.MSC) Random.Shift();

            // Juicy eye stuff
            Random.Shift(); // we love Mathf.Lerp(0.6f, 0.6f, Random.Value) which literally does nothing :huntersmile:
            float eyeCol = Random.Value;

            return new MirosVultureResults
            {
                colorA = ColorA,
                colorB = ColorB,
                feathersPerWing = featherCount,
                beakFatness = beakFatness,
                topTeethCount = topTeethCount,
                bottomTeethCount = btmTeethCount,
                eyeColor = eyeCol
            };
        }

        public override float Execute(XORShift128 Random)
        {
            var results = GetResults(Random);
            float r = 0f;

            r += DistanceIf(results.colorA, ColorAInput);
            r += DistanceIf(results.colorB, ColorBInput);
            r += DistanceIf(results.feathersPerWing, FeatherCountInput);
            r += DistanceIf(results.beakFatness, BeakFatnessInput);
            r += DistanceIf(results.topTeethCount, TopTeethCountInput);
            r += DistanceIf(results.bottomTeethCount, BottomTeethCountInput);
            r += DistanceIf(results.eyeColor, EyeColorInput);

            return r;
        }

        protected override IEnumerable<string> GetValues(XORShift128 Random)
        {
            var results = GetResults(Random);
            yield return $"Wing color A: hsl({results.colorA.hue}, {results.colorA.saturation}, {results.colorA.lightness})";
            yield return $"Wing color B: hsl({results.colorB.hue}, {results.colorB.saturation}, {results.colorB.lightness})";
            yield return $"Feathers per wing: {results.feathersPerWing}";
            yield return null!;
            yield return $"Beak fatness: {results.beakFatness}";
            yield return $"Top teeth count: {results.topTeethCount}";
            yield return $"Bottom teeth count: {results.bottomTeethCount}";
            yield return null!;
            yield return $"Eye color: hsl({Mathf.Lerp(0.08f, 0.17f, results.eyeColor)}, 1, 0.5)";
        }

        private struct MirosVultureResults
        {
            public HSLColor colorA;
            public HSLColor colorB;
            public int feathersPerWing;
            public float beakFatness;
            public int topTeethCount;
            public int bottomTeethCount;
            public float eyeColor;
        }
    }
}
