using System.Collections;
using System.Collections.Generic;
using FinderMod.Inputs;
using FinderMod.Inputs.LizardCosmetics;
using static FinderMod.Search.Util.LizardUtil;

namespace FinderMod.Search.Options.LizardCosmetics
{
    public abstract class BaseLizardCosmetics : Option
    {
        protected LizardType type;
        protected CosmeticsItemContainer cosmetics;

        private readonly EnumInput<RotType> rotTypeInput = null!;
        protected LizardRotCosmetic lizardRotCosmetic = null!;

        public BaseLizardCosmetics(LizardType type)
        {
            this.type = type;
            elements = [cosmetics = new CosmeticsItemContainer()];
            if (ModManager.Watcher)
            {
                var submodule = new LizardRotSubholder(lizardRotCosmetic = new LizardRotCosmetic());
                rotTypeInput = submodule.RotTypeInput;
                cosmetics.Add(submodule);
            }
        }

        protected IEnumerable GetResults(XORShift128 Random)
        {
            return GetResults(Random, ModManager.Watcher ? rotTypeInput.value : RotType.None);
        }

        protected IEnumerable GetResults(XORShift128 Random, RotType rotType)
        {
            // Lizard rot!
            var (x, y, z, w) = (Random.x, Random.y, Random.z, Random.w);

            switch (type)
            {
                case LizardType.Pink
                or LizardType.Green
                or LizardType.Blue
                or LizardType.Yellow
                or LizardType.Salamander
                or LizardType.Red
                or LizardType.Caramel
                or LizardType.Blizzard
                or LizardType.Basilisk
                or LizardType.Indigo:
                    Random.Shift(4);
                    break;
                case LizardType.Cyan:
                    Random.Shift(5);
                    break;
                default: break;
            }

            var rotModule = new LizardRotModule(Random, rotType);

            Random.InitState(x, y, z, w);

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
                );

            bool blackSalamander = Random.Value < 0.33333334f; // melanistic salamander chance (yes it gets called even if not a salamander)
            if (type == LizardType.Salamander)
            {
                yield return new SalamanderCosmetics.Melanistic(blackSalamander);
            }

            // Cosmetics! (finally)
            TailTuftVars.TailTuftGraphicCalculation? tailTuftGraphic = null;
            if (type == LizardType.Eel)
            {
                yield return new AxolotlGillsVars(Random, ref tailTuftGraphic);

                yield return new TailGeckoScalesVars(Random, tailColor, null);
                if (Random.Value < 0.75f)
                {
                    yield return new LongShoulderScalesVars(Random, tailLength, type, ref tailTuftGraphic);
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
                        yield return new TailTuftVars(Random, tailLength, type, ref tailTuftGraphic);
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
                    yield return new SpineSpikesVars(Random, tailLength, type, ref tailTuftGraphic);
                }
                yield return new TailTuftVars(Random, tailLength, type, ref tailTuftGraphic);
            }

