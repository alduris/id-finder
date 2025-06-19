using System;
using System.Collections.Generic;
using FinderMod.Inputs;
using Menu.Remix.MixedUI;
using Newtonsoft.Json.Linq;
using RWCustom;
using UnityEngine;
using static FinderMod.Search.Util.LizardUtil;

namespace FinderMod.Search.Options
{
    internal class LizardColorsOption : Option
    {
        private readonly LizardInput typeInp;
        private readonly LizardColor colrInp;

        public LizardColorsOption() : base()
        {
            elements = [
                typeInp = new LizardInput(),
                colrInp = new LizardColor(typeInp)
            ];
        }

        private (float h, float s, float l) GetColor(XORShift128 Random, LizardType type)
        {
            float h, s, l;

            switch (type)
            {
                case LizardType.Pink:
                    h = WrappedRandomVariation(0.87f, 0.1f, 0.6f, Random);
                    s = 1f;
                    l = WrappedRandomVariation(0.5f, 0.15f, 0.1f, Random);
                    break;
                case LizardType.Green:
                    h = WrappedRandomVariation(0.32f, 0.1f, 0.6f, Random);
                    s = 1f;
                    l = WrappedRandomVariation(0.5f, 0.15f, 0.1f, Random);
                    break;
                case LizardType.Blue:
                    h = WrappedRandomVariation(0.57f, 0.08f, 0.6f, Random);
                    s = 1f;
                    l = WrappedRandomVariation(0.5f, 0.15f, 0.1f, Random);
                    break;
                case LizardType.Yellow:
                    h = WrappedRandomVariation(0.1f, 0.05f, 0.6f, Random);
                    s = 1f;
                    l = WrappedRandomVariation(0.5f, 0.15f, 0.1f, Random);
                    break;
                case LizardType.Salamander:
                    h = WrappedRandomVariation(0.9f, 0.15f, 0.6f, Random);
                    s = 1f;
                    l = WrappedRandomVariation(0.4f, 0.15f, 0.1f, Random);
                    break;
                case LizardType.Red:
                    h = WrappedRandomVariation(0.0025f, 0.02f, 0.6f, Random);
                    s = 1f;
                    l = WrappedRandomVariation(0.5f, 0.15f, 0.1f, Random);
                    break;
                case LizardType.Cyan:
                    h = WrappedRandomVariation(0.49f, 0.04f, 0.6f, Random);
                    s = 1f;
                    l = WrappedRandomVariation(0.5f, 0.15f, 0.1f, Random);
                    break;
                case LizardType.Blizzard:
                    var blizzardStandardColor = new Color(0.55f, 0.6f, 0.7f);
                    var blizzardRandomColor = Custom.HSL2RGB(WrappedRandomVariation(0.5f, 0.2f, 0.44f, Random), 1f, ClampedRandomVariation(0.7f, 0.15f, 0.85f, Random));
                    var rawBlizzardColor = Color.Lerp(blizzardStandardColor, blizzardRandomColor, 0.05f);
                    var blizzardHSL = Custom.RGB2HSL(rawBlizzardColor);
                    (h, s, l) = (blizzardHSL.x, blizzardHSL.y, blizzardHSL.z);
                    break;
                case LizardType.Basilisk:
                    var basiliskRandomColor = Custom.HSL2RGB(WrappedRandomVariation(0.11f, 0.06f, 0.44f, Random), 1f, ClampedRandomVariation(0.5f, 0.15f, 0.1f, Random));
                    var basiliskHSL = Custom.RGB2HSL(Color.Lerp(basiliskRandomColor, Color.black, 0.85f));
                    (h, s, l) = (basiliskHSL.x, basiliskHSL.y, basiliskHSL.z);
                    break;
                case LizardType.Indigo:
                    h = WrappedRandomVariation(0.7f, 0.08f, 0.6f, Random);
                    s = 1f;
                    l = ClampedRandomVariation(0.52f, 0.15f, 0.1f, Random);
                    break;
                default:
                    h = s = l = 0; break;
            }
            return (h, s, l);
        }

        public override float Execute(XORShift128 Random)
        {
            LizardType type = typeInp.value;
            var (h, s, l) = GetColor(Random, type);

            return WrapDistanceIf(h, colrInp.HueInput) + DistanceIf(s, colrInp.SatInput) + DistanceIf(l, colrInp.LightInput);
        }

        protected override IEnumerable<string> GetValues(XORShift128 Random)
        {
            List<LizardType> types = [LizardType.Pink, LizardType.Green, LizardType.Blue, LizardType.Yellow, LizardType.Salamander, LizardType.Red, LizardType.Cyan];
            if (ModManager.Watcher)
            {
                types.AddRange([LizardType.Blizzard, LizardType.Basilisk, LizardType.Indigo]);
            }
            var (x, y, z, w) = (Random.x, Random.y, Random.z, Random.w);

            foreach (var type in types)
            {
                Random.InitState(x, y, z, w);
                var (h, s, l) = GetColor(Random, type);
                yield return $"{type}: hsl({h}, {s}, {l})";
            }
        }

