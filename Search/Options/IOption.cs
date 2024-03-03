using FinderMod.Inputs;

namespace FinderMod.Search.Options
{
    public abstract class IOption
    {
        public abstract BaseInput[] CreateInputs();

        public abstract void Apply(float[] input, float[] output, Personality personality, SearchData data);
    }
}
