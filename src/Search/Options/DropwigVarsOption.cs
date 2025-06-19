using System;
using System.Collections.Generic;
using FinderMod.Inputs;
using UnityEngine;

namespace FinderMod.Search.Options
{
    internal class DropwigVarsOption : Option
    {
        private readonly FloatInput BodyThicknessInput;
        private readonly FloatInput LegsThicknessInput;
        private readonly FloatInput AntennaeLengthInput;
        private readonly FloatInput PinchersLengthInput;
        private readonly FloatInput ColoredAntennaeInput;
        private readonly HueInput ColorInput;

        public DropwigVarsOption()
        {
            elements = [
                BodyThicknessInput = new FloatInput("Body thickness", 0.6f, 1.4f),
                LegsThicknessInput = new FloatInput("Legs thickness", 0.6f, 1.4f),
                AntennaeLengthInput = new FloatInput("Antennae length", 0.6f, 1.4f),
                PinchersLengthInput = new FloatInput("Pinchers length", 40f, 60f),
                ColoredAntennaeInput = new FloatInput("Antennae color intensity"),
                ColorInput = new HueInput("Hue", 0.6722222f - 0.2f, 0.6722222f + 0.2f),
                ];
        }

        private DropwigResults GetResults(XORShift128 Random)
        {
            return new DropwigResults
            {
                bodyThickness = Mathf.Lerp(0.6f, 1.4f, Random.Value),
                legsThickness = Mathf.Lerp(0.6f, 1.4f, Random.Value),
                antennaeLength = Mathf.Lerp(0.5f, 1.5f, Random.Value),
                pinchersLength = Mathf.Lerp(40f, 60f, Random.Value),
                coloredAntennae = ((Random.Value < 0.5f) ? Random.Value : 0f),
                hue = 0.6722222f + (1f - ClampedRandomVariation(0.5f, 0.5f, 0.3f, Random) * 2f) * 0.2f // fancy code for "add or subtract 0.2 but with weird curves"
            };
        }

        public override float Execute(XORShift128 Random)
        {
            Random.Shift(10);
            var results = GetResults(Random);
            return DistanceIf(results.bodyThickness, BodyThicknessInput)
                + DistanceIf(results.legsThickness, LegsThicknessInput)
                + DistanceIf(results.antennaeLength, AntennaeLengthInput)
                + DistanceIf(results.pinchersLength, PinchersLengthInput) / 20f
                + DistanceIf(results.coloredAntennae, ColoredAntennaeInput)
                + DistanceIf(results.hue, ColorInput);
        }

        protected override IEnumerable<string> GetValues(XORShift128 Random)
        {
            var results = GetResults(Random);
            yield return $"Body thickness: {results.bodyThickness}";
            yield return $"Legs thickness: {results.legsThickness}";
            yield return $"Antennae length: {results.antennaeLength}";
            yield return $"Pinchers length: {results.pinchersLength}";
            yield return $"Antennae color intensity: {results.coloredAntennae}";
            yield return $"Hue: {results.hue}";
        }

        private struct DropwigResults
        {
            public float bodyThickness;
            public float legsThickness;
            public float antennaeLength;
            public float pinchersLength;
            public float coloredAntennae;
            public float hue;
        }
    }
}
