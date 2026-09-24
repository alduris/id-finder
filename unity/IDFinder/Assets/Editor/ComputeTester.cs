using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;
using System.Linq;
using UnityEngine.Rendering;
using static LizardCosmeticsTesting;


public class ComputeTester : EditorWindow
{
    [SerializeField]
    private Dictionary<string, List<Input>> TestInputs = new Dictionary<string, List<Input>>()
    {
        ["Personality"] = new List<Input>()
        {
            new Input("Aggression"),
            new Input("Bravery"),
            new Input("Dominance"),
            new Input("Energy"),
            new Input("Nervous"),
            new Input("Sympathy")
        },
        ["SlugpupBehavior"] = new List<Input>()
        {
            new Input("Blue fruit", -1, 1),
            new Input("Water nut", -1, 1),
            new Input("Jellyfish", -1, 1),
            new Input("Slime mold", -1, 1),
            new Input("Eggbug egg", -1, 1),
            new Input("Fire egg", -1, 1),
            new Input("Popcorn", -1, 1),
            new Input("Gooieduck", -1, 1),
            new Input("Lilypuck", -1, 1),
            new Input("Glow weed", -1, 1),
            new Input("Dandelion peach", -1, 1),
            new Input("Neuron", -1, 1),
            new Input("Centipede", -1, 1),
            new Input("Small centipede", -1, 1),
            new Input("Vulture grub", -1, 1),
            new Input("Small noodlefly", -1, 1),
            new Input("Hazer", -1, 1),
            new Input("Wiggles when held", 0, 1, 1),
            new Input("Takes naps", 0, 1, 1),
            new Input("Plays with items", 0, 1, 1),
            new Input("Lays near parent", 0, 1, 1),
        },
        ["SlugpupFood"] = new List<Input>()
        {
            new Input("Blue fruit", -1, 1),
            new Input("Water nut", -1, 1),
            new Input("Jellyfish", -1, 1),
            new Input("Slime mold", -1, 1),
            new Input("Eggbug egg", -1, 1),
            new Input("Fire egg", -1, 1),
            new Input("Popcorn", -1, 1),
            new Input("Gooieduck", -1, 1),
            new Input("Lilypuck", -1, 1),
            new Input("Glow weed", -1, 1),
            new Input("Dandelion peach", -1, 1),
            new Input("Neuron", -1, 1),
            new Input("Centipede", -1, 1),
            new Input("Small centipede", -1, 1),
            new Input("Vulture grub", -1, 1),
            new Input("Small noodlefly", -1, 1),
            new Input("Hazer", -1, 1),
        },
        ["SlugpupStats"] = new List<Input>()
        {
            new Input("Body weight", 0.5525f, 0.715f),
            new Input("Visibility (standing)", -0.24f, -0.16f),
            new Input("Visibility (crouching)", 0.45f, 0.75f),
            new Input("Loudness", 0.4f, 0.6f),
            new Input("Lung capacity", 0.64f, 0.96f),
            new Input("Pole climbing speed", 0.68f, 1f),
            new Input("Tunnel crawling speed", 0.68f, 1f),
            new Input("Running speed", 0.68f, 1f),
        },
        ["SlugpupVars"] = new List<Input>()
        {
            new Input("Size"),
            new Input("Wideness"),
            new Input("Hue"),
            new Input("Saturation"),
            new Input("L (different!!!)", 0.01f, 1), // original code has this flip
            new Input("Dark?", 0, 1, 1),
            new Input("Eye (L)")
        },
        ["ScavengerSkills"] = new List<Input>()
        {
            new Input("Dodge"),
            new Input("Mid-range"),
            new Input("Melee"),
            new Input("Blocking"),
            new Input("Reaction"),
            new Input("MSC enabled?", 0, 1, 1),
        },
        ["ScavengerVars"] = new List<Input>()
        {
            new Input("Head size"),
            new Input("Eartler thickness"),
            new Input("Eye size"),
            new Input("Eye narrowness"),
            new Input("Eye angle"),
            new Input("Fatness"),
            new Input("Waist narrowness"),
            new Input("Neck thickness"),
            new Input("Pupil size"),
            new Input("Deep pupils?", 0, 1, 1),
            new Input("Hands color blend"),
            new Input("Leg size"),
            new Input("Arm thickness"),
            new Input("Colored eartler tips?", 0, 1, 1),
            new Input("Teeth wideness"),
            new Input("Tail segments", 0, 4, 1),
        },
        ["ScavengerColors"] = new List<Input>()
        {
            new Input("Body color H"),
            new Input("Body color S"),
            new Input("Body color L"),
            new Input("Head color H"),
            new Input("Head color S"),
            new Input("Head color L"),
            new Input("Deco color H"),
            new Input("Deco color S"),
            new Input("Deco color L"),
            new Input("Eye color H"),
            new Input("Eye color L"),
        },
        ["EliteScavengerSkills"] = new List<Input>()
        {
            new Input("Dodge"),
            new Input("Mid-range"),
            new Input("Melee"),
            new Input("Blocking"),
            new Input("Reaction")
        },
        ["EliteScavengerColors"] = new List<Input>()
        {
            new Input("Body color H"),
            new Input("Body color S"),
            new Input("Body color L"),
            new Input("Head color H"),
            new Input("Head color S"),
            new Input("Head color L"),
            new Input("Deco color H"),
            new Input("Deco color S"),
            new Input("Deco color L"),
        },
        ["LizardColors"] = new List<Input>()
        {
            new Input("Lizard type", 1, 17, 1),
            new Input("Hue", 0, 1),
            new Input("Sat", 0, 1),
            new Input("Lgt", 0, 1),
            /*new Input("Pink lizard", 1, 2, 2),
            new Input("Hue", 0.77f, 0.97f),
            new Input("Sat", 1, 2, 2),
            new Input("Lgt", 0.35f, 0.65f),*/
        },
        ["LizardVars"] = new List<Input>()
        {
            new Input("Lizard type", 1, 17, 1),
            new Input("Head size", 0.86f, 1.14f),
            new Input("Fatness", 0.76f, 1.24f),
            new Input("Tail length", 0.6f, 1.4f),
            new Input("Tail fatness", 0.7f, 1.1f),
            new Input("Tail color")
        },
        ["VultureWings"] = new List<Input>()
        {
            new Input("Color A hue"),
            new Input("Color A sat", 0.5f, 0.7f),
            new Input("Color A lightness", 0.7f, 0.8f),
            new Input("Color B hue"),
            new Input("Color B sat", 0.8f, 1f),
            new Input("Color B lightness", 0.45f, 1f),
            new Input("Feather count", 13, 19, 1)
        },
        ["VultureKingWings"] = new List<Input>()
        {
            new Input("Color A hue"),
            new Input("Color A sat", 0.5f, 0.7f),
            new Input("Color A lightness", 0.7f, 0.8f),
            new Input("Color B hue"),
            new Input("Color B sat", 0.8f, 1f),
            new Input("Color B lightness", 0.45f, 1f),
            new Input("Feather count", 13, 19, 1)
        },
        ["NoodleflyAdultVars"] = new List<Input>()
        {
            new Input("Wing size", 0.8f, 1.2f),
            new Input("Leg size", 0.6f, 1.4f),
            new Input("Fatness"),
            new Input("Snout length"),
            new Input("Body color R"),
            new Input("Body color G"),
            new Input("Body color B"),
            new Input("Eye color R"),
            new Input("Eye color G"),
            new Input("Eye color B"),
        },
        ["NoodleflyBabyVars"] = new List<Input>()
        {
            new Input("Wing size", 0.4f, 0.8f),
            new Input("Leg size", 0.48f, 1.12f),
            new Input("Fatness"),
            new Input("Snout length"),
            new Input("Body color R"),
            new Input("Body color G"),
            new Input("Body color B"),
            new Input("Eye color R"),
            new Input("Eye color G"),
            new Input("Eye color B"),
        },
        ["BigSpiderVars"] = new List<Input>()
        {
            new Input("Spider type", 0, 2, 1),
            new Input("Number of spines", 10, 38, 1),
            new Input("Leg thickness", 0.7f, 1.1f),
            new Input("Body thickness", 1.8f, 6.1f),
            new Input("Spine color R", value: 1f),
            new Input("Spine color G", value: 0.8f),
            new Input("Spine color B", value: 0.3f)
        },
        ["CentipedeVars"] = new List<Input>()
        {
            new Input("Type", 0, 3, 1),
            new Input("Hue", 0f, 1f, value: 0.04f),
            new Input("Saturation", 0.8f, 1f) { enabled = false },
            new Input("Size", 0f, 1f)
        },
        ["CoalescipedeSize"] = new List<Input>()
        {
            new Input("Size", 0f, 1f)
        },
        ["DropwigVars"] = new List<Input>()
        {
            new Input("Body thickness", 0.6f, 1.4f),
            new Input("Legs thickness", 0.6f, 1.4f),
            new Input("Antennae length", 0.6f, 1.4f),
            new Input("Pinchers length", 40f, 60f),
            new Input("Antennae color intensity"),
            new Input("Hue", 0.6722222f - 0.2f, 0.6722222f + 0.2f),
        },
        ["EggbugVars"] = new List<Input>()
        {
            new Input("Hue", 0, 2, value: 1.35f),
            new Input("Firebug?", 0, 1, 1)
        },
        ["GrappleWormColors"] = new List<Input>()
        {
            new Input("Hue", 0.52f, 0.68f),
            new Input("Saturation", 0.4f, 0.9f),
            new Input("Lightness", 0.15f, 0.3f)
        },
        ["JetfishVars"] = new List<Input>()
        {
            new Input("Tentacle length"),
            new Input("Number of whiskers", 0, 3, 1),
            new Input("Flipper graphic", 0, 4, 1),
            new Input("Flipper size", 0.7f, 1.1f)
        },
        ["LanternMouseVars"] = new List<Input>()
        {
            new Input("Hue"),
            new Input("Dominance")
        },
        ["SnailVars"] = new List<Input>()
        {
            new Input("Size", 0.6f, 1.4f),
            new Input("Color A red"),
            new Input("Color A green"),
            new Input("Color A blue"),
            new Input("Color B red"),
            new Input("Color B green"),
            new Input("Color B blue"),
            new Input("Same color?", 0, 1, 1) { enabled = false }
        },
        ["SquidcadaVars"] = new List<Input>()
        {
            new Input("Hue", 0.45f, 0.65f),
            new Input("Fatness", 0.8f, 1.2f),
            new Input("Tentacle length", 0.6f, 1.4f),
            new Input("Tentacle thickness", 0.6f, 1.4f),
            new Input("Wing length", 0.4f, 1f),
            new Input("Wing thickness", (0.66667f - 0.3f) * 1.5f, (0.66667f + 0.3f) * 1.5f),
            new Input("Has busted wing?", 0, 1, 1),
            new Input("Busted wing", 0, 3, 1)
        },
        ["YeekVars"] = new List<Input>()
        {
            new Input("Red"),
            new Input("Green"),
            new Input("Blue"),
        },
        ["BarnacleVars"] = new List<Input>()
        {
            new Input("Body size", 0.6f, 1f),
            new Input("Eye size", 0.7f, 1f),
            new Input("Leg thickness", 0.7f ,1.3f),
            new Input("Number of cones", 13, 19, 1),
            new Input("Cone radius variance"),
            new Input("Cone length variance"),
            new Input("Cone color variance"),
            new Input("Cone base color H", -0.06f, 0.14f),
            new Input("Cone base color S", 0.3f, 0.7f),
            new Input("Cone base color L", 0.55f, 0.95f),
            new Input("Low-end lighness offset", -0.2f, -0.1f),
            new Input("Hi-end lightness offset", 0f, 0.1f)
        },
        ["DrillCrabVars"] = new List<Input>()
        {
            new Input("Leg thickness"),
            new Input("Leg length"),
            new Input("Body size"),
            new Input("Drill size"),
            new Input("Eyestalk length"),
            new Input("Eye lightness"),
        },
        ["FrogVars"] = new List<Input>()
        {
            new Input("Horn count", 0, 3),
            new Input("Horn scale", 0.8f, 1.2f),
            new Input("Body color H", -0.04f, 0.3f),
            new Input("Body color S", 0.07f, 0.57f, value: 0.57f),
            new Input("Body color L", 0f, 0.58f, value: 0.58f),
            new Input("Belly color H", -0.01f, 0.22f),
            new Input("Belly color S", 0f, 0.55f, value: 0.55f),
            new Input("Belly color L", 0.2f, 0.94f, value: 0.94f)
        },
        ["RatVars"] = new List<Input>()
        {
            new Input("Has big eyes", 0, 1, 1),
            new Input("Whisker length", 20f, 35f),
            new Input("Coat darkness", 0.6f, 1f),
            new Input("Coat color"),
            new Input("Head color H", -0.1f, 0.1f),
            new Input("Head color S", 0f, 0.4f),
            new Input("Head color L", 0.2f, 0.4f)
        },
        ["TardigradeVars"] = new List<Input>
        {
            new Input("Body color H", 0.3f, 0.9f),
            new Input("Body color S", 0.3f, 0.47f),
            new Input("Body color L", 0.5f, 0.8f),
            new Input("Seconary color H"),
            new Input("Secondary color S", 0.75f, 1f),
            new Input("Secondary color L", 0.5f, 0.7f),
            new Input("General scale", 0.46f, 0.7f),
            new Input("Spikes per side", 2, 6, 1),
            new Input("Spike width"),
            new Input("Spike length"),
            new Input("SPike lay back", 0.3f, 1f),
            new Input("Spike puff out", 0f, 0.8f),
            new Input("Ear width"),
            new Input("Ear length")
        },
        #region Lizard cosmetics
        ["PinkLizardCosmetics"] = SpineSpikesVars(LizardType.Pink)
            .Concat(BumpHawkVars(LizardType.Pink))
            .Concat(LongShoulderScalesVars(LizardType.Pink))
            .Concat(ShortBodyScalesVars(LizardType.Pink))
            .Concat(TailTuftVars(LizardType.Pink))
            .Concat(LongHeadScalesVars())
            .ToList(),
        ["GreenLizardCosmetics"] = SpineSpikesVars(LizardType.Green)
            .Concat(BumpHawkVars(LizardType.Green))
            .Concat(LongShoulderScalesVars(LizardType.Green))
            .Concat(ShortBodyScalesVars(LizardType.Green)) // an optimization was made in LizardCosmetics.cginc because in the C# code, there are
                                                           // two cases in which ShortBodyScales can be added but only one input to cover both cases
            .Concat(TailTuftVars(LizardType.Green)) // there's another of the same that got made here, just with TailTuft
            .Concat(LongShoulderScalesVars(LizardType.Green)) // and there's two LongShoulderScales possibilities
            .Concat(LongHeadScalesVars())
            .ToList(),
        ["BlueLizardCosmetics"] = SpineSpikesVars(LizardType.Blue)
            .Concat(BumpHawkVars(LizardType.Blue))
            .Concat(LongShoulderScalesVars(LizardType.Blue))
            .Concat(ShortBodyScalesVars(LizardType.Blue))
            .Concat(TailTuftVars(LizardType.Blue))
            .Concat(LongHeadScalesVars())
            .ToList(),
        ["YellowLizardCosmetics"] = SpineSpikesVars(LizardType.Yellow)
            .Concat(BumpHawkVars(LizardType.Yellow))
            .Concat(LongShoulderScalesVars(LizardType.Yellow))
            .Concat(ShortBodyScalesVars(LizardType.Yellow))
            .Concat(TailTuftVars(LizardType.Yellow))
            .Concat(AntennaeVars())
            .ToList(),
        ["WhiteLizardCosmetics"] = BumpHawkVars(LizardType.White)
            .Concat(ShortBodyScalesVars(LizardType.White))
            .Concat(LongShoulderScalesVars(LizardType.White))
            .Concat(LongHeadScalesVars())
            .Concat(TailTuftVars(LizardType.White))
            .ToList(),
        ["RedLizardCosmetics"] = SpineSpikesVars(LizardType.Red)
            .Concat(BumpHawkVars(LizardType.Red))
            .Concat(LongShoulderScalesVars(LizardType.Red))
            .Concat(ShortBodyScalesVars(LizardType.Red))
            .Concat(TailTuftVars(LizardType.Red))
            .Concat(LongHeadScalesVars())
            .Concat(LongShoulderScalesVars(LizardType.Red))
            .Concat(SpineSpikesVars(LizardType.Red))
            .Concat(TailFinVars(LizardType.Red))
            .Concat(TailTuftVars(LizardType.Red))
            .ToList(),
        ["BlackLizardCosmetics"] = SpineSpikesVars(LizardType.Black)
            .Concat(BumpHawkVars(LizardType.Black))
            .Concat(LongShoulderScalesVars(LizardType.Black))
            .Concat(ShortBodyScalesVars(LizardType.Black))
            .Concat(TailTuftVars(LizardType.Black))
            .Concat(LongHeadScalesVars())
            .Concat(WhiskersVars())
            .ToList(),
        ["SalamanderLizardCosmetics"] = Melanistic()
            .Concat(SpineSpikesVars(LizardType.Salamander))
            .Concat(BumpHawkVars(LizardType.Salamander))
            .Concat(AxolotlGillsVars())
            .Concat(TailFinVars(LizardType.Salamander))
            .ToList(),
        ["CyanLizardCosmetics"] = WingScalesVars()
            .Concat(TailTuftVars(LizardType.Cyan))
            .Concat(TailGeckoScalesVars())
            .ToList(),
        ["CaramelLizardCosmetics"] = BodyStripesVars(LizardType.Caramel)
            .Concat(SpineSpikesVars(LizardType.Caramel))
            .Concat(LongShoulderScalesVars(LizardType.Caramel))
            .Concat(ShortBodyScalesVars(LizardType.Caramel))
            .Concat(TailTuftVars(LizardType.Caramel))
            .Concat(LongHeadScalesVars())
            .Concat(BumpHawkVars(LizardType.Caramel))
            .Append(new Input("Body color hue", 0.075f, 0.125f))
            .Append(new Input("Body color sat", 0.3f, 0.9f))
            .Append(new Input("Body color light", 0.7f, 1f))
            .Append(new Input("Head color hue", 0.07f, 0.13f))
            .Append(new Input("Head color light", 0.19f, 0.91f))
            .ToList(),
        ["ZoopLizardCosmetics"] = WingScalesVars()
            .Concat(SpineSpikesVars(LizardType.Zoop))
            .Concat(TailTuftVars(LizardType.Zoop))
            .Concat(SpineSpikesVars(LizardType.Zoop))
            .Concat(BumpHawkVars(LizardType.Zoop))
            .Concat(LongShoulderScalesVars(LizardType.Zoop))
            .Concat(ShortBodyScalesVars(LizardType.Zoop))
            .Concat(TailTuftVars(LizardType.Zoop))
            .Concat(LongHeadScalesVars())
            .ToList(),
        ["TrainLizardCosmetics"] = SpineSpikesVars(LizardType.Train)
            .Concat(BumpHawkVars(LizardType.Train))
            .Concat(LongShoulderScalesVars(LizardType.Train))
            .Concat(ShortBodyScalesVars(LizardType.Train))
            .Concat(TailTuftVars(LizardType.Train))
            .Concat(LongHeadScalesVars())
            .Concat(LongShoulderScalesVars(LizardType.Train))
            .Concat(SpineSpikesVars(LizardType.Train))
            .Concat(TailFinVars(LizardType.Train))
            .Concat(TailTuftVars(LizardType.Train))
            .ToList(),
        ["EelLizardCosmetics"] = AxolotlGillsVars()
            .Concat(TailGeckoScalesVars())
            .Concat(LongShoulderScalesVars(LizardType.Eel))
            .Concat(TailFinVars(LizardType.Eel))
            .Concat(ShortBodyScalesVars(LizardType.Eel))
            .Concat(TailFinVars(LizardType.Eel))
            .Concat(TailTuftVars(LizardType.Eel))
            .Concat(SpineSpikesVars(LizardType.Eel))
            .Concat(BumpHawkVars(LizardType.Eel))
            .Concat(LongShoulderScalesVars(LizardType.Eel))
            .Concat(ShortBodyScalesVars(LizardType.Eel))
            .Concat(TailTuftVars(LizardType.Eel))
            .Concat(LongHeadScalesVars())
            .ToList(),
        ["IndigoLizardCosmetics"] = SkinkSpecklesVars()
            .ToList(),
        ["PeachLizardCosmetics"] = SpineSpikesVars(LizardType.Peach)
            .Concat(BumpHawkVars(LizardType.Peach))
            .Concat(LongShoulderScalesVars(LizardType.Peach))
            .Concat(ShortBodyScalesVars(LizardType.Peach))
            .Concat(TailTuftVars(LizardType.Peach))
            .Concat(LongHeadScalesVars())
            .Concat(PeachHeadStripesVars())
            .Concat(TailFinVars(LizardType.Peach))
            .Concat(PeachBackFinVars())
            .ToList(),
        #endregion
    };

