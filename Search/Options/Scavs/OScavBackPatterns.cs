using FinderMod.Inputs;
using UnityEngine;
using RWCustom;
using static FinderMod.Search.SearchOptions;
using static FinderMod.Search.ScavUtil;

namespace FinderMod.Search.Options.Scavs
{
    internal class OScavBackPatterns() : IOption(157, false)
    {
        public override BaseInput[] CreateInputs()
        {
            return [
                new PickerInput("Type", ["HardBackSpikes", "WobblyBackTufts"]),
                new PickerInput("Color type", ["None (color strength = 0)", "Decoration", "Head"]),
                new FloatInput("Color strength"),
                new PickerInput("Pattern", ["SpineRidge", "DoubleSpineRidge", "RandomBackBlotch"]),
                new FloatInput("Range top", 0.02f, 0.3f),
                new FloatInput("Range bottom", 0.4f, 1f),
                new IntInput("# of spines", 2, 40) { Description = "SpineRidge range: (2, 37), DoubleSpineRidge range: (2, 40), RandomBackBlotch range: (4, 40)" },
                new FloatInput("General spine size")
            ];
        }

        public override void Run(float[] i, float[] o, SearchData data)
        {
            var p = data.Personality;

            // Pre-generate some attributes we will need later
            float generalMelanin = Custom.PushFromHalf(i[0], 2f);
            float headSize = ClampedRandomVariation(0.5f, 0.5f, 0.1f, i[1], i[2]);
            float eyeSize = Mathf.Pow(Mathf.Max(0f, Mathf.Lerp(i[4], Mathf.Pow(headSize, 0.5f), i[5] * 0.4f)), Mathf.Lerp(0.95f, 0.55f, p.Sympathy));
            float narrowEyes = (i[6] < Mathf.Lerp(0.3f, 0.7f, p.Sympathy)) ? 0f : Mathf.Pow(i[7], Mathf.Lerp(0.5f, 1.5f, p.Sympathy));

            // Calculate how far we have to advance the pointer
            int j = 6; // first 6 rolls (gen. melanin, head size * 2, eartler width, eye size * 2)
            #region skip advancement stuff

            if (i[j++] >= Mathf.Lerp(0.3f, 0.7f, p.Sympathy)) j++; // narrow eyes
            j += 9; // next 9 rolls (eyes angle, fatness * 3, narrow waist * 3, neck thickness * 2)

            if (i[j++] < 0.65f && eyeSize > 0.4f && narrowEyes < 0.3f) // check to add pupils
            {
                // Pupils and stuff
                if (i[j++] < Mathf.Pow(p.Sympathy, 1.5f) * 0.8f) // determine if sympathetic enough to have colored pupils
                {
                    // colored pupils
                    j++;
                    if (i[j++] < 0.6666667f) j++;
                }
                // else deep pupils
            }

            j += 8; // tick 8 more rolls (hands head color * 3, legs size, arm thickness * 2, colored eartler tips, wide teeth)
            int tailSegs = (i[j++] < 0.5f) ? 0 : SearchUtil.GetRangeAt((1, 5), j++, data.States);
            if (i[j++] < 0.25f) j++; // unused scruffy calculation that's still done for some reason

            // Color time!
            HSLColor bodyColor, headColor;
            float bodyColorBlack, headColorBlack;

            float bodyHue = i[j++] * 0.1f;
            if (i[j++] < 0.025f)
            {
                bodyHue = Mathf.Pow(i[j++], 0.4f);
            }
            float accentHue1 = bodyHue + Mathf.Lerp(-1f, 1f, i[j++]) * 0.3f * Mathf.Pow(i[j++], 2f);
            if (accentHue1 > 1f)
            {
                accentHue1 -= 1f;
            }
            else if (accentHue1 < 0f)
            {
                accentHue1 += 1f;
            }

            // Body color
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

            // Magic number electric boogaloo
            float accentHue2;
            if (i[j++] < Custom.LerpMap(bodyColorBlack, 0.5f, 0.8f, 0.9f, 0.3f))
            {
                accentHue2 = accentHue1 + Mathf.Lerp(-1f, 1f, i[j++]) * 0.1f * Mathf.Pow(i[j++], 1.5f);
                accentHue2 = Mathf.Lerp(accentHue2, 0.15f, i[j++]);
            }
            else
            {
                accentHue2 = ((i[j++] < 0.5f) ? Custom.Decimal(bodyHue + 0.5f) : Custom.Decimal(accentHue1 + 0.5f)) + Mathf.Lerp(-1f, 1f, i[j++]) * 0.25f * Mathf.Pow(i[j++], 2f);
                if (i[j++] < Mathf.Lerp(0.8f, 0.2f, p.Energy)) // energy
                {
                    accentHue2 = Mathf.Lerp(accentHue2, 0.15f, i[j++]);
                }
            }
            if (accentHue2 > 1f)
            {
                accentHue2 -= 1f;
            }
            else if (accentHue2 < 0f)
            {
                accentHue2 += 1f;
            }

            // Head color
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
            if (headColor.saturation < bodyColor.saturation * 0.75f) j++;

            if (i[j++] >= 0.65f) j++; // deco color
            j += 3; // also deco color

            if (i[j++] < 0.2f) j++; // eye color

            j += 2; // belly color
            if (i[j++] < 0.033333335f)
            {
                j += 3;
            }

            j += tailSegs; // tail
            #endregion

            // Scav back thing time!
            bool useHardBackSpikes = i[j++] < 0.1f;
            float colorType = 0;
            float colorStrength = 0;
            ScavBodyScalePattern pattern; // 1 = SpineRidge, 2 = DoubleSpineRidge, 3 = RandomBackBlotch
            float top, bottom;
            int numScales;
            float generalSize;

            // BackTuftsAndRidges constructor
            j += 2; // graphic
            if (i[j++] < 0.5f) j++;
            if (i[j++] > generalMelanin)
            {
                colorStrength = Mathf.Pow(i[j++], 0.5f);
            }
            if (colorStrength > 0)
            {
                colorType = i[j++] < 0.5 ? 1 : 2;
            }
            else j++;

            // Inheritance time
            if (useHardBackSpikes)
            {
                // HardBackSpikes (requires 56 random values max)

                // Calculate pattern and generate corresponding attributes
                pattern = i[j++] < 0.6f ? ScavBodyScalePattern.SpineRidge : ScavBodyScalePattern.DoubleSpineRidge;
                if (i[j++] < 0.1f) pattern = ScavBodyScalePattern.RandomBackBlotch;
                ScavUtil.GeneratePattern(i, ref j, ScavUtil.ScavBackType.HardBackSpikes, pattern, out top, out bottom, out numScales);

                // Advance pointer
                if (i[j++] < 0.5f && i[j++] < 0.85f) j++;
                j += 2;

                // General size
                generalSize = Custom.LerpMap(numScales, 5f, 35f, 1f, 0.2f);
                generalSize = Mathf.Lerp(generalSize, p.Dominance, i[j++]); // uses dominance
                generalSize = Mathf.Lerp(generalSize, Mathf.Pow(i[j++], 0.75f), i[j++]);

                // Extra pointer offsetting (for future reference purposes)
                // if (colored > 0 && i[j++] < 0.25f + 0.5f * colored) j++;
            }
            else
            {
                // WobblyBackTufts (requires 59 random values max)

                // Calculate pattern, tick pointer, and generate corresponding attributes
                pattern = ScavBodyScalePattern.RandomBackBlotch;
                if (i[j++] < 0.25f && i[j++] < 0.05f) // scruffy is never 0f because it is hardcoded to 1f
                {
                    pattern = (i[j++] < 0.5f) ? ScavBodyScalePattern.DoubleSpineRidge : ScavBodyScalePattern.SpineRidge;
                }

                if (i[j++] >= 0.2f)
                {
                    if (i[j++] < 0.5f) j += 2;
                    else j++;
                }

                GeneratePattern(i, ref j, ScavBackType.WobblyBackTufts, pattern, out top, out bottom, out numScales);

                // Pointer offsetting
                if (pattern == ScavBodyScalePattern.RandomBackBlotch) j++;
                j++;

                // General size (p.Dominance = dominance)
                generalSize = Mathf.Lerp(i[j++], p.Dominance, i[j++]);
                generalSize = Mathf.Lerp(generalSize, i[j++], i[j++]);
                generalSize = Mathf.Pow(generalSize, Mathf.Lerp(2f, 0.65f, p.Dominance));

                // More pointer offsetting (for future reference purposes)
                // j += 9;
                // j += numScales * (2 + (pattern == ScavBodyScalePattern.RandomBackBlotch ? 1 : 0) + (i[j++] != 1f ? 2 : 0));
                // if (colored > 0 && i[j++] < 0.25f + 0.5f * colored) j++;
            }

            o[0] = useHardBackSpikes ? 0 : 1;
            o[1] = colorType;
            o[2] = colorStrength;
            o[3] = (int)pattern;
            o[4] = top;
            o[5] = bottom;
            o[6] = numScales;
            o[7] = generalSize;
        }
    }
}
