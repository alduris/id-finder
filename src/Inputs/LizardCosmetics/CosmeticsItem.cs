using System.Collections.Generic;
using FinderMod.Search.Options;
using static FinderMod.Search.Util.LizardUtil;

namespace FinderMod.Inputs.LizardCosmetics
{
    public abstract class CosmeticsItem(CosmeticType cosmeticType) : Group([new Label(cosmeticType.ToString())], cosmeticType.ToString())
    {
        internal protected Subholder parent = null!;
        public bool Enabled => parent != null && parent.Enabled;
        public bool Toggled => parent == null || parent.IsToggled;
        public bool Active => Enabled && Toggled;
        public readonly CosmeticType cosmeticType = cosmeticType;

        public override IEnumerable<string> GetHistoryLines()
        {
            if (Active)
            {
                yield return cosmeticType.ToString();
                foreach (var line in base.GetHistoryLines())
                {
                    yield return line;
                }
            }
            yield break;
        }
    }

    public class AntennaeCosmetic : CosmeticsItem
    {
        public FloatInput LengthInput;
        public FloatInput AlphaInput;

        public AntennaeCosmetic() : base(CosmeticType.Antennae)
        {
            children.Add(LengthInput = new("Length") { enabled = false });
            children.Add(AlphaInput = new("Alpha") { enabled = false });
        }

        public float Distance(AntennaeVars vars)
        {
            if (Active)
            {
                return Option.DistanceIf(vars.length, LengthInput)
                    + Option.DistanceIf(vars.alpha, AlphaInput);
            }
            else if (Enabled && !Toggled)
            {
                return MISSING_PENALTY;
            }
            return 0f;
        }
    }

    public class AxolotlGillsCosmetic : CosmeticsItem
    {
        public FloatInput RigorInput;
        public IntInput NumGillsInput;
        public IntInput GraphicInput;

        public AxolotlGillsCosmetic() : base(CosmeticType.AxolotlGills)
        {
            children.Add(RigorInput = new("Rigor") { enabled = false });
            children.Add(NumGillsInput = new("Number of gills", 2, 7) { enabled = false });
            children.Add(GraphicInput = new("Graphic", 0, 5) { enabled = false });
        }

        public float Distance(AxolotlGillsVars vars)
        {
            if (Active)
            {
                return Option.DistanceIf(vars.rigor, RigorInput)
                    + Option.DistanceIf(vars.numGills, NumGillsInput)
                    + (GraphicInput.enabled && vars.graphic != GraphicInput.value ? GraphicInput.bias : 0f);
            }
            else if (Enabled && !Toggled)
            {
                return MISSING_PENALTY;
            }
            return 0f;
        }
    }

    public class BodyStripesCosmetic : CosmeticsItem
    {
        public IntInput NumScalesInput;

        private BodyStripesCosmetic(int max) : base(CosmeticType.BodyStripes)
        {
            children.Add(NumScalesInput = new("Number of scales", 3, max) { enabled = false });
        }
        public BodyStripesCosmetic(LizardType type) : this(BodyStripesVars.MaxNumScales(type)) { }

        public float Distance(BodyStripesVars vars)
        {
            if (Active)
            {
                return Option.DistanceIf(vars.numScales, NumScalesInput);
            }
            else if (Enabled && !Toggled)
            {
                return MISSING_PENALTY;
            }
            return 0f;
        }
    }

    public class BumpHawkCosmetic : CosmeticsItem
    {
        public FloatInput SpineLenInput;
        public IntInput NumBumpsInput;
        public BoolInput ColoredInput;

        private BumpHawkCosmetic(float minLen, float maxLen, int minBump, int maxBump) : base(CosmeticType.BumpHawk)
        {
            children.Add(SpineLenInput = new("Spine length", minLen, maxLen) { enabled = false });
            children.Add(NumBumpsInput = new("Number of bumps", minBump, maxBump) { enabled = false });
            children.Add(ColoredInput = new("Is colored") { enabled = false, hasBias = true });
        }
        public BumpHawkCosmetic(LizardType type)
            : this(BumpHawkVars.MinSpineLength(type), BumpHawkVars.MaxSpineLength(type), BumpHawkVars.MinNumBumps(type), BumpHawkVars.MaxNumBumps(type)) { }

        public float Distance(BumpHawkVars vars)
        {
            if (Active)
            {
                return Option.DistanceIf(vars.spineLength, SpineLenInput)
                    + Option.DistanceIf(vars.numBumps, NumBumpsInput)
                    + Option.DistanceIf(vars.colored, ColoredInput);
            }
            else if (Enabled && !Toggled)
            {
                return MISSING_PENALTY;
            }
            return 0f;
        }
    }

