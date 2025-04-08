using System.Collections.Generic;
using Menu.Remix.MixedUI;
using static FinderMod.Search.Util.LizardUtil;

namespace FinderMod.Inputs.LizardCosmetics
{
    public abstract class CosmeticsItem(CosmeticType cosmeticType) : Group([new Label(cosmeticType.ToString())], cosmeticType.ToString())
    {
        internal protected CosmeticsItemContainer.Subholder parent = null!;
        public bool Enabled => parent != null && parent.Enabled;
        public bool Toggled => parent.IsToggled;
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

        /*public static implicit operator CosmeticsItem(CosmeticType type)
        {
            return type switch
            {
                CosmeticType.Antennae => new AntennaeCosmetic(),
                CosmeticType.AxolotlGills => new AxolotlGillsCosmetic(),
                CosmeticType.BodyStripes => new BodyStripesCosmetic(),
                CosmeticType.BumpHawk => new BumpHawkCosmetic(),
                CosmeticType.JumpRings => new JumpRingsCosmetic(),
                CosmeticType.LongHeadScales => new LongHeadScalesCosmetic(),
                CosmeticType.LongShoulderScales => new LongShoulderScalesCosmetic(),
                CosmeticType.ShortBodyScales => new ShortBodyScalesCosmetic(),
                CosmeticType.SnowAccumulation => new SnowAccumulationCosmetic(),
                CosmeticType.SpineSpikes => new SpineSpikesCosmetic(),
                CosmeticType.TailFin => new TailFinCosmetic(),
                CosmeticType.TailGeckoScales => new TailGeckoScalesCosmetic(),
                CosmeticType.TailTuft => new TailTuftCosmetic(),
                CosmeticType.Whiskers => new WhiskersCosmetic(),
                CosmeticType.WingScales => new WingScalesCosmetic(),
                _ => null!,
            };
        }*/
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
    }

    public class BodyStripesCosmetic : CosmeticsItem
    {
        public IntInput NumScalesInput;
        // internal BodyStripesCosmetic() : this(44) { }
        private BodyStripesCosmetic(int max) : base(CosmeticType.BodyStripes)
        {
            children.Add(NumScalesInput = new("Number of scales", 3, max) { enabled = false });
        }
        public BodyStripesCosmetic(LizardType type) : this(BodyStripesVars.MaxNumScales(type)) { }
    }

    public class BumpHawkCosmetic : CosmeticsItem
    {
        public FloatInput SpineLenInput;
        public IntInput NumBumpsInput;
        public BoolInput ColoredInput;

        // internal BumpHawkCosmetic() : this(21.24f, 410.238f, 1, 106) { }
        private BumpHawkCosmetic(float minLen, float maxLen, int minBump, int maxBump) : base(CosmeticType.BumpHawk)
        {
            children.Add(SpineLenInput = new("Spine length", minLen, maxLen) { enabled = false });
            children.Add(NumBumpsInput = new("Number of bumps", minBump, maxBump) { enabled = false });
            children.Add(ColoredInput = new("Is colored") { enabled = false, hasBias = true });
        }
        public BumpHawkCosmetic(LizardType type)
            : this(BumpHawkVars.MinSpineLength(type), BumpHawkVars.MaxSpineLength(type), BumpHawkVars.MinNumBumps(type), BumpHawkVars.MaxNumBumps(type)) { }
    }

    public class JumpRingsCosmetic : CosmeticsItem
    {
        public JumpRingsCosmetic() : base(CosmeticType.JumpRings)
        {
            children.Add(new Label("This cosmetic has no variations."));
        }
    }

    public class LongHeadScalesCosmetic : CosmeticsItem
    {
        public FloatInput LengthInput;
        public FloatInput WidthInput;
        public FloatInput RigorInput;
        public BoolInput ColoredInput;

        public LongHeadScalesCosmetic() : base(CosmeticType.LongHeadScales)
        {
            children.Add(LengthInput = new("Length", 5f, 35f) { enabled = false });
            children.Add(WidthInput = new("Width", 0.65f, 1.2f) { enabled = false });
            children.Add(RigorInput = new("Rigor") { enabled = false });
            children.Add(ColoredInput = new("Is colored") { enabled = false, hasBias = true });
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

        // private SpineSpikesCosmetic() : this(14.16f, 433.029f, 1, 86) { }
        private SpineSpikesCosmetic(float minLen, float maxLen, int minScales, int maxScales) : base(CosmeticType.SpineSpikes)
        {
            children.Add(LengthInput = new("Length", minLen, maxLen) { enabled = false });
            children.Add(NumScalesInput = new("Number of scales", minScales, maxScales) { enabled = false });
            children.Add(GraphicInput = new("Graphic", 0, 6) { enabled = false });
        }

        public SpineSpikesCosmetic(LizardType type)
            : this(SpineSpikesVars.MinSpineLength(type), SpineSpikesVars.MaxSpineLength(type), SpineSpikesVars.MinNumScales(type), SpineSpikesVars.MaxNumScales(type)) { }
    }

    public class TailFinCosmetic : CosmeticsItem
    {
        public FloatInput LengthInput;
        public FloatInput UndersideSizeInput;
        public FloatInput ScaleXInput;
        public IntInput NumScalesInput;
        public IntInput GraphicInput;
        public BoolInput ColoredInput;

        // private TailFinCosmetic() : this(20.26f, 309.4f, 2, 76) { }
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
    }

    public class TailGeckoScalesCosmetic : CosmeticsItem
    {
        public IntInput RowsInput;
        public IntInput LinesInput;

        public TailGeckoScalesCosmetic() : base(CosmeticType.TailGeckoScales)
        {
            children.Add(RowsInput = new("Rows", 7, 18) { enabled = false });
            children.Add(LinesInput = new("Lines", 3, 4) { enabled = false });
        }
    }

    public class TailTuftCosmetic : CosmeticsItem
    {
        public IntInput NumScalesInput;
        public EnumInput<LizardBodyScaleType> ScaleTypeInput;

        private TailTuftCosmetic(int minScales, int maxScales) : base(CosmeticType.TailTuft)
        {
            children.Add(NumScalesInput = new("Number of scales", minScales, maxScales) { enabled = false });
            children.Add(ScaleTypeInput = new("Scale type", LizardBodyScaleType.TwoLines) { enabled = false, excludeOptions = [LizardBodyScaleType.Patch] });
        }

        public TailTuftCosmetic(LizardType type) : this(TailTuftVars.MinNumScales(type), TailTuftVars.MaxNumScales(type)) { }
    }

    public class WhiskersCosmetic : CosmeticsItem
    {
        public IntInput NumWhiskersInput;
        public WhiskersCosmetic() : base(CosmeticType.Whiskers)
        {
            children.Add(NumWhiskersInput = new("Number of whiskers", 3, 4) { enabled = false });
        }
    }

    public class WingScalesCosmetic : CosmeticsItem
    {
        public FloatInput LengthInput;
        public IntInput NumScalesInput;

        public WingScalesCosmetic() : base(CosmeticType.WingScales)
        {
            children.Add(LengthInput = new("Length", 31.25f, 40f) { enabled = false });
            children.Add(NumScalesInput = new("Scales per side", 2, 3) { enabled = false });
        }
    }
}
