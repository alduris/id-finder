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
        public enum CosmeticType
        {
            Antennae,
            AxolotlGills,
            BodyStripes,
            BumpHawk,
            JumpRings, // no vars
            LongHeadScales,
            LongShoulderScales,
            ShortBodyScales,
            SnowAccumulation, // no vars
            SpineSpikes,
            TailFin,
            TailGeckoScales,
            TailTuft,
            Whiskers,
            WingScales
        }


        private static void GeneratePatchPattern(XORShift128 Random, int numScales)
        {
            Random.Shift(1 + 2 * numScales);
        }
        private static int GenerateTwoLines(XORShift128 Random, float tailLengthIVar, LizardType type, float startPoint, float maxLength, float lengthExponent, float spacingScale)
        {
            float bodyAndTailLength = GetBodyAndTailLength(type, tailLengthIVar);
            float linesLength = Mathf.Lerp(startPoint + 0.1f, Mathf.Max(startPoint + 0.2f, maxLength), Mathf.Pow(Random.Value, lengthExponent));
            float totLength = linesLength * bodyAndTailLength;
            float spacing = Mathf.Lerp(2f, 9f, Random.Value);
            if (type == LizardType.Blue)
            {
                spacing = 2f;
            }
            spacing *= spacingScale;
            int numOfScales = (int)(totLength / spacing);
            if (numOfScales < 3)
            {
                numOfScales = 3;
            }
            return numOfScales; // * 2;
        }
        private static int GenerateSegments(XORShift128 Random, float tailLengthIVar, LizardType type, float startPoint, float maxLength, float lengthExponent)
        {
            float bodyAndTailLength = GetBodyAndTailLength(type, tailLengthIVar);
            float linesLength = Mathf.Lerp(startPoint + 0.1f, Mathf.Max(startPoint + 0.2f, maxLength), Mathf.Pow(Random.Value, lengthExponent));
            float totLength = linesLength * bodyAndTailLength;
            float spacing = Mathf.Lerp(7f, 14f, Random.Value);
            if (type == LizardType.Red)
            {
                spacing = Mathf.Min(spacing, 11f) * 0.75f;
            }
            int numOfSegments = Math.Max(3, (int)(totLength / spacing));
            int scalesPerSegment = Random.Range(1, 4) * 2;
            return numOfSegments * scalesPerSegment;
        }

        private static int MinGenTwoLines(LizardType type, float startPoint, float spacingScale)
        {
            float totLength = (startPoint + 0.1f) * GetBodyAndTailLength(type, 0.6f);
            float spacing = (type == LizardType.Blue ? 2f : 9f) * spacingScale;
            int numOfScales = (int)(totLength / spacing);
            if (numOfScales < 3) numOfScales = 3;
            return numOfScales;
        }
        private static int MaxGenTwoLines(LizardType type, float startPoint, float maxLength, float spacingScale)
        {
            float totLength = Mathf.Max(startPoint + 0.2f, maxLength) * GetBodyAndTailLength(type, 1.4f);
            float spacing = 2f * spacingScale;
            int numOfScales = (int)(totLength / spacing);
            if (numOfScales < 3) numOfScales = 3;
            return numOfScales;
        }
        private static int MinGenSegments(LizardType type, float startPoint)
        {
            float totLength = (startPoint + 0.1f) * GetBodyAndTailLength(type, 0.6f);
            float spacing = (type == LizardType.Red ? 11f * 0.75f : 14f);
            int numOfSegments = Math.Max(3, (int)(totLength / spacing));
            return numOfSegments * 2;
        }
        private static int MaxGenSegments(LizardType type, float startPoint, float maxLength)
        {
            float totLength = Mathf.Max(startPoint + 0.2f, maxLength) * GetBodyAndTailLength(type, 1.4f);
            float spacing = (type == LizardType.Red ? 7f * 0.75f : 7f);
            int numOfSegments = Math.Max(3, (int)(totLength / spacing));
            return numOfSegments * 6;
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

        private static float GetMinBodyAndTailLength(LizardType type) => GetBodyAndTailLength(type, 0.6f);
        private static float GetMaxBodyAndTailLength(LizardType type) => GetBodyAndTailLength(type, 1.4f);


        public struct AntennaeVars
        {
            public AntennaeVars(XORShift128 Random)
            {
                // Get values
                length = Random.Value;
                alpha = length * 0.9f + Random.Value * 0.1f;

                // Offset for next
                int segments = Mathf.FloorToInt(Mathf.Lerp(3f, 8f, Mathf.Pow(length, Mathf.Lerp(1f, 6f, length))));
                Random.Shift(2 * segments);
            }

            public float length;
            public float alpha;
        }

        public struct AxolotlGillsVars
        {
            public AxolotlGillsVars(XORShift128 Random)
            {
                rigor = Random.Value;

                Random.Shift();

                graphic = Random.Range(0, 6);
                if (graphic == 2)
                {
                    graphic = Random.Range(0, 6);
                }

                numGills = Random.Range(2, 8);

                // Offset picker
                Random.Shift(2 + 4 * numGills);
            }

            public float rigor;
            public int numGills;
            public int graphic;
        }

        public struct BodyStripesVars
        {
            public BodyStripesVars(XORShift128 Random, float tailLength, LizardType type)
            {
                float bodyAndTailLength = GetBodyAndTailLength(type, tailLength);
                float bodyRange = Mathf.Lerp(0.4f, 0.8f, Random.Value) * bodyAndTailLength;
                float num4 = Mathf.Lerp(5f, 12f, Random.Value) * 1.5f;
                numScales = (int)(bodyRange / num4);
                if (numScales > 3) numScales = 3;
            }

            public int numScales;

            public static int MaxNumScales(LizardType type) => (int)((0.8f * GetMaxBodyAndTailLength(type)) / (5f * 1.5f));
        }

        public struct BumpHawkVars
        {
            public BumpHawkVars(XORShift128 Random, float tailLength, LizardType type)
            {
                // Get body and tail length
                float bodyAndTailLength = GetBodyAndTailLength(type, tailLength);

                // Get output stuff
                colored = Random.Value < 0.5f;

                float spacing;
                if (colored)
                {
                    spacing = Mathf.Lerp(3f, 8f, Mathf.Pow(Random.Value, 0.7f));
                    spineLength = Mathf.Lerp(0.3f, 0.7f, Random.Value) * bodyAndTailLength;
                }
                else
                {
                    spacing = Mathf.Lerp(6f, 12f, Mathf.Pow(Random.Value, 0.5f));
                    spineLength = Mathf.Lerp(0.3f, 0.9f, Random.Value) * bodyAndTailLength;
                }
                Random.Shift(3);
                numBumps = (int)(spineLength / spacing);
            }

            public static float MinSpineLength(LizardType type) => 0.3f * GetMinBodyAndTailLength(type);
            public static float MaxSpineLength(LizardType type) => 0.9f * GetMaxBodyAndTailLength(type);
            public static int MinNumBumps(LizardType type) => (int)(MinSpineLength(type) / 12f);
            public static int MaxNumBumps(LizardType type) => (int)(0.7f * GetMaxBodyAndTailLength(type) / 3f);

            public bool colored;
            public float spineLength;
            public int numBumps;
        }

        public struct JumpRingsVars();

        public struct LongHeadScalesVars
        {
            public LongHeadScalesVars(XORShift128 Random, LizardType type)
            {
                // Some values
                rigor = Random.Value;
                Random.Shift(2); // GenerateTwoHorns()

                float size = Mathf.Pow(Random.Value, 0.7f) * GetParamHeadSize(type);

                // Colored?
                colored = Random.Value < 0.5f && type != LizardType.White;

                Random.Shift();
                if (size < 0.5f) Random.Shift();

                if (size < 0.2f && type != LizardType.White)
                {
                    colored = true;
                }
                if (type == LizardType.Black)
                {
                    colored = false;
                }

                // Length and width and stuff
                float randomWidth = Random.Value;
                Random.Shift();
                length = Mathf.Lerp(5f, 35f, size);
                width = Mathf.Lerp(0.65f, 1.2f, randomWidth * size);
            }

            public float rigor;
            public bool colored;
            public float length;
            public float width;
        }

        public struct LongShoulderScalesVars
        {
            public LongShoulderScalesVars(XORShift128 Random, float tailLength, LizardType type)
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
                            numScales = GenerateTwoLines(Random, tailLength, type, 0.07f, 1f, 1.5f, 3f);
                            break;
                        }
                    case LizardBodyScaleType.Segments:
                        {
                            // Segments, GenerateSegments(0.1f, 0.8f, 5f)
                            numScales = GenerateSegments(Random, tailLength, type, 0.1f, 0.8f, 5f);
                            break;
                        }
                }

                // Calculate some stuff for the scale
                float generalScale = Mathf.Lerp(1f, 1f / Mathf.Lerp(1f, numScales, Mathf.Pow(Random.Value, 2f)), 0.5f);
                minSize = Mathf.Lerp(5f, 15f, Random.Value) * generalScale;
                maxSize = Mathf.Lerp(minSize, 35f, Mathf.Pow(Random.Value, 0.5f)) * generalScale;
                if (type == LizardType.Red)
                {
                    // generalScale = Mathf.Max(0.5f, generalScale);
                    minSize = Mathf.Max(10f, minSize) * 1.2f;
                    maxSize = Mathf.Max(25f, maxSize) * 1.2f;
                }

                colored = type == LizardType.Green || type == LizardType.Red || Random.Value < 0.4f;

                if (Random.Value < 0.1f)
                {
                    graphic = Random.Range(0, 7);
                }
                else
                {
                    graphic = Random.Range(3, 6);
                }

                // Offset for any other code
                if (type == LizardType.Pink && Random.Value < 0.25f) graphic = 0;
                if (type == LizardType.Red)
                {
                    graphic = 0;
                    if (Random.Value < 0.3f)
                    {
                        graphic = 3;
                        Random.Shift();
                    }
                }
                //float sizeSkewExponent = Mathf.Lerp(0.1f, 0.9f, Random.Value);
                Random.Shift();
            }

            public static int MinNumScales(LizardType type) => Math.Min(4, Math.Min(MinGenTwoLines(type, 0.07f, 3f), MinGenSegments(type, 0.1f)));
            public static int MaxNumScales(LizardType type) => Math.Max(14, Math.Max(MaxGenTwoLines(type, 0.07f, 1f, 3f), MaxGenSegments(type, 0.1f, 0.8f)));

            public LizardBodyScaleType scaleType;
            public int numScales;
            public bool colored;
            public float minSize;
            public float maxSize;
            public int graphic;
        }

        public struct ShortBodyScalesVars
        {
            public ShortBodyScalesVars(XORShift128 Random, float tailLength, LizardType type)
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
                            numScales = GenerateTwoLines(Random, tailLength, type, 0.1f, 1f, 1.5f, 1f);
                            break;
                        }
                    case LizardBodyScaleType.Segments:
                        {
                            // Segments, GenerateSegments(0.1f, 0.9f, (lGraphics.lizard.Template.type == CreatureTemplate.Type.PinkLizard) ? 1.5f : 0.6f)
                            numScales = GenerateSegments(Random, tailLength, type, 0.1f, 0.9f, type == LizardType.Pink ? 1.5f : 0.6f);
                            break;
                        }
                }
            }

            public static int MinNumScales(LizardType type) => Math.Min(4, Math.Min(MinGenTwoLines(type, 0.1f, 1f), MinGenSegments(type, 0.1f)));
            public static int MaxNumScales(LizardType type) => Math.Max(14, Math.Max(MaxGenTwoLines(type, 0.1f, 1f, 1f), MaxGenSegments(type, 0.1f, 0.9f)));

            public LizardBodyScaleType scaleType;
            public int numScales;
        }

        public struct SnowAccumulationVars
        {
            public SnowAccumulationVars(XORShift128 Random) => Random.Shift(48);
            // no funny things to return :(
        }

        public struct SpineSpikesVars
        {
            public SpineSpikesVars(XORShift128 Random, float tailLength, LizardType type)
            {
                float bodyAndTailLength = GetBodyAndTailLength(type, tailLength);

                float bumpDiv = Mathf.Lerp(5f, 8f, Mathf.Pow(Random.Value, 0.7f));
                spineLength = Mathf.Lerp(0.2f, 0.95f, Random.Value) * bodyAndTailLength;
                numScales = (int)(spineLength / bumpDiv);

                Random.Shift(4);
                if (type != LizardType.Blue) Random.Shift();
                // Random.value in the else-if chain is guaranteed to be called exactly once if it's not a blue lizor
                // if it's not a green lizor, it might fail the random value check, but then it's still not a green lizor in the next else if

                graphic = Random.Range(0, 5);
                if (graphic == 1) graphic = 0;
                if (graphic == 4) graphic = 3;
                else if (graphic != 3 || Random.Value >= 0.5f) Random.Shift();

                // Will need to redo this section if need sprite (graphic) because it is a big if-else chain to set graphic depending on lizard type
                if (type == LizardType.Pink && Random.Value < 0.7f) graphic = 0;
                else if (type == LizardType.Green && Random.Value < 0.5f) graphic = 3;

                Random.Shift();
                if (type == LizardType.Pink) Random.Shift();
                else if (type == LizardType.Green && Random.Value >= 0.5f) Random.Shift();
            }

            public static float MinSpineLength(LizardType type) => 0.2f * GetMinBodyAndTailLength(type);
            public static float MaxSpineLength(LizardType type) => 0.95f * GetMaxBodyAndTailLength(type);
            public static int MinNumScales(LizardType type) => (int)(MinSpineLength(type) / 8f);
            public static int MaxNumScales(LizardType type) => (int)(MaxSpineLength(type) / 5f);

            public float spineLength;
            public int numScales;
            public int graphic;
        }

        public struct TailFinVars
        {
            public TailFinVars(XORShift128 Random, float tailLength, LizardType type)
            {
                float bodyAndTailLength = GetBodyAndTailLength(type, tailLength);

                float bumpDiv = Mathf.Lerp(4f, 7f, Mathf.Pow(Random.Value, 0.7f));
                spineLength = Option.ClampedRandomVariation(0.5f, 0.17f, 0.5f, Random) * bodyAndTailLength;
                undersideSize = Mathf.Lerp(0.3f, 0.9f, Random.Value);
                Random.Shift(3);
                graphic = Random.Range(0, 6);
                if (type == LizardType.Red)
                {
                    graphic = 0;
                    // NOTE: if lizard has LongBodyScales, it uses the same graphic. However, that's a lot of work so I'm not going to do that lmao
                    spineLength = Option.ClampedRandomVariation(0.3f, 0.17f, 0.5f, Random) * bodyAndTailLength;
                }
                numScales = (int)(spineLength / bumpDiv);
                spineScaleX = Mathf.Lerp(1f, 2f, Random.Value);
                if (graphic == 3 && Random.Value < 0.5f)
                {
                    // do nothing lol
                }
                else if (graphic != 0) Random.Shift();
                colored = Random.Value > 0.33333334f;
            }

            public static float MinSpineLength(LizardType type) => (type == LizardType.Red ? 0.3f - 0.17f : 0.5f - 0.17f) * GetMinBodyAndTailLength(type);
            public static float MaxSpineLength(LizardType type) => (type == LizardType.Red ? 0.3f + 0.17f : 0.5f + 0.17f) * GetMaxBodyAndTailLength(type);
            public static int MinNumScales(LizardType type) => (int)(MinSpineLength(type) / 7f);
            public static int MaxNumScales(LizardType type) => (int)(MaxSpineLength(type) / 4f);

            public float spineLength;
            public float undersideSize;
            public float spineScaleX;
            public int numScales;
            public bool colored;
            public int graphic;
        }

        public struct TailGeckoScalesVars
        {
            public TailGeckoScalesVars(XORShift128 Random, float tailColor)
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

            public int rows;
            public int lines;
        }

        public struct TailTuftVars
        {
            public TailTuftVars(XORShift128 Random, float tailLength, LizardType type)
            {

                if (Random.Value < 0.14285715f || Random.Value < 0.9f && type == LizardType.Blue || type == LizardType.Red || type == LizardType.Zoop)
                {
                    scaleType = LizardBodyScaleType.TwoLines;

                    if (type == LizardType.Blue || type == LizardType.Red)
                    {
                        // GenerateTwoLines(0f, (lGraphics.lizard.Template.type == CreatureTemplate.Type.RedLizard) ? 0.3f : 0.7f, 1f, 3f);
                        numScales = GenerateTwoLines(Random, tailLength, type, 0f, type == LizardType.Red ? 0.3f : 0.7f, 1f, 3f);
                    }
                    else
                    {
                        // GenerateTwoLines(0f, 0.4f, 1.2f, 1.3f)
                        numScales = GenerateTwoLines(Random, tailLength, type, 0f, 0.4f, 1.2f, 1.3f);
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

            public static int MinNumScales(LizardType type) =>
                (type == LizardType.Blue || type == LizardType.Red) ? MinGenTwoLines(type, 0f, 3f) : MinGenTwoLines(type, 0f, 1.3f);
            public static int MaxNumScales(LizardType type) =>
                Math.Max(
                    (type == LizardType.Blue || type == LizardType.Red) ? MaxGenTwoLines(type, 0f, type == LizardType.Red ? 0.3f : 0.7f, 3f) : MaxGenTwoLines(type, 0f, 0.4f, 1.3f)
                    , 7);

            public LizardBodyScaleType scaleType;
            public int numScales;
        }

        public struct WhiskersVars
        {
            public WhiskersVars(XORShift128 Random)
            {
                // Number of whiskers
                numWhiskers = Random.Range(3, 5);

                // Offset for next usage
                Random.Shift(numWhiskers * 9 + (numWhiskers - 1) * 4);
            }

            public int numWhiskers;
        }

        public struct WingScalesVars
        {
            public WingScalesVars(XORShift128 Random)
            {
                numScales = Random.Value < 0.2f ? 3 : 2;

                if (Random.Value >= 0.4f) Random.Shift();

                float sturdy = Random.Value;
                Random.Shift();
                scaleLength = Mathf.Lerp(5f, 40f, Mathf.Pow(Random.Value, 0.75f + 1.25f * sturdy));

                // More offsetting
                Random.Shift(2);
                Random.Shift(2 * numScales);
            }

            public int numScales;
            public float scaleLength;
        }
    }
}
