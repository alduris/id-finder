using System;
using System.Collections.Generic;
using System.Linq;
using RWCustom;
using UnityEngine;
using LizardType = FinderMod.Search.LizardUtil.LizardType;
using LizardBodyScaleType = FinderMod.Search.LizardUtil.LizardBodyScaleType;
using ScavBodyScalePattern = FinderMod.Search.ScavUtil.ScavBodyScalePattern;

namespace FinderMod.Search
{
    public struct Setup {
        public SearchInput[] Inputs;
        public bool MSC;
        public int MinFloats;
        public float[,] FloatRanges;
        public int[,] IntRanges;
        // Parameters: i(nput), p(ersonality), s(eed), f(loat) r(ange start), i(nteger) r(ange start); output: floats mirroring Inputs
        // Parameter i is a bunch of concatenated arrays
        public Func<float[], float[], int, int, int, float[]> Apply;
    }

    public enum InputType
    {
        Label,
        Whitespace,
        Float,
        Integer,
        Boolean,
        Hue,
        //ColorHSL,
        ColorRGB,
        MultiChoice
    }

    public struct SearchInput
    {
        public string Name;
        public string Description;
        public InputType Type = InputType.Float;
        public (float, float) Range = (0f, 1f);
        public bool Wrap = false;

        public SearchInput (string name) { Name = name; }
        public SearchInput (string name, string desc) { Name = name; Description = desc; }
        public SearchInput (string name, InputType type)
        {
            Name = name;
            Type = type;
        }
        public SearchInput (string name, string desc, InputType type)
        {
            Name = name;
            Description = desc;
            Type = type;
        }
        public SearchInput(string name, (float, float) range) : this(name, InputType.Float, range) { }
        public SearchInput(string name, string desc, (float, float) range) : this(name, desc, InputType.Float, range) { }
        public SearchInput (string name, string desc, InputType type, (float, float) range)
        {
            Name = name;
            Description = desc;
            Type = type;
            Range = range;
        }
        public SearchInput (string name, InputType type, (float, float) range)
        {
            Name = name;
            Type = type;
            Range = range;
        }
    }

    public static class SearchOptions
    {
        public static SearchInput Whitespace = new(null, InputType.Whitespace);

        public static int GetNumOutputs(SearchInput[] inputs)
        {
            return inputs.Aggregate(0, (curr, val) => curr + val.Type switch
            {
                InputType.Whitespace or InputType.Label => 0,
                InputType.ColorRGB => 3,
                _ => 1
            });
        }

        public static float ClampedRandomVariation(float baseValue, float maxDeviation, float k, float i1, float i2)
        {
            return Mathf.Clamp(baseValue + Custom.SCurve(i1 * 0.5f, k) * 2f * ((i2 < 0.5f) ? 1f : -1f) * maxDeviation, 0f, 1f);
        }
        public static float WrappedRandomVariation(float baseValue, float maxDeviation, float k, float i1, float i2)
        {
            float num = baseValue + Custom.SCurve(i1 * 0.5f, k) * 2f * ((i2 < 0.5f) ? 1f : -1f) * maxDeviation + 1f;
            return num - Mathf.Floor(num);
        }


