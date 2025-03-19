using Menu.Remix.MixedUI;
using Menu.Remix.MixedUI.ValueTypes;
using UnityEngine;

namespace FinderMod.Inputs
{
    public class BoolInput(string name, bool init = false) : Input<bool>(name, init)
    {

        public override float Height => 24f;

        protected override UIconfig GetElement(Vector2 pos) => new OpLabelCheckbox(Config(), pos);

        protected override bool GetValue(UIconfig element) => (element as OpLabelCheckbox)!.boolValue;

        private class OpLabelCheckbox : OpCheckBox
        {
            public bool boolValue;
            private readonly FLabel label;

            public OpLabelCheckbox(Configurable<bool> config, Vector2 pos) : base(config, pos)
            {
                _size = new Vector2(40f, 24f);
                fixedSize = size;
                symbolSprite.RemoveFromContainer();
                label = new FLabel(LabelTest.GetFont(false), config.Value ? "Yes" : "No")
                {
                    anchorX = 0.5f,
                    anchorY = 0.5f,
                    x = 20f,
                    y = 12f
                };
                myContainer.AddChild(label);

                foreach (var sprite in rect.sprites)
                {
                    sprite.RemoveFromContainer();
                }
                rect = new DyeableRect(myContainer, Vector2.zero, size, true);

                OnValueChanged += UpdateText;
            }

            private void UpdateText(UIconfig config, string value, string oldValue)
            {
                label.text = this.GetValueBool() ? "Yes" : "No";
            }

            public override void GrafUpdate(float timeStacker)
            {
                base.GrafUpdate(timeStacker);
                boolValue = this.GetValueBool();

                label.color = bumpBehav.GetColor(colorEdge);
            }
        }
    }
}
