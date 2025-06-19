namespace FinderMod.Inputs.LizardCosmetics
{
    /// <summary>
    /// Denotes a subholder that has controls which may toggle a child element.
    /// </summary>
    public interface IToggleableChildren
    {
        /// <summary>
        /// Checks if a child subholder is toggled.
        /// </summary>
        /// <param name="child">The child subholder to check</param>
        /// <returns>Returns true if the subholder is the element's child and is toggled on.</returns>
        public bool IsChildToggled(Subholder child);
    }
}
