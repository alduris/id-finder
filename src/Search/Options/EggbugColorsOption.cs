using System;
using System.Collections.Generic;
using FinderMod.Inputs;
using MoreSlugcats;
using RWCustom;
using UnityEngine;

namespace FinderMod.Search.Options
{
    internal class EggbugColorsOption : Option, ICanGPU
    {
        private readonly bool FireBug;

        // private readonly HueInput BodyHueInput;
        private readonly HueInput EggHueInput;

        public ComputeShader Shader => InternalShaders.eggbugColorsShader;

        public EggbugColorsOption(bool firebug)
        {
            RepresentedCreature = firebug ? MoreSlugcatsEnums.CreatureTemplateType.FireBug : CreatureTemplate.Type.EggBug;
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
            float hue = Hue(Random);
            float value = Custom.Decimal(EggHueInput.value);
            return EggHueInput.enabled ? Mathf.Min(Mathf.Abs(hue - (value - 1f)), Mathf.Min(Mathf.Abs(hue - value), Mathf.Abs(hue - (value + 1f)))) * EggHueInput.bias : 0f;
        }

        protected override IEnumerable<string> GetValues(XORShift128 Random)
        {
            return [$"Egg hue: {Hue(Random)}"];
        }

        public ICanGPU.GPUInput[] GetGPUInputs()
        {
            return [
                EggHueInput.AsGPUInput(),
                new ICanGPU.GPUInput(FireBug ? 1 : 0, 1, 0)
                ];
        }
    }
}
