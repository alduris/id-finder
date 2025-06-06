﻿using System;
using Menu.Remix.MixedUI;
using Menu.Remix.MixedUI.ValueTypes;
using UnityEngine;

namespace FinderMod.Inputs
{
    public class FloatInput(string name, float min, float max) : RangedInput<float>(name, (min + max) / 2)
    {
        private readonly float min = min, max = max;

        /// <summary>
        /// Initializes with min=0, max=1
        /// </summary>
        /// <param name="name">The name of the input</param>
        public FloatInput(string name) : this(name, 0f, 1f) { }

        public override float InputHeight => 24f;

        protected override UIconfig GetElement(Vector2 pos)
        {
            int dNum = Math.Max(1, -Mathf.CeilToInt(Mathf.Log10(max - min)) + 3);
            return new OpFloatSlider(ConfigRange(min, max), pos - new Vector2(0, 3f), 160) { _dNum = (byte)dNum };
        }

        protected override float GetValue(UIconfig element) => (element as OpFloatSlider).GetValueFloat();
    }
}