    private const string KERNEL_NAME = "CS_IDFinderMain";

    [SerializeField] private int startingId;
    [SerializeField] private int threadsX = 1;
    [SerializeField] private int threadsY = 1;
    [SerializeField] private int numResults = 32;

    [SerializeField] private int selectedShaderIndex;

    private ComputeShader selectedShader;
    private VisualElement inputPane;
    private VisualElement outputPane;
    private Label countLabel;


    [MenuItem("ID Finder/Compute Shader Tester")]
    public static void ShowWindow()
    {
        ComputeTester wnd = GetWindow<ComputeTester>();
        wnd.titleContent = new GUIContent("Compute Shader Tester");
        wnd.minSize = new Vector2(1050, 650);
    }

    [MenuItem("ID Finder/Reset Compute Shader Tester")]
    public static void HideWindow()
    {
        ComputeTester wnd = GetWindow<ComputeTester>();
        wnd.Close();
    }

    public void CreateGUI()
    {
        // Why do these start as 0...
        if (threadsX < 1) threadsX = 1;
        if (threadsY < 1) threadsY = 1;

        // Find all compute shaders in the project
        string[] guids = AssetDatabase.FindAssets("t:ComputeShader");
        List<ComputeShader> shaderList = new List<ComputeShader>();
        foreach (var guid in guids)
        {
            shaderList.Add(AssetDatabase.LoadAssetAtPath<ComputeShader>(AssetDatabase.GUIDToAssetPath(guid)));
        }
        
        // Create panes
        var splitView = new TwoPaneSplitView(0, 200, TwoPaneSplitViewOrientation.Horizontal);
        rootVisualElement.Add(splitView);
        var shaderPanel = new ListView();
        splitView.Add(shaderPanel);
        var rightPane = new ScrollView();
        splitView.Add(rightPane);

        Box extraBox, extraRow1, extraRow2, extraRow3;
        IntegerField startInput, numResultsInput, threadsXInput, threadsYInput;
        Button startButton;
        rightPane.Add(new Label("Inputs:"));
        rightPane.Add(inputPane = new Box());

        rightPane.Add(new Label("Extra setup:"));
        rightPane.Add(extraBox = new Box());
        extraBox.Add(extraRow1 = new Box());
        extraBox.Add(extraRow2 = new Box());
        extraBox.Add(extraRow3 = new Box());
        extraRow1.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
        extraRow2.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
        extraRow3.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);

