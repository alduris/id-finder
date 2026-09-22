// Include file for lizard cosmetic code.
// Cosmetics intentionally not implemented:
//   - LizardRotModule
//   - LizardRotVars

#ifndef IDFINDER_LIZARDS
#define IDFINDER_LIZARDS

#include "../IDFinder.cginc"
#include "../RWCustom.cginc"
#include "LizardUtil.cginc"

#define Inputs StructuredBuffer<Input>
#define nextInput inputs[inputPtr++]
#define MISSING_PENALTY 1000

#define BodyScaleType_Patch 0
#define BodyScaleType_TwoLines 1
#define BodyScaleType_Segments 2

#define SpineSpikes_ColorMode_None 0
#define SpineSpikes_ColorMode_Full 1
#define SpineSpikes_ColorMode_Gradient 2

#define RotType_None 0
#define RotType_Slight 1
#define RotType_Opossum 2
#define RotType_Full 3


void AntennaeVars(inout float d, Inputs inputs, inout int inputPtr, inout uint4 random, int condition)
{
    // Check that it is here at all
    d += MatchDistance(condition, nextInput) * MISSING_PENALTY;
    
    // Get values
    float length = RandomValueIf(random, condition);
    d += Distance(length, nextInput) * condition;
    float alpha = length * 0.9 + RandomValueIf(random, condition) * 0.1;
    d += Distance(alpha, nextInput) * condition;
    
    // Offset for next
    int segments = (int) floor(lerp(3, 8, pow(length, lerp(1, 6, length))));
    ShiftIf(random, segments, condition);
}

void AxolotlGillsVars(inout float d, Inputs inputs, inout int inputPtr, inout uint4 random, int condition, inout int tailTuftGraphic)
{
    // Check that it is here at all
    d += MatchDistance(condition, nextInput) * MISSING_PENALTY;
    
    // Set values
    float rigor = RandomValueIf(random, condition);
    d += Distance(rigor, nextInput) * condition;
    
    float sizeFac = pow(RandomValueIf(random, condition), 0.7);
    d += Distance(sizeFac, nextInput) * condition;
    
    int graphic = RandomRangeIf(0, 6, random, condition);
    graphic = (graphic == 2 ? RandomRangeIf(0, 6, random, condition && graphic == 2) : graphic);
    MaybeSetTailTuftGraphic(graphic, tailTuftGraphic, condition);
    d += MatchDistance(graphic, nextInput) * condition;
    
    int numGills = RandomRangeIf(2, 8, random, condition);
    d += Distance(numGills, nextInput) * condition;
    
    float widthFac = RandomValueIf(random, condition);
    d += Distance(widthFac, nextInput) * condition;
    
    float backwardsFac = lerp(0.1, 0.9, RandomValueIf(random, condition));
    d += Distance(backwardsFac, nextInput) * condition;
    
    // Offset picker
    ShiftIf(random, 4 * numGills, condition);
}

void BodyStripesVars(inout float d, Inputs inputs, inout int inputPtr, inout uint4 random, int condition, float tailLengthIVar)
{
    // Check that it is here at all
    d += MatchDistance(condition, nextInput) * MISSING_PENALTY;
    
    // Set values
    float bodyAndTailLength = GetBodyAndTailLength(tailLengthIVar);
    float bodyRange = lerp(0.4, 0.8, RandomValueIf(random, condition)) * bodyAndTailLength;
    float divisions = lerp(5, 12, RandomValueIf(random, condition)) * 1.5;
    
    int numScales = (int) max(3, bodyRange / divisions);
    d += Distance(numScales, nextInput) * condition;
}

void BumpHawkVars(inout float d, Inputs inputs, inout int inputPtr, inout uint4 random, int condition, float tailLengthIVar)
{
    // Check that it is here at all
    d += MatchDistance(condition, nextInput) * MISSING_PENALTY;
    
    // Precalculate
    float bodyAndTailLength = GetBodyAndTailLength(tailLengthIVar);

    // Get values
    int colored = RandomValueIf(random, condition) < 0.5;
    d += MatchDistance(colored, nextInput) * condition;

    float val1 = RandomValueIf(random, condition);
    float val2 = RandomValueIf(random, condition);
    float spacing = colored ? lerp(3, 8, pow(val1, 0.7)) : lerp(6, 12, pow(val1, 0.5));
    float spineLength = lerp(0.3, colored ? 0.7 : 0.9, val2) * bodyAndTailLength;
    d += Distance(spineLength, nextInput) * condition;
    
    // Shift
    ShiftIf(random, 3, condition);
    
    // More values
    int numBumps = (int) (spineLength / spacing);
    d += Distance(numBumps, nextInput) * condition;

}

