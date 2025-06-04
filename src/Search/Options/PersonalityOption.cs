using System.Collections.Generic;
using FinderMod.Inputs;
using Unity.Burst;

namespace FinderMod.Search.Options
{
    public class PersonalityOption : Option
    {
        public PersonalityOption() : base()
        {
            elements = [
                new FloatInput("Aggression"),
                new FloatInput("Bravery"),
                new FloatInput("Dominance"),
                new FloatInput("Energy"),
                new FloatInput("Nervous"),
                new FloatInput("Sympathy")
            ];
        }

        [BurstCompile]
        public override float Execute(XORShift128 Random)
        {
            Personality v = new(Random);

            float[] p = [v.agg, v.brv, v.dom, v.nrg, v.nrv, v.sym];

            float r = 0f;
            for (int i = 0; i < 6; i++)
            {
                var inp = elements[i] as RangedInput<float>;
                r += DistanceIf(p[i], inp);
            }
            return r;
        }

        protected override IEnumerable<string> GetValues(XORShift128 Random)
        {
            Personality p = new(Random);
            yield return $"Aggression: {p.agg}";
            yield return $"Bravery: {p.brv}";
            yield return $"Dominance: {p.dom}";
            yield return $"Energy: {p.nrg}";
            yield return $"Nervous: {p.nrv}";
            yield return $"Sympathy: {p.sym}";
            yield break;
        }
    }
}
