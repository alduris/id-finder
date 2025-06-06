﻿using System;
using System.Collections.Generic;
using FinderMod.Inputs;
using RWCustom;
using UnityEngine;

namespace FinderMod.Search.Options
{
    public class ScavSkillsOption : Option
    {
        private readonly FloatInput dgeInp, midInp, mleInp, blkInp, reaInp;

        public ScavSkillsOption() : base()
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

            if (ModManager.MSC)
            {
                float n = 1f - p.dom;
                dge = Mathf.Lerp(dge, 0f, n * 0.085f); // Dodge
                mid = Mathf.Lerp(mid, 0f, n * 0.085f); // Mid-range
                blk = Mathf.Lerp(blk, 0f, n * 0.05f);  // Blocking
                rea = Mathf.Lerp(rea, 0f, n * 0.15f);  // Reaction
            }

            float r = 0f;
            if (dgeInp.enabled) r += Mathf.Abs(dge - dgeInp.value);
            if (midInp.enabled) r += Mathf.Abs(mid - midInp.value);
            if (mleInp.enabled) r += Mathf.Abs(mle - mleInp.value);
            if (blkInp.enabled) r += Mathf.Abs(blk - blkInp.value);
            if (reaInp.enabled) r += Mathf.Abs(rea - reaInp.value);
            return r;
        }

        protected override IEnumerable<string> GetValues(XORShift128 Random)
        {
            float dge, mid, mle, blk, rea;
            Personality p = new(Random);

            dge = Custom.PushFromHalf(Mathf.Lerp((Random.Value < 0.5f) ? p.nrv : p.agg, Random.Value, Random.Value), 1f + Random.Value);
            mid = Custom.PushFromHalf(Mathf.Lerp((Random.Value < 0.5f) ? p.nrg : p.agg, Random.Value, Random.Value), 1f + Random.Value);
            mle = Custom.PushFromHalf(Random.Value, 1f + Random.Value);
            blk = Custom.PushFromHalf(Mathf.InverseLerp(0.35f, 1f, Mathf.Lerp((Random.Value < 0.5f) ? p.brv : p.nrg, Random.Value, Random.Value)), 1f + Random.Value);
            rea = Custom.PushFromHalf(Mathf.Lerp(p.nrg, Random.Value, Random.Value), 1f + Random.Value);

            if (ModManager.MSC)
            {
                float n = 1f - p.dom;
                dge = Mathf.Lerp(dge, 0f, n * 0.085f); // Dodge
                mid = Mathf.Lerp(mid, 0f, n * 0.085f); // Mid-range
                blk = Mathf.Lerp(blk, 0f, n * 0.05f);  // Blocking
                rea = Mathf.Lerp(rea, 0f, n * 0.15f);  // Reaction
            }

            yield return $"Dodge: {dge}";
            yield return $"Mid-range: {mid}";
            yield return $"Melee: {mle}";
            yield return $"Blocking: {blk}";
            yield return $"Reaction: {rea}";
            yield break;
        }
    }
}
