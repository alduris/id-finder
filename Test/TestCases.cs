using System;
using FinderMod.Search;
using FinderMod.Search.Options;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FinderMod.Test
{
    internal static class TestCases
    {
        public static float IGNORE = float.MaxValue;

        public static void TestCase(string group, int id, float[] expected) => TestCase(group, id, 0, expected);

        public static void TestCase(string group, int id, int offset, float[] expected)
        {
            IOption option = SearchOptions.Options[group];

            // Count how many random values we need to get (guaranteed at least 9 floats, personality requires it)
            int floatCount = Math.Max(9, option.MinFloats);
            float[] floats = new float[floatCount];
            float[] values = new float[option.NumOutputs];
            float[] personality = new float[6];
            Random.State[] states = new Random.State[option.NumOutputs];

            // Get the random values
            SearchUtil.GetRandomFloats(id, floatCount, floats, states);
            SearchUtil.FillPersonality(floats, personality);
            option.Run(floats, values, new SearchData
            {
                Seed = id,
                Personality = new Personality
                {
                    Aggression = personality[0],
                    Bravery = personality[1],
                    Dominance = personality[2],
                    Energy = personality[3],
                    Nervous = personality[4],
                    Sympathy = personality[5]
                },
                States = states
            });

            // Compare
            for (int i = 0; i < expected.Length; i++)
            {
                if (expected[i] == IGNORE) continue;
                if (values[i + offset] != expected[i] && !Mathf.Approximately(values[i + offset], expected[i]))
                {
                    Plugin.logger.LogDebug($"{group} test case {id} failed at result {i}! Expected: {expected[i]}; Found: {values[i + offset]}");
                }
            }
        }
    }
}
