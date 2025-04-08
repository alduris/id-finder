using System;
using FinderMod.Inputs.LizardCosmetics;
using static FinderMod.Inputs.LizardCosmetics.CosmeticsItemContainer;
using static FinderMod.Search.Util.LizardUtil;

namespace FinderMod.Search.Options.LizardCosmetics
{
    public class SalamanderCosmetics : BaseLizardCosmetics
    {
        private readonly SpineSpikesCosmetic spineSpikesCosmetic;
        private readonly BumpHawkCosmetic bumpHawkCosmetic;
        private readonly AxolotlGillsCosmetic axolotlGillsCosmetic;
        private readonly TailFinCosmetic tailFinCosmetic;

        public SalamanderCosmetics() : base(LizardType.Salamander)
        {
            cosmetics.Add(axolotlGillsCosmetic = new AxolotlGillsCosmetic());
            cosmetics.Add(tailFinCosmetic = new TailFinCosmetic(type));
            cosmetics.Add(OneOf(
                "Body cosmetic",
                spineSpikesCosmetic = new SpineSpikesCosmetic(type),
                bumpHawkCosmetic = new BumpHawkCosmetic(type),
                None()
                ));
        }

        public override float Execute(XORShift128 Random)
        {
            float r = 0f;
            bool body = false;

            foreach (var result in GetResults(Random))
            {
                switch (result)
                {
                    case SpineSpikesVars spineSpikesVars:
                        body = true;
                        if (spineSpikesCosmetic.Active)
                        {
                            r += DistanceIf(spineSpikesVars.spineLength, spineSpikesCosmetic.LengthInput);
                            r += DistanceIf(spineSpikesVars.numScales, spineSpikesCosmetic.NumScalesInput);
                            r += DistanceIf(spineSpikesVars.graphic, spineSpikesCosmetic.GraphicInput);
                        }
                        else if (spineSpikesCosmetic.Enabled && !spineSpikesCosmetic.Toggled)
                        {
                            r += MISSING_PENALTY;
                        }
                        break;
                    case BumpHawkVars bumpHawkVars:
                        body = true;
                        if (bumpHawkCosmetic.Active)
                        {
                            r += DistanceIf(bumpHawkVars.spineLength, bumpHawkCosmetic.SpineLenInput);
                            r += DistanceIf(bumpHawkVars.numBumps, bumpHawkCosmetic.NumBumpsInput);
                            r += DistanceIf(bumpHawkVars.colored, bumpHawkCosmetic.ColoredInput);
                        }
                        else if (bumpHawkCosmetic.Enabled && !bumpHawkCosmetic.Toggled)
                        {
                            r += MISSING_PENALTY;
                        }
                        break;
                    case AxolotlGillsVars axolotlGillsVars:
                        r += DistanceIf(axolotlGillsVars.rigor, axolotlGillsCosmetic.RigorInput);
                        r += DistanceIf(axolotlGillsVars.numGills, axolotlGillsCosmetic.NumGillsInput);
                        r += DistanceIf(axolotlGillsVars.graphic, axolotlGillsCosmetic.GraphicInput);
                        break;

                    case TailFinVars tailFinVars:
                        r += DistanceIf(tailFinVars.spineLength, tailFinCosmetic.LengthInput);
                        r += DistanceIf(tailFinVars.undersideSize, tailFinCosmetic.UndersideSizeInput);
                        r += DistanceIf(tailFinVars.spineScaleX, tailFinCosmetic.ScaleXInput);
                        r += DistanceIf(tailFinVars.numScales, tailFinCosmetic.NumScalesInput);
                        r += DistanceIf(tailFinVars.graphic, tailFinCosmetic.GraphicInput);
                        r += DistanceIf(tailFinVars.colored, tailFinCosmetic.ColoredInput);
                        break;

                    case LizardRotVars lizardRotVars:
                        r += DistanceIf(lizardRotVars.numLegs, lizardRotCosmetic.NumTentaclesInput);
                        r += DistanceIf(lizardRotVars.numDeadLegs, lizardRotCosmetic.NumDeadTentaclesInput);
                        r += DistanceIf(lizardRotVars.numEyes, lizardRotCosmetic.NumEyesInput);
                        break;

                    default:
                        throw new InvalidOperationException("Unexpected result! " + result.GetType().Name);
                }
            }

            if (!body && (spineSpikesCosmetic.Active || bumpHawkCosmetic.Active))
            {
                r += MISSING_PENALTY;
            }

            return r;
        }
    }
}
