using System;
using FinderMod.Inputs;
using UnityEngine;

namespace FinderMod.Search.Options
{
    public class VultureWingOption : Option
    {
        private readonly ColorHSLInput colorA, colorB;
        private readonly IntInput countInp;

        public VultureWingOption() : base("Vulture Wing Variations")
        {
            elements = [
                colorA = new ColorHSLInput("Color A"),
                colorB = new ColorHSLInput("Color B"),
                countInp = new IntInput("Feather count", 13, 19)
            ];
        }

        public override float Execute(XORShift128 Random)
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

            // Return result
            float r = 0f;
            r += DistanceIf(nha, colorA.HueInput);
            r += DistanceIf(nsa, colorA.SatInput);
            r += DistanceIf(nla, colorA.LightInput);
            r += DistanceIf(nhb, colorB.HueInput);
            r += DistanceIf(nsb, colorB.SatInput);
            r += DistanceIf(nlb, colorB.LightInput);
            if (countInp.enabled) r += Math.Abs(nf - countInp.Value);

            return r;
        }
    }
}
