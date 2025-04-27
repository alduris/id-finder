using System;
using System.Collections.Generic;
using FinderMod.Inputs;
using RWCustom;
using UnityEngine;

namespace FinderMod.Search.Options
{
    public class EggbugColorsOption : Option
    {
        private readonly bool FireBug;

        // private readonly HueInput BodyHueInput;
        private readonly HueInput EggHueInput;

        public EggbugColorsOption(bool firebug)
        {
            FireBug = firebug;
            elements = [
                // BodyHueInput = new HueInput("Body hue", FireBug ? 0.35f : -0.15f, FireBug ? 0.6f : 0.1f),
                EggHueInput = new HueInput("Eggs hue", FireBug ? (0.35f + EggBugGraphics.HUE_OFF) : (-0.15f + 1.5f), FireBug ? (0.6f + EggBugGraphics.HUE_OFF) : (0.1f + 1.5f)),
                ];
        }

        private float Hue(XORShift128 Random)
        {
            float body = Mathf.Lerp(FireBug ? 0.35f : (-0.15f), FireBug ? 0.6f : 0.1f, ClampedRandomVariation(0.5f, 0.5f, 2f, Random));
            float egg = Custom.Decimal(body + (FireBug ? EggBugGraphics.HUE_OFF : 1.5f));
            return egg;
        }

        public override float Execute(XORShift128 Random)
        {
            return WrapDistanceIf(Hue(Random), EggHueInput);
        }

        protected override IEnumerable<string> GetValues(XORShift128 Random)
        {
            return [$"Egg hue: {Hue(Random)}"];
        }
    }
}
