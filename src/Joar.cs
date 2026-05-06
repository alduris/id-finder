using System;

namespace FinderMod
{
    /// <summary>
    /// Summon him to die instantly
    /// </summary>
    public class Joar : Exception
    {
        /// <summary>
        /// Summon him to die instantly
        /// </summary>
        public Joar() : base()
        {
            throw new Joar();
        }
    }
}
