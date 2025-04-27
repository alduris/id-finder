using System.Collections.Generic;
using FinderMod.Inputs;

namespace FinderMod.Search.Options
{
    public class CoalescipedeOption : Option
    {
        private readonly FloatInput SizeInput = new("Size");
        public CoalescipedeOption()
        {
            elements = [SizeInput];
        }

        public override float Execute(XORShift128 Random)
        {
            return DistanceIf(Random.Value, SizeInput);
        }

        protected override IEnumerable<string> GetValues(XORShift128 Random)
        {
            yield return $"Size: {Random.Value}";
        }
    }
}
