using System;
using FinderMod.Inputs;

namespace FinderMod.Search.Options
{
    public abstract class IOption(int floats, bool msc)
    {
        private readonly int MinFloats = floats;
        public readonly bool MSC = msc;

        public abstract BaseInput[] CreateInputs();

        public void DetermineMaxFloats(ref int max) => max = Math.Max(MinFloats, max);

        public abstract void Run(float[] i, float[] o, SearchData data);
    }
}
