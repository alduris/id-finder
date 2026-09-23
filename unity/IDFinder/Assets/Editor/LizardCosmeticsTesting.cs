using System;
using System.Collections.Generic;
using UnityEngine;
using Input = ComputeTester.Input;

public static class LizardCosmeticsTesting
{
    public static IEnumerable<Input> AntennaeVars()
    {
        yield return new Input("Has Antennae", 0, 1, 1) { enabled = false };
        yield return new Input("    Antennae length") { enabled = false };
        yield return new Input("    Antennae alpha") { enabled = false };
    }

    public static IEnumerable<Input> AxolotlGillsVars()
    {
        yield return new Input("Has AxolotlGills", 0, 1, 1) { enabled = false };
        yield return new Input("    AxolotlGills rigor") { enabled = false };
        yield return new Input("    AxolotlGills size fac") { enabled = false };
        yield return new Input("    AxolotlGills graphic", 0, 5, 1) { enabled = false };
        yield return new Input("    AxolotlGills num gills", 2, 7, 1) { enabled = false };
        yield return new Input("    AxolotlGills width fac") { enabled = false };
        yield return new Input("    AxolotlGills backwards fac", 0.1f, 0.9f) { enabled = false };
    }

    public static IEnumerable<Input> BodyStripesVars(LizardType type)
    {
        yield return new Input("Has BodyStripes", 0, 1, 1) { enabled = false };
        int maxScales = (int)((0.8f * GetMaxBodyAndTailLength(type)) / (5f * 1.5f));
        yield return new Input("    BodyStripes num stripes", 3, maxScales, 1) { enabled = false };
    }

    public static IEnumerable<Input> BumpHawkVars(LizardType type)
    {
        yield return new Input("Has BumpHawk", 0, 1, 1) { enabled = false };
        yield return new Input("    BumpHawk colored", 0, 1, 1) { enabled = false };
        float minSpineLength = 0.3f * GetMinBodyAndTailLength(type);
        float maxSpineLength = 0.9f * GetMaxBodyAndTailLength(type);
        yield return new Input("    BumpHawk spine length", minSpineLength, maxSpineLength) { enabled = false };
        int minBumps = (int)(minSpineLength / 12f);
        int maxBumps = (int)(0.7f * GetMaxBodyAndTailLength(type) / 3f);
        yield return new Input("    BumpHawk num bumps", minBumps, maxBumps, 1) { enabled = false };
    }

    public static IEnumerable<Input> LongHeadScalesVars()
    {
        yield return new Input("Has LongHeadScales", 0, 1, 1) { enabled = false };
        yield return new Input("    LongHeadScales rigor") { enabled = false };
        yield return new Input("    LongHeadScales colored", 0, 1, 1) { enabled = false };
        yield return new Input("    LongHeadScales graphic", 4, 6, 1) { enabled = false };
        yield return new Input("    LongHeadScales length", 5, 35) { enabled = false };
        yield return new Input("    LongHeadScales width", 0.65f, 1.2f) { enabled = false };
    }

    public static IEnumerable<Input> LongShoulderScalesVars(LizardType type)
    {
        yield return new Input("Has LongShoulderScales", 0, 1, 1) { enabled = false };
        yield return new Input("    LongShoulderScales scale type", 0, 2, 1) { enabled = false };
        int minNumScales = Math.Min(4, Math.Min(MinGenTwoLines(type, 0.07f, 3f), MinGenSegments(type, 0.1f)));
        int maxNumScales = Math.Max(14, Math.Max(MaxGenTwoLines(type, 0.07f, 1f, 3f), MaxGenSegments(type, 0.1f, 0.8f)));
        yield return new Input("    LongShoulderScales num scales", minNumScales, maxNumScales, 1) { enabled = false };
        yield return new Input("    LongShoulderScales min size", 2.5f, 15f) { enabled = false };
        yield return new Input("    LongShoulderScales max size", 2.5f, 35f) { enabled = false };
        yield return new Input("    LongShoulderScales colored", 0, 1, 1) { enabled = false };
        yield return new Input("    LongShoulderScales graphic", 0, 6, 1) { enabled = false };
    }

    public static IEnumerable<Input> PeachBackFinVars()
    {
        yield return new Input("Has PeachBackFin", 0, 1, 1) { enabled = false };
        yield return new Input("    PeachBackFin min size", 0.27f, 0.35f) { enabled = false };
        yield return new Input("    PeachBackFin max size", 0.27f, 0.7f) { enabled = false };
        yield return new Input("    PeachBackFin size skew exp", 0.5f, 1.5f) { enabled = false };
        yield return new Input("    PeachBackFin graphic", 4, 5) { enabled = false };
        yield return new Input("    PeachBackFin num bumps", 3, 4, 1) { enabled = false };
        yield return new Input("    PeachBackFin scale x", 1.5f, 2.5f) { enabled = false };
    }