        /// <summary>
        /// Version of <see cref="EnumInput{T}"/> for <see cref="LizardType"/> that forces a menu update
        /// </summary>
        private class LizardInput : EnumInput<LizardType>
        {
            public LizardInput() : base("Lizard type", LizardType.Green)
            {
                forceEnabled = true;
                OnValueChanged += LizardInput_OnValueChanged;
                excludeOptions = [
                    LizardType.White,
                    LizardType.Black,
                    LizardType.Caramel,
                    LizardType.Zoop,
                    LizardType.Train,
                    LizardType.Eel,
                    LizardType.NONE
                ];
                if (!ModManager.Watcher)
                {
                    excludeOptions.AddRange([LizardType.Blizzard, LizardType.Basilisk, LizardType.Indigo]);
                }
            }

            private void LizardInput_OnValueChanged(Input<LizardType> input, LizardType value, LizardType oldValue)
            {
                if (value != oldValue) UpdateQueryBox();
            }

            protected override UIconfig GetElement(Vector2 pos)
            {
                var el = base.GetElement(pos);
                (el as OpResourceSelector)!.listHeight = (ushort)(Enum.GetNames(typeof(LizardType)).Length - excludeOptions.Count);
                return el;
            }
        }

        /// <summary>
        /// A very poor implementation of a switch statement
        /// </summary>
        /// <param name="lizInput"></param>
        private class LizardColor(LizardInput lizInput) : IElement, ISaveInHistory
        {
            private readonly LizardInput lizInput = lizInput;
            private readonly Dictionary<LizardType, ColorHSLInput> groups = new()
            {
                { LizardType.NONE, null! },
                { LizardType.Pink, new ColorHSLInput("Pink Lizard Color", true, 0.77f, 0.97f, false, 1f, 1f, true, 0.35f, 0.65f) },
                { LizardType.Green, new ColorHSLInput("Green Lizard Color", true, 0.22f, 0.42f, false, 1f, 1f, true, 0.35f, 0.65f) },
                { LizardType.Blue, new ColorHSLInput("Blue Lizard Color", true, 0.49f, 0.65f, false, 1f, 1f, true, 0.35f, 0.65f) },
                { LizardType.Yellow, new ColorHSLInput("Yellow Lizard Color", true, 0.05f, 0.15f, false, 1f, 1f, true, 0.35f, 0.65f) },
                { LizardType.White, null! },
                { LizardType.Black, null! },
                { LizardType.Salamander, new ColorHSLInput("Salamander Frill Color", true, 0.75f, 1.05f, false, 1f, 1f, true, 0.25f, 0.55f) { fixColors = true } },
                { LizardType.Red, new ColorHSLInput("Red Lizard Color", true, -0.0175f, 0.0225f, false, 1f, 1f, true, 0.35f, 0.65f) },
                { LizardType.Cyan, new ColorHSLInput("Cyan Lizard Color", true, 0.45f, 0.53f, false, 1f, 1f, true, 0.35f, 0.65f) },
                // { LizardType.Caramel, new ColorHSLInput("Caramel Lizard Color", true, 0.07f, 0.13f, false, 0.55f, 0.55f, true, 0.19f, 0.91f) },
                { LizardType.Caramel, null! }, // we have to skip this one because it's fucked up and evil and does its own color thing
                { LizardType.Zoop, null! },
                { LizardType.Train, null! },
                { LizardType.Eel, null! },
                { LizardType.Blizzard, new ColorHSLInput("Blizzard Lizard Color", true, 0.558f, 0.631f, true, 0.168f, 0.248f, true, 0.598f, 0.638f) },
                { LizardType.Basilisk, new ColorHSLInput("Basilisk Lizard Color", true, 0.05f, 0.17f, true, 0.538f, 1f, true, 0.0525f, 0.0975f) },
                { LizardType.Indigo, new ColorHSLInput("Indigo Lizard Color", true, 0.62f, 0.78f, false, 1f, 1f, true, 0.37f, 0.67f) },
            };

            public float Height => groups[lizInput.value]?.Height ?? LabelTest.LineHeight(false);
            public FloatInput? HueInput => groups[lizInput.value]?.HueInput;
            public FloatInput? SatInput => groups[lizInput.value]?.SatInput;
            public FloatInput? LightInput => groups[lizInput.value]?.LightInput;

            public void Create(float x, ref float y, List<UIelement> elements)
            {
                if (groups.TryGetValue(lizInput.value, out var input) && input != null)
                {
                    input.Create(x, ref y, elements);
                }
                else
                {
                    y -= LabelTest.LineHeight(false);
                    elements.Add(new OpLabel(x, y, "Color input not available for this lizard type!", false));
                }
            }

            // Save data
            public string SaveKey => "Lizard color";

            public JObject ToSaveData()
            {
                var data = new JObject();
                foreach (var kvp in groups)
                {
                    if (kvp.Value != null)
                    {
                        data[kvp.Key.ToString()] = kvp.Value.ToSaveData();
                    }
                }
                return data;
            }

            public void FromSaveData(JObject data)
            {
                foreach (var kvp in data)
                {
                    var liz = (LizardType)Enum.Parse(typeof(LizardType), kvp.Key);
                    if (groups.TryGetValue(liz, out var group))
                    {
                        group.FromSaveData((kvp.Value as JObject)!);
                    }
                }
            }

            public IEnumerable<string> GetHistoryLines()
            {
                var activeInp = groups[lizInput.value];
                foreach (var line in activeInp.GetHistoryLines())
                {
                    yield return line;
                }
            }
        }
    }
}
