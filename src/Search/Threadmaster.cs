using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using FinderMod.Search.Options;
using Unity.Burst;
using UnityEngine;
using UnityEngine.Rendering;

namespace FinderMod.Search
{
    /// <summary>
    /// Runs ID Finder queries
    /// </summary>
    public class Threadmaster
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

        /// <summary>Whether or not the search is running</summary>
        public bool Running => (gpu ? progress[progress.Length - 1] < 1f : finished != threads) && started && !abort;
        /// <summary>Progress from 0 to 1. Progress for GPU searches unsupported.</summary>
        public float Progress => gpu ? progress.Sum() / progress.Length : progress.Min();
        /// <summary>Reason for an abort, or null if not</summary>
        public string? AbortReason { get; private set; } = null;

        /// <summary>Whether or not the search is a GPU search</summary>
        public bool IsGPU => gpu;

        /// <summary>
        /// Initializes a searcher. Use <see cref="Run"/> to actually run the thing.
        /// </summary>
        /// <param name="options">Options to run with</param>
        /// <param name="threads">Number of threads to use</param>
        /// <param name="results">Number of results to return</param>
        /// <param name="range">Min and max, inclusive, as a tuple</param>
        /// <param name="gpu">Not implemented. Do not use.</param>
        public Threadmaster(List<Option> options, int threads, int results, (int min, int max) range, bool gpu)
        {
            var span = PositiveDirGap(range.min, range.max, 1);
            this.options = [.. options];
            this.threads = threads;
            this.results = results;
            this.range = range;
            this.gpu = gpu;
            distinctLinks = options.Count - options.Count(x => x.linked);
            progress = new float[gpu ? distinctLinks : threads];
            output = new Result[gpu ? 1 : threads, distinctLinks, results];
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

        /// <summary>Starts a search.</summary>
        /// <remarks>Does nothing if a search has already been started. Use <see cref="Abort(string)"/> to stop a search.</remarks>
        public void Run()
        {
            abort = false;
            AbortReason = null!;
            if (Running) return;
            finished = 0;
            started = true;

            // Handle GPU
            if (gpu)
            {
                if (!options.All(x => x is ICanGPU))
                {
                    Abort("To run a GPU search, all options must implement ICanGPU!");
                    return;
                }
                if (options.Any(x => x.linked))
                {
                    Abort("Options must not be linked!");
                    return;
                }
                RunGPU();
                return;
            }

            // Handle CPU
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

                // Mark as finished, but lock in case race condition
                lock (this)
                {
                    finished++;
                }
            }
        }

