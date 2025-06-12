using System;
using System.Collections.Generic;
using FinderMod.Inputs;
using RWCustom;
using UnityEngine;

namespace FinderMod.Search.Options
{
    public class SlupFoodOption : Option
    {
        private static readonly string[] foodList = [
            "Dangle fruit", "Water nut", "Jellyfish", "Slime mold", "Eggbug egg", "Fire egg", "Popcorn", "Gooieduck",
            "Lillypuck", "Glow weed", "Dandelion peach", "Neuron", "Centipede", "Small centipede", "Vulture grub",
            "Small noodlefly", "Hazer", "Pomegranate"
        ];
        private static readonly int foodLength = foodList.Length;
        private readonly FloatInput[] inputs;
        public SlupFoodOption() : base()
        {
            inputs = new FloatInput[foodLength];
            for (int i = 0; i < foodLength; i++)
            {
                inputs[i] = new FloatInput(foodList[i], -1f, 1f);
            }
            elements = [.. inputs];
        }

        private (float a, float b) GetTraits(Personality p, int i) => i switch
        {
            0 => (p.nrv, p.nrg),
            1 => (p.sym, p.agg),
            2 => (p.nrg, p.nrv),
            3 => (p.nrg, p.agg),
            4 => (p.dom, p.nrg),
            5 => (p.agg, p.sym),
            6 => (p.dom, p.brv),
            7 => (p.sym, p.brv),
            8 => (p.agg, p.nrv),
            9 => (p.nrv, p.nrg),
            10 => (p.brv, p.dom),
            11 => (p.brv, p.nrv),
            12 => (p.brv, p.dom),
            13 => (p.nrg, p.agg),
            14 => (p.dom, p.brv),
            15 => (p.agg, p.sym),
            16 => (p.nrv, p.sym),
            17 => (p.nrg, p.brv),
            _ => throw new Exception($"Invalid index ({i}) while processing food preferences")
        };

        public override float Execute(XORShift128 Random)
        {
            var p = new Personality(Random);

            float r = 0;

            for (int j = 0; j < foodLength; j++)
            {
                (float a, float b) = GetTraits(p, j);

                a *= Custom.PushFromHalf(Random.Value, 2f);
                b *= Custom.PushFromHalf(Random.Value, 2f);
                float o = Mathf.Clamp(Mathf.Lerp(a - b, Mathf.Lerp(-1f, 1f, Custom.PushFromHalf(Random.Value, 2f)), Custom.PushFromHalf(Random.Value, 2f)), -1f, 1f);
                r += DistanceIf(o, inputs[j]);
            }

            return r;
        }

        protected override IEnumerable<string> GetValues(XORShift128 Random)
        {
            var p = new Personality(Random);
            for (int j = 0; j < foodLength; j++)
            {
                (float a, float b) = GetTraits(p, j);

                a *= Custom.PushFromHalf(Random.Value, 2f);
                b *= Custom.PushFromHalf(Random.Value, 2f);
                float o = Mathf.Clamp(Mathf.Lerp(a - b, Mathf.Lerp(-1f, 1f, Custom.PushFromHalf(Random.Value, 2f)), Custom.PushFromHalf(Random.Value, 2f)), -1f, 1f);
                yield return $"{foodList[j]}: {o}";
            }
            yield break;
        }
    }
}
