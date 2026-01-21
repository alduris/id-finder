using System.Diagnostics;
using System.IO;
using Menu;
using Menu.Remix.MixedUI;
using RWCustom;
using Steamworks;
using UnityEngine;

namespace FinderMod.Tabs
{
    internal class HelpTab : BaseTab
    {
        private static Texture2D? logoTex;

        public HelpTab(OptionInterface option) : base(option, "Help") { }
        private const string TEXT_INTRO = "Welcome to ID Finder! This mod can help you find the id of any supported creature or object so long as you know " +
            "what properties you're looking for. It can also tell you values of properties for specific ids. This tab contains detailed instructions on the " +
            "operation of ID Finder. If you ever find any issues, please report them on the GitHub! Click the REPORT ISSUES button above to take you to the " +
            "issues page.";

        private const string TEXT_SEARCH_OVERVIEW = "The SEARCH tab is where searches take place. To create a search, choose an item from the dropdown at the " +
            "top of the tab, then press the ADD button. This adds it to the search list, which allows you to tweak the various settings. Multiple search items " +
            "can be added to a single search, and they can even be combined if desired via the green + button. Combining search items makes it so they are " +
            "combined in the actual search process; otherwise, each search item will search for its own id. Search items can be removed via the red \xd7 button.";
        private const string TEXT_SEARCH_INPUTS = "Search items have a list of inputs. Inputs have a checkbox, which allows you to toggle accounting for that " +
            "property while searching; the actual input, which will only appear while the checkbox is checked; and usually a bias input on the right side, " +
            "although not all inputs have this. More on bias later. In addition to the inputs for individual search items, there are also inputs for the " +
            "overall search: the id range to cover, how many results to return, the number of CPU (or GPU) threads to use, and whether to use a GPU search (if " +
            "supported). Most of these are self-explanatory, but a useful note about the id range inputs is that it always runs right from the From input." +
            "This allows you to take advantage of integer wrapping, and also means you can search every single id by making the From input one more than the " +
            "To input (like entering the range as From = 1 and To = 0).";
        private const string TEXT_SEARCH_ACTUAL = "When you've set up your inputs, click the blue RUN button below the search box and wait. ID Finder will " +
            "tell you how far through the search it is as a percentage as well as give you an estimation of how long the search will take. The estimation gets " +
            "more accurate over time, and the search itself may slow down if you do stuff on other windows, although you are still welcome to (id searches can " +
            "take hours, after all).";
        private const string TEXT_SEARCH_RESULTS = "When the search is complete, ID Finder will list out the closest ids it could find to your search items, as well " +
            "as the option to copy the search results for easier sharing and an option to save the search to your History. ID Finder returns results based on " +
            "distance. But what is distance?";
        private const string TEXT_SEARCH_DISTANCE = "Distance is ID Finder's measure of how far away a particular id is from your search item. Lower is better, and " +
            "an ideal search result has a distance of less than one. A distance of 0 means a perfect match. But how does it measure it? Distance measures the " +
            "difference between each search input in your search item and the corresponding property in the id, and each input can give a maximum distance of " +
            "1. However, there is a way to influence particular inputs: bias. Bias multiplies the distance of its corresponding input, thus giving it greater " +
            "weight and increasing its maximum possible distance.";

        private const string TEXT_VALUES_OVERVIEW = "The VALUES tab is where you can look at the actual properties of specific ids. This is useful if you have " +
            "a reference image of the specific id, so you can compare how it looks to the values of the actual searchable properties. Its operation is simple: " +
            "select which search to use as the basis and then input the id. The results will automatically appear as you type.";

        private const string TEXT_HISTORY_OVERVIEW = "The HISTORY tab allows easy access and management of past saved searches. Each entry in the history tab " +
            "keeps track of when the search took place, what the inputs were set to, and what the results of the search are, accessed by pressing the button " +
            "on the left side of the name. The name itself is editable, allowing for easier organization.";
        private const string TEXT_HISTORY_BUTTONS = "To the right of the name are three buttons: a copy button (with an arrow icon), a restore button (with a " +
            "reload icon), and a delete button. The copy button copies the search, which allows for easier sharing with others or for bug reports, while the " +
            "restore button sets up the search in the SEARCH tab with all previous inputs. Note that ID Finder version differences may change some inputs.";


