using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using FinderMod.Inputs;
using Menu.Remix.MixedUI;
using RWCustom;
using Unity.Burst;
using UnityEngine;

namespace FinderMod.Search.Options
{
    /// <summary>
    /// A search option. Fill out <see cref="elements"/> in the constructor, and keep references so you can use the values in <see cref="Execute"/>.
    /// </summary>
    /// <param name="key">Serves as both the internal key the option will be identified as</param>
    public abstract class Option(string key)
    {
        private readonly string name = key;

        internal bool firstOption = false;
        internal bool linked = false;

        /// <summary>
        /// The elements to show in the space. Must be added to in the constructor.
        /// </summary>
        protected List<IElement> elements = null!;

        public void CreateOptions(ref float y, List<UIelement> output)
        {
            const float MARGIN = 6f;

            if (!firstOption)
            {
                y -= 2f;
                output.Add(new OpImage(new Vector2(10f, y), "pixel") { scale = new Vector2(580f, 2f) });
                y -= MARGIN;
            }

            y -= 27f;
            var deleteButton = new OpSimpleButton(new Vector2(10f, y), new Vector2(24f, 24f), "\xd7") { colorEdge = OpUtil.color_del, colorFill = OpUtil.color_del };
            deleteButton.OnClick += (_) => OnDelete?.Invoke();
            var linkButton = new OpSimpleButton(new Vector2(40f, y), new Vector2(24f, 24f), "+") { colorEdge = OpUtil.color_link, colorFill = OpUtil.color_link };
            if (firstOption)
            {
                linked = !linked;
                linkButton.OnClick += (_) => OnLink?.Invoke();
            }
            else
            {
                linkButton.greyedOut = true;
            }
            output.Add(deleteButton);
            output.Add(linkButton);

            y -= 3f;
            output.Add(new OpLabel(70f, y, name, true));

            foreach (var element in elements)
            {
                y -= MARGIN;
                element.Create(10f, ref y, output);
            }
        }

        /// <summary>
        /// Execution, assuming an arbitrary given seed.
        /// </summary>
        /// <param name="Random">The random number generator correlating with the seed.</param>
        /// <returns>The calculated distance</returns>
        public abstract float Execute(XORShift128 Random);

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Helper stuff

        protected struct Personality
        {
            public Personality(XORShift128 Random)
            {
                var (x,y,z,w) = (Random.x, Random.y, Random.z, Random.w);

                sym = Custom.PushFromHalf(Random.Value, 1.5f); // sympathy
                nrg = Custom.PushFromHalf(Random.Value, 1.5f); // energy
                brv = Custom.PushFromHalf(Random.Value, 1.5f); // bravery

                nrv = Mathf.Lerp(Random.Value, Mathf.Lerp(nrg, 1f - brv, 0.5f), Mathf.Pow(Random.Value, 0.25f)); // nervous; uses energy and bravery
                agg = Mathf.Lerp(Random.Value, (nrg + brv) / 2f * (1f - sym), Mathf.Pow(Random.Value, 0.25f));   // aggression; uses energy, bravery, and sympathy
                dom = Mathf.Lerp(Random.Value, (nrg + brv + agg) / 3f, Mathf.Pow(Random.Value, 0.25f));          // dominance; uses energy, bravery, and aggression

                nrv = Custom.PushFromHalf(nrv, 2.5f); // nervous
                agg = Custom.PushFromHalf(agg, 2.5f); // aggression

                Random.InitState(x,y,z,w);
            }

            public float agg;
            public float brv;
            public float dom;
            public float nrg;
            public float nrv;
            public float sym;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected internal static float ClampedRandomVariation(float baseValue, float maxDeviation, float k, XORShift128 Random)
        {
            return Mathf.Clamp(baseValue + Custom.SCurve(Random.Value * 0.5f, k) * 2f * ((Random.Value < 0.5f) ? 1f : -1f) * maxDeviation, 0f, 1f);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected internal static float WrappedRandomVariation(float baseValue, float maxDeviation, float k, XORShift128 Random)
        {
            float num = baseValue + Custom.SCurve(Random.Value * 0.5f, k) * 2f * ((Random.Value < 0.5f) ? 1f : -1f) * maxDeviation + 1f;
            return num - Mathf.Floor(num);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static float Distance(float num, float target)
        {
            return Mathf.Abs(num - target);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static float Distance(float num, Input<float> target)
        {
            return Mathf.Abs(num - target.value);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static float DistanceIf(float num, Input<float>? target)
        {
            if (target != null && target.enabled) return Mathf.Abs(num - target.value);
            return 0f;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static float DistanceIf(Color col, Input<Color>? target)
        {
            if (target != null && target.enabled)
            {
                return Vector4.Distance((Vector4)col, (Vector4)target.value);
            }
            return 0f;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static float WrapDistance(float num, float target)
        {
            return Mathf.Min(Mathf.Abs(num - target), Mathf.Abs(num - (target + 1)), Mathf.Abs(num - (target - 1)));
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Internal events

        internal event Action? OnDelete;
        internal event Action? OnLink;
    }
}