void LongHeadScalesVars(inout float d, Inputs inputs, inout int inputPtr, inout uint4 random, int condition, inout int tailTuftGraphic)
{
    // Check that it is here at all
    d = MatchDistance(condition, nextInput) * MISSING_PENALTY;
    
    // Rigor
    float rigor = RandomValueIf(random, condition);
    d += Distance(rigor, nextInput) * condition;
    
    // Call to GenerateTwoHorns
    ShiftIf(random, 2, condition);
    
    // Calculation for other values
    float size = pow(RandomValueIf(random, condition), 0.7) * GetParamHeadSize();
    
    // Colored (which involves lizard types)
    int colored = RandomValueIf(random, condition) < 0.5;
#if defined(LizardType_White) || defined(LizardType_Black)
    colored = 0;
#else
    colored = colored || size < 0.2;
#endif
    d += MatchDistance(colored, nextInput) * condition;
    
    // Graphic
    int graphic = RandomRangeIf(4, 6, random, condition);
    
    bool check = size < 0.5;
    check = RandomValueIf(random, condition && check) < 0.5 && check;
    graphic = check ? 6 : graphic;      // equivalent: if (size < 0.5f && Random.Value < 0.5f) graphic = 6;
    graphic = size > 0.8 ? 5 : graphic; // equivalent: else if (size > 0.8f) graphic = 5;
                                        // notice: size < 0.5f and size > 0.8f are mutually exclusive so no need to check if the other condition failed
    
    MaybeSetTailTuftGraphic(graphic, tailTuftGraphic, condition);
    d += MatchDistance(graphic, nextInput) * condition;
    
    // Other, much simpler values
    float randomWidth = RandomValueIf(random, condition);
    ShiftIf(random, condition);
    
    float length = lerp(5, 35, saturate(size));
    float width = lerp(0.65, 1.2, saturate(randomWidth * size));
    d += Distance(length, nextInput) * condition;
    d += Distance(width, nextInput) * condition;
    
}

void LongShoulderScalesVars(inout float d, Inputs inputs, inout int inputPtr, inout uint4 random, int condition, float tailLengthIVar, inout int tailTuftGraphic)
{
    // Check that it is here at all
    d += MatchDistance(condition, nextInput) * MISSING_PENALTY;
    
    // Determine scale type
    int scaleType = BodyScaleType_Patch;
#if !defined(LizardType_Pink)
    scaleType = RandomRangeIf(0, 3, random, condition);
#else
    int check1 = RandomValueIf(random, condition) < 0.33333334;
    scaleType = check1 ? RandomRangeIf(0, 3, random, condition && check1) : scaleType;
    int check2 = !check1 && RandomValueIf(random, condition && !check1) < 0.5;
    scaleType = check2 ? 2 : scaleType;
#endif
    
    d += MatchDistance(scaleType, nextInput) * condition;
    
    // Body scale type (mainly looking for numScales)
    int numScales = 0;
    int temp;
    int isPatch = scaleType == BodyScaleType_Patch;
    int isTwoLines = scaleType == BodyScaleType_TwoLines;
    int isSegments = scaleType == BodyScaleType_Segments;
    
    numScales = isPatch ? RandomRangeIf(4, 15, random, condition && isPatch) : numScales;
    GeneratePatchPattern(random, numScales, condition && isPatch);
    
    temp = GenerateTwoLines(random, tailLengthIVar, 0.07, 1, 1.5, 3, condition && isTwoLines);
    numScales = isTwoLines ? temp : numScales;
    
    temp = GenerateSegments(random, tailLengthIVar, 0.1, 0.8, 5, condition && isSegments);
    numScales = isSegments ? temp : numScales;
    
    d += Distance(numScales, nextInput) * condition;
    
    // Scaling
    float val1 = RandomValueIf(random, condition);
    float generalScale = lerp(1, 1 / lerp(1, numScales, val1 * val1), 0.5);
    
    float minSize = lerp(5, 15, RandomValueIf(random, condition)) * generalScale;
    float maxSize = lerp(minSize, 35, sqrt(RandomValueIf(random, condition))) * generalScale;
    
#if defined(LizardType_Red)
    minSize = max(10, minSize) * 1.2;
    maxSize = max(25, maxSize) * 1.2;
#endif
    
    d += Distance(minSize, nextInput);
    d += Distance(maxSize, nextInput);
    
    // Colored
#if defined(LizardType_Green) || defined(LizardType_Red)
    int colored = 1;
#else
    int colored = RandomValueIf(random, condition) < 0.4;
#endif
    d += MatchDistance(colored, nextInput);
    
    // Graphic
    int check = RandomValueIf(random, condition) < 0.1;
    int graphic = RandomRangeIf(check ? 0 : 3, check ? 7 : 6, random, condition);
#if defined(LizardType_Pink)
    check = RandomValueIf(random, condition) < 0.25;
    graphic = check ? 0 : graphic;
#elif defined(LizardType_Red)
    check = RandomValueIf(random, condition) < 0.3;
    graphic = check ? 3 : 0;
    ShiftIf(random, condition && check);
#endif
    MaybeSetTailTuftGraphic(graphic, tailTuftGraphic, condition);
    d += MatchDistance(graphic, nextInput);
    
    // Final offsetting
    ShiftIf(random, condition);
}

