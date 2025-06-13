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
        /// <para>
        /// Personality struct. When used with the <see cref="XORShift128"/> constructor, generates personality values the same way as the game without changing the random state.
        /// Field names are based on shortenings by the mod Visible ID.
        /// </para>
        /// <para>When using, ensure that it is created first or immediately after initializing a state so that it may make use of the seed properly.</para>
        /// </summary>
        public struct Personality
        {
            /// <summary>Do not use parameterless constructor.</summary>
            /// <exception cref="InvalidOperationException"></exception>
            public Personality() => throw new InvalidOperationException("Please use XORShift128 argument");

            /// <summary>
            /// Initializes the personality struct and resets the random state when done so it can be used.
            /// </summary>
            /// <param name="Random">The random state. Ensure that it is created before any other Random calls so it may make use of the seed properly.</param>
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

            /// <summary>Aggression</summary>
            public float agg;
            /// <summary>Bravery</summary>
            public float brv;
            /// <summary>Dominance</summary>
            public float dom;
            /// <summary>Energy</summary>
            public float nrg;
            /// <summary>Nervous</summary>
            public float nrv;
            /// <summary>Sympathy</summary>
            public float sym;
        }

        /// <summary>
        /// Simulates <see cref="Custom.ClampedRandomVariation(float, float, float)"/> using <see cref="XORShift128"/>.
        /// Creates an S-curve centered at <c>baseValue</c> and clamps between 0 and 1.
        /// </summary>
        /// <param name="baseValue">Median value</param>
        /// <param name="maxDeviation">Max value</param>
        /// <param name="k"></param>
        /// <param name="Random">Current state of random number generator</param>
        /// <returns>Value clamped between 0 and 1</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ClampedRandomVariation(float baseValue, float maxDeviation, float k, XORShift128 Random)
        {
            return Mathf.Clamp(baseValue + Custom.SCurve(Random.Value * 0.5f, k) * 2f * ((Random.Value < 0.5f) ? 1f : -1f) * maxDeviation, 0f, 1f);
        }

        /// <summary>
        /// Simulates <see cref="Custom.ClampedRandomVariation(float, float, float)"/> using <see cref="XORShift128"/>.
        /// Creates an S-curve centered at <c>baseValue</c> and wraps between 0 and 1.
        /// </summary>
        /// <param name="baseValue">Median value</param>
        /// <param name="maxDeviation">Max value</param>
        /// <param name="k"></param>
        /// <param name="Random">Current state of random number generator</param>
        /// <returns>Value wrapped between 0 and 1</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float WrappedRandomVariation(float baseValue, float maxDeviation, float k, XORShift128 Random)
        {
            float num = baseValue + Custom.SCurve(Random.Value * 0.5f, k) * 2f * ((Random.Value < 0.5f) ? 1f : -1f) * maxDeviation + 1f;
            return num - Mathf.Floor(num);
        }

        /// <summary>Determines the distance for a ranged float input.</summary>
        /// <param name="num">Value to compare</param>
        /// <param name="target">Input to check</param>
        /// <returns>Distance between 0 and 1 times bias</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DistanceIf(float num, RangedInput<float>? target)
        {
            if (target != null && target.enabled) return Mathf.Abs(num - target.value) / (target.max - target.min) * target.bias;
            return 0f;
        }

        /// <summary>Determines the distance for a ranged int input.</summary>
        /// <param name="num">Value to compare</param>
        /// <param name="target">Input to check</param>
        /// <returns>Distance between 0 and 1 times bias</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DistanceIf(int num, RangedInput<int>? target)
        {
            if (target != null && target.enabled) return Math.Abs(num - target.value) / (float)(target.max - target.min) * target.bias;
            return 0f;
        }

        /// <summary>Determines the distance for an RGB color input.</summary>
        /// <param name="col">Color to compare</param>
        /// <param name="target">Input to check</param>
        /// <returns>Distance</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DistanceIf(Color col, Input<Color>? target)
        {
            if (target != null && target.enabled)
            {
                return Vector4.Distance((Vector4)col, (Vector4)target.value) * target.bias;
            }
            return 0f;
        }

        /// <summary>Determines the distance for an HSL color input.</summary>
        /// <param name="col">Color to compare</param>
        /// <param name="target">Input to check</param>
        /// <returns>Distance</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DistanceIf(HSLColor col, ColorHSLInput? target)
        {
            if (target is null) return 0f;
            return WrapDistanceIf(col.hue, target.HueInput) + DistanceIf(col.saturation, target.SatInput) + DistanceIf(col.lightness, target.LightInput);
        }

        /// <summary>Determines the distance for a boolean input.</summary>
        /// <param name="b">Value to compare</param>
        /// <param name="target">Input to check</param>
        /// <returns>Distance between 0 and 1 times bias</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DistanceIf(bool b, Input<bool>? target)
        {
            if (target != null && target.enabled)
            {
                if (b != target.value) return target.bias;
            }
            return 0f;
        }

        /// <summary>Determines the distance for a toggleable ranged int input.</summary>
        /// <param name="val">Value to compare. Use <c>null</c> to also check against toggle.</param>
        /// <param name="target">Input to check</param>
        /// <returns>Distance between 0 and 1 times bias</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DistanceIf(int? val, Toggleable<IntInput>? target)
        {
            if (target == null) return 0f;
            float r = DistanceIf(val.HasValue, target.ToggleInput);
            if (target.Toggled && val.HasValue)
            {
                return Math.Abs(target.Element.value - val.Value) / (float)(target.Element.max - target.Element.min) * target.Element.bias;
            }
            return r;
        }

        /// <summary>Determines the distance for a toggleable ranged float input.</summary>
        /// <param name="val">Value to compare. Use <c>null</c> to also check against toggle.</param>
        /// <param name="target">Input to check</param>
        /// <returns>Distance between 0 and 1 times bias</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DistanceIf(float? val, Toggleable<FloatInput>? target)
        {
            if (target == null) return 0f;
            float r = DistanceIf(val.HasValue, target.ToggleInput);
            if (target.Toggled && val.HasValue)
            {
                return Mathf.Abs(target.Element.value - val.Value) / (target.Element.max - target.Element.min) * target.Element.bias;
            }
            return r;
        }

        /// <summary>Determines the distance for a ranged float input, wrapped between 0 and 1. Useful for <see cref="HueInput"/>.</summary>
        /// <param name="num">Value to compare</param>
        /// <param name="target">Input to check</param>
        /// <returns>Distance between 0 and 1 times bias</returns>
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
