using System;
using System.Collections.Generic;
using FinderMod.Inputs;
using UnityEngine;

namespace FinderMod.Search.Options
{
    public class JetfishVarsOption : Option
    {
        // private readonly FloatInput FatnessInput;
        private readonly FloatInput TentacleLengthInput;
        private readonly IntInput NumWhiskersInput;
        private readonly IntInput FlipperGraphicInput;
        private readonly FloatInput FlipperSizeInput;

        public JetfishVarsOption()
        {
            elements = [
                // FatnessInput = new FloatInput("Fatness", 0.8f, 1.2f),
                TentacleLengthInput = new FloatInput("Tentacle length"),
                NumWhiskersInput = new IntInput("Number of whiskers", 0, 3),
                FlipperGraphicInput = new IntInput("Flipper graphic", 0, 4),
                FlipperSizeInput = new FloatInput("Flipper size", 0.7f, 1.1f)
                ];
            // A brief note on why I removed the fatness input:
            // The only place it is used is determining the size of the body sprite, where it is used in Mathf.Lerp(0.8f, 1.0f, fatness).
            // This puts the actual range between 0.96f and 1f, which is unnoticeable to the human eye with how small of a sprite it is already.
        }

        private JetfishResults GetResults(XORShift128 Random)
        {
            float fatness = ClampedRandomVariation(0.5f, 0.1f, 0.5f, Random) * 2f;
            int numWhiskers = Random.Range(0, 4);
            float tentacleLength = Random.Value;
            Random.Shift();
            int flipperGraphic = Random.Range(0, 5);
            float flipperSize = Mathf.Lerp(0.7f, 1.1f, Random.Value);
            return new JetfishResults
            {
                fatness = fatness,
                numWhiskers = numWhiskers,
                tentacleLength = tentacleLength,
                flipperGraphic = flipperGraphic,
                flipperSize = flipperSize
            };
        }

        public override float Execute(XORShift128 Random)
        {
            var results = GetResults(Random);
            return 0f // DistanceIf(results.fatness, FatnessInput)
                + DistanceIf(results.tentacleLength, TentacleLengthInput)
                + DistanceIf(results.numWhiskers, NumWhiskersInput)
                + DistanceIf(results.flipperGraphic, FlipperGraphicInput)
                + DistanceIf(results.flipperSize, FlipperSizeInput);
        }

        protected override IEnumerable<string> GetValues(XORShift128 Random)
        {
            var results = GetResults(Random);
            // yield return $"Fatness: {results.fatness}";
            yield return $"Tentacle length: {results.tentacleLength}";
            yield return $"Number of whiskers: {results.numWhiskers}";
            yield return $"Flipper graphic: {results.flipperGraphic}";
            yield return $"Flipper size: {results.flipperSize}";
        }

        private struct JetfishResults
        {
            public float fatness;
            public float tentacleLength;
            public int numWhiskers;
            public int flipperGraphic;
            public float flipperSize;
        }
    }
}
