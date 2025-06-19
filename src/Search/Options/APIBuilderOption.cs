using System.Collections.Generic;
using FinderMod.Inputs;

namespace FinderMod.Search.Options
{
    internal sealed class APIBuilderOption : Option
    {
        private readonly List<IElement> options;
        private readonly OptionBuilder.OptionExecute executeFunc = null!;
        private readonly OptionBuilder.OptionValues valuesFunc = null!;

        public APIBuilderOption(OptionBuilder.OptionElements elementFunc, OptionBuilder.OptionExecute executeFunc, OptionBuilder.OptionValues valuesFunc)
        {
            options = [.. elementFunc()];
            this.executeFunc = executeFunc;
            this.valuesFunc = valuesFunc;
        }

        public override float Execute(XORShift128 Random)
        {
            return executeFunc.Invoke(Random, options);
        }

        protected override IEnumerable<string> GetValues(XORShift128 Random)
        {
            if (valuesFunc == null)
            {
                yield return "This option does not have a values implementation.";
            }
            else
            {
                foreach (var value in valuesFunc(Random))
                {
                    yield return value;
                }
            }
        }
    }
}
