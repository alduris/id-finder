using UnityEngine;

namespace FinderMod.Search.Options
{
    /// <summary>
    /// Interface for search options that can use the GPU
    /// </summary>
    public interface ICanGPU
    {
        /// <summary>
        /// Returns GPU inputs to be loaded onto the created compute shaders.
        /// </summary>
        /// <returns></returns>
        public GPUInput[] GetGPUInputs();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ComputeShader CreateShader();

        /// <summary>
        /// Struct representing a single input. Transferred to the GPU.
        /// </summary>
        public struct GPUInput
        {
            /// <summary>
            /// Whether the input is enabled. In the GPU, treated like an integer.
            /// </summary>
            public bool enabled;

            /// <summary>
            /// The value of the input.
            /// </summary>
            public float value;

            /// <summary>
            /// The bias of the input.
            /// </summary>
            public int bias;
        }
    }
}
