using System;
using FinderMod.Search.Options;
using MoreSlugcats;
using UnityEngine;

namespace FinderMod.Search.Util
{
    public static class LizardUtil
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


        private static void GeneratePatchPattern(XORShift128 Random, int numScales)
        {
            Random.Shift(1 + 2 * numScales);
        }
        private static void GenerateTwoLines(XORShift128 Random, float tailLengthIVar, LizardType type, float startPoint, float maxLength, float lengthExponent, float spacingScale, out int numScales)
        {
            float bodyAndTailLength = GetBodyAndTailLength(type, tailLengthIVar);
            float num = Mathf.Lerp(startPoint + 0.1f, Mathf.Max(startPoint + 0.2f, maxLength), Mathf.Pow(Random.Value, lengthExponent));
            float num2 = num * bodyAndTailLength;
            float num3 = Mathf.Lerp(2f, 9f, Random.Value);
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
        private static void GenerateSegments(XORShift128 Random, float tailLengthIVar, LizardType type, float startPoint, float maxLength, float lengthExponent, out int numScales)
        {
            float bodyAndTailLength = GetBodyAndTailLength(type, tailLengthIVar);
            float num = Mathf.Lerp(startPoint + 0.1f, Mathf.Max(startPoint + 0.2f, maxLength), Mathf.Pow(Random.Value, lengthExponent));
            float num2 = num * bodyAndTailLength;
            float num3 = Mathf.Lerp(7f, 14f, Random.Value);
            if (type == LizardType.Red)
            {
                num3 = Mathf.Min(num3, 11f) * 0.75f;
            }
            int num4 = Math.Max(3, (int)(num2 / num3));
            int num5 = Random.Range(1, 4) * 2;
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
        public static void AntennaeVars(XORShift128 Random, out float length, out float alpha)
        {
            // Get values
            length = Random.Value;
            alpha = length * 0.9f + Random.Value * 0.1f;

            // Offset for next
            int segments = Mathf.FloorToInt(Mathf.Lerp(3f, 8f, Mathf.Pow(length, Mathf.Lerp(1f, 6f, length))));
            Random.Shift(2 * segments);
        }

        // Min add. values needed: 35
        public static void AxolotlGillsVars(XORShift128 Random, out float rigor, out int numGills)
        {
            rigor = Random.Value;
            //float gillScalar = GetParamHeadSize(type) * Mathf.Pow(Random.Value, 0.7f);
            Random.Shift();

            if (Random.Range(0, 6) == 2)
            {
                Random.Shift();
            }

            numGills = Random.Range(2, 8);

            // Offset picker
            Random.Shift(2 + 4 * numGills);
        }

        // Min add. values needed: 2
        public static void BodyStripesVars(XORShift128 Random, float tailLengthIVar, LizardType type, out int numStripes)
        {
            float bodyAndTailLength = GetBodyAndTailLength(type, tailLengthIVar);
            float num3 = Mathf.Lerp(0.4f, 0.8f, Random.Value) * bodyAndTailLength;
            float num4 = Mathf.Lerp(5f, 12f, Random.Value) * 1.5f;
            numStripes = (int)(num3 / num4);
            if (numStripes > 3) numStripes = 3;
        }

        // Min add. values needed: 6
        public static void BumpHawkVars(XORShift128 Random, float tailLengthIVar, LizardType type, out bool coloredHawk, out float spineLength, out int numBumps)
        {
            // Get body and tail length
            float bodyAndTailLength = GetBodyAndTailLength(type, tailLengthIVar);

            // Get output stuff
            coloredHawk = Random.Value < 0.5f;

            float num;
            if (coloredHawk)
            {
                num = Mathf.Lerp(3f, 8f, Mathf.Pow(Random.Value, 0.7f));
                spineLength = Mathf.Lerp(0.3f, 0.7f, Random.Value) * bodyAndTailLength;
                // sizeRangeMin = Mathf.Lerp(0.1f, 0.2f, Random.Value);
                // sizeRangeMax = Mathf.Lerp(sizeRangeMin, 0.35f, Mathf.Pow(Random.Value, 0.5f));
            }
            else
            {
                num = Mathf.Lerp(6f, 12f, Mathf.Pow(Random.Value, 0.5f));
                spineLength = Mathf.Lerp(0.3f, 0.9f, Random.Value) * bodyAndTailLength;
                // sizeRangeMin = Mathf.Lerp(0.2f, 0.3f, Mathf.Pow(Random.Value, 0.5f));
                // sizeRangeMax = Mathf.Lerp(sizeRangeMin, 0.5f, Random.Value);
            }
            // float sizeSkewExponent = Mathf.Lerp(0.1f, 0.7f, Random.Value);
            Random.Shift(3);
            numBumps = (int)(spineLength / num);
        }

        // Jump rings don't use random variations

        // Min add. values needed: 9
        public static void LongHeadScalesVars(XORShift128 Random, LizardType type, out float rigor, out bool colored, out float length, out float width)
        {
            // Some values
            rigor = Random.Value;
            Random.Shift(2); // GenerateTwoHorns()

            float num = Mathf.Pow(Random.Value, 0.7f) * GetParamHeadSize(type);

            // Colored?
            colored = Random.Value < 0.5f && type != LizardType.White;

            Random.Shift();
            if (num < 0.5f) Random.Shift();

            if (num < 0.2f && type != LizardType.White)
            {
                colored = true;
            }
            if (type == LizardType.Black)
            {
                colored = false;
            }

            // Length and width and stuff
            float value = Random.Value;
            Random.Shift();
            length = Mathf.Lerp(5f, 35f, num);
            width = Mathf.Lerp(0.65f, 1.2f, value * num);
        }

        // Min add. values needed: 43
        public static void LongShoulderScalesVars(XORShift128 Random, float tailLengthIVar, LizardType type, out LizardBodyScaleType scaleType, out int numScales, out bool colored)
        {
            scaleType = LizardBodyScaleType.Patch;
            if (type != LizardType.Pink || Random.Value < 0.33333334f)
            {
                scaleType = (LizardBodyScaleType)Random.Range(0, 3);
            }
            else if (type == LizardType.Green || Random.Value < 0.5f)
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
                        numScales = Random.Range(4, 15);
                        GeneratePatchPattern(Random, numScales);
                        break;
                    }
                case LizardBodyScaleType.TwoLines:
                    {
                        // Two lines, GenerateTwoLines(0.07f, 1f, 1.5f, 3f)
                        GenerateTwoLines(Random, tailLengthIVar, type, 0.07f, 1f, 1.5f, 3f, out numScales);
                        break;
                    }
                case LizardBodyScaleType.Segments:
                    {
                        // Segments, GenerateSegments(0.1f, 0.8f, 5f)
                        GenerateSegments(Random, tailLengthIVar, type, 0.1f, 0.8f, 5f, out numScales);
                        break;
                    }
            }

            Random.Shift(3);
            colored = type == LizardType.Green || type == LizardType.Red || Random.Value < 0.4f;

            // Offset for any other code
            Random.Shift(2);
            if (type == LizardType.Pink) Random.Shift();
            if (type == LizardType.Red && Random.Value < 0.3f) Random.Shift();
            Random.Shift();

            // note: extracting scale size is fairly straightforward if want to do that in the future although it also is a bit more code
        }

