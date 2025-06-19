using System;
using System.Collections.Generic;
using FinderMod.Inputs;
using RWCustom;
using Unity.Burst;
using UnityEngine;

namespace FinderMod.Search.Options
{
    internal class ScavVarsOption : Option
    {
        private readonly FloatInput hsInp, etInp, esInp, enInp, eaInp, fatInp, wnInp, ntInp, psInp, hcbInp, lsInp, atInp, twInp;
        private readonly BoolInput dpInp, cetInp;
        private readonly IntInput tsInp;
        public ScavVarsOption() : base()
        {
            elements = [
                hsInp = new FloatInput("Head size"),
                etInp = new FloatInput("Eartler thickness"),
                new Whitespace(),
                esInp = new FloatInput("Eye size"),
                enInp = new FloatInput("Eye narrowness"),
                eaInp = new FloatInput("Eye angle") { description = "Eye angle varies from -5 (value=0) to 60 (value=1) degrees, though there are extra calculations" },
                new Whitespace(),
                fatInp = new FloatInput("Fatness"),
                wnInp = new FloatInput("Waist narrowness"),
                ntInp = new FloatInput("Neck thickness") { description = "Note: smaller values mean a thicker neck." },
                new Whitespace(),
                psInp = new FloatInput("Pupil size") { description = "Deep pupils always makes this 0.7" },
                dpInp = new BoolInput("Deep pupils?") { description = "Creates an inset look in the eyes. Makes pupil size constant." },
                new Whitespace(),
                hcbInp = new FloatInput("Hands color blend") { description = "Controls how much of the body (value=0) and the head (value=1) colors are used in the hands" },
                lsInp = new FloatInput("Leg size"),
                atInp = new FloatInput("Arm thickness"),
                new Whitespace(),
                cetInp = new BoolInput("Colored eartler tips?"),
                twInp = new FloatInput("Teeth wideness"),
                tsInp = new IntInput("Tail segments", 1, 4)
            ];
        }

        private struct Results
        {
            public float headSize;
            public float eartlerThickness;
            public float eyeSize;
            public float eyeNarrowness;
            public float eyeAngle;
            public float fatness;
            public float waistNarrowness;
            public float neckThickness;
            public float pupilSize;
            public bool deepPupils;
            public float handsColorBlend;
            public float legSize;
            public float armThickness;
            public bool coloredEartlerTips;
            public float teethWideness;
            public int tailSegments;
        }

