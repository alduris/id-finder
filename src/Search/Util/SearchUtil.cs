using System;
using System.Runtime.CompilerServices;
using FinderMod.Inputs;
using RWCustom;
using UnityEngine;

namespace FinderMod.Search.Options
{
    public partial class Option
    {
        /// <summary>
        /// Personality struct. When used with the <see cref="XORShift128"/> constructor, generates personality values the same way as the game without changing the random state.
        /// </summary>
        public struct Personality
        {
            public Personality() => throw new InvalidOperationException("Please use XORShift128 argument");

            /// <summary>
            /// Initializes the personality struct and resets the random state when done so it can be used.
            /// </summary>
            /// <param name="Random"></param>
            public Personality(XORShift128 Random)
            {
                var (x, y, z, w) = (Random.x, Random.y, Random.z, Random.w);

                sym = Custom.PushFromHalf(Random.Value, 1.5f); // sympathy
                nrg = Custom.PushFromHalf(Random.Value, 1.5f); // energy
                brv = Custom.PushFromHalf(Random.Value, 1.5f); // bravery

                nrv = Mathf.Lerp(Random.Value, Mathf.Lerp(nrg, 1f - brv, 0.5f), Mathf.Pow(Random.Value, 0.25f)); // nervous; uses energy and bravery
                agg = Mathf.Lerp(Random.Value, (nrg + brv) / 2f * (1f - sym), Mathf.Pow(Random.Value, 0.25f));   // aggression; uses energy, bravery, and sympathy
                dom = Mathf.Lerp(Random.Value, (nrg + brv + agg) / 3f, Mathf.Pow(Random.Value, 0.25f));          // dominance; uses energy, bravery, and aggression

                nrv = Custom.PushFromHalf(nrv, 2.5f); // nervous
                agg = Custom.PushFromHalf(agg, 2.5f); // aggression

                Random.InitState(x, y, z, w);
            }

            public float agg;
            public float brv;
            public float dom;
            public float nrg;
            public float nrv;
            public float sym;
        }

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
            {
                float a = Custom.Decimal(num);
                float b = Custom.Decimal(target.value);
                return Mathf.Min(Mathf.Abs(a - b), Mathf.Abs(a - (b + 1f)), Mathf.Abs(a - (b - 1f))) * target.bias;
            }
            return 0f;
        }
    }
}