void PeachBackFinsVars(inout float d, Inputs inputs, inout int inputPtr, inout uint4 random, int condition)
{
    // Check that it is here at all
    d += MatchDistance(condition, nextInput) * MISSING_PENALTY;
    
    // Values
    float temp = RandomValueIf(random, condition);
    float minSize = lerp(0.27, 0.35, temp * temp);
    d += Distance(minSize, nextInput) * condition;
    
    float maxSize = lerp(minSize, 0.7, RandomValueIf(random, condition));
    d += Distance(maxSize, nextInput) * condition;
    
    float sizeSkewExponent = lerp(0.5, 1.5, RandomValueIf(random, condition));
    d += Distance(sizeSkewExponent, nextInput) * condition;
    
    int graphic = RandomValueIf(random, condition) < 0.5 ? 4 : 5;
    d += MatchDistance(graphic, nextInput) * condition;
    
    int bumps = RandomValueIf(random, condition) < 0.5 ? 3 : 4;
    d += Distance(bumps, nextInput) * condition;
    
    float scaleX = lerp(1.5, 2.5, RandomValueIf(random, condition));
    d += Distance(scaleX, nextInput) * condition;

}

void PeachHeadStripesVars(inout float d, Inputs inputs, inout int inputPtr, inout uint4 random, int condition)
{
    // Check that it is here at all
    d += MatchDistance(condition, nextInput) * MISSING_PENALTY;
    
    // Value (singular)
    float alpha = RandomValueIf(random, condition);
    d += Distance(alpha, nextInput) * condition;
}

void ShortBodyScalesVars(inout float d, Inputs inputs, inout int inputPtr, inout uint4 random, int condition, float tailLengthIVar)
{
    // Check that it is here at all
    d += MatchDistance(condition, nextInput) * MISSING_PENALTY;
    
    // Scale type
    int scaleType = RandomRangeIf(0, 3, random, condition);
#if defined(LizardType_Green)
    int check = RandomValueIf(random, condition) < 0.7;
    scaleType = check ? BodyScaleType_Segments : scaleType;
#elif defined(LizardType_Blue)
    int check = RandomValueIf(random, condition) < 0.93;
    scaleType = check ? BodyScaleType_TwoLines : scaleType;
#endif
    d += MatchDistance(scaleType, nextInput) * condition;
    
    // Body scale type
    int numScales = 0;
    int temp;
    int isPatch = scaleType == BodyScaleType_Patch;
    int isTwoLines = scaleType == BodyScaleType_TwoLines;
    int isSegments = scaleType == BodyScaleType_Segments;
    
    numScales = isPatch ? RandomRangeIf(4, 15, random, condition && isPatch) : numScales;
    GeneratePatchPattern(random, numScales, condition && isPatch);
    
    temp = GenerateTwoLines(random, tailLengthIVar, 0.1, 1, 1.5, 1, condition && isTwoLines);
    numScales = isTwoLines ? temp : numScales;
    
#if defined(LizardType_Pink)
    float lengthExponent = 1.5;
#else
    float lengthExponent = 0.6;
#endif
    temp = GenerateSegments(random, tailLengthIVar, 0.1, 0.9, lengthExponent, condition && isSegments);
    numScales = isSegments ? temp : numScales;
    
    d += Distance(numScales, nextInput) * condition;

}

void SkinkSpecklesVars(inout float d, Inputs inputs, inout int inputPtr, inout uint4 random, int condition)
{
    // Check that it is here at all
    d += MatchDistance(condition, nextInput) * MISSING_PENALTY;
    
    // Value
    int spots = RandomRangeIf(RandomRangeIf(0, 20, random, condition), 50, random, condition);
    d += Distance(spots, nextInput) * condition;
    
    // Offsetting
    ShiftIf(random, 3 * spots, condition);
}

void SnowAccumulationVars(inout uint4 random, int condition)
{
    ShiftIf(random, 48, condition);
}