        extraRow1.Add(startInput = new IntegerField("Starting id"));
        startInput.style.width = new StyleLength(Length.Percent(30f));
        startInput.value = startingId;
        extraRow1.Add(numResultsInput = new IntegerField("Results to show"));
        numResultsInput.style.width = new StyleLength(Length.Percent(30f));
        numResultsInput.value = numResults;
        extraRow2.Add(threadsXInput = new IntegerField("Threads x"));
        threadsXInput.style.width = new StyleLength(Length.Percent(30f));
        threadsXInput.value = threadsX;
        extraRow2.Add(threadsYInput = new IntegerField("Threads y"));
        threadsYInput.style.width = new StyleLength(Length.Percent(30f));
        threadsYInput.value = threadsY;
        extraRow3.Add(countLabel = new Label($"{threadsX * threadsY * 32} results"));
        rightPane.Add(startButton = new Button() { text = "Run" });
        rightPane.Add(new Label("Output:"));
        rightPane.Add(outputPane = new Box());

        startInput.RegisterCallback<ChangeEvent<int>>((evt) => startingId = startInput.value);
        numResultsInput.RegisterCallback<ChangeEvent<int>>((evt) => numResults = numResultsInput.value);
        threadsXInput.RegisterCallback<ChangeEvent<int>>((evt) =>
        {
            uint x = 32;
            uint y = 32;
            if (selectedShader != null)
            {
                selectedShader.GetKernelThreadGroupSizes(selectedShader.FindKernel(KERNEL_NAME), out x, out y, out _);
            }
            (threadsX, countLabel.text) = (threadsXInput.value, $"{threadsXInput.value * threadsY * 32 * x * y} results");
        });
        threadsYInput.RegisterCallback<ChangeEvent<int>>((evt) =>
        {
            uint x = 32;
            uint y = 32;
            if (selectedShader != null)
            {
                selectedShader.GetKernelThreadGroupSizes(selectedShader.FindKernel(KERNEL_NAME), out x, out y, out _);
            }
            (threadsY, countLabel.text) = (threadsYInput.value, $"{threadsYInput.value * threadsX * 32 * x * y} results");
        });
        startButton.clicked += StartButton_clicked;

