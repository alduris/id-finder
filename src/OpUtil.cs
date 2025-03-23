using System;
using UnityEngine;

namespace FinderMod
{
    // Credit for solution: Visible ID mod https://github.com/SirRandom/rainworld-visibleid
    internal sealed class OpUtil : OptionInterface
    {
        private OpUtil() { }
        public readonly static OpUtil Instance = new();

        public static Configurable<T> CosmeticBind<T>(T init) => new(Instance, null, init, null);
        public static Configurable<T> CosmeticRange<T>(T val, T min, T max) where T : IComparable => new(val, new ConfigAcceptableRange<T>(min, max));

        public static readonly Color RedColor = new(0.85f, 0.5f, 0.55f);
        public static readonly Color YellowColor = new(0.95f, 0.9f, 0.65f);
        public static readonly Color GreenColor = new(0.65f, 0.95f, 0.8f);
        public static readonly Color BlueColor = new(0.5f, 0.65f, 0.95f);
    }
}
