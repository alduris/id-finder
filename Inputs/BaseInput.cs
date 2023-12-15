using System;
using System.Collections.Generic;
using Menu.Remix.MixedUI;

namespace FinderMod.Inputs
{
    public abstract class BaseInput
    {
        protected const float LINE_HEIGHT = 30f;
        protected const float LABEL_OFFSET = 32f;
        protected const float INPUT_OFFSET = 8f;

        // This can be publicly changed
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

        public abstract void AddUI(float x, ref float y, List<UIelement> inputs, Action UpdateQueryBox);

        public override abstract string ToString();
        
    }
}