void SpineSpikesVars(inout float d, Inputs inputs, inout int inputPtr, inout uint4 random, int condition, float tailLengthIVar, inout int tailTuftGraphic)
{
    // Check that it is here at all
    d += MatchDistance(condition, nextInput) * MISSING_PENALTY;
    
    // Precalc
    float bodyAndTailLength = GetBodyAndTailLength(tailLengthIVar);
    
    // Spine stuff
    float bumpDiv = lerp(5, 8, pow(RandomValueIf(random, condition), 0.7));
    float spineLength = lerp(0.2, 0.95, RandomValueIf(random, condition)) * bodyAndTailLength;
    d += Distance(spineLength, nextInput) * condition;
    
    int numScales = (int) (spineLength / bumpDiv);
    d += Distance(numScales, nextInput) * condition;
    
    ShiftIf(random, 4, condition);
#if !defined(LizardType_Blue)
    ShiftIf(random, condition);
#endif
    
    // Spine graphic and flip
    int spinesFlipped = 0;
    int graphic = RandomRangeIf(0, 5, random, condition);
    graphic = (graphic == 1) ? 0 : graphic;
    bool check1 = graphic == 4;
    graphic = check1 ? 3 : graphic;
    bool check2 = !check1 && graphic == 3;
    check2 = RandomValueIf(random, condition && check2) < 0.5 && check2;
    spinesFlipped = check2 ? 1 : spinesFlipped;
    bool check3 = !check1 && !check2;
    check3 = RandomValueIf(random, condition && check3) < 0.06666667 && check3;
    spinesFlipped = check3 ? 1 : spinesFlipped;
    
#if defined(LizardType_Pink)
    bool check = RandomValueIf(random, condition) < 0.7;
    graphic = check ? 0 : graphic;
#elif defined(LizardType_Green)
    bool check = RandomValueIf(random, condition) < 0.5;
    graphic = check ? 3 : graphic;
#endif
    
    MaybeSetTailTuftGraphic(graphic, tailTuftGraphic, condition);
    d += MatchDistance(spinesFlipped, nextInput) * condition;
    d += MatchDistance(graphic, nextInput) * condition;
    
    // Color mode
    int colorMode = RandomRangeIf(0, 3, random, condition);
#if defined(LizardType_Pink)
    bool check4 = RandomValueIf(random, condition) < 0.5;
    colorMode = check4 ? SpineSpikes_ColorMode_None : colorMode;
#elif defined(LizardType_Green)
    bool check4 = RandomValueIf(random, condition) < 0.5;
    colorMode = check4 ? SpineSpikes_ColorMode_Gradient : colorMode;
    check4 = RandomValueIf(random, condition && !check4) < 0.5 && !check4;
    colorMode = check4 ? SpineSpikes_ColorMode_Full : colorMode;
#elif defined(LizardType_Train)
    colorMode = SpineSpikes_ColorMode_Full;
#endif
    
    d += MatchDistance(colorMode, nextInput) * condition;
    
}

void TailFinVars(inout float d, Inputs inputs, inout int inputPtr, inout uint4 random, int condition, float tailLengthIVar)
{
    // Check that it is here at all
    d += MatchDistance(condition, nextInput) * MISSING_PENALTY;
    
    // Precalc
    float bodyAndTailLength = GetBodyAndTailLength(tailLengthIVar);
    
    // Spine sizes
    float bumpDiv = lerp(4, 7, pow(RandomValueIf(random, condition), 0.7));
    float spineLength = ClampedRandomVariationIf(0.5, 0.17, 0.5, random, condition);
    d += Distance(spineLength, nextInput) * condition;
    
    float undersideSize = lerp(0.3, 0.9, RandomValueIf(random, condition));
    d += Distance(undersideSize, nextInput) * condition;
    
    // Random offsetting
    ShiftIf(random, 3, condition);
    
    // Graphic
    int graphic = RandomRangeIf(0, 6, random, condition);
#if defined(LizardType_Red)
    graphic = 0;
#endif
    d += MatchDistance(graphic, nextInput) * condition;
    
    // More scale stuff
    int numScales = (int) (spineLength / bumpDiv);
    d += Distance(numScales, nextInput) * condition;
    
    float spineScaleX = lerp(1, 2, RandomValueIf(random, condition));
    d += Distance(spineScaleX, nextInput) * condition;
    
    // Offsetting
    bool check = (graphic == 3);
    check = RandomValueIf(random, condition && check) && check;
    check = !check && graphic != 0;
    ShiftIf(random, condition && check);
    
    // Colored
    int colored = RandomValueIf(random, condition) > 0.33333334;
    d += MatchDistance(colored, nextInput) * condition;

}

void TailGeckoScalesVars(inout float d, Inputs inputs, inout int inputPtr, inout uint4 random, int condition, float tailColorIVar, float wingScalesScaleLength)
{
    // Check that it is here at all
    d += MatchDistance(condition, nextInput) * MISSING_PENALTY;
    
    // Base calculations
    int rows = RandomRangeIf(7, 14, random, condition);
    int lines = 3;
    ShiftIf(random, condition);
    
    // Big scales
    int bigScales = tailColorIVar > 0.1;
    bigScales = RandomValueIf(random, condition && bigScales) < lerp(0.7, 0.99, tailColorIVar);
    bigScales = bigScales && wingScalesScaleLength <= 10;

    d += MatchDistance(bigScales, nextInput) * condition;
    
    // Get actual rows and lines
    bool check = RandomValueIf(random, condition) < 0.5;
    rows += RandomRangeIf(0, RandomRangeIf(0, 7, random, condition && check), random, condition && check) * check;
    lines += RandomRangeIf(0, RandomRangeIf(0, 3, random, condition && check), random, condition && check) * check;
    
    d += Distance(rows, nextInput) * condition;
    d += Distance(lines, nextInput) * condition;
}

