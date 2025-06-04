using System;
using Menu.Remix.MixedUI;
using Menu.Remix.MixedUI.ValueTypes;
using UnityEngine;

namespace FinderMod.Inputs
{
    public class ColorLerpInput(string name, Color end1, Color end2) : RangedInput<float>(name, 0.5f, 0f, 1f)
    {
        private readonly Color end1 = end1, end2 = end2;

        public override float InputHeight => 30f;

        protected override UIconfig GetElement(Vector2 pos) => new OpFloatColorSlider(ConfigRange(), pos, 160, end1, end2);

        protected override float GetValue(UIconfig element) => (element as OpFloatSlider).GetValueFloat();

        public class OpFloatColorSlider : OpFloatSlider
        {
            private readonly DyeableRect rect;
            private readonly Color end1, end2;

            public OpFloatColorSlider(ConfigurableBase configFloat, Vector2 pos, int length, Color end1, Color end2) : base(configFloat, pos, length)
            {
                this.end1 = end1;
                this.end2 = end2;
                _dNum = 3;

                rect = new DyeableRect(myContainer, new Vector2(_size.x + 12f, 0f), new Vector2(30f, 30f), true)
                {
                    colorEdge = colorEdge,
                    colorFill = Color.Lerp(end1, end2, this.GetValueFloat()),
                    fillAlpha = 1f
                };
            }

            public override void GrafUpdate(float timeStacker)
            {
                base.GrafUpdate(timeStacker);
                rect.colorFill = Color.Lerp(end1, end2, this.GetValueFloat());
                rect.addSize = new Vector2(4f, 4f) * bumpBehav.AddSize;
                rect.GrafUpdate(timeStacker);
            }

            public override void Update()
            {
                base.Update();
                rect.Update();
            }
        }
    }
}
