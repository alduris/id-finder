using System;
using System.Collections.Generic;
using FinderMod.Inputs;
using UnityEngine;

namespace FinderMod.Search.Options
{
    public class DrillCrabVarsOption : Option
    {
        private readonly FloatInput LegThicknessInput;
        private readonly FloatInput LegLengthInput;
        private readonly FloatInput BodySizeInput;
        private readonly FloatInput DrillSizeInput;
        private readonly FloatInput EyestalkLengthInput;
        private readonly FloatInput EyeLightnessInput;

        public DrillCrabVarsOption()
        {
            elements = [
                LegThicknessInput = new FloatInput("Leg thickness"),
                LegLengthInput = new FloatInput("Leg length"),
                BodySizeInput = new FloatInput("Body size"),
                DrillSizeInput = new FloatInput("Drill size"),
                EyestalkLengthInput = new FloatInput("Eyestalk length"),
                EyeLightnessInput = new FloatInput("Eye lightness"),
                ];
        }

        private DrillCrabResults GetResults(XORShift128 Random)
        {
            var p = new Personality(Random);

            Random.Shift(2);
            float legThickness = Mathf.Lerp(p.dom, Random.Value, Random.Value);
            // float legUniformity = Mathf.Lerp(1f - p.nrg, Random.Value, Random.Value);
            Random.Shift(2);
            float legLength = Mathf.Lerp(p.dom, Random.Value, Random.Value);
            float bodySize = Mathf.Lerp(Mathf.Max(1f - p.nrg, p.dom), Random.Value, Random.Value);
            float drillSize = Mathf.Lerp(Mathf.Max(p.agg, p.dom), Random.Value, Random.Value);
            float eyestalkLength = Random.Value;
            if (Random.Value < 0.25f)
            {
                Random.Shift(3);
            }
            float eyeLightness = Random.Value;

            return new DrillCrabResults
            {
                legThickness = legThickness,
                legLength = legLength,
                bodySize = bodySize,
                drillSize = drillSize,
                eyestalkLength = eyestalkLength,
                eyeLightness = eyeLightness
            };
        }

        public override float Execute(XORShift128 Random)
        {
            var results = GetResults(Random);

            float r = 0f;
            r += DistanceIf(results.legThickness, LegThicknessInput);
            r += DistanceIf(results.legLength, LegLengthInput);
            r += DistanceIf(results.bodySize, BodySizeInput);
            r += DistanceIf(results.drillSize, DrillSizeInput);
            r += DistanceIf(results.eyestalkLength, EyestalkLengthInput);
            r += DistanceIf(results.eyeLightness, EyeLightnessInput);

            return r;
        }

        protected override IEnumerable<string> GetValues(XORShift128 Random)
        {
            var results = GetResults(Random);

            yield return $"Leg thickness: {results.legThickness}";
            yield return $"Leg length: {results.legLength}";
            yield return $"Body size: {results.bodySize}";
            yield return $"Drill size: {results.drillSize}";
            yield return $"Eyestalk length: {results.eyestalkLength}";
            yield return $"Eye lightness: {results.eyeLightness}";
        }

        private struct DrillCrabResults
        {
            public float legThickness;
            public float legLength;
            public float bodySize;
            public float drillSize;
            public float eyestalkLength;
            public float eyeLightness;
        }
    }
}