    public class JumpRingsCosmetic : CosmeticsItem
    {
        public JumpRingsCosmetic() : base(CosmeticType.JumpRings)
        {
            children.Add(new Label("This cosmetic has no variations."));
        }
    }

    public class LizardRotCosmetic : CosmeticsItem
    {
        public IntInput NumTentaclesInput;
        public IntInput NumDeadTentaclesInput;
        public IntInput NumEyesInput;

        public LizardRotCosmetic() : base(CosmeticType.LizardRot)
        {
            children.Add(NumTentaclesInput = new("Number of alive tentacles", 5, 9) { enabled = false });
            children.Add(NumDeadTentaclesInput = new("Number of dead tentacles", 0, 2) { enabled = false });
            children.Add(NumEyesInput = new("Number of eyes", 2, 5) { enabled = false });
        }

        public float Distance(LizardRotVars vars)
        {
            if (Active)
            {
                return Option.DistanceIf(vars.numLegs, NumTentaclesInput)
                    + Option.DistanceIf(vars.numDeadLegs, NumDeadTentaclesInput)
                    + Option.DistanceIf(vars.numEyes, NumEyesInput);
            }
            else if (Enabled && !Toggled)
            {
                return MISSING_PENALTY;
            }
            return 0f;
        }
    }

    public class LongHeadScalesCosmetic : CosmeticsItem
    {
        public FloatInput LengthInput;
        public FloatInput WidthInput;
        public FloatInput RigorInput;
        public IntInput GraphicInput;
        public BoolInput ColoredInput;

        public LongHeadScalesCosmetic() : base(CosmeticType.LongHeadScales)
        {
            children.Add(LengthInput = new("Length", 5f, 35f) { enabled = false });
            children.Add(WidthInput = new("Width", 0.65f, 1.2f) { enabled = false });
            children.Add(RigorInput = new("Rigor") { enabled = false });
            children.Add(GraphicInput = new("Graphic", 4, 6) { enabled = false });
            children.Add(ColoredInput = new("Is colored") { enabled = false, hasBias = true });
        }

        public float Distance(LongHeadScalesVars vars)
        {
            if (Active)
            {
                return Option.DistanceIf(vars.length, LengthInput)
                    + Option.DistanceIf(vars.width, WidthInput)
                    + Option.DistanceIf(vars.rigor, RigorInput)
                    + Option.DistanceIf(vars.graphic, GraphicInput)
                    + Option.DistanceIf(vars.colored, ColoredInput);
            }
            else if (Enabled && !Toggled)
            {
                return MISSING_PENALTY;
            }
            return 0f;
        }
    }

    public class LongShoulderScalesCosmetic : CosmeticsItem
    {
        public FloatInput MinSizeInput;
        public FloatInput MaxSizeInput;
        public IntInput NumScalesInput;
        public IntInput GraphicInput;
        public EnumInput<LizardBodyScaleType> ScaleTypeInput;
        public BoolInput ColoredInput;

        private LongShoulderScalesCosmetic(int minScales, int maxScales) : base(CosmeticType.LongShoulderScales)
        {
            children.Add(MinSizeInput = new("Min size", 2.5f, 15f) { enabled = false });
            children.Add(MaxSizeInput = new("Max size", 2.5f, 35f) { enabled = false });
            children.Add(NumScalesInput = new("Number of scales", minScales, maxScales) { enabled = false });
            children.Add(GraphicInput = new("Graphic", 0, 6) { enabled = false });
            children.Add(ScaleTypeInput = new("Scale type", LizardBodyScaleType.Patch) { enabled = false });
            children.Add(ColoredInput = new("Is colored") { enabled = false, hasBias = true });
        }
        public LongShoulderScalesCosmetic(LizardType type) : this(LongShoulderScalesVars.MinNumScales(type), LongShoulderScalesVars.MaxNumScales(type)) { }

        public float Distance(LongShoulderScalesVars vars)
        {
            if (Active)
            {
                return Option.DistanceIf(vars.minSize, MinSizeInput)
                    + Option.DistanceIf(vars.maxSize, MaxSizeInput)
                    + Option.DistanceIf(vars.numScales, NumScalesInput)
                    + Option.DistanceIf(vars.colored, ColoredInput)
                    + (GraphicInput.enabled && GraphicInput.value != vars.graphic ? GraphicInput.bias : 0f)
                    + (ScaleTypeInput.enabled && ScaleTypeInput.value != vars.scaleType ? ScaleTypeInput.bias : 0f);
            }
            else if (Enabled && !Toggled)
            {
                return MISSING_PENALTY;
            }
            return 0f;
        }
    }

