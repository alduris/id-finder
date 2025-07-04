﻿using System.Collections.Generic;
using FinderMod.Inputs;
using UnityEngine;

namespace FinderMod.Search.Options
{
    internal class LanternMouseOption : Option
    {
        public LanternMouseOption() : base()
        {
            elements = [new HueInput("Hue"), new FloatInput("Dominance")];
        }

        private (float hue, float dominance) GetResults(XORShift128 Random)
        {
            float hue, dominance;
            if (Random.Value < 0.01f)
            {
                hue = Random.Value;
            }
            else
            {
                if (Random.Value < 0.5f)
                {
                    hue = Mathf.Lerp(0f, 0.1f, Random.Value);
                }
                else
                {
                    hue = Mathf.Lerp(0.5f, 0.65f, Random.Value);
                }
            }
            dominance = Random.Value;

            return (hue, dominance);
        }

        public override float Execute(XORShift128 Random)
        {
            var (hue, dominance) = GetResults(Random);
            return DistanceIf(hue, elements[0] as RangedInput<float>) + DistanceIf(dominance, elements[1] as RangedInput<float>);
        }

        protected override IEnumerable<string> GetValues(XORShift128 Random)
        {
            var (hue, dominance) = GetResults(Random);
            yield return $"Hue: {hue}";
            yield return $"Dominance: {dominance}";
        }
    }
}
