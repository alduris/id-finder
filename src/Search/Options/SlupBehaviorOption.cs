using System;
using System.Collections.Generic;
using FinderMod.Inputs;
using Menu.Remix.MixedUI;
using Menu.Remix.MixedUI.ValueTypes;
using Newtonsoft.Json.Linq;

namespace FinderMod.Search.Options
{
    internal class SlupBehaviorOption : Option
    {
        private readonly GroupWithEnable behaviorGroup, foodGroup;
        private readonly BoolInput doesWiggle, doesNap, doesPlayWithItems, doesLayNearParent;
        private readonly EnumInput<FoodLike>[] foodLikes;

        public SlupBehaviorOption()
        {
            // Initialize food like inputs
            foodLikes = new EnumInput<FoodLike>[SlupFoodOption.foodLength];
            for (int i = 0; i < foodLikes.Length; i++)
            {
                foodLikes[i] = new EnumInput<FoodLike>(SlupFoodOption.foodList[i], FoodLike.None) { enabled = false };
            }

            // Add the rest of the elements
            behaviorGroup = new GroupWithEnable([
                doesWiggle = new BoolInput("Wiggles when held", false) { enabled = false },
                doesNap = new BoolInput("Takes naps", false) { enabled = false },
                doesPlayWithItems = new BoolInput("Plays with items", false) { enabled = false },
                doesLayNearParent = new BoolInput("Lays near parent", false) { enabled = false, description = "Does not count if laying near when parent is dead" },
                ], "AI behaviors");
            foodGroup = new GroupWithEnable([.. foodLikes], "Food reactions");

            elements = [
                behaviorGroup,
                new Whitespace(4f),
                foodGroup,
                ];
        }

        private FoodLike DetermineFoodLike(float o) => o switch
        {
            < -0.85f => FoodLike.VeryNegative,
            < -0.4f  => FoodLike.Negative,
            > 0.85f  => FoodLike.VeryPositive,
            > 0.4f   => FoodLike.Positive,
            _ => FoodLike.None
        };

        public override float Execute(XORShift128 Random)
        {
            var (x, y, z, w) = (Random.x, Random.y, Random.z, Random.w);
            var p = new Personality(Random);

            float r = 0;

            if (behaviorGroup.enabled)
            {
                r += DistanceIf(p.nrg > 0.6f, doesWiggle);
                r += DistanceIf(p.nrg < 0.15f, doesNap);
                r += DistanceIf(p.nrg > 0.85f, doesPlayWithItems);
            }

            if (foodGroup.enabled)
            {
                Random.InitState(x, y, z, w);
                for (int j = 0; j < SlupFoodOption.foodLength; j++)
                {
                    float o = SlupFoodOption.GetFoodLike(Random, p, j);
                    if (foodLikes[j].enabled && foodLikes[j].value != DetermineFoodLike(o)) r += foodLikes[j].bias;
                }
            }

            return r;
        }

        protected override IEnumerable<string> GetValues(XORShift128 Random)
        {
            var (x, y, z, w) = (Random.x, Random.y, Random.z, Random.w);
            var p = new Personality(Random);

            yield return $"Wiggles when held: {(p.nrg > 0.6f ? "Yes" : "No")}";
            yield return $"Takes naps: {(p.nrg < 0.15f ? "Yes" : "No")}";
            yield return $"Plays with items: {(p.nrg > 0.85f ? "Yes" : "No")}";
            yield return $"Lays near parent: {(p.nrg < 0.35f ? "Yes" : "No")}";

            yield return null!;

            Random.InitState(x, y, z, w);
            for (int j = 0; j < SlupFoodOption.foodLength; j++)
            {
                float o = SlupFoodOption.GetFoodLike(Random, p, j);
                yield return $"Reaction to {SlupFoodOption.foodList[j].ToLowerInvariant()}: {DetermineFoodLike(o)}";
            }
        }

        private enum FoodLike
        {
            VeryPositive,
            Positive,
            None,
            Negative,
            VeryNegative
        }

        private class GroupWithEnable : IElement, ISaveInHistory
        {
            private readonly Group internalGroup;
            private readonly string label;
            public bool enabled;

            public List<IElement> children => internalGroup.children;

            public GroupWithEnable(List<IElement> elements, string name)
            {
                label = name;
                internalGroup = new Group(elements, "internal");
                enabled = true;
            }

            public float Height => enabled ? 30f + internalGroup.Height : 24f;

            public string SaveKey => label;

            public void Create(float x, ref float y, List<UIelement> elements)
            {
                y -= 24f;

                var cb = new OpCheckBox(OpUtil.CosmeticBind(enabled), new(x, y));
                cb.OnValueUpdate += ToggleEnable;
                elements.Add(cb);
                elements.Add(new OpLabel(x + 34f, y + 12f - LabelTest._lineHalfHeight, label) { verticalAlignment = OpLabel.LabelVAlignment.Bottom });

                if (enabled)
                {
                    y -= 6f;
                    internalGroup.Create(x, ref y, elements);
                }
            }

            private void ToggleEnable(UIconfig config, string value, string oldValue)
            {
                if (value != oldValue)
                {
                    enabled = (config as OpCheckBox)!.GetValueBool();
                    UpdateQueryBox();
                }
            }

            public IEnumerable<string> GetHistoryLines()
            {
                if (enabled)
                {
                    foreach (var line in internalGroup.GetHistoryLines())
                    {
                        yield return line;
                    }
                }
            }

            public void FromSaveData(JObject data)
            {
                enabled = (bool)data["enabled"]!;
                internalGroup.FromSaveData((JObject)data["internal"]!);
            }

            public JObject ToSaveData()
            {
                return new JObject()
                {
                    ["enabled"] = enabled,
                    ["internal"] = internalGroup.ToSaveData(),
                };
            }
        }
    }
}
