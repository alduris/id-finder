// Include file for lizard cosmetic utility

#ifndef IDFINDER_LIZARDUTIL
#define IDFINDER_LIZARDUTIL

#include "../IDFinder.cginc"


inline int NumTailSegments()
{
#if defined(LizardType_Pink)
    return 5;
#elif defined(LizardType_Green)
    return 7;
#elif defined(LizardType_Blue)
    return 4;
#elif defined(LizardType_Yellow)
    return 5;
#elif defined(LizardType_White)
    return 5;
#elif defined(LizardType_Red)
    return 11;
#elif defined(LizardType_Black)
    return 6;
#elif defined(LizardType_Salamander)
    return 5;
#elif defined(LizardType_Cyan)
    return 6;
#elif defined(LizardType_Caramel)
    return 4;
#elif defined(LizardType_Zoop)
    return 4;
#elif defined(LizardType_Train)
    return 15;
#elif defined(LizardType_Eel)
    return 16;
#elif defined(LizardType_Blizzard)
    return 6;
#elif defined(LizardType_Basilisk)
    return 5;
#elif defined(LizardType_Indigo)
    return 7;
#elif defined(LizardType_Peach)
    return 5;
#else
    return 0;
#endif
}

inline int NumTongueSegments()
{
#if defined(LizardType_Blue)
    return 5;
#elif defined(LizardType_White)
    return 10;
#elif defined(LizardType_Red)
    if (_cfgAlphaRedLizards)
    {
        return 10;
    }
    else
    {
        return 0;
    }
#elif defined(LizardType_Salamander)
    return 7;
#elif defined(LizardType_Cyan)
    return 7;
#elif defined(LizardType_Zoop)
    return 10;
#elif defined(LizardType_Train)
    return 10;
#elif defined(LizardType_Indigo)
    return 7;
#elif defined(LizardType_Peach)
    return 7;
#else
    return 0;
#endif
}

inline float GetParamHeadSize()
{
#if defined(LizardType_Blue)
    return 0.9;
#elif defined(LizardType_Red)
    return 1.2;
#elif defined(LizardType_Salamander)
    return 0.9;
#elif defined(LizardType_Cyan)
    return 0.95;
#elif defined(LizardType_Caramel)
    return 1.2;
#elif defined(LizardType_Zoop)
    return 0.9;
#elif defined(LizardType_Train)
    return 1.4;
#elif defined(LizardType_Eel)
    return 0.95;
#elif defined(LizardType_Blizzard)
    return 1.08;
#elif defined(LizardType_Basilisk)
    return 0.8;
#elif defined(LizardType_Indigo)
    return 0.72;
#else
    return 1;
#endif
}

