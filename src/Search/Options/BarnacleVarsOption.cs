using System;
using System.Collections.Generic;
using FinderMod.Inputs;
using RWCustom;
using UnityEngine;

namespace FinderMod.Search.Options
{
    internal class BarnacleVarsOption : Option
    {
        private readonly FloatInput BodySizeInput;
        private readonly FloatInput EyeSizeInput;
        private readonly FloatInput LegThicknessInput;

        private readonly IntInput NumConesInput;
        private readonly FloatInput ConeRadVarianceInput;
        private readonly FloatInput ConeLengthVarianceInput;
        private readonly FloatInput ConeColorVarianceInput;

        private readonly ColorHSLInput ConeColorInput;
        private readonly FloatInput DarkColorInput;
        private readonly FloatInput LightColorInput;

        public BarnacleVarsOption()
        {
            elements = [
                BodySizeInput = new FloatInput("Body size", 0.6f, 1f),
                EyeSizeInput = new FloatInput("Eye size", 0.7f, 1f),
                LegThicknessInput = new FloatInput("Leg thickness", 0.7f, 1.3f),
                new Whitespace(),
                NumConesInput = new IntInput("Number of cones", 13, 19),
                ConeRadVarianceInput = new FloatInput("Cone radius variance"),
                ConeLengthVarianceInput = new FloatInput("Cone length variance"),
                ConeColorVarianceInput = new FloatInput("Cone color variance"),
                new Whitespace(),
                ConeColorInput = new ColorHSLInput("Cone base color", -0.06f, 0.14f, 0.3f, 0.7f, 0.55f, 0.95f) { fixColors = true },
                DarkColorInput = new FloatInput("Low-end lightness offset", -0.2f, -0.1f),
                LightColorInput = new FloatInput("High-end lightness offset", 0f, 0.1f)
                ];
        }

        private BarnacleResults GetResults(XORShift128 Random)
        {
            var p = new Personality(Random);

            float bodySize = Mathf.Lerp(p.dom, Random.Value, Random.Value) * 0.4f + 0.6f;
            float eyeSize = Random.Range(0.7f, 1f);
            int numCones = Random.Range(13, 20);
            float coneRadVariance = Random.Value;
            float coneLengthVariance = Custom.PushFromHalf(Random.Value, 1.5f);
            float coneColorVariance = Mathf.Pow(Random.Value, 0.5f);
            float legThickness = Mathf.Lerp(0.7f, 1.3f, Custom.PushFromHalf(Random.Value, 0.7f));
            HSLColor bodyColor = new(WrappedRandomVariation(0.04f, 0.1f, 0.1f, Random), ClampedRandomVariation(0.5f, 0.2f, 0.3f, Random), ClampedRandomVariation(0.75f, 0.2f, 1.5f, Random));
            float darkFac = -Random.Range(0.1f, 0.2f);
            float lightFac = Random.Range(0f, 0.1f);

            return new BarnacleResults
            {
                bodySize = bodySize,
                eyeSize = eyeSize,
                legThickness = legThickness,
                numCones = numCones,
                coneRadVariance = coneRadVariance,
                coneLengthVariance = coneLengthVariance,
                coneColorVariance = coneColorVariance,
                coneColor = bodyColor,
                darkColor = darkFac,
                lightColor = lightFac,
            };
        }

        public override float Execute(XORShift128 Random)
        {
            var results = GetResults(Random);
            return DistanceIf(results.bodySize, BodySizeInput)
                + DistanceIf(results.eyeSize, EyeSizeInput)
                + DistanceIf(results.legThickness, LegThicknessInput)
                + DistanceIf(results.numCones, NumConesInput)
                + DistanceIf(results.coneRadVariance, ConeRadVarianceInput)
                + DistanceIf(results.coneLengthVariance, ConeLengthVarianceInput)
                + DistanceIf(results.coneColorVariance, ConeColorVarianceInput)
                + DistanceIf(results.coneColor, ConeColorInput)
                + DistanceIf(results.darkColor, DarkColorInput)
                + DistanceIf(results.lightColor, LightColorInput)
                ;
        }

        protected override IEnumerable<string> GetValues(XORShift128 Random)
        {
            var results = GetResults(Random);
            yield return $"Body size: {results.bodySize}";
            yield return $"Eye size: {results.eyeSize}";
            yield return $"Leg thickness: {results.legThickness}";
            yield return null!;
            yield return $"Number of cones: {results.numCones}";
            yield return $"Cone radius variance: {results.coneRadVariance}";
            yield return $"Cone length variance: {results.coneLengthVariance}";
            yield return $"Cone color variance: {results.coneColorVariance}";
            yield return null!;
            yield return $"Cone base color: hsl({results.coneColor.hue}, {results.coneColor.saturation}, {results.coneColor.lightness})";
            yield return $"Low-end lightness offset: {results.darkColor}";
            yield return $"High-end lightness offset: {results.lightColor}";
        }

        private struct BarnacleResults
        {
            public float bodySize;
            public float eyeSize;
            public float legThickness;
            public int numCones;
            public float coneRadVariance;
            public float coneLengthVariance;
            public float coneColorVariance;
            public HSLColor coneColor;
            public float darkColor;
            public float lightColor;
        }

        // Go lesbiabs! The ids to make a lesbian flag from barnacles:
        // 83214
        // 2509
        // 75652
        // 41122
        // 908
    }
}
