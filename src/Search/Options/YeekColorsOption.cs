using System;
using System.Collections.Generic;
using FinderMod.Inputs;
using UnityEngine;

namespace FinderMod.Search.Options
{
    public class YeekColorsOption : Option
    {
        private readonly ColorRGBInput FeatherColorInput = new("Feather color", new Color(0.9f, 0f, 0f)) { forceEnabled = true };

        public YeekColorsOption()
        {
            elements = [
                new Label("All other colors are based on the feather color. Cannot be yellow or green."),
                FeatherColorInput
                ];
        }

        private Color GetFeatherColor(XORShift128 Random)
        {
            var p = new Personality(Random);
            var groupLeaderPotential = (p.nrg + p.sym + p.brv) * Mathf.Clamp(p.dom, 0f, 1f);

            return new Color((1f - groupLeaderPotential) * Random.Range(0.4f, 0.9f), Mathf.Clamp(Random.Value * groupLeaderPotential, 0f, 1f), Mathf.Clamp(groupLeaderPotential, 0f, 1f));
        }

        public override float Execute(XORShift128 Random)
        {
            return DistanceIf(GetFeatherColor(Random), FeatherColorInput);
        }

        protected override IEnumerable<string> GetValues(XORShift128 Random)
        {
            var color = GetFeatherColor(Random);
            yield return $"Feather color: rgb({color.r}, {color.g}, {color.b})";
        }
    }
}
