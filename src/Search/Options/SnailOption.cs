using System;
using System.Collections.Generic;
using FinderMod.Inputs;
using RWCustom;
using UnityEngine;

namespace FinderMod.Search.Options
{
    public class SnailOption : Option
    {
        private FloatInput sizeInput;
        private ColorRGBInput colorAInput, colorBInput;
        private BoolInput sameInput;

        public SnailOption()
        {
            elements = [
                sizeInput = new FloatInput("Size", 0.6f, 1.4f),
                colorAInput = new ColorRGBInput("Color A"),
                colorBInput = new ColorRGBInput("Color B"),
                sameInput = new BoolInput("Same Color?", false) { enabled = false }
            ];
        }

        public override float Execute(XORShift128 Random)
        {
            var rand = Random.Value;
            float num = Mathf.Lerp(0.2f, 0.4f, rand);
            float size = Mathf.Lerp(1f - num, 1f + num, rand);
            Color a, b;
            float num2 = Mathf.Lerp(0.85f, 1.1f, Random.Value);
            if (num2 > 1f)
            {
                num2 -= 1f;
            }
            float num3 = Mathf.Lerp(0.85f, 1.1f, Random.Value);
            if (num3 > 1f)
            {
                num3 -= 1f;
            }
            a = Custom.HSL2RGB(num2, Mathf.Lerp(0.7f, 1f, 1f - Mathf.Pow(Random.Value, 3f)), Mathf.Lerp(0f, 0.3f, Mathf.Pow(Random.Value, 2f)));
            b = Custom.HSL2RGB(Mathf.Lerp(num3, num2, Mathf.Pow(Random.Value, 3f)), Mathf.Lerp(0.7f, 1f, 1f - Mathf.Pow(Random.Value, 3f)), Mathf.Lerp(0.05f, 1f, Mathf.Pow(Random.Value, 3f)));

            bool same = (Random.Value >= 0.8f);
            a = Color.Lerp(a, b, same ? 1f : Mathf.Pow(Random.Value, 3f));

            a = new Color(Mathf.Max(a.r, 0.007843138f), Mathf.Max(a.g, 0.007843138f), Mathf.Max(a.b, 0.007843138f));
            b = new Color(Mathf.Max(b.r, 0.007843138f), Mathf.Max(b.g, 0.007843138f), Mathf.Max(b.b, 0.007843138f));

            float r = 0f;
            r += DistanceIf(size, sizeInput);
            r += DistanceIf(a, colorAInput);
            r += DistanceIf(b, colorBInput);
            r += DistanceIf(same, sameInput);
            return r;
        }

        protected override IEnumerable<string> GetValues(XORShift128 Random)
        {
            var rand = Random.Value;
            float sizeDiff = Mathf.Lerp(0.2f, 0.4f, rand);
            float size = Mathf.Lerp(1f - sizeDiff, 1f + sizeDiff, rand);
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

            yield return $"Size: {size}";
            yield return $"Color A: rgb({a.r}, {a.g}, {a.b})";
            yield return $"Color B: rgb({b.r}, {b.g}, {b.b})";
            yield return $"Same color? {(same ? "Yes" : "No")}";
            yield break;
        }
    }
}
