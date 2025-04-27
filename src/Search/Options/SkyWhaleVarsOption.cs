using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using FinderMod.Inputs;
using UnityEngine;

namespace FinderMod.Search.Options
{
    public class SkyWhaleVarsOption : Option
    {
        private readonly FloatInput SizeMultiplierInput;
        private readonly FloatInput FinSharpnessInput;

        private readonly FloatInput EyeOnBodyAngleInput;
        private readonly FloatInput EyeOnBodySegmentInput;

        private readonly Toggleable<IntInput> NumFrecklesInput;
        private readonly Toggleable<IntInput> NumScarsInput;
        private readonly Toggleable<IntInput> NumArmorPlatesInput;

        public SkyWhaleVarsOption()
        {
            elements = [
                SizeMultiplierInput = new FloatInput("Size multiplier", 3.75f, 4.25f),
                FinSharpnessInput = new FloatInput("Fin sharpness"),
                new Whitespace(),
                EyeOnBodyAngleInput = new FloatInput("Eye on body angle (deg)", 95f, 125f),
                EyeOnBodySegmentInput = new FloatInput("Eye on body segment", 6.7f, 7.7f),
                new Whitespace(),
                new Group([
                    new Label("Markings"),
                    NumFrecklesInput = new Toggleable<IntInput>("Has freckles", true, new IntInput("Number of freckles", 20, 59)),
                    NumScarsInput = new Toggleable<IntInput>("Has scars", true, new IntInput("Number of scars", 0, 80)),
                    NumArmorPlatesInput = new Toggleable<IntInput>("Has armor plates", true, new IntInput("Number of armor plates", 4, 8)),
                    ], "Markings group"),
                ];
        }

        private SkyWhaleResults GetResults(XORShift128 Random)
        {
            // SkyWhale.GenerateIVars()
            var (x, y, z, w) = (Random.x, Random.y, Random.z, Random.w);
            float sizeFac = Random.Value;
            float sizeMultiplier = 3.75f + 0.5f * sizeFac;
            float ruggedness = Mathf.Pow(Mathf.Lerp(sizeFac, Random.Value, Random.Value), 1.5f);
            float finSharpness = Random.Value;
            Random.InitState(x, y, z, w);

            // SkyWhaleGraphics.ctor()
            Random.Shift(5); // tail segments
            int? numFreckles = null;
            int? numScars = null;
            int? numArmorPlates = null;

            if (Random.Value < Mathf.Pow(1f - ruggedness, 2f))
            {
                // SkyWhaleGraphics.AddFreckles()
                if (Random.Value < 0.5f) Random.Shift();
                numFreckles = Random.Range(20, 60);
                Random.Shift(3 * numFreckles.Value);
            }

            if (Random.Value < ruggedness * 3f)
            {
                // SkyWhaleGraphics.AddScars()
                numScars = (int)(Random.Range(30f, 80f) * ruggedness);
                Random.Shift(7 * numScars.Value);
            }

            if (Random.Value < 0.4f * ruggedness + 0.2f)
            {
                // SkyWhaleGraphics.GenerateArmor()
                Random.Shift();
                numArmorPlates = Random.Range(4, 9);
            }

            float eyeOnBodyAngle = 110f + Random.Range(-15f, 15f); // code calculates this in degrees then converts to radians. I cut the last part
            float eyeOnBodySegment = 7.2f + Random.Range(-0.5f, 0.5f);

            return new SkyWhaleResults
            {
                sizeMultiplier = sizeMultiplier,
                finSharpness = finSharpness,

                eyeAngle = eyeOnBodyAngle,
                eyeSegment = eyeOnBodySegment,

                numFreckles = numFreckles,
                numScars = numScars,
                numArmorPlates = numArmorPlates
            };
        }

        public override float Execute(XORShift128 Random)
        {
            var results = GetResults(Random);
            float r = 0f;

            r += DistanceIf(results.sizeMultiplier, SizeMultiplierInput);
            r += DistanceIf(results.finSharpness, FinSharpnessInput);

            r += DistanceIf(results.eyeAngle, EyeOnBodyAngleInput);
            r += DistanceIf(results.eyeSegment, EyeOnBodySegmentInput);

            r += DistanceIf(results.numFreckles, NumFrecklesInput);
            r += DistanceIf(results.numScars, NumScarsInput);
            r += DistanceIf(results.numArmorPlates, NumArmorPlatesInput);

            return r;
        }

        protected override IEnumerable<string> GetValues(XORShift128 Random)
        {
            var results = GetResults(Random);

            yield return $"Size multiplier: {results.sizeMultiplier}";
            yield return $"Fin sharpness: {results.finSharpness}";
            yield return null!;
            yield return $"Eye on body angle: {results.eyeAngle} degrees";
            yield return $"Eye on body segment: {results.eyeSegment}";
            yield return null!;
            if (results.numFreckles.HasValue) yield return $"Has freckles. Count: {results.numFreckles.Value}";
            if (results.numScars.HasValue) yield return $"Has scars. Count: {results.numScars.Value}";
            if (results.numArmorPlates.HasValue) yield return $"Has armor plates. Count: {results.numArmorPlates.Value}";
        }

        private struct SkyWhaleResults
        {
            public float sizeMultiplier;
            public float finSharpness;

            public float eyeAngle;
            public float eyeSegment;

            public int? numArmorPlates;
            public int? numFreckles;
            public int? numScars;
        }
    }
}
