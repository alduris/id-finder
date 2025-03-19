using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using FinderMod.Search.Options;

namespace FinderMod.Search
{
    internal class Threadmaster
    {
        private readonly List<Option> options;
        private readonly int threads;
        private readonly int results;
        private readonly (int min, int max) range;
        private readonly bool gpu;
        private readonly List<Task> tasks = [];
        private readonly int distinctLinks;

        private readonly float[] progress;
        private readonly Result[,,] output;
        private bool started = false;
        private bool abort = false;
        private int finished = 0;

        public bool Running => finished != threads && started && !abort;
        public float Progress => progress.Sum() / threads;
        public string? AbortReason { get; private set; } = null;

        public Threadmaster(List<Option> options, int threads, int results, (int min, int max) range, bool gpu)
        {
            var span = PositiveDirGap(range.min, range.max, 1);
            this.options = options;
            this.threads = threads;
            this.results = results;
            this.range = range;
            this.gpu = gpu;
            progress = new float[threads];
            distinctLinks = options.Count - options.Count(x => x.linked);
            output = new Result[threads, distinctLinks, results];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint PositiveDirGap(int i, int j, int div)
        {
            unchecked
            {
                if (i - 1 == j) return 0xffffffffu / (uint)div; // edge case fix thingy

                // Unchecked type cast because we want the negative bit to count as a 2**31 bit without throwing errors (it does otherwise for some reason)
                uint a = (uint)i;
                uint b = (uint)j + 1;
                uint dist = b > a ? (b - a) : 0xffffffffu - (a - b);
                return (uint)(dist / div);
            }
        }

        public void Run()
        {
            abort = false;
            if (Running) return;
            started = true;
            if (gpu) throw new NotImplementedException();

            uint gap = PositiveDirGap(range.min, range.max, threads);
            for (int i = 0; i < threads; i++)
            {
                unchecked
                {
                    uint min = (uint)range.min + gap * (uint)i;
                    uint max = (uint)range.min + gap * (uint)(i + 1) - 1;
                    if (i == threads - 1) max = (uint)range.max;
                    int j = i;
                    Plugin.logger.LogInfo("THREAD " + (i + 1) + ": " + min + ", " + max);
                    tasks.Add(Task.Run(() => RunThread(min, max, j)));
                }
            }
        }

        private void RunThread(uint min, uint max, int thread)
        {
            unchecked
            {
                // Create local storage mediums
                XORShift128 rng = new();

                var res = new Result[distinctLinks, results];
                for (int i = 0; i < res.GetLength(0); i++)
                {
                    for (int j = 0; j < res.GetLength(1); j++)
                    {
                        res[i,j] = new Result
                        {
                            id = 0,
                            dist = float.MaxValue
                        };
                    }
                }

                var calls = new Func<XORShift128, float>[options.Count];
                for (int i = 0; i < calls.Length; i++) calls[i] = options[i].Execute; // we resolve the callvirt here so hopefully quicker to call :pleading:

                // Caches and local stuff because quicker I think
                int opts = options.Count;
                ref float prog = ref progress[thread];

                // Search
                Plugin.logger.LogDebug(thread + ": " + res.GetLength(0) + "," + res.GetLength(1) + " / " + distinctLinks + "," + results);
                try
                {
                    uint gap = PositiveDirGap((int)min, (int)max, 1);
                    uint i = min;
                    do
                    {
                        int link = 0;
                        Result r = new()
                        {
                            id = (int)i
                        };
                        for (int j = 0; j < opts; j++)
                        {
                            if (j > 0 && !options[j].linked)
                            {
                                // sort in and regenerate
                                int k = results - 1;
                                if (res[link, k].dist > r.dist)
                                {
                                    res[link, k] = r;
                                    while (--k >= 0 && res[link, k].dist > r.dist)
                                    {
                                        (res[link, k], res[link, k + 1]) = (res[link, k + 1], res[link, k]);
                                    }
                                }
                                link++;
                                r = new Result() { id = (int)i };
                            }
                            rng.InitState(i);
                            r.dist += calls[j].Invoke(rng);
                        }

                        // sort in but don't regenerate
                        int k2 = results - 1;
                        if (res[link, k2].dist > r.dist)
                        {
                            res[link, k2] = r;
                            while (--k2 >= 0 && res[link, k2].dist > r.dist)
                            {
                                (res[link, k2], res[link, k2 + 1]) = (res[link, k2 + 1], res[link, k2]);
                            }
                        }

                        // update progress
                        prog = (float)(i - min) / gap;
                    }
                    while (i++ != max && !abort);
                }
                catch (Exception e)
                {
                    Abort("encountered exception");

                    Plugin.logger.LogError("Thread " + thread + " encountered exception: " + e.Message + "\n" + e.ToString());
                }

                // Sort and return
                for (int i = 0; i < distinctLinks; i++)
                {
                    for (int j = 0; j < results; j++)
                    {
                        output[thread, i, j] = res[i, j];
                    }
                }

                finished++;
            }
        }

        /// <summary>
        /// Returns search results such that the first dimension is each query and the second dimension is the results from each query.
        /// </summary>
        /// <returns>Results in a two-dimensional array with queries on the outer and individual results on the inner</returns>
        public Result[][] GetResults()
        {
            if (Running || !started) throw new InvalidOperationException("Please wait until the operation is complete, or abort first.");
            List<List<Result>> combinedResults = [];
            for (int j = 0; j < output.GetLength(1); j++)
            {
                combinedResults.Add([]);
                for (int i = 0; i < output.GetLength(0); i++)
                {
                    for (int k = 0; k < output.GetLength(2); k++)
                    {
                        combinedResults[j].Add(output[i, j, k]);
                    }
                }
                combinedResults[j] = combinedResults[j]
                    .OrderByDescending(x => x.dist) // sort so the biggest distances are at the front
                    .Skip((threads - 1) * results)  // remove the extras created by having multiple threads, which only does the biggest ones which we don't want anyway
                    .Reverse()                      // reverse so the smaller distances are at the front
                    .ToList();
            }
            return combinedResults.Select(x => x.ToArray()).ToArray(); //wheeee!!!
        }

        public void Abort(string reason)
        {
            abort = true;
            AbortReason = reason;
        }

        public struct Result
        {
            public int id;
            public float dist;
        }
    }
}
