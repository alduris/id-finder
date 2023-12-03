using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RWCustom;
using UnityEngine;
using Random = UnityEngine.Random;
//using System.Threading;

namespace FinderMod.Search
{
    internal class Query
    {
        public string Name;
        public Setup[] Setups;
        // Null = not searching that value, otherwise try to get close to the value if possible
        public float?[][] Requests;
        public int[][] Biases;
    }
    internal class PreQuery
    {
        public string Name;
        public Setup Setup;
        public float?[] Requests;
        public int[] Biases;
        public bool Linked;
    }

    internal class SearchUtil
    {
        private static readonly object _RandomLock = new();

        /**
         * Takes an input array and fills it with as many random floats as you need.
         * Modifies input array in an attempt to save on memory allocation.
         */
        public static void GetRandomFloats(int seed, int length, float[] floats)
        {
            lock (_RandomLock)
            {
                Random.State temp = Random.state;

                // Get floats
                Random.InitState(seed);
                for(int i = 0; i < length; i++)
                {
                    floats[i] = Random.value;
                }

                Random.state = temp;
            }
        }
        public static void GetRandomRanges(int seed, float[,] ranges, float[] output, int offset)
        {
            if (ranges == null) return;

            lock (_RandomLock)
            {
                Random.State temp = Random.state;

                // Get floats
                Random.InitState(seed);
                for(int i = 0; i < ranges.GetLength(0); i++)
                {
                    output[i + offset] = Random.Range(ranges[i, 0], ranges[i, 1]);
                }

                Random.state = temp;
            }
            //return floats;
        }
        public static void GetRandomRanges(int seed, int[,] ranges, float[] output, int offset)
        {
            if (ranges == null) return;

            lock (_RandomLock)
            {
                Random.State temp = Random.state;

                // Get floats
                Random.InitState(seed);
                for(int i = 0; i < ranges.GetLength(0); i++)
                {
                    output[i + offset] = Random.Range(ranges[i,0], ranges[i,1]);
                }

                Random.state = temp;
            }
            // return ints;
        }
        public static float GetRangeAt(int seed, float[] range, int index)
        {
            float res;
            float store = 0f;
            lock (_RandomLock)
            {
                Random.State temp = Random.state;

                // Get floats
                Random.InitState(seed);
                for (int i = 0; i < index; i++)
                {
                    store += Random.value;
                }
                res = Random.Range(range[0], range[1]);

                Random.state = temp;
            }
            return res;
        }
        public static int GetRangeAt(int seed, int[] range, int index)
        {
            int res;
            float store = 0f;
            lock (_RandomLock)
            {
                Random.State temp = Random.state;

                // Get floats
                Random.InitState(seed);
                for (int i = 0; i < index; i++)
                {
                    store += Random.value;
                }
                res = Random.Range(range[0], range[1]);

                Random.state = temp;
            }
            return res;
        }

        public static float Distance(float num, float target)
        {
            return Mathf.Abs(num - target);
        }
        public static float WrapDistance(float num, float target)
        {
            return Mathf.Min(Mathf.Abs(num - target), Mathf.Abs(num - (target + 1)), Mathf.Abs(num - (target - 1)));
        }
        
        public static void FillPersonality(float[] i, float[] p)
        {
            p[5] = Custom.PushFromHalf(i[0], 1.5f); // sympathy
            p[3] = Custom.PushFromHalf(i[1], 1.5f); // energy
            p[1] = Custom.PushFromHalf(i[2], 1.5f); // bravery

            p[4] = Mathf.Lerp(i[3], Mathf.Lerp(p[3], 1f - p[1], 0.5f), Mathf.Pow(i[4], 0.25f)); // nervous; uses energy and bravery
            p[0] = Mathf.Lerp(i[5], (p[3] + p[1]) / 2f * (1f - p[5]), Mathf.Pow(i[6], 0.25f));  // aggression; uses energy, bravery, and sympathy
            p[2] = Mathf.Lerp(i[7], (p[3] + p[1] + p[0]) / 3f, Mathf.Pow(i[8], 0.25f));         // dominance; uses energy, bravery, and aggression

            p[4] = Custom.PushFromHalf(p[4], 2.5f); // nervous
            p[0] = Custom.PushFromHalf(p[0], 2.5f); // aggression
        }

