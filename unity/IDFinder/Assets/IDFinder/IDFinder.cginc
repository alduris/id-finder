// Include file containing various ID Finder utilities including:
// - XORShift128 implementation
// - Distance calculating utilities

#ifndef IDFINDER
#define IDFINDER

#ifndef IDFINDER_X
#define IDFINDER_X 32
#endif

#ifndef IDFINDER_Y
#define IDFINDER_Y 32
#endif

///////////////////////////////////////////////////////////////////////////////
// XORShift128

#define MT19937 0x6c078965u

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

inline void Shift(inout uint4 state)
{
    NextU32(state);
}

inline void ShiftIf(inout uint4 state, int cond)
{
    uint4 old = state;
    NextU32(state);
    state = state * cond + old * (1 - cond);
}

inline float RandomValue(inout uint4 state)
{
    return (NextU32(state) & 0x7FFFFFu) * 1.192093E-07;
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
    //return lerp(b, a, (NextU32(state) & 0x7FFFFFu) * 1.192093E-07); // in case this reduces local vars
    float f = (NextU32(state) & 0x7FFFFFu) * 1.192093E-07;
    return ((1.0f - f) * b) + (f * a);
}

inline float RandomValueIf(inout uint4 state, int cond)
{
    uint4 old = state;
    float f = RandomValue(state);
    state = state * cond + old * (1 - cond);
    return f;
}

inline int RandomRangeIf(int a, int b, inout uint4 state, int cond)
{
    uint4 old = state;
    int i = RandomRange(a, b, state);
    state = state * cond + old * (1 - cond);
    return i;
}

inline float RandomRangeIf(float a, float b, inout uint4 state, int cond)
{
    uint4 old = state;
    float f = RandomRange(a, b, state);
    state = state * cond + old * (1 - cond);
    return f;
}

///////////////////////////////////////////////////////////////////////////////
// Distance helpers

struct Input
{
    float value;
    float range;
    int bias;
};

inline float Distance(float value, Input input)
{
    return abs(value - input.value) / input.range * input.bias;
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
    return input.bias * (input.value != (float) value);
}

inline float MatchDistance(bool value, Input input)
{
    return input.bias * (value != (input.value > 0));
    //return MatchDistance((int)value, input);
}

///////////////////////////////////////////////////////////////////////////////
// ID Finder main functionality

struct Result
{
    int id;
    float dist;
};

StructuredBuffer<Input> _IDFinderInputs;
RWStructuredBuffer<Result> _IDFinderResults;
uint3 _IDFinderDispatch;
int _IDFinderStart;

// This is user-defined, operates on a single id. Works just like FinderMod.Search.Option.Execute()
float Execute(uint4 random, StructuredBuffer<Input> inputs);

// The user must define this as a pragma kernel, as it will not work if I define it in here
[numthreads(IDFINDER_X, IDFINDER_Y, 1)]
void CS_IDFinderMain(uint3 thread : SV_DispatchThreadID)
{
    uint offset = (thread.x + thread.y * IDFINDER_X * _IDFinderDispatch.x) * 32;
    uint seed = _IDFinderStart + offset;
    
    // [unroll]
    for (uint i = 0; i < 32; i++)
    {
        Result r;
        r.id = seed + i;
        uint4 random = InitState(r.id);
        r.dist = Execute(random, _IDFinderInputs);
        _IDFinderResults[offset + i] = r;
    }
}

#endif