    public static IEnumerable<Input> PeachHeadStripesVars()
    {
        yield return new Input("Has PeachHeadStripes", 0, 1, 1) { enabled = false };
        yield return new Input("    PeachHeadStripes alpha") { enabled = false };
    }

    public static IEnumerable<Input> ShortBodyScalesVars(LizardType type)
    {
        yield return new Input("Has ShortBodyScales", 0, 1, 1) { enabled = false };
        yield return new Input("    ShortBodyScales scale type", 0, 2, 1) { enabled = false };
        int minNumScales = Math.Min(4, Math.Min(MinGenTwoLines(type, 0.1f, 1f), MinGenSegments(type, 0.1f)));
        int maxNumScales = Math.Max(14, Math.Max(MaxGenTwoLines(type, 0.1f, 1f, 1f), MaxGenSegments(type, 0.1f, 0.9f)));
        yield return new Input("    ShortBodyScales num scales", minNumScales, maxNumScales, 1) { enabled = false };
    }

    public static IEnumerable<Input> SkinkSpecklesVars()
    {
        yield return new Input("Has SkinkSpeckles", 0, 1, 1) { enabled = false };
        yield return new Input("    SkinkSpeckles num spots", 0, 49, 1) { enabled = false };
    }

    public static IEnumerable<Input> SpineSpikesVars(LizardType type)
    {
        yield return new Input("Has SpineSpikes", 0, 1, 1) { enabled = false };
        float minSpineLen = 0.2f * GetMinBodyAndTailLength(type);
        float maxSpineLen = 0.95f * GetMaxBodyAndTailLength(type);
        yield return new Input("    SpineSpikes length", minSpineLen, maxSpineLen) { enabled = false };
        int minSpikes = (int)(minSpineLen / 8f);
        int maxSpikes = (int)(maxSpineLen / 5f);
        yield return new Input("    SpineSpikes num spikes", minSpikes, maxSpikes, 1) { enabled = false };
        yield return new Input("    SpineSpikes flipped", 0, 1, 1) { enabled = false };
        yield return new Input("    SpineSpikes graphic", 0, 6, 1) { enabled = false };
        yield return new Input("    SpineSpikes color mode", 0, 2, 1) { enabled = false };
    }

    public static IEnumerable<Input> TailFinVars(LizardType type)
    {
        yield return new Input("Has TailFin", 0, 1, 1) { enabled = false };
        float minSpineLen = (type == LizardType.Red ? 0.3f - 0.17f : 0.5f - 0.17f) * GetMinBodyAndTailLength(type);
        float maxSpineLen = (type == LizardType.Red ? 0.3f + 0.17f : 0.5f + 0.17f) * GetMaxBodyAndTailLength(type);
        yield return new Input("    TailFin spine len", minSpineLen, maxSpineLen) { enabled = false };
        yield return new Input("    TailFin  underside size", 0.3f, 0.9f) { enabled = false };
        yield return new Input("    TailFin graphic", 0, 5, 1) { enabled = false };
        int minScales = (int)(minSpineLen / 7f);
        int maxScales = (int)(maxSpineLen / 4f);
        yield return new Input("    TailFin num scales", minScales, maxScales, 1) { enabled = false };
        yield return new Input("    TailFin scale x", 1, 2) { enabled = false };
        yield return new Input("    TailFin colored", 0, 1, 1) { enabled = false };
    }

    public static IEnumerable<Input> TailGeckoScalesVars()
    {
        yield return new Input("Has TailGeckoScales", 0, 1, 1) { enabled = false };
        yield return new Input("    TailGeckoScales big", 0, 1, 1) { enabled = false };
        yield return new Input("    TailGeckoScales rows", 7, 18, 1) { enabled = false };
        yield return new Input("    TailGeckoScales lines", 3, 4, 1) { enabled = false };
    }

