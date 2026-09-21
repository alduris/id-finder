namespace FinderMod.Search
{
    /// <summary>
    /// Interface for search options that can use the GPU, but only in some circumstances.
    /// </summary>
    /// <remarks>
    /// Implementors should make sure to call <see cref="Options.Option.UpdateGPUCheckbox"/> when updating any inputs that may affect <see cref="AllowGPU"/>.
    /// </remarks>
    public interface ICanGPUSometimes : ICanGPU
    {
        /// <summary>
        /// Whether or not the option allows using the GPU
        /// </summary>
        public bool AllowGPU { get; }
    }
}
