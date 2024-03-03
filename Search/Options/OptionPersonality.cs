using System;
using FinderMod.Inputs;

namespace FinderMod.Search.Options
{
    internal class OptionPersonality() : IOption(6)
    {
        public override BaseInput[] CreateInputs()
        {
            return [
                new FloatInput("Aggression"),
                new FloatInput("Bravery"),
                new FloatInput("Dominance"),
                new FloatInput("Energy"),
                new FloatInput("Nervousness"),
                new FloatInput("Sympathy")
            ];
        }

        public override void Run(float[] input, float[] output, Personality personality, SearchData data)
        {
            output[0] = personality.Aggression;
            output[1] = personality.Bravery;
            output[2] = personality.Dominance;
            output[3] = personality.Energy;
            output[4] = personality.Nervous;
            output[5] = personality.Sympathy;
        }
    }
}
