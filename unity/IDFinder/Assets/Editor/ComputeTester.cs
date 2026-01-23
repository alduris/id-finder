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
        }
    };

    [SerializeField] private int startingId;
    [SerializeField] private int idsToSearch;

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
        IntegerField startInput;
        IntegerField rangeInput;
        Button startButton;
        rightPane.Add(new Label("Inputs:"));
        rightPane.Add(inputPane = new Box());
        rightPane.Add(new Label("Extra setup:"));
        rightPane.Add(extraBox = new Box());
        extraBox.Add(startInput = new IntegerField("Starting id"));
        extraBox.Add(rangeInput = new IntegerField("Ids to search"));
        rightPane.Add(startButton = new Button() { text = "Run" });
        rightPane.Add(new Label("Output:"));
        rightPane.Add(outputPane = new Box());

        startInput.RegisterCallback<ChangeEvent<int>>((evt) => startingId = startInput.value);
        rangeInput.RegisterCallback<ChangeEvent<int>>((evt) => idsToSearch = rangeInput.value);
        startButton.clicked += StartButton_clicked;

        // Set up shader panel
        shaderPanel.makeItem = () => new Label();
        shaderPanel.bindItem = (item, index) => (item as Label).text = shaderList[index].name;
        shaderPanel.itemsSource = shaderList;
        shaderPanel.selectedIndex = selectedShaderIndex;
        shaderPanel.onSelectionChange += ShaderPanel_onSelectionChange;
        shaderPanel.onSelectionChange += (_) =>
        {
            selectedShaderIndex = shaderPanel.selectedIndex;
            selectedShader = shaderList[selectedShaderIndex];
        };
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

                var valueInput = new Slider("Value", input.min, input.max)
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
                    }
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
            int kernel = selectedShader.FindKernel("CSMain");
            selectedShader.GetKernelThreadGroupSizes(kernel, out uint sizeX, out _, out _);
            int numThreads = Mathf.Max(1, (int)(idsToSearch / sizeX));
            int total = numThreads * (int)sizeX;

            // Load shader buffers and values
            ComputeBuffer inputBuffer = new ComputeBuffer(inputs.Count, 16);
            ComputeBuffer resultsBuffer = new ComputeBuffer(total, sizeof(float));
            ComputeBuffer outputIDBuffer = new ComputeBuffer(total, sizeof(int));

            var gpuInputs = inputs.Select(x => x.AsGPUInput()).ToArray();
            inputBuffer.SetData(gpuInputs);

            selectedShader.SetBuffer(kernel, "_IDFinderInputs", inputBuffer);
            selectedShader.SetBuffer(kernel, "_IDFinderResults", resultsBuffer);
            selectedShader.SetBuffer(kernel, "_IDFinderIDs", outputIDBuffer);
            selectedShader.SetInt("_IDFinderStart", startingId);

            // Dispatch and request
            outputPane.Add(new Label("Dispatching to GPU..."));
            selectedShader.Dispatch(kernel, numThreads, 1, 1);
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
                    float[] results = new float[total];
                    int[] resultIDs = new int[total];
                    resultsBuffer.GetData(results);
                    outputIDBuffer.GetData(resultIDs);

                    for (int i = 0; i < total; i++)
                    {
                        outputPane.Add(new Label($"Result: {resultIDs[i]} (dist: {results[i]})"));
                    }
                }

                // Free resources
                inputBuffer.Release();
                resultsBuffer.Release();
                outputIDBuffer.Release();
            });
        }
        else
        {
            outputPane.Add(new Label("No test input setup found :("));
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4, Size = 16)]
    private struct GPUInput
    {
        public float value;
        public float start;
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

        public Input(string name, float min = 0f, float max = 1f, float step = 0f)
        {
            this.name = name;
            this.min = min;
            this.max = max;
            this.step = step;
            enabled = true;
            bias = 1;
            value = min;
        }

        public GPUInput AsGPUInput()
        {
            return new GPUInput
            {
                value = value,
                start = min,
                range = max - min,
                bias = enabled ? bias : 0,
            };
        }
    }
}