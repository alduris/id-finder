using System;
using FinderMod.Inputs;
using RWCustom;
using UnityEngine;

namespace FinderMod.Search.Options
{
    public class EliteScavSkillsOption : Option
    {
        private readonly FloatInput dgeInp, midInp, mleInp, blkInp, reaInp;

        public EliteScavSkillsOption() : base()
        {
            elements = [
                dgeInp = new FloatInput("Dodge"),
                midInp = new FloatInput("Mid-range"),
                mleInp = new FloatInput("Melee"),
                blkInp = new FloatInput("Blocking"),
                reaInp = new FloatInput("Reaction")
            ];
        }

        public override float Execute(XORShift128 Random)
        {
            float dge, mid, mle, blk, rea;
            Personality p = new(Random);

            dge = Custom.PushFromHalf(Mathf.Lerp((Random.Value < 0.5f) ? p.nrv : p.agg, Random.Value, Random.Value), 1f + Random.Value);
            mid = Custom.PushFromHalf(Mathf.Lerp((Random.Value < 0.5f) ? p.nrg : p.agg, Random.Value, Random.Value), 1f + Random.Value);
            mle = Custom.PushFromHalf(Random.Value, 1f + Random.Value);
            blk = Custom.PushFromHalf(Mathf.InverseLerp(0.35f, 1f, Mathf.Lerp((Random.Value < 0.5f) ? p.brv : p.nrg, Random.Value, Random.Value)), 1f + Random.Value);
            rea = Custom.PushFromHalf(Mathf.Lerp(p.nrg, Random.Value, Random.Value), 1f + Random.Value);

            float n = Mathf.Lerp(p.dom, 1f, 0.15f);
            dge = Mathf.Lerp(dge, 1f, n * 0.15f); // Dodge
            mid = Mathf.Lerp(mid, 1f, n * 0.1f); // Mid-range
            blk = Mathf.Lerp(blk, 1f, n * 0.1f);  // Blocking
            rea = Mathf.Lerp(rea, 1f, n * 0.05f);  // Reaction

            float r = 0f;
            if (dgeInp.enabled) r += Mathf.Abs(dge - dgeInp.value);
            if (midInp.enabled) r += Mathf.Abs(mid - midInp.value);
            if (mleInp.enabled) r += Mathf.Abs(mle - mleInp.value);
            if (blkInp.enabled) r += Mathf.Abs(blk - blkInp.value);
            if (reaInp.enabled) r += Mathf.Abs(rea - reaInp.value);
            return r;
        }
    }
}
