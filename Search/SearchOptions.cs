using System;
using System.Collections.Generic;
using System.Linq;
using RWCustom;
using UnityEngine;
using LizardType = FinderMod.Search.LizardUtil.LizardType;
using LizardBodyScaleType = FinderMod.Search.LizardUtil.LizardBodyScaleType;
using ScavBodyScalePattern = FinderMod.Search.ScavUtil.ScavBodyScalePattern;
using FinderMod.Search.Options;
using FinderMod.Search.Options.Scavs;
using Random = UnityEngine.Random;

namespace FinderMod.Search
{
    public struct Personality
    {
        public float Aggression;
        public float Bravery;
        public float Dominance;
        public float Energy;
        public float Nervous;
        public float Sympathy;
    }

    public struct SearchData
    {
        public int Seed;
        public Personality Personality;
        public Random.State[] States;
    }

    public struct Setup {
        public SearchInput[] Inputs;
        public bool MSC;
        public int MinFloats;
        public float[,] FloatRanges;
        public int[,] IntRanges;
        // Parameters: i(nput), p(ersonality), s(eed), f(loat) r(ange start), i(nteger) r(ange start); output: floats mirroring Inputs
        // Parameter i is a bunch of concatenated arrays
        public Func<float[], Personality, int, int, int, float[]> Apply;
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

        internal static readonly Dictionary<string, IOption> Options = new()
        {
            // Perseonality
            { "Personality", new OPersonality() },

            // Scavs
            { "Scavenger Skills", new OScavSkills() },
            { "Elite Scavenger Skills", new OEliteScavSkills() },
            { "Scavenger Colors", new OScavColors() },
            { "Elite Scavenger Colors", new OEliteScavColors() },
        };

        /*internal static readonly Dictionary<string, Setup> Groups = new()
        {
            
            // Scavs
            
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
                        /*float wgt = 0.65f; // body weight
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
                                0 => (p.Nervous, p.Energy),
                                1 => (p.Sympathy, p.Aggression),
                                2 => (p.Energy, p.Nervous),
                                3 => (p.Energy, p.Aggression),
                                4 => (p.Dominance, p.Energy),
                                5 => (p.Aggression, p.Sympathy),
                                6 => (p.Dominance, p.Bravery),
                                7 => (p.Sympathy, p.Bravery),
                                8 => (p.Aggression, p.Nervous),
                                9 => (p.Nervous, p.Energy),
                                10 => (p.Bravery, p.Dominance),
                                11 => (p.Bravery, p.Nervous),
                                12 => (p.Bravery, p.Dominance),
                                13 => (p.Energy, p.Aggression),
                                14 => (p.Dominance, p.Bravery),
                                15 => (p.Aggression, p.Sympathy),
                                16 => (p.Nervous, p.Sympathy),
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

                        /*return new float[] {
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
        };*/
    }
}