    public static IEnumerable<Input> TailTuftVars(LizardType type)
    {
        yield return new Input("Has TailTuft", 0, 1, 1) { enabled = false };
        yield return new Input("    TailTuft scale type", 0, 1, 1) { enabled = false }; // no segments, so no 2
        int minScales = (type == LizardType.Blue || type == LizardType.Red) ? MinGenTwoLines(type, 0f, 3f) : MinGenTwoLines(type, 0f, 1.3f);
        int maxScales = Math.Max((type == LizardType.Blue || type == LizardType.Red) ? MaxGenTwoLines(type, 0f, type == LizardType.Red ? 0.3f : 0.7f, 3f) : MaxGenTwoLines(type, 0f, 0.4f, 1.3f), 7);
        yield return new Input("    TailTuft num scales", minScales, maxScales, 1) { enabled = false };
        yield return new Input("    TailTuft colored", 0, 1, 1) { enabled = false };
        yield return new Input("    TailTuft graphic", 0, 6, 1) { enabled = false };
    }

    public static IEnumerable<Input> WhiskersVars()
    {
        yield return new Input("Has Whiskers", 0, 1, 1) { enabled = false };
        yield return new Input("    Whiskers num", 3, 4, 1) { enabled = false };
    }

    public static IEnumerable<Input> WingScalesVars()
    {
        yield return new Input("Has WingScales", 0, 1, 1) { enabled = false };
        yield return new Input("    WingScales num", 2, 3, 1) { enabled = false };
        yield return new Input("    WingScales graphic", 0, 4, 1) { enabled = false };
        yield return new Input("    WingScales length", 5, 40) { enabled = false };
        yield return new Input("    WingScales front dir", -0.1f, 0.2f) { enabled = false };
        yield return new Input("    WingScales back dir", 0f, 0.8f) { enabled = false };
    }

    public static IEnumerable<Input> Melanistic()
    {
        yield return new Input("Is melanistic", 0, 1, 1) { enabled = false };
    }

    #region calculations

    public enum LizardType
    {
        Pink,
        Green,
        Blue,
        Yellow,
        White,
        Red,
        Black,
        Salamander,
        Cyan,
        Caramel,
        Zoop,
        Train,
        Eel,
        Blizzard,
        Basilisk,
        Indigo,
        Peach
    }
    public static int NumTailSegments(LizardType type)
    {
        return type switch
        {
            LizardType.Pink => 5,
            LizardType.Green => 7,
            LizardType.Blue => 4,
            LizardType.Yellow => 5,
            LizardType.White => 5,
            LizardType.Red => 11,
            LizardType.Black => 6,
            LizardType.Salamander => 5,
            LizardType.Cyan => 6,
            LizardType.Caramel => 4,
            LizardType.Zoop => 4,
            LizardType.Train => 15,
            LizardType.Eel => 16,
            LizardType.Blizzard => 6,
            LizardType.Basilisk => 5,
            LizardType.Indigo => 7,
            LizardType.Peach => 5,
            _ => throw new NotImplementedException()
        };
    }

