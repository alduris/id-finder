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

inline float Decimal(float val)
{
    return val - floor(val);
}

inline float DistanceBetweenZeroToOneFloats(float a, float b)
{
    return min(min(abs(a - b), abs(a + 1.0 - b)), abs(a - 1.0 - b));
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

inline float ClampedRandomVariation(float3 values, inout uint4 random)
{
    return ClampedRandomVariation(values.x, values.y, values.z, random);
}

inline float WrappedRandomVariation(float baseValue, float maxDeviation, float k, inout uint4 random)
{
    float val = baseValue + RandomDeviation(k, random) * maxDeviation;
    return val - floor(val);
}

inline float WrappedRandomVariation(float3 values, inout uint4 random)
{
    return WrappedRandomVariation(values.x, values.y, values.z, random);
}

inline float3 HSL2RGB(float3 hsl)
{
    // I would use (and in fact have) a more GPU-optimized HSL2RGB and vice versa,
    // but alas I have to deal with the flawed implementation Joar uses
    float3 result;
    float v = lerp((hsl.z + hsl.y - hsl.z * hsl.y), (hsl.z * (1.0 + hsl.y)), (hsl.z <= 0.5));
    float i = hsl.z + hsl.z - v;
    float sv = (v - i) / v;
    hsl.x *= 6;
    int sextant = (int) hsl.x;
    float fract = hsl.x - sextant;
    float vsf = v * sv * fract;
    float mid1 = i + vsf;
    float mid2 = v - vsf;
    /*switch (sextant)
    {
        case 0:
            result = float3(v, mid1, i);
            break;
        case 1:
            result = float3(mid2, v, i);
            break;
        case 2:
            result = float3(i, v, mid1);
            break;
        case 3:
            result = float3(i, mid2, v);
            break;
        case 4:
            result = float3(mid1, i, v);
            break;
        case 5:
            result = float3(v, i, mid2);
            break;
    }*/
    result = float3(v, mid1, i) * (sextant == 0);
    result += float3(mid2, v, i) * (sextant == 1);
    result += float3(i, v, mid1) * (sextant == 2);
    result += float3(i, mid2, v) * (sextant == 3);
    result += float3(mid1, i, v) * (sextant == 4);
    result += float3(v, i, mid2) * (sextant == 5);
    result = lerp(result, float3(hsl.z, hsl.z, hsl.z), v <= 0 || sextant < 0 || sextant >= 6);
    return result;
}

inline float3 HSL2RGB(float h, float s, float l)
{
    return HSL2RGB(float3(h, s, l));
}

inline float3 RGB2HSL(float3 color)
{
    float cmax = max(max(color.r, color.g), color.b);
    float cmin = min(min(color.r, color.g), color.b);
    float cmid = (cmax + cmin) / 2.0;
    float h = 0;
    
    float d = cmax - cmin;
    float s = lerp((d / (cmax + cmin)), (d / (2.0 - cmax - cmin)), (cmid > 0.5));
    
    h += (cmax == color.r) * ((color.g - color.b) / d + 6.0 * (color.g < color.b));
    h += (cmax == color.g) * ((color.b - color.r) / d + 2.0);
    h += (cmax == color.b) * ((color.r - color.g) / d + 4.0);
    h /= 6;
    
    return (cmax == cmin) ? float3(0, 0, cmid) : float3(h, s, cmid);
}

#endif