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
            "supported). Most of these are self-explanatory, but a useful note about the id range inputs is that it always runs right from the From: input." +
            "This allows you to take advantage of integer wrapping, and also means you can search every single id by making the From: input one more than the " +
            "To: input (like entering the range as From = 1 and To = 0).";
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

        private const string TEXT_TIPS_INTRO = "This section documents several tips and tricks, as well as hopefully answering any remaining major questions " +
            "about searching for ids with ID Finder.";

        private const string TEXT_TIPS_RANGE = "The id search range (the From: and To: inputs) determines what id range ID Finder will search, searching in " +
            "the positive direction from the From: input and wrapping around on integer overflow. However, most searches won't need to search every single id; " +
            "in fact, most likely don't even need to search more than 0.00116% of the id range. This is because without the help of mods, every single creature " +
            "id you come across will likely be between 0 and 50000. The default range set for ID Finder is 0 to 100000 just in case, although you likely will " +
            "not need it.";

        private const string TEXT_TIPS_THREADS = "You may be wondering what an appropriate number of threads to set your search at is. It depends on your CPU " +
            "and how much you want to torture your poor computer's CPU. I recommend setting it to anywhere between half and one minus the number of cores in " +
            "your CPU (on Windows, you can find this number through Task Manager). The number of threads used essentially divides the amount of work into " +
            "groups, assigned to a thread, which operate simultaneously to find the results. These work best if they are able to use one core of your computer " +
            "each. Using more threads than there are cores on your computer has diminishing results and may even start to increase the time it takes compared " +
            "to a more optimal therad count. Regardless, the more threads you use, the higher CPU usage the game will have, which may affect other applications " +
            "running on your computer, so use this feature wisely.";

        private const string TEXT_TIPS_GPU = "GPU searching is a powerful tool that allows you to save lots of time while searching at the cost of performance. " +
            "Especially on searches across large id ranges, this can bring your search from several minutes or hours down to several seconds or minutes, but it " +
            "is only suitable for computers with several free gigabytes RAM and VRAM. Expect taxing memory performance, the potential for blue screening, and " +
            "the lack of an ability to see how much time is remaining (this is due to technical limitations).";
        private const string TEXT_TIPS_GPU2 = "Additionally, GPU searching is somewhat experimental and although I have tried to verify results, there may be " +
            "minor inaccuracies I haven't spotted yet or do not know how to fix. If you do find any discrepancies between GPU and CPU search results, please " +
            "report the issue on the GitHub using the REPORT ISSUES button listed above. Numbers being slightly different between CPU and GPU searches in the " +
            "very least significant digits are to be expected, and an issue does not need to be reported for them.";


        public override void Initialize()
        {
            float lineHeight = LabelTest.LineHeight(false);
            float bigLineHeight = LabelTest.LineHeight(true);
            float y = 590f;

            // Scrollbox
            var box = new OpScrollBox(this, 0f);

            // Logo
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
                issueButton = new OpSimpleButton(new Vector2(300f + 5f, y), new Vector2(topButtonWidth, 30f), "REPORT ISSUES")
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

            AddHorizontalRule();
            AddHeading("TIPS AND TRICKS");
            AddLongLabel(10f, 580f, TEXT_TIPS_INTRO);

            AddHeading("SEARCH RANGE");
            AddLongLabel(10f, 580f, TEXT_TIPS_RANGE);

            AddHeading("THREAD COUNT");
            AddLongLabel(10f, 580f, TEXT_TIPS_THREADS);

            AddHeading("GPU SEARCHING");
            AddLongLabel(10f, 580f, TEXT_TIPS_GPU);
            AddLongLabel(10f, 580f, TEXT_TIPS_GPU2);


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