        [BurstCompile]
        private Results GetResults(XORShift128 Random)
        {
            Personality p = new(Random);
            float generalMelanin = Custom.PushFromHalf(Random.Value, 2f);
            float headSize = ClampedRandomVariation(0.5f, 0.5f, 0.1f, Random);
            float eartlerWidth = Random.Value;
            float eyeSize = Mathf.Pow(Mathf.Max(0f, Mathf.Lerp(Random.Value, Mathf.Pow(headSize, 0.5f), Random.Value * 0.4f)), Mathf.Lerp(0.95f, 0.55f, p.sym));
            float narrowEyes = ((Random.Value < Mathf.Lerp(0.3f, 0.7f, p.sym)) ? 0f : Mathf.Pow(Random.Value, Mathf.Lerp(0.5f, 1.5f, p.sym)));
            float eyesAngle = Mathf.Pow(Random.Value, Mathf.Lerp(2.5f, 0.5f, Mathf.Pow(p.nrg, 0.03f)));
            float fatness = Mathf.Lerp(Random.Value, p.dom, Random.Value * 0.2f);
            if (p.nrg < 0.5f)
            {
                fatness = Mathf.Lerp(fatness, 1f, Random.Value * Mathf.InverseLerp(0.5f, 0f, p.nrg));
            }
            else
            {
                fatness = Mathf.Lerp(fatness, 0f, Random.Value * Mathf.InverseLerp(0.5f, 1f, p.nrg));
            }
            float narrowWaist = Mathf.Lerp(Mathf.Lerp(Random.Value, 1f - fatness, Random.Value), 1f - p.nrg, Random.Value);
            float neckThickness = Mathf.Lerp(Mathf.Pow(Random.Value, 1.5f - p.agg), 1f - fatness, Random.Value * 0.5f);
            float pupilSize = 0f;
            bool deepPupils = false;
            int coloredPupils = 0;
            if (Random.Value < 0.65f && eyeSize > 0.4f && narrowEyes < 0.3f)
            {
                if (Random.Value < Mathf.Pow(p.sym, 1.5f) * 0.8f)
                {
                    pupilSize = Mathf.Lerp(0.4f, 0.8f, Mathf.Pow(Random.Value, 0.5f));
                    if (Random.Value < 0.6666667f)
                    {
                        coloredPupils = Random.Range(1, 4);
                    }
                }
                else
                {
                    pupilSize = 0.7f;
                    deepPupils = true;
                }
            }
            float handsHeadColor;
            if (Random.Value < generalMelanin)
            {
                handsHeadColor = ((Random.Value < 0.3f) ? Random.Value : ((Random.Value < 0.6f) ? 1f : 0f));
            }
            else
            {
                handsHeadColor = ((Random.Value < 0.2f) ? Random.Value : ((Random.Value < 0.8f) ? 1f : 0f));
            }
            float legsSize = Random.Value;
            float armThickness = Mathf.Lerp(Random.Value, Mathf.Lerp(p.dom, fatness, 0.5f), Random.Value);
            bool coloredEartlerTips = Random.Value < 1f / Mathf.Lerp(1.2f, 10f, generalMelanin);
            float wideTeeth = Random.Value;
            int tailSegs = (Random.Value < 0.5f) ? 0 : Random.Range(1, 5);

            return new Results()
            {
                headSize = headSize,
                eartlerThickness = eartlerWidth,
                eyeSize = eyeSize,
                eyeNarrowness = narrowEyes,
                eyeAngle = eyesAngle,
                fatness = fatness,
                waistNarrowness = narrowWaist,
                neckThickness = neckThickness,
                pupilSize = pupilSize,
                deepPupils = deepPupils,
                handsColorBlend = handsHeadColor,
                legSize = legsSize,
                armThickness = armThickness,
                coloredEartlerTips = coloredEartlerTips,
                teethWideness = wideTeeth,
                tailSegments = tailSegs
            };
        }

        public override float Execute(XORShift128 Random)
        {
            var results = GetResults(Random);

            float r = 0f;
            r += DistanceIf(results.headSize, hsInp);
            r += DistanceIf(results.eartlerThickness, etInp);
            r += DistanceIf(results.eyeSize, esInp);
            r += DistanceIf(results.eyeNarrowness, enInp);
            r += DistanceIf(results.eyeAngle, eaInp);
            r += DistanceIf(results.fatness, fatInp);
            r += DistanceIf(results.waistNarrowness, wnInp);
            r += DistanceIf(results.neckThickness, ntInp);
            r += DistanceIf(results.pupilSize, psInp);
            r += DistanceIf(results.deepPupils, dpInp);
            r += DistanceIf(results.handsColorBlend, hcbInp);
            r += DistanceIf(results.legSize, lsInp);
            r += DistanceIf(results.armThickness, atInp);
            r += DistanceIf(results.coloredEartlerTips, cetInp);
            r += DistanceIf(results.teethWideness, twInp);
            r += DistanceIf(results.tailSegments, tsInp);

            return r;
        }

        protected override IEnumerable<string> GetValues(XORShift128 Random)
        {
            var results = GetResults(Random);

            yield return $"Head size: {results.headSize}";
            yield return $"Eartler thickness: {results.eartlerThickness}";
            yield return null!;
            yield return $"Eye size: {results.eyeSize}";
            yield return $"Eye narrowness: {results.eyeNarrowness}";
            yield return $"Eye angle: {results.eyeAngle}";
            yield return null!;
            yield return $"Fatness: {results.fatness}";
            yield return $"Waist narrowness: {results.waistNarrowness}";
            yield return $"Neck thickness: {results.neckThickness}";
            yield return null!;
            yield return $"Pupil size: {results.pupilSize}";
            yield return $"Has deep pupils: {(results.deepPupils ? "Yes" : "No")}";
            yield return null!;
            yield return $"Hands color blend: {results.handsColorBlend}";
            yield return $"Leg size: {results.legSize}";
            yield return $"Arm thickness: {results.armThickness}";
            yield return null!;
            yield return $"Has colored eartler tips: {(results.coloredEartlerTips ? "Yes" : "No")}";
            yield return $"Teeth wideness: {results.teethWideness}";
            yield return $"Tail segments: {results.tailSegments}";
            yield break;
        }
    }
}