    public class ShortBodyScalesCosmetic : CosmeticsItem
    {
        public IntInput NumScalesInput;
        public EnumInput<LizardBodyScaleType> ScaleTypeInput;

        private ShortBodyScalesCosmetic(int minScales, int maxScales) : base(CosmeticType.ShortBodyScales)
        {
            children.Add(NumScalesInput = new("Number of scales", minScales, maxScales) { enabled = false });
            children.Add(ScaleTypeInput = new("Scale type", LizardBodyScaleType.Patch) { enabled = false });
        }
        public ShortBodyScalesCosmetic(LizardType type) : this(ShortBodyScalesVars.MinNumScales(type), ShortBodyScalesVars.MaxNumScales(type)) { }

        public float Distance(ShortBodyScalesVars vars)
        {
            if (Active)
            {
                return Option.DistanceIf(vars.numScales, NumScalesInput)
                    + (ScaleTypeInput.enabled && ScaleTypeInput.value != vars.scaleType ? ScaleTypeInput.bias : 0f);
            }
            else if (Enabled && !Toggled)
            {
                return MISSING_PENALTY;
            }
            return 0f;
        }
    }

    public class SkinkSpecklesCosmetic : CosmeticsItem
    {
        public IntInput NumSpotsInput;

        public SkinkSpecklesCosmetic() : base(CosmeticType.SkinkSpeckles)
        {
            children.Add(NumSpotsInput = new("Number of speckles", 0, 49) { enabled = false });
        }

        public float Distance(SkinkSpecklesVars vars)
        {
            if (Active)
            {
                return Option.DistanceIf(vars.spots, NumSpotsInput);
            }
            else if (Enabled && !Toggled)
            {
                return MISSING_PENALTY;
            }
            return 0f;
        }
    }

    public class SnowAccumulationCosmetic : CosmeticsItem
    {
        public SnowAccumulationCosmetic() : base(CosmeticType.SnowAccumulation)
        {
            children.Add(new Label("This cosmetic has no variations."));
        }
    }

    public class SpineSpikesCosmetic : CosmeticsItem
    {
        public FloatInput LengthInput;
        public IntInput NumScalesInput;
        public IntInput GraphicInput;

        private SpineSpikesCosmetic(float minLen, float maxLen, int minScales, int maxScales) : base(CosmeticType.SpineSpikes)
        {
            children.Add(LengthInput = new("Length", minLen, maxLen) { enabled = false });
            children.Add(NumScalesInput = new("Number of scales", minScales, maxScales) { enabled = false });
            children.Add(GraphicInput = new("Graphic", 0, 6) { enabled = false });
        }

        public SpineSpikesCosmetic(LizardType type)
            : this(SpineSpikesVars.MinSpineLength(type), SpineSpikesVars.MaxSpineLength(type), SpineSpikesVars.MinNumScales(type), SpineSpikesVars.MaxNumScales(type)) { }

        public float Distance(SpineSpikesVars vars)
        {
            if (Active)
            {
                return Option.DistanceIf(vars.spineLength, LengthInput)
                    + Option.DistanceIf(vars.numScales, NumScalesInput)
                    + (GraphicInput.enabled && GraphicInput.value != vars.graphic ? GraphicInput.bias : 0f);
            }
            else if (Enabled && !Toggled)
            {
                return MISSING_PENALTY;
            }
            return 0f;
        }
    }

    public class TailFinCosmetic : CosmeticsItem
    {
        public FloatInput LengthInput;
        public FloatInput UndersideSizeInput;
        public FloatInput ScaleXInput;
        public IntInput NumScalesInput;
        public IntInput GraphicInput;
        public BoolInput ColoredInput;

        private TailFinCosmetic(float minLen, float maxLen, int minScales, int maxScales) : base(CosmeticType.TailFin)
        {
            children.Add(LengthInput = new("Scale length", minLen, maxLen) { enabled = false });
            children.Add(UndersideSizeInput = new("Underside size", 0.3f, 0.9f) { enabled = false });
            children.Add(ScaleXInput = new("Scale size x", 1f, 2f) { enabled = false });
            children.Add(NumScalesInput = new("Number of scales", minScales, maxScales) { enabled = false });
            children.Add(GraphicInput = new("Graphic", 0, 5) { enabled = false });
            children.Add(ColoredInput = new("Is colored") { enabled = false, hasBias = true });
        }

        public TailFinCosmetic(LizardType type)
            : this(TailFinVars.MinSpineLength(type), TailFinVars.MaxSpineLength(type), TailFinVars.MinNumScales(type), TailFinVars.MaxNumScales(type)) { }

