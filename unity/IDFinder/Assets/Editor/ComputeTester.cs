using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;
using System.Linq;
using UnityEngine.Rendering;


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
    };

    [SerializeField] private int startingId;
    [SerializeField] private int threadsX = 1;
    [SerializeField] private int threadsY = 1;
    [SerializeField] private int numResults = 32;

    [SerializeField] private int selectedShaderIndex;

    private ComputeShader selectedShader;
    private VisualElement inputPane;
    private VisualElement outputPane;


    [MenuItem("ID Finder/Compute Shader Tester")]
    public static void ShowWindow()
    {
        ComputeTester wnd = GetWindow<ComputeTester>();
        wnd.titleContent = new GUIContent("Compute Shader Tester");
        wnd.minSize = new Vector2(250, 450);
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

        Box extraBox;
        IntegerField startInput, numResultsInput, threadsXInput, threadsYInput;
        Button startButton;
        Label countLabel;
        rightPane.Add(new Label("Inputs:"));
        rightPane.Add(inputPane = new Box());
        rightPane.Add(new Label("Extra setup:"));
        rightPane.Add(extraBox = new Box());
        extraBox.Add(startInput = new IntegerField("Starting id"));
        startInput.value = startingId;
        extraBox.Add(numResultsInput = new IntegerField("Results to show"));
        numResultsInput.value = numResults;
        extraBox.Add(threadsXInput = new IntegerField("Threads x"));
        threadsXInput.value = threadsX;
        extraBox.Add(threadsYInput = new IntegerField("Threads y"));
        threadsYInput.value = threadsY;
        extraBox.Add(countLabel = new Label($"{threadsX * threadsY * 32} results"));
        rightPane.Add(startButton = new Button() { text = "Run" });
        rightPane.Add(new Label("Output:"));
        rightPane.Add(outputPane = new Box());

        startInput.RegisterCallback<ChangeEvent<int>>((evt) => startingId = startInput.value);
        threadsXInput.RegisterCallback<ChangeEvent<int>>((evt) => (threadsX, countLabel.text) = (threadsXInput.value, $"{threadsXInput.value * threadsY * 32 * 32 * 32} results"));
        threadsYInput.RegisterCallback<ChangeEvent<int>>((evt) => (threadsY, countLabel.text) = (threadsYInput.value, $"{threadsYInput.value * threadsX * 32 * 32 * 32} results"));
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
                inputPane.Add(containerBox);
                containerBox.Add(new Label(input.name));

                var valueInput = new Slider($"Value ({input.value})", input.min, input.max)
                {
                    value = input.value
                };
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

                var enableInput = new Toggle("Enabled")
                {
                    value = input.enabled
                };
                enableInput.RegisterCallback<ChangeEvent<bool>>((evt) => input.enabled = enableInput.value);
                containerBox.Add(enableInput);

                var biasInput = new IntegerField("Bias")
                {
                    value = input.bias
                };
                biasInput.RegisterCallback<ChangeEvent<int>>((evt) => input.bias = biasInput.value);
                containerBox.Add(biasInput);
            }
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
            int kernel = selectedShader.FindKernel("CS_IDFinderMain");
            selectedShader.GetKernelThreadGroupSizes(kernel, out uint sizeX, out uint sizeY, out _);
            int total = (int)sizeX * threadsX * (int)sizeY * threadsY * 32;

            // Load shader buffers and values
            ComputeBuffer inputBuffer = new ComputeBuffer(inputs.Count, 12);
            ComputeBuffer resultsBuffer = new ComputeBuffer(total, 8);

            var gpuInputs = inputs.Select(x => x.AsGPUInput()).ToArray();
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
    private struct Result
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
                return x.dist.CompareTo(y.dist);
            }
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4, Size = 12)]
    private struct GPUInput
    {
        public float value;
        public float range;
        public int bias;
    }

    private class Input
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