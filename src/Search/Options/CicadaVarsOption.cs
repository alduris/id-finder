using System;
using System.Collections.Generic;
using FinderMod.Inputs;
using UnityEngine;

namespace FinderMod.Search.Options
{
    internal class CicadaVarsOption : Option
    {
        private readonly HueInput ColorInput;
        private readonly FloatInput FatnessInput;
        private readonly FloatInput TentacleLengthInput;
        private readonly FloatInput TentacleThicknessInput;
        private readonly FloatInput WingLengthInput;
        private readonly FloatInput WingThicknessInput;
        private readonly Toggleable<IntInput> BustedWingInput;

        public CicadaVarsOption()
        {
            elements = [
                ColorInput = new HueInput("Hue", 0.45f, 0.65f),
                FatnessInput = new FloatInput("Fatness", 0.8f, 1.2f)
                { description = "For white squidcada, add 0.2 for in-game value. For black squidcada, subtract 0.2 for in-game value." },
                TentacleLengthInput = new FloatInput("Tentacle length", 0.6f, 1.4f),
                TentacleThicknessInput = new FloatInput("Tentacle thickness", 0.6f, 1.4f),
                WingLengthInput = new FloatInput("Wing length", 0.4f, 1f),
                WingThicknessInput = new FloatInput("Wing thickness", (0.66667f - 0.3f) * 1.5f, (0.66667f + 0.3f) * 1.5f),
                BustedWingInput = new Toggleable<IntInput>("Has busted wing", false, new IntInput("Busted wing", 0, 3) { description = "1/8 chance" })
                ];
        }

        private CicadaResults GetResults(XORShift128 Random)
        {
            float hue = ClampedRandomVariation(0.55f, 0.1f, 0.5f, Random);
            float fatness = ClampedRandomVariation(0.5f, 0.1f, 0.5f, Random) * 2f;
            int? brokenWing = null;
            if (Random.Value < 0.125f)
            {
                brokenWing = Random.Range(0, 4);
            }
            float tentacleLength = Mathf.Lerp(0.6f, 1.4f, Random.Value);
            float tentacleThickness = Mathf.Lerp(0.6f, 1.4f, Random.Value);
            float wingLength = Mathf.Lerp(1f, 0.4f, Random.Value * Random.Value);
            float wingThickness = ClampedRandomVariation(0.66667f, 0.3f, 0.2f, Random) * 1.5f;
            return new CicadaResults
            {
                hue = hue,
                fatness = fatness,
                tentacleLength = tentacleLength,
                tentacleThickness = tentacleThickness,
                wingLength = wingLength,
                wingThickness = wingThickness,
                bustedWing = brokenWing
            };
        }

        public override float Execute(XORShift128 Random)
        {
            var results = GetResults(Random);
            return DistanceIf(results.hue, ColorInput)
                + DistanceIf(results.fatness, FatnessInput)
                + DistanceIf(results.tentacleLength, TentacleLengthInput)
                + DistanceIf(results.tentacleThickness, TentacleThicknessInput)
                + DistanceIf(results.wingLength, WingLengthInput)
                + DistanceIf(results.wingThickness, WingThicknessInput)
                + DistanceIf(results.bustedWing, BustedWingInput);
        }

        protected override IEnumerable<string> GetValues(XORShift128 Random)
        {
            var results = GetResults(Random);
            yield return $"Hue: {results.hue}";
            yield return $"Fatness: {results.fatness}";
            yield return $"Tentacle length: {results.tentacleLength}";
            yield return $"Tentacle thickness: {results.tentacleThickness}";
            yield return $"Wing length: {results.wingLength}";
            yield return $"Wing thickness: {results.wingThickness}";
            if (results.bustedWing.HasValue)
                yield return $"Busted wing: {results.wingThickness}";
            else
                yield return "No busted wing";
        }

        private struct CicadaResults
        {
            public float hue;
            public float fatness;
            public float tentacleLength;
            public float tentacleThickness;
            public float wingLength;
            public float wingThickness;
            public int? bustedWing;
        }
    }
}