        public static uint PositiveDirGap(int i, int j, int div)
        {
            // Unchecked type cast because we want the negative bit to count as a 2**31 bit without throwing errors (it does otherwise for some reason)
            uint a = unchecked((uint)i), b = unchecked((uint)j + 1);
            uint dist = b > a ? (b - a) : 0xffffffffu - (a - b);
            return (uint)(dist / div);
        }


        public static (int, float)[,] finalResults;
        public static bool done = false;
        private static Task[] tasks;
        public static bool abort = false;
        public static string abortReason;
        public static double[] progress;

        public static async void Search(Query[] queries, (int, int) range, int threadCount, int resultsPer)
        {
            // Create threads
            unchecked
            {
                uint diff = PositiveDirGap(range.Item1, range.Item2, 1);
                if (diff < threadCount)
                {
                    threadCount = Math.Max(1, (int)diff);
                }
            }
            resultsPer = Math.Min(resultsPer, Math.Abs(range.Item2 - range.Item1));
            tasks = new Task[threadCount];

            var results = new (int, float)[threadCount, queries.Length, resultsPer];
            progress = new double[threadCount];
            finalResults = null;

            abort = false;
            abortReason = null;
            done = false;

            if (threadCount == 1)
            {
                // A single thread
                await Task.Run(() => SearchHelper(results, 0, range.Item1, range.Item2, queries));
            }
            else
            {
                // Need to divy up the id searching
                int gap = (int)PositiveDirGap(range.Item1, range.Item2, threadCount);
                for (int i = 0; i < threadCount; i++)
                {
                    int min = range.Item1 + gap * i;
                    int max = range.Item1 + gap * (i+1) - 1;
                    if (i == threadCount - 1) max = range.Item2;
                    int j = i;
                    FinderPlugin.logger.LogInfo("THREAD " + i + ": " + min + ", " + max);
                    tasks[i] = Task.Run(() => SearchHelper(results, j, min, max, queries));
                }
                for (int i = 0; i < threadCount; i++)
                {
                    await tasks[i];
                    tasks[i].Dispose();
                }
            }

            // if (abort) return;

            // Compile and publish the results
            finalResults = new (int, float)[queries.Length, resultsPer];
            for (int i = 0; i < threadCount; i++)
            {
                if (i == 0)
                {
                    // If this is the first thread's result, set thresholds
                    for (int j = 0; j < queries.Length; j++)
                    {
                        for (int k = 0; k < resultsPer; k++)
                        {
                            finalResults[j,k] = results[i, j, k];
                        }
                    }
                }
                else
                {
                    // Refine thresholds if necessary for other threads' results
                    for (int j = 0; j < queries.Length; j++)
                    {
                        // This ugly mess is because many people requested multiple results per search.
                        // finalResults and results are sorted, so we just have to merge them.

                        int l = 0; // Pointer for results
                        for (int k = 0; k < resultsPer; k++)
                        {
                            if (results[i, j, l].Item2 < finalResults[j, k].Item2)
                            {
                                // Merge item into finalResults and shift lesser elements
                                var temp = results[i, j, l];
                                for (int m = resultsPer-1; m > k; m--)
                                {
                                    finalResults[j, m] = finalResults[j, m - 1];
                                }
                                finalResults[j, k] = temp;
                                l++;
                            }
                        }
                    }
                }
            }
            done = true;
            tasks = null;
        }

