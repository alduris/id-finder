// Include file for some slugpup calculations

#ifndef RWSLUGPUP
#define RWSLUGPUP

#include "Personality.cginc"

inline float GetLikeOf(int i, Personality p, inout uint4 random)
{
    // Shitty representation of a switch statement
    float a = p.aggression * ((i == 5) + (i == 8) + (i == 15))
        + p.bravery * ((i == 10) + (i == 11) + (i == 12))
        + p.dominance * ((i == 4) + (i == 6) + (i == 14))
        + p.energy * ((i == 2) + (i == 3) + (i == 13) + (i == 17))
        + p.nervous * ((i == 0) + (i == 9) + (i == 16))
        + p.sympathy * ((i == 1) + (i == 7));
    
    float b = p.aggression * ((i == 1) + (i == 3) + (i == 13))
        + p.bravery * ((i == 6) + (i == 7) + (i == 14) + (i == 17))
        + p.dominance * ((i == 10) + (i == 12))
        + p.energy * ((i == 0) + (i == 4) + (i == 9))
        + p.nervous * ((i == 2) + (i == 8) + (i == 11))
        + p.sympathy * ((i == 5) + (i == 15) + (i == 16));

    a *= PushFromHalf(RandomValue(random), 2.0);
    b *= PushFromHalf(RandomValue(random), 2.0);
    
    return clamp(lerp(a - b, lerp(-1.0, 1.0, PushFromHalf(RandomValue(random), 2.0)), PushFromHalf(RandomValue(random), 2.0)), -1.0, 1.0);
}

inline float FoodLikeDistance(float value, Input input)
{
    // VeryPositive (> 0.85)  -> 0
    // Positive     (> 0.4)   -> 1
    // None                   -> 2
    // Negative     (< -0.4)  -> 3
    // VeryNegative (< -0.85) -> 4
    // 0.4 and 0.85 are thresholds
    
    int v = (value <= 0.85) + (value <= 0.4) + (value < -0.4) + (value < -0.85);
    return MatchDistance(v, input);
}

#endif