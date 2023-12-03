using UnityEngine;

namespace FinderMod
{
    // Credit for solution: Visible ID mod https://github.com/SirRandom/rainworld-visibleid
    internal sealed class OpUtil : OptionInterface
    {
        private OpUtil() { }
        public readonly static OpUtil Instance = new();

        public static Configurable<T> CosmeticBind<T>(T init) => new(Instance, null, init, null);
        // public static Configurable<float> CosmeticFloat(float val, float min, float max) => new(val, new ConfigAcceptableRange<float>(min, max));
        public static Configurable<int> CosmeticInt(int val, int min, int max) => new(val, new ConfigAcceptableRange<int>(min, max));

        public static readonly Color color_del = new(0.85f, 0.5f, 0.55f);
        public static readonly Color color_warn = new(0.95f, 0.9f, 0.65f);
        public static readonly Color color_link = new(0.65f, 0.95f, 0.8f);
        public static readonly Color color_start = new(0.5f, 0.65f, 0.95f);
    }
}
