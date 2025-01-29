using System;
using FinderMod.Inputs;
using UnityEngine;

namespace FinderMod.Search.Options
{
    public class KingVultureWingOption : Option
    {
        private readonly ColorHSLInput colorA, colorB;
        private readonly IntInput countInp;

        public KingVultureWingOption() : base("King Vulture Wing Variations")
        {
            elements = [
                colorA = new ColorHSLInput("Color A"),
                colorB = new ColorHSLInput("Color B"),
                countInp = new IntInput("Feather count", 15, 24)
            ];
        }

        public override float Execute(XORShift128 Random)
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

            // Return result
            float r = 0f;
            r += DistanceIf(kha, colorA.HueInput);
            r += DistanceIf(ksa, colorA.SatInput);
            r += DistanceIf(kla, colorA.LightInput);
            r += DistanceIf(khb, colorB.HueInput);
            r += DistanceIf(ksb, colorB.SatInput);
            r += DistanceIf(klb, colorB.LightInput);
            if (countInp.enabled) r += Math.Abs(nf - countInp.Value);

            return r;
        }
    }
}
