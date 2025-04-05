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

        public static implicit operator CosmeticsItem(CosmeticType type)
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
        }
    }

    public class AntennaeCosmetic : CosmeticsItem
    {
        public FloatInput LengthInput;
        public FloatInput AlphaInput;
        public AntennaeCosmetic() : base(CosmeticType.Antennae)
        {
            children.Add(LengthInput = new("Length"));
            children.Add(AlphaInput = new("Alpha"));
        }
    }

    public class AxolotlGillsCosmetic : CosmeticsItem
    {
        public FloatInput RigorInput;
        public IntInput NumGillsInput;
        public IntInput GraphicInput;
        public AxolotlGillsCosmetic() : base(CosmeticType.AxolotlGills)
        {
            children.Add(RigorInput = new("Rigor"));
            children.Add(NumGillsInput = new("Number of gills", 2, 7));
            children.Add(GraphicInput = new("Graphic", 0, 5));
        }
    }

    public class BodyStripesCosmetic : CosmeticsItem
    {
        public IntInput NumScalesInput;
        public BodyStripesCosmetic() : base(CosmeticType.BodyStripes)
        {
            children.Add(NumScalesInput = new("Number of scales", 3, 44)); // the max took like 30 minutes to calculate
        }
    }

    public class BumpHawkCosmetic : CosmeticsItem
    {
        public FloatInput SpineLenInput;
        public IntInput NumBumpsInput;
        public BoolInput ColoredInput;

        public BumpHawkCosmetic() : base(CosmeticType.BumpHawk)
        {
            children.Add(SpineLenInput = new("Spine length", 21.24f, 410.238f));
            children.Add(NumBumpsInput = new("Number of bumps", 1, 106));
            children.Add(ColoredInput = new("Is colored"));
        }
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
            children.Add(LengthInput = new("Length", 5f, 35f));
            children.Add(WidthInput = new("Width", 0.65f, 1.2f));
            children.Add(RigorInput = new("Rigor"));
            children.Add(ColoredInput = new("Is colored"));
        }
    }

    public class LongShoulderScalesCosmetic : CosmeticsItem
    {
        public FloatInput MinSizeInput;
        public FloatInput MaxSizeInput;
        public IntManualInput NumScalesInput;
        public IntInput GraphicInput;
        public EnumInput<LizardBodyScaleType> ScaleTypeInput;
        public BoolInput ColoredInput;

        public LongShoulderScalesCosmetic() : base(CosmeticType.LongShoulderScales)
        {
            children.Add(MinSizeInput = new("Min size", 5f, 15f));
            children.Add(MaxSizeInput = new("Max size", 5f, 35f));
            children.Add(NumScalesInput = new("Number of scales", 10) { minValue = 3 });
            children.Add(GraphicInput = new("Graphic", 0, 6));
            children.Add(ScaleTypeInput = new("Scale type", LizardBodyScaleType.Patch));
            children.Add(ColoredInput = new("Is colored"));
        }
    }

    public class ShortBodyScalesCosmetic : CosmeticsItem
    {
        public IntManualInput NumScalesInput;
        public EnumInput<LizardBodyScaleType> ScaleTypeInput;

        public ShortBodyScalesCosmetic() : base(CosmeticType.ShortBodyScales)
        {
            children.Add(NumScalesInput = new("Number of scales", 10) { minValue = 3 });
            children.Add(ScaleTypeInput = new("Scale type", LizardBodyScaleType.Patch));
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

        public SpineSpikesCosmetic() : base(CosmeticType.SpineSpikes)
        {
            children.Add(LengthInput = new("Length", 14.16f, 433.029f));
            children.Add(NumScalesInput = new("Number of scales", 1, 86));
            children.Add(GraphicInput = new("Graphic", 0, 6));
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

        public TailFinCosmetic() : base(CosmeticType.TailFin)
        {
            children.Add(LengthInput = new("Scale length", 20.26f, 309.4f));
            children.Add(UndersideSizeInput = new("Underside size", 0.3f, 0.9f));
            children.Add(ScaleXInput = new("Scale size x", 1f, 2f));
            children.Add(NumScalesInput = new("Number of scales", 2, 76));
            children.Add(GraphicInput = new("Graphic", 0, 5));
            children.Add(ColoredInput = new("Colored input"));
        }
    }

    public class TailGeckoScalesCosmetic : CosmeticsItem
    {
        public IntInput RowsInput;
        public IntInput LinesInput;

        public TailGeckoScalesCosmetic() : base(CosmeticType.TailGeckoScales)
        {
            children.Add(RowsInput = new("Rows", 7, 19));
            children.Add(LinesInput = new("Lines", 3, 4));
        }
    }

    public class TailTuftCosmetic : CosmeticsItem
    {
        public TailTuftCosmetic() : base(CosmeticType.TailTuft)
        {
            children.Add(new Label("This cosmetic has no variations."));
        }
    }

    public class WhiskersCosmetic : CosmeticsItem
    {
        public IntInput NumWhiskersInput;
        public WhiskersCosmetic() : base(CosmeticType.Whiskers)
        {
            children.Add(NumWhiskersInput = new("Number of whiskers", 3, 4));
        }
    }

    public class WingScalesCosmetic : CosmeticsItem
    {
        public FloatInput LengthInput;
        public IntInput NumScalesInput;

        public WingScalesCosmetic() : base(CosmeticType.WingScales)
        {
            children.Add(LengthInput = new("Length", 5f, 40f));
            children.Add(NumScalesInput = new("Scales per side", 2, 3));
        }
    }
}
