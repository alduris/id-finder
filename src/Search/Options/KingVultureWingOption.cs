using System;
using System.Collections.Generic;
using FinderMod.Inputs;
using UnityEngine;

namespace FinderMod.Search.Options
{
    internal class KingVultureWingOption : Option
    {
        private readonly ColorHSLInput ColorAInput, ColorBInput;
        private readonly IntInput FeathersInput;

        public KingVultureWingOption() : base()
        {
            elements = [
                ColorAInput = new ColorHSLInput("Color A", 0.68f, 1.33f, 0.5f, 0.7f, 0.7f, 0.8f) { fixColors = true },
                ColorBInput = new ColorHSLInput("Color B", 0.93f, 1.07f, 0.8f, 1f, 0.45f, 1f) { fixColors = true },
                FeathersInput = new IntInput("Feather count", 15, 24)
            ];
        }

        private struct Results
        {
            public HSLColor a, b;
            public int feathers;
        }

        private Results GetResults(XORShift128 Random)
        {
            // Color variables
            float kha, ksa, kla, khb, ksb, klb;

            khb = Mathf.Lerp(0.93f, 1.07f, Random.Value);
            ksb = Mathf.Lerp(0.8f, 1f, 1f - Random.Value * Random.Value);
            klb = Mathf.Lerp(0.45f, 1f, Random.Value * Random.Value);
            kha = khb + Mathf.Lerp(-0.25f, 0.25f, Random.Value);
            ksa = Mathf.Lerp(0.5f, 0.7f, Random.Value);
            kla = Mathf.Lerp(0.7f, 0.8f, Random.Value);

            kha %= 1f; khb %= 1f;

            // Wing feather count
            int nf = Random.Range(15, 25);

            return new Results
            {
                a = new HSLColor(kha, ksa, kla),
                b = new HSLColor(khb, ksb, klb),
                feathers = nf,
            };
        }

        public override float Execute(XORShift128 Random)
        {
            var results = GetResults(Random);

            // Return result
            float r = 0f;
            r += DistanceIf(results.a, ColorAInput);
            r += DistanceIf(results.b, ColorBInput);
            r += DistanceIf(results.feathers, FeathersInput);

            return r;
        }

        protected override IEnumerable<string> GetValues(XORShift128 Random)
        {
            var results = GetResults(Random);

            // Relay results
            yield return $"Color A: hsl({results.a.hue}, {results.a.saturation}, {results.a.lightness})";
            yield return $"Color A: hsl({results.b.hue}, {results.b.saturation}, {results.b.lightness})";
            yield return $"Feather count: {results.feathers}";
        }
    }
}
