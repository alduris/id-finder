using static FinderMod.Test.TestCases;

namespace FinderMod.Test
{
    internal static class LizardTests
    {
        /// <summary>
        /// Tests different test cases per lizard and throws an exception if something is incorrect.
        /// </summary>
        public static void TestAllLizards()
        {
            int i;

            // Pink lizor
            // Body pattern 1: N/A, 2: BumpHawk, 3: LongShoulderScales, 4: ShortBodyScales, 5: SpineSpikes
            // TailTuft
            // LongHeadScales
            i = 9616;
            TestCase("Pink Lizard Variations", i++, [3, 0, 0]); // 9616
            TestCase("Pink Lizard Variations", i++, [1, 1, 0]);
            TestCase("Pink Lizard Variations", i++, [1, 1, 1]);
            TestCase("Pink Lizard Variations", i++, [1, 1, 1]);
            TestCase("Pink Lizard Variations", i++, [1, 0, 1]); // 9620
            TestCase("Pink Lizard Variations", i++, [3, 1, 0]);
            TestCase("Pink Lizard Variations", i++, [1, 1, 1]);
            TestCase("Pink Lizard Variations", i++, [4, 1, 0]);
            TestCase("Pink Lizard Variations", i++, [1, 1, 1]);
            TestCase("Pink Lizard Variations", i++, [1, 0, 1]); // 9625
            TestCase("Pink Lizard Variations", i++, [3, 0, 0]);
            TestCase("Pink Lizard Variations", i++, [3, 0, 0]);
            TestCase("Pink Lizard Variations", i++, [3, 0, 0]);
            TestCase("Pink Lizard Variations", i++, [1, 0, 1]);
            TestCase("Pink Lizard Variations", i++, [3, 1, 0]); // 9630
            TestCase("Pink Lizard Variations", i++, [1, 1, 1]);
            TestCase("Pink Lizard Variations", i++, [1, 1, 1]);
            TestCase("Pink Lizard Variations", i++, [3, 1, 0]);
            TestCase("Pink Lizard Variations", i++, [3, 0, 0]);
            TestCase("Pink Lizard Variations", i++, [3, 1, 0]); // 9635

            // Green lizor
            // Body pattern 1: N/A, 2: BumpHawk, 3: ShortBodyScales, 4: SpineSpikes
            // TailTuft
            // LongShoulderScales
            // LongHeadScales
            i = 8653;
            TestCase("Green Lizard Variations", i++, [4, 1, 0, 0]); // 8653
            TestCase("Green Lizard Variations", i++, [4, 0, 0, 0]);
            TestCase("Green Lizard Variations", i++, [3, 1, 0, 0]); // 8655
            TestCase("Green Lizard Variations", i++, [4, 1, 0, 0]);
            TestCase("Green Lizard Variations", i++, [4, 0, 1, 0]);
            TestCase("Green Lizard Variations", i++, [4, 1, 0, 0]);
            TestCase("Green Lizard Variations", i++, [4, 0, 0, 0]);
            TestCase("Green Lizard Variations", i++, [4, 1, 0, 0]); // 8660
            TestCase("Green Lizard Variations", i++, [3, 1, 0, 0]);
            TestCase("Green Lizard Variations", i++, [4, 0, 0, 0]);
            TestCase("Green Lizard Variations", i++, [3, 1, 0, 0]);
            TestCase("Green Lizard Variations", i++, [4, 1, 0, 0]);
            TestCase("Green Lizard Variations", i++, [4, 1, 0, 0]); // 8665
            TestCase("Green Lizard Variations", i++, [4, 0, 1, 0]);
            TestCase("Green Lizard Variations", i++, [4, 1, 0, 1]);
            TestCase("Green Lizard Variations", i++, [1, 1, 1, 0]);
            TestCase("Green Lizard Variations", i++, [4, 1, 0, 0]);
            TestCase("Green Lizard Variations", i++, [4, 1, 0, 0]); // 8670
            TestCase("Green Lizard Variations", i++, [4, 1, 0, 0]);
            TestCase("Green Lizard Variations", i++, [4, 1, 0, 0]);

            // Blue lizor
            // Body pattern 1: N/A, 2: BumpHawk, 3: LongShoulderScales, 4: ShortBodyScales, 5: SpineSpikes
            // TailTuft
            // LongHeadScales
            i = 2657;
            TestCase("Blue Lizard Variations", i++, [1, 1, 0]); // 2657
            TestCase("Blue Lizard Variations", i++, [1, 1, 0]);
            TestCase("Blue Lizard Variations", i++, [4, 1, 0]);
            TestCase("Blue Lizard Variations", i++, [4, 1, 0]); // 2660
            TestCase("Blue Lizard Variations", i++, [4, 1, 0]);
            TestCase("Blue Lizard Variations", i++, [4, 1, 0]);
            TestCase("Blue Lizard Variations", i++, [4, 1, 0]);
            TestCase("Blue Lizard Variations", i++, [4, 1, 0]);
            TestCase("Blue Lizard Variations", i++, [1, 1, 1]); // 2665
            TestCase("Blue Lizard Variations", i++, [1, 1, 1]);
            TestCase("Blue Lizard Variations", i++, [1, 1, 1]);
            TestCase("Blue Lizard Variations", i++, [4, 1, 0]);
            TestCase("Blue Lizard Variations", i++, [2, 1, 0]);
            TestCase("Blue Lizard Variations", i++, [4, 1, 0]); // 2670
            TestCase("Blue Lizard Variations", i++, [4, 1, 0]);
            TestCase("Blue Lizard Variations", i++, [2, 1, 0]);
            TestCase("Blue Lizard Variations", i++, [1, 1, 1]);
            TestCase("Blue Lizard Variations", i++, [4, 1, 1]);
            TestCase("Blue Lizard Variations", i++, [4, 1, 0]); // 2675
            TestCase("Blue Lizard Variations", i++, [4, 1, 0]);

            TestCase("Blue Lizard Variations", 1013, [4, 0, 1]); // non-TailTuft case

            // Yellow lizor
            // Body pattern 1: N/A, 2: BumpHawk, 3: LongShoulderScales, 4: ShortBodyScales, 5: SpineSpikes
            // Length
            // Alpha
            i = 6639;
            TestCase("Yellow Lizard Variations", i++, [1, 1, 0.6977374f, 0.6465018f]); // 6639
            TestCase("Yellow Lizard Variations", i++, [4, 1, 0.2383887f, 0.234062f]); // 6640
            TestCase("Yellow Lizard Variations", i++, [1, 1, 0.3045831f, 0.2996762f]);
            TestCase("Yellow Lizard Variations", i++, [4, 1, 0.9072969f, 0.8427213f]);
            TestCase("Yellow Lizard Variations", i++, [1, 1, 0.9137725f, 0.8744563f]);
            TestCase("Yellow Lizard Variations", i++, [1, 1, 0.9844799f, 0.9619467f]);
            TestCase("Yellow Lizard Variations", i++, [4, 1, 0.5430859f, 0.5697154f]); // 6645
            TestCase("Yellow Lizard Variations", i++, [4, 1, 0.1237443f, 0.1583027f]);
            TestCase("Yellow Lizard Variations", i++, [4, 1, 0.08149982f, 0.154146f]);
            TestCase("Yellow Lizard Variations", i++, [4, 1, 0.4811222f, 0.4698509f]);
            TestCase("Yellow Lizard Variations", i++, [4, 1, 0.1415253f, 0.2110136f]);
            TestCase("Yellow Lizard Variations", i++, [5, 0, 0.0889603f, 0.1339938f]); // 6650
            TestCase("Yellow Lizard Variations", i++, [4, 0, 0.5217251f, 0.5504261f]);
            TestCase("Yellow Lizard Variations", i++, [3, 1, 0.7139561f, 0.7308314f]);
            TestCase("Yellow Lizard Variations", i++, [1, 1, 0.1891064f, 0.2540206f]);
            TestCase("Yellow Lizard Variations", i++, [1, 1, 0.4175919f, 0.4298473f]);
            TestCase("Yellow Lizard Variations", i++, [4, 1, 0.2775053f, 0.2820467f]); // 6655
            TestCase("Yellow Lizard Variations", i++, [1, 1, 0.7116777f, 0.726635f]);
            TestCase("Yellow Lizard Variations", i++, [4, 1, 0.2983792f, 0.3381503f]);
            TestCase("Yellow Lizard Variations", i++, [4, 1, 0.9622838f, 0.8662739f]);

            // Zonkerdoodle :D
            // Body pattern 1: N/A, 2: BumpHawk, 3: LongHeadScales, 4: LongShoulderScales, 5: ShortBodyScales
            // TailTuft
            i = 7116;
            TestCase("White Lizard Variations", i++, [2, 0]); // 7116
            TestCase("White Lizard Variations", i++, [1, 0]);
            TestCase("White Lizard Variations", i++, [2, 1]);
            TestCase("White Lizard Variations", i++, [2, 0]);
            TestCase("White Lizard Variations", i++, [2, 0]); // 7120
            TestCase("White Lizard Variations", i++, [2, 1]);
            TestCase("White Lizard Variations", i++, [2, 0]);
            TestCase("White Lizard Variations", i++, [1, 1]);
            TestCase("White Lizard Variations", i++, [2, 1]);
            TestCase("White Lizard Variations", i++, [2, 0]); // 7125
            TestCase("White Lizard Variations", i++, [2, 0]);
            TestCase("White Lizard Variations", i++, [1, 1]);
            TestCase("White Lizard Variations", i++, [1, 1]);
            TestCase("White Lizard Variations", i++, [2, 0]);
            TestCase("White Lizard Variations", i++, [5, 0]); // 7130
            TestCase("White Lizard Variations", i++, [1, 1]);
            TestCase("White Lizard Variations", i++, [2, 0]);
            TestCase("White Lizard Variations", i++, [5, 0]);
            TestCase("White Lizard Variations", i++, [2, 1]);
            TestCase("White Lizard Variations", i++, [1, 1]); // 7135

            // Red lizor
            // Body pattern 1: N/A, 2: BumpHawk, 3: extra LongShoulderScales, 4: ShortBodyScales, 5: extra SpineSpikes
            // Tail pattern 1: TailTuft, 2: TailFin
            // Extra TailTuft
            // LongHeadScales
            i = 5058;
            TestCase("Red Lizard Variations", i++, [3, 1, 0, 0]); // 5058
            TestCase("Red Lizard Variations", i++, [3, 2, 0, 0]);
            TestCase("Red Lizard Variations", i++, [3, 2, 0, 0]); // 5060
            TestCase("Red Lizard Variations", i++, [3, 1, 0, 0]);
            TestCase("Red Lizard Variations", i++, [3, 1, 0, 0]);
            TestCase("Red Lizard Variations", i++, [3, 1, 0, 0]);
            TestCase("Red Lizard Variations", i++, [2, 1, 0, 0]);
            TestCase("Red Lizard Variations", i++, [3, 2, 0, 0]); // 5065
            TestCase("Red Lizard Variations", i++, [3, 2, 0, 0]);
            TestCase("Red Lizard Variations", i++, [3, 1, 0, 0]);
            TestCase("Red Lizard Variations", i++, [3, 2, 0, 0]);
            TestCase("Red Lizard Variations", i++, [3, 2, 0, 0]);
            TestCase("Red Lizard Variations", i++, [3, 1, 0, 0]); // 5070
            TestCase("Red Lizard Variations", i++, [3, 2, 0, 0]);
            TestCase("Red Lizard Variations", i++, [3, 2, 0, 0]);
            TestCase("Red Lizard Variations", i++, [3, 1, 0, 0]);
            TestCase("Red Lizard Variations", i++, [3, 1, 0, 0]);
            TestCase("Red Lizard Variations", i++, [3, 1, 0, 0]); // 5075
            TestCase("Red Lizard Variations", i++, [3, 1, 0, 0]);
            TestCase("Red Lizard Variations", i++, [3, 2, 0, 0]);

            // Black lizor
            // Body pattern 1: N/A, 2: BumpHawk, 3: LongShoulderScales, 4: ShortBodyScales, 5: SpineSpikes
            // TailTuft
            // LongHeadScales
            // # whiskers
            i = 1299;
            TestCase("Black Lizard Variations", i++, [5, 0, 0, 3]); // 1299
            TestCase("Black Lizard Variations", i++, [5, 0, 0, 4]); // 1300
            TestCase("Black Lizard Variations", i++, [5, 0, 0, 4]);
            TestCase("Black Lizard Variations", i++, [5, 0, 0, 4]);
            TestCase("Black Lizard Variations", i++, [5, 0, 1, 4]);
            TestCase("Black Lizard Variations", i++, [5, 0, 0, 3]);
            TestCase("Black Lizard Variations", i++, [5, 0, 0, 4]); // 1305
            TestCase("Black Lizard Variations", i++, [5, 0, 0, 4]);
            TestCase("Black Lizard Variations", i++, [5, 0, 0, 3]);
            TestCase("Black Lizard Variations", i++, [2, 1, 0, 3]);
            TestCase("Black Lizard Variations", i++, [5, 0, 0, 3]);
            TestCase("Black Lizard Variations", i++, [5, 0, 1, 3]); // 1310
            TestCase("Black Lizard Variations", i++, [5, 0, 0, 4]);
            TestCase("Black Lizard Variations", i++, [5, 0, 0, 4]);
            TestCase("Black Lizard Variations", i++, [5, 0, 0, 3]);
            TestCase("Black Lizard Variations", i++, [5, 0, 0, 4]);
            TestCase("Black Lizard Variations", i++, [5, 1, 1, 3]); // 1315
            TestCase("Black Lizard Variations", i++, [5, 0, 1, 4]);
            TestCase("Black Lizard Variations", i++, [5, 0, 0, 4]);
            TestCase("Black Lizard Variations", i++, [2, 0, 0, 4]);

            // Axolotls :3
            // Body pattern 1: N/A, 2: BumpHawk, 3: SpineSpikes
            // Dark
            i = 4097;
            TestCase("Salamander Variations", i++, [1, 1]); // 4097
            TestCase("Salamander Variations", i++, [1, 1]);
            TestCase("Salamander Variations", i++, [2, 1]);
            TestCase("Salamander Variations", i++, [1, 1]); // 4100
            TestCase("Salamander Variations", i++, [1, 1]);
            TestCase("Salamander Variations", i++, [1, 0]);
            TestCase("Salamander Variations", i++, [1, 0]);
            TestCase("Salamander Variations", i++, [1, 0]);
            TestCase("Salamander Variations", i++, [1, 1]); // 4105
            TestCase("Salamander Variations", i++, [1, 0]);
            TestCase("Salamander Variations", i++, [1, 0]);
            TestCase("Salamander Variations", i++, [1, 0]);
            TestCase("Salamander Variations", i++, [3, 0]);
            TestCase("Salamander Variations", i++, [1, 0]); // 4110
            TestCase("Salamander Variations", i++, [3, 0]);
            TestCase("Salamander Variations", i++, [1, 0]);
            TestCase("Salamander Variations", i++, [1, 0]);
            TestCase("Salamander Variations", i++, [1, 0]);
            TestCase("Salamander Variations", i++, [1, 0]); // 4115
            TestCase("Salamander Variations", i++, [1, 1]);

            // Cyan lizor
            // WingScales
            // TailGeckoScales (0) or TailTuft (1)
            i = 8689;
            TestCase("Cyan Lizard Variations", i++, [1, 0]); // 8689
            TestCase("Cyan Lizard Variations", i++, [1, 1]); // 8690
            TestCase("Cyan Lizard Variations", i++, [0, 0]);
            TestCase("Cyan Lizard Variations", i++, [1, 1]);
            TestCase("Cyan Lizard Variations", i++, [1, 0]);
            TestCase("Cyan Lizard Variations", i++, [0, 1]);
            TestCase("Cyan Lizard Variations", i++, [0, 0]); // 8695
            TestCase("Cyan Lizard Variations", i++, [1, 0]);
            TestCase("Cyan Lizard Variations", i++, [1, 0]);
            TestCase("Cyan Lizard Variations", i++, [1, 0]);
            TestCase("Cyan Lizard Variations", i++, [1, 0]);
            TestCase("Cyan Lizard Variations", i++, [0, 0]); // 8700
            TestCase("Cyan Lizard Variations", i++, [0, 1]);
            TestCase("Cyan Lizard Variations", i++, [1, 0]);
            TestCase("Cyan Lizard Variations", i++, [1, 0]);
            TestCase("Cyan Lizard Variations", i++, [1, 1]);
            TestCase("Cyan Lizard Variations", i++, [1, 0]); // 8705
            TestCase("Cyan Lizard Variations", i++, [1, 0]);
            TestCase("Cyan Lizard Variations", i++, [0, 1]);
            TestCase("Cyan Lizard Variations", i++, [1, 0]);

            // Caramel lizor
            // Body pattern 1: BodyStripes, 2: BumpHawk, 3: LongShoulderScales, 4: ShortBodyScales, 5: SpineSpikes
            // TailTuft
            // LongHeadScales
            i = 2327;
            TestCase("Caramel Lizard Variations", i++, [1, 0, 0]); // 2327
            TestCase("Caramel Lizard Variations", i++, [2, 0, 1]);
            TestCase("Caramel Lizard Variations", i++, [1, 1, 0]);
            TestCase("Caramel Lizard Variations", i++, [2, 1, 1]); // 2330
            TestCase("Caramel Lizard Variations", i++, [1, 1, 0]);
            TestCase("Caramel Lizard Variations", i++, [1, 1, 0]);
            TestCase("Caramel Lizard Variations", i++, [1, 1, 0]);
            TestCase("Caramel Lizard Variations", i++, [2, 1, 1]);
            TestCase("Caramel Lizard Variations", i++, [1, 1, 0]); // 2335
            TestCase("Caramel Lizard Variations", i++, [2, 1, 0]);
            TestCase("Caramel Lizard Variations", i++, [2, 1, 1]);
            TestCase("Caramel Lizard Variations", i++, [1, 1, 0]);
            TestCase("Caramel Lizard Variations", i++, [1, 1, 0]);
            TestCase("Caramel Lizard Variations", i++, [2, 1, 0]); // 2340
            TestCase("Caramel Lizard Variations", i++, [2, 1, 1]);
            TestCase("Caramel Lizard Variations", i++, [2, 1, 0]);
            TestCase("Caramel Lizard Variations", i++, [1, 0, 0]);
            TestCase("Caramel Lizard Variations", i++, [1, 1, 0]);
            TestCase("Caramel Lizard Variations", i++, [1, 1, 0]); // 2345
            TestCase("Caramel Lizard Variations", i++, [1, 0, 0]);

            i = 3643;
            TestCase("Caramel Lizard Variations", i++, [2, 1, 1, 0.09750497f, 0.5240445f, 0.08607189f, 0.8690302f, 0.9866259f]); // 3643
            TestCase("Caramel Lizard Variations", i++, [2, 1, 1, 0.08401366f, 0.5463953f, 0.1136334f,  0.5311269f, 0.8838773f]);
            TestCase("Caramel Lizard Variations", i++, [2, 1, 1, 0.100222f,   0.2303146f, 0.08323372f, 0.4436544f, 0.7861173f]); // 3645
            TestCase("Caramel Lizard Variations", i++, [1, 0, 0, 0.09900896f, 0.5593776f, 0.1224537f,  0.4353313f, 0.8537061f]);
            TestCase("Caramel Lizard Variations", i++, [1, 1, 0, 0.1179478f,  0.5501207f, 0.09519099f, 0.6167142f, 0.877188f]);
            TestCase("Caramel Lizard Variations", i++, [1, 0, 0, 0.08263152f, 0.5530499f, 0.1173399f,  0.7275768f, 0.8014214f]);
            TestCase("Caramel Lizard Variations", i++, [1, 1, 0, 0.1037543f,  0.5256527f, 0.1237949f,  0.4187644f, 0.765835f]);
            TestCase("Caramel Lizard Variations", i++, [2, 0, 1, 0.09812989f, 0.5595561f, 0.1077282f,  0.8374706f, 0.8900954f]); // 3650
            TestCase("Caramel Lizard Variations", i++, [2, 1, 1, 0.09782159f, 0.5287414f, 0.1101693f,  0.57503f,   0.8202078f]);
            TestCase("Caramel Lizard Variations", i++, [4, 1, 0, 0.1012788f,  0.4392435f, 0.1093942f,  0.3711579f, 0.7739241f]);
            TestCase("Caramel Lizard Variations", i++, [2, 0, 1, 0.1246946f,  0.5534877f, 0.1026088f,  0.4059989f, 0.8317223f]);
            TestCase("Caramel Lizard Variations", i++, [1, 1, 1, 0.09837732f, 0.5527837f, 0.1037705f,  0.4803292f, 0.7280923f]);
            TestCase("Caramel Lizard Variations", i++, [1, 1, 0, 0.1061774f,  0.5571561f, 0.1162217f,  0.8423144f, 0.8250072f]); // 3655
            TestCase("Caramel Lizard Variations", i++, [1, 1, 0, 0.0936344f,  0.486583f,  0.08953451f, 0.3126313f, 0.7051101f]);
            TestCase("Caramel Lizard Variations", i++, [2, 0, 1, 0.09998321f, 0.5585134f, 0.09829382f, 0.3067564f, 0.7036517f]);
            TestCase("Caramel Lizard Variations", i++, [2, 1, 1, 0.1075257f,  0.5081916f, 0.1161929f,  0.374274f,  0.7324593f]);
            TestCase("Caramel Lizard Variations", i++, [2, 0, 0, 0.08014005f, 0.5582449f, 0.07576215f, 0.5459885f, 0.9971078f]);
            TestCase("Caramel Lizard Variations", i++, [1, 0, 0, 0.1110538f,  0.4757296f, 0.08485165f, 0.437174f,  0.7994829f]); // 3660
            TestCase("Caramel Lizard Variations", i++, [1, 1, 0, 0.07625629f, 0.5366333f, 0.1145932f,  0.8619498f, 0.9808389f]);
            TestCase("Caramel Lizard Variations", i++, [5, 1, 0, 0.09972882f, 0.5722712f, 0.07566615f, 0.7306578f, 0.976003f]);

            // Zoop lizor
            // Body pattern 1: BodyStripes, 2: BumpHawk, 3: LongShoulderScales, 4: ShortBodyScales, 5: SpineSpikes
            // WingScales (0) or SpineSpikes (1, unrelated to above)
            // Extra TailTuft
            // LongHeadScales
            i = 9300;
            TestCase("Zoop Lizard Variations", i++, [5, 1, 0, 0]); // 9300
            TestCase("Zoop Lizard Variations", i++, [1, 1, 1, 1]);
            TestCase("Zoop Lizard Variations", i++, [1, 0, 0, 1]);
            TestCase("Zoop Lizard Variations", i++, [1, 0, 1, 0]);
            TestCase("Zoop Lizard Variations", i++, [5, 1, 0, 0]);
            TestCase("Zoop Lizard Variations", i++, [1, 1, 0, 0]); // 9305
            TestCase("Zoop Lizard Variations", i++, [1, 1, 1, 0]);
            TestCase("Zoop Lizard Variations", i++, [1, 0, 1, 1]);
            TestCase("Zoop Lizard Variations", i++, [4, 1, 0, 1]);
            TestCase("Zoop Lizard Variations", i++, [1, 1, 0, 0]);
            TestCase("Zoop Lizard Variations", i++, [1, 1, 1, 1]); // 9310
            TestCase("Zoop Lizard Variations", i++, [1, 1, 1, 1]);
            TestCase("Zoop Lizard Variations", i++, [1, 0, 1, 0]);
            TestCase("Zoop Lizard Variations", i++, [1, 0, 1, 1]);
            TestCase("Zoop Lizard Variations", i++, [1, 1, 1, 0]);
            TestCase("Zoop Lizard Variations", i++, [1, 1, 1, 1]); // 9315
            TestCase("Zoop Lizard Variations", i++, [1, 1, 0, 1]);
            TestCase("Zoop Lizard Variations", i++, [1, 0, 1, 0]);
            TestCase("Zoop Lizard Variations", i++, [1, 1, 1, 1]);
            TestCase("Zoop Lizard Variations", i++, [1, 0, 1, 0]);

            // Train lizor
            // Body pattern 1: N/A, 2: BumpHawk, 3: extra LongShoulderScales, 4: ShortBodyScales, 5: extra SpineSpikes
            // Tail pattern 1: TailTuft, 2: TailFin
            // Extra TailTuft
            // LongHeadScales
            i = 6307;
            TestCase("Train Lizard Variations", i++, [1, 2, 1, 0]); // 6307
            TestCase("Train Lizard Variations", i++, [1, 2, 1, 1]);
            TestCase("Train Lizard Variations", i++, [1, 1, 1, 1]);
            TestCase("Train Lizard Variations", i++, [3, 1, 0, 0]); // 6310
            TestCase("Train Lizard Variations", i++, [1, 1, 1, 0]);
            TestCase("Train Lizard Variations", i++, [1, 1, 0, 1]);
            TestCase("Train Lizard Variations", i++, [3, 1, 0, 0]);
            TestCase("Train Lizard Variations", i++, [1, 2, 1, 1]);
            TestCase("Train Lizard Variations", i++, [1, 1, 1, 1]); // 6315
            TestCase("Train Lizard Variations", i++, [1, 1, 1, 0]);
            TestCase("Train Lizard Variations", i++, [1, 2, 0, 0]);
            TestCase("Train Lizard Variations", i++, [3, 2, 0, 0]);
            TestCase("Train Lizard Variations", i++, [1, 1, 0, 1]);
            TestCase("Train Lizard Variations", i++, [1, 1, 0, 1]); // 6320
            TestCase("Train Lizard Variations", i++, [1, 2, 1, 0]);
            TestCase("Train Lizard Variations", i++, [1, 1, 1, 1]);
            TestCase("Train Lizard Variations", i++, [1, 1, 0, 0]);
            TestCase("Train Lizard Variations", i++, [1, 1, 1, 0]);
            TestCase("Train Lizard Variations", i++, [1, 2, 1, 0]); // 6325
            TestCase("Train Lizard Variations", i++, [2, 2, 0, 0]);

            // Eel lizor
            // Body pattern 1: LSS and TailFin, 2: SBS and TailFin, 3: SBS and TailTuft
            // Body pattern 2: N/A, 2: BumpHawk, 3: extra LongShoulderScales, 4: ShortBodyScales, 5: SpineSpikes
            // Extra TailTuft
            // LongHeadScales
            i = 5553;
            TestCase("Eel Lizard Variations", i++, [1, 1, 0, 1]); // 5553
            TestCase("Eel Lizard Variations", i++, [2, 1, 1, 1]);
            TestCase("Eel Lizard Variations", i++, [1, 1, 1, 1]); // 5555
            TestCase("Eel Lizard Variations", i++, [1, 1, 0, 0]);
            TestCase("Eel Lizard Variations", i++, [1, 1, 1, 0]);
            TestCase("Eel Lizard Variations", i++, [1, 1, 1, 1]);
            TestCase("Eel Lizard Variations", i++, [1, 2, 1, 0]);
            TestCase("Eel Lizard Variations", i++, [1, 1, 1, 1]); // 5560
            TestCase("Eel Lizard Variations", i++, [1, 1, 1, 1]);
            TestCase("Eel Lizard Variations", i++, [1, 1, 1, 0]);
            TestCase("Eel Lizard Variations", i++, [3, 1, 1, 1]);
            TestCase("Eel Lizard Variations", i++, [1, 1, 1, 0]);
            TestCase("Eel Lizard Variations", i++, [1, 5, 0, 0]); // 5565
            TestCase("Eel Lizard Variations", i++, [2, 1, 0, 1]);
            TestCase("Eel Lizard Variations", i++, [1, 1, 0, 0]);
            TestCase("Eel Lizard Variations", i++, [1, 1, 0, 1]);
            TestCase("Eel Lizard Variations", i++, [1, 1, 1, 1]);
            TestCase("Eel Lizard Variations", i++, [1, 1, 1, 1]); // 5570
            TestCase("Eel Lizard Variations", i++, [1, 1, 1, 0]);
            TestCase("Eel Lizard Variations", i++, [1, 1, 1, 1]);
        }
    }
}