        public float Distance(TailFinVars vars)
        {
            if (Active)
            {
                return Option.DistanceIf(vars.spineLength, LengthInput)
                    + Option.DistanceIf(vars.undersideSize, UndersideSizeInput)
                    + Option.DistanceIf(vars.spineScaleX, ScaleXInput)
                    + Option.DistanceIf(vars.numScales, NumScalesInput)
                    + Option.DistanceIf(vars.colored, ColoredInput)
                    + (GraphicInput.enabled && GraphicInput.value != vars.graphic ? GraphicInput.bias : 0f);
            }
            else if (Enabled && !Toggled)
            {
                return MISSING_PENALTY;
            }
            return 0f;
        }
    }

    public class TailGeckoScalesCosmetic : CosmeticsItem
    {
        public IntInput RowsInput;
        public IntInput LinesInput;
        public BoolInput BigScalesInput;

        public TailGeckoScalesCosmetic() : base(CosmeticType.TailGeckoScales)
        {
            children.Add(RowsInput = new("Rows", 7, 18) { enabled = false });
            children.Add(LinesInput = new("Lines", 3, 4) { enabled = false });
            children.Add(BigScalesInput = new("Big scales", true) { enabled = false });
        }

        public float Distance(TailGeckoScalesVars vars)
        {
            if (Active)
            {
                return Option.DistanceIf(vars.rows, RowsInput)
                    + Option.DistanceIf(vars.lines, LinesInput)
                    + Option.DistanceIf(vars.bigScales, BigScalesInput);
            }
            else if (Enabled && !Toggled)
            {
                return MISSING_PENALTY;
            }
            return 0f;
        }
    }

    public class TailTuftCosmetic : CosmeticsItem
    {
        public IntInput NumScalesInput;
        public IntInput GraphicInput;
        public EnumInput<LizardBodyScaleType> ScaleTypeInput;
        public BoolInput ColoredInput;

        private TailTuftCosmetic(int minScales, int maxScales) : base(CosmeticType.TailTuft)
        {
            children.Add(NumScalesInput = new("Number of scales", minScales, maxScales) { enabled = false });
            children.Add(GraphicInput = new("Graphic", 0, 6) { enabled = false });
            children.Add(ScaleTypeInput = new("Scale type", LizardBodyScaleType.TwoLines) { enabled = false, excludeOptions = [LizardBodyScaleType.Patch] });
            children.Add(ColoredInput = new("Is colored") { enabled = false });
        }

        public TailTuftCosmetic(LizardType type) : this(TailTuftVars.MinNumScales(type), TailTuftVars.MaxNumScales(type)) { }

        public float Distance(TailTuftVars vars)
        {
            if (Active)
            {
                return Option.DistanceIf(vars.numScales, NumScalesInput)
                    + Option.DistanceIf(vars.colored, ColoredInput)
                    + (GraphicInput.enabled && GraphicInput.value != vars.graphic ? GraphicInput.bias : 0f)
                    + (ScaleTypeInput.enabled && ScaleTypeInput.value != vars.scaleType ? ScaleTypeInput.bias : 0f);
            }
            else if (Enabled && !Toggled)
            {
                return MISSING_PENALTY;
            }
            return 0f;
        }
    }

    public class WhiskersCosmetic : CosmeticsItem
    {
        public IntInput NumWhiskersInput;

        public WhiskersCosmetic() : base(CosmeticType.Whiskers)
        {
            children.Add(NumWhiskersInput = new("Number of whiskers", 3, 4) { enabled = false });
        }

        public float Distance(WhiskersVars vars)
        {
            if (Active)
            {
                return Option.DistanceIf(vars.numWhiskers, NumWhiskersInput);
            }
            else if (Enabled && !Toggled)
            {
                return MISSING_PENALTY;
            }
            return 0f;
        }
    }

    public class WingScalesCosmetic : CosmeticsItem
    {
        public FloatInput LengthInput;
        public IntInput NumScalesInput;

        public WingScalesCosmetic() : base(CosmeticType.WingScales)
        {
            children.Add(LengthInput = new("Length", 5f, 40f) { enabled = false });
            children.Add(NumScalesInput = new("Scales per side", 2, 3) { enabled = false });
        }

        public float Distance(WingScalesVars vars)
        {
            if (Active)
            {
                return Option.DistanceIf(vars.scaleLength, LengthInput)
                    + Option.DistanceIf(vars.numScales, NumScalesInput);
            }
            else if (Enabled && !Toggled)
            {
                return MISSING_PENALTY;
            }
            return 0f;
        }
    }
}
