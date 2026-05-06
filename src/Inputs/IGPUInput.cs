using FinderMod.Search;

namespace FinderMod.Inputs
{
    /// <summary>
    /// Denotes an input that is able to be turned into a <see cref="ICanGPU.GPUInput"/>
    /// </summary>
    public interface IGPUInput
    {
        /// <summary>
        /// Converts into a <see cref="ICanGPU.GPUInput"/>
        /// </summary>
        /// <returns>The created <see cref="ICanGPU.GPUInput"/></returns>
        public ICanGPU.GPUInput AsGPUInput();
    }
}