void TailTuftVars(inout float d, Inputs inputs, inout int inputPtr, inout uint4 random, int condition, float tailLengthIVar, inout int tailTuftGraphic)
{
    // Check that it is here at all
    d += MatchDistance(condition, nextInput) * MISSING_PENALTY;
    
    // Figure out the evil check
    int check = RandomValueIf(random, condition) < 0.14285715;
#if defined(LizardType_Blue)
    check = RandomValueIf(random, condition && !check) < 0.9 || check;
#else
    ShiftIf(random, condition && !check);
#endif
#if defined(LizardType_Red) || defined(LizardType_Zoop)
    check = 1;
#endif
    
    // Scale type and num scales (evil combined if-else)
    int scaleType = check ? BodyScaleType_TwoLines : BodyScaleType_Patch;
    int numScales = RandomRangeIf(3, 7, random, condition && !check);
#if defined(LizardType_Blue)
    int temp = GenerateTwoLines(random, tailLengthIVar, 0, 0.7, 1, 3, condition && check);
    numScales = check ? temp : numScales;
#elif defined(LizardType_Red)
    int temp = GenerateTwoLines(random, tailLengthIVar, 0, 0.3, 1, 3, condition && check);
    numScales = check ? temp : numScales;
#else
    int temp = GenerateTwoLines(random, tailLengthIVar, 0, 0.4, 1.2, 1.3, condition && check);
    numScales = check ? temp : numScales;
#endif
    GeneratePatchPattern(random, numScales, condition && !check);
    
    d += MatchDistance(scaleType, nextInput) * condition;
    d += Distance(numScales, nextInput) * condition;
    
    // Offset
    ShiftIf(random, 3, condition);
    
    // More stuff
    int colored = RandomValueIf(random, condition) < 0.8;
    d += MatchDistance(colored, nextInput);
    
    int graphic = RandomRangeIf(3, 7, random, condition);
    graphic = (graphic == 3) ? 1 : graphic;
    bool check2 = (RandomValueIf(random, condition) < 0.033333335);
    graphic = check2 ? RandomRangeIf(0, 7, random, condition && check2) : graphic;
    check2 = RandomValueIf(random, condition) < 0.8;
#if defined(LizardType_Red) || defined(LizardType_Zoop)
    check2 = 1;
#endif
    check2 = check2 && tailTuftGraphic > -1;
    graphic = check2 ? tailTuftGraphic : graphic;
    
    MaybeSetTailTuftGraphic(graphic, tailTuftGraphic, condition);
    d += MatchDistance(graphic, nextInput) * condition;
    
    // More offsetting
    ShiftIf(random, condition);
}

void WhiskersVars(inout float d, Inputs inputs, inout int inputPtr, inout uint4 random, int condition)
{
    // Check that it is here at all
    d += MatchDistance(condition, nextInput) * MISSING_PENALTY;
    
    // Value
    int numWhiskers = RandomRangeIf(3, 5, random, condition);
    d += Distance(numWhiskers, nextInput) * condition;
    
    // Offset
    ShiftIf(random, numWhiskers * 9 + (numWhiskers - 1) * 4, condition);
}

void WingScalesVars(inout float d, Inputs inputs, inout int inputPtr, inout uint4 random, int condition, out float wingScalesScaleLength)
{
    // Check that it is here at all
    d += MatchDistance(condition, nextInput) * MISSING_PENALTY;
    
    // Values
    int numScales = RandomValueIf(random, condition) < 0.2 ? 3 : 2;
    d += Distance(numScales, nextInput) * condition;
    
    bool check = RandomValueIf(random, condition) >= 0.4;
    int graphic = check ? RandomRangeIf(0, 5, random, condition && check) : 0;
    d += MatchDistance(graphic, nextInput) * condition;
    
    float sturdy = RandomValueIf(random, condition);
    ShiftIf(random, condition);
    
    float scaleLength = lerp(5, 40, pow(RandomValueIf(random, condition), 0.75 + 1.25 * sturdy));
    d += Distance(scaleLength, nextInput) * condition;
    
    float frontDir = lerp(-0.1, 0.2, RandomValueIf(random, condition));
    float backDir = lerp(max(0, frontDir), frontDir + numScales * 0.2, RandomValueIf(random, condition));
    d += Distance(frontDir, nextInput) * condition;
    d += Distance(backDir, nextInput) * condition;
    
    // Offsetting
    ShiftIf(random, 2 * numScales, condition);
}



