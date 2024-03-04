using System;
using FinderMod.Inputs;
using RWCustom;
using Unity.Mathematics;
using UnityEngine;
using static FinderMod.Search.SearchOptions;
using static FinderMod.Search.SearchUtil;

namespace FinderMod.Search.Options.Scavs
{
    internal class OScavVars() : IOption(34, false)
    {
        public override BaseInput[] CreateInputs()
        {
            return [
                new FloatInput("Head size"),
                new FloatInput("Eartler thickness"),
                new FloatInput("Eye size"),
                new FloatInput("Eye narrowness"),
                new FloatInput("Eye angle") { Description = "Eye angle varies from -5 (value=0) to 60 (value=1) degrees, though there are extra calculations" },
                new FloatInput("Fatness"),
                new FloatInput("Waist narrowness"),
                new FloatInput("Neck thickness"),
                new FloatInput("Pupil size") { Description = "Deep pupils makes this 0.7" },
                new BoolInput("Deep pupils?") { Description = "Creates an inset look in the eyes. Makes pupil size constant." },
                new FloatInput("Hands color blend") { Description = "Controls how much of the body (value=0) and the head (value=1) colors are used in the hands" },
                new FloatInput("Leg size"),
                new FloatInput("Arm thickness"), 
                new BoolInput("Colored eartler tips?"),
                new FloatInput("Teeth wideness"),
                new IntInput("Tail segments", 1, 4)
            ];
        }

        public override void Run(float[] i, float[] o, SearchData data)
        {
            Personality p = data.Personality;
            int j = 7;
            float generalMelanin = Custom.PushFromHalf(i[0], 2f);
            float headSize = ClampedRandomVariation(0.5f, 0.5f, 0.1f, i[1], i[2]);
            float eartlerWidth = i[3];
            float eyeSize = Mathf.Pow(Mathf.Max(0f, Mathf.Lerp(i[4], Mathf.Pow(headSize, 0.5f), i[5] * 0.4f)), Mathf.Lerp(0.95f, 0.55f, p.Sympathy));
            float narrowEyes = ((i[6] < Mathf.Lerp(0.3f, 0.7f, p.Sympathy)) ? 0f : Mathf.Pow(i[j++], Mathf.Lerp(0.5f, 1.5f, p.Sympathy)));
            float eyesAngle = Mathf.Pow(i[j++], Mathf.Lerp(2.5f, 0.5f, Mathf.Pow(p.Energy, 0.03f)));
            float fatness = Mathf.Lerp(i[j++], p.Dominance, i[j++] * 0.2f);
            if (p.Energy < 0.5f)
            {
                fatness = Mathf.Lerp(fatness, 1f, i[j++] * Mathf.InverseLerp(0.5f, 0f, p.Energy));
            }
            else
            {
                fatness = Mathf.Lerp(fatness, 0f, i[j++] * Mathf.InverseLerp(0.5f, 1f, p.Energy));
            }
            float narrowWaist = Mathf.Lerp(Mathf.Lerp(i[j++], 1f - fatness, i[j++]), 1f - p.Energy, i[j++]);
            float neckThickness = Mathf.Lerp(Mathf.Pow(i[j++], 1.5f - p.Aggression), 1f - fatness, i[j++] * 0.5f);
            float pupilSize = 0f;
            bool deepPupils = false;
            //int coloredPupils = 0;
            if (i[j++] < 0.65f && eyeSize > 0.4f && narrowEyes < 0.3f)
            {
                if (i[j++] < Mathf.Pow(p.Sympathy, 1.5f) * 0.8f)
                {
                    pupilSize = Mathf.Lerp(0.4f, 0.8f, Mathf.Pow(i[j++], 0.5f));
                    if (i[j++] < 0.6666667f)
                    {
                        j++;
                        //coloredPupils = GetRangeAt((1, 4), j++, data.States);
                    }
                }
                else
                {
                    pupilSize = 0.7f;
                    deepPupils = true;
                }
            }
            float handsHeadColor;
            if (i[j++] < generalMelanin)
            {
                handsHeadColor = ((i[j++] < 0.3f) ? i[j++] : ((i[j++] < 0.6f) ? 1f : 0f));
            }
            else
            {
                handsHeadColor = ((i[j++] < 0.2f) ? i[j++] : ((i[j++] < 0.8f) ? 1f : 0f));
            }
            float legsSize = i[j++];
            float armThickness = Mathf.Lerp(i[j++], Mathf.Lerp(p.Dominance, fatness, 0.5f), i[j++]);
            bool coloredEartlerTips = i[j++] < 1f / Mathf.Lerp(1.2f, 10f, generalMelanin);
            float wideTeeth = i[j++];
            float tailSegs = ((i[j++] < 0.5f) ? 0 : GetRangeAt((1, 5), j++, data.States));

            o[0] = headSize;
            o[1] = eartlerWidth;
            o[2] = eyeSize;
            o[3] = narrowEyes;
            o[4] = eyesAngle;
            o[5] = fatness;
            o[6] = narrowWaist;
            o[7] = neckThickness;
            o[8] = pupilSize;
            o[9] = (deepPupils ? 1 : 0);
            o[10] = handsHeadColor;
            o[11] = legsSize;
            o[12] = armThickness;
            o[13] = (coloredEartlerTips ? 1 : 0);
            o[14] = wideTeeth;
            o[15] = tailSegs;
        }
    }
}
