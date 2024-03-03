using System;
using FinderMod.Inputs;

namespace FinderMod.Search.Options
{
    public abstract class IOption(int floats)
    {
        private readonly int MinFloats = floats;

        public abstract BaseInput[] CreateInputs();

        public void DetermineMaxFloats(ref int max) => max = Math.Max(MinFloats, max);

        public abstract void Run(float[] input, float[] output, Personality personality, SearchData data);
    }
}