float GenerateLizardCosmetics(inout uint4 random, Inputs inputs)
{
    int check, check2, check3, check4;
    int inputPtr = 0;
    float d = 0;
    
    // Calculate IVars ahead of time
    Shift(random, 5);
    float tailLengthIVar = ClampedRandomVariation(0.5, 0.2, 0.3, random) * 2;
    Shift(random, 2);
    float tailColorIVar = 0;
#if !defined(LizardType_White)
    check = RandomValue(random) > 0.5;
    tailColorIVar = check ? RandomValueIf(random, check) : tailColorIVar;
#endif
#if defined(LizardType_Black)
    Shift(random, 2);
#endif
    
    // Extra offsetting
    int numLegs = 4;
#if defined(LizardType_Caramel)
    numLegs = 6;
#endif
    Shift(random, numLegs + NumTailSegments() + NumTongueSegments() + 1);
    
    // Melanistic
#if defined(LizardType_Salamander)
    int blackSalamander = RandomValue(random) < 0.33333334;
    d += MatchDistance(blackSalamander, nextInput);
#else
    Shift(random);
#endif
    
    // Start of cosmetics
    int tailTuftGraphic = -1;
    float wingScalesScaleLength = -1;
    
#if defined(LizardType_Eel)
    AxolotlGillsVars(d, inputs, inputPtr, random, 1, tailTuftGraphic);
    TailGeckoScalesVars(d, inputs, inputPtr, random, 1, tailColorIVar, wingScalesScaleLength);
    
    check = RandomValue(random) < 0.75;
    
    LongShoulderScalesVars(d, inputs, inputPtr, random, check, tailLengthIVar, tailTuftGraphic);
    TailFinVars(d, inputs, inputPtr, random, check, tailLengthIVar);
    
    ShortBodyScalesVars(d, inputs, inputPtr, random, !check, tailLengthIVar);
    check2 = RandomValueIf(random, !check) < 0.75 && !check;
    TailFinVars(d, inputs, inputPtr, random, check2, tailLengthIVar);
    TailTuftVars(d, inputs, inputPtr, random, !check && !check2, tailLengthIVar, tailTuftGraphic);
#elif defined(LizardType_Zoop)
    check = RandomValue(random) < 0.175;
    WingScalesVars(d, inputs, inputPtr, random, check, wingScalesScaleLength);
    SpineSpikesVars(d, inputs, inputPtr, random, !check, tailLengthIVar, tailTuftGraphic);
    
    TailTuftVars(d, inputs, inputPtr, random, 1, tailLengthIVar, tailTuftGraphic);
#endif
    
#if defined(LizardType_Cyan)
    check = RandomValue(random) < 0.75;
    WingScalesVars(d, inputs, inputPtr, random, check, wingScalesScaleLength);
    
    check = RandomValue(random) < 0.5 && tailColorIVar == 0;
    TailTuftVars(d, inputs, inputPtr, random, check, tailLengthIVar, tailTuftGraphic);
    TailGeckoScalesVars(d, inputs, inputPtr, random, !check, tailColorIVar, wingScalesScaleLength);
    
#elif defined(LizardType_White)
    check = RandomValue(random) < 0.4;
    BumpHawkVars(d, inputs, inputPtr, random, check, tailLengthIVar);
    check2 = RandomValueIf(random, !check) < 0.4 && !check;
    ShortBodyScalesVars(d, inputs, inputPtr, random, check2, tailLengthIVar);
    check3 = RandomValueIf(random, !check && !check2) < 0.2 && !check && !check2;
    LongShoulderScalesVars(d, inputs, inputPtr, random, check3, tailLengthIVar, tailTuftGraphic);
    check4 = RandomValueIf(random, !check && !check2 && !check3) < 0.2 && !check && !check2 && !check3;
    LongHeadScalesVars(d, inputs, inputPtr, random, check4, tailTuftGraphic);
    
    check = RandomValue(random) < 0.5;
    TailTuftVars(d, inputs, inputPtr, random, check, tailLengthIVar, tailTuftGraphic);
    
#elif defined(LizardType_Indigo)
    // This technically isn't its own branch, but it is equivalent here
    SkinkSpecklesVars(d, inputs, inputPtr, random, 1);
    
#else
    // Generic lizard stuff. I hate this part
    int backDecals = 0;
    bool longShoulderScales = false;
    bool shortBodyScales = false;
    
    // else if (type == LizardType.Caramel && Random.Value < 0.6f)
    check = 0;
#if defined(LizardType_Caramel)
    check = RandomValue(random) < 0.6;
    BodyStripesVars(d, inputs, inputPtr, random, check, tailLengthIVar);
    backDecals += check;
#endif // caramel
    
    // else if (Random.Value < 0.06666667f || (Random.Value < 0.8f && type == LizardType.Green) || (Random.Value < 0.7f && type == LizardType.Black))
    check2 = RandomValueIf(random, !check) < 0.06666667 && !check;
#if defined(LizardType_Green)
    check3 = RandomValueIf(random, !check && !check2) < 0.8 && !check && !check2;
    check2 = check2 || check3;
#else // green
    ShiftIf(random, !check && !check2);
#endif // green
#if defined(LizardType_Black)
    check3 = RandomValueIf(random, !check && !check2) < 0.7 && !check && !check2;
    check2 = check2 || check3;
#else // black
    ShiftIf(random, !check && !check2);
#endif // black
    SpineSpikesVars(d, inputs, inputPtr, random, check2, tailLengthIVar, tailTuftGraphic);
    backDecals += check2;
    check = check || check2;
    
    // else if (Random.Value < 0.033333335f && type != LizardType.Caramel)
#if !defined(LizardType_Caramel)
    check2 = RandomValueIf(random, !check) < 0.033333335 && !check;
    BumpHawkVars(d, inputs, inputPtr, random, check2, tailLengthIVar);
    backDecals += check2;
    check = check || check2;
#else // caramel
    ShiftIf(random, !check);
#endif // caramel
    
    // else if ((Random.Value < 0.04761905f || (type == LizardType.Pink && Random.Value < 0.5f) || (type == LizardType.Red && Random.Value < 0.9f)) && type != LizardType.Salamander)
#if !defined(LizardType_Salamander)
    check2 = RandomValueIf(random, !check) < 0.04761905 && !check;
#if defined(LizardType_Pink)
    check3 = RandomValueIf(random, !check && !check2) < 0.5 && !check && !check2;
    check2 = check3 || check2;
#endif // pink
#if defined(LizardType_Red)
    check3 = RandomValueIf(random, !check && !check2) < 0.9 && !check && !check2;
    check2 = check3 || check2;
#endif // red
    LongShoulderScalesVars(d, inputs, inputPtr, random, check2, tailLengthIVar, tailTuftGraphic);
    longShoulderScales = longShoulderScales || check2;
    backDecals += check2;
    check = check || check2;
#else // salamander
    ShiftIf(random, !check);
#endif // salamander
    
    // else if ((Random.Value < 0.0625f || (type == LizardType.Blue && Random.Value < 0.5f)) && type != LizardType.Salamander)
#if !defined(LizardType_Salamander)
    check2 = RandomValueIf(random, !check) < 0.0625 && !check;
#if defined(LizardType_Blue)
    check3 = RandomValueIf(random, !check && !check2) < 0.5 && !check && !check2;
    check2 = check3 || check2;
#endif // blue
    ShortBodyScalesVars(d, inputs, inputPtr, random, check2, tailLengthIVar);
    shortBodyScales = shortBodyScales || check2;
    backDecals += check2;
    check = check || check2;
#else // salamander
    ShiftIf(random, !check);
#endif // salamander
    
    // else if (type == LizardType.Green && Random.Value < 0.5f)
#if defined(LizardType_Green)
    check2 = RandomValueIf(random, !check) < 0.5 && !check;
    ShortBodyScalesVars(d, inputs, inputPtr, random, check2, tailLengthIVar);
    shortBodyScales = shortBodyScales || check2;
    backDecals += check2;
#endif // green
    
    // end of first if-else chain
    
    // if (type != LizardType.Salamander && type != LizardType.Indigo)
#if !defined(LizardType_Salamander) && !defined(LizardType_Indigo)
    
    // if (type == LizardType.Caramel && Random.Value < 0.5f)
    check = 0;
#if defined(LizardType_Caramel)
    check = RandomValue(random) < 0.5;
    TailTuftVars(d, inputs, inputPtr, random, check, tailLengthIVar, tailTuftGraphic);
#endif // caramel
    
    // else if (Random.Value < 0.11111111f || (backDecals == 0 && Random.Value < 0.7f) || (type == LizardType.Pink && Random.Value < 0.6f) || (type == LizardType.Blue && Random.Value < 0.96f))
    check2 = RandomValueIf(random, !check) < 0.11111111 && !check;
    check2 = check2 || (backDecals == 0 && RandomValueIf(random, !check && !check2 && backDecals == 0) < 0.7 && !check);
#if defined(LizardType_Pink)
    check3 = RandomValueIf(random, !check && !check2) < 0.6 && !check && !check2;
    check2 = check3 || check2;
#endif // pink
#if defined(LizardType_Blue)
    check3 = RandomValueIf(random, !check && !check2) < 0.96 && !check && !check2;
    check2 = check3 || check2;
#endif // blue
    TailTuftVars(d, inputs, inputPtr, random, check2, tailLengthIVar, tailTuftGraphic);
    check = check || check2;
    
    // else if (backDecals < 2 && type == LizardType.Green && Random.Value < 0.7f)
#if defined(LizardType_Green)
    check2 = RandomValueIf(random, !check && backDecals < 2) < 0.7 && !check && backDecals < 2;
    
    // if (Random.Value < 0.5f || longShoulderScales || shortBodyScales)
    check3 = RandomValueIf(random, check2) < 0.5 || longShoulderScales || shortBodyScales;
    
    TailTuftVars(d, inputs, inputPtr, random, check2 && check3, tailLengthIVar, tailTuftGraphic);
    
    LongShoulderScalesVars(d, inputs, inputPtr, random, check2 && !check3, tailLengthIVar, tailTuftGraphic);
    longShoulderScales = longShoulderScales || (check2 && !check3);
    backDecals += (check2 && !check3);
#endif // green
    
#endif // !salamdner && !indigo
    
    // end of second if-else chain
    
    // if (Random.Value < (backDecals == 0 ? 0.7f : 0.1f) && type != LizardType.Salamander && type != LizardType.Yellow && type != LizardType.Indigo 
    //      && ((!longShoulderScales && Random.Value < 0.9f) || Random.Value < 0.033333335f))
#if !defined(LizardType_Salamander) && !defined(LizardType_Yellow) && !defined(LizardType_Indigo)
    check = RandomValue(random) < (backDecals == 0 ? 0.7 : 0.1);
    check2 = RandomValueIf(random, check && !longShoulderScales) < 0.9 && !longShoulderScales;
    check2 = check2 || RandomValueIf(random, check && !check2) < 0.033333335;
    
    LongHeadScalesVars(d, inputs, inputPtr, random, check && check2, tailTuftGraphic);
#else
    Shift(random);
#endif // salamander && yellow && indigo
    
    // ok time for all the type-specific shit
#if defined(LizardType_Salamander)
    AxolotlGillsVars(d, inputs, inputPtr, random, 1, tailTuftGraphic);
    TailFinVars(d, inputs, inputPtr, random, 1, tailLengthIVar);
#elif defined(LizardType_Black)
    WhiskersVars(d, inputs, inputPtr, random, 1);
#elif defined(LizardType_Yellow)
    AntennaeVars(d, inputs, inputPtr, random, 1);
    
    // if (backDecals == 0 && Random.Value < 0.6f)
    check = RandomValueIf(random, backDecals == 0) < 0.6 && backDecals == 0;
    ShortBodyScalesVars(d, inputs, inputPtr, random, check, tailLengthIVar);
    backDecals += check;
#elif defined(LizardType_Red) || defined(LizardType_Train)
    LongShoulderScalesVars(d, inputs, inputPtr, random, 1, tailLengthIVar, tailTuftGraphic);
    SpineSpikesVars(d, inputs, inputPtr, random, 1, tailLengthIVar, tailTuftGraphic);
    backDecals += 2;
    
    check = RandomValue(random) < 0.5;
    TailFinVars(d, inputs, inputPtr, random, check, tailLengthIVar);
    TailTuftVars(d, inputs, inputPtr, random, !check, tailLengthIVar, tailTuftGraphic);
#elif defined(LizardType_Basilisk)
    // BumpHawkVars
    // TailGeckoScalesVars
    // TailGeckoScalesVars
    // for (int i = 0; i < 9; i++)
    //     ShortBodyScalesVars
    //     TailTuftVars
    // LongShoulderScalesVars
    // SkinkSpecklesVars
#elif defined(LizardType_Blizzard)
    // for (int i = 0; i < 5; i++)
    //     check = RandomValue(random) + 0.00001 < 0.5;
    //     AxolotlGillsVars <- check
    // for (int i = 0; i < 5; i++)
    //     check = RandomValue(random) + 0.00001 < 0.5;
    //     AxolotlGillsVars <- check
    // LongHeadScalesVars
    // for (int i = 0; i < 8; i++)
    //     check = RandomValue(random) + 0.00001 < 0.5;
    //     ShortBodyScalesVars <- check
    // for (int i = 0; i < 5; i++)
    //     check = RandomValue(random) + 0.00001 <= 0.8;
    //     SpineSpikesVars <- check
    // for (int i = 0; i < 10; i++)
    //     TailTuftVars
    // check = RandomValue(random) + 0.00001 <= 0.050000000;
    // WhiskersVars <- check
#elif defined(LizardType_Peach)
    PeachHeadStripesVars(d, inputs, inputPtr, random, 1);
    TailFinVars(d, inputs, inputPtr, random, 1, tailLengthIVar);
    PeachBackFinsVars(d, inputs, inputPtr, random, 1);
#endif // type-specific cosmetics
    
    // if (backDecals == 0 && type == LizardType.Caramel)
#if defined(LizardType_Caramel)
    check = (backDecals == 0);
    BumpHawkVars(d, inputs, inputPtr, random, check, tailLengthIVar);
    // backDecals += check; // but we don't actually need this for anything past this point :trollface:
#endif // caramel
    
    // if zoop: SnowAccumulationVars
    // (not that it matters because it has no searchable properties)
    
#endif // outer
    
    return d;
}

#endif
