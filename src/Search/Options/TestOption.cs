using System.Collections.Generic;
using FinderMod.Inputs;

namespace FinderMod.Search.Options
{
    internal class TestOption : Option
    {
        public TestOption()
        {
            elements = [
                new Label("This is a test option. It does nothing."),
                new Whitespace(),
                new BoolInput("Bool"),
                new ColorHSLInput("Color HSL"),
                new ColorRGBInput("Color RGB"),
                new EnumInput<TestEnum>("Enum", TestEnum.Apple),
                new FloatInput("Float"),
                new Group([new FloatInput("A"), new FloatInput("B"), new FloatInput("C")], "Group"),
                new HueInput("Hue"),
                new IntInput("Int", 1, 10),
                new MultiChoiceInput("Multichoice", ["One", "Two", "Three", "Four", "Five"])
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
