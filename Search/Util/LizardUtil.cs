using System;
using MoreSlugcats;
using UnityEngine;

namespace FinderMod.Search.Util
{
    internal static class LizardUtil
    {
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
            Eel
        }
        public enum LizardBodyScaleType
        {
            Patch = 0,
            TwoLines = 1,
            Segments = 2
        }
        private static readonly int[] bodyScaleTypeRange = new int[] { 0, 3 };
        private static readonly int[] bodyScalesPatchRange = new int[] { 4, 15 };
        private static readonly int[] bodyScaleSegmentsRange = new int[] { 1, 4 };

        private static void GeneratePatchPattern(ref int picker, int numScales)
        {
            picker++;
            picker += 2 * numScales;
        }
        private static void GenerateTwoLines(float[] vals, ref int picker, LizardType type, float startPoint, float maxLength, float lengthExponent, float spacingScale, out int numScales)
        {
            float bodyAndTailLength = GetBodyAndTailLength(type, vals);
            float num = Mathf.Lerp(startPoint + 0.1f, Mathf.Max(startPoint + 0.2f, maxLength), Mathf.Pow(vals[picker++], lengthExponent));
            float num2 = num * bodyAndTailLength;
            float num3 = Mathf.Lerp(2f, 9f, vals[picker++]);
            if (type == LizardType.Blue)
            {
                num3 = 2f;
            }
            num3 *= spacingScale;
            int num4 = (int)(num2 / num3);
            if (num4 < 3)
            {
                num4 = 3;
            }
            numScales = num4; // * 2;
        }
        private static void GenerateSegments(float[] vals, ref int picker, int seed, LizardType type, float startPoint, float maxLength, float lengthExponent, out int numScales)
        {
            float bodyAndTailLength = GetBodyAndTailLength(type, vals);
            float num = Mathf.Lerp(startPoint + 0.1f, Mathf.Max(startPoint + 0.2f, maxLength), Mathf.Pow(vals[picker++], lengthExponent));
            float num2 = num * bodyAndTailLength;
            float num3 = Mathf.Lerp(7f, 14f, vals[picker++]);
            if (type == LizardType.Red)
            {
                num3 = Mathf.Min(num3, 11f) * 0.75f;
            }
            int num4 = Math.Max(3, (int)(num2 / num3));
            int num5 = SearchUtil.GetRangeAt(seed, bodyScaleSegmentsRange, picker++) * 2;
            numScales = num4 * num5;
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
                _ => 1f
            };
        }
        private static float GetBodyAndTailLength(LizardType type, float[] vals)
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
                _ => throw new NotImplementedException(),
            };


            float headSize = GetParamHeadSize(type);

            float bodyDistance = 17f * bodyLenFacParam * ((bodySizeFacParam + 1f) / 2f);
            float bodyLength = 5.5f * headSize + 2 * bodyDistance + bodyDistance * (1f + bodyStiffnessParam);

            int tailSegments = NumTailSegments(type);
            float tailLengthIVar = SearchOptions.ClampedRandomVariation(0.5f, 0.2f, 0.3f, vals[5], vals[6]) * 2f;
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


        // Min add. values needed: 18
        public static void AntennaeVars(float[] vals, ref int picker, out float length, out float alpha)
        {
            // Get values
            length = vals[picker++];
            alpha = length * 0.9f + vals[picker++] * 0.1f;

            // Offset for next
            int segments = Mathf.FloorToInt(Mathf.Lerp(3f, 8f, Mathf.Pow(length, Mathf.Lerp(1f, 6f, length))));
            picker += 2 * segments;
        }

        // Min add. values needed: 35
        private static readonly int[] scaleSpriteRange = new int[] { 0, 6 };
        private static readonly int[] numGillsRange = new int[] { 2, 8 };
        public static void AxolotlGillsVars(float[] vals, ref int picker, int seed, out float rigor, out int numGills)
        {
            rigor = vals[picker++];
            //float gillScalar = GetParamHeadSize(type) * Mathf.Pow(vals[picker++], 0.7f);
            picker++;

            if (SearchUtil.GetRangeAt(seed, scaleSpriteRange, picker++) == 2)
            {
                picker++;
            }

            numGills = SearchUtil.GetRangeAt(seed, numGillsRange, picker++);

            // Offset picker
            picker += 2 + 4 * numGills;
        }

        // Min add. values needed: 2
        public static void BodyStripesVars(float[] vals, ref int picker, LizardType type, out int numStripes)
        {
            float bodyAndTailLength = GetBodyAndTailLength(type, vals);
            float num3 = Mathf.Lerp(0.4f, 0.8f, vals[picker++]) * bodyAndTailLength;
            float num4 = Mathf.Lerp(5f, 12f, vals[picker++]) * 1.5f;
            numStripes = (int)(num3 / num4);
            if (numStripes > 3) numStripes = 3;
        }

        // Min add. values needed: 6
        public static void BumpHawkVars(float[] vals, ref int picker, LizardType type, out bool coloredHawk, out float spineLength, out int numBumps)
        {
            // Get body and tail length
            float bodyAndTailLength = GetBodyAndTailLength(type, vals);

            // Get output stuff
            coloredHawk = vals[picker++] < 0.5f;

            float num;
            if (coloredHawk)
            {
                num = Mathf.Lerp(3f, 8f, Mathf.Pow(vals[picker++], 0.7f));
                spineLength = Mathf.Lerp(0.3f, 0.7f, vals[picker++]) * bodyAndTailLength;
                // sizeRangeMin = Mathf.Lerp(0.1f, 0.2f, vals[picker++]);
                // sizeRangeMax = Mathf.Lerp(sizeRangeMin, 0.35f, Mathf.Pow(vals[picker++], 0.5f));
            }
            else
            {
                num = Mathf.Lerp(6f, 12f, Mathf.Pow(vals[picker++], 0.5f));
                spineLength = Mathf.Lerp(0.3f, 0.9f, vals[picker++]) * bodyAndTailLength;
                // sizeRangeMin = Mathf.Lerp(0.2f, 0.3f, Mathf.Pow(vals[picker++], 0.5f));
                // sizeRangeMax = Mathf.Lerp(sizeRangeMin, 0.5f, vals[picker++]);
            }
            // float sizeSkewExponent = Mathf.Lerp(0.1f, 0.7f, vals[picker++]);
            picker += 3;
            numBumps = (int)(spineLength / num);
        }

        // Jump rings don't use random variations

        // Min add. values needed: 9
        public static void LongHeadScalesVars(float[] vals, ref int picker, LizardType type, out float rigor, out bool colored, out float length, out float width)
        {
            // Some values
            rigor = vals[picker++];
            picker += 2; // GenerateTwoHorns()

            float num = Mathf.Pow(vals[picker++], 0.7f) * GetParamHeadSize(type);

            // Colored?
            colored = vals[picker++] < 0.5f && type != LizardType.White;

            picker++;
            if (num < 0.5f) picker++;

            if (num < 0.2f && type != LizardType.White)
            {
                colored = true;
            }
            if (type == LizardType.Black)
            {
                colored = false;
            }

            // Length and width and stuff
            float value = vals[picker++];
            picker++;
            length = Mathf.Lerp(5f, 35f, num);
            width = Mathf.Lerp(0.65f, 1.2f, value * num);
        }

        // Min add. values needed: 43
        public static void LongShoulderScalesVars(float[] vals, ref int picker, int seed, LizardType type, out LizardBodyScaleType scaleType, out int numScales, out bool colored)
        {
            scaleType = LizardBodyScaleType.Patch;
            if (type != LizardType.Pink || vals[picker++] < 0.33333334f)
            {
                scaleType = (LizardBodyScaleType)SearchUtil.GetRangeAt(seed, bodyScaleTypeRange, picker++);
            }
            else if (type == LizardType.Green || vals[picker++] < 0.5f)
            {
                scaleType = LizardBodyScaleType.Segments;
            }

            // 0 = patches, 1 = two lines, 2 = segments
            numScales = 0;
            switch (scaleType)
            {
                case LizardBodyScaleType.Patch:
                    {
                        // Patches, GeneratePatchPattern(0.05f, Random.Range(4, 15), 0.9f, 2f)
                        numScales = SearchUtil.GetRangeAt(seed, bodyScalesPatchRange, picker++);
                        GeneratePatchPattern(ref picker, numScales);
                        break;
                    }
                case LizardBodyScaleType.TwoLines:
                    {
                        // Two lines, GenerateTwoLines(0.07f, 1f, 1.5f, 3f)
                        GenerateTwoLines(vals, ref picker, type, 0.07f, 1f, 1.5f, 3f, out numScales);
                        break;
                    }
                case LizardBodyScaleType.Segments:
                    {
                        // Segments, GenerateSegments(0.1f, 0.8f, 5f)
                        GenerateSegments(vals, ref picker, seed, type, 0.1f, 0.8f, 5f, out numScales);
                        break;
                    }
            }

            picker += 3;
            colored = type == LizardType.Green || type == LizardType.Red || vals[picker++] < 0.4f;

            // Offset for any other code
            picker += 2;
            if (type == LizardType.Pink) picker++;
            if (type == LizardType.Red && vals[picker++] < 0.3f) picker++;
            picker++;

            // note: extracting scale size is fairly straightforward if want to do that in the future although it also is a bit more code
        }

        // Min add. values needed: 33
        public static void ShortBodyScalesVars(float[] vals, ref int picker, int seed, LizardType type, out LizardBodyScaleType scaleType, out int numScales)
        {
            scaleType = (LizardBodyScaleType)SearchUtil.GetRangeAt(seed, bodyScaleTypeRange, picker++);
            if (type == LizardType.Green && vals[picker++] < 0.7f) scaleType = LizardBodyScaleType.Segments;
            else if (type == LizardType.Blue && vals[picker++] < 0.93f) scaleType = LizardBodyScaleType.TwoLines;

            numScales = 0;
            switch (scaleType)
            {
                case LizardBodyScaleType.Patch:
                    {
                        // Patches, GeneratePatchPattern(0.1f, Random.Range(4, 15), 0.9f, 1.2f)
                        numScales = SearchUtil.GetRangeAt(seed, bodyScalesPatchRange, picker++);
                        GeneratePatchPattern(ref picker, numScales);
                        break;
                    }
                case LizardBodyScaleType.TwoLines:
                    {
                        // Two lines, GenerateTwoLines(0.1f, 1f, 1.5f, 1f)
                        GenerateTwoLines(vals, ref picker, type, 0.1f, 1f, 1.5f, 1f, out numScales);
                        break;
                    }
                case LizardBodyScaleType.Segments:
                    {
                        // Segments, GenerateSegments(0.1f, 0.9f, (lGraphics.lizard.Template.type == CreatureTemplate.Type.PinkLizard) ? 1.5f : 0.6f)
                        GenerateSegments(vals, ref picker, seed, type, 0.1f, 0.9f, type == LizardType.Pink ? 1.5f : 0.6f, out numScales);
                        break;
                    }
            }
        }

        // Min add. values needed: 48
        public static void SnowAccumulationVars(float[] vals, ref int picker)
        {
            // no funny things to return :(
            // though we could return what sprites are used
            picker += 48;
        }

        // Min add. values needed: 14
        private static readonly int[] spineSpikeGraphicRange = new int[] { 0, 5 };
        // private static readonly int[] spineSpikeColoredRange = new int[] { 0, 3 };
        public static void SpineSpikesVars(float[] vals, ref int picker, int seed, LizardType type, out float spineLength, out int numBumps)
        {
            float bodyAndTailLength = GetBodyAndTailLength(type, vals);

            float bumpDiv = Mathf.Lerp(5f, 8f, Mathf.Pow(vals[picker++], 0.7f));
            spineLength = Mathf.Lerp(0.2f, 0.95f, vals[picker++]) * bodyAndTailLength;
            numBumps = (int)(spineLength / bumpDiv);

            picker += 4;
            if (type != LizardType.Blue) picker++;
            // Random.value in the else-if chain is guaranteed to be called exactly once if it's not a blue lizor
            // if it's not a green lizor, it might fail the random value check, but then it's still not a green lizor in the next else if

            int graphic = SearchUtil.GetRangeAt(seed, spineSpikeGraphicRange, picker++);
            if (graphic != 4)
            {
                if (graphic != 3 || vals[picker++] >= 0.5f) picker++;
            }

            // Will need to redo this section if need sprite (graphic) because it is a big if-else chain to set graphic depending on lizard type
            if (type == LizardType.Pink || type == LizardType.Green) picker++;

            picker++;
            if (type == LizardType.Pink) picker++;
            else if (type == LizardType.Green && vals[picker++] >= 0.5f) picker++;
        }

        // Min add. values needed: 14
        private static readonly int[] tailFinGraphicRange = new int[] { 0, 6 };
        public static void TailFinVars(float[] vals, ref int picker, int seed, LizardType type, out float spineLength, out float undersideSize, out float spineScaleX, out int numBumps, out bool colored)
        {
            float bodyAndTailLength = GetBodyAndTailLength(type, vals);

            float bumpDiv = Mathf.Lerp(4f, 7f, Mathf.Pow(vals[picker++], 0.7f));
            spineLength = SearchOptions.ClampedRandomVariation(0.5f, 0.17f, 0.5f, vals[picker++], vals[picker++]) * bodyAndTailLength;
            undersideSize = Mathf.Lerp(0.3f, 0.9f, vals[picker++]);
            picker += 3;
            int graphic = SearchUtil.GetRangeAt(seed, tailFinGraphicRange, picker++);
            if (type == LizardType.Red)
            {
                graphic = 0;
                // NOTE: if lizard has LongBodyScales, it uses the same graphic. Red lizard code doesn't use this yet so didn't put it here.
                spineLength = SearchOptions.ClampedRandomVariation(0.3f, 0.17f, 0.5f, vals[picker++], vals[picker++]) * bodyAndTailLength;
            }
            numBumps = (int)(spineLength / bumpDiv);
            spineScaleX = Mathf.Lerp(1f, 2f, vals[picker++]);
            if (graphic == 3 && vals[picker++] < 0.5f)
            {
                // do nothing lol
            }
            else if (graphic != 0) picker++;
            colored = vals[picker++] < 0.33333334f;
        }

        // Min add. values needed: 9
        private static readonly int[] tgsRowsFirstRange = new int[] { 7, 14 };
        // private static readonly int[] tgsLinesFirstRange = new int[] { 3, 4 };
        private static readonly int[] tgsRowsSecondRange = new int[] { 0, 7 };
        private static readonly int[] tgsLinesSecondRange = new int[] { 0, 3 };
        public static void TailGeckoSpritesVars(float[] vals, ref int picker, int seed, out int rows, out int lines)
        {
            // Get rows, lines, and increment picker
            rows = SearchUtil.GetRangeAt(seed, tgsRowsFirstRange, picker++);
            lines = 3;
            picker++;

            // Get tail color for thingy (whites will never have this without modded so /shrug)
            float tailColor = vals[9] > 0.5f ? vals[10] : 0f;

            // Pointer offset
            if (tailColor > 0.1f) picker++;

            // Extra tidbit thing for rows and lines
            if (vals[picker++] < 0.5f)
            {
                // IMPORTANT: calling Random.Range(a, b) where a == b does not advance the random state
                int r = SearchUtil.GetRangeAt(seed, tgsRowsSecondRange, picker++);
                if (r != 0) rows += SearchUtil.GetRangeAt(seed, new int[] { 0, r }, picker++);
                int c = SearchUtil.GetRangeAt(seed, tgsLinesSecondRange, picker++);
                if (c != 0) lines += SearchUtil.GetRangeAt(seed, new int[] { 0, c }, picker++);
            }
        }

        // Min add. values needed: 27
        private static readonly int[] tailTuft3to7Range = new int[] { 3, 7 };
        public static void TailTuftVars(float[] vals, ref int picker, int seed, LizardType type, out LizardBodyScaleType scaleType, out int numScales)
        {
            float bodyAndTailLength = GetBodyAndTailLength(type, vals);

            if (vals[picker++] < 0.14285715f || vals[picker++] < 0.9f && type == LizardType.Blue || type == LizardType.Red || type == LizardType.Zoop)
            {
                scaleType = LizardBodyScaleType.TwoLines;

                if (type == LizardType.Blue || type == LizardType.Red)
                {
                    // GenerateTwoLines(0f, (lGraphics.lizard.Template.type == CreatureTemplate.Type.RedLizard) ? 0.3f : 0.7f, 1f, 3f);
                    GenerateTwoLines(vals, ref picker, type, 0f, type == LizardType.Red ? 0.3f : 0.7f, 1f, 3f, out numScales);
                }
                else
                {
                    // GenerateTwoLines(0f, 0.4f, 1.2f, 1.3f)
                    GenerateTwoLines(vals, ref picker, type, 0f, 0.4f, 1.2f, 1.3f, out numScales);
                }
            }
            else
            {
                scaleType = LizardBodyScaleType.Patch;
                numScales = SearchUtil.GetRangeAt(seed, tailTuft3to7Range, picker++);
                GeneratePatchPattern(ref picker, numScales);
            }

            // Offset for future endeavors
            picker += 5;
            if (vals[picker++] < 0.033333335f) picker++;
            picker += 2;
        }

        // Min add. values needed: 48
        private static readonly int[] numWhiskersRange = new[] { 3, 5 };
        public static void WhiskersVars(float[] vals, ref int picker, int seed, out int numWhiskers)
        {
            // Number of whiskers
            numWhiskers = SearchUtil.GetRangeAt(seed, numWhiskersRange, picker++);

            // Offset for next usage
            picker += numWhiskers * 9 + (numWhiskers - 1) * 4;
        }

        // Min add. values needed: 14
        public static void WingScalesVars(float[] vals, ref int picker, out int numScales, out float scaleLength)
        {
            // Num scales
            numScales = vals[picker++] < 0.2f ? 3 : 2;

            // Offsetting
            if (vals[picker++] >= 0.4f) picker++;

            // Scale length
            float sturdy = vals[picker++];
            picker++;
            scaleLength = Mathf.Lerp(5f, 40f, Mathf.Pow(vals[picker++], 0.75f + 1.25f * sturdy));

            // More offsetting
            picker += 2;
            picker += 2 * numScales;
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
                _ => throw new NotImplementedException()
            };
        }

        public static int NumTongueSegments(LizardType type)
        {
            return type switch
            {
                LizardType.Blue => 5,
                LizardType.White => 10,
                LizardType.Red => ModManager.MMF && MMF.cfgAlphaRedLizards.Value ? 10 : 0,
                LizardType.Salamander => 7,
                LizardType.Cyan => 7,
                LizardType.Zoop => 10,
                LizardType.Train => 10,
                _ => 0
            };
        }
    }
}
