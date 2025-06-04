using System.Collections.Generic;
using FinderMod.Inputs;
using UnityEngine;

namespace FinderMod.Search.Options
{
    internal class TestOption : Option
    {
        public TestOption()
        {
            elements = [
                new Label("This is a test option. It does nothing."),

                new Whitespace(),

                new Alternatable<Label>("Alternatable", false, new("This is option A."), new("This is option B.")),
                new BoolInput("Bool"),
                new ColorHSLInput("Color HSL"),
                new ColorLerpInput("Color lerp", Color.red, Color.yellow),
                new ColorRGBInput("Color RGB"),
                new EnumInput<TestEnum>("Enum", TestEnum.Apple),
                new FloatInput("Float"),
                new Group([new FloatInput("A"), new FloatInput("B"), new FloatInput("C")], "Group"),
                new HueInput("Hue"),
                new IntInput("Int", 1, 10),
                new IntManualInput("Int manual", 0),
                new MultiChoiceInput("Multichoice", ["One", "Two", "Three", "Four", "Five"]),
                new Switchable<Label>("Switchable", [
                    ("Option A", new("This is option A.")),
                    ("Option B", new("This is option B.")),
                    ("Option C", new("This is option C.")),
                    ("Option D", new("This is option D.")),
                    ("Option E", new("This is option E.")),
                ]),
                new Toggleable<Label>("Toggleable", true, new Label("This element is toggled on. That is why you can see this text!")),

                new Whitespace(),

                new Group([
                    new Label("Group nesting test"),
                    new Group([
                        new Label("Inner 1"),
                        new Group([
                            new Label("Inner 2"),
                            new Group([
                                new Label("Inner 3")
                            ], "inner3")
                        ], "inner2")
                    ], "inner1")
                ], "nesting"),
            ];
        }

        public override float Execute(XORShift128 Random)
        {
            return Random.Value;
        }

        protected override IEnumerable<string> GetValues(XORShift128 Random)
        {
            yield break;
        }

        private enum TestEnum
        {
            Apple,
            Banana,
            Carrot,
            Donut,
            Egg
        }
    }
}
