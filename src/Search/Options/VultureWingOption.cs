using System;
using System.Collections.Generic;
using FinderMod.Inputs;
using UnityEngine;

namespace FinderMod.Search.Options
{
    internal class VultureWingOption : Option
    {
        private readonly ColorHSLInput ColorAInput, ColorBInput;
        private readonly IntInput FeathersInput;

        public VultureWingOption() : base()
        {
            elements = [
                ColorAInput = new ColorHSLInput("Color A", 0.9f, 1.6f, 0.5f, 0.7f, 0.7f, 0.8f) { fixColors = true },
                ColorBInput = new ColorHSLInput("Color B", 0.65f, 1.85f, 0.8f, 1f, 0.45f, 1f) { fixColors = true },
                FeathersInput = new IntInput("Feather count", 13, 19)
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
            float nha, nsa, nla, nhb, nsb, nlb;

            nha = Mathf.Lerp(0.9f, 1.6f, Random.Value);
            nsa = Mathf.Lerp(0.5f, 0.7f, Random.Value);
            nla = Mathf.Lerp(0.7f, 0.8f, Random.Value);
            nhb = nha + Mathf.Lerp(-0.25f, 0.25f, Random.Value);
            nsb = Mathf.Lerp(0.8f, 1f, 1f - Random.Value * Random.Value);
            nlb = Mathf.Lerp(0.45f, 1f, Random.Value * Random.Value);

            nha %= 1f; nhb %= 1f;

            // Wing feather count
            int nf = Random.Range(13, 20);

            return new Results
            {
                a = new HSLColor(nha, nsa, nla),
                b = new HSLColor(nhb, nsb, nlb),
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
