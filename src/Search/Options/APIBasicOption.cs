using System.Collections.Generic;
using FinderMod.Inputs;

namespace FinderMod.Search.Options
{
    /// <summary>
    /// Wrapper class used by the API if the user doesn't want to make their own Option. Do not use this directly; use the <see cref="API"/>.
    /// </summary>
    public sealed class APIBasicOption : Option
    {
        private readonly APIExecute run;
        internal APIBasicOption(List<IElement> elements, APIExecute run)
        {
            this.elements = elements;
            this.run = run;
        }

        /// <summary>
        /// Delegate for Execute method. Should return a distance.
        /// </summary>
        /// <param name="Random">
        /// Instance of <see cref="XORShift128"/> initialized with the seed of the creature to search.
        /// Use as you would <see cref="UnityEngine.Random"/>.
        /// </param>
        /// <param name="elements">List of elements provided by the user.</param>
        /// <returns></returns>
        public delegate float APIExecute(XORShift128 Random, List<IElement> elements);

        /// <summary>
        /// Invokes the provided <see cref="APIExecute"/> delegate to return a distance.
        /// </summary>
        /// <param name="Random">Instance of <see cref="XORShift128"/> initialized with the seed of the creature to search.</param>
        /// <returns></returns>
        public override float Execute(XORShift128 Random)
        {
            return run.Invoke(Random, elements);
        }

        /// <summary>
        /// Basic API option does not support GetValues. Returns an error message.
        /// </summary>
        /// <param name="Random">Instance of <see cref="XORShift128"/> initialized with the seed of the creature to search.</param>
        /// <returns>Message stating that this option does not have a values implementation.</returns>
        protected override IEnumerable<string> GetValues(XORShift128 Random)
        {
            yield return "This option does not have a values implementation.";
        }
    }
}
