using System;
using System.Collections.Generic;
using FinderMod.Inputs;
using FinderMod.Inputs.LizardCosmetics;
using static FinderMod.Inputs.LizardCosmetics.CosmeticsItemContainer;
using static FinderMod.Search.Util.LizardUtil;

namespace FinderMod.Search.Options.LizardCosmetics
{
    internal class CaramelLizardCosmetics : BaseLizardCosmetics
    {
        private readonly BodyStripesCosmetic bodyStripesCosmetic;
        private readonly SpineSpikesCosmetic spineSpikesCosmetic;
        private readonly LongShoulderScalesCosmetic longShoulderScalesCosmetic;
        private readonly ShortBodyScalesCosmetic shortBodyScalesCosmetic;
        private readonly BumpHawkCosmetic bumpHawkCosmetic;
        private readonly TailTuftCosmetic tailTuftCosmetic;
        private readonly LongHeadScalesCosmetic longHeadScalesCosmetic;
        private readonly ColorHSLInput bodyColorInput, headColorInput;

        public CaramelLizardCosmetics() : base(LizardType.Caramel)
        {
            cosmetics.Add(
                OneOf(
                    "Body cosmetic",
                    bodyStripesCosmetic = new BodyStripesCosmetic(type),
                    spineSpikesCosmetic = new SpineSpikesCosmetic(type),
                    longShoulderScalesCosmetic = new LongShoulderScalesCosmetic(type),
                    shortBodyScalesCosmetic = new ShortBodyScalesCosmetic(type),
                    bumpHawkCosmetic = new BumpHawkCosmetic(type),
                    None()
                    )
                );
            cosmetics.Add(Toggleable("Has TailTuft", tailTuftCosmetic = new TailTuftCosmetic(type)));
            cosmetics.Add(Toggleable("Has LongHeadScales", longHeadScalesCosmetic = new LongHeadScalesCosmetic()));

            if (ModManager.MSC) elements.Add(new Label("Note: colors may not be accurate in Saint's campaign"));
            elements.Add(bodyColorInput = new ColorHSLInput("Body color", true, 0.075f, 0.125f, true, 0.3f, 0.9f, true, 0.7f, 1f));
            elements.Add(headColorInput = new ColorHSLInput("Head color", true, 0.07f, 0.13f, false, 0.55f, 0.55f, true, 0.5f, 0.6f));
        }

