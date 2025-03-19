using FinderMod.Inputs;
using Unity.Burst;
using UnityEngine;

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
                var inp = elements[i] as Input<float>;
                if (inp!.enabled) r += Mathf.Abs(inp.value - p[i]);
            }
            return r;
        }
    }
}
