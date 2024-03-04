using System;
using FinderMod.Inputs;
using RWCustom;
using UnityEngine;

namespace FinderMod.Search.Options.Scavs
{
    internal class OScavSkills() : IOption(17, false)
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

        public override void Run(float[] i, float[] o, SearchData data)
        {
            float dge, mid, mle, blk, rea;
            var p = data.Personality;

            dge = Custom.PushFromHalf(Mathf.Lerp(i[0] < 0.5f ? p.Nervous : p.Aggression, i[1], i[2]), 1f + i[3]);
            mid = Custom.PushFromHalf(Mathf.Lerp(i[4] < 0.5f ? p.Energy : p.Aggression, i[5], i[6]), 1f + i[7]);
            mle = Custom.PushFromHalf(i[8], 1f + i[9]);
            blk = Custom.PushFromHalf(Mathf.InverseLerp(0.35f, 1f, Mathf.Lerp(i[10] < 0.5f ? p.Bravery : p.Energy, i[11], i[12])), 1f + i[13]);
            rea = Custom.PushFromHalf(Mathf.Lerp(p.Energy, i[14], i[15]), 1f + i[16]);

            if (ModManager.MSC)
            {
                float n = 1f - p.Dominance;
                dge = Mathf.Lerp(dge, 0f, n * 0.085f); // Dodge
                mid = Mathf.Lerp(mid, 0f, n * 0.085f); // Mid-range
                blk = Mathf.Lerp(blk, 0f, n * 0.05f);  // Blocking
                rea = Mathf.Lerp(rea, 0f, n * 0.15f);  // Reaction
            }

            o[0] = dge;
            o[1] = mid;
            o[2] = mle;
            o[3] = blk;
            o[4] = rea;
        }
    }
}
