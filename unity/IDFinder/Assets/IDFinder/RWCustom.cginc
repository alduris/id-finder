// Include file containg various functions from RWCustom.cs in Rain World
// And some extra goods, like Mathf.InverseLerp

#ifndef RWCUSTOM
#define RWCUSTOM

#include "IDFinder.cginc"

inline float invlerp(float a, float b, float x)
{
    return saturate((x - a) / (b - a));
}

inline float LerpMap(float val, float fromA, float toA, float fromB, float toB)
{
    return lerp(fromB, toB, invlerp(fromA, toA, val));
}

inline float LerpMap(float val, float fromA, float toA, float fromB, float toB, float exponent)
{
    return lerp(fromB, toB, pow(invlerp(fromA, toA, val), exponent));
}

inline float PushFromHalf(float val, float pushExponent)
{
    return LerpMap(val, round(val), 0.5, round(val), 0.5, pushExponent);
}

inline float SCurve(float x, float k)
{
    x = x * 2.0 - 1.0;
    /*if (x < 0)
    {
        x = abs(1 + x);
        return k * x / (k - x + 1) * 0.5;
    }
    k = -1 - k;
    return 0.5 + k * x / (k - x + 1) * 0.5;*/
    return lerp(0.5 + (-1 - k) * x / ((-1 - k) - x + 1) * 0.5, k * abs(1 + x) / (k - abs(1 + x) + 1) * 0.5, x < 0);
}

inline float RandomDeviation(float k, inout uint4 random)
{
    return SCurve(RandomValue(random) * 0.5, k) * 2.0 * lerp(-1.0, 1.0, RandomValue(random) < 0.5);
}

inline float ClampedRandomVariation(float baseValue, float maxDeviation, float k, inout uint4 random)
{
    return saturate(baseValue + RandomDeviation(k, random) * maxDeviation);
}

inline float WrappedRandomVariation(float baseValue, float maxDeviation, float k, inout uint4 random)
{
    float val = baseValue + RandomDeviation(k, random) * maxDeviation;
    return val - floor(val);
}

#endif