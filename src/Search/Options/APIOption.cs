using System.Collections.Generic;
using FinderMod.Inputs;

namespace FinderMod.Search.Options
{
    /// <summary>
    /// Wrapper class used by the API if the user doesn't want to make their own Option. Do not use this to make your own option; use the <see cref="API"/>.
    /// </summary>
    public sealed class APIOption : Option
    {
        private readonly APIExecute run;
        internal APIOption(List<IElement> elements, APIExecute run)
        {
            this.elements = elements;
            this.run = run;
        }

        public delegate float APIExecute(XORShift128 Random, List<IElement> elements);

        public override float Execute(XORShift128 Random)
        {
            return run.Invoke(Random, elements);
        }

        protected override IEnumerable<string> GetValues(XORShift128 Random)
        {
            yield return "This option does not have a values implementation.";
            yield break;
        }
    }
}
