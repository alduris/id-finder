using System.Collections;
using System.Collections.Generic;
using FinderMod.Inputs.LizardCosmetics;
using static FinderMod.Search.Util.LizardUtil;

namespace FinderMod.Search.Options.LizardCosmetics
{
    public abstract class BaseLizardCosmetics : Option
    {
        protected LizardType type;
        protected CosmeticsItemContainer cosmetics;
        public BaseLizardCosmetics(LizardType type)
        {
            this.type = type;
            elements = [cosmetics = new CosmeticsItemContainer()];
        }

        protected IEnumerable GetResults(XORShift128 Random)
        {
            // Calculate IVars ahead of time
            Random.Shift(5);
            float tailLength = ClampedRandomVariation(0.5f, 0.2f, 0.3f, Random) * 2f;
            Random.Shift(2);
            float tailColor = 0f;
            if (type != LizardType.White && Random.Value > 0.5f)
            {
                tailColor = Random.Value;
            }
            if (type == LizardType.Black) Random.Shift(2);

            // Extra offsetting
            Random.Shift(
                (type == LizardType.Caramel ? 6 : 4)
                + NumTailSegments(type)
                + NumTongueSegments(type)
                + 1 // head
                + 1 // melanistic salamander chance (yes it gets called even if not a salamander)
                );

            // Cosmetics! (finally)
            if (type == LizardType.Eel)
            {
                yield return new AxolotlGillsVars(Random);
                yield return new TailGeckoScalesVars(Random, tailColor);
                if (Random.Value < 0.75f)
                {
                    yield return new LongShoulderScalesVars(Random, tailLength, type);
                    yield return new TailFinVars(Random, tailLength, type);
                }
                else
                {
                    yield return new ShortBodyScalesVars(Random, tailLength, type);
                    if (Random.Value < 0.75f)
                    {
                        yield return new TailFinVars(Random, tailLength, type);
                    }
                    else
                    {
                        yield return new TailTuftVars(Random, tailLength, type);
                    }
                }
            }
            else if (type == LizardType.Zoop)
            {
                if (Random.Value < 0.175f)
                {
                    yield return new WingScalesVars(Random);
                }
                else
                {
                    yield return new SpineSpikesVars(Random, tailLength, type);
                }
                yield return new TailTuftVars(Random, tailLength, type);
            }

            if (type == LizardType.Cyan)
            {
                if (Random.Value < 0.75f)
                {
                    yield return new WingScalesVars(Random);
                }
                if (Random.Value < 0.5f && tailColor == 0f)
                {
                    yield return new TailTuftVars(Random, tailLength, type);
                }
                else
                {
                    yield return new TailGeckoScalesVars(Random, tailColor);
                }
                yield return new JumpRingsVars();
            }
            else if (type == LizardType.White)
            {
                if (Random.Value < 0.4f)
                {
                    yield return new BumpHawkVars(Random, tailLength, type);
                }
                else if (Random.Value < 0.4f)
                {
                    yield return new ShortBodyScalesVars(Random, tailLength, type);
                }
                else if (Random.Value < 0.2f)
                {
                    yield return new LongShoulderScalesVars(Random, tailLength, type);
                }
                else if (Random.Value < 0.2f)
                {
                    yield return new LongHeadScalesVars(Random, type);
                }
                if (Random.Value < 0.5f)
                {
                    yield return new TailTuftVars(Random, tailLength, type);
                }
            }
            else
            {
                int backDecals = 0;
                bool longShoulderScales = false;
                bool shortBodyScales = false;

                if (type == LizardType.Caramel && Random.Value < 0.6f)
                {
                    yield return new BodyStripesVars(Random, tailLength, type);
                    backDecals++;
                }
                else if (Random.Value < 0.06666667f || Random.Value < 0.8f && type == LizardType.Green || Random.Value < 0.7f && type == LizardType.Black)
                {
                    yield return new SpineSpikesVars(Random, tailLength, type);
                    backDecals++;
                }
                else if (Random.Value < 0.033333335f && type != LizardType.Caramel)
                {
                    yield return new BumpHawkVars(Random, tailLength, type);
                    backDecals++;
                }
                else if ((Random.Value < 0.04761905f || type == LizardType.Pink && Random.Value < 0.5f || type == LizardType.Red && Random.Value < 0.9f) && type != LizardType.Salamander)
                {
                    yield return new LongShoulderScalesVars(Random, tailLength, type);
                    longShoulderScales = true;
                    backDecals++;
                }
                else if ((Random.Value < 0.0625f || type == LizardType.Blue && Random.Value < 0.5f) && type != LizardType.Salamander)
                {
                    yield return new ShortBodyScalesVars(Random, tailLength, type);
                    shortBodyScales = true;
                    backDecals++;
                }
                else if (type == LizardType.Green && Random.Value < 0.5f)
                {
                    yield return new ShortBodyScalesVars(Random, tailLength, type);
                    shortBodyScales = true;
                    backDecals++;
                }

                if (type != LizardType.Salamander)
                {
                    if (type == LizardType.Caramel && Random.Value < 0.5f)
                    {
                        yield return new TailTuftVars(Random, tailLength, type);
                    }
                    else if (Random.Value < 0.11111111f || backDecals == 0 && Random.Value < 0.7f || type == LizardType.Pink && Random.Value < 0.6f || type == LizardType.Blue && Random.Value < 0.96f)
                    {
                        yield return new TailTuftVars(Random, tailLength, type);
                    }
                    else if (backDecals < 2 && type == LizardType.Green && Random.Value < 0.7f)
                    {
                        if (Random.Value < 0.5f || longShoulderScales || shortBodyScales)
                        {
                            yield return new TailTuftVars(Random, tailLength, type);
                        }
                        else
                        {
                            yield return new LongShoulderScalesVars(Random, tailLength, type);
                            longShoulderScales = true;
                            backDecals++;
                        }
                    }
                }

                if (Random.Value < (backDecals == 0 ? 0.7f : 0.1f) && type != LizardType.Salamander && type != LizardType.Yellow && (!longShoulderScales && Random.Value < 0.9f || Random.Value < 0.033333335f))
                {
                    yield return new LongHeadScalesVars(Random, type);
                }

                if (type == LizardType.Salamander)
                {
                    yield return new AxolotlGillsVars(Random);
                    yield return new TailFinVars(Random, tailLength, type);
                }
                else if (type == LizardType.Black)
                {
                    yield return new WhiskersVars(Random);
                }
                else if (type == LizardType.Yellow)
                {
                    yield return new AntennaeVars(Random);
                    if (backDecals == 0 && Random.Value < 0.6f)
                    {
                        yield return new ShortBodyScalesVars(Random, tailLength, type);
                        backDecals++;
                    }
                }
                else if (type == LizardType.Red || type == LizardType.Train)
                {
                    yield return new LongShoulderScalesVars(Random, tailLength, type);
                    yield return new SpineSpikesVars(Random, tailLength, type);
                    backDecals += 2;
                    if (Random.Value < 0.5f)
                    {
                        yield return new TailFinVars(Random, tailLength, type);
                    }
                    else
                    {
                        yield return new TailTuftVars(Random, tailLength, type);
                    }
                }

                if (backDecals == 0 && type == LizardType.Caramel)
                {
                    yield return new BumpHawkVars(Random, tailLength, type);
                    // backDecals++;
                }
            }

            if (type == LizardType.Zoop)
            {
                yield return new SnowAccumulationVars(Random);
            }

            // Caramel colors
            /*if (type == LizardType.Caramel)
            {
                // Offsetting nonsense
                Random.Shift(4); // techinically there is also a shift for saint but boo hoo I don't care


                float val = Random.Range(0.7f, 1f);
                if (val >= 0.8f)
                {
                    // body color
                    yield return new HSLColor(Random.Range(0.075f, 0.125f), Random.Range(0.4f, 0.9f), val).rgb;

                    // head color
                    yield return Custom.HSL2RGB(WrappedRandomVariation(0.1f, 0.03f, 0.2f, Random), 0.55f, ClampedRandomVariation(0.55f, 0.05f, 0.2f, Random));
                }
                else
                {
                    // body color
                    yield return new HSLColor(Random.Range(0.075f, 0.125f), Random.Range(0.3f, 0.5f), val).rgb;
                }
            }*/
        }

