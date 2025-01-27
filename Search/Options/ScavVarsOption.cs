using System;
using FinderMod.Inputs;
using RWCustom;
using UnityEngine;

namespace FinderMod.Search.Options
{
    public class ScavVarsOption : Option
    {
        private readonly FloatInput hsInp, etInp, esInp, enInp, eaInp, fatInp, wnInp, ntInp, psInp, hcbInp, lsInp, atInp, twInp;
        private readonly BoolInput dpInp, cetInp;
        private readonly IntInput tsInp;
        public ScavVarsOption() : base("Scavenger Variations")
        {
            elements = [
                hsInp = new FloatInput("Head size"),
                etInp = new FloatInput("Eartler thickness"),
                esInp = new FloatInput("Eye size"),
                enInp = new FloatInput("Eye narrowness"),
                eaInp = new FloatInput("Eye angle") { description = "Eye angle varies from -5 (value=0) to 60 (value=1) degrees, though there are extra calculations" },
                fatInp = new FloatInput("Fatness"),
                wnInp = new FloatInput("Waist narrowness"),
                ntInp = new FloatInput("Neck thickness"),
                psInp = new FloatInput("Pupil size") { description = "Deep pupils always makes this 0.7" },
                dpInp = new BoolInput("Deep pupils?") { description = "Creates an inset look in the eyes. Makes pupil size constant." },
                hcbInp = new FloatInput("Hands color blend") { description = "Controls how much of the body (value=0) and the head (value=1) colors are used in the hands" },
                lsInp = new FloatInput("Leg size"),
                atInp = new FloatInput("Arm thickness"),
                cetInp = new BoolInput("Colored eartler tips?"),
                twInp = new FloatInput("Teeth wideness"),
                tsInp = new IntInput("Tail segments", 1, 4)
            ];
        }

        public override float Execute(XORShift128 Random)
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
            float tailSegs = (Random.Value < 0.5f) ? 0 : Random.Range(1, 5);

            float r = 0f;
            if (hsInp.enabled) r += Mathf.Abs(headSize - hsInp.Value);
            if (etInp.enabled) r += Mathf.Abs(eartlerWidth - etInp.Value);
            if (esInp.enabled) r += Mathf.Abs(eyeSize - esInp.Value);
            if (enInp.enabled) r += Mathf.Abs(narrowEyes - enInp.Value);
            if (eaInp.enabled) r += Mathf.Abs(eyesAngle - eaInp.Value);
            if (fatInp.enabled) r += Mathf.Abs(fatness - fatInp.Value);
            if (wnInp.enabled) r += Mathf.Abs(narrowWaist - wnInp.Value);
            if (ntInp.enabled) r += Mathf.Abs(neckThickness - ntInp.Value);
            if (psInp.enabled) r += Mathf.Abs(pupilSize - psInp.Value);
            if (dpInp.enabled) r += deepPupils ^ dpInp.Value ? 0 : 1;
            if (hcbInp.enabled) r += Mathf.Abs(handsHeadColor - hcbInp.Value);
            if (lsInp.enabled) r += Mathf.Abs(legsSize - lsInp.Value);
            if (atInp.enabled) r += Mathf.Abs(armThickness - atInp.Value);
            if (cetInp.enabled) r += coloredEartlerTips ^ cetInp.Value ? 0 : 1;
            if (twInp.enabled) r += Mathf.Abs(wideTeeth - twInp.Value);
            if (tsInp.enabled) r += Math.Abs(tailSegs - tsInp.Value);
            return r;
        }
    }
}
