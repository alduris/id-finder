global using OpComboBox = FinderMod.OpComboBox2;
global using OpResourceSelector = FinderMod.OpResourceSelector2;
global using OpTextBox = FinderMod.OpTextBox2;

using System.Collections.Generic;
using Menu.Remix.MixedUI;
using UnityEngine;

namespace FinderMod
{

    /**
     * OpComboBox that has no transparent background
     * 
     * Thanks Henpemaz
     */
    public class OpComboBox2 : Menu.Remix.MixedUI.OpComboBox
    {
        public OpComboBox2(Configurable<string> config, Vector2 pos, float width, string[] array) : base(config, pos, width, array) { }
        public OpComboBox2(Configurable<string> config, Vector2 pos, float width, List<ListItem> list) : base(config, pos, width, list) { }

        public override void GrafUpdate(float timeStacker)
        {
            base.GrafUpdate(timeStacker);
            if (_rectList != null && !_rectList.isHidden)
            {
                myContainer.MoveToFront();

                for (int j = 0; j < 9; j++)
                {
                    _rectList.sprites[j].alpha = 1;
                }
            }
        }
    }

    public class OpResourceSelector2 : Menu.Remix.MixedUI.OpResourceSelector
    {
        public OpResourceSelector2(ConfigurableBase config, Vector2 pos, float width) : base(config, pos, width) { }
        public OpResourceSelector2(Configurable<string> config, Vector2 pos, float width, SpecialEnum listType) : base(config, pos, width, listType) { }

        public override void GrafUpdate(float timeStacker)
        {
            base.GrafUpdate(timeStacker);
            if (_rectList != null && !_rectList.isHidden)
            {
                myContainer.MoveToFront();

                for (int j = 0; j < 9; j++)
                {
                    _rectList.sprites[j].alpha = 1;
                }
            }
        }
    }

    public class OpTextBox2 : Menu.Remix.MixedUI.OpTextBox
    {
        public OpTextBox2(ConfigurableBase config, Vector2 pos, float sizeX) : base(config, pos, sizeX)
        {
            lastLastValue = value;
            if (accept == Accept.StringEng)
            {
                accept = Accept.StringASCII;
            }
            if (accept == Accept.StringASCII)
            {
                allowSpace = true;
            }
        }

        public bool test = false;
        private string lastLastValue;
        public event OnValueChangeHandler OnValueChangedFix = null!;

        public override void Update()
        {
            base.Update();
            if (lastLastValue != lastValue)
            {
                OnValueChangedFix?.Invoke(this, value, lastLastValue);
            }
            lastLastValue = lastValue;
        }
    }
}
