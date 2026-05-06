using System;
using System.Diagnostics;
using Menu;
using Menu.Remix;
using Menu.Remix.MixedUI;
using RWCustom;
using UnityEngine;

namespace FinderMod.Tabs
{
    internal class ErrorTab : OpTab
    {
        private const string DESCRIPTION = "ID Finder unexpectedly encountered an issue! The details are below. " +
            "Report the issue and how you triggered it so it can be fixed in the future.";
        private Exception exception;

        public ErrorTab(OptionInterface owner, Exception ex) : base(owner, "")
        {
            exception = ex;
            float y = 580f;

            // Logo
            FAtlasElement logoElement = Futile.atlasManager.GetElementWithName("idfinder-logo");
            float logoSize = logoElement.sourcePixelSize.y;
            y -= logoSize;

            OpImage logoImage;
            AddItems(logoImage = new OpImage(new Vector2(300f - logoElement.sourcePixelSize.x / 2, y), "idfinder-logo"));
            logoImage.sprite.shader = Custom.rainWorld.Shaders["MenuText"];

            // Frowny faces
            float frownyY = logoImage.PosY + logoElement.sourcePixelSize.y / 2f - 15f;
            AddItems([
                new OpLabel(new Vector2(40f, frownyY), new Vector2(30f, 30f), ":(", FLabelAlignment.Center, true)
                {
                    verticalAlignment = OpLabel.LabelVAlignment.Center
                },
                new OpLabel(new Vector2(530f, frownyY), new Vector2(30f, 30f), ":(", FLabelAlignment.Center, true)
                {
                    verticalAlignment = OpLabel.LabelVAlignment.Center
                },
                ]);

            // Version text
            y -= 24f;
            string versionText = ModdingMenu.instance.Translate("Version") + ": " + owner.mod.version;
            AddItems(new OpLabel(new Vector2(0f, y), new Vector2(600f, 24f), versionText, FLabelAlignment.Center));

            // Uh oh spaghetti-os text
            y -= 10f;
            var descriptionLabel = new OpLabelLong(new Vector2(30f, y), new Vector2(540f, 0f), DESCRIPTION, true)
            {
                verticalAlignment = OpLabel.LabelVAlignment.Top,
                allowOverflow = true,
            };
            y -= descriptionLabel.GetDisplaySize().y;
            descriptionLabel.PosY -= descriptionLabel.GetDisplaySize().y;
            descriptionLabel.size = new Vector2(descriptionLabel.size.x, descriptionLabel.GetDisplaySize().y);
            AddItems(descriptionLabel);

            // Buttons
            y -= 40f;
            var copyButton = new OpSimpleButton(new Vector2(20f, y), new Vector2(180f, 30f), "COPY EXCEPTION");
            var reportButton = new OpSimpleButton(new Vector2(210f, y), new Vector2(180f, 30f), "REPORT ISSUE");
            var restartButton = new OpSimpleButton(new Vector2(400f, y), new Vector2(180f, 30f), "RESTART");

            copyButton.OnClick += CopyButton_OnClick;
            reportButton.OnClick += ReportButton_OnClick;
            restartButton.OnClick += RestartButton_OnClick;

            AddItems(copyButton, reportButton, restartButton);

            // Error text
            y -= 10f;
            var rect = new OpRect(new Vector2(20f, 20f), new Vector2(560f, y - 20f), 0.7f)
            {
                colorFill = OptionInterface.errorBlue
            };
            AddItems(rect);
            var exceptionText = new OpLabelLong(new Vector2(40f, 30f), new Vector2(rect.size.x - 40f, rect.size.y - 20f), ex.ToString(), true, FLabelAlignment.Left)
            {
                verticalAlignment = OpLabel.LabelVAlignment.Top,
                allowOverflow = true,
                color = MenuColorEffect.rgbWhite,
            };

            if (exceptionText.GetDisplaySize().y > rect.size.y - 20f)
            {
                var scroll = new OpScrollBox(rect.pos, rect.size, exceptionText.GetDisplaySize().y + 20f, false, false, true);
                AddItems(scroll);

                exceptionText.pos = new Vector2(10f, 10f);
                scroll.AddItems(exceptionText);
            }
            else
            {
                AddItems(exceptionText);
            }
        }

        private void CopyButton_OnClick(UIfocusable trigger)
        {
            UniClipboard.SetText(exception.ToString());
            ConfigContainer.instance.CfgMenu.ShowAlert(OptionalText.GetText(OptionalText.ID.ConfigContainer_AlertCopyCosmetic).Replace("<Text>", "exception"));
        }

        private void ReportButton_OnClick(UIfocusable trigger)
        {
            Process.Start(new ProcessStartInfo
            {
                UseShellExecute = true,
                FileName = "https://github.com/alduris/id-finder/issues"
            });
        }

        private void RestartButton_OnClick(UIfocusable trigger)
        {
            owner.Initialize();
            ConfigContainer._ChangeActiveTab(0);
            ConfigContainer.menuTab.tabCtrler.Change();
        }
    }
}