inline float GetBodyAndTailLength(float tailLengthIVar)
{
    float bodyLenFacParam = 1; // always 1
    float bodySizeFacParam, bodyStiffnessParam, tailLengthFactorParam;
    
#if defined(LizardType_Pink)
    bodySizeFacParam = 1;
    bodyStiffnessParam = 0.2;
    tailLengthFactorParam = 1.2;
#elif defined(LizardType_Green)
    bodySizeFacParam = 1.2;
    bodyStiffnessParam = 0.5;
    tailLengthFactorParam = 0.9;
#elif defined(LizardType_Blue)
    bodySizeFacParam = 0.9;
    bodyStiffnessParam = 0;
    tailLengthFactorParam = 1;
#elif defined(LizardType_Yellow)
    bodySizeFacParam = 0.93;
    bodyStiffnessParam = 0.2;
    tailLengthFactorParam = 1.2;
#elif defined(LizardType_White)
    bodySizeFacParam = 1;
    bodyStiffnessParam = 0.15;
    tailLengthFactorParam = 1.2;
#elif defined(LizardType_Red)
    bodySizeFacParam = 1.2;
    bodyStiffnessParam = 0.3;
    tailLengthFactorParam = 1.9;
#elif defined(LizardType_Black)
    bodySizeFacParam = 0.9;
    bodyStiffnessParam = 0.25;
    tailLengthFactorParam = 1.2;
#elif defined(LizardType_Salamander)
    bodySizeFacParam = 0.9;
    bodyStiffnessParam = 0.2;
    tailLengthFactorParam = 1.2;
#elif defined(LizardType_Cyan)
    bodySizeFacParam = 1;
    bodyStiffnessParam = 0;
    tailLengthFactorParam = 1.8;
#elif defined(LizardType_Caramel)
    bodySizeFacParam = 1.75;
    bodyStiffnessParam = 0.2;
    tailLengthFactorParam = 1.2;
#elif defined(LizardType_Zoop)
    bodySizeFacParam = 0.74;
    bodyStiffnessParam = 0.32;
    tailLengthFactorParam = 1.8;
#elif defined(LizardType_Train)
    bodySizeFacParam = 1.4;
    bodyStiffnessParam = 0.3;
    tailLengthFactorParam = 2.5;
#elif defined(LizardType_Eel)
    bodySizeFacParam = 0.95;
    bodyStiffnessParam = 0.7;
    tailLengthFactorParam = 1.1;
#elif defined(LizardType_Blizzard)
    bodySizeFacParam = 1.4;
    bodyStiffnessParam = 0.55;
    tailLengthFactorParam = 0.3;
#elif defined(LizardType_Basilisk)
    bodySizeFacParam = 1.1;
    bodyStiffnessParam = 0.2;
    tailLengthFactorParam = 0.6;
#elif defined(LizardType_Indigo)
    bodySizeFacParam = 1.2;
    bodyStiffnessParam = 0.5;
    tailLengthFactorParam = 0.9;
#elif defined(LizardType_Peach)
    bodySizeFacParam = 0.65;
    bodyStiffnessParam = 0.8;
    tailLengthFactorParam = 1.44;
#endif
    
    float headSize = GetParamHeadSize();

    float bodyDistance = 17 * bodyLenFacParam * ((bodySizeFacParam + 1) / 2);
    float bodyLength = 5.5 * headSize + 2 * bodyDistance + bodyDistance * (1 + bodyStiffnessParam);

    int tailSegments = NumTailSegments();
    float tailLength = 0;
    for (int i = 0; i < tailSegments; i++)
    {
        float num2 = 8 * bodySizeFacParam;
        num2 *= (tailSegments - i) / (float) tailSegments;
        float num3 = ((i > 0 ? 8 : 16) + num2) / 2;
        num3 *= tailLengthFactorParam * tailLengthIVar;
        tailLength += num3;
    }

    return bodyLength + tailLength;
}

void GeneratePatchPattern(inout uint4 random, int numScales, int condition)
{
    ShiftIf(random, 1 + 2 * numScales, condition);
}

int GenerateTwoLines(inout uint4 random, float tailLengthIVar, float startPoint, float maxLength, float lengthExponent, float spacingScale, int condition)
{
    float bodyAndTailLength = GetBodyAndTailLength(tailLengthIVar);
    float linesLength = lerp(startPoint + 0.1, max(startPoint + 0.2, maxLength), pow(RandomValueIf(random, condition), lengthExponent));
    float totLength = linesLength * bodyAndTailLength;
    float spacing = lerp(2, 9, RandomValueIf(random, condition));
#if defined(LizardType_Blue)
    spacing = 2;
#endif
    spacing *= spacingScale;
    int numOfScales = (int) (totLength / spacing);
    if (numOfScales < 3)
    {
        numOfScales = 3;
    }
    return numOfScales; // * 2;
}

int GenerateSegments(inout uint4 random, float tailLengthIVar, float startPoint, float maxLength, float lengthExponent, int condition)
{
    float bodyAndTailLength = GetBodyAndTailLength(tailLengthIVar);
    float linesLength = lerp(startPoint + 0.1, max(startPoint + 0.2, maxLength), pow(RandomValueIf(random, condition), lengthExponent));
    float totLength = linesLength * bodyAndTailLength;
    float spacing = lerp(7, 14, RandomValueIf(random, condition));
#if defined(LizardType_Red)
    spacing = min(spacing, 11) * 0.75;
#endif
    int numOfSegments = max(3, (int) (totLength / spacing));
    int scalesPerSegment = RandomRangeIf(1, 4, random, condition) * 2;
    return numOfSegments * scalesPerSegment;
}

inline void MaybeSetTailTuftGraphic(in int graphic, inout int tailTuftGraphic, int condition)
{
    tailTuftGraphic = (tailTuftGraphic == -1 && condition) ? graphic : tailTuftGraphic;
}

#endif
