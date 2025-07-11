﻿using System.Collections.Generic;
using FinderMod.Inputs;
using RWCustom;
using UnityEngine;

namespace FinderMod.Search.Options
{
    internal class ScavSkillsOption : Option
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

        private struct Results
        {
            public float dge, mid, mle, blk, rea;
        }

        private Results GetResults(XORShift128 Random)
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

            return new Results
            {
                dge = dge,
                mid = mid,
                mle = mle,
                blk = blk,
                rea = rea
            };
        }

        public override float Execute(XORShift128 Random)
        {
            var results = GetResults(Random);

            float r = 0f;
            r += DistanceIf(results.dge, dgeInp);
            r += DistanceIf(results.mid, midInp);
            r += DistanceIf(results.mle, mleInp);
            r += DistanceIf(results.blk, blkInp);
            r += DistanceIf(results.rea, reaInp);
            return r;
        }

        protected override IEnumerable<string> GetValues(XORShift128 Random)
        {
            var results = GetResults(Random);

            yield return $"Dodge: {results.dge}";
            yield return $"Mid-range: {results.mid}";
            yield return $"Melee: {results.mle}";
            yield return $"Blocking: {results.blk}";
            yield return $"Reaction: {results.rea}";
            yield break;
        }
    }
}