        // Set up shader panel
        shaderPanel.makeItem = () => new Label();
        shaderPanel.bindItem = (item, index) => (item as Label).text = shaderList[index].name;
        shaderPanel.itemsSource = shaderList;
        shaderPanel.selectedIndex = selectedShaderIndex;
        shaderPanel.onSelectionChange += (_) =>
        {
            selectedShaderIndex = shaderPanel.selectedIndex;
            selectedShader = shaderList[selectedShaderIndex];
        };
        shaderPanel.onSelectionChange += ShaderPanel_onSelectionChange;
    }

    private void ShaderPanel_onSelectionChange(IEnumerable<object> obj)
    {
        inputPane.Clear();
        if (selectedShader != null && TestInputs.TryGetValue(selectedShader.name, out var inputs))
        {
            foreach (var input in inputs)
            {
                var containerBox = new Box();
                containerBox.style.width = new StyleLength(Length.Percent(100));
                containerBox.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);

                var enableInput = new Toggle()
                {
                    value = input.enabled
                };
                enableInput.style.width = new StyleLength(new Length(20f, LengthUnit.Pixel));
                enableInput.RegisterCallback<ChangeEvent<bool>>((evt) => input.enabled = enableInput.value);
                containerBox.Add(enableInput);

                inputPane.Add(containerBox);
                var label = new Label(input.name);
                label.style.width = new StyleLength(Length.Percent(25));
                containerBox.Add(label);

                var valueInput = new Slider($"Value ({input.value})", input.min, input.max)
                {
                    value = input.value
                };
                //valueInput.style.width = new StyleLength(Length.Percent(55));
                valueInput.style.flexGrow = new StyleFloat(1f);
                valueInput.GetInput().style.position = new StyleEnum<Position>(Position.Relative);
                valueInput.RegisterCallback<ChangeEvent<float>>((evt) =>
                {
                    if (input.step == 0)
                    {
                        input.value = valueInput.value;
                    }
                    else
                    {
                        input.value = input.min + Mathf.Round((valueInput.value - input.min) / input.step) * input.step;
                        valueInput.value = input.value;
                    }
                    valueInput.label = $"Value ({input.value})";
                });
                containerBox.Add(valueInput);

                var biasLabel = new Label("Bias: ");
                biasLabel.style.width = new StyleLength(StyleKeyword.Auto);
                containerBox.Add(biasLabel);

                var biasInput = new IntegerField()
                {
                    value = input.bias
                };
                biasInput.style.width = new StyleLength(new Length(40f, LengthUnit.Pixel));
                biasInput.RegisterCallback<ChangeEvent<int>>((evt) => input.bias = biasInput.value);
                containerBox.Add(biasInput);
            }

            // update expecting panel
            uint x = 32;
            uint y = 32;
            if (selectedShader != null)
            {
                selectedShader.GetKernelThreadGroupSizes(selectedShader.FindKernel(KERNEL_NAME), out x, out y, out _);
            }
            countLabel.text = $"{threadsX * threadsY * 32 * x * y} results";
        }
        else
        {
            inputPane.Add(new Label("No test input setup found :("));
        }
    }

    private void StartButton_clicked()
    {
        outputPane.Clear();

        if (selectedShader == null)
        {
            outputPane.Add(new Label("Selected shader was null!"));
            return;
        }

        if (TestInputs.TryGetValue(selectedShader.name, out var inputs))
        {
            // Setup
            int kernel = selectedShader.FindKernel(KERNEL_NAME);
            selectedShader.GetKernelThreadGroupSizes(kernel, out uint sizeX, out uint sizeY, out _);
            int total = (int)sizeX * threadsX * (int)sizeY * threadsY * 32;

            // Load shader buffers and values
            ComputeBuffer inputBuffer = new ComputeBuffer(inputs.Count, 12);
            ComputeBuffer resultsBuffer = new ComputeBuffer(total, 8);

            var gpuInputs = inputs.Select(x => x.AsGPUInput()).ToArray();
            //Debug.Log(string.Join("\n", gpuInputs));
            inputBuffer.SetData(gpuInputs);

            selectedShader.SetBuffer(kernel, "_IDFinderInputs", inputBuffer);
            selectedShader.SetBuffer(kernel, "_IDFinderResults", resultsBuffer);
            selectedShader.SetInts("_IDFinderDispatch", threadsX, threadsY, 1);
            selectedShader.SetInt("_IDFinderStart", startingId);

            // Dispatch and request
            outputPane.Add(new Label($"Dispatching to GPU... (expecting {total} results)"));
            selectedShader.Dispatch(kernel, threadsX, threadsY, 1);
            AsyncGPUReadback.Request(resultsBuffer, (request) =>
            {
                if (request.hasError)
                {
                    // Uh oh! Fucky wucky!
                    outputPane.Add(new Label("GPU error!"));
                }
                else
                {
                    outputPane.Add(new Label("Successfully dispatched without errors! Done: " + request.done));

                    // Retrieve results
                    Result[] results = new Result[total];
                    resultsBuffer.GetData(results);

                    Array.Sort(results, new Result.ResultComparer());

                    int num = Math.Min(total, numResults);
                    for (int i = 0; i < num; i++)
                    {
                        outputPane.Add(new Label(results[i].ToString()));
                    }
                }

                // Free resources
                inputBuffer.Release();
                resultsBuffer.Release();
            });
        }
        else
        {
            outputPane.Add(new Label("No test input setup found :("));
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4, Size = 8)]
    public struct Result
    {
        public int id;
        public float dist;

        public override readonly string ToString()
        {
            return $"Result: {id} (dist: {dist})";
        }

        public class ResultComparer : IComparer<Result>
        {
            int IComparer<Result>.Compare(Result x, Result y)
            {
                int dist = x.dist.CompareTo(y.dist);
                if (dist == 0)
                {
                    if (x.id == int.MinValue) return 1;
                    if (y.id == int.MinValue) return -1;
                    return Math.Abs(x.id) - Math.Abs(y.id);
                }
                return dist;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4, Size = 12)]
    public struct GPUInput
    {
        public float value;
        public float range;
        public int bias;

        public override readonly string ToString()
        {
            return $"GPUInput(value={value}, range={range}, bias={bias})";
        }
    }

    public class Input
    {
        [SerializeField] public bool enabled;
        [SerializeField] public string name;

        [SerializeField] public float value;
        [SerializeField] public float min;
        [SerializeField] public float max;
        [SerializeField] public float step;

        [SerializeField] public int bias;

        public Input(string name, float min = 0f, float max = 1f, float step = 0f, float? value = null)
        {
            this.name = name;
            this.min = min;
            this.max = max;
            this.step = step;
            enabled = true;
            bias = 1;
            this.value = value ?? min;
        }

        public GPUInput AsGPUInput()
        {
            return new GPUInput
            {
                value = value,
                range = max - min,
                bias = enabled ? bias : 0,
            };
        }
    }
}