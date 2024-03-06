using System;
using System.Collections.Generic;
using Menu.Remix.MixedUI;

namespace FinderMod.Inputs
{
    public abstract class BaseInput(string name, int inputs)
    {
        protected const float LINE_HEIGHT = 30f;
        protected const float LABEL_OFFSET = 32f;
        protected const float INPUT_OFFSET = 8f;

        // This can be publicly changed
        public bool Enabled = false;
        public bool Wrap = false;
        public string Description = "";

        // These are set by the config itself
        public int ValueCount { get; protected set; } = inputs;
        public string Name { get; protected set; } = name;

        public abstract float? GetValue(int index);

        public virtual void SetValues(bool enabled, List<float> values)
        {
            Enabled = enabled;
        }

        public abstract void AddUI(float x, ref float y, List<UIelement> inputs, Action UpdateQueryBox);

        public override abstract string ToString();
        
    }
}
