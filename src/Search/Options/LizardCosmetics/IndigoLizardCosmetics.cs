using System;
using FinderMod.Inputs.LizardCosmetics;
using UnityEngine;
using static FinderMod.Search.Util.LizardUtil;

namespace FinderMod.Search.Options.LizardCosmetics
{
    internal class IndigoLizardCosmetics : BaseLizardCosmetics, ICanGPUSometimes
    {
        private readonly SkinkSpecklesCosmetic skinkSpecklesCosmetic;

        public IndigoLizardCosmetics() : base(LizardType.Indigo)
        {
            cosmetics.Add(skinkSpecklesCosmetic = new SkinkSpecklesCosmetic());
        }

        public bool AllowGPU => rotTypeInput == null || rotTypeInput.value == RotType.None;
        public ComputeShader Shader => InternalShaders.indigoLizardCosmeticsShader;

        public ICanGPU.GPUInput[] GetGPUInputs()
        {
            return [
                .. skinkSpecklesCosmetic.GetGPUInputs(false)
                ];
        }

        public override float Execute(XORShift128 Random)
        {
            float r = 0f;
            foreach (var result in GetResults(Random))
            {
                if (result is SkinkSpecklesVars skinkSpecklesVars)
                {
                    r += skinkSpecklesCosmetic.Distance(skinkSpecklesVars);
                }
                else if (result is LizardRotVars lizardRotVars)
                {
                    r += lizardRotCosmetic!.Distance(lizardRotVars);
                }
                else if (result is not SkinkStripesVars)
                {
                    throw new InvalidOperationException("Unexpected result! " + result.GetType().Name);
                }
            }
            return r;
        }
    }
}
