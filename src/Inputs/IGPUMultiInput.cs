using FinderMod.Search;

namespace FinderMod.Inputs
{
    /// <summary>
    /// Similar to <see cref="IGPUInput"/> but returns multiple <see cref="ICanGPU.GPUInput"/>s.
    /// </summary>
    public interface IGPUMultiInput
    {
        /// <summary>
        /// Converts to multiple GPU inputs
        /// </summary>
        public ICanGPU.GPUInput[] GetGPUInputs();
    }
}
