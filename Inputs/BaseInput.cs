using Menu.Remix.MixedUI;

namespace FinderMod.Inputs
{
    public abstract class BaseInput
    {
        // This can be publically changed
        public bool Enabled = false;
        public bool Wrap = false;

        // These are set by the config itself
        public int ValueCount { get; protected set; }
        public string Name { get; protected set; }

        public BaseInput(string name, int inputs)
        {
            ValueCount = inputs;
            Name = name;
        }

        public abstract float? GetValue(int index);

        public abstract UIelement GetUI(float tx, float x, ref float y);

        public override abstract string ToString();
        
    }
}
