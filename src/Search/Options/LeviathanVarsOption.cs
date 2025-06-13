using System;
using System.Collections.Generic;
using FinderMod.Inputs;
using UnityEngine;

namespace FinderMod.Search.Options
{
    internal class LeviathanVarsOption : Option
    {
        private readonly ColorHSLInput ColorAInput;
        private readonly ColorHSLInput ColorBInput;
        private readonly IntInput ScaleStartInput;
        private readonly IntInput NumFinsInput;
        private readonly FloatInput FinWidthInput;
        private readonly IntInput NumScalesInput;
        private readonly FloatInput ScaleSizeInput;

        public LeviathanVarsOption()
        {
            elements = [
                ColorAInput = new ColorHSLInput("Pattern color A", 0.45f, 0.85f, 0.5f, 0.95f, 0.12f, 0.18f),
                ColorBInput = new ColorHSLInput("Pattern color B", true, 0.3f, 1f, false, 1f, 1f, false, 0.2f, 0.2f),
                new Whitespace(),
                NumFinsInput = new IntInput("Number of fins", 6, 7),
                FinWidthInput = new FloatInput("Fin width input", 6f, 8f) { description = "If number of fins is not 6, fin width is 80% this value in-game." },
                new Whitespace(),
                NumScalesInput = new IntInput("Number of scales", 20, 39),
                ScaleStartInput = new IntInput("Scales start", 1, 5),
                ScaleSizeInput = new FloatInput("Scale size", 6f, 8f),
                ];
        }

        private LeviathanResults GetResults(XORShift128 Random)
        {
            // BigEel.GenerateIVars()
            float baseHue = WrappedRandomVariation(0.65f, 0.2f, 0.8f, Random);
            Random.Shift();
            int finSeed = Random.Range(0, int.MaxValue);
            var colorA = new HSLColor(baseHue, Mathf.Lerp(0.5f, 0.95f, Random.Value), Mathf.Lerp(0.12f, 0.18f, Random.Value));
            float colorB = baseHue + ((Random.Value < 0.5f) ? (-0.15f) : 0.15f);

            // BigEelGraphics..ctor()
            Random.InitState(finSeed);
            Random.Shift(100); // 60 (tail segments) + 20 (body chunks) * 2
            int scaleStart = Random.Range(1, 6);
            int numFins = Random.Range(6, 8);
            float finWidth = Mathf.Lerp(6f, 8f, Random.Value);
            // if (numFins > 6) finWidth *= 0.8f; // we can pretend this doesn't exist for simplicity :3
            for (int i = 0; i < numFins; i++)
            {
                float finData = 100f + 200f * Mathf.Sin(Mathf.Pow((float)i / (numFins - 1), 0.5f) * 3.1415927f);
                int segments = Mathf.FloorToInt(finData / 20f) + 1;
                Random.Shift((1 + segments) * 2);
            }
            Random.Shift(40);
            int numScales = Random.Range(20, 40);
            float scaleSize = Mathf.Lerp(0.5f, 1.2f, Mathf.Pow(Random.Value, 0.5f));

            return new LeviathanResults
            {
                colorA = colorA,
                colorB = colorB,
                numFins = numFins,
                finWidth = finWidth,
                scalesStart = scaleStart,
                numScales = numScales,
                scaleSize = scaleSize,
            };
        }

        public override float Execute(XORShift128 Random)
        {
            var results = GetResults(Random);
            float r = 0f;
            r += DistanceIf(results.colorA, ColorAInput);
            r += DistanceIf(results.colorB, ColorBInput.HueInput);
            r += DistanceIf(results.numFins, NumFinsInput);
            r += DistanceIf(results.finWidth, FinWidthInput);
            r += DistanceIf(results.scalesStart, ScaleStartInput);
            r += DistanceIf(results.numScales, NumScalesInput);
            r += DistanceIf(results.scaleSize, ScaleSizeInput);
            return r;
        }

        protected override IEnumerable<string> GetValues(XORShift128 Random)
        {
            var results = GetResults(Random);
            yield return $"Pattern color A: hsl({results.colorA.hue}, {results.colorA.saturation}, {results.colorA.lightness})";
            yield return $"Pattern color B: hsl({results.colorB}, 1f, 0.2f)";
            yield return null!;
            yield return $"Number of fins: {results.numFins}";
            yield return $"Fin width: {results.finWidth}";
            yield return null!;
            yield return $"Number of scales: {results.numScales}";
            yield return $"Scales start: {results.scalesStart}";
            yield return $"Scale size: {results.scaleSize}";
        }

        private struct LeviathanResults
        {
            public HSLColor colorA;
            public float colorB;
            public int numFins;
            public float finWidth;
            public int scalesStart;
            public int numScales;
            public float scaleSize;
        }
    }
}