        protected override IEnumerable<string> GetValues(XORShift128 Random)
        {
            foreach (var value in GetResults(Random))
            {
                switch (value)
                {
                    case AntennaeVars antennae:
                        yield return "Has Antennae:";
                        yield return $"  Length: {antennae.length}";
                        yield return $"  Alpha: {antennae.alpha}";
                        break;
                    case AxolotlGillsVars axolotlGills:
                        yield return "Has AxolotlGills:";
                        yield return $"  Graphic: {axolotlGills.graphic}";
                        yield return $"  Rigor: {axolotlGills.rigor}";
                        yield return $"  Number of gills: {axolotlGills.numGills}";
                        break;
                    case BodyStripesVars bodyStripes:
                        yield return "Has BodyStripes:";
                        yield return $"  Number of scales: {bodyStripes.numScales}";
                        break;
                    case BumpHawkVars bumpHawk:
                        yield return "Has BumpHawk:";
                        yield return $"  Number of bumps: {bumpHawk.numBumps}";
                        yield return $"  Spine length: {bumpHawk.spineLength}";
                        yield return $"  Is colored: {bumpHawk.colored}";
                        break;
                    case JumpRingsVars:
                        yield return "Has JumpRings";
                        break;
                    case LongHeadScalesVars longHeadScales:
                        yield return "Has LongHeadScales:";
                        yield return $"  Length: {longHeadScales.length}";
                        yield return $"  Width: {longHeadScales.width}";
                        yield return $"  Rigor: {longHeadScales.rigor}";
                        yield return $"  Is colored: {longHeadScales.colored}";
                        break;
                    case LongShoulderScalesVars longShoulderScales:
                        yield return "Has LongShoulderScales:";
                        yield return $"  Min size: {longShoulderScales.minSize}";
                        yield return $"  Max size: {longShoulderScales.maxSize}";
                        yield return $"  Graphic: {longShoulderScales.graphic}";
                        yield return $"  Scale type: {longShoulderScales.scaleType}";
                        yield return $"  Number of scales: {longShoulderScales.numScales}";
                        yield return $"  Is colored: {longShoulderScales.colored}";
                        break;
                    case ShortBodyScalesVars shortBodyScales:
                        yield return "Has ShortBodyScaleVars:";
                        yield return $"  Scale type: {shortBodyScales.scaleType}";
                        yield return $"  Number of scales: {shortBodyScales.numScales}";
                        break;
                    case SnowAccumulationVars:
                        yield return "Has SnowAccumulation";
                        break;
                    case SpineSpikesVars spineSpikes:
                        yield return "Has SpineSpikes:";
                        yield return $"  Spine length: {spineSpikes.spineLength}";
                        yield return $"  Graphic: {spineSpikes.graphic}";
                        yield return $"  Number of spines: {spineSpikes.numScales}";
                        break;
                    case TailFinVars tailFin:
                        yield return "Has TailFin:";
                        yield return $"  Spine length: {tailFin.spineLength}";
                        yield return $"  Spine scale X: {tailFin.spineScaleX}";
                        yield return $"  Underside size: {tailFin.undersideSize}";
                        yield return $"  Graphic: {tailFin.graphic}";
                        yield return $"  Number of spines: {tailFin.numScales}";
                        yield return $"  Is colored: {tailFin.colored}";
                        break;
                    case TailGeckoScalesVars tailGeckoScales:
                        yield return "Has TailGeckoScales:";
                        yield return $"  Rows: {tailGeckoScales.rows}";
                        yield return $"  Lines: {tailGeckoScales.lines}";
                        break;
                    case TailTuftVars tailTuft:
                        yield return "Has TailTuft:";
                        yield return $"  Scale type: {tailTuft.scaleType}";
                        yield return $"  Number of scales: {tailTuft.numScales}";
                        break;
                    case WhiskersVars whiskers:
                        yield return "Has Whiskers:";
                        yield return $"  Number of whiskers: {whiskers.numWhiskers}";
                        break;
                    case WingScalesVars wingScales:
                        yield return "Has WingScales:";
                        yield return $"  Scale length: {wingScales.scaleLength}";
                        yield return $"  Number of scales: {wingScales.numScales}";
                        break;
                    default: break;
                }
            }
        }
    }
}
