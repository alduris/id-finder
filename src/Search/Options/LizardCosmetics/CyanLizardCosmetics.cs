using System;
using FinderMod.Inputs.LizardCosmetics;
using static FinderMod.Inputs.LizardCosmetics.CosmeticsItemContainer;
using static FinderMod.Search.Util.LizardUtil;

namespace FinderMod.Search.Options.LizardCosmetics
{
    public class CyanLizardCosmetics : BaseLizardCosmetics
    {
        private readonly WingScalesCosmetic wingScalesInput;
        private readonly TailTuftCosmetic tailTuftInput;
        private readonly TailGeckoScalesCosmetic tailGeckoScalesInput;

        public CyanLizardCosmetics() : base(LizardType.Cyan)
        {
            cosmetics.Add(Toggleable("Has WingScales", wingScalesInput = new WingScalesCosmetic()));
            cosmetics.Add(OneOf(
                "Tail cosmetic",
                tailTuftInput = new TailTuftCosmetic(type),
                tailGeckoScalesInput = new TailGeckoScalesCosmetic()
                ));
            // cosmetics.Add(new JumpRingsCosmetic());
        }

        public override float Execute(XORShift128 Random)
        {
            float r = 0f;
            bool wingScales = false;

            foreach (var result in GetResults(Random))
            {
                switch (result)
                {
                    case WingScalesVars wingScalesVars:
                        wingScales = true;
                        r += wingScalesInput.Distance(wingScalesVars);
                        break;
                    case TailTuftVars tailTuftVars:
                        r += tailTuftInput.Distance(tailTuftVars);
                        break;
                    case TailGeckoScalesVars tailGeckoScalesVars:
                        r += tailGeckoScalesInput.Distance(tailGeckoScalesVars);
                        break;
                    case JumpRingsVars:
                        break;

                    case LizardRotVars lizardRotVars:
                        r += lizardRotCosmetic.Distance(lizardRotVars);
                        break;

                    default:
                        throw new InvalidOperationException("Unexpected result! " + result.GetType().Name);
                }
            }

            if (!wingScales && wingScalesInput.Enabled && wingScalesInput.Toggled) r += MISSING_PENALTY;

            return r;
        }
    }
}
