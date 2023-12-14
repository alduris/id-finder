using static FinderMod.Test.TestCases;

namespace FinderMod.Test
{
    internal class ScavTests
    {
        public static void TestAllScavs()
        {
            // Scavenger colors                              Body H         Body S        Body L       Head H         Head S        Head L       Deco H         Deco S        Deco L       Eye H        Eye L
            TestCase("Scavenger Colors", 6562, new float[] { 0.03927018f,   0.1016362f,   0.5098599f,  0.2257873f,    0.6311247f,   0.174932f,   0.03927018f,   0.7725409f,   0.3268195f,  0.2269589f,  0.65f });
            TestCase("Scavenger Colors", 6563, new float[] { 0.008640707f,  0.7348285f,   0.9946128f,  0.9958526f,    0.9075065f,   0.6405754f,  0.3375438f,    0.121703f,    0.1194663f,  0.3375438f,  0.1f });
            TestCase("Scavenger Colors", 6564, new float[] { 0.01822638f,   0.1707397f,   0.2690711f,  0.01822638f,   0.1707397f,   0.2690711f,  0.1224235f,    0.1537622f,   0.4812316f,  0.1224235f,  0.5f });
            TestCase("Scavenger Colors", 6565, new float[] { 0.08232065f,   0.003095639f, 0.4367136f,  0.08888692f,   0.8277774f,   0.1409272f,  0.08232065f,   0.1403171f,   0.5463402f,  0.1273654f,  0.5f });
            TestCase("Scavenger Colors", 6566, new float[] { 0.0675993f,    0.8267188f,   0.8204471f,  0.0675993f,    0.8267188f,   0.8204471f,  0.5695145f,    0.03915525f,  0.2191887f,  0.5695145f,  0.1f });
            TestCase("Scavenger Colors", 6567, new float[] { 0.004825009f,  0.01663712f,  0.6376163f,  0.01204274f,   0.8460221f,   0.1495164f,  0.004825009f,  0.1918851f,   0.6509836f,  0.1066455f,  0.5f });
            TestCase("Scavenger Colors", 6568, new float[] { 0.006453443f,  0.7565702f,   0.6247283f,  0.006453443f,  0.7565702f,   0.6247283f,  0.006453443f,  0.5203807f,   0.2605545f,  0.04877108f, 0.1f });
            TestCase("Scavenger Colors", 6569, new float[] { 0.07317828f,   0.2883185f,   0.375261f,   0.07317828f,   0.2883185f,   0.375261f,   0.07317828f,   0.4918061f,   0.3427151f,  0.123756f,   0.5f });
            TestCase("Scavenger Colors", 6570, new float[] { 0.0003771067f, 0.304457f,    0.8566296f,  0.0003771067f, 0.2283428f,   0.6334845f,  0.0003771067f, 0.991048f,    0.2044024f,  0.1886323f,  0.1604177f });
            TestCase("Scavenger Colors", 6571, new float[] { 0.09831458f,   0.9205362f,   0.2911566f,  0.09831458f,   0.9205362f,   0.2911566f,  0.09831458f,   0.2903242f,   0.4290386f,  0.1473219f,  0.7004695f });
            TestCase("Scavenger Colors", 6572, new float[] { 0.07098702f,   0.07248832f,  0.7618944f,  0.04472485f,   0.8156875f,   0.1820773f,  0.07098702f,   0.8746629f,   0.4098181f,  0.08820463f, 0.5f });
            TestCase("Scavenger Colors", 6573, new float[] { 0.02779202f,   0.5164752f,   0.6566148f,  0.02779202f,   0.3873564f,   0.6446329f,  0.02779202f,   0.3346376f,   0.2008889f,  0.08105632f, 0.1f });
            TestCase("Scavenger Colors", 6574, new float[] { 0.09869953f,   0.05960437f,  0.4857436f,  0.6317518f,    0.7143267f,   0.1434734f,  0.6317518f,    0.4986019f,   0.6891977f,  0.6317518f,  0.5f });
            TestCase("Scavenger Colors", 6575, new float[] { 0.008721007f,  0.2362256f,   0.9356834f,  0.04377977f,   0.8417727f,   0.8770241f,  0.04987093f,   0.006763936f, 0.1690136f,  0.04377977f, 0.1f });
            TestCase("Scavenger Colors", 6576, new float[] { 0.02522365f,   0.2275454f,   0.652414f,   0.06965613f,   0.3133495f,   0.2906501f,  0.02522365f,   0.4764195f,   0.5308331f,  0.4838903f,  0.5f });
            TestCase("Scavenger Colors", 6577, new float[] { 0.06457417f,   0.005561376f, 0.07633376f, 0.06457417f,   0.005561376f, 0.07633376f, 0.06457417f,   0.153608f,    0.601204f,   0.1255781f,  0.65f });
            TestCase("Scavenger Colors", 6578, new float[] { 0.09657335f,   0.7522927f,   0.5382589f,  0.09657335f,   0.7522927f,   0.5382589f,  0.1228372f,    0.6966373f,   0.3150999f,  0.1208953f,  0.1f });
            TestCase("Scavenger Colors", 6579, new float[] { 0.009567596f,  0.1991292f,   0.4007898f,  0.9548995f,    0.2762567f,   0.255993f,   0.009567596f,  0.4835517f,   0.7615235f,  0.5132461f,  0.5f });
            TestCase("Scavenger Colors", 6580, new float[] { 0.04574991f,   0.248228f,    0.8041081f,  0.05373043f,   0.803716f,    0.7624683f,  0.04574991f,   0.7686872f,   0.05485548f, 0.06967282f, 0.1f });
            TestCase("Scavenger Colors", 6581, new float[] { 0.09327173f,   0.3449658f,   0.3817644f,  0.09327173f,   0.3449658f,   0.3817644f,  0.09327173f,   0.06665815f,  0.517639f,   0.08954267f, 0.5f });

            // Scavenger Back Patterns                              Type         Pattern  Top          Bottom      Num Size
            TestCase("Scavenger Back Patterns", 6562, new float[] { 2, IGNORE, IGNORE, 3, 0.04611824f, 0.4158052f, 35, 0.2922258f });
            TestCase("Scavenger Back Patterns", 6563, new float[] { 2, IGNORE, IGNORE, 3, 0.08988458f, 0.4355393f, 20, 0.6011468f });
            TestCase("Scavenger Back Patterns", 6564, new float[] { 2, IGNORE, IGNORE, 3, 0.04872464f, 0.8921559f, 14, 0.5401762f });
            TestCase("Scavenger Back Patterns", 6565, new float[] { 2, IGNORE, IGNORE, 3, 0.1089053f,  0.542159f,  38, 0.06315385f });
            TestCase("Scavenger Back Patterns", 6566, new float[] { 2, IGNORE, IGNORE, 2, 0.2153272f,  0.8683605f, 20, 0.5299606f });
            TestCase("Scavenger Back Patterns", 6567, new float[] { 2, IGNORE, IGNORE, 1, 0.07984871f, 0.9036555f, 7,  0.05996877f });
            TestCase("Scavenger Back Patterns", 6568, new float[] { 2, IGNORE, IGNORE, 3, 0.1562746f,  0.6212453f, 18, 0.2809117f });
            TestCase("Scavenger Back Patterns", 6569, new float[] { 2, IGNORE, IGNORE, 3, 0.08422018f, 0.720827f,  13, 0.4246287f });
            TestCase("Scavenger Back Patterns", 6570, new float[] { 2, IGNORE, IGNORE, 3, 0.1870115f,  0.7037598f, 20, 0.3164729f });
            TestCase("Scavenger Back Patterns", 6571, new float[] { 2, IGNORE, IGNORE, 3, 0.1140204f,  0.5751875f, 20, 0.5231121f });
            TestCase("Scavenger Back Patterns", 6572, new float[] { 2, IGNORE, IGNORE, 3, 0.05065926f, 0.5326367f, 25, 0.1701946f });
            TestCase("Scavenger Back Patterns", 6573, new float[] { 1, IGNORE, IGNORE, 1, 0.269342f,   0.8744684f, 8,  0.7447622f });
            TestCase("Scavenger Back Patterns", 6574, new float[] { 2, IGNORE, IGNORE, 3, 0.0469945f,  0.4323828f, 19, 0.3674024f });
            TestCase("Scavenger Back Patterns", 6575, new float[] { 2, IGNORE, IGNORE, 3, 0.1745057f,  0.6127298f, 32, 0.6246613f });
            TestCase("Scavenger Back Patterns", 6576, new float[] { 2, IGNORE, IGNORE, 3, 0.1901461f,  0.7297542f, 19, 0.7010092f });
            TestCase("Scavenger Back Patterns", 6577, new float[] { 2, IGNORE, IGNORE, 3, 0.1602716f,  0.7158399f, 35, 0.4446268f });
            TestCase("Scavenger Back Patterns", 6578, new float[] { 1, IGNORE, IGNORE, 2, 0.1454922f,  0.8855549f, 14, 0.7339344f });
            TestCase("Scavenger Back Patterns", 6579, new float[] { 2, IGNORE, IGNORE, 3, 0.151232f,   0.4849416f, 33, 0.2036929f });
            TestCase("Scavenger Back Patterns", 6580, new float[] { 2, IGNORE, IGNORE, 3, 0.133683f,   0.7759459f, 14, 0.4389523f });
            TestCase("Scavenger Back Patterns", 6581, new float[] { 1, IGNORE, IGNORE, 2, 0.1342574f,  0.7211277f, 24, 0.5605704f });
            // Type (1 = HBS, 2 = WBT), Pattern (1 = SR, 2 = DSR, 3 = RBB)

        }
    }
}