        public override float Execute(XORShift128 Random)
        {
            float r = 0f;

            // Original head color
            HSLColor bodyColor, headColor;
            var (x, y, z, w) = (Random.x, Random.y, Random.z, Random.w);
            headColor = new HSLColor(WrappedRandomVariation(0.1f, 0.03f, 0.2f, Random), 0.55f, ClampedRandomVariation(0.55f, 0.36f, 0.2f, Random));
            Random.InitState(x, y, z, w);

            // Caramel cosmetics
            bool body = false;
            bool tail = false;
            bool lhs = false;
            foreach (var result in GetResults(Random))
            {
                switch (result)
                {
                    case BodyStripesVars bodyStripesVars:
                        body = true;
                        r += bodyStripesCosmetic.Distance(bodyStripesVars);
                        break;
                    case SpineSpikesVars spineSpikesVars:
                        body = true;
                        r += spineSpikesCosmetic.Distance(spineSpikesVars);
                        break;
                    case LongShoulderScalesVars longShoulderScalesVars:
                        body = true;
                        r += longShoulderScalesCosmetic.Distance(longShoulderScalesVars);
                        break;
                    case ShortBodyScalesVars shortBodyScalesVars:
                        body = true;
                        r += shortBodyScalesCosmetic.Distance(shortBodyScalesVars);
                        break;
                    case BumpHawkVars bumpHawkVars:
                        body = true;
                        r += bumpHawkCosmetic.Distance(bumpHawkVars);
                        break;

                    case TailTuftVars tailTuftVars:
                        tail = true;
                        r += tailTuftCosmetic.Distance(tailTuftVars);
                        break;

                    case LongHeadScalesVars longHeadScalesVars:
                        lhs = true;
                        r += longHeadScalesCosmetic.Distance(longHeadScalesVars);
                        break;

                    case LizardRotVars lizardRotVars:
                        r += lizardRotCosmetic.Distance(lizardRotVars);
                        break;

                    default:
                        throw new InvalidOperationException("Unexpected result! " + result.GetType().Name);
                }
            }

            bool wantedBodyCosmetic = bodyStripesCosmetic.Enabled &&  bodyStripesCosmetic.Toggled;
            wantedBodyCosmetic |= spineSpikesCosmetic.Enabled && spineSpikesCosmetic.Toggled;
            wantedBodyCosmetic |= longShoulderScalesCosmetic.Enabled && longShoulderScalesCosmetic.Toggled;
            wantedBodyCosmetic |= shortBodyScalesCosmetic.Enabled && shortBodyScalesCosmetic.Toggled;
            wantedBodyCosmetic |= bumpHawkCosmetic.Enabled && bumpHawkCosmetic.Toggled;
            if (wantedBodyCosmetic && !body) r += MISSING_PENALTY;

            bool wantedTailCosmetic = tailTuftCosmetic.Enabled && tailTuftCosmetic.Toggled;
            if (wantedTailCosmetic && !tail) r += MISSING_PENALTY;

            if (!lhs && longHeadScalesCosmetic.Enabled && longHeadScalesCosmetic.Toggled) r += MISSING_PENALTY;

            // Caramel colors
            Random.Shift(4); // techinically there is also a shift for saint but boo hoo I don't care

            float val = Random.Range(0.7f, 1f);
            if (val >= 0.8f)
            {
                // body color
                bodyColor = new HSLColor(Random.Range(0.075f, 0.125f), Random.Range(0.4f, 0.9f), val);

                // head color
                headColor = new HSLColor(WrappedRandomVariation(0.1f, 0.03f, 0.2f, Random), 0.55f, ClampedRandomVariation(0.55f, 0.05f, 0.2f, Random));
            }
            else
            {
                // body color
                bodyColor = new HSLColor(Random.Range(0.075f, 0.125f), Random.Range(0.3f, 0.5f), val);
            }

            r += WrapDistanceIf(bodyColor.hue, bodyColorInput.HueInput);
            r += DistanceIf(bodyColor.saturation, bodyColorInput.SatInput);
            r += DistanceIf(bodyColor.lightness, bodyColorInput.LightInput);

            r += WrapDistanceIf(headColor.hue, headColorInput.HueInput);
            r += DistanceIf(headColor.saturation, headColorInput.SatInput);
            r += DistanceIf(headColor.lightness, headColorInput.LightInput);

            return r;
        }

        protected override IEnumerable<string> GetValues(XORShift128 Random)
        {
            // Original head color
            HSLColor bodyColor, headColor;
            var (x, y, z, w) = (Random.x, Random.y, Random.z, Random.w);
            headColor = new HSLColor(WrappedRandomVariation(0.1f, 0.03f, 0.2f, Random), 0.55f, ClampedRandomVariation(0.55f, 0.36f, 0.2f, Random));
            Random.InitState(x, y, z, w);

            // Caramel cosmetics
            foreach (var str in base.GetValues(Random))
            {
                yield return str;
            }

            // Caramel colors
            Random.Shift(4); // techinically there is also a shift for saint but boo hoo I don't care

            float val = Random.Range(0.7f, 1f);
            if (val >= 0.8f)
            {
                // body color
                bodyColor = new HSLColor(Random.Range(0.075f, 0.125f), Random.Range(0.4f, 0.9f), val);

                // head color
                headColor = new HSLColor(WrappedRandomVariation(0.1f, 0.03f, 0.2f, Random), 0.55f, ClampedRandomVariation(0.55f, 0.05f, 0.2f, Random));
            }
            else
            {
                // body color
                bodyColor = new HSLColor(Random.Range(0.075f, 0.125f), Random.Range(0.3f, 0.5f), val);
            }

            yield return $"Body color: hsl({bodyColor.hue}, {bodyColor.saturation}, {bodyColor.lightness})";
            yield return $"Head color: hsl({headColor.hue}, {headColor.saturation}, {headColor.lightness})";
        }
    }
}