        private void RunGPU()
        {
            const int maxDispatchPerSide = 64;
            const int maxDispatchAtOnce = maxDispatchPerSide * maxDispatchPerSide; // 1GB of data
            const int unitDispatch = 32 * 32 * 32;

            ICanGPU[] gpuOptions = [.. options.Cast<ICanGPU>()];

            // Get info from shaders
            int[] kernels = [.. gpuOptions.Select(x => x.Shader.FindKernel("CS_IDFinderMain"))];
            int inputsProperty = Shader.PropertyToID("_IDFinderInputs");
            int resultsProperty = Shader.PropertyToID("_IDFinderResults");
            int startingIdProperty = Shader.PropertyToID("_IDFinderStart");
            int dispatchCountProperty = Shader.PropertyToID("_IDFinderDispatch");

            // Set up input buffers
            ComputeBuffer[] inputBuffers = new ComputeBuffer[gpuOptions.Length];
            for (int i = 0; i < gpuOptions.Length; i++)
            {
                var gpuInputs = gpuOptions[i].GetGPUInputs();
                inputBuffers[i] = new ComputeBuffer(gpuInputs.Length, 16);
                inputBuffers[i].SetData(gpuInputs);
                Plugin.logger.LogDebug($"GPU OPTION {i} INPUTS");
                foreach (var gpuInput in gpuInputs)
                {
                    Plugin.logger.LogDebug(gpuInput.ToString());
                }
            }

            // Set up output buffer
            var queue = new Queue<GPUQueueElement>();
            uint idsToSearch = PositiveDirGap(range.min, range.max, 1);
            int totalDispatches = (int)((idsToSearch + unitDispatch - 1) / unitDispatch); // integer division rounding up
            if (totalDispatches > maxDispatchAtOnce)
            {
                int startId = range.min;
                while (totalDispatches > 0)
                {
                    for (int i = 0; i < gpuOptions.Length; i++)
                    {
                        queue.Enqueue(new GPUQueueElement
                        {
                            startingId = startId,
                            dispatchCount = Math.Min(totalDispatches, maxDispatchAtOnce),
                            optionIndex = i,
                        });
                    }
                    totalDispatches -= maxDispatchAtOnce;
                }
                totalDispatches = maxDispatchAtOnce; // we know at least one is max dispatch length
            }
            else
            {

                for (int i = 0; i < gpuOptions.Length; i++)
                {
                    queue.Enqueue(new GPUQueueElement
                    {
                        startingId = range.min,
                        dispatchCount = totalDispatches,
                        optionIndex = i,
                    });
                }
            }
                var resultsBuffer = new ComputeBuffer(totalDispatches * unitDispatch, 8);

            // Init our specific stuff
            var actualResults = new Result[gpuOptions.Length, results];
            for (int i = 0; i < actualResults.GetLength(0); i++)
            {
                for (int j = 0; j < actualResults.GetLength(1); j++)
                {
                    actualResults[i, j] = new Result
                    {
                        id = 0,
                        dist = float.MaxValue
                    };
                }
            }
            Result[] rawResults = new Result[totalDispatches * unitDispatch];

            Plugin.logger.LogInfo($"Started GPU search! Length: {PositiveDirGap(range.min, range.max, 1)} Dispatches: {queue.Count}");

            // Dispatch the first instance
            GPUDispatch();
            return; // everything now gets handled asynchronously

            void GPUDispatch()
            {
                if (queue.Count == 0)
                {
                    DisposeBuffers();
                    return;
                }

                Plugin.logger.LogInfo("Dispatching!");

                var toDispatch = queue.Peek();
                var shader = gpuOptions[toDispatch.optionIndex].Shader;

                int dispatchX = Math.Min(toDispatch.dispatchCount, maxDispatchPerSide);
                int dispatchY = (toDispatch.dispatchCount + maxDispatchPerSide - 1) / maxDispatchPerSide; // integer division rounding up

                shader.SetInt(startingIdProperty, toDispatch.startingId);
                shader.SetInts(dispatchCountProperty, dispatchX, dispatchY, 1);
                shader.SetBuffer(kernels[toDispatch.optionIndex], inputsProperty, inputBuffers[toDispatch.optionIndex]);
                shader.SetBuffer(kernels[toDispatch.optionIndex], resultsProperty, resultsBuffer);
                shader.Dispatch(kernels[toDispatch.optionIndex], dispatchX, dispatchY, 1);
                AsyncGPUReadback.Request(resultsBuffer, GPUReadback);
            }

            void GPUReadback(AsyncGPUReadbackRequest request)
            {
                try
                {
                    if (request.hasError)
                    {
                        Abort("GPU readback reported error");
                        DisposeBuffers();
                        return;
                    }
                    else if (abort)
                    {
                        DisposeBuffers();
                        return;
                    }

                    Plugin.logger.LogInfo("Readbacking!");


                    // Read in new data
                    resultsBuffer.GetData(rawResults);

                    // Handle our existing results
                    var dispatch = queue.Dequeue();
                    for (int i = 0; i < rawResults.Length; i++)
                    {
                        int j = results - 1;
                        if (rawResults[i].dist < actualResults[dispatch.optionIndex, j].dist)
                        {
                            actualResults[dispatch.optionIndex, j] = rawResults[i];
                            while (j > 0 && actualResults[dispatch.optionIndex, j].dist < actualResults[dispatch.optionIndex, j - 1].dist)
                            {
                                (actualResults[dispatch.optionIndex, j], actualResults[dispatch.optionIndex, j - 1]) = (actualResults[dispatch.optionIndex, j - 1], actualResults[dispatch.optionIndex, j]);
                                j--;
                            }
                        }
                    }

                    // Dispatch a new thing (also handles if we can't dispatch anymore)
                    GPUDispatch();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    Plugin.logger.LogError(e);
                    Abort("Encountered error processing data");
                    throw;
                }
            }

            void DisposeBuffers()
            {
                Plugin.logger.LogInfo("Disposing!");

                foreach (var buffer in inputBuffers)
                {
                    buffer.Dispose();
                }
                resultsBuffer.Dispose();

                for (int i = 0; i < gpuOptions.Length; i++)
                {
                    for (int j = 0; j < results; j++)
                    {
                        output[0, i, j] = actualResults[i, j];
                    }
                }

                for (int i = 0; i < progress.Length; i++)
                {
                    progress[i] = 1f;
                }
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
                    .OrderByDescending(x => x.dist)          // sort so the biggest distances are at the front
                    .ThenBy(x => x, new IdComparer())        // also subsort so ids closer to 0 are favored, easier sharing/typing if lots of distance = 0
                    .Skip(gpu ? 0 : (threads - 1) * results) // since biggest are at the front, skip them until there are no more
                    .Reverse()                               // reverse so the smaller distances are at the front
                    .ToList();
            }
            return combinedResults.Select(x => x.ToArray()).ToArray(); //wheeee!!!
        }

        /// <summary>
        /// Aborts an ongoing search
        /// </summary>
        /// <param name="reason"></param>
        public void Abort(string reason)
        {
            abort = true;
            AbortReason = reason;
        }

        /// <summary>
        /// Contains a search result.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 4, Size = 8)]
        public struct Result
        {
            /// <summary>The id of the result</summary>
            public int id;
            /// <summary>The distance of the result from the actual query</summary>
            public float dist;
        }

        private class IdComparer : IComparer<Result>
        {
            public int Compare(Result x, Result y)
            {
                if (x.id == int.MinValue) return -1;
                if (y.id == int.MinValue) return 1;
                return Math.Abs(y.id) - Math.Abs(x.id);
            }
        }

        private struct GPUQueueElement
        {
            public int startingId;
            public int dispatchCount;
            public int optionIndex;
        }
    }
}
