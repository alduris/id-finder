using System;
using RWCustom;
using UnityEngine;

namespace FinderMod.Search
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
}
