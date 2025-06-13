global using OpComboBox = FinderMod.OpComboBox2;
global using OpResourceSelector = FinderMod.OpResourceSelector2;
// global using OpTextBox = FinderMod.OpTextBox2;

using System.Collections.Generic;
using Menu.Remix.MixedUI;
using UnityEngine;

namespace FinderMod
{
    /// <summary>
    /// <see cref="OpComboBox"/> with no transparent background and moves to front when selected.
    /// </summary>
    /// <remarks>Thanks to Henpemaz for providing this.</remarks>
    public class OpComboBox2 : Menu.Remix.MixedUI.OpComboBox
    {
        /// <param name="config">Configurable for selector</param>
        /// <param name="pos">Position</param>
        /// <param name="width">Width</param>
        /// <param name="array">List items</param>
        public OpComboBox2(Configurable<string> config, Vector2 pos, float width, string[] array) : base(config, pos, width, array) { }
        /// <param name="config">Configurable for selector</param>
        /// <param name="pos">Position</param>
        /// <param name="width">Width</param>
        /// <param name="list">List items</param>
        public OpComboBox2(Configurable<string> config, Vector2 pos, float width, List<ListItem> list) : base(config, pos, width, list) { }

        ///
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

    /// <summary>
    /// <see cref="OpResourceSelector"/> with no transparent background and moves to front when selected.
    /// </summary>
    /// <remarks>Thanks to Henpemaz for providing this.</remarks>
    public class OpResourceSelector2 : Menu.Remix.MixedUI.OpResourceSelector
    {
        /// <param name="config">Configurable for selector</param>
        /// <param name="pos">Position</param>
        /// <param name="width">Width</param>
        public OpResourceSelector2(ConfigurableBase config, Vector2 pos, float width) : base(config, pos, width) { }
        /// <param name="config">Configurable for selector</param>
        /// <param name="pos">Position</param>
        /// <param name="width">Width</param>
        /// <param name="listType">Special list type</param>
        public OpResourceSelector2(Configurable<string> config, Vector2 pos, float width, SpecialEnum listType) : base(config, pos, width, listType) { }

        /// 
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

    /// <summary>
    /// <see cref="OpTextBox"/> with some minor fixes for both bugs and QOL.
    /// </summary>
    public class OpTextBox2 : Menu.Remix.MixedUI.OpTextBox
    {
        /// <summary>
        /// Creates element. Contains differing functionality for setting <see cref="OpTextBox.accept"/> and <see cref="OpTextBox.allowSpace"/>.
        /// </summary>
        /// <param name="config">Configurable for text box</param>
        /// <param name="pos">Position</param>
        /// <param name="sizeX">Width</param>
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

        private string lastLastValue;

        /// <summary>
        /// Fixed implementation of OnValueChanged, called when user has unselected the checkbox after changing its value.
        /// </summary>
        public event OnValueChangeHandler OnValueChangedFix = null!;

        /// 
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