        // Min add. values needed: 33
        public static void ShortBodyScalesVars(XORShift128 Random, float tailLengthIVar, LizardType type, out LizardBodyScaleType scaleType, out int numScales)
        {
            scaleType = (LizardBodyScaleType)Random.Range(0, 3);
            if (type == LizardType.Green && Random.Value < 0.7f) scaleType = LizardBodyScaleType.Segments;
            else if (type == LizardType.Blue && Random.Value < 0.93f) scaleType = LizardBodyScaleType.TwoLines;

            numScales = 0;
            switch (scaleType)
            {
                case LizardBodyScaleType.Patch:
                    {
                        // Patches, GeneratePatchPattern(0.1f, Random.Range(4, 15), 0.9f, 1.2f)
                        numScales = Random.Range(4, 15);
                        GeneratePatchPattern(Random, numScales);
                        break;
                    }
                case LizardBodyScaleType.TwoLines:
                    {
                        // Two lines, GenerateTwoLines(0.1f, 1f, 1.5f, 1f)
                        GenerateTwoLines(Random, tailLengthIVar, type, 0.1f, 1f, 1.5f, 1f, out numScales);
                        break;
                    }
                case LizardBodyScaleType.Segments:
                    {
                        // Segments, GenerateSegments(0.1f, 0.9f, (lGraphics.lizard.Template.type == CreatureTemplate.Type.PinkLizard) ? 1.5f : 0.6f)
                        GenerateSegments(Random, tailLengthIVar, type, 0.1f, 0.9f, type == LizardType.Pink ? 1.5f : 0.6f, out numScales);
                        break;
                    }
            }
        }

        // Min add. values needed: 48
        public static void SnowAccumulationVars(XORShift128 Random)
        {
            // no funny things to return :(
            // though we could return what sprites are used
            Random.Shift(48);
        }

        // Min add. values needed: 14
        public static void SpineSpikesVars(XORShift128 Random, float tailLengthIVar, LizardType type, out float spineLength, out int numBumps)
        {
            float bodyAndTailLength = GetBodyAndTailLength(type, tailLengthIVar);

            float bumpDiv = Mathf.Lerp(5f, 8f, Mathf.Pow(Random.Value, 0.7f));
            spineLength = Mathf.Lerp(0.2f, 0.95f, Random.Value) * bodyAndTailLength;
            numBumps = (int)(spineLength / bumpDiv);

            Random.Shift(4);
            if (type != LizardType.Blue) Random.Shift();
            // Random.value in the else-if chain is guaranteed to be called exactly once if it's not a blue lizor
            // if it's not a green lizor, it might fail the random value check, but then it's still not a green lizor in the next else if

            int graphic = Random.Range(0, 5);
            if (graphic != 4)
            {
                if (graphic != 3 || Random.Value >= 0.5f) Random.Shift();
            }

            // Will need to redo this section if need sprite (graphic) because it is a big if-else chain to set graphic depending on lizard type
            if (type == LizardType.Pink || type == LizardType.Green) Random.Shift();

            Random.Shift();
            if (type == LizardType.Pink) Random.Shift();
            else if (type == LizardType.Green && Random.Value >= 0.5f) Random.Shift();
        }

