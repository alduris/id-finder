using System;
using FinderMod.Inputs;

namespace FinderMod.Search.Options
{
    internal class OPersonality() : IOption(9, false)
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

        public override void Run(float[] i, float[] o, SearchData data)
        {
            var p = data.Personality;
            o[0] = p.Aggression;
            o[1] = p.Bravery;
            o[2] = p.Dominance;
            o[3] = p.Energy;
            o[4] = p.Nervous;
            o[5] = p.Sympathy;
        }
    }
}
