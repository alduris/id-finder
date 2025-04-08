using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using FinderMod.Inputs;
using Menu.Remix.MixedUI;
using Newtonsoft.Json.Linq;
using RWCustom;
using UnityEngine;
using static Menu.Menu;

namespace FinderMod.Search.Options
{
    /// <summary>
    /// A search option. Fill out <see cref="elements"/> in the constructor, and keep references so you can use the values in <see cref="Execute"/>.
    /// </summary>
    /// <param name="key">Serves as both the internal key the option will be identified as</param>
    public abstract class Option()
    {
        internal string name = "Search Option";

        internal bool firstOption = false;
        internal bool linked = false;

        /// <summary>
        /// The elements to show in the space. Must be added to in the constructor.
        /// </summary>
        protected List<IElement> elements = null!;

        public void CreateOptions(ref float y, List<UIelement> output)
        {
            const float MARGIN = 6f;

            if (!firstOption && !linked)
            {
                y -= 2f;
                output.Add(new OpImage(new Vector2(10f, y), "pixel") { scale = new Vector2(580f, 2f), color = MenuRGB(MenuColors.MediumGrey) });
                y -= MARGIN;
            }

            y -= 27f;
            var deleteButton = new OpSimpleButton(new Vector2(10f, y), new Vector2(24f, 24f), "\xd7") { colorEdge = OpUtil.RedColor, colorFill = OpUtil.RedColor };
            deleteButton.OnClick += (_) => OnDelete?.Invoke();
            var linkButton = new OpSimpleButton(new Vector2(40f, y), new Vector2(24f, 24f), linked ? "-" : "+") { colorEdge = OpUtil.GreenColor, colorFill = OpUtil.GreenColor };
            if (!firstOption)
            {
                linkButton.OnClick += (_) =>
                {
                    linked = !linked;
                    OnLink?.Invoke();
                };
            }
            else
            {
                linked = false;
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

        /// <summary>
        /// Values to display in the the values tab.
        /// </summary>
        /// <param name="id">The id of the seed. Feed into</param>
        /// <returns>Strings to display in labels, or null for whitespace.</returns>
        public IEnumerable<string> GetValues(int id)
        {
            var random = new XORShift128();
            random.InitState(id);
            foreach (var str in GetValues(random))
            {
                yield return str;
            }
        }
        protected abstract IEnumerable<string> GetValues(XORShift128 Random);


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Save stuff
        public JObject ToJson()
        {
            var children = new JObject();
            foreach (var child in elements)
            {
                if (child is ISaveInHistory saveable)
                {
                    children[saveable.SaveKey] = saveable.ToSaveData();
                }
            }
            return new JObject()
            {
                ["name"] = name,
                ["linked"] = linked,
                ["children"] = children
            };
        }

        public void FromJson(JObject json)
        {
            linked = (bool)json["linked"]!;
            HashSet<ISaveInHistory> saveable = elements.Where(x => x is ISaveInHistory).Cast<ISaveInHistory>().ToHashSet();
            foreach (var kvp in (JObject)json["children"]!)
            {
                var child = saveable.FirstOrDefault(x => x.SaveKey == kvp.Key);
                if (child != null)
                {
                    child.FromSaveData((kvp.Value as JObject)!);
                    saveable.Remove(child);
                }
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var child in elements)
            {
                if (child is ISaveInHistory saveable)
                {
                    foreach (var line in saveable.GetHistoryLines())
                    {
                        sb.AppendLine(line.ToString());
                    }
                }
            }
            return sb.ToString();
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Helper stuff

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
        public static float WrapDistanceIf(float num, Input<float> target)
        {
            if (target != null && target.enabled)
                return Mathf.Min(Mathf.Abs(num - target.value), Mathf.Abs(num - (target.value + 1)), Mathf.Abs(num - (target.value - 1))) * target.bias;
            return 0f;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Internal events

        internal event Action? OnDelete;
        internal event Action? OnLink;
    }
}
