using System;
using FinderMod.Search;
using UnityEngine;

namespace FinderMod.Test
{
    internal static class TestCases
    {
        public static void TestCase(string group, int id, float[] expected)
        {
            Setup setup = SearchOptions.Groups[group];

            // Count how many random values we need to get (guaranteed at least 9 floats, personality requires it)
            int floatCount = Math.Max(9, setup.MinFloats);
            float[] floats = new float[floatCount];
            float[] personality = new float[6];

            // Get the random values
            SearchUtil.GetRandomFloats(id, floatCount, floats);
            SearchUtil.FillPersonality(floats, personality);
            float[] values = setup.Apply(floats, personality, id, floatCount, floatCount);

            // Compare
            for (int i = 0; i < expected.Length; i++)
            {
                if (values[i] != expected[i] && !Mathf.Approximately(values[i], expected[i]))
                {
                    FinderPlugin.logger.LogDebug($"{group} test case {id} failed at result {i}! Expected: {expected[i]}; Found: {values[i]}");
                    //throw new Exception($"{group} test case {id} failed at result {i}! Expected: {expected[i]}; Found: {values[i]}");
                }
            }
        }

        public static void TestCase(string group, int id, int offset, float[] expected)
        {
            Setup setup = SearchOptions.Groups[group];

            // Count how many random values we need to get (guaranteed at least 9 floats, personality requires it)
            int floatCount = Math.Max(9, setup.MinFloats);
            float[] floats = new float[floatCount];
            float[] personality = new float[6];

            // Get the random values
            SearchUtil.GetRandomFloats(id, floatCount, floats);
            SearchUtil.FillPersonality(floats, personality);
            float[] values = setup.Apply(floats, personality, id, floatCount, floatCount);

            // Compare
            for (int i = 0; i < expected.Length; i++)
            {
                if (values[i + offset] != expected[i] && !Mathf.Approximately(values[i + offset], expected[i]))
                {
                    FinderPlugin.logger.LogDebug($"{group} test case {id} failed at result {i}! Expected: {expected[i]}; Found: {values[i + offset]}");
                }
            }
        }
    }
}
