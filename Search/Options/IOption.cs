using System.Linq;
using FinderMod.Inputs;

namespace FinderMod.Search.Options
{
    public abstract class IOption(int floats, bool msc)
    {
        public readonly int MinFloats = floats;
        public readonly bool MSC = msc;

        public abstract BaseInput[] CreateInputs();

        public int NumOutputs => CreateInputs().Sum(x => x.ValueCount);

        public abstract void Run(float[] i, float[] o, SearchData data);
    }
}
