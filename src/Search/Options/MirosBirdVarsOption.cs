using System;
using System.Collections.Generic;
using FinderMod.Inputs;
using FinderMod.Tabs;
using Menu.Remix.MixedUI;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace FinderMod.Search.Options
{
    internal class MirosBirdVarsOption : Option
    {
        private readonly IntInput PlumageGraphicInput;
        private readonly FloatInput PlumageLengthInput;
        private readonly FloatInput PlumageDirectionInput;
        private readonly PlumageInput PlumageDensityInput;

        private readonly FloatInput BeakFatnessInput;
        private readonly IntInput TopTeethCountInput;
        private readonly IntInput BottomTeethCountInput;

        private readonly FloatInput EyeSizeInput;
        private readonly ColorLerpInput EyeColorInput;
        
        private readonly FloatInput NeckFatnessInput;
        private readonly FloatInput ThighSizeInput;

        public MirosBirdVarsOption()
        {
            elements = [
                PlumageGraphicInput = new IntInput("Plumage graphic", 0, 6),
                PlumageLengthInput = new FloatInput("Plumage length", 0.2f, 1.2f),
                PlumageDirectionInput = new FloatInput("Plumage direction", 1f, 11f),
                PlumageDensityInput = new PlumageInput(),
                new Whitespace(),
                BeakFatnessInput = new FloatInput("Beak fatness"),
                TopTeethCountInput = new IntInput("Beak top teeth count", 10, 19),
                BottomTeethCountInput = new IntInput("Beak bottom teeth count", 10, 19),
                new Whitespace(),
                EyeSizeInput = new FloatInput("Eye size", 0.5f, 1.4f),
                EyeColorInput = new ColorLerpInput("Eye color", new HSLColor(0.08f, 1f, 0.5f).rgb, new HSLColor(0.17f, 1f, 0.5f).rgb),
                new Whitespace(),
                NeckFatnessInput = new FloatInput("Neck fatness", 0.8f, 1.2f),
                ThighSizeInput = new FloatInput("Thigh size", 0.5f, 1.4f)
                ];
        }

        private MirosBirdResults GetResults(XORShift128 Random)
        {
            // Plumage and some misc stuff
            int plumageGraphic = Random.Range(0, 7);
            float neckFatness = Mathf.Lerp(0.8f, 1.2f, Random.Value);
            float beakFatness = Random.Value;
            float plumageLength = Mathf.Lerp(0.2f, 1.2f, Mathf.Pow(Random.Value, 0.75f));
            Random.Shift(4);
            float plumageDirection = Mathf.Lerp(1f, 11f, Random.Value);
            float plumageDensity = Random.Value;
            float eyeSize = Mathf.Lerp(0.5f, 1.4f, Random.Value);
            float tighSize = Mathf.Lerp(0.5f, 1.4f, Random.Value); // yes the typo is in the base game code

            // Beak
            int topTeethCount = Random.Range(10, 20);
            Random.Shift(topTeethCount * 3);
            int btmTeethCount = Random.Range(10, 20);
            Random.Shift(btmTeethCount * 3);

            // Plumage. But specific.
            bool hasNeckPlumage = false, hasTailPlumage = false;
            if (Random.Value > 0.055555556f)
            {
                hasNeckPlumage = true;
                Random.Shift();
                int featherCount = (int)Mathf.Lerp(6f, 30f, Mathf.Pow(plumageDensity, 1.7f));
                int contour = Random.Range(0, 3);
                for (int i = 0; i < featherCount; i++)
                {
                    if (contour == 0) Random.Shift();
                    Random.Shift();
                    if (Random.Value < 0.04761905f) Random.Shift();
                    else if (Random.Value < 0.5f) Random.Shift();
                }
                Random.Shift();
            }
            if (Random.Value > 0.055555556f)
            {
                hasTailPlumage = true;
                int featherCount = (int)Mathf.Lerp(2f, 30f, plumageDensity);
                Random.Shift();
                if (Random.Value < 0.05f) Random.Shift();
                for (int i = 0; i < featherCount; i++)
                {
                    Random.Shift(4);
                    if (Random.Value < 0.04761905f) Random.Shift();
                    else if (Random.Value < 0.5f) Random.Shift();
                }
            }
            float eyeColor = Random.Value;

            return new MirosBirdResults
            {
                plumageGraphic = plumageGraphic,
                plumageLength = plumageLength,
                plumageDirection = plumageDirection,
                plumageDensity = plumageDensity,
                hasNeckPlumage = hasNeckPlumage,
                hasTailPlumage = hasTailPlumage,
                beakFatness = beakFatness,
                topTeethCount = topTeethCount,
                bottomTeethCount = btmTeethCount,
                eyeSize = eyeSize,
                eyeColor = eyeColor,
                neckFatness = neckFatness,
                thighSize = tighSize
            };
        }

        public override float Execute(XORShift128 Random)
        {
            var results = GetResults(Random);
            float r = 0f;

            r += DistanceIf(results.plumageGraphic, PlumageGraphicInput);
            r += DistanceIf(results.plumageLength, PlumageLengthInput);
            r += DistanceIf(results.plumageDirection, PlumageDirectionInput);
            r += DistanceIf(results.plumageDensity, PlumageDensityInput.DensityInput);
            r += DistanceIf(results.hasNeckPlumage, PlumageDensityInput.NeckInput);
            r += DistanceIf(results.hasTailPlumage, PlumageDensityInput.TailInput);

            r += DistanceIf(results.beakFatness, BeakFatnessInput);
            r += DistanceIf(results.topTeethCount, TopTeethCountInput);
            r += DistanceIf(results.bottomTeethCount, BottomTeethCountInput);

            r += DistanceIf(results.eyeSize, EyeSizeInput);
            r += DistanceIf(results.eyeColor, EyeColorInput);

            r += DistanceIf(results.neckFatness, NeckFatnessInput);
            r += DistanceIf(results.thighSize, ThighSizeInput);

            return r;
        }

        protected override IEnumerable<string> GetValues(XORShift128 Random)
        {
            var results = GetResults(Random);

            int neck = (int)Mathf.Lerp(6f, 30f, Mathf.Pow(results.plumageDensity, 1.7f));
            int tail = (int)Mathf.Lerp(2f, 30f, results.plumageDensity);

            yield return $"Plumage graphic: {results.plumageGraphic}";
            yield return $"Plumage length: {results.plumageLength}";
            yield return $"Plumage direction: {results.plumageDirection}";
            yield return $"Plumage density: {results.plumageDensity}";
            if (results.hasNeckPlumage) yield return $"Has neck plumage with {neck} feathers";
            if (results.hasTailPlumage) yield return $"Has tail plumage with {tail} feathers";
            yield return null!;
            yield return $"Beak fatness: {results.beakFatness}";
            yield return $"Beak top teeth count: {results.topTeethCount}";
            yield return $"Beak bottom teeth count: {results.bottomTeethCount}";
            yield return null!;
            yield return $"Eye size: {results.eyeSize}";
            yield return $"Eye color: hsl({Mathf.Lerp(0.08f, 0.17f, results.eyeColor)}, 1, 0.5)";
            yield return null!;
            yield return $"Neck fatness: {results.neckFatness}";
            yield return $"Thigh size: {results.thighSize}";
        }

        private struct MirosBirdResults
        {
            public int plumageGraphic;
            public float plumageLength;
            public float plumageDirection;
            public float plumageDensity;
            public bool hasNeckPlumage;
            public bool hasTailPlumage;

            public float beakFatness;
            public int topTeethCount;
            public int bottomTeethCount;

            public float eyeSize;
            public float eyeColor;

            public float neckFatness;
            public float thighSize;
        }

        private class PlumageInput : IElement, ISaveInHistory
        {
            public readonly BoolInput NeckInput;
            public readonly BoolInput TailInput;
            public readonly FloatInput DensityInput;
            private OpLabel countLabel = null!;

            public bool HasPlumage => (!NeckInput.enabled || NeckInput.value || !TailInput.enabled || TailInput.value) && DensityInput.enabled;

            public PlumageInput()
            {
                NeckInput = new BoolInput("Has neck plumage", true);
                TailInput = new BoolInput("Has tail plumage", true);
                DensityInput = new FloatInput("Plumage density");

                NeckInput.OnValueChanged += UpdateQueryBox;
                TailInput.OnValueChanged += UpdateQueryBox;
                DensityInput.OnValueChanged += DensityInput_OnValueChanged;
            }

            private void UpdateQueryBox(Input<bool> input, bool value, bool oldValue)
            {
                if (value != oldValue) SearchTab.instance.UpdateQueryBox();
            }

            private string DensityString(float value)
            {
                int neck = (int)Mathf.Lerp(6f, 30f, Mathf.Pow(value, 1.7f));
                int tail = (int)Mathf.Lerp(2f, 30f, value);
                return $"Neck feathers: {neck} | Tail feathers: {tail}";
            }

            private void DensityInput_OnValueChanged(Input<float> input, float value, float oldValue)
            {
                if (countLabel != null)
                {
                    countLabel.text = DensityString(value);
                }
            }

            public float Height => NeckInput.Height + TailInput.Height + DensityInput.Height + (HasPlumage ? 33f : 12f);

            public string SaveKey => "Plumage density";

            public void Create(float x, ref float y, List<UIelement> elements)
            {
                countLabel = null!;

                NeckInput.Create(x, ref y, elements);
                y -= 6f;
                TailInput.Create(x, ref y, elements);
                y -= 6f;
                DensityInput.Create(x, ref y, elements);

                if (HasPlumage)
                {
                    y -= 21f;
                    countLabel = new OpLabel(x, y, DensityString(DensityInput.value));
                    elements.Add(countLabel);
                }
            }

            public JObject ToSaveData()
            {
                return new JObject()
                {
                    ["density"] = DensityInput.ToSaveData(),
                    ["neck"] = NeckInput.ToSaveData(),
                    ["tail"] = TailInput.ToSaveData(),
                };
            }

            public void FromSaveData(JObject data)
            {
                NeckInput.FromSaveData((JObject)data["neck"]!);
                TailInput.FromSaveData((JObject)data["tail"]!);
                DensityInput.FromSaveData((JObject)data["density"]!);
            }

            public IEnumerable<string> GetHistoryLines()
            {
                foreach (var line in NeckInput.GetHistoryLines()) yield return line;
                foreach (var line in TailInput.GetHistoryLines()) yield return line;
                foreach (var line in DensityInput.GetHistoryLines()) yield return line;
            }
        }
    }
}