        internal static readonly Dictionary<string, Setup> Groups = new()
        {
            // Personality
            {
                "Personality",
                new Setup {
                    Inputs = new SearchInput[] {
                        new("Aggression"), new("Bravery"), new("Dominance"), new("Energy"), new("Nervous"), new("Sympathy")
                        // "Aggression", "Bravery", "Dominance", "Energy", "Nervous", "Sympathy"
                    },
                    MSC = false,
                    MinFloats = 9,
                    FloatRanges = null, IntRanges = null,
                    Apply = (_, p, _, _, _) => {
                        return p;
                    }
                }
            },
            
            // Scavs
            {
                "Scavenger Skills",
                new Setup {
                    Inputs = new SearchInput[] {
                        new("Dodge"), new("Mid-range"), new("Melee"), new("Blocking"), new("Reaction")
                        // "Dodge", "Mid-range", "Melee", "Blocking", "Reaction"
                    },
                    MSC = false,
                    MinFloats = 17,
                    FloatRanges = null, IntRanges = null,
                    Apply = (i, p, s, _, _) => {
                        float dge, mid, mle, blk, rea;

                        dge = Custom.PushFromHalf(Mathf.Lerp((i[0] < 0.5f) ? p[4] : p[0], i[1], i[2]), 1f + i[3]);
                        mid = Custom.PushFromHalf(Mathf.Lerp((i[4] < 0.5f) ? p[3] : p[0], i[5], i[6]), 1f + i[7]);
                        mle = Custom.PushFromHalf(i[8], 1f + i[9]);
                        blk = Custom.PushFromHalf(Mathf.InverseLerp(0.35f, 1f, Mathf.Lerp((i[10] < 0.5f) ? p[1] : p[3], i[11], i[12])), 1f + i[13]);
                        rea = Custom.PushFromHalf(Mathf.Lerp(p[3], i[14], i[15]), 1f + i[16]);

                        if(ModManager.MSC)
                        {
                            float n = 1f - p[2];
                            dge = Mathf.Lerp(dge, 0f, n * 0.085f); // Dodge
                            mid = Mathf.Lerp(mid, 0f, n * 0.085f); // Mid-range
                            blk = Mathf.Lerp(blk, 0f, n * 0.05f);  // Blocking
                            rea = Mathf.Lerp(rea, 0f, n * 0.15f);  // Reaction
                        }
                        return new float[] { dge, mid, mle, blk, rea };
                    }
                }
            },
            {
                "Elite Scavenger Skills",
                new Setup {
                    Inputs = new SearchInput[] {
                        new("Dodge"), new("Mid-range"), new("Melee"), new("Blocking"), new("Reaction")
                        // "Dodge", "Mid-range", "Melee", "Blocking", "Reaction"
                    },
                    MSC = true,
                    MinFloats = 17,
                    FloatRanges = null, IntRanges = null,
                    Apply = (i, p, s, _, _) => {
                        float dge, mid, mle, blk, rea;

                        dge = Custom.PushFromHalf(Mathf.Lerp((i[0] < 0.5f) ? p[4] : p[0], i[1], i[2]), 1f + i[3]);
                        mid = Custom.PushFromHalf(Mathf.Lerp((i[4] < 0.5f) ? p[3] : p[0], i[5], i[6]), 1f + i[7]);
                        mle = Custom.PushFromHalf(i[8], 1f + i[9]);
                        blk = Custom.PushFromHalf(Mathf.InverseLerp(0.35f, 1f, Mathf.Lerp((i[10] < 0.5f) ? p[1] : p[3], i[11], i[12])), 1f + i[13]);
                        rea = Custom.PushFromHalf(Mathf.Lerp(p[3], i[14], i[15]), 1f + i[16]);

                        float n = Mathf.Lerp(p[2], 1f, 0.15f);
                        dge = Mathf.Lerp(dge, 1f, n * 0.15f); // Dodge
                        mid = Mathf.Lerp(mid, 1f, n * 0.1f); // Mid-range
                        blk = Mathf.Lerp(blk, 1f, n * 0.1f);  // Blocking
                        rea = Mathf.Lerp(rea, 1f, n * 0.05f);  // Reaction

                        return new float[] { dge, mid, mle, blk, rea };
                    }
                }
            },
            {
                "Scavenger Variations",
                new Setup
                {
                    Inputs = new SearchInput[] {
                        new("Head size"), new("Eartler thickness"), new("Eye size"), new("Eye narrowness"),
                        new("Eye angle", "Eye angle varies from -5 (value=0) to 60 (value=1) degrees, though there are extra calculations"),
                        new("Fatness"), new("Waist narrowness"), new("Neck thickness"), new("Pupil size", "Deep pupils makes this 0.7"),
                        new("Deep pupils?", "Creates an inset look in the eyes. Makes pupil size constant.", InputType.Boolean),
                        new("Hands color blend", "Controls how much of the body (value=0) and the head (value=1) colors are used in the hands"),
                        new("Leg size"), new("Arm thickness"), new("Colored eartler tips?", InputType.Boolean), new("Teeth wideness"),
                        new("Tail segments", InputType.Integer, (1f, 4f))
                        // "Head size", "Eartler width", "Eye size", "Eye narrowness", "Eye angle", "Fatness", "Waist narrowness", "Neck thickness",
                        // "Pupil size", "Hands head color", "Leg size", "Arm thickness", "Colored eartler tips?", "Teeth wideness", "Tail segments"
                    },
                    MSC = false,
                    MinFloats = 34, // 26 to 34 needed (varies)
                    FloatRanges = null,
                    IntRanges = new int[,] {
                        // block ----------------------------------------------------------------------------------------
                        { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 },
                        // block -------------------------------------------------------------------      coloredPupils
                        { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 1, 4 }, { 1, 4 },
                        // -------  block ------------------------------------------------------------------------------
                        { 1, 4 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 },
                        // tailSegs ---------------------------------- (I have no clue if I need this much but I'm playing it safe)
                        { 1, 5 }, { 1, 5 }, { 1, 5 }, { 1, 5 }, { 1, 5 }
                    },
                    Apply = (i, p, s, fr, ir) =>
                    {
                        int j = 7;
                        float generalMelanin = Custom.PushFromHalf(i[0], 2f);
                        float headSize = ClampedRandomVariation(0.5f, 0.5f, 0.1f, i[1], i[2]);
                        float eartlerWidth = i[3];
                        float eyeSize = Mathf.Pow(Mathf.Max(0f, Mathf.Lerp(i[4], Mathf.Pow(headSize, 0.5f), i[5] * 0.4f)), Mathf.Lerp(0.95f, 0.55f, p[5]));
                        float narrowEyes = ((i[6] < Mathf.Lerp(0.3f, 0.7f, p[5])) ? 0f : Mathf.Pow(i[j++], Mathf.Lerp(0.5f, 1.5f, p[5])));
                        float eyesAngle = Mathf.Pow(i[j++], Mathf.Lerp(2.5f, 0.5f, Mathf.Pow(p[3], 0.03f)));
                        float fatness = Mathf.Lerp(i[j++], p[2], i[j++] * 0.2f);
                        if (p[3] < 0.5f)
                        {
                            fatness = Mathf.Lerp(fatness, 1f, i[j++] * Mathf.InverseLerp(0.5f, 0f, p[3]));
                        }
                        else
                        {
                            fatness = Mathf.Lerp(fatness, 0f, i[j++] * Mathf.InverseLerp(0.5f, 1f, p[3]));
                        }
                        float narrowWaist = Mathf.Lerp(Mathf.Lerp(i[j++], 1f - fatness, i[j++]), 1f - p[3], i[j++]);
                        float neckThickness = Mathf.Lerp(Mathf.Pow(i[j++], 1.5f - p[0]), 1f - fatness, i[j++] * 0.5f);
                        float pupilSize = 0f;
                        bool deepPupils = false;
                        int coloredPupils = 0;
                        if (i[j++] < 0.65f && eyeSize > 0.4f && narrowEyes < 0.3f)
                        {
                            if (i[j++] < Mathf.Pow(p[5], 1.5f) * 0.8f)
                            {
                                pupilSize = Mathf.Lerp(0.4f, 0.8f, Mathf.Pow(i[j++], 0.5f));
                                if (i[j++] < 0.6666667f)
                                {
                                    coloredPupils = (int)i[ir + j++];
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
                        float armThickness = Mathf.Lerp(i[j++], Mathf.Lerp(p[2], fatness, 0.5f), i[j++]);
                        bool coloredEartlerTips = i[j++] < 1f / Mathf.Lerp(1.2f, 10f, generalMelanin);
                        float wideTeeth = i[j++];
                        float tailSegs = ((i[j++] < 0.5f) ? 0 : i[ir + j++]);
                        return new float[] {
                            headSize, eartlerWidth, eyeSize, narrowEyes, eyesAngle, fatness, narrowWaist, neckThickness, pupilSize,
                            (deepPupils ? 1 : 0), handsHeadColor, legsSize, armThickness, (coloredEartlerTips ? 1 : 0), wideTeeth, tailSegs
                        };
                    }
                }
            },
            {
                "Scavenger Colors",
                new Setup
                {
                    Inputs = new SearchInput[]
                    {
                        new("Body color (H)", InputType.Hue),
                        new("Body color (S)", InputType.Float),
                        new("Body color (L)", InputType.Float),
                        Whitespace,
                        // new("Body color", InputType.ColorHSL),
                        new("Head color (H)", InputType.Hue),
                        new("Head color (S)", InputType.Float),
                        new("Head color (L)", InputType.Float),
                        Whitespace,
                        // new("Head color", InputType.ColorHSL),
                        new("Deco color (H)", "Controls eartler tip colors if enabled. Can sometimes control eye or belly color.", InputType.Hue),
                        new("Deco color (S)", "Controls eartler tip colors if enabled. Can sometimes control eye or belly color.", InputType.Float),
                        new("Deco color (L)", "Controls eartler tip colors if enabled. Can sometimes control eye or belly color.", InputType.Float),
                        Whitespace,
                        // new("Decoration color", InputType.ColorHSL), // used for colored eartler tips
                        new("Eye color (H)", InputType.Hue),
                        new("Eye color (L)", InputType.Float),
                        // new("Eye color", InputType.ColorHSL),
                        // new("Belly color", InputType.ColorHSL)
                        // There is a belly color that is just a lerp between body and decoration color. Add it back if wanted
                    },
                    MSC = false,
                    MinFloats = 86,
                    FloatRanges = null, IntRanges = null,
                    Apply = (i, p, s, _, _) => {
                        // Pre-generate some attributes we will need later
                        float generalMelanin = Custom.PushFromHalf(i[0], 2f);
                        float headSize = ClampedRandomVariation(0.5f, 0.5f, 0.1f, i[1], i[2]);
                        float eyeSize = Mathf.Pow(Mathf.Max(0f, Mathf.Lerp(i[4], Mathf.Pow(headSize, 0.5f), i[5] * 0.4f)), Mathf.Lerp(0.95f, 0.55f, p[5]));
                        float narrowEyes = (i[6] < Mathf.Lerp(0.3f, 0.7f, p[5])) ? 0f : Mathf.Pow(i[7], Mathf.Lerp(0.5f, 1.5f, p[5]));
                        float pupilSize = 0f;
                        bool deepPupils = false;
                        bool hasColoredPupils = false;

                        // Calculate how far we have to advance the pointer
                        int j = 6; // first 6 rolls (gen. melanin, head size * 2, eartler width, eye size * 2)
                        if(i[j++] >= Mathf.Lerp(0.3f, 0.7f, p[5])) j++; // narrow eyes
                        j += 9; // next 9 rolls (eyes angle, fatness * 3, narrow waist * 3, neck thickness * 2)

                        if (i[j++] < 0.65f && eyeSize > 0.4f && narrowEyes < 0.3f) // check to add pupils
                        {
                            if (i[j++] < Mathf.Pow(p[5], 1.5f) * 0.8f) // determine if sympathetic enough to have colored pupils
                            {
                                // Colored pupils
                                pupilSize = Mathf.Lerp(0.4f, 0.8f, Mathf.Pow(i[j++], 0.5f));
                                if(i[j++] < 0.6666667f)
                                {
                                    hasColoredPupils = true;
                                    j++;
                                }
                            }
                            else
                            {
                                // Deep pupils
                                pupilSize = 0.7f;
                                deepPupils = true;
                            }
                        }

                        j += 8; // tick 8 more rolls (hands head color * 3, legs size, arm thickness * 2, colored eartler tips, wide teeth)
                        // bool coloredEartlerTips = i[j++] < 1f / Mathf.Lerp(1.2f, 10f, generalMelanin);
                        // j++; // wide teeth
                        if (i[j++] >= 0.5f) j++; // tail segments
                        if (i[j++] < 0.25f) j++; // unused scruffy calculation that's still done for some reason

                        // OK NOW WE GET TO THE COLOR CRAP
                        HSLColor bodyColor, headColor, decoColor, eyeColor, bellyColor;
                        float bodyColorBlack, headColorBlack, bellyColorBlack;

                        float bodyHue = i[j++] * 0.1f;
                        if (i[j++] < 0.025f)
                        {
                            bodyHue = Mathf.Pow(i[j++], 0.4f);
                        }
                        //if (elite)
                        //{
                        //    bodyHue = Mathf.Pow(i[j++], 5f);
                        //}
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
                            if (i[j++] < Mathf.Lerp(0.8f, 0.2f, p[3])) // energy
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

                        // Eye colors
                        eyeColor = new HSLColor(accentHue2, 1f, (i[j++] < 0.2f) ? (0.5f + i[j++] * 0.5f) : 0.5f);
                        // this.eyeColor = new HSLColor(this.scavenger.Elite ? 0f : num3, 1f, (i[j++] < 0.2f) ? (0.5f + i[j++] * 0.5f) : 0.5f);
                        if (hasColoredPupils)
                        {
                            eyeColor.lightness = Mathf.Lerp(eyeColor.lightness, 1f, 0.3f);
                        }
                        if (headColor.lightness * (1f - headColorBlack) > eyeColor.lightness / 2f && (pupilSize == 0f || deepPupils))
                        {
                            eyeColor.lightness *= 0.2f;
                        }

                        // Belly color
                        float value = i[j++];
                        float value2 = i[j++];
                        bellyColor = new HSLColor(
                            Mathf.Lerp(bodyColor.hue, decoColor.hue, value * 0.7f),
                            bodyColor.saturation * Mathf.Lerp(1f, 0.5f, value),
                            bodyColor.lightness + 0.05f + 0.3f * value2
                        );
                        bellyColorBlack = Mathf.Lerp(bodyColorBlack, 1f, 0.3f * Mathf.Pow(value2, 1.4f));
                        if (i[j++] < 0.033333335f)
                        {
                            headColor.lightness = Mathf.Lerp(0.2f, 0.35f, i[j++]);
                            headColorBlack *= Mathf.Lerp(1f, 0.8f, i[j++]);
                            bellyColor.hue = Mathf.Lerp(bellyColor.hue, headColor.hue, Mathf.Pow(i[j++], 0.5f));
                        }

                        // Return values
                        return new float[] {
                            bodyColor.hue,  bodyColor.saturation,  bodyColor.lightness,
                            headColor.hue,  headColor.saturation,  headColor.lightness,
                            decoColor.hue,  decoColor.saturation,  decoColor.lightness,
                            eyeColor.hue,   /* saturation = 1 */   eyeColor.lightness,
                            // bellyColor.hue, bellyColor.saturation, bellyColor.lightness
                        };
                    }
                }
            },
            {
                "Elite Scavenger Colors",
                new Setup
                {
                    Inputs = new SearchInput[]
                    {
                        new("Body color (H)", InputType.Hue),
                        new("Body color (S)", InputType.Float),
                        new("Body color (L)", InputType.Float),
                        Whitespace,
                        // new("Body color", InputType.ColorHSL),
                        new("Head color (H)", InputType.Hue),
                        new("Head color (S)", InputType.Float),
                        new("Head color (L)", InputType.Float),
                        Whitespace,
                        // new("Head color", InputType.ColorHSL),
                        new("Deco color (H)", "Controls eartler tip colors if enabled. Can sometimes control eye or belly color.", InputType.Hue),
                        new("Deco color (S)", "Controls eartler tip colors if enabled. Can sometimes control eye or belly color.", InputType.Float),
                        new("Deco color (L)", "Controls eartler tip colors if enabled. Can sometimes control eye or belly color.", InputType.Float)
                        // new("Decoration color", InputType.ColorHSL)
                    },
                    MSC = true,
                    MinFloats = 86,
                    FloatRanges = null, IntRanges = null,
                    Apply = (i, p, s, _, _) => {
                        // Pre-generate some attributes we will need later
                        float generalMelanin = Custom.PushFromHalf(i[0], 2f);
                        float headSize = ClampedRandomVariation(0.5f, 0.5f, 0.1f, i[1], i[2]);
                        float eyeSize = Mathf.Pow(Mathf.Max(0f, Mathf.Lerp(i[4], Mathf.Pow(headSize, 0.5f), i[5] * 0.4f)), Mathf.Lerp(0.95f, 0.55f, p[5]));
                        float narrowEyes = (i[6] < Mathf.Lerp(0.3f, 0.7f, p[5])) ? 0f : Mathf.Pow(i[7], Mathf.Lerp(0.5f, 1.5f, p[5]));
                        float pupilSize = 0f;

                        // Calculate how far we have to advance the pointer
                        int j = 6; // first 6 rolls (gen. melanin, head size * 2, eartler width, eye size * 2)
                        if(i[j++] >= Mathf.Lerp(0.3f, 0.7f, p[5])) j++; // narrow eyes
                        j += 9; // next 9 rolls (eyes angle, fatness * 3, narrow waist * 3, neck thickness * 2)

                        if (i[j++] < 0.65f && eyeSize > 0.4f && narrowEyes < 0.3f) // check to add pupils
                        {
                            if (i[j++] < Mathf.Pow(p[5], 1.5f) * 0.8f) // determine if sympathetic enough to have colored pupils
                            {
                                // Colored pupils
                                pupilSize = Mathf.Lerp(0.4f, 0.8f, Mathf.Pow(i[j++], 0.5f));
                                if(i[j++] < 0.6666667f)
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

                        j += 7; // tick 7 more rolls (hands head color * 3, legs size, arm thickness * 2, wide teeth)
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
                            if (i[j++] < Mathf.Lerp(0.8f, 0.2f, p[3])) // energy
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
                        return new float[] {
                            bodyColor.hue,  bodyColor.saturation,  bodyColor.lightness,
                            headColor.hue,  headColor.saturation,  headColor.lightness,
                            decoColor.hue,  decoColor.saturation,  decoColor.lightness,
                        };
                    }
                }
            },
            {
                "Scavenger Back Patterns",
                new Setup
                {
                    Inputs = new SearchInput[]
                    {
                        new("See hover descriptions at bottom for most inputs", InputType.Label),
                        new("Type", "1: HardBackSpikes, 2: WobblyBackTufts", InputType.MultiChoice, (1, 2)),
                        new("Color type", "1: none (color strength = 0), 2: decoration, 3: head", InputType.MultiChoice, (1, 3)),
                        new("Color strength", InputType.Float),
                        new("Pattern", "1: SpineRidge, 2: DoubleSpineRidge, 3: RandomBackBlotch", InputType.MultiChoice, (1, 3)),
                        new("Range top", InputType.Float, (0.02f, 0.3f)),
                        new("Range bottom", InputType.Float, (0.4f, 1f)),
                        new("Number of spines", "SpineRidge range: (2, 37), DoubleSpineRidge range: (2, 40), RandomBackBlotch range: (4, 40)", InputType.Integer, (2, 40)),
                        new("General spine size", InputType.Float),
                    },
                    MSC = false,
                    MinFloats = 157, // 86 (colors + before) + 4 (tail) + 59 (WobblyBackTufts) + 8
                    FloatRanges = null, IntRanges = null,
                    Apply = (i, p, s, _, _) =>
                    {
                        // Pre-generate some attributes we will need later
                        float generalMelanin = Custom.PushFromHalf(i[0], 2f);
                        float headSize = ClampedRandomVariation(0.5f, 0.5f, 0.1f, i[1], i[2]);
                        float eyeSize = Mathf.Pow(Mathf.Max(0f, Mathf.Lerp(i[4], Mathf.Pow(headSize, 0.5f), i[5] * 0.4f)), Mathf.Lerp(0.95f, 0.55f, p[5]));
                        float narrowEyes = (i[6] < Mathf.Lerp(0.3f, 0.7f, p[5])) ? 0f : Mathf.Pow(i[7], Mathf.Lerp(0.5f, 1.5f, p[5]));

                        // Calculate how far we have to advance the pointer
                        int j = 6; // first 6 rolls (gen. melanin, head size * 2, eartler width, eye size * 2)
                        #region skip advancement stuff

                        if(i[j++] >= Mathf.Lerp(0.3f, 0.7f, p[5])) j++; // narrow eyes
                        j += 9; // next 9 rolls (eyes angle, fatness * 3, narrow waist * 3, neck thickness * 2)

                        if (i[j++] < 0.65f && eyeSize > 0.4f && narrowEyes < 0.3f) // check to add pupils
                        {
	                        // Pupils and stuff
	                        if (i[j++] < Mathf.Pow(p[5], 1.5f) * 0.8f) // determine if sympathetic enough to have colored pupils
	                        {
		                        // colored pupils
		                        j++;
                                if(i[j++] < 0.6666667f) j++;
                            }
	                        // else deep pupils
                        }

                        j += 8; // tick 8 more rolls (hands head color * 3, legs size, arm thickness * 2, colored eartler tips, wide teeth)
                        int tailSegs = ((i[j++] < 0.5f) ? 0 : SearchUtil.GetRangeAt(s, new int[] {1, 5}, j++));
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
                            if (i[j++] < Mathf.Lerp(0.8f, 0.2f, p[3])) // energy
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

                        if(i[j++] >= 0.65f) j++; // deco color
                        j += 3; // also deco color

                        if(i[j++] < 0.2f) j++; // eye color

                        j += 2; // belly color
                        if (i[j++] < 0.033333335f)
                        {
                            j += 3;
                        }

                        j += tailSegs; // tail
                        #endregion

                        // Scav back thing time!
                        bool useHardBackSpikes = i[j++] < 0.1f;
                        float colorType = 1;
                        float colored = 0;
                        ScavBodyScalePattern pattern; // 1 = SpineRidge, 2 = DoubleSpineRidge, 3 = RandomBackBlotch
                        float top, bottom;
                        int numScales;
                        float generalSize;

                        // BackTuftsAndRidges constructor
                        j += 2; // graphic
                        if (i[j++] < 0.5f) j++;
                        if (i[j++] > generalMelanin)
                        {
                            colored = Mathf.Pow(i[j++], 0.5f);
                        }
                        if (colored > 0)
                        {
                            colorType = i[j++] < 0.5 ? 2 : 3;
                        }
                        else j++;

                        // Inheritance time
                        if (useHardBackSpikes)
                        {
                            // HardBackSpikes (requires 56 random values max)

                            // Calculate pattern and generate corresponding attributes
                            pattern = i[j++] < 0.6f ? ScavBodyScalePattern.SpineRidge : ScavBodyScalePattern.DoubleSpineRidge;
                            if(i[j++] < 0.1f) pattern = ScavBodyScalePattern.RandomBackBlotch;
                            ScavUtil.GeneratePattern(i, ref j, ScavUtil.ScavBackType.HardBackSpikes, pattern, out top, out bottom, out numScales);

                            // Advance pointer
                            if (i[j++] < 0.5f && i[j++] < 0.85f) j++;
                            j += 2;
                            
                            // General size
			                generalSize = Custom.LerpMap(numScales, 5f, 35f, 1f, 0.2f);
                            generalSize = Mathf.Lerp(generalSize, p[2], i[j++]); // uses dominance
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

                            ScavUtil.GeneratePattern(i, ref j, ScavUtil.ScavBackType.WobblyBackTufts, pattern, out top, out bottom, out numScales);

                            // Pointer offsetting
                            if (pattern == ScavBodyScalePattern.RandomBackBlotch) j++;
                            j++;

                            // General size (p[2] = dominance)
			                generalSize = Mathf.Lerp(i[j++], p[2], i[j++]);
                            generalSize = Mathf.Lerp(generalSize, i[j++], i[j++]);
                            generalSize = Mathf.Pow(generalSize, Mathf.Lerp(2f, 0.65f, p[2]));

                            // More pointer offsetting (for future reference purposes)
                            // j += 9;
                            // j += numScales * (2 + (pattern == ScavBodyScalePattern.RandomBackBlotch ? 1 : 0) + (i[j++] != 1f ? 2 : 0));
                            // if (colored > 0 && i[j++] < 0.25f + 0.5f * colored) j++;
                        }

                        return new float[]
                        {
                            useHardBackSpikes ? 1 : 2,
                            colorType,
                            colored,
                            (int)pattern,
                            top, bottom,
                            numScales,
                            generalSize
                        };
                    }
                }
            },
            {
                "Elite Scavenger Back Patterns",
                new Setup
                {
                    Inputs = new SearchInput[]
                    {
                        new("See hover descriptions at bottom for most inputs", InputType.Label),
                        new("Color type", "1: none (color strength = 0), 2: decoration, 3: head", InputType.MultiChoice, (1, 3)),
                        new("Color strength", InputType.Float),
                        new("Pattern", "1: SpineRidge, 2: DoubleSpineRidge, 3: RandomBackBlotch", InputType.MultiChoice, (1, 3)),
                        new("Range top", InputType.Float, (0.02f, 0.3f)),
                        new("Range bottom", InputType.Float, (0.4f, 1f)),
                        new("Number of spines", "SpineRidge range: (2, 37), DoubleSpineRidge range: (2, 40), RandomBackBlotch range: (4, 40)", InputType.Integer, (2, 40)),
                        new("General spine size", InputType.Float),
                    },
                    MSC = true,
                    MinFloats = 154, // 86 (colors + before) + 4 (tail) + 56 (WobblyBackTufts) + 8
                    FloatRanges = null, IntRanges = null,
                    Apply = (i, p, s, _, _) =>
                    {
                        // Pre-generate some attributes we will need later
                        float generalMelanin = Custom.PushFromHalf(i[0], 2f);
                        float headSize = ClampedRandomVariation(0.5f, 0.5f, 0.1f, i[1], i[2]);
                        float eyeSize = Mathf.Pow(Mathf.Max(0f, Mathf.Lerp(i[4], Mathf.Pow(headSize, 0.5f), i[5] * 0.4f)), Mathf.Lerp(0.95f, 0.55f, p[5]));
                        float narrowEyes = (i[6] < Mathf.Lerp(0.3f, 0.7f, p[5])) ? 0f : Mathf.Pow(i[7], Mathf.Lerp(0.5f, 1.5f, p[5]));

                        // Calculate how far we have to advance the pointer
                        int j = 6; // first 6 rolls (gen. melanin, head size * 2, eartler width, eye size * 2)
                        #region skip advancement stuff

                        if(i[j++] >= Mathf.Lerp(0.3f, 0.7f, p[5])) j++; // narrow eyes
                        j += 9; // next 9 rolls (eyes angle, fatness * 3, narrow waist * 3, neck thickness * 2)

                        if (i[j++] < 0.65f && eyeSize > 0.4f && narrowEyes < 0.3f) // check to add pupils
                        {
	                        // Pupils and stuff
	                        if (i[j++] < Mathf.Pow(p[5], 1.5f) * 0.8f) // determine if sympathetic enough to have colored pupils
	                        {
		                        // colored pupils
		                        j++;
                                if(i[j++] < 0.6666667f) j++;
                            }
	                        // else deep pupils
                        }

                        j += 8; // tick 8 more rolls (hands head color * 3, legs size, arm thickness * 2, colored eartler tips, wide teeth)
                        int tailSegs = ((i[j++] < 0.5f) ? 0 : SearchUtil.GetRangeAt(s, new int[] {1, 5}, j++));
                        if (i[j++] < 0.25f) j++; // unused scruffy calculation that's still done for some reason

                        // Color time!
                        HSLColor bodyColor, headColor;
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
                            if (i[j++] < Mathf.Lerp(0.8f, 0.2f, p[3])) // energy
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

                        if(i[j++] >= 0.65f) j++; // deco color
                        j += 3; // also deco color

                        if(i[j++] < 0.2f) j++; // eye color

                        j += 2; // belly color
                        if (i[j++] < 0.033333335f)
                        {
                            j += 3;
                        }

                        j += tailSegs; // tail
                        #endregion

                        // Scav back thing time!
                        bool useHardBackSpikes = i[j++] < 0.1f;
                        float colorType = 1;
                        float colored = 0;
                        ScavBodyScalePattern pattern; // 1 = SpineRidge, 2 = DoubleSpineRidge, 3 = RandomBackBlotch
                        float top, bottom;
                        int numScales;
                        float generalSize;

                        // BackTuftsAndRidges constructor
                        j += 2; // graphic
                        if (i[j++] < 0.5f) j++;
                        if (i[j++] > generalMelanin)
                        {
                            colored = Mathf.Pow(i[j++], 0.5f);
                        }
                        if (colored > 0)
                        {
                            colorType = i[j++] < 0.5 ? 2 : 3;
                        }
                        else j++;
                        // HardBackSpikes (requires 56 random values max)

                        // Calculate pattern and generate corresponding attributes
                        pattern = i[j++] < 0.6f ? ScavBodyScalePattern.SpineRidge : ScavBodyScalePattern.DoubleSpineRidge;
                        if(i[j++] < 0.1f) pattern = ScavBodyScalePattern.RandomBackBlotch;
                        ScavUtil.GeneratePattern(i, ref j, ScavUtil.ScavBackType.HardBackSpikes, pattern, out top, out bottom, out numScales);

                        // Advance pointer
                        if (i[j++] < 0.5f && i[j++] < 0.85f) j++;
                        j += 2;
                            
                        // General size
			            generalSize = Custom.LerpMap(numScales, 5f, 35f, 1f, 0.2f);
                        generalSize = Mathf.Lerp(generalSize, p[2], i[j++]); // uses dominance
                        generalSize = Mathf.Lerp(generalSize, Mathf.Pow(i[j++], 0.75f), i[j++]);

                        // Extra pointer offsetting (for future reference purposes)
                        // if (colored > 0 && i[j++] < 0.25f + 0.5f * colored) j++;

                        return new float[]
                        {
                            useHardBackSpikes ? 1 : 2,
                            colorType,
                            colored,
                            (int)pattern,
                            top, bottom,
                            numScales,
                            generalSize
                        };
                    }
                }
            },
            
            // Slugpups
            {
                "Slugpup Variations",
                new Setup
                {
                    Inputs = new SearchInput[]
                    {
                        new("Size"), new("Wideness"),
                        Whitespace,
                        new("Color (H)", InputType.Float), new("Color (S)", InputType.Float), new("Color (L)", "Inverse if dark", (0.75f, 1f)),
                        new("Dark?", InputType.Boolean), new("Eye (L)", "Inverse if dark"),
                    },
                    MSC = true,
                    MinFloats = 0,
                    FloatRanges = new float[,]
                    {
                        //bal         met         stl         siz         wde         h                 block       block
                        { 0f, 1f }, { 0f, 1f }, { 0f, 1f }, { 0f, 1f }, { 0f, 1f }, { 0.15f, 0.58f }, { 0f, 0f }, { 0f, 0f },
                        //s           drk       block       eye
                        { 0f, 1f }, { 0f, 1f }, { 0f, 0f }, { 0f, 1f }
                    },
                    IntRanges = null,
                    Apply = (i, p, seed, fr, ir) =>
                    {
                        float bal, met, stl, siz, wde, eye, h, s, l;
                        bool drk;

                        // Physical attributes
                        met = Mathf.Pow(i[fr+1], 1.5f);
                        stl = Mathf.Pow(i[fr+2], 1.5f);
                        siz = Mathf.Pow(i[fr+3], 1.5f);
                        wde = Mathf.Pow(i[fr+4], 1.5f);
                        h = Mathf.Lerp(i[fr+5], i[6], Mathf.Pow(i[7], 1.5f - met));
                        s = Mathf.Pow(i[fr+8], 0.3f + stl * 0.3f);
                        drk = (i[fr+9] <= 0.3f + stl * 0.2f);
                        l = Mathf.Pow(SearchUtil.GetRangeAt(seed, new float[] { drk ? 0.9f : 0.75f, 1f }, 10), 1.5f - stl);
                        eye = Mathf.Pow(i[fr+11], 2f - stl * 1.5f);
                        // eye L actually more complicated than just lightness, lerps between room dark or white and inverse of some other color based on 0.25x this
                        // except it's not even the inverse, this is the calculation: new Color(1f - color.r, 1f - color.b, 1f - color.g)

                        // Special color cases
                        switch (seed)
                        {
                            case 1000:
                                // Custom.RGB2HSL(new Color(.6f, .7f, .9f))
                                h = 0.6111111f;
                                s = 0.6f;
                                l = 0.75f;
                                eye = 0f;
                                break;
                            case 1001:
                                // Custom.RGB2HSL(new Color(.48f, .87f, .81f))
                                drk = false;
                                h = 0.4743589f;
                                s = 0.6f;
                                l = 0.675f;
                                eye = 0f;
                                break;
                            case 1002:
                                // Custom.RGB2HSL(new Color(.43922f, .13725f, .23529f))
                                drk = true;
                                h = 0.9458887f;
                                s = 0.5238261f;
                                l = 0.288235f;
                                break;
                        }

                        return new float[]
                        {
                            siz, wde, h, s, l, drk ? 1f : 0f, eye,
                        };
                    }
                }
            },
            {
                "Slugpup Stats",
                new Setup
                {
                    Inputs = new SearchInput[] {
                        new("Body weight", (0.5525f, 0.715f)), new("Visibility (standing)", (-0.24f, -0.16f)), new("Visibility (crouching)", (0.45f, 0.75f)),
                        new("Loudness", (0.4f, 0.6f)), new("Lung capacity", (0.64f, 0.96f)), new("Pole climbing", (0.68f, 1f)),
                        new("Tunnel climbing", (0.68f, 1f)), new("Run speed", (0.68f, 1f))
                        // "Size", "Wideness", "Hue", "Saturation", "Lightness", "Dark?", "Eye",
                        // "Body weight", "Visibility (standing)", "Visibility (sneaking)", "Loudness",
                        // "Lung capacity", "Pole climbing", "Corridor climbing", "Run speed"
                    },
                    MSC = true,
                    MinFloats = 8,
                    FloatRanges = new float[,] {
                        //bal         met         stl         block       wde     
                        { 0f, 1f }, { 0f, 1f }, { 0f, 1f }, { 0f, 0f }, { 0f, 1f }
                    },
                    IntRanges = null,
                    Apply = (i, p, seed, fr, ir) => {
                        // float[] o = new float[18];
                        float bal, met, stl, wde;
                        // float bal, met, stl, siz, wde, eye, h, s, l;
                        // bool drk;

                        // Physical attributes
                        bal = Mathf.Pow(i[fr], 1.5f);
                        met = Mathf.Pow(i[fr+1], 1.5f);
                        stl = Mathf.Pow(i[fr+2], 1.5f);
                        //siz = Mathf.Pow(i[fr+3], 1.5f);
                        wde = Mathf.Pow(i[fr+4], 1.5f);
                        /*h = Mathf.Lerp(i[fr+5], i[6], Mathf.Pow(i[7], 1.5f - met));
                        s = Mathf.Pow(i[fr+8], 0.3f + stl * 0.3f);
                        drk = (i[fr+9] <= 0.3f + stl * 0.2f);
                        l = Mathf.Pow(SearchUtil.GetRangeAt(seed, new float[] { drk ? 0.9f : 0.75f, 1f }, 10), 1.5f - stl);
                        eye = Mathf.Pow(i[fr+11], 2f - stl * 1.5f);

                        // Special color cases
                        switch (seed)
                        {
                            case 1000:
                                // Custom.RGB2HSL(new Color(.6f, .7f, .9f))
                                h = 0.6111111f;
                                s = 0.6f;
                                l = 0.75f;
                                eye = 0f;
                                break;
                            case 1001:
                                // Custom.RGB2HSL(new Color(.48f, .87f, .81f))
                                drk = false;
                                h = 0.4743589f;
                                s = 0.6f;
                                l = 0.675f;
                                eye = 0f;
                                break;
                            case 1002:
                                // Custom.RGB2HSL(new Color(.43922f, .13725f, .23529f))
                                drk = true;
                                h = 0.9458887f;
                                s = 0.5238261f;
                                l = 0.288235f;
                                break;
                        }*/

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

                        return new float[] {
                            /*bal, met, stl,*/ // siz, wde, h, s, l, drk ? 1f : 0f, eye,
                            wgt, vz0, vz1, lou, lng, pol, tun, spd
                        };
                    }
                }
            },
            {
                "Slugpup Food",
                new Setup {
                    Inputs = new SearchInput[] {
                        new("Dangle fruit", (-1f, 1f)), new("Water nut", (-1f, 1f)), new("Jellyfish", (-1f, 1f)), new("Slime mold", (-1f, 1f)),
                        new("Eggbug egg", (-1f, 1f)), new("Fire egg", (-1f, 1f)), new("Popcorn", (-1f, 1f)), new("Gooieduck", (-1f, 1f)),
                        new("Lillypuck", (-1f, 1f)), new("Glow weed", (-1f, 1f)), new("Dandelion peach", (-1f, 1f)), new("Neuron", (-1f, 1f)),
                        new("Centipede", (-1f, 1f)), new("Small centipede", (-1f, 1f)), new("Vulture grub", (-1f, 1f)), new("Small noodlefly", (-1f, 1f)),
                        new("Hazer", (-1f, 1f))
                        // "Dangle fruit", "Water nut", "Jellyfish", "Slime mold", "Eggbug egg", "Fire egg", "Popcorn", "Gooieduck",
                        // "Lillypuck", "Glow weed", "Dandelion peach", "Neuron", "Centipede", "Small centipede", "Vulture grub",
                        // "Small noodlefly", "Hazer"
                    },
                    MSC = true,
                    MinFloats = 17 * 4,
                    FloatRanges = null, IntRanges = null,
                    Apply = (i, p, s, _, _) => {
                        float[] o = new float[17];

                        for (int j = 0; j < 17; j++)
                        {
                            #pragma warning disable CS8509 // Disables a warning here about not covering all cases/having a default case
                            (float a, float b) = j switch
                            {
                                0 => (p[4], p[3]),
                                1 => (p[5], p[0]),
                                2 => (p[3], p[4]),
                                3 => (p[3], p[0]),
                                4 => (p[2], p[3]),
                                5 => (p[0], p[5]),
                                6 => (p[2], p[1]),
                                7 => (p[5], p[1]),
                                8 => (p[0], p[4]),
                                9 => (p[4], p[3]),
                                10 => (p[1], p[2]),
                                11 => (p[1], p[4]),
                                12 => (p[1], p[2]),
                                13 => (p[3], p[0]),
                                14 => (p[2], p[1]),
                                15 => (p[0], p[5]),
                                16 => (p[4], p[5]),
                                // _ => throw new Exception("Invalid index while processing food preferences")
                            };
                            #pragma warning restore CS8509

                            a *= Custom.PushFromHalf(i[j * 4], 2f);
                            b *= Custom.PushFromHalf(i[j * 4 + 1], 2f);
                            o[j] = Mathf.Clamp(Mathf.Lerp(a - b, Mathf.Lerp(-1f, 1f, Custom.PushFromHalf(i[j * 4 + 2], 2f)), Custom.PushFromHalf(i[j * 4 + 3], 2f)), -1f, 1f);
                        }
                        return o;
                    }
                }
            },
            
            // Lantern mice
            {
                "Lantern Mouse Variations",
                new Setup
                {
                    Inputs = new SearchInput[] { new("Hue", InputType.Hue) { Wrap = true } },
                    MSC = false,
                    MinFloats = 3,
                    FloatRanges = null, IntRanges = null,
                    Apply = (i, p, s, _, _) =>
                    {
                        float hue;
                        if (i[0] < 0.01f)
                        {
                            hue = i[1];
                            // dominance = i[2];
                        }
                        else
                        {
                            if (i[1] < 0.5f)
                            {
                                hue = Mathf.Lerp(0f, 0.1f, i[2]);
                            }
                            else
                            {
                                hue = Mathf.Lerp(0.5f, 0.65f, i[2]);
                            }
                            // dominance = i[3];
                        }
                        return new float[] { hue };
                    }
                }
            },
            
            // Vultures
            {
                "Vulture Wing Variations",
                new Setup
                {
                    Inputs = new SearchInput[] {
                        new("Color A (H)", InputType.Hue),
                        new("Color A (S)", InputType.Float),
                        new("Color A (L)", InputType.Float),
                        Whitespace,
                        // new("Color A", InputType.ColorHSL),
                        new("Color B (H)", InputType.Hue),
                        new("Color B (S)", InputType.Float),
                        new("Color B (L)", InputType.Float),
                        Whitespace,
                        // new("Color B", InputType.ColorHSL),
                        new("Feather count", InputType.Integer, (13, 19))
                    },
                    MSC = false,
                    MinFloats = 8,
                    FloatRanges = null, IntRanges = new int[,] { { 13, 20 } },
                    Apply = (i, p, s, _, ir) =>
                    {
                        // Color variables
                        float nha, nsa, nla, nhb, nsb, nlb;

                        nha = Mathf.Lerp(0.9f, 1.6f, i[0]);
                        nsa = Mathf.Lerp(0.5f, 0.7f, i[1]);
                        nla = Mathf.Lerp(0.7f, 0.8f, i[2]);
                        nhb = nha + Mathf.Lerp(-0.25f, 0.25f, i[3]);
                        nsb = Mathf.Lerp(0.8f, 1f, 1f - i[4] * i[5]);
                        nlb = Mathf.Lerp(0.45f, 1f, i[6] * i[7]);

                        nha %= 1f; nhb %= 1f;

                        // Wing feather count
                        float nf = i[ir];

                        return new float[] {
                            nha, nsa, nla, nhb, nsb, nlb, nf
                        };
                    }
                }
            },
            {
                "King Vulture Wing Variations",
                new Setup
                {
                    Inputs = new SearchInput[] {
                        new("Color A (H)", InputType.Hue),
                        new("Color A (S)", InputType.Float),
                        new("Color A (L)", InputType.Float),
                        Whitespace,
                        // new("Color A", InputType.ColorHSL),
                        new("Color B (H)", InputType.Hue),
                        new("Color B (S)", InputType.Float),
                        new("Color B (L)", InputType.Float),
                        Whitespace,
                        // new("Color B", InputType.ColorHSL),
                        new("Feather count", InputType.Integer, (15, 24))
                    },
                    MSC = false,
                    MinFloats = 8,
                    FloatRanges = null, IntRanges = new int[,] { { 15, 25 } },
                    Apply = (i, p, s, _, ir) =>
                    {
                        // Color variables
                        float kha, ksa, kla, khb, ksb, klb;

                        khb = Mathf.Lerp(0.93f, 1.07f, i[0]);
                        ksb = Mathf.Lerp(0.8f, 1f, 1f - i[1] * i[2]);
                        klb = Mathf.Lerp(0.45f, 1f, i[3] * i[4]);
                        kha = khb + Mathf.Lerp(-0.25f, 0.25f, i[5]);
                        ksa = Mathf.Lerp(0.5f, 0.7f, i[6]);
                        kla = Mathf.Lerp(0.7f, 0.8f, i[7]);

                        kha %= 1f; khb %= 1f;

                        // Wing feather count
                        float kf = i[ir];

                        return new float[] {
                            kha, ksa, kla, khb, ksb, klb, kf
                        };
                    }
                }
            },
            
            // Noodleflies
            {
                "Adult Noodlefly Variations",
                new Setup
                {
                    Inputs = new SearchInput[]{
                        new("Wing size", (0.8f, 1.2f)),
                        new("Leg size", (0.6f, 1.4f)),
                        new("Fatness"),
                        new("Snout length", (0.5f, 1.5f)),
                        new("Body color", InputType.ColorRGB),
                        new("Eye color", "Tied to body color, may not give expected results", InputType.ColorRGB)
                    },
                    MSC = false,
                    MinFloats = 19,
                    FloatRanges = null, IntRanges = null,
                    Apply = (i, p, s, _, _) =>
                    {
                        float wingsSize, legsFac, fatness, snoutLength, hue, hueDiv, lightness;
                        int j = 13;
                        bool cosBools2 = (i[10] < 0.5f);

                        wingsSize = Mathf.Lerp(0.8f, 1.2f, ClampedRandomVariation(0.5f, 0.5f, 0.4f, i[0], i[1]));
                        legsFac = Mathf.Lerp(0.6f, 1.4f, i[2]);
                        fatness = ClampedRandomVariation(0.5f, 0.5f, 0.4f, i[3], i[4]);
                        snoutLength = Mathf.Lerp(0.5f, 1.5f, i[5]);
                        hue = WrappedRandomVariation(0.5f, 0.08f, 0.2f, i[6], i[7]);
                        lightness = 0.4f;
                        if (i[12] < 0.33333334f)
                        {
                            lightness = 0.4f * Mathf.Pow(i[j++], 5f);
                        }
                        else if (i[j++] < 0.05882353f)
                        {
                            lightness = 1f - 0.6f * Mathf.Pow(i[j++], 5f);
                        }
                        hueDiv = Mathf.Lerp(-1f, 1f, i[j++]) * Custom.LerpMap(Mathf.Abs(0.5f - hue), 0f, 0.08f, 0.06f, 0.3f);
                        if (lightness < 0.4f && i[j++] > lightness && i[j++] < 0.1f)
                        {
                            hue = i[j++];
                        }

                        // Calculate true colors
                        Color bodyColor, eyeColor; // there is also a highlight color and detail color I think affects legs and wings
                        hue += 0.478f;
                        bodyColor = Custom.HSL2RGB(hue, Custom.LerpMap(lightness, 0.5f, 1f, 0.9f, 0.5f), Mathf.Lerp(0.1f, 0.8f, Mathf.Pow(lightness, 2f)));

                        hue += Mathf.InverseLerp(0.5f, 0.6f, lightness) * 0.5f;
                        if (cosBools2)
                        {
                            eyeColor = Custom.HSL2RGB(hue + 0.5f - hueDiv, 1f, Mathf.Lerp(0.7f, 0.3f, Mathf.Pow(lightness, 1.5f)));
                        }
                        else
                        {
                            eyeColor = Custom.HSL2RGB(hue + 0.5f, 1f, Mathf.Lerp(0.7f, 0.3f, Mathf.Pow(lightness, 1.5f)));
                        }

                        return new float[] {
                            wingsSize, legsFac, fatness, snoutLength,
                            bodyColor.r, bodyColor.g, bodyColor.b,
                            eyeColor.r, eyeColor.g, eyeColor.b
                        };
                    }
                }
            },
            {
                "Baby Noodlefly Variations",
                new Setup
                {
                    Inputs = new SearchInput[]{
                        new("Wing size", (0.4f, 0.8f)),
                        new("Leg size", (0.48f, 1.12f)),
                        new("Fatness"),
                        new("Snout length"),
                        new("Body color", InputType.ColorRGB),
                        new("Eye color", "Tied to body color, may not give expected results", InputType.ColorRGB)
                        // "Wing size", "Fatness", "Snout length",
                        // "Body R", "Body G", "Body B",
                        // "Eyes R", "Eyes G", "Eyes B"
                    },
                    MSC = false,
                    MinFloats = 14,
                    FloatRanges = null, IntRanges = null,
                    Apply = (i, p, s, _, _) =>
                    {
                        float wingsSize, legsFac, fatness, snoutLength, hue, hueDiv, lightness;
                        bool cosBools2 = (i[10] < 0.5f);

                        wingsSize = 0.5f * Mathf.Lerp(0.8f, 1.2f, ClampedRandomVariation(0.5f, 0.5f, 0.4f, i[0], i[1]));
                        legsFac = 0.8f * Mathf.Lerp(0.6f, 1.4f, i[2]);
                        fatness = ClampedRandomVariation(0.5f, 0.5f, 0.4f, i[3], i[4]);
                        snoutLength = Mathf.Lerp(0.5f, 1.5f, i[5]);
                        hue = WrappedRandomVariation(0.5f, 0.08f, 0.2f, i[6], i[7]);
                        lightness = Mathf.Lerp(0.3f, 1f, Mathf.Pow(ClampedRandomVariation(0.5f, 0.5f, 0.4f, i[12], i[13]), 0.4f));
                        hueDiv = 0f;

                        // Calculate true colors
                        Color bodyColor, eyeColor; // there is also a highlight color and detail color I think affects legs and wings
                        hue += 0.478f;
                        bodyColor = Custom.HSL2RGB(hue, Custom.LerpMap(lightness, 0.5f, 1f, 0.9f, 0.5f), Mathf.Lerp(0.1f, 0.8f, Mathf.Pow(lightness, 2f)));

                        hue += Mathf.InverseLerp(0.5f, 0.6f, lightness) * 0.5f;
                        if (cosBools2)
                        {
                            eyeColor = Custom.HSL2RGB(hue + 0.5f - hueDiv, 1f, Mathf.Lerp(0.7f, 0.3f, Mathf.Pow(lightness, 1.5f)));
                        }
                        else
                        {
                            eyeColor = Custom.HSL2RGB(hue + 0.5f, 1f, Mathf.Lerp(0.7f, 0.3f, Mathf.Pow(lightness, 1.5f)));
                        }

                        return new float[] {
                            wingsSize, legsFac, fatness, snoutLength,
                            bodyColor.r, bodyColor.g, bodyColor.b,
                            eyeColor.r, eyeColor.g, eyeColor.b
                        };
                    }
                }
            },
            
            // Lizors (generic)
            {
                "Lizard Variations (Generic)",
                new Setup
                {
                    Inputs = new SearchInput[]
                    {
                        new("Head size", InputType.Float, (0.86f, 1.14f)),
                        new("Fatness", "This value does not work for black lizards. Red lizards will never have this value above 1." + (ModManager.MSC ? " Strawberry lizards will never have this value above 0.8" : ""), InputType.Float, (0.76f, 1.24f)),
                        new("Tail length", InputType.Float, (0.6f, 1.4f)),
                        new("Tail fatness", "Red lizards will never have this value above 1." + (ModManager.MSC ? " Strawberry lizards will never have this value above 0.9" : ""), InputType.Float, (0.7f, 1.1f)),
                        new("Tail color", "Strength of tail gradient. This will be 0 (none) roughly 50% of the time, and will always be 0 for white lizards.")
                    },
                    MSC = false,
                    MinFloats = 11,
                    FloatRanges = null, IntRanges = null,
                    Apply = (i, p, s, _, _) =>
                    {
                        float headSize = ClampedRandomVariation(0.5f, 0.07f, 0.5f, i[0], i[1]) * 2f;
                        if (i[2] < 0.5f)
                        {
                            headSize = 1f;
                        }
                        float fatness = ClampedRandomVariation(0.5f, 0.12f, 0.5f, i[3], i[4]) * 2f;
                        float tailLength = ClampedRandomVariation(0.5f, 0.2f, 0.3f, i[5], i[6]) * 2f;
                        float tailFatness = ClampedRandomVariation(0.45f, 0.1f, 0.3f, i[7], i[8]) * 2f;
                        float tailColor = 0f;
                        if (i[9] > 0.5f)
                        {
                            tailColor = i[10];
                        }
                        return new float[]
                        {
                            headSize,
                            fatness,
                            tailLength,
                            tailFatness,
                            tailColor
                        };
                    }
                }
            },
            
            // Lizors (specific)
            {
                "Pink Lizard Variations",
                new Setup
                {
                    Inputs = new SearchInput[] {
                        new("Body pattern (see description on hover)", "1: N/A, 2: BumpHawk, 3: LongShoulderScales, 4: ShortBodyScales, 5: SpineSpikes", InputType.MultiChoice, (1, 5)),
                        new("Has TailTuft?", InputType.Boolean),
                        new("Has LongHeadScales?", InputType.Boolean),
                        new("Color (H)", InputType.Hue, (0.77f, 0.97f)),
                        new("Color (L)", InputType.Float, (0.35f, 0.65f)),
                    },
                    MSC = false,
                    MinFloats = 114, // 12 (iVars) + 5 (legs & head) + 5 (tail segs) + 43 (LongShoulderScales) + 27 (TailTufts) + 9 (LongHeadScales) + 13
                    FloatRanges = null, IntRanges = null,
                    Apply = (i, p, s, _, _) =>
                    {
                        int j = i[9] > 0.5f ? 12 : 11; // accounts for generic i vars + black salamander chance

                        // Deal with stupid body parts and stuff
                        j += 5; // 7 if caramel, accounts for legs and head
                        j += LizardUtil.NumTailSegments(LizardType.Pink); // 5
                        
                        // 1: N/A, 2: BumpHawk, 3: LongShoulderScales, 4: ShortBodyScales, 5: SpineSpikes
                        int bodyDecoration = 1;

                        if (i[j++] < 0.06666667f) // 1/15
                        {
                            LizardUtil.SpineSpikesVars(i, ref j, s, LizardType.Pink, out _, out _);
                            bodyDecoration = 5;
                        }
                        else
                        {
                            j += 2;
                            if (i[j++] < 0.033333335f) // 1/30
                            {
                                LizardUtil.BumpHawkVars(i, ref j, LizardType.Pink, out _, out _, out _);
                                bodyDecoration = 2;
                            }
                            else if (i[j++] < 0.04761905f || i[j++] < 0.5f) // 1/21 on first number
                            {
                                LizardUtil.LongShoulderScalesVars(i, ref j, s, LizardType.Pink, out _, out _, out _);
                                bodyDecoration = 3;
                            }
                            else if (i[j++] < 0.0625f)
                            {
                                LizardUtil.ShortBodyScalesVars(i, ref j, s, LizardType.Pink, out _, out _);
                                bodyDecoration = 4;
                            }
                        }

                        bool hasTailTuft = false;
                        if (i[j++] < 0.11111111f || (bodyDecoration == 1 && i[j++] < 0.7f) || i[j++] < 0.6f)
                        {
                            LizardUtil.TailTuftVars(i, ref j, s, LizardType.Pink, out _, out _);
                            hasTailTuft = true;
                        }

                        bool hasLHS = false;
                        if (i[j++] < (bodyDecoration == 1 ? 0.7f : 0.1f) && ((bodyDecoration != 3 && i[j++] < 0.9f) || i[j++] < 0.033333335f))
                        {
                            LizardUtil.LongHeadScalesVars(i, ref j, LizardType.Pink, out _, out _, out _, out _);
                            hasLHS = true;
                        }

                        return new float[] {
                            bodyDecoration,
                            hasTailTuft ? 1f : 0f,
                            hasLHS ? 1f : 0f,
                            // Hue
                            WrappedRandomVariation(0.87f, 0.1f, 0.6f, i[0], i[1]),
                            // Lightness
                            ClampedRandomVariation(0.5f, 0.15f, 0.1f, i[2], i[3])
                        };
                    }
                }
            },
            {
                "Green Lizard Variations",
                new Setup
                {
                    Inputs = new SearchInput[] {
                        new("Body pattern (see description on hover)", "1: N/A, 2: BumpHawk, 3: ShortBodyScales, 4: SpineSpikes", InputType.MultiChoice, (1, 4)),
                        new("Has TailTuft?", InputType.Boolean),
                        new("Has LongShoulderScales?", InputType.Boolean),
                        new("Has LongHeadScales?", InputType.Boolean),
                        new("Color (H)", InputType.Hue, (0.22f, 0.42f)),
                        new("Color (L)", InputType.Float, (0.35f, 0.65f)),
                    },
                    MSC = false,
                    MinFloats = 150, // 12 (iVars) + 5 (head & legs) + 7 (tail segs) + 33 (ShortBodyScales) + 43 (LongShoulderScales) + 27 (TailTufts) + 9 (LongHeadScales) + 14
                    FloatRanges = null, IntRanges = null,
                    Apply = (i, p, s, _, _) =>
                    {
                        int j = i[9] > 0.5f ? 12 : 11; // accounts for generic i vars + black salamander chance

                        // Deal with stupid body parts and stuff
                        j += 5; // 7 if caramel, accounts for legs and head
                        j += LizardUtil.NumTailSegments(LizardType.Green); // 7
                        
                        // 1: N/A, 2: BumpHawk, 3: ShortBodyScales, 4: SpineSpikes
                        int bodyDecoration = 1;
                        bool hasLSS = false;

                        if (i[j++] < 0.06666667f || i[j++] < 0.8f)
                        {
                            LizardUtil.SpineSpikesVars(i, ref j, s, LizardType.Green, out _, out _);
                            bodyDecoration = 4;
                        }
                        else
                        {
                            j++;
                            if (i[j++] < 0.033333335f)
                            {
                                LizardUtil.BumpHawkVars(i, ref j, LizardType.Green, out _, out _, out _);
                                bodyDecoration = 2;
                            }
                            else if (i[j++] < 0.04761905f)
                            {
                                LizardUtil.LongShoulderScalesVars(i, ref j, s, LizardType.Green, out _, out _, out _);
                                hasLSS = true;
                            }
                            else if (i[j++] < 0.0625f || i[j++] < 0.5f)
                            {
                                LizardUtil.ShortBodyScalesVars(i, ref j, s, LizardType.Green, out _, out _);
                                bodyDecoration = 3;
                            }
                        }

                        bool hasTailTuft = false;
                        if (i[j++] < 0.11111111f || ((bodyDecoration == 1 && !hasLSS) && i[j++] < 0.7f))
                        {
                            LizardUtil.TailTuftVars(i, ref j, s, LizardType.Green, out _, out _);
                            hasTailTuft = true;
                        }
                        else if (i[j++] < 0.7f)
                        {
                            if (i[j++] < 0.5f || hasLSS || bodyDecoration == 3)
                            {
                                LizardUtil.TailTuftVars(i, ref j, s, LizardType.Green, out _, out _);
                                hasTailTuft = true;
                            }
                            else
                            {
                                LizardUtil.LongShoulderScalesVars(i, ref j, s, LizardType.Green, out _, out _, out _);
                                hasLSS = true;
                            }
                        }

                        bool hasLHS = false;
                        if (i[j++] < (bodyDecoration == 1 ? 0.7f : 0.1f) && ((!hasLSS && i[j++] < 0.9f) || i[j++] < 0.033333335f))
                        {
                            LizardUtil.LongHeadScalesVars(i, ref j, LizardType.Green, out _, out _, out _, out _);
                            hasLHS = true;
                        }

                        return new float[] {
                            bodyDecoration,
                            hasTailTuft ? 1f : 0f,
                            hasLSS ? 1f : 0f,
                            hasLHS ? 1f : 0f,
                            // Hue
                            WrappedRandomVariation(0.32f, 0.1f, 0.6f, i[0], i[1]),
                            // Lightness
                            ClampedRandomVariation(0.5f, 0.15f, 0.1f, i[2], i[3])
                        };
                    }
                }
            },
            {
                "Blue Lizard Variations",
                new Setup
                {
                    Inputs = new SearchInput[] {
                        new("Body pattern (see description on hover)", "1: N/A, 2: BumpHawk, 3: LongShoulderScales, 4: ShortBodyScales, 5: SpineSpikes", InputType.MultiChoice, (1, 5)),
                        new("Has TailTuft?", InputType.Boolean),
                        new("Has LongHeadScales?", InputType.Boolean),
                        new("Color (H)", InputType.Hue, (0.49f, 0.65f)),
                        new("Color (L)", InputType.Float, (0.35f, 0.65f)),
                    },
                    MSC = false,
                    MinFloats = 118, // 12 (iVars) + 5 (legs & head) + 4 (tail segs) + 5 (tongue segs) + 43 (LongShoulderScales) + 27 (TailTufts) + 9 (LongHeadScales) + 13
                    FloatRanges = null, IntRanges = null,
                    Apply = (i, p, s, _, _) =>
                    {
                        int j = i[9] > 0.5f ? 12 : 11; // accounts for generic i vars + black salamander chance

                        // Deal with stupid body parts and stuff
                        j += 5; // accounts for legs and head
                        j += LizardUtil.NumTailSegments(LizardType.Blue); // 4
                        j += LizardUtil.NumTongueSegments(LizardType.Blue); // 5
                        
                        // 1: N/A, 2: BumpHawk, 3: LongShoulderScales, 4: ShortBodyScales, 5: SpineSpikes
                        int bodyDecoration = 1;

                        if (i[j++] < 0.06666667f)
                        {
                            LizardUtil.SpineSpikesVars(i, ref j, s, LizardType.Blue, out _, out _);
                            bodyDecoration = 5;
                        }
                        else
                        {
                            j += 2;
                            if (i[j++] < 0.033333335f)
                            {
                                LizardUtil.BumpHawkVars(i, ref j, LizardType.Blue, out _, out _, out _);
                                bodyDecoration = 2;
                            }
                            else if (i[j++] < 0.04761905f)
                            {
                                LizardUtil.LongShoulderScalesVars(i, ref j, s, LizardType.Blue, out _, out _, out _);
                                bodyDecoration = 3;
                            }
                            else if (i[j++] < 0.0625f || i[j++] < 0.5f)
                            {
                                LizardUtil.ShortBodyScalesVars(i, ref j, s, LizardType.Blue, out _, out _);
                                bodyDecoration = 4;
                            }
                        }

                        bool hasTailTuft = false;
                        if (i[j++] < 0.11111111f || (bodyDecoration == 1 && i[j++] < 0.7f) || i[j++] < 0.96f) // it is RARE to have a blue lizor that doesn't have TailTuft
                        {
                            LizardUtil.TailTuftVars(i, ref j, s, LizardType.Blue, out _, out _);
                            hasTailTuft = true;
                        }

                        bool hasLHS = false;
                        if (i[j++] < (bodyDecoration == 1 ? 0.7f : 0.1f) && ((bodyDecoration != 3 && i[j++] < 0.9f) || i[j++] < 0.033333335f))
                        {
                            LizardUtil.LongHeadScalesVars(i, ref j, LizardType.Blue, out _, out _, out _, out _);
                            hasLHS = true;
                        }

                        return new float[] {
                            bodyDecoration,
                            hasTailTuft ? 1f : 0f,
                            hasLHS ? 1f : 0f,
                            // Hue
                            WrappedRandomVariation(0.57f, 0.08f, 0.6f, i[0], i[1]),
                            // Lightness
                            ClampedRandomVariation(0.5f, 0.15f, 0.1f, i[2], i[3])
                        };
                    }
                }
            },
            {
                "Yellow Lizard Variations",
                new Setup
                {
                    Inputs = new SearchInput[] {
                        new("Body pattern (see description on hover)", "1: N/A, 2: BumpHawk, 3: LongShoulderScales, 4: ShortBodyScales, 5: SpineSpikes", InputType.MultiChoice, (1, 5)),
                        new("Has TailTuft?", InputType.Boolean),
                        new("Antennae length", InputType.Float),
                        new("Antennae effect alpha", "This will always be no less than 0.1 under antennae length, nor will it exceed antennae length.", InputType.Float),
                        new("Color (H)", InputType.Hue, (0.05f, 0.15f)),
                        new("Color (L)", InputType.Float, (0.35f, 0.65f)),
                    },
                    MSC = false,
                    MinFloats = 119, // 12 (iVars) + 5 (legs & head) + 5 (tail segs) + 43 (LongShoulderScales) + 27 (TailTufts) + 18 (Antennae) + 9
                    FloatRanges = null, IntRanges = null,
                    Apply = (i, p, s, _, _) =>
                    {
                        int j = i[9] > 0.5f ? 12 : 11; // accounts for generic i vars + black salamander chance

                        // Deal with stupid body parts and stuff
                        j += 5; // accounts for legs and head
                        j += LizardUtil.NumTailSegments(LizardType.Yellow); // 5


                        // Possible patterns: BumpHawk, LongHeadScales, LongShoulderScales, ShortBodyScales, SpineSpikes, TailTuft
                        
                        // 1: N/A, 2: BumpHawk, 3: LongShoulderScales, 4: ShortBodyScales, 5: SpineSpikes
                        int bodyDecoration = 1;

                        if (i[j++] < 0.06666667f)
                        {
                            LizardUtil.SpineSpikesVars(i, ref j, s, LizardType.Yellow, out _, out _);
                            bodyDecoration = 5;
                        }
                        else
                        {
                            j += 2;
                            if (i[j++] < 0.033333335f)
                            {
                                LizardUtil.BumpHawkVars(i, ref j, LizardType.Yellow, out _, out _, out _);
                                bodyDecoration = 2;
                            }
                            else if (i[j++] < 0.04761905f)
                            {
                                LizardUtil.LongShoulderScalesVars(i, ref j, s, LizardType.Yellow, out _, out _, out _);
                                bodyDecoration = 3;
                            }
                            else if (i[j++] < 0.0625f)
                            {
                                LizardUtil.ShortBodyScalesVars(i, ref j, s, LizardType.Yellow, out _, out _);
                                bodyDecoration = 4;
                            }
                        }

                        bool hasTailTuft = false;
                        if (i[j++] < 0.11111111f || (bodyDecoration == 1 && i[j++] < 0.7f))
                        {
                            LizardUtil.TailTuftVars(i, ref j, s, LizardType.Yellow, out _, out _);
                            hasTailTuft = true;
                        }

                        j++;

                        LizardUtil.AntennaeVars(i, ref j, out float antLength, out float alpha);
                        if (bodyDecoration == 1 && i[j++] < 0.6f)
                        {
                            LizardUtil.ShortBodyScalesVars(i, ref j, s, LizardType.Yellow, out _, out _);
                            bodyDecoration = 4;
                        }

                        return new float[] {
                            bodyDecoration,
                            hasTailTuft ? 1f : 0f,
                            antLength,
                            alpha,
                            // Hue
                            WrappedRandomVariation(0.1f, 0.05f, 0.6f, i[0], i[1]),
                            // Lightness
                            ClampedRandomVariation(0.5f, 0.15f, 0.1f, i[2], i[3])
                        };
                    }
                }
            },
            {
                "White Lizard Variations",
                new Setup
                {
                    Inputs = new SearchInput[] {
                        new("Body pattern (see description on hover)", "1: N/A, 2: BumpHawk, 3: LongHeadScales, 4: LongShoulderScales, 5: ShortBodyScales", InputType.MultiChoice, (1, 5)),
                        new("Has TailTuft?", InputType.Boolean),
                    },
                    MSC = false,
                    MinFloats = 105, // 10 (iVars) + 5 (legs & head) + 5 (tail segs) + 10 (tongue segs) + 43 (LongShoulderScales) + 27 (TailTufts) + 5
                    FloatRanges = null, IntRanges = null,
                    Apply = (i, p, s, _, _) =>
                    {
                        int j = 10; // accounts for generic i vars (different behavior for white lizor) + black salamander chance

                        // Deal with stupid body parts and stuff
                        j += 5; // accounts for legs and head
                        j += LizardUtil.NumTailSegments(LizardType.White); // 5
                        j += LizardUtil.NumTongueSegments(LizardType.White); // 10

                        
                        // 1: N/A, 2: BumpHawk, 3: LongShoulderScales, 4: ShortBodyScales
                        int bodyDecoration = 1;
                        if (i[j++] < 0.4f)
                        {
                            LizardUtil.BumpHawkVars(i, ref j, LizardType.White, out _, out _, out _);
                            bodyDecoration = 2;
                        }
                        else if (i[j++] < 0.4f)
                        {
                            LizardUtil.ShortBodyScalesVars(i, ref j, s, LizardType.White, out _, out _);
                            bodyDecoration = 5;
                        }
                        else if (i[j++] < 0.2f)
                        {
                            LizardUtil.LongShoulderScalesVars(i, ref j, s, LizardType.White, out _, out _, out _);
                            bodyDecoration = 4;
                        }
                        else if (i[j++] < 0.2f)
                        {
                            LizardUtil.LongHeadScalesVars(i, ref j, LizardType.White, out _, out _, out _, out _);
                            bodyDecoration = 3;
                        }

                        bool hasTailTuft = false;
                        if (i[j++] < 0.5f)
                        {
                            LizardUtil.TailTuftVars(i, ref j, s, LizardType.White, out _, out _);
                            hasTailTuft = true;
                        }


                        return new float[] {
                            bodyDecoration,
                            hasTailTuft ? 1f : 0f
                        };
                    }
                }
            },
            {
                "Red Lizard Variations",
                new Setup
                {
                    Inputs = new SearchInput[] {
                        new("NOTE: red lizards are different if \"Alpha Reds\" is enabled in Rain World Remix.", InputType.Label),
                        new("Body pattern (see description on hover)", "1: N/A, 2: BumpHawk, 3: extra LongShoulderScales, 4: ShortBodyScales, 5: extra SpineSpikes", InputType.MultiChoice, (1, 5)),
                        new("Main tail pattern (see description on hover)", "1: TailTuft, 2: TailFin", InputType.MultiChoice, (1, 2)),
                        new("Additional TailTuft?", "Separate from choice above", InputType.Boolean),
                        new("Has LongHeadScales?", InputType.Boolean),
                        new("Color (H)", InputType.Hue, (-0.0175f, 0.0225f)),
                        new("Color (L)", InputType.Float, (0.35f, 0.65f)),
                    },
                    MSC = false,
                    MinFloats = 178, // 205 if re-add TailTuft/TailFin check; 12 (ivars) + 5 (legs & head) + 11 (tail segs) + 10 (potential tongue segs) + 43 (LongShoulderScales, potential) + 43 (LongShoulderScales, guaranteed) + 27 (SKIPPED: TailTuft, 50% chance) + 27 (TailTuft, extra) + 14 (SpineSpikes, guaranteed) + 13
                    FloatRanges = null, IntRanges = null,
                    Apply = (i, p, s, _, _) =>
                    {
                        int j = i[9] > 0.5f ? 12 : 11; // accounts for generic i vars + black salamander chance

                        // Deal with stupid body parts and stuff
                        j += 5; // accounts for legs and head
                        j += LizardUtil.NumTailSegments(LizardType.Red); // 11
                        j += LizardUtil.NumTongueSegments(LizardType.Red); // 0 or 10 depending on Rain World Remix -> Alpha Reds
                        
                        // 1: N/A, 2: BumpHawk, 3: LongShoulderScales, 4: ShortBodyScales, 5: SpineSpikes
                        int bodyDecoration = 1;

                        if (i[j++] < 0.06666667f)
                        {
                            LizardUtil.SpineSpikesVars(i, ref j, s, LizardType.Red, out _, out _);
                            bodyDecoration = 5;
                        }
                        else
                        {
                            j += 2;
                            if (i[j++] < 0.033333335f)
                            {
                                LizardUtil.BumpHawkVars(i, ref j, LizardType.Red, out _, out _, out _);
                                bodyDecoration = 2;
                            }
                            else if (i[j++] < 0.04761905f || i[j++] < 0.9f)
                            {
                                LizardUtil.LongShoulderScalesVars(i, ref j, s, LizardType.Red, out _, out _, out _);
                                bodyDecoration = 3;
                            }
                            else if (i[j++] < 0.0625f)
                            {
                                LizardUtil.ShortBodyScalesVars(i, ref j, s, LizardType.Red, out _, out _);
                                bodyDecoration = 4;
                            }
                        }

                        bool hasExtraTailTuft = false;
                        if (i[j++] < 0.11111111f || (bodyDecoration == 1 && i[j++] < 0.7f))
                        {
                            LizardUtil.TailTuftVars(i, ref j, s, LizardType.Red, out _, out _);
                            hasExtraTailTuft = true;
                        }

                        bool hasLHS = false;
                        if (i[j++] < (bodyDecoration == 1 ? 0.7f : 0.1f) && ((bodyDecoration != 3 && i[j++] < 0.9f) || i[j++] < 0.033333335f))
                        {
                            LizardUtil.LongHeadScalesVars(i, ref j, LizardType.Red, out _, out _, out _, out _);
                            hasLHS = true;
                        }

                        LizardUtil.LongShoulderScalesVars(i, ref j, s, LizardType.Red, out _, out _, out _);
                        LizardUtil.SpineSpikesVars(i, ref j, s, LizardType.Red, out _, out _);

                        bool hasTailFin = i[j++] < 0.5f;
                        /*if (hasTailFin)
                            LizardUtil.TailFinVars(i, ref j, LizardType.Red, out _, out _, out _, out _, out _);
                        else
                            LizardUtil.TailTuftVars(i, ref j, s, LizardType.Red, out _, out _);*/

                        return new float[] {
                            bodyDecoration,
                            hasTailFin ? 2f : 1f,
                            hasExtraTailTuft ? 1f : 0f,
                            hasLHS ? 1f : 0f,
                            // Hue
                            WrappedRandomVariation(0.0025f, 0.02f, 0.6f, i[0], i[1]),
                            // Lightness
                            ClampedRandomVariation(0.5f, 0.15f, 0.1f, i[2], i[3])
                        };
                    }
                }
            },
            {
                "Black Lizard Variations",
                new Setup
                {
                    Inputs = new SearchInput[] {
                        new("Body pattern (see description on hover)", "1: N/A, 2: BumpHawk, 3: LongShoulderScales, 4: ShortBodyScales, 5: SpineSpikes", InputType.MultiChoice, (1, 5)),
                        new("Has TailTuft?", InputType.Boolean),
                        new("Has LongHeadScales?", InputType.Boolean),
                        new("# whiskers per side", InputType.Integer, (3, 4))
                    },
                    MSC = false,
                    MinFloats = 163, // 14 (iVars) + 5 (legs & head) + 6 (tail segs) + 43 (LongShoulderScales) + 27 (TailTufts) + 9 (LongHeadScales) + 48 (Whiskers) + 11
                    FloatRanges = null, IntRanges = null,
                    Apply = (i, p, s, _, _) =>
                    {
                        int j = i[9] > 0.5f ? 14 : 13; // accounts for generic i vars (different behavior for black lizards) + black salamander chance

                        // Deal with stupid body parts and stuff
                        j += 5; // accounts for legs and head
                        j += LizardUtil.NumTailSegments(LizardType.Black); // 6


                        // Possible patterns: BumpHawk, LongHeadScales, LongShoulderScales, ShortBodyScales, SpineSpikes, TailTuft
                        
                        // 1: N/A, 2: BumpHawk, 3: LongShoulderScales, 4: ShortBodyScales, 5: SpineSpikes
                        int bodyDecoration = 1;

                        if (i[j++] < 0.06666667f || j++ == 0 || i[j++] < 0.7f) // second condition will never be true, just there to increase j
                        {
                            LizardUtil.SpineSpikesVars(i, ref j, s, LizardType.Black, out _, out _);
                            bodyDecoration = 5;
                        }
                        else
                        {
                            if (i[j++] < 0.033333335f)
                            {
                                LizardUtil.BumpHawkVars(i, ref j, LizardType.Black, out _, out _, out _);
                                bodyDecoration = 2;
                            }
                            else if (i[j++] < 0.04761905f)
                            {
                                LizardUtil.LongShoulderScalesVars(i, ref j, s, LizardType.Black, out _, out _, out _);
                                bodyDecoration = 3;
                            }
                            else if (i[j++] < 0.0625f)
                            {
                                LizardUtil.ShortBodyScalesVars(i, ref j, s, LizardType.Black, out _, out _);
                                bodyDecoration = 4;
                            }
                        }

                        bool hasTailTuft = false;
                        if (i[j++] < 0.11111111f || (bodyDecoration == 1 && i[j++] < 0.7f))
                        {
                            LizardUtil.TailTuftVars(i, ref j, s, LizardType.Black, out _, out _);
                            hasTailTuft = true;
                        }

                        bool hasLHS = false;
                        if (i[j++] < (bodyDecoration == 1 ? 0.7f : 0.1f) && ((bodyDecoration != 3 && i[j++] < 0.9f) || i[j++] < 0.033333335f))
                        {
                            LizardUtil.LongHeadScalesVars(i, ref j, LizardType.Black, out _, out _, out _, out _);
                            hasLHS = true;
                        }

                        LizardUtil.WhiskersVars(i, ref j, s, out int numWhiskers);

                        return new float[]
                        {
                            bodyDecoration,
                            hasTailTuft ? 1f : 0f,
                            hasLHS ? 1f : 0f,
                            numWhiskers
                        };
                    }
                }
            },
            {
                "Salamander Variations",
                new Setup
                {
                    Inputs = new SearchInput[] {
                        new("NOTE: may be inaccurate for custom regions.", InputType.Label),
                        new("Body pattern (see description on hover)", "1: N/A, 2: BumpHawk, 3: SpineSpikes", InputType.MultiChoice, (1, 3)),
                        new("Melanistic?", InputType.Boolean),
                        Whitespace,
                        new("NOTE: this is not the actual body color, it just influences it.", InputType.Label),
                        new("Color (H)", InputType.Hue, (0.75f, 1.05f)),
                        new("Color (L)", InputType.Float, (0.25f, 0.55f)),
                    },
                    MSC = false,
                    MinFloats = 46, // 95 if readd AxolotlGills and TailFin; 11 (ivars) + 1 (black salamander) + 5 (legs & head) + 5 (tail segs) + 7 (tongue segs) + 14 (SpineSpikes) + 35 (SKIPPED; AxolotlGills) + 14 (SKIPPED; TailFin) + 3
                    FloatRanges = null, IntRanges = null,
                    Apply = (i, p, s, _, _) =>
                    {
                        int j = i[9] > 0.5f ? 11 : 10; // accounts for generic i vars

                        // Deal with stupid body parts and stuff
                        j += 5; // accounts for legs and head
                        j += LizardUtil.NumTailSegments(LizardType.Salamander); // 5
                        j += LizardUtil.NumTongueSegments(LizardType.Salamander); // 7

                        // Black salamander chance (default)
                        bool blackSalamander = i[j++] < 0.33333334f;


                        // 1: N/A, 2: BumpHawk, 3: SpineSpikes
                        int bodyDecoration = 1;

                        if (i[j++] < 0.06666667f)
                        {
                            LizardUtil.SpineSpikesVars(i, ref j, s, LizardType.Salamander, out _, out _);
                            bodyDecoration = 3;
                        }
                        else
                        {
                            j += 2;
                            if (i[j++] < 0.033333335f)
                            {
                                LizardUtil.BumpHawkVars(i, ref j, LizardType.Salamander, out _, out _, out _);
                                bodyDecoration = 2;
                            }
                            j += 2;
                        }

                        j++;

                        // LizardUtil.AxolotlGillsVars(i, ref j, s, out _, out _, out _);
                        // LizardUtil.TailFinVars(i, ref j, LizardType.Salamander, out _, out _, out _, out _, out _);

                        return new float[] {
                            bodyDecoration,
                            blackSalamander ? 1 : 0,
                            // Hue
                            WrappedRandomVariation(0.9f, 0.15f, 0.6f, i[0], i[1]),
                            // Lightness
                            ClampedRandomVariation(0.4f, 0.15f, 0.2f, i[2], i[3])
                        };
                    }
                }
            },
            {
                "Cyan Lizard Variations",
                new Setup
                {
                    Inputs = new SearchInput[] {
                        new("Has WingScales?", InputType.Boolean),
                        new("TailGeckoScales (1) or TailTuft (2)?", "NOTE: TailTuft will only appear if tail color is 0 (see generic lizard variations).", InputType.MultiChoice, (0, 1)),
                        new("Color (H)", InputType.Hue, (0.45f, 0.53f)),
                        new("Color (L)", InputType.Float, (0.35f, 0.65f)),
                    },
                    MSC = false,
                    MinFloats = 46, // 73 if TailTuft/GeckoScales readded; 12 (ivars) + 5 (legs & head) + 6 (tail segs) + 7 (tongue segs) + 14 (WingScales) + 27 (SKIPPED; TailTuft) + 2
                    FloatRanges = null, IntRanges = null,
                    Apply = (i, p, s, _, _) =>
                    {
                        int j = i[9] > 0.5f ? 12 : 11; // accounts for generic i vars + black salamander chance

                        // Deal with stupid body parts and stuff
                        j += 5; // accounts for legs and head
                        j += LizardUtil.NumTailSegments(LizardType.Cyan); // 6
                        j += LizardUtil.NumTongueSegments(LizardType.Cyan); // 7

                        float tailColor = i[9] > 0.5f ? i[10] : 0;
                        
                        bool hasWingScales = i[j++] < 0.75f;
                        if (hasWingScales) LizardUtil.WingScalesVars(i, ref j, out _, out _);

                        bool hasTailTuft = i[j++] < 0.5f && tailColor == 0;
                        // if (hasTailTuft) LizardUtil.TailTuftVars(i, ref j, s, LizardType.Cyan, out _, out _);
                        // else LizardUtil.TailGeckoSpritesVars(i, ref j, s, out _, out _);

                        return new float[] {
                            hasWingScales ? 1 : 0,
                            hasTailTuft ? 1 : 0,
                            // Hue
                            WrappedRandomVariation(0.49f, 0.04f, 0.6f, i[0], i[1]),
                            // Lightness
                            ClampedRandomVariation(0.5f, 0.15f, 0.1f, i[2], i[3]),
                        };
                    }
                }
            },
            {
                "Caramel Lizard Variations",
                new Setup
                {
                    Inputs = new SearchInput[] {
                        new("Body pattern (see description on hover)", "1: BodyStripes, 2: BumpHawk, 3: LongShoulderScales, 4: ShortBodyScales, 5: SpineSpikes", InputType.MultiChoice, (1, 5)),
                        new("Has TailTuft?", InputType.Boolean),
                        new("Has LongHeadScales?", InputType.Boolean),
                        // Whitespace,
                        new("Effect Color (H)", InputType.Hue, (0.07f, 0.13f)),
                        new("Effect Color (L)", InputType.Float, (0.19f, 0.91f)),
                        Whitespace,
                        new("Body Color (H)", InputType.Hue, (0.075f, 0.125f)),
                        new("Body Color (S)", InputType.Float, (0.3f, 0.9f)),
                        new("Body Color (L)", InputType.Float, (0.7f, 1f)),
                    },
                    MSC = true,
                    MinFloats = 124, // 12 (ivars) + 7 (legs & head) + 4 (tail segs) + 43 (LongShoulderScales) + 27 (TailTufts) + 9 (LongHeadScales) + 22
                    FloatRanges = null, IntRanges = null,
                    Apply = (i, p, s, _, _) =>
                    {
                        int j = i[9] > 0.5f ? 12 : 11; // accounts for generic i vars + black salamander chance

                        // Deal with stupid body parts and stuff
                        j += 7; // accounts for legs and head
                        j += LizardUtil.NumTailSegments(LizardType.Caramel); // 4
                        
                        // 1: BodyStripes, 2: BumpHawk, 3: LongShoulderScales, 4: ShortBodyScales, 5: SpineSpikes
                        int bodyDecoration = 2;

                        if (i[j++] < 0.6f)
                        {
                            LizardUtil.BodyStripesVars(i, ref j, LizardType.Caramel, out _);
                            bodyDecoration = 1;
                        }
                        else if (i[j++] < 0.06666667f)
                        {
                            LizardUtil.SpineSpikesVars(i, ref j, s, LizardType.Caramel, out _, out _);
                            bodyDecoration = 5;
                        }
                        else
                        {
                            j += 3;
                            if (i[j++] < 0.04761905f)
                            {
                                LizardUtil.LongShoulderScalesVars(i, ref j, s, LizardType.Caramel, out _, out _, out _);
                                bodyDecoration = 3;
                            }
                            else if (i[j++] < 0.0625f)
                            {
                                LizardUtil.ShortBodyScalesVars(i, ref j, s, LizardType.Caramel, out _, out _);
                                bodyDecoration = 4;
                            }
                        }

                        bool hasTailTuft = false;
                        if (i[j++] < 0.5f || i[j++] < 0.11111111f || (bodyDecoration == 2 && i[j++] < 0.7f))
                        {
                            LizardUtil.TailTuftVars(i, ref j, s, LizardType.Caramel, out _, out _);
                            hasTailTuft = true;
                        }

                        bool hasLHS = false;
                        if (i[j++] < (bodyDecoration == 2 ? 0.7f : 0.1f) && ((bodyDecoration != 3 && i[j++] < 0.9f) || i[j++] < 0.033333335f))
                        {
                            LizardUtil.LongHeadScalesVars(i, ref j, LizardType.Caramel, out _, out _, out _, out _);
                            hasLHS = true;
                        }

                        if (bodyDecoration == 2)
                        {
                            LizardUtil.BumpHawkVars(i, ref j, LizardType.Caramel, out _, out _, out _);
                            // bodyDecoration stays 2
                        }

                        j += 5;
                        float effectH = WrappedRandomVariation(0.1f, 0.03f, 0.2f, i[0], i[1]);
                        float effectL = ClampedRandomVariation(0.55f, 0.36f, 0.2f, i[2], i[3]);
                        float bodyH;
                        float bodyS;
                        float bodyL = SearchUtil.GetRangeAt(s, new float[] { 0.7f, 1f }, j++);

                        if (bodyL >= 0.8f)
                        {
                            bodyH = SearchUtil.GetRangeAt(s, new float[] { 0.075f, 0.125f }, j++);
                            bodyS = SearchUtil.GetRangeAt(s, new float[] { 0.4f, 0.9f }, j++);
                            effectH = WrappedRandomVariation(0.1f, 0.03f, 0.2f, i[j++], i[j++]);
                            effectL = ClampedRandomVariation(0.55f, 0.05f, 0.2f, i[j++], i[j++]);
                        }
                        else
                        {
                            bodyH = SearchUtil.GetRangeAt(s, new float[] { 0.075f, 0.125f }, j++);
                            bodyS = SearchUtil.GetRangeAt(s, new float[] { 0.3f, 0.5f }, j++);
                        }

                        // Convert to and from because of floating point errors causing tests to fail
                        Vector3 reconverted = Custom.RGB2HSL(Custom.HSL2RGB(bodyH, bodyS, bodyL));
                        bodyH = reconverted.x; bodyS = reconverted.y; bodyL = reconverted.z;

                        return new float[] {
                            bodyDecoration,
                            hasTailTuft ? 1 : 0,
                            hasLHS ? 1 : 0,
                            effectH,
                            effectL,
                            bodyH,
                            bodyS,
                            bodyL
                        };
                    }
                }
            },
            {
                "Zoop Lizard Variations",
                new Setup
                {
                    Inputs = new SearchInput[] {
                        new("Body pattern (see description on hover)", "1: N/A, 2: BumpHawk, 3: LongShoulderScales, 4: ShortBodyScales, 5: SpineSpikes", InputType.MultiChoice, (1, 5)),
                        new("Has WingScales (1) or SpineSpikes (2)?", "The SpineSpikes here are unerelated to the body pattern spine spikes", InputType.MultiChoice, (0, 1)),
                        new("Has extra TailTuft?", InputType.Boolean),
                        new("Has LongHeadScales?", InputType.Boolean),
                    },
                    MSC = true,
                    MinFloats = 163, // 211 with SnowAccumulation; 12 (ivars) + 5 (legs & head) + 4 (tail segs) + 10 (tongue segs) + 14 (WingScales) + 27 (TailTuft) + 43 (LongShoulderScales) + 27 (TailTufts) + 9 (LongHeadScales) + 48 (SKIPPED; SnowAccumulation) + 12
                    FloatRanges = null, IntRanges = null,
                    Apply = (i, p, s, _, _) =>
                    {
                        int j = i[9] > 0.5f ? 12 : 11; // accounts for generic i vars + black salamander chance

                        // Deal with stupid body parts and stuff
                        j += 5; // accounts for legs and head
                        j += LizardUtil.NumTailSegments(LizardType.Zoop); // 4
                        j += LizardUtil.NumTongueSegments(LizardType.Zoop); // 10

                        bool hasWingScales = i[j++] < 0.175f;
                        if (hasWingScales)
                        {
                            LizardUtil.WingScalesVars(i, ref j, out _, out _);
                        }
                        else
                        {
                            LizardUtil.SpineSpikesVars(i, ref j, s, LizardType.Zoop, out _, out _);
                        }
                        LizardUtil.TailTuftVars(i, ref j, s, LizardType.Zoop, out _, out _);
                        
                        // 1: N/A, 2: BumpHawk, 3: LongShoulderScales, 4: ShortBodyScales, 5: SpineSpikes
                        int bodyDecoration = 1;

                        if (i[j++] < 0.06666667f)
                        {
                            LizardUtil.SpineSpikesVars(i, ref j, s, LizardType.Zoop, out _, out _);
                            bodyDecoration = 5;
                        }
                        else
                        {
                            j += 2;
                            if (i[j++] < 0.033333335f)
                            {
                                LizardUtil.BumpHawkVars(i, ref j, LizardType.Zoop, out _, out _, out _);
                                bodyDecoration = 2;
                            }
                            else if (i[j++] < 0.04761905f)
                            {
                                LizardUtil.LongShoulderScalesVars(i, ref j, s, LizardType.Zoop, out _, out _, out _);
                                bodyDecoration = 3;
                            }
                            else if (i[j++] < 0.0625f)
                            {
                                LizardUtil.ShortBodyScalesVars(i, ref j, s, LizardType.Zoop, out _, out _);
                                bodyDecoration = 4;
                            }
                        }

                        bool hasTailTuft = false;
                        if (i[j++] < 0.11111111f || (bodyDecoration == 1 && i[j++] < 0.7f))
                        {
                            LizardUtil.TailTuftVars(i, ref j, s, LizardType.Zoop, out _, out _);
                            hasTailTuft = true;
                        }

                        bool hasLHS = false;
                        if (i[j++] < (bodyDecoration == 1 ? 0.7f : 0.1f) && ((bodyDecoration != 3 && i[j++] < 0.9f) || i[j++] < 0.033333335f))
                        {
                            LizardUtil.LongHeadScalesVars(i, ref j, LizardType.Zoop, out _, out _, out _, out _);
                            hasLHS = true;
                        }

                        return new float[] {
                            bodyDecoration,
                            hasWingScales ? 0 : 1,
                            hasTailTuft ? 1 : 0,
                            hasLHS ? 1 : 0,
                            i[1] < 0.175f ? 1f : 2f
                        };
                    }
                }
            },
            {
                "Train Lizard Variations",
                new Setup
                {
                    Inputs = new SearchInput[] {
                        new("Body pattern (see description on hover)", "1: N/A, 2: BumpHawk, 3: extra LongShoulderScales, 4: ShortBodyScales, 5: extra SpineSpikes", InputType.MultiChoice, (1, 5)),
                        new("Main tail pattern (see description on hover)", "1: TailTuft, 2: TailFin", InputType.MultiChoice, (1, 2)),
                        new("Additional TailTuft?", "Separate from choice above", InputType.Boolean),
                        new("Has LongHeadScales?", InputType.Boolean),
                    },
                    MSC = true,
                    MinFloats = 182,
                    FloatRanges = null, IntRanges = null,
                    Apply = (i, p, s, _, _) =>
                    {
                        int j = i[9] > 0.5f ? 12 : 11; // accounts for generic i vars + black salamander chance

                        // Deal with stupid body parts and stuff
                        j += 5; // accounts for legs and head
                        j += LizardUtil.NumTailSegments(LizardType.Train); // 15
                        j += LizardUtil.NumTongueSegments(LizardType.Train); // 10
                        
                        // 1: N/A, 2: BumpHawk, 3: LongShoulderScales, 4: ShortBodyScales, 5: SpineSpikes
                        int bodyDecoration = 1;

                        if (i[j++] < 0.06666667f)
                        {
                            LizardUtil.SpineSpikesVars(i, ref j, s, LizardType.Train, out _, out _);
                            bodyDecoration = 5;
                        }
                        else
                        {
                            j += 2;
                            if (i[j++] < 0.033333335f)
                            {
                                LizardUtil.BumpHawkVars(i, ref j, LizardType.Train, out _, out _, out _);
                                bodyDecoration = 2;
                            }
                            else if (i[j++] < 0.04761905f)
                            {
                                LizardUtil.LongShoulderScalesVars(i, ref j, s, LizardType.Train, out _, out _, out _);
                                bodyDecoration = 3;
                            }
                            else if (i[j++] < 0.0625f)
                            {
                                LizardUtil.ShortBodyScalesVars(i, ref j, s, LizardType.Train, out _, out _);
                                bodyDecoration = 4;
                            }
                        }

                        bool hasExtraTailTuft = false;
                        if (i[j++] < 0.11111111f || (bodyDecoration == 1 && i[j++] < 0.7f))
                        {
                            LizardUtil.TailTuftVars(i, ref j, s, LizardType.Train, out _, out _);
                            hasExtraTailTuft = true;
                        }

                        bool hasLHS = false;
                        if (i[j++] < (bodyDecoration == 1 ? 0.7f : 0.1f) && ((bodyDecoration != 3 && i[j++] < 0.9f) || i[j++] < 0.033333335f))
                        {
                            LizardUtil.LongHeadScalesVars(i, ref j, LizardType.Train, out _, out _, out _, out _);
                            hasLHS = true;
                        }

                        LizardUtil.LongShoulderScalesVars(i, ref j, s, LizardType.Train, out _, out _, out _);
                        LizardUtil.SpineSpikesVars(i, ref j, s, LizardType.Train, out _, out _);

                        bool hasTailFin = i[j++] < 0.5f;
                        
                        return new float[] {
                            bodyDecoration,
                            hasTailFin ? 2 : 1,
                            hasExtraTailTuft ? 1 : 0,
                            hasLHS ? 1 : 0,
                        };
                    }
                }
            },
            {
                "Eel Lizard Variations",
                new Setup
                {
                    Inputs = new SearchInput[] {
                        new("Body pattern 1 (see description on hover)", "1: LongShoulderScales and TailFin, 2: ShortBodyScales and TailFin, 3: ShortBodyScales and TailTuft", InputType.MultiChoice, (1, 3)),
                        new("Body pattern 2 (see description on hover)", "1: N/A, 2: BumpHawk, 3: extra LongShoulderScales, 4: ShortBodyScales, 5: extra SpineSpikes", InputType.MultiChoice, (1, 5)),
                        new("Has TailTuft?", "Separate from body pattern 1", InputType.Boolean),
                        new("Has LongHeadScales?", InputType.Boolean),
                    },
                    MSC = true,
                    MinFloats = 208, // 12 (ivars) + 5 (legs & head) + 35 (AxolotlGills) + 9 (TailGeckoSprites) + 57 (bodyPattern1 = 1) + 43 (LongShoulderScales) + 27 (TailTufts) + 9 (LongHeadScales) + 11
                    FloatRanges = null, IntRanges = null,
                    Apply = (i, p, s, _, _) =>
                    {
                        int j = i[9] > 0.5f ? 12 : 11; // accounts for generic i vars + black salamander chance

                        // Deal with stupid body parts and stuff
                        j += 5; // accounts for legs and head (yeah eels only have two legs but they have 4 in the code)
                        j += LizardUtil.NumTailSegments(LizardType.Eel); // 16

                        LizardUtil.AxolotlGillsVars(i, ref j, s, out _, out _);
                        LizardUtil.TailGeckoSpritesVars(i, ref j, s, out _, out _);

                        int bodyPattern1 = i[j++] < 0.75f ? 1 : 0;
                        if (bodyPattern1 == 1)
                        {
                            LizardUtil.LongShoulderScalesVars(i, ref j, s, LizardType.Eel, out _, out _, out _);
                            LizardUtil.TailFinVars(i, ref j, s, LizardType.Eel, out _, out _, out _, out _, out _);
                        }
                        else
                        {
                            LizardUtil.ShortBodyScalesVars(i, ref j, s, LizardType.Eel, out _, out _);
                            if (i[j++] < 0.75f)
                            {
                                bodyPattern1 = 2;
                                LizardUtil.TailFinVars(i, ref j, s, LizardType.Eel, out _, out _, out _, out _, out _);
                            }
                            else
                            {
                                bodyPattern1 = 3;
                                LizardUtil.TailTuftVars(i, ref j, s, LizardType.Eel, out _, out _);
                            }
                        }
                        
                        // 1: N/A, 2: BumpHawk, 3: LongShoulderScales, 4: ShortBodyScales, 5: SpineSpikes
                        int bodyDecoration2 = 1;

                        if (i[j++] < 0.06666667f)
                        {
                            LizardUtil.SpineSpikesVars(i, ref j, s, LizardType.Eel, out _, out _);
                            bodyDecoration2 = 5;
                        }
                        else
                        {
                            j += 2;
                            if (i[j++] < 0.033333335f)
                            {
                                LizardUtil.BumpHawkVars(i, ref j, LizardType.Eel, out _, out _, out _);
                                bodyDecoration2 = 2;
                            }
                            else if (i[j++] < 0.04761905f)
                            {
                                LizardUtil.LongShoulderScalesVars(i, ref j, s, LizardType.Eel, out _, out _, out _);
                                bodyDecoration2 = 3;
                            }
                            else if (i[j++] < 0.0625f)
                            {
                                LizardUtil.ShortBodyScalesVars(i, ref j, s, LizardType.Eel, out _, out _);
                                bodyDecoration2 = 4;
                            }
                        }

                        bool hasExtraTailTuft = false;
                        if (i[j++] < 0.11111111f || (bodyDecoration2 == 1 && i[j++] < 0.7f))
                        {
                            LizardUtil.TailTuftVars(i, ref j, s, LizardType.Eel, out _, out _);
                            hasExtraTailTuft = true;
                        }

                        bool hasLHS = false;
                        if (i[j++] < (bodyDecoration2 == 1 ? 0.7f : 0.1f) && ((bodyDecoration2 != 3 && i[j++] < 0.9f) || i[j++] < 0.033333335f))
                        {
                            LizardUtil.LongHeadScalesVars(i, ref j, LizardType.Eel, out _, out _, out _, out _);
                            hasLHS = true;
                        }

                        return new float[] {
                            bodyPattern1,
                            bodyDecoration2,
                            hasExtraTailTuft ? 1 : 0,
                            hasLHS ? 1 : 0
                        };
                    }
                }
            },

            // Next thingy here
            // {},
        };
    }
}
