using System;
using FinderMod.Inputs;
using RWCustom;
using UnityEngine;

namespace FinderMod.Search.Options.Scavs
{
    internal class OEliteScavSkills() : IOption(17, true)
    {
        public override BaseInput[] CreateInputs()
        {
            return [
                new FloatInput("Dodge"),
                new FloatInput("Mid-range"),
                new FloatInput("Melee"),
                new FloatInput("Blocking"),
                new FloatInput("Reaction")
            ];
        }

        public override void Run(float[] input, float[] output, SearchData data)
        {
            float dge, mid, mle, blk, rea;
            var p = data.Personality;

            dge = Custom.PushFromHalf(Mathf.Lerp(input[0] < 0.5f ? p.Nervous : p.Aggression, input[1], input[2]), 1f + input[3]);
            mid = Custom.PushFromHalf(Mathf.Lerp(input[4] < 0.5f ? p.Energy : p.Aggression, input[5], input[6]), 1f + input[7]);
            mle = Custom.PushFromHalf(input[8], 1f + input[9]);
            blk = Custom.PushFromHalf(Mathf.InverseLerp(0.35f, 1f, Mathf.Lerp(input[10] < 0.5f ? p.Bravery : p.Energy, input[11], input[12])), 1f + input[13]);
            rea = Custom.PushFromHalf(Mathf.Lerp(p.Energy, input[14], input[15]), 1f + input[16]);

            float n = Mathf.Lerp(p.Dominance, 1f, 0.15f);

            dge = Mathf.Lerp(dge, 1f, n * 0.15f); // Dodge
            mid = Mathf.Lerp(mid, 1f, n * 0.1f); // Mid-range
            blk = Mathf.Lerp(blk, 1f, n * 0.1f);  // Blocking
            rea = Mathf.Lerp(rea, 1f, n * 0.05f);  // Reaction

            output[0] = dge;
            output[1] = mid;
            output[2] = mle;
            output[3] = blk;
            output[4] = rea;
        }
    }
}
