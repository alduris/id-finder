using System;
using System.Collections.Generic;
using FinderMod.Inputs;
using RWCustom;
using UnityEngine;

namespace FinderMod.Search.Options
{
    internal class SnailOption : Option
    {
        private FloatInput sizeInput;
        private ColorRGBInput colorAInput, colorBInput;
        private BoolInput sameInput;

        public SnailOption()
        {
            elements = [
                sizeInput = new FloatInput("Size", 0.6f, 1.4f),
                colorAInput = new ColorRGBInput("Color A") { description = "Head side" },
                colorBInput = new ColorRGBInput("Color B") { description = "Tail side" },
                sameInput = new BoolInput("Same Color?", false) { enabled = false }
            ];
        }

        private struct SnailResults
        {
            public float size;
            public Color colorA;
            public Color colorB;
            public bool same;
        }

        private SnailResults GetResults(XORShift128 Random)
        {
            var rand = Random.Value;
            float sizeDiff = Mathf.Lerp(0.2f, 0.4f, rand);
            float size = Mathf.Lerp(1f - sizeDiff, 1f + sizeDiff, rand); // technically the same number as the last one
            Color a, b;
            float hue1 = Mathf.Lerp(0.85f, 1.1f, Random.Value);
            if (hue1 > 1f)
            {
                hue1 -= 1f;
            }
            float hue2 = Mathf.Lerp(0.85f, 1.1f, Random.Value);
            if (hue2 > 1f)
            {
                hue2 -= 1f;
            }
            a = Custom.HSL2RGB(hue1, Mathf.Lerp(0.7f, 1f, 1f - Mathf.Pow(Random.Value, 3f)), Mathf.Lerp(0f, 0.3f, Mathf.Pow(Random.Value, 2f)));
            b = Custom.HSL2RGB(Mathf.Lerp(hue2, hue1, Mathf.Pow(Random.Value, 3f)), Mathf.Lerp(0.7f, 1f, 1f - Mathf.Pow(Random.Value, 3f)), Mathf.Lerp(0.05f, 1f, Mathf.Pow(Random.Value, 3f)));

            bool same = (Random.Value >= 0.8f);
            a = Color.Lerp(a, b, same ? 1f : Mathf.Pow(Random.Value, 3f));

            a = new Color(Mathf.Max(a.r, 0.007843138f), Mathf.Max(a.g, 0.007843138f), Mathf.Max(a.b, 0.007843138f));
            b = new Color(Mathf.Max(b.r, 0.007843138f), Mathf.Max(b.g, 0.007843138f), Mathf.Max(b.b, 0.007843138f));

            return new SnailResults
            {
                size = size,
                colorA = a,
                colorB = b,
                same = same
            };
        }

        public override float Execute(XORShift128 Random)
        {
            var results = GetResults(Random);

            float r = 0f;
            r += DistanceIf(results.size, sizeInput);
            r += DistanceIf(results.colorA, colorAInput);
            r += DistanceIf(results.colorB, colorBInput);
            r += DistanceIf(results.same, sameInput);
            return r;
        }

        protected override IEnumerable<string> GetValues(XORShift128 Random)
        {
            var results = GetResults(Random);

            yield return $"Size: {results.size}";
            yield return $"Color A: rgb({results.colorA.r}, {results.colorA.g}, {results.colorA.b})";
            yield return $"Color B: rgb({results.colorB.r}, {results.colorB.g}, {results.colorB.b})";
            yield return $"Same color? {(results.same ? "Yes" : "No")}";
            yield break;
        }
    }
}