        private static void SearchHelper((int, float)[,,] results, int threadId, int min, int max, Query[] queries)
        {
            try {
                int i = min;
                uint gap = PositiveDirGap(min, max, 1);

                int resultsPer = results.GetLength(2);
                var topValues = new (int, float)[queries.Length, resultsPer];

                for (int j = 0; j < queries.Length; j++)
                {
                    for (int k = 0; k < resultsPer; k++)
                    {
                        topValues[j, k] = (0, float.MaxValue);
                    }
                }
                
                // Count how many random values we need to get (guaranteed at least 9 floats, personality requires it)
                int floatCount = 9, rangeCount = 0, intRangeCount = 0;
                for (int j = 0; j < queries.Length; j++)
                {
                    for (int k = 0; k < queries[j].Setups.Length; k++)
                    {
                        floatCount = Math.Max(floatCount, queries[j].Setups[k].MinFloats);
                        rangeCount = Math.Max(rangeCount, queries[j].Setups[k].FloatRanges?.GetLength(0) ?? 0);
                        intRangeCount = Math.Max(intRangeCount, queries[j].Setups[k].IntRanges?.GetLength(0) ?? 0);
                    }
                }
                float[] floats = new float[floatCount + rangeCount + intRangeCount];
                float[] personality = new float[6];
                int frStart = floatCount;
                int irStart = floatCount + rangeCount;

                // Run the loop
                do
                {
                    if (abort) break;
                    // Get the initial random numbers
                    GetRandomFloats(i, floatCount, floats);

                    // Find personality
                    FillPersonality(floats, personality);

                    // Check every query
                    for (int j = 0; j < queries.Length; j++)
                    {
                        if (abort) break;

                        Query query = queries[j];
                        float compute = 0f;

                        // Check every joined request in the query too
                        for (int k = 0; k < query.Setups.Length; k++)
                        {
                            Setup setup = query.Setups[k];
                            float?[] requests = query.Requests[k];
                            int[] biases = query.Biases[k];

                            // Get other random numbers
                            if (rangeCount > 0)
                            {
                                GetRandomRanges(i, setup.FloatRanges, floats, floatCount);
                            }
                            if (intRangeCount > 0)
                            {
                                // Yeah I'm putting ints in a float array. What're ya gonna do about it?
                                GetRandomRanges(i, setup.IntRanges, floats, floatCount + rangeCount);
                            }

                            // Calculate results
                            float[] values = setup.Apply(floats, personality, i, frStart, irStart);
                            int colorInpCount = 0;
                            int inputPtr = 0;

                            for (int l = 0; l < requests.Length; l++)
                            {
                                // Deal with whitespace and label stuff that we want to skip
                                InputType type = InputType.Float; // assigned so visual studio doesn't throw a fit
                                while (inputPtr < setup.Inputs.Length)
                                {
                                    type = setup.Inputs[inputPtr].Type;
                                    if (type != InputType.Whitespace && type != InputType.Label) break;
                                    inputPtr++;
                                }

                                // Deal with id if it's good and stuff
                                if (requests[l] != null)
                                {
                                    bool wrap = setup.Inputs[inputPtr].Wrap || type == InputType.Hue;
                                    float value = wrap ? WrapDistance(values[l], (float)requests[l]) : Distance(values[l], (float)requests[l]);
                                    value *= biases[inputPtr];
                                    compute += value;
                                }

                                // Deal with checking if we need to wrap
                                if (type == InputType.ColorRGB)
                                {
                                    if (colorInpCount == 2)
                                    {
                                        colorInpCount = 0;
                                        inputPtr++;
                                    }
                                    else colorInpCount++;
                                }
                                else
                                {
                                    inputPtr++;
                                }
                            }
                        }

                        // Determine new minimum
                        if (resultsPer == 1)
                        {
                            if (compute < topValues[j, 0].Item2)
                            {
                                topValues[j, 0] = (i, compute);
                            }
                        }
                        else
                        {
                            uint ptr = PositiveDirGap(min, i, 1);
                            if (ptr < 0 || ptr >= resultsPer)
                            {
                                ptr = (uint)resultsPer - 1;
                            }
                            if (topValues[j, ptr].Item2 > compute)
                            {
                                while (ptr > 0 && topValues[j, ptr - 1].Item2 > compute)
                                {
                                    topValues[j, ptr] = topValues[j, ptr - 1];
                                    ptr--;
                                }
                                topValues[j, ptr] = (i, compute);
                            }
                        }
                    }

                    // Next id
                    unchecked
                    {
                        progress[threadId] = (uint)(i - min) / (double)gap;
                    }
                }
                while (i++ != max); // Doing it this way to account for bits wrapping

                // Merge our found data with all thread results so we can sort it more later
                for (int j = 0; j < queries.Length; j++)
                {
                    for (int k = 0; k < resultsPer; k++)
                    {
                        results[threadId, j, k] = topValues[j, k];
                    }
                }
                FinderPlugin.logger.LogInfo("Thread " + threadId + (abort ? " aborted" : " finished"));
            }
            catch (Exception e)
            {
                FinderPlugin.logger.LogError("Thread " + threadId + " encountered exception: " + e.Message);
                FinderPlugin.logger.LogError(e);
                Abort("encountered exception");
            }
        }

        public static void Abort(string reason)
        {
            abort = true;
            abortReason = reason;
        }
    }
}
