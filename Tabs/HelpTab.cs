using Menu.Remix.MixedUI;

namespace FinderMod.Tabs
{
    internal class HelpTab : FinderTab
    {
        public HelpTab(OptionInterface option) : base(option, "Help") { }
        private static string HELP_TEXT = "Lorem ipsum dolor sit amet";

        public override void Initialize()
        {
            AddItems(
                new OpLabel(10f, 560f, "HOW TO USE", true),
                new OpLabelLong(new(10f, 530f), new(580f, 30f), HELP_TEXT, true) { verticalAlignment = OpLabel.LabelVAlignment.Top }
            );
        }

        public override void Update() { }
    }
}