            if (type == LizardType.Cyan)
            {
                WingScalesVars? wingScalesVars = null;
                if (Random.Value < 0.75f)
                {
                    yield return wingScalesVars = new WingScalesVars(Random);
                }
                if (Random.Value < 0.5f && tailColor == 0f)
                {
                    yield return new TailTuftVars(Random, tailLength, type, ref tailTuftGraphic);
                }
                else
                {
                    yield return new TailGeckoScalesVars(Random, tailColor, wingScalesVars);
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
                    yield return new LongShoulderScalesVars(Random, tailLength, type, ref tailTuftGraphic);
                }
                else if (Random.Value < 0.2f)
                {
                    yield return new LongHeadScalesVars(Random, type, ref tailTuftGraphic);
                }
                if (Random.Value < 0.5f)
                {
                    yield return new TailTuftVars(Random, tailLength, type, ref tailTuftGraphic);
                }
            }
            else
            {
                int backDecals = 0;
                bool longShoulderScales = false;
                bool shortBodyScales = false;

                if (type == LizardType.Indigo)
                {
                    yield return new SkinkSpecklesVars(Random);
                }
                else if (type == LizardType.Caramel && Random.Value < 0.6f)
                {
                    yield return new BodyStripesVars(Random, tailLength, type);
                    backDecals++;
                }
                else if (Random.Value < 0.06666667f || Random.Value < 0.8f && type == LizardType.Green || Random.Value < 0.7f && type == LizardType.Black)
                {
                    yield return new SpineSpikesVars(Random, tailLength, type, ref tailTuftGraphic);
                    backDecals++;
                }
                else if (Random.Value < 0.033333335f && type != LizardType.Caramel)
                {
                    yield return new BumpHawkVars(Random, tailLength, type);
                    backDecals++;
                }
                else if ((Random.Value < 0.04761905f || type == LizardType.Pink && Random.Value < 0.5f || type == LizardType.Red && Random.Value < 0.9f) && type != LizardType.Salamander)
                {
                    yield return new LongShoulderScalesVars(Random, tailLength, type, ref tailTuftGraphic);
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

                if (type != LizardType.Salamander && type != LizardType.Indigo)
                {
                    if (type == LizardType.Caramel && Random.Value < 0.5f)
                    {
                        yield return new TailTuftVars(Random, tailLength, type, ref tailTuftGraphic);
                    }
                    else if (Random.Value < 0.11111111f || backDecals == 0 && Random.Value < 0.7f || type == LizardType.Pink && Random.Value < 0.6f || type == LizardType.Blue && Random.Value < 0.96f)
                    {
                        yield return new TailTuftVars(Random, tailLength, type, ref tailTuftGraphic);
                    }
                    else if (backDecals < 2 && type == LizardType.Green && Random.Value < 0.7f)
                    {
                        if (Random.Value < 0.5f || longShoulderScales || shortBodyScales)
                        {
                            yield return new TailTuftVars(Random, tailLength, type, ref tailTuftGraphic);
                        }
                        else
                        {
                            yield return new LongShoulderScalesVars(Random, tailLength, type, ref tailTuftGraphic);
                            longShoulderScales = true;
                            backDecals++;
                        }
                    }
                }

                if (Random.Value < (backDecals == 0 ? 0.7f : 0.1f) && type != LizardType.Salamander && type != LizardType.Yellow && type != LizardType.Indigo && (!longShoulderScales && Random.Value < 0.9f || Random.Value < 0.033333335f))
                {
                    yield return new LongHeadScalesVars(Random, type, ref tailTuftGraphic);
                }

                if (type == LizardType.Salamander)
                {
                    yield return new AxolotlGillsVars(Random, ref tailTuftGraphic);
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
                    yield return new LongShoulderScalesVars(Random, tailLength, type, ref tailTuftGraphic);
                    yield return new SpineSpikesVars(Random, tailLength, type, ref tailTuftGraphic);
                    backDecals += 2;
                    if (Random.Value < 0.5f)
                    {
                        yield return new TailFinVars(Random, tailLength, type);
                    }
                    else
                    {
                        yield return new TailTuftVars(Random, tailLength, type, ref tailTuftGraphic);
                    }
                }
                else if (type == LizardType.Basilisk)
                {
                    yield return new BumpHawkVars(Random, tailLength, type);
                    yield return new TailGeckoScalesVars(Random, tailColor, null);
                    yield return new TailGeckoScalesVars(Random, tailColor, null);
                    for (int i = 0; i < 9; i++)
                    {
                        yield return new ShortBodyScalesVars(Random, tailLength, type);
                        yield return new TailTuftVars(Random, tailLength, type, ref tailTuftGraphic);
                    }
                    yield return new LongHeadScalesVars(Random, type, ref tailTuftGraphic);
                    // BasiliskSlowField
                    yield return new SkinkSpecklesVars(Random);
                }
                else if (type == LizardType.Blizzard)
                {
                    for (int num10 = 0; num10 < 5; num10++)
                    {
                        if ((double)Random.Value + 1E-05 < 0.5)
                        {
                            yield return new AxolotlGillsVars(Random, ref tailTuftGraphic);
                        }
                    }
                    for (int num11 = 0; num11 < 5; num11++)
                    {
                        if ((double)Random.Value + 1E-05 < 0.5)
                        {
                            yield return new AxolotlGillsVars(Random, ref tailTuftGraphic);
                        }
                    }
                    yield return new LongHeadScalesVars(Random, type, ref tailTuftGraphic);
                    for (int num12 = 0; num12 < 8; num12++)
                    {
                        if ((double)Random.Value + 1E-05 < 0.5)
                        {
                            yield return new ShortBodyScalesVars(Random, tailLength, type);
                        }
                    }
                    for (int num13 = 0; num13 < 5; num13++)
                    {
                        if ((double)Random.Value + 1E-05 < 0.800000011920929)
                        {
                            yield return new SpineSpikesVars(Random, tailLength, type, ref tailTuftGraphic);
                        }
                    }
                    for (int num14 = 0; num14 < 10; num14++)
                    {
                        yield return new TailTuftVars(Random, tailLength, type, ref tailTuftGraphic);
                    }
                    if ((double)Random.Value + 1E-05 < 0.05000000074505806)
                    {
                        yield return new WhiskersVars(Random);
                    }
                    // BlizzardBeam
                    // BlizzardSteam
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

            if (rotType != RotType.None)
            {
                yield return new LizardRotVars(Random, rotModule, rotType);
            }
        }

        protected override IEnumerable<string> GetValues(XORShift128 Random)
        {
            var (x, y, z, w) = (Random.x, Random.y, Random.z, Random.w);

            foreach (var value in GetResults(Random, RotType.None))
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
                        yield return $"  Is colored: {(bumpHawk.colored ? "Yes" : "No")}";
                        break;
                    case JumpRingsVars:
                        yield return "Has JumpRings";
                        break;
                    case LongHeadScalesVars longHeadScales:
                        yield return "Has LongHeadScales:";
                        yield return $"  Length: {longHeadScales.length}";
                        yield return $"  Width: {longHeadScales.width}";
                        yield return $"  Rigor: {longHeadScales.rigor}";
                        yield return $"  Graphic: {longHeadScales.graphic}";
                        yield return $"  Is colored: {(longHeadScales.colored ? "Yes" : "No")}";
                        break;
                    case LongShoulderScalesVars longShoulderScales:
                        yield return "Has LongShoulderScales:";
                        yield return $"  Min size: {longShoulderScales.minSize}";
                        yield return $"  Max size: {longShoulderScales.maxSize}";
                        yield return $"  Graphic: {longShoulderScales.graphic}";
                        yield return $"  Scale type: {longShoulderScales.scaleType}";
                        yield return $"  Number of scales: {longShoulderScales.numScales}";
                        yield return $"  Is colored: {(longShoulderScales.colored ? "Yes" : "No")}";
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
                        yield return $"  Is colored: {(tailFin.colored ? "Yes" : "No")}";
                        break;
                    case TailGeckoScalesVars tailGeckoScales:
                        yield return "Has TailGeckoScales:";
                        yield return $"  Rows: {tailGeckoScales.rows}";
                        yield return $"  Lines: {tailGeckoScales.lines}";
                        yield return $"  Big scales: {(tailGeckoScales.bigScales ? "Yes" : "No")}";
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

            if (ModManager.Watcher)
            {
                yield return null!;
                RotType[] rotTypes = [RotType.Slight, RotType.Opossom, RotType.Full];
                foreach (var rotType in rotTypes)
                {
                    Random.InitState(x, y, z, w);
                    foreach (var result in GetResults(Random, rotType))
                    {
                        if (result is LizardRotVars rotVars)
                        {
                            yield return $"LizardRotGraphics ({rotType}):";
                            yield return $"  Number of alive tentacles: {rotVars.numLegs}";
                            yield return $"  Number of dead tentacles: {rotVars.numDeadLegs}";
                            yield return $"  Number of eyes: {rotVars.numEyes}";
                        }
                    }
                }
            }
        }

        protected static float DistanceIf(LizardBodyScaleType type, Input<LizardBodyScaleType> input) => input.enabled && input.value != type ? input.bias : 0f;
    }
}
