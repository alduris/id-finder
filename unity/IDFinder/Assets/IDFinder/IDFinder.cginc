// Include file containing various ID Finder utilities including:
// - XORShift128 implementation
// - Distance calculating utilities

#ifndef IDFINDER
#define IDFINDER

///////////////////////////////////////////////////////////////////////////////
// XORShift128

const uint MT19937 = 0x6c078965u;

inline uint4 InitState(uint seed)
{
    uint x = seed;
    uint y = MT19937 * x + 1;
    uint z = MT19937 * y + 1;
    uint w = MT19937 * z + 1;
    return uint4(x, y, z, w);
}

inline uint NextU32(inout uint4 state)
{
    uint t = state.x ^ (state.x << 11);
    state.x = state.y;
    state.y = state.z;
    state.z = state.w;
    return state.w = state.w ^ (state.w >> 19) ^ t ^ (t >> 8);
}

inline float RandomValue(inout uint4 state)
{
    return (NextU32(state) & 0x7FFFFFu) * 1.192093E-07f;
}

inline int RandomRange(int a, int b, inout uint4 state)
{
    if (a == b)
        return a;

    int u = min(a, b);
    int v = max(a, b);
    
    return u + (NextU32(state) % (v - u));
}

inline float RandomRange(float a, float b, inout uint4 state)
{
    float f = (NextU32(state) & 0x7FFFFFu) * 1.192093E-07f;
    return ((1.0f - f) * b) + (f * a);
}

///////////////////////////////////////////////////////////////////////////////
// Distance helpers

struct Input
{
    bool enabled;
    float value;
    float range;
    int bias;
};

/*struct Result
{
    int id;
    float dist;
};*/

inline float Distance(float value, Input input)
{
    return lerp(0, abs(value - input.value) / input.range * input.bias, input.enabled);
}

inline float Distance(int value, Input input)
{
    return Distance((float) value, input);
}

inline float WrapDistance(float value, Input input)
{
    return min(Distance(value, input), min(Distance(value - 1, input), Distance(value + 1, input)));
}

inline float MatchDistance(int value, Input input)
{
    return lerp(0, input.bias, input.enabled * (input.value == value));
}

#endif