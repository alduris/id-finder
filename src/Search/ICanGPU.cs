using System.Runtime.InteropServices;
using FinderMod.Inputs;
using UnityEngine;

namespace FinderMod.Search
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
        public ComputeShader Shader { get; }

        /// <summary>
        /// Struct representing a single input. Transferred to the GPU.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 4, Size = 12)]
        public struct GPUInput(float value, float range, int bias)
        {
            /// <summary>
            /// The value of the input.
            /// </summary>
            public float value = value;

            /// <summary>
            /// The range of the input.
            /// </summary>
            public float range = range;

            /// <summary>
            /// The bias of the input. Setting to 0 disables the input.
            /// </summary>
            public int bias = bias;

            /// <summary>
            /// Returns information about this GPUInput
            /// </summary>
            /// <returns>Information</returns>
            public readonly override string ToString()
            {
                return $"GPU Input (value: {value}, range: {range}, bias: {bias})";
            }
        }
    }
}
