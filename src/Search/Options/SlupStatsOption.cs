using System.Collections.Generic;
using FinderMod.Inputs;
using UnityEngine;

namespace FinderMod.Search.Options
{
    public class SlupStatsOption : Option
    {
        private readonly FloatInput wgtInp, vz0Inp, vz1Inp, louInp, lngInp, polInp, tunInp, spdInp;

        public SlupStatsOption() : base()
        {
            elements = [
                wgtInp = new FloatInput("Body weight", 0.5525f, 0.715f),
                vz0Inp = new FloatInput("Visibility (standing)", -0.24f, -0.16f),
                vz1Inp = new FloatInput("Visibility (crouching)", 0.45f, 0.75f),
                louInp = new FloatInput("Loudness", 0.4f, 0.6f),
                lngInp = new FloatInput("Lung capacity", 0.64f, 0.96f),
                polInp = new FloatInput("Pole climbing speed", 0.68f, 1f),
                tunInp = new FloatInput("Tunnel crawling speed", 0.68f, 1f),
                spdInp = new FloatInput("Running speed", 0.68f, 1f),
            ];
        }

        private Results GetResults(XORShift128 Random)
        {
            float bal, met, stl, wde;

            // Physical attributes
            bal = Mathf.Pow(Random.Range(0f, 1f), 1.5f);
            met = Mathf.Pow(Random.Range(0f, 1f), 1.5f);
            stl = Mathf.Pow(Random.Range(0f, 1f), 1.5f);
            wde = Mathf.Pow(Random.Range(0f, 1f), 1.5f);

            // Performance attributes
            float wgt = 0.65f; // body weight
            float vz0 = -0.2f; // visibility (standing)
            float vz1 = 0.6f;  // visibility (crouching)
            float lou = 0.5f;  // loudness factor
            float lng = 0.8f;  // lung capacity
            float pol = 0.8f;  // pole climbing speed
            float tun = 0.8f;  // corridor climbing speed
            float spd = 0.8f;  // running speed

            spd *= 0.85f + 0.15f * met + 0.15f * (1f - bal) + 0.1f * (1f - stl);
            wgt *= 0.85f + 0.15f * wde + 0.1f * met;
            vz0 *= 0.8f + 0.2f * (1f - stl) + 0.2f * met;
            vz1 *= 0.75f + 0.35f * stl + 0.15f * (1f - met);
            lou *= 0.8f + 0.2f * wde + 0.2f * (1f - stl);
            lng *= 0.8f + 0.2f * (1f - met) + 0.2f * (1f - stl);
            pol *= 0.85f + 0.15f * met + 0.15f * bal + 0.1f * (1f - stl);
            tun *= 0.85f + 0.15f * met + 0.15f * (1f - bal) + 0.1f * (1f - stl);

            return new Results
            {
                wgt = wgt,
                vz0 = vz0,
                vz1 = vz1,
                lou = lou,
                lng = lng,
                pol = pol,
                tun = tun,
                spd = spd,
            };
        }

        public override float Execute(XORShift128 Random)
        {
            var results = GetResults(Random);

            float r = 0f;
            r += DistanceIf(results.wgt, wgtInp);
            r += DistanceIf(results.vz0, vz0Inp);
            r += DistanceIf(results.vz1, vz1Inp);
            r += DistanceIf(results.lou, louInp);
            r += DistanceIf(results.lng, lngInp);
            r += DistanceIf(results.pol, polInp);
            r += DistanceIf(results.tun, tunInp);
            r += DistanceIf(results.spd, spdInp);
            return r;
        }

        protected override IEnumerable<string> GetValues(XORShift128 Random)
        {
            var results = GetResults(Random);

            // Compile results
            yield return $"Body weight: {results.wgt}";
            yield return $"Visibility (standing): {results.vz0}";
            yield return $"Visibility (crouching): {results.vz1}";
            yield return $"Loudness: {results.lou}";
            yield return $"Lung capacity: {results.lng}";
            yield return $"Pole climbing speed: {results.pol}";
            yield return $"Tunnel crawling speed: {results.tun}";
            yield return $"Running speed: {results.spd}";
        }

        private struct Results
        {
            public float wgt;
            public float vz0;
            public float vz1;
            public float lou;
            public float lng;
            public float pol;
            public float tun;
            public float spd;
        }
    }
}
