using System;
using System.Collections.Generic;
using FinderMod.Inputs;
using Menu.Remix.MixedUI;
using UnityEngine;
using static FinderMod.Search.Util.LizardUtil;

namespace FinderMod.Search.Options
{
    public class LizardPatternsOption : Option
    {
        private readonly LizardInput typeInp;

        public LizardPatternsOption() : base("Lizard Patterns")
        {
            elements = [
                typeInp = new LizardInput(),
                new Whitespace(),
                // TODO: color here
                new Whitespace(),
                // TODO: patterns here
            ];
        }

        public override float Execute(XORShift128 Random)
        {
            LizardType type = typeInp.value;

            // Advance past unnecessary stuff
            Random.Shift(2);
            if (Random.Value < 0.5f) Random.Shift();
            Random.Shift(2);
            float tailLength = ClampedRandomVariation(0.5f, 0.2f, 0.3f, Random) * 2f;
            Random.Shift(2);
            float tailColor = 0f;
            if (type != LizardType.White && Random.Value > 0.5f)
            {
                tailColor = Random.Value;
            }

            Random.Shift(NumLimbs(type) + NumTailSegments(type) + NumTongueSegments(type) + 2); // 2 = head + salamader chance (not included because it can vary)

            // Ok now figure out variations
            float r = 0f;

            // TODO:

            throw new NotImplementedException();
            return r;
        }

        /// <summary>
        /// Version of <see cref="EnumInput{T}"/> for <see cref="LizardType"/> that forces a menu update
        /// </summary>
        private class LizardInput : EnumInput<LizardType>
        {
            public LizardInput() : base("Lizard type", LizardType.Green)
            {
                forceEnabled = true;
            }

            protected override UIconfig GetElement(Vector2 pos)
            {
                var el = base.GetElement(pos);
                el.OnValueChanged += (_, _, _) => UpdateQueryBox();
                return el;
            }
        }

        /// <summary>
        /// A very poor implementation of a switch statement
        /// </summary>
        /// <param name="lizInput"></param>
        private class LizardColor(LizardInput lizInput) : IElement
        {
            private readonly LizardInput lizInput = lizInput;
            private readonly Dictionary<LizardType, ColorHSLInput> groups = new()
            {
                {  }
            };

            public float Height => groups[lizInput.value].Height;
            public FloatInput HueInput => groups[lizInput.value].HueInput;
            public FloatInput SatInput => groups[lizInput.value].SatInput;
            public FloatInput LightInput => groups[lizInput.value].LightInput;

            public void Create(float x, ref float y, List<UIelement> elements)
            {
                groups[lizInput.value].Create(x, ref y, elements);
            }
        }
    }
}
