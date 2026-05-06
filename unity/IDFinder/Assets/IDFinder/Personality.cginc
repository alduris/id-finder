// Include file for personality calculations

#ifndef RWPERSONALITY
#define RWPERSONALITY

#include "IDFinder.cginc"
#include "RWCustom.cginc"

struct Personality
{
    float aggression;
    float bravery;
    float dominance;
    float energy;
    float nervous;
    float sympathy;
};

inline Personality InitPersonality(uint4 random)
{
    Personality p;
    
    p.sympathy = PushFromHalf(RandomValue(random), 1.5);
    p.energy = PushFromHalf(RandomValue(random), 1.5);
    p.bravery = PushFromHalf(RandomValue(random), 1.5);

    p.nervous = lerp(RandomValue(random), lerp(p.energy, 1.0 - p.bravery, 0.5), pow(RandomValue(random), 0.25));
    p.aggression = lerp(RandomValue(random), (p.energy + p.bravery) / 2.0 * (1.0 - p.sympathy), pow(RandomValue(random), 0.25));
    p.dominance = lerp(RandomValue(random), (p.energy + p.bravery + p.aggression) / 3.0, pow(RandomValue(random), 0.25));

    p.nervous = PushFromHalf(p.nervous, 2.5);
    p.aggression = PushFromHalf(p.aggression, 2.5);
    
    return p;
}

#endif