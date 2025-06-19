using System.Collections.Generic;
using System.ComponentModel;
using FinderMod.Inputs;
using Menu.Remix.MixedUI;
using Newtonsoft.Json.Linq;
using RWCustom;
using UnityEngine;

namespace FinderMod.Search.Options
{
    internal class BigSpiderVarsOption : Option
    {
        public enum SpiderType
        {
            Big,
            Spitter,
            Mother
        }

        private readonly SpiderType type;

        private readonly IntInput NumScalesInput;
        private readonly FloatInput LegThicknessInput;
        private readonly FloatInput BodyThicknessInput;
        private readonly BigSpiderColorInput SpineColorInput;

        public BigSpiderVarsOption(SpiderType type)
        {
            this.type = type;
            float bodyThicknessAdd = type switch
            {
                SpiderType.Spitter => 1.7f,
                SpiderType.Mother => 5f,
                _ => 0.9f
            };
            elements = [
                NumScalesInput = new IntInput("Number of spines", type == SpiderType.Spitter ? 26 : 10, type == SpiderType.Spitter ? 36 : 26),
                LegThicknessInput = new FloatInput("Leg thickness", 0.7f, 1.1f),
                BodyThicknessInput = new FloatInput("Body thickness", 0.9f + bodyThicknessAdd, 1.1f + bodyThicknessAdd),
                SpineColorInput = new BigSpiderColorInput(type),
                ];
        }

        private BigSpiderResults GetResults(XORShift128 Random)
        {
            // BigSpider..ctor
            var (x, y, z, w) = (Random.x, Random.y, Random.z, Random.w);
            Color color = type switch
            {
                SpiderType.Spitter => Color.Lerp(new Color(1f, 0f, 0f), Custom.HSL2RGB(Random.Value, Random.Value, Random.Value), Random.Value * 0.2f),
                SpiderType.Mother => Color.Lerp(new Color(0f, 1f, 0f), Custom.HSL2RGB(Random.Value, Random.Value, Random.Value), Random.Value * 0.2f),
                _ => Color.Lerp(new Color(1f, 0.8f, 0.3f), Custom.HSL2RGB(Random.Value, Random.Value, Random.Value), Random.Value * 0.2f),
            };

            // BigSpiderGraphics..ctor
            Random.InitState(x, y, z, w);
            bool spitter = type == SpiderType.Spitter;
            bool mother = type == SpiderType.Mother;

            int scales = (spitter ? 10 : 0) + Random.Range(spitter ? 16 : 10, Random.Range(20, 28));
            float legsThickness = Mathf.Lerp(0.7f, 1.1f, Random.Value);
            float bodyThickness = Mathf.Lerp(0.9f, 1.1f, Random.Value) + (spitter ? 1.7f : 0.9f);
            if (mother)
            {
                bodyThickness = Mathf.Lerp(0.9f, 1.1f, Random.Value) + 5f;
            }

            // dead leg check is here if we want to include it later

            return new BigSpiderResults
            {
                numScales = scales,
                legThickness = legsThickness,
                bodyThickness = bodyThickness,
                spineColor = color,
            };
        }

        public override float Execute(XORShift128 Random)
        {
            var results = GetResults(Random);
            float colorDistance = 0f;
            if (SpineColorInput.Enabled)
            {
                // I think this might be the most scuffed UI element I've made for ID Finder so far
                // Being so cursed to the point where I have to code in the distance calculation instead of using the ones I made for the inputs
                int bias = SpineColorInput.Bias;
                Color blend = SpineColorInput.BlendedColor;
                colorDistance = Vector4.Distance((Vector4)blend, (Vector4)results.spineColor) * bias;
            }
            return DistanceIf(results.numScales, NumScalesInput)
                + DistanceIf(results.legThickness, LegThicknessInput)
                + DistanceIf(results.bodyThickness, BodyThicknessInput)
                + colorDistance;
        }

        protected override IEnumerable<string> GetValues(XORShift128 Random)
        {
            var results = GetResults(Random);
            yield return $"Number of spines: {results.numScales}";
            yield return $"Leg thickness: {results.legThickness}";
            yield return $"Body thickness: {results.bodyThickness}";
            yield return $"Spine color: rgb({results.spineColor.r}, {results.spineColor.g}, {results.spineColor.b})";
        }

        private struct BigSpiderResults
        {
            public int numScales;
            public float legThickness;
            public float bodyThickness;
            public Color spineColor;
        }

        private class BigSpiderColorInput(SpiderType type) : IElement, ISaveInHistory
        {
            private readonly FloatInput BlendInput = new("Spine color", 0f, 0.2f);
            private Color selectedColor = new(Random.value, Random.value, Random.value);
            private OpColorPicker.PickerMode pickerMode = OpColorPicker.PickerMode.HSL;

            public Color BlendedColor
            {
                get
                {
                    Color orig = type switch
                    {
                        SpiderType.Spitter => new Color(1f, 0f, 0f),
                        SpiderType.Mother => new Color(0f, 1f, 0f),
                        _ => new Color(1f, 0.8f, 0.3f),
                    };
                    return Color.Lerp(orig, selectedColor, BlendInput.value);
                }
            }
            public bool Enabled => BlendInput.enabled;
            public int Bias => BlendInput.bias;
            public float Height => BlendInput.Height + (Enabled ? 156f : 0f);

            public string SaveKey => "Spider color";

            public void Create(float x, ref float y, List<UIelement> elements)
            {
                BlendInput.Create(x, ref y, elements);
                if (Enabled)
                {
                    OpColorPicker picker;
                    OpRect rect;

                    y -= 156f;
                    elements.Add(picker = new OpColorPicker(OpUtil.CosmeticBind(selectedColor), new Vector2(x + 34f, y)));
                    elements.Add(rect = new OpRect(new Vector2(x + picker.size.x + 6f + 34f, y), picker.size, 1f) { colorFill = BlendedColor });
                    picker._SwitchMode(pickerMode);

                    picker.OnValueUpdate += (_, _, _) =>
                    {
                        selectedColor = picker.valueColor;
                        rect.colorFill = BlendedColor;
                    };
                    picker.OnChange += () =>
                    {
                        pickerMode = picker._mode;
                    };
                }
            }

            public void FromSaveData(JObject data)
            {
                BlendInput.FromSaveData((JObject)data["blender"]!);
                selectedColor = new Color((float)data["r"]!, (float)data["g"]!, (float)data["b"]!);
            }

            public JObject ToSaveData()
            {
                return new JObject()
                {
                    ["blender"] = BlendInput.ToSaveData(),
                    ["r"] = selectedColor.r, ["g"] = selectedColor.g, ["b"] = selectedColor.b,
                };
            }

            public IEnumerable<string> GetHistoryLines()
            {
                if (Enabled)
                {
                    yield return $"{BlendInput.name}: blend({BlendInput.value})" + (Bias != 1 ? $" (bias: {Bias})" : "");
                    yield return $"    rgb({selectedColor.r}, {selectedColor.g}, {selectedColor.b})";
                    yield return $"    Result: rgb({BlendedColor.r}, {BlendedColor.g}, {BlendedColor.b})";
                }
                yield break;
            }
        }
    }
}