        // Min add. values needed: 14
        public static void TailFinVars(XORShift128 Random, float tailLengthIVar, LizardType type, out float spineLength, out float undersideSize, out float spineScaleX, out int numBumps, out bool colored)
        {
            float bodyAndTailLength = GetBodyAndTailLength(type, tailLengthIVar);

            float bumpDiv = Mathf.Lerp(4f, 7f, Mathf.Pow(Random.Value, 0.7f));
            spineLength = Option.ClampedRandomVariation(0.5f, 0.17f, 0.5f, Random) * bodyAndTailLength;
            undersideSize = Mathf.Lerp(0.3f, 0.9f, Random.Value);
            Random.Shift(3);
            int graphic = Random.Range(0, 6);
            if (type == LizardType.Red)
            {
                graphic = 0;
                // NOTE: if lizard has LongBodyScales, it uses the same graphic. Red lizard code doesn't use this yet so didn't put it here.
                spineLength = Option.ClampedRandomVariation(0.3f, 0.17f, 0.5f, Random) * bodyAndTailLength;
            }
            numBumps = (int)(spineLength / bumpDiv);
            spineScaleX = Mathf.Lerp(1f, 2f, Random.Value);
            if (graphic == 3 && Random.Value < 0.5f)
            {
                // do nothing lol
            }
            else if (graphic != 0) Random.Shift();
            colored = Random.Value < 0.33333334f;
        }

        // Min add. values needed: 9
        public static void TailGeckoSpritesVars(XORShift128 Random, float tailColor, out int rows, out int lines)
        {
            // Get rows, lines, and increment picker
            rows = Random.Range(7, 14);
            lines = 3;
            Random.Shift();

            // Pointer offset
            if (tailColor > 0.1f) Random.Shift();

            // Extra tidbit thing for rows and lines
            if (Random.Value < 0.5f)
            {
                rows += Random.Range(0, Random.Range(0, 7));
                lines += Random.Range(0, Random.Range(0, 3));
            }
        }

        // Min add. values needed: 27
        public static void TailTuftVars(XORShift128 Random, float tailLengthIVar, LizardType type, out LizardBodyScaleType scaleType, out int numScales)
        {
            //float bodyAndTailLength = GetBodyAndTailLength(type, tailLengthIVar);

            if (Random.Value < 0.14285715f || Random.Value < 0.9f && type == LizardType.Blue || type == LizardType.Red || type == LizardType.Zoop)
            {
                scaleType = LizardBodyScaleType.TwoLines;

                if (type == LizardType.Blue || type == LizardType.Red)
                {
                    // GenerateTwoLines(0f, (lGraphics.lizard.Template.type == CreatureTemplate.Type.RedLizard) ? 0.3f : 0.7f, 1f, 3f);
                    GenerateTwoLines(Random, tailLengthIVar, type, 0f, type == LizardType.Red ? 0.3f : 0.7f, 1f, 3f, out numScales);
                }
                else
                {
                    // GenerateTwoLines(0f, 0.4f, 1.2f, 1.3f)
                    GenerateTwoLines(Random, tailLengthIVar, type, 0f, 0.4f, 1.2f, 1.3f, out numScales);
                }
            }
            else
            {
                scaleType = LizardBodyScaleType.Patch;
                numScales = Random.Range(3, 7);
                GeneratePatchPattern(Random, numScales);
            }

            // Offset for future endeavors
            Random.Shift(5);
            if (Random.Value < 0.033333335f) Random.Shift();
            Random.Shift(2);
        }

        // Min add. values needed: 48
        public static void WhiskersVars(XORShift128 Random, out int numWhiskers)
        {
            // Number of whiskers
            numWhiskers = Random.Range(3, 5);

            // Offset for next usage
            Random.Shift(numWhiskers * 9 + (numWhiskers - 1) * 4);
        }

        // Min add. values needed: 14
        public static void WingScalesVars(XORShift128 Random, out int numScales, out float scaleLength)
        {
            // Num scales
            numScales = Random.Value < 0.2f ? 3 : 2;

            // Offsetting
            if (Random.Value >= 0.4f) Random.Shift();

            // Scale length
            float sturdy = Random.Value;
            Random.Shift();
            scaleLength = Mathf.Lerp(5f, 40f, Mathf.Pow(Random.Value, 0.75f + 1.25f * sturdy));

            // More offsetting
            Random.Shift(2);
            Random.Shift(2 * numScales);
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

        public static int NumLimbs(LizardType type)
        {
            return type switch
            {
                LizardType.Caramel => 6,
                // yeah eels are treatetd as though they have all 4
                _ => 4
            };
        }
    }
}