    public static int NumTongueSegments(LizardType type, bool cfgAlphaRedLizards)
    {
        return type switch
        {
            LizardType.Blue => 5,
            LizardType.White => 10,
            LizardType.Red => cfgAlphaRedLizards ? 10 : 0,
            LizardType.Salamander => 7,
            LizardType.Cyan => 7,
            LizardType.Zoop => 10,
            LizardType.Train => 10,
            LizardType.Indigo => 7,
            LizardType.Peach => 7,
            _ => 0
        };
    }
    private static float GetParamHeadSize(LizardType type)
    {
        return type switch
        {
            LizardType.Blue => 0.9f,
            LizardType.Red => 1.2f,
            LizardType.Salamander => 0.9f,
            LizardType.Cyan => 0.95f,
            LizardType.Caramel => 1.2f,
            LizardType.Zoop => 0.9f,
            LizardType.Train => 1.4f,
            LizardType.Eel => 0.95f,
            LizardType.Blizzard => 1.08f,
            LizardType.Basilisk => 0.8f,
            LizardType.Indigo => 0.72f,
            _ => 1f
        };
    }
    private static float GetBodyAndTailLength(LizardType type, float tailLengthIVar)
    {
        float bodyLenFacParam = 1f; // always 1
        float bodySizeFacParam = type switch
        {
            LizardType.Pink => 1f,
            LizardType.Green => 1.2f,
            LizardType.Blue => 0.9f,
            LizardType.Yellow => 0.93f,
            LizardType.White => 1f,
            LizardType.Red => 1.2f,
            LizardType.Black => 0.9f,
            LizardType.Salamander => 0.9f,
            LizardType.Cyan => 1f,
            LizardType.Caramel => 1.75f,
            LizardType.Zoop => 0.74f,
            LizardType.Train => 1.4f,
            LizardType.Eel => 0.95f,
            LizardType.Blizzard => 1.4f,
            LizardType.Basilisk => 1.1f,
            LizardType.Indigo => 1.2f,
            LizardType.Peach => 0.65f,
            _ => throw new NotImplementedException()
        };
        float bodyStiffnessParam = type switch
        {
            LizardType.Pink => 0.2f,
            LizardType.Green => 0.5f,
            LizardType.Blue => 0f,
            LizardType.Yellow => 0.2f,
            LizardType.White => 0.15f,
            LizardType.Red => 0.3f,
            LizardType.Black => 0.25f,
            LizardType.Salamander => 0.2f,
            LizardType.Cyan => 0f,
            LizardType.Caramel => 0.2f,
            LizardType.Zoop => 0.32f,
            LizardType.Train => 0.3f,
            LizardType.Eel => 0.7f,
            LizardType.Blizzard => 0.55f,
            LizardType.Basilisk => 0.2f,
            LizardType.Indigo => 0.5f,
            LizardType.Peach => 0.8f,
            _ => throw new NotImplementedException()
        };
        float tailLengthFactorParam = type switch
        {
            LizardType.Pink => 1.2f,
            LizardType.Green => 0.9f,
            LizardType.Blue => 1f,
            LizardType.Yellow => 1.2f,
            LizardType.White => 1.2f,
            LizardType.Red => 1.9f,
            LizardType.Black => 1.2f,
            LizardType.Salamander => 1.2f,
            LizardType.Cyan => 1.8f,
            LizardType.Caramel => 1.2f,
            LizardType.Zoop => 1.8f,
            LizardType.Train => 2.5f,
            LizardType.Eel => 1.1f,
            LizardType.Blizzard => 0.3f,
            LizardType.Basilisk => 0.6f,
            LizardType.Indigo => 0.9f,
            LizardType.Peach => 1.44f,
            _ => throw new NotImplementedException(),
        };


        float headSize = GetParamHeadSize(type);

        float bodyDistance = 17f * bodyLenFacParam * ((bodySizeFacParam + 1f) / 2f);
        float bodyLength = 5.5f * headSize + 2 * bodyDistance + bodyDistance * (1f + bodyStiffnessParam);

        int tailSegments = NumTailSegments(type);
        float tailLength = 0f;
        for (int i = 0; i < tailSegments; i++)
        {
            float num2 = 8f * bodySizeFacParam;
            num2 *= (tailSegments - i) / (float)tailSegments;
            float num3 = ((i > 0 ? 8f : 16f) + num2) / 2f;
            num3 *= tailLengthFactorParam * tailLengthIVar;
            tailLength += num3;
        }

        return bodyLength + tailLength;
    }

    private static float GetMinBodyAndTailLength(LizardType type) => GetBodyAndTailLength(type, 0.6f);
    private static float GetMaxBodyAndTailLength(LizardType type) => GetBodyAndTailLength(type, 1.4f);

    private static int MinGenTwoLines(LizardType type, float startPoint, float spacingScale)
    {
        float totLength = (startPoint + 0.1f) * GetMinBodyAndTailLength(type);
        float spacing = (type == LizardType.Blue ? 2f : 9f) * spacingScale;
        int numOfScales = (int)(totLength / spacing);
        if (numOfScales < 3) numOfScales = 3;
        return numOfScales;
    }
    private static int MaxGenTwoLines(LizardType type, float startPoint, float maxLength, float spacingScale)
    {
        float totLength = Mathf.Max(startPoint + 0.2f, maxLength) * GetMaxBodyAndTailLength(type);
        float spacing = 2f * spacingScale;
        int numOfScales = (int)(totLength / spacing);
        if (numOfScales < 3) numOfScales = 3;
        return numOfScales;
    }
    private static int MinGenSegments(LizardType type, float startPoint)
    {
        float totLength = (startPoint + 0.1f) * GetMinBodyAndTailLength(type);
        float spacing = (type == LizardType.Red ? 11f * 0.75f : 14f);
        int numOfSegments = Math.Max(3, (int)(totLength / spacing));
        return numOfSegments * 2;
    }
    private static int MaxGenSegments(LizardType type, float startPoint, float maxLength)
    {
        float totLength = Mathf.Max(startPoint + 0.2f, maxLength) * GetMaxBodyAndTailLength(type);
        float spacing = (type == LizardType.Red ? 7f * 0.75f : 7f);
        int numOfSegments = Math.Max(3, (int)(totLength / spacing));
        return numOfSegments * 6;
    }

    #endregion
}
