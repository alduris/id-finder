using static FinderMod.Test.TestCases;

namespace FinderMod.Test
{
    internal class ScavTests
    {
        public static void TestAllScavs()
        {
            // Scavenger colors                              Body H         Body S        Body L       Head H         Head S        Head L       Deco H         Deco S        Deco L       Eye H        Eye L
            TestCase("Scavenger Colors", 6562, [0.03927018f,   0.1016362f,   0.5098599f,  0.2257873f,    0.6311247f,   0.174932f,   0.03927018f,   0.7725409f,   0.3268195f,  0.2269589f,  0.65f]);
            TestCase("Scavenger Colors", 6563, [0.008640707f,  0.7348285f,   0.9946128f,  0.9958526f,    0.9075065f,   0.6405754f,  0.3375438f,    0.121703f,    0.1194663f,  0.3375438f,  0.1f]);
            TestCase("Scavenger Colors", 6564, [0.01822638f,   0.1707397f,   0.2690711f,  0.01822638f,   0.1707397f,   0.2690711f,  0.1224235f,    0.1537622f,   0.4812316f,  0.1224235f,  0.5f]);
            TestCase("Scavenger Colors", 6565, [0.08232065f,   0.003095639f, 0.4367136f,  0.08888692f,   0.8277774f,   0.1409272f,  0.08232065f,   0.1403171f,   0.5463402f,  0.1273654f,  0.5f]);
            TestCase("Scavenger Colors", 6566, [0.0675993f,    0.8267188f,   0.8204471f,  0.0675993f,    0.8267188f,   0.8204471f,  0.5695145f,    0.03915525f,  0.2191887f,  0.5695145f,  0.1f]);
            TestCase("Scavenger Colors", 6567, [0.004825009f,  0.01663712f,  0.6376163f,  0.01204274f,   0.8460221f,   0.1495164f,  0.004825009f,  0.1918851f,   0.6509836f,  0.1066455f,  0.5f]);
            TestCase("Scavenger Colors", 6568, [0.006453443f,  0.7565702f,   0.6247283f,  0.006453443f,  0.7565702f,   0.6247283f,  0.006453443f,  0.5203807f,   0.2605545f,  0.04877108f, 0.1f]);
            TestCase("Scavenger Colors", 6569, [0.07317828f,   0.2883185f,   0.375261f,   0.07317828f,   0.2883185f,   0.375261f,   0.07317828f,   0.4918061f,   0.3427151f,  0.123756f,   0.5f]);
            TestCase("Scavenger Colors", 6570, [0.0003771067f, 0.304457f,    0.8566296f,  0.0003771067f, 0.2283428f,   0.6334845f,  0.0003771067f, 0.991048f,    0.2044024f,  0.1886323f,  0.1604177f]);
            TestCase("Scavenger Colors", 6571, [0.09831458f,   0.9205362f,   0.2911566f,  0.09831458f,   0.9205362f,   0.2911566f,  0.09831458f,   0.2903242f,   0.4290386f,  0.1473219f,  0.7004695f]);
            TestCase("Scavenger Colors", 6572, [0.07098702f,   0.07248832f,  0.7618944f,  0.04472485f,   0.8156875f,   0.1820773f,  0.07098702f,   0.8746629f,   0.4098181f,  0.08820463f, 0.5f]);
            TestCase("Scavenger Colors", 6573, [0.02779202f,   0.5164752f,   0.6566148f,  0.02779202f,   0.3873564f,   0.6446329f,  0.02779202f,   0.3346376f,   0.2008889f,  0.08105632f, 0.1f]);
            TestCase("Scavenger Colors", 6574, [0.09869953f,   0.05960437f,  0.4857436f,  0.6317518f,    0.7143267f,   0.1434734f,  0.6317518f,    0.4986019f,   0.6891977f,  0.6317518f,  0.5f]);
            TestCase("Scavenger Colors", 6575, [0.008721007f,  0.2362256f,   0.9356834f,  0.04377977f,   0.8417727f,   0.8770241f,  0.04987093f,   0.006763936f, 0.1690136f,  0.04377977f, 0.1f]);
            TestCase("Scavenger Colors", 6576, [0.02522365f,   0.2275454f,   0.652414f,   0.06965613f,   0.3133495f,   0.2906501f,  0.02522365f,   0.4764195f,   0.5308331f,  0.4838903f,  0.5f]);
            TestCase("Scavenger Colors", 6577, [0.06457417f,   0.005561376f, 0.07633376f, 0.06457417f,   0.005561376f, 0.07633376f, 0.06457417f,   0.153608f,    0.601204f,   0.1255781f,  0.65f]);
            TestCase("Scavenger Colors", 6578, [0.09657335f,   0.7522927f,   0.5382589f,  0.09657335f,   0.7522927f,   0.5382589f,  0.1228372f,    0.6966373f,   0.3150999f,  0.1208953f,  0.1f]);
            TestCase("Scavenger Colors", 6579, [0.009567596f,  0.1991292f,   0.4007898f,  0.9548995f,    0.2762567f,   0.255993f,   0.009567596f,  0.4835517f,   0.7615235f,  0.5132461f,  0.5f]);
            TestCase("Scavenger Colors", 6580, [0.04574991f,   0.248228f,    0.8041081f,  0.05373043f,   0.803716f,    0.7624683f,  0.04574991f,   0.7686872f,   0.05485548f, 0.06967282f, 0.1f]);
            TestCase("Scavenger Colors", 6581, [0.09327173f,   0.3449658f,   0.3817644f,  0.09327173f,   0.3449658f,   0.3817644f,  0.09327173f,   0.06665815f,  0.517639f,   0.08954267f, 0.5f]);

            // Scavenger Back Patterns                              Type         Pattern  Top          Bottom      Num Size
            TestCase("Scavenger Back Patterns", 6562, [2, IGNORE, IGNORE, 3, 0.04611824f, 0.4158052f, 35, 0.2922258f]);
            TestCase("Scavenger Back Patterns", 6563, [2, IGNORE, IGNORE, 3, 0.08988458f, 0.4355393f, 20, 0.6011468f]);
            TestCase("Scavenger Back Patterns", 6564, [2, IGNORE, IGNORE, 3, 0.04872464f, 0.8921559f, 14, 0.5401762f]);
            TestCase("Scavenger Back Patterns", 6565, [2, IGNORE, IGNORE, 3, 0.1089053f,  0.542159f,  38, 0.06315385f]);
            TestCase("Scavenger Back Patterns", 6566, [2, IGNORE, IGNORE, 2, 0.2153272f,  0.8683605f, 20, 0.5299606f]);
            TestCase("Scavenger Back Patterns", 6567, [2, IGNORE, IGNORE, 1, 0.07984871f, 0.9036555f, 7,  0.05996877f]);
            TestCase("Scavenger Back Patterns", 6568, [2, IGNORE, IGNORE, 3, 0.1562746f,  0.6212453f, 18, 0.2809117f]);
            TestCase("Scavenger Back Patterns", 6569, [2, IGNORE, IGNORE, 3, 0.08422018f, 0.720827f,  13, 0.4246287f]);
            TestCase("Scavenger Back Patterns", 6570, [2, IGNORE, IGNORE, 3, 0.1870115f,  0.7037598f, 20, 0.3164729f]);
            TestCase("Scavenger Back Patterns", 6571, [2, IGNORE, IGNORE, 3, 0.1140204f,  0.5751875f, 20, 0.5231121f]);
            TestCase("Scavenger Back Patterns", 6572, [2, IGNORE, IGNORE, 3, 0.05065926f, 0.5326367f, 25, 0.1701946f]);
            TestCase("Scavenger Back Patterns", 6573, [1, IGNORE, IGNORE, 1, 0.269342f,   0.8744684f, 8,  0.7447622f]);
            TestCase("Scavenger Back Patterns", 6574, [2, IGNORE, IGNORE, 3, 0.0469945f,  0.4323828f, 19, 0.3674024f]);
            TestCase("Scavenger Back Patterns", 6575, [2, IGNORE, IGNORE, 3, 0.1745057f,  0.6127298f, 32, 0.6246613f]);
            TestCase("Scavenger Back Patterns", 6576, [2, IGNORE, IGNORE, 3, 0.1901461f,  0.7297542f, 19, 0.7010092f]);
            TestCase("Scavenger Back Patterns", 6577, [2, IGNORE, IGNORE, 3, 0.1602716f,  0.7158399f, 35, 0.4446268f]);
            TestCase("Scavenger Back Patterns", 6578, [1, IGNORE, IGNORE, 2, 0.1454922f,  0.8855549f, 14, 0.7339344f]);
            TestCase("Scavenger Back Patterns", 6579, [2, IGNORE, IGNORE, 3, 0.151232f,   0.4849416f, 33, 0.2036929f]);
            TestCase("Scavenger Back Patterns", 6580, [2, IGNORE, IGNORE, 3, 0.133683f,   0.7759459f, 14, 0.4389523f]);
            TestCase("Scavenger Back Patterns", 6581, [1, IGNORE, IGNORE, 2, 0.1342574f,  0.7211277f, 24, 0.5605704f]);
            // Type (1 = HBS, 2 = WBT), Pattern (1 = SR, 2 = DSR, 3 = RBB)


            // Elite scavenger colors                              Body H         Body S        Body L       Head H         Head S       Head L      Deco H         Deco S       Deco L
            TestCase("Elite Scavenger Colors", 7795, [0.001136018f,  0.8451249f,   0.4949799f,  0.001136018f,  0.8451249f, 0.4949799f,  0.8616755f,    0.8868557f,  0.4356378f]);
            TestCase("Elite Scavenger Colors", 7796, [2.203775E-05f, 0.036598f,    0.2258678f,  0.9990518f,    0.7276163f, 0.171484f,   0.4731144f,    0.6885896f,  0.4046143f]);
            TestCase("Elite Scavenger Colors", 7797, [0.1190408f,    0.49203f,     0.7721173f,  0.1190408f,    0.3690225f, 0.4819592f,  0.1368859f,    0.3375374f,  0.4621389f]);
            TestCase("Elite Scavenger Colors", 7798, [0.008530766f,  0.006343696f, 0.2729666f,  0.01633167f,   0.8655018f, 0.09051473f, 0.1421851f,    0.4155952f,  0.5863674f]);
            TestCase("Elite Scavenger Colors", 7799, [0.3840314f,    0.7023476f,   0.1583831f,  0.3840314f,    0.7023476f, 0.1583831f,  0.3687373f,    0.5306676f,  0.6660133f]);
            TestCase("Elite Scavenger Colors", 7800, [0.00177244f,   0.9586796f,   0.3597798f,  0.00177244f,   0.9586796f, 0.3597798f,  0.00177244f,   0.7420791f,  0.4001982f]);
            TestCase("Elite Scavenger Colors", 7801, [0.04008427f,   0.01763009f,  0.7416434f,  0.9502224f,    0.4095998f, 0.2882306f,  0.04008427f,   0.3397519f,  0.7456605f]);
            TestCase("Elite Scavenger Colors", 7802, [0.01807125f,   0.7733045f,   0.7825821f,  0.0395337f,    0.5823412f, 0.6524848f,  0.01807125f,   0.5352952f,  0.3294647f]);
            TestCase("Elite Scavenger Colors", 7803, [0.0002055083f, 0.001559882f, 0.4025975f,  0.940299f,     0.8864723f, 0.07516699f, 0.0002055083f, 0.4992239f,  0.6218862f]);
            TestCase("Elite Scavenger Colors", 7804, [0.03498837f,   0.6321658f,   0.8254362f,  0.03498837f,   0.4741243f, 0.5753596f,  0.03498837f,   0.5248517f,  0.3396667f]);
            TestCase("Elite Scavenger Colors", 7805, [0.001209362f,  0.6189785f,   0.9871998f,  0.9410684f,    0.5612665f, 0.6434841f,  0.001209362f,  0.9032965f,  0.342405f]);
            TestCase("Elite Scavenger Colors", 7806, [2.421688E-05f, 0.1204021f,   0.3082095f,  0.006482281f,  0.5741996f, 0.2467866f,  2.421688E-05f, 0.5347711f,  0.8325319f]);
            TestCase("Elite Scavenger Colors", 7807, [0.06691504f,   0.5324982f,   0.6968583f,  0.08904279f,   0.4687217f, 0.4157182f,  0.06691504f,   0.1282826f,  0.1308671f]);
            TestCase("Elite Scavenger Colors", 7808, [0.7638471f,    0.02916162f,  0.31856f,    0.9348224f,    0.7714231f, 0.1286913f,  0.7638471f,    0.4501479f,  0.646425f]);
            TestCase("Elite Scavenger Colors", 7809, [0.3572006f,    0.6041891f,   0.01097584f, 0.3572006f,    0.6041891f, 0.01097584f, 0.3572006f,    0.4350207f,  0.2163298f]);
            TestCase("Elite Scavenger Colors", 7810, [0.3336366f,    0.9510788f,   0.5589507f,  0.3336366f,    0.9510788f, 0.5589507f,  0.3330438f,    0.4805457f,  0.378161f]);
            TestCase("Elite Scavenger Colors", 7811, [0.01587369f,   0.2235081f,   0.4410057f,  0.01587369f,   0.2235081f, 0.4410057f,  0.01156811f,   0.7263564f,  0.4145347f]);
            TestCase("Elite Scavenger Colors", 7812, [0.0001444513f, 0.6892556f,   0.820048f,   0.9994635f,    0.793424f,  0.7891791f,  0.3657962f,    0.06069852f, 0.09363019f]);
            TestCase("Elite Scavenger Colors", 7813, [1.900745E-05f, 0.02573412f,  0.4005555f,  0.1258937f,    0.8319358f, 0.1140471f,  1.900745E-05f, 0.7156214f,  0.5367416f]);
            TestCase("Elite Scavenger Colors", 7814, [7.805145E-10f, 0.9771295f,   0.6494907f,  7.805145E-10f, 0.9771295f, 0.6494907f,  0.06642074f,   0.2719971f,  0.1529266f]);

            // Elite scavenger back patterns                           Pattern  Top          Bottom      Num Size
            TestCase("Elite Scavenger Back Patterns", 7795, 2, [2, 0.2395298f,  0.8447037f, 14, 0.4366659f]);
            TestCase("Elite Scavenger Back Patterns", 7796, 2, [1, 0.2027242f,  0.8034261f, 7,  0.652213f]);
            TestCase("Elite Scavenger Back Patterns", 7797, 2, [1, 0.1674842f,  0.7329226f, 10, 0.5170734f]);
            TestCase("Elite Scavenger Back Patterns", 7798, 2, [2, 0.1256906f,  0.670608f,  8,  0.2306415f]);
            TestCase("Elite Scavenger Back Patterns", 7799, 2, [2, 0.2587558f,  0.6111765f, 8,  0.3728951f]);
            TestCase("Elite Scavenger Back Patterns", 7800, 2, [1, 0.2305054f,  0.7875323f, 4,  0.8728709f]);
            TestCase("Elite Scavenger Back Patterns", 7801, 2, [1, 0.2448776f,  0.8951868f, 7,  0.6854035f]);
            TestCase("Elite Scavenger Back Patterns", 7802, 2, [1, 0.1697445f,  0.7447858f, 13, 0.7565591f]);
            TestCase("Elite Scavenger Back Patterns", 7803, 2, [1, 0.1947737f,  0.9662684f, 8,  0.2035128f]);
            TestCase("Elite Scavenger Back Patterns", 7804, 2, [1, 0.2332188f,  0.7945f,    16, 0.4710213f]);
            TestCase("Elite Scavenger Back Patterns", 7805, 2, [3, 0.04970961f, 0.8736076f, 22, 0.4442028f]);
            TestCase("Elite Scavenger Back Patterns", 7806, 2, [1, 0.1177f,     0.6409903f, 6,  0.7007739f]);
            TestCase("Elite Scavenger Back Patterns", 7807, 2, [2, 0.193835f,   0.7888674f, 14, 0.5269699f]);
            TestCase("Elite Scavenger Back Patterns", 7808, 2, [3, 0.05012065f, 0.5445747f, 12, 0.9226902f]);
            TestCase("Elite Scavenger Back Patterns", 7809, 2, [1, 0.08912542f, 0.8593001f, 8,  0.5668983f]);
            TestCase("Elite Scavenger Back Patterns", 7810, 2, [1, 0.237359f,   0.8034862f, 12, 0.9441476f]);
            TestCase("Elite Scavenger Back Patterns", 7811, 2, [2, 0.2235446f,  0.9200004f, 26, 0.3437088f]);
            TestCase("Elite Scavenger Back Patterns", 7812, 2, [2, 0.2710113f,  0.8539116f, 10, 0.5486535f]);
            TestCase("Elite Scavenger Back Patterns", 7813, 2, [3, 0.1211188f,  0.5532407f, 22, 0.4769053f]);
            TestCase("Elite Scavenger Back Patterns", 7814, 2, [1, 0.21252f,    0.7039604f, 10, 0.4637166f]);
            // Type hardcoded to HBS; Pattern (1 = SR, 2 = DSR, 3 = RBB)
        }
    }
}
