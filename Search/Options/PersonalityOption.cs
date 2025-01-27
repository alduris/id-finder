using FinderMod.Inputs;
using RWCustom;
using Unity.Burst;
using UnityEngine;

namespace FinderMod.Search.Options
{
    public class PersonalityOption : Option
    {
        public PersonalityOption() : base("Personality")
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
            var p = new float[6];
            p[5] = Custom.PushFromHalf(Random.Value, 1.5f); // sympathy
            p[3] = Custom.PushFromHalf(Random.Value, 1.5f); // energy
            p[1] = Custom.PushFromHalf(Random.Value, 1.5f); // bravery

            p[4] = Mathf.Lerp(Random.Value, Mathf.Lerp(p[3], 1f - p[1], 0.5f), Mathf.Pow(Random.Value, 0.25f)); // nervous; uses energy and bravery
            p[0] = Mathf.Lerp(Random.Value, (p[3] + p[1]) / 2f * (1f - p[5]), Mathf.Pow(Random.Value, 0.25f));  // aggression; uses energy, bravery, and sympathy
            p[2] = Mathf.Lerp(Random.Value, (p[3] + p[1] + p[0]) / 3f, Mathf.Pow(Random.Value, 0.25f));         // dominance; uses energy, bravery, and aggression

            p[4] = Custom.PushFromHalf(p[4], 2.5f); // nervous
            p[0] = Custom.PushFromHalf(p[0], 2.5f); // aggression

            float r = 0f;
            for (int i = 0; i < 6; i++)
            {
                var inp = elements[i] as Input<float>;
                if (inp.enabled) r += Mathf.Abs(inp.Value - p[i]);
            }
            return r;
        }
    }
}
