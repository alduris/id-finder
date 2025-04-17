using System;
using System.Runtime.CompilerServices;
using FinderMod.Inputs;
using RWCustom;
using UnityEngine;

namespace FinderMod.Search.Options
{
    public partial class Option
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ClampedRandomVariation(float baseValue, float maxDeviation, float k, XORShift128 Random)
        {
            return Mathf.Clamp(baseValue + Custom.SCurve(Random.Value * 0.5f, k) * 2f * ((Random.Value < 0.5f) ? 1f : -1f) * maxDeviation, 0f, 1f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float WrappedRandomVariation(float baseValue, float maxDeviation, float k, XORShift128 Random)
        {
            float num = baseValue + Custom.SCurve(Random.Value * 0.5f, k) * 2f * ((Random.Value < 0.5f) ? 1f : -1f) * maxDeviation + 1f;
            return num - Mathf.Floor(num);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Distance(float num, Input<float> target)
        {
            return Mathf.Abs(num - target.value) * target.bias;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Distance(float num, Input<int> target)
        {
            return Math.Abs(num - target.value) * target.bias;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DistanceIf(float num, Input<float>? target)
        {
            if (target != null && target.enabled) return Mathf.Abs(num - target.value) * target.bias;
            return 0f;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DistanceIf(float num, Input<int>? target)
        {
            if (target != null && target.enabled) return Math.Abs(num - target.value) * target.bias;
            return 0f;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DistanceIf(Color col, Input<Color>? target)
        {
            if (target != null && target.enabled)
            {
                return Vector4.Distance((Vector4)col, (Vector4)target.value) * target.bias;
            }
            return 0f;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DistanceIf(HSLColor col, ColorHSLInput? target)
        {
            if (target is null) return 0f;
            return WrapDistanceIf(col.hue, target.HueInput) + DistanceIf(col.saturation, target.SatInput) + DistanceIf(col.lightness, target.LightInput);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DistanceIf(bool b, Input<bool>? target)
        {
            if (target != null && target.enabled)
            {
                if (b != target.value) return target.bias;
            }
            return 0f;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DistanceIf(int? val, Toggleable<IntInput>? target)
        {
            if (target == null) return 0f;
            float r = DistanceIf(val.HasValue, target.ToggleInput);
            if (target.Toggled && val.HasValue)
            {
                return Math.Abs(target.Element.value - val.Value) * target.Element.bias;
            }
            return r;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DistanceIf(float? val, Toggleable<FloatInput>? target)
        {
            if (target == null) return 0f;
            float r = DistanceIf(val.HasValue, target.ToggleInput);
            if (target.Toggled && val.HasValue)
            {
                return Mathf.Abs(target.Element.value - val.Value) * target.Element.bias;
            }
            return r;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float WrapDistanceIf(float num, Input<float> target)
        {
            if (target != null && target.enabled)
                return Mathf.Min(Mathf.Abs(num - target.value), Mathf.Abs(num - (target.value + 1)), Mathf.Abs(num - (target.value - 1))) * target.bias;
            return 0f;
        }
    }
}
