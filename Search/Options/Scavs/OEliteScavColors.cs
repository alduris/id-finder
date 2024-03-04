﻿using FinderMod.Inputs;
using static FinderMod.Search.SearchOptions;
using RWCustom;
using UnityEngine;

namespace FinderMod.Search.Options.Scavs
{
    internal class OEliteScavColors() : IOption(86, true)
    {
        public override BaseInput[] CreateInputs()
        {
            return [
                new ColorHSLInput("Body color"),
                new ColorHSLInput("Head color"),
                new ColorHSLInput("Deco color")
            ];
        }

        public override void Run(float[] i, float[] o, SearchData data)
        {
            var p = data.Personality;

            // Pre-generate some attributes we will need later
            float generalMelanin = Custom.PushFromHalf(i[0], 2f);
            float headSize = ClampedRandomVariation(0.5f, 0.5f, 0.1f, i[1], i[2]);
            float eyeSize = Mathf.Pow(Mathf.Max(0f, Mathf.Lerp(i[4], Mathf.Pow(headSize, 0.5f), i[5] * 0.4f)), Mathf.Lerp(0.95f, 0.55f, p.Sympathy));
            float narrowEyes = 1f;
            float pupilSize = 0f;

            // Calculate how far we have to advance the pointer
            int j = 6; // first 6 rolls (gen. melanin, head size * 2, eartler width, eye size * 2)
            if (i[j++] >= Mathf.Lerp(0.3f, 0.7f, p.Sympathy)) j++; // narrow eyes
            j += 9; // next 9 rolls (eyes angle, fatness * 3, narrow waist * 3, neck thickness * 2)

            if (i[j++] < 0.65f && eyeSize > 0.4f && narrowEyes < 0.3f) // check to add pupils
            {
                if (i[j++] < Mathf.Pow(p.Sympathy, 1.5f) * 0.8f) // determine if sympathetic enough to have colored pupils
                {
                    // Colored pupils
                    pupilSize = Mathf.Lerp(0.4f, 0.8f, Mathf.Pow(i[j++], 0.5f));
                    if (i[j++] < 0.6666667f)
                    {
                        // hasColoredPupils = true;
                        j++;
                    }
                }
                else
                {
                    // Deep pupils
                    pupilSize = 0.7f;
                    // deepPupils = true;
                }
            }

            j += 8; // tick 8 more rolls (colored pupils, hands head color * 3, legs size, arm thickness * 2, wide teeth)
            if (i[j++] >= 0.5f) j++; // tail segments
            if (i[j++] < 0.25f) j++; // unused scruffy calculation that's still done for some reason

            // OK NOW WE GET TO THE COLOR CRAP
            HSLColor bodyColor, headColor, decoColor;
            float bodyColorBlack, headColorBlack;

            float bodyHue = i[j++] * 0.1f;
            if (i[j++] < 0.025f)
            {
                bodyHue = Mathf.Pow(i[j++], 0.4f);
            }
            bodyHue = Mathf.Pow(i[j++], 5f);
            float accentHue1 = bodyHue + Mathf.Lerp(-1f, 1f, i[j++]) * 0.3f * Mathf.Pow(i[j++], 2f);
            if (accentHue1 > 1f)
            {
                accentHue1 -= 1f;
            }
            else if (accentHue1 < 0f)
            {
                accentHue1 += 1f;
            }

            // Body color calculation
            bodyColor = new HSLColor(bodyHue, Mathf.Lerp(0.05f, 1f, Mathf.Pow(i[j++], 0.85f)), Mathf.Lerp(0.05f, 0.8f, i[j++]));
            bodyColor.saturation *= (1f - generalMelanin);
            bodyColor.lightness = Mathf.Lerp(bodyColor.lightness, 0.5f + 0.5f * Mathf.Pow(i[j++], 0.8f), 1f - generalMelanin);
            bodyColorBlack = Custom.LerpMap((bodyColor.rgb.r + bodyColor.rgb.g + bodyColor.rgb.b) / 3f, 0.04f, 0.8f, 0.3f, 0.95f, 0.5f);
            bodyColorBlack = Mathf.Lerp(bodyColorBlack, Mathf.Lerp(0.5f, 1f, i[j++]), i[j++] * i[j++] * i[j++]);
            bodyColorBlack *= generalMelanin;
            Vector2 vector = new(bodyColor.saturation, Mathf.Lerp(-1f, 1f, bodyColor.lightness * (1f - bodyColorBlack)));
            if (vector.magnitude < 0.5f)
            {
                vector = Vector2.Lerp(vector, vector.normalized, Mathf.InverseLerp(0.5f, 0.3f, vector.magnitude));
                bodyColor = new HSLColor(bodyColor.hue, Mathf.InverseLerp(-1f, 1f, vector.x), Mathf.InverseLerp(-1f, 1f, vector.y));
                bodyColorBlack = Custom.LerpMap((bodyColor.rgb.r + bodyColor.rgb.g + bodyColor.rgb.b) / 3f, 0.04f, 0.8f, 0.3f, 0.95f, 0.5f);
                bodyColorBlack = Mathf.Lerp(bodyColorBlack, Mathf.Lerp(0.5f, 1f, i[j++]), i[j++] * i[j++] * i[j++]);
                bodyColorBlack *= generalMelanin;
            }

            // More magic number calculation
            float accentHue2;
            if (i[j++] < Custom.LerpMap(bodyColorBlack, 0.5f, 0.8f, 0.9f, 0.3f))
            {
                accentHue2 = accentHue1 + Mathf.Lerp(-1f, 1f, i[j++]) * 0.1f * Mathf.Pow(i[j++], 1.5f);
                accentHue2 = Mathf.Lerp(accentHue2, 0.15f, i[j++]);
                if (accentHue2 > 1f)
                {
                    accentHue2 -= 1f;
                }
                else if (accentHue2 < 0f)
                {
                    accentHue2 += 1f;
                }
            }
            else
            {
                accentHue2 = ((i[j++] < 0.5f) ? Custom.Decimal(bodyHue + 0.5f) : Custom.Decimal(accentHue1 + 0.5f)) + Mathf.Lerp(-1f, 1f, i[j++]) * 0.25f * Mathf.Pow(i[j++], 2f);
                if (i[j++] < Mathf.Lerp(0.8f, 0.2f, p.Energy)) // energy
                {
                    accentHue2 = Mathf.Lerp(accentHue2, 0.15f, i[j++]);
                }
                if (accentHue2 > 1f)
                {
                    accentHue2 -= 1f;
                }
                else if (accentHue2 < 0f)
                {
                    accentHue2 += 1f;
                }
            }

            // Head color time
            headColor = new HSLColor((i[j++] < 0.75f) ? accentHue1 : accentHue2, 1f, 0.05f + 0.15f * i[j++]);
            headColor.saturation *= Mathf.Pow(1f - generalMelanin, 2f);
            headColor.lightness = Mathf.Lerp(headColor.lightness, 0.5f + 0.5f * Mathf.Pow(i[j++], 0.8f), 1f - generalMelanin);
            headColor.saturation *= (0.1f + 0.9f * Mathf.InverseLerp(0.1f, 0f, Custom.DistanceBetweenZeroToOneFloats(bodyColor.hue, headColor.hue) * Custom.LerpMap(Mathf.Abs(0.5f - headColor.lightness), 0f, 0.5f, 1f, 0.3f)));
            if (headColor.lightness < 0.5f)
            {
                headColor.lightness *= (0.5f + 0.5f * Mathf.InverseLerp(0.2f, 0.05f, Custom.DistanceBetweenZeroToOneFloats(bodyColor.hue, headColor.hue)));
            }
            headColorBlack = Custom.LerpMap((headColor.rgb.r + headColor.rgb.g + headColor.rgb.b) / 3f, 0.035f, 0.26f, 0.7f, 0.95f, 0.25f);
            headColorBlack = Mathf.Lerp(headColorBlack, Mathf.Lerp(0.8f, 1f, i[j++]), i[j++] * i[j++] * i[j++]);
            headColorBlack *= 0.2f + 0.7f * generalMelanin;
            headColorBlack = Mathf.Max(headColorBlack, bodyColorBlack);
            headColor.saturation = Custom.LerpMap(headColor.lightness * (1f - headColorBlack), 0f, 0.15f, 1f, headColor.saturation);
            if (headColor.lightness > bodyColor.lightness)
            {
                headColor = bodyColor;
            }
            if (headColor.saturation < bodyColor.saturation * 0.75f)
            {
                if (i[j++] < 0.5f)
                {
                    headColor.hue = bodyColor.hue;
                }
                else
                {
                    headColor.lightness *= 0.25f;
                }
                headColor.saturation = bodyColor.saturation * 0.75f;
            }

            // Decoration colors (eartler tips)
            decoColor = new HSLColor((i[j++] < 0.65f) ? bodyHue : ((i[j++] < 0.5f) ? accentHue1 : accentHue2), i[j++], 0.5f + 0.5f * Mathf.Pow(i[j++], 0.5f));
            decoColor.lightness *= Mathf.Lerp(generalMelanin, i[j++], 0.5f);

            // Return values
            o[0] = bodyColor.hue;
            o[1] = bodyColor.saturation;
            o[2] = bodyColor.lightness;

            o[3] = headColor.hue;
            o[4] = headColor.saturation;
            o[5] = headColor.lightness;

            o[6] = decoColor.hue;
            o[7] = decoColor.saturation;
            o[8] = decoColor.lightness;
        }
    }
}