        public override void Initialize()
        {
            float lineHeight = LabelTest.LineHeight(false);
            float bigLineHeight = LabelTest.LineHeight(true);
            float y = 600f;

            // Scrollbox
            var box = new OpScrollBox(this, 0f);

            // Logo
            LoadFile("idfinder-logo", ref logoTex);
            FAtlasElement logoElement = Futile.atlasManager.GetElementWithName("idfinder-logo");

            y -= logoElement.sourcePixelSize.y;
            OpImage logoImage;
            box.AddItems(logoImage = new OpImage(new Vector2(300f - logoElement.sourcePixelSize.x / 2, y), "idfinder-logo"));
            logoImage.sprite.shader = Custom.rainWorld.Shaders["MenuText"];

            // Info at the top
            y -= 10f + lineHeight;
            box.AddItems(new OpLabel(new Vector2(0f, y), new Vector2(600f, lineHeight), "Created by Alduris"));

            y -= 10f + 30f;
            OpSimpleButton workshopButton, issueButton;
            const float topButtonWidth = 120f;
            box.AddItems(
                workshopButton = new OpSimpleButton(new Vector2(300f - topButtonWidth - 5f, y), new Vector2(topButtonWidth, 30f), "WORKSHOP PAGE"),
                issueButton = new OpSimpleButton(new Vector2(300f + 5f, y), new Vector2(topButtonWidth, 30f), "REPORT ISSUE")
                );
            workshopButton.OnClick += WorkshopButton_OnClick;
            issueButton.OnClick += IssueButton_OnClick;

            // Horizontal line
            AddHorizontalRule();

            // Actual text
            AddHeading("HOW TO USE");
            AddLongLabel(10f, 580f, TEXT_INTRO);

            AddHeading("SEARCH TAB");
            AddLongLabel(10f, 580f, TEXT_SEARCH_OVERVIEW);
            AddLongLabel(10f, 580f, TEXT_SEARCH_INPUTS);
            AddLongLabel(10f, 580f, TEXT_SEARCH_ACTUAL);
            AddLongLabel(10f, 580f, TEXT_SEARCH_RESULTS);
            AddLongLabel(10f, 580f, TEXT_SEARCH_DISTANCE);

            AddHeading("VALUES TAB");
            AddLongLabel(10f, 580f, TEXT_VALUES_OVERVIEW);

            AddHeading("HISTORY TAB");
            AddLongLabel(10f, 580f, TEXT_HISTORY_OVERVIEW);
            AddLongLabel(10f, 580f, TEXT_HISTORY_BUTTONS);


            // Set scrollbox size
            box.SetContentSize(606f - y);
            box.ScrollToTop();
            return;

            // Helper functions
            void AddHorizontalRule()
            {
                y -= 10f + 2f;
                box.AddItems(new OpImage(new Vector2(10f, y), "pixel") { scale = new Vector2(580f, 2f), color = MenuColorEffect.rgbMediumGrey });
            }

            void AddHeading(string text)
            {
                y -= 10f;
                y -= bigLineHeight;
                box.AddItems(new OpLabel(new Vector2(0f, y), new Vector2(600f, bigLineHeight), text, bigText: true));
            }

            void AddLongLabel(float x, float width, string text)
            {
                y -= 10f;
                var label = new OpLabelLong(new Vector2(x, y), new Vector2(width, 600f), text, true, FLabelAlignment.Left);
                float height = label.GetDisplaySize().y;
                label.size = new Vector2(width, height);
                label.PosY -= height;
                box.AddItems(label);
                y -= height;
            }
        }

        public override void Update() { }

        private void LoadFile(string fileName, ref Texture2D? tex)
        {
            if (Futile.atlasManager.GetAtlasWithName(fileName) != null) return;
            string path = AssetManager.ResolveFilePath(Path.Combine("Illustrations", fileName + ".png"));
            tex = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            AssetManager.SafeWWWLoadTexture(ref tex, "file:///" + path, true, true);
            Futile.atlasManager.LoadAtlasFromTexture(fileName, tex, false);
        }

        private void WorkshopButton_OnClick(UIfocusable trigger)
        {
            if (!SteamManager.Initialized)
            {
                Process.Start(new ProcessStartInfo
                {
                    UseShellExecute = true,
                    FileName = "https://steamcommunity.com/sharedfiles/filedetails/?id=3040378054"
                });
            }
            else
            {
                SteamFriends.ActivateGameOverlayToWebPage("steam://url/CommunityFilePage/3040378054",
                    EActivateGameOverlayToWebPageMode.k_EActivateGameOverlayToWebPageMode_Default);
            }
        }

        private void IssueButton_OnClick(UIfocusable trigger)
        {
            Process.Start(new ProcessStartInfo
            {
                UseShellExecute = true,
                FileName = "https://github.com/alduris/id-finder/issues"
            });
        }
    }
}
