using System.Collections.Generic;
using FinderMod.Tabs;
using Menu;
using UnityEngine;
using UnityEngine.Assertions.Must;

namespace FinderMod
{
    internal class FinderProcess : Menu.Menu
    {
        private int _activeTab = 1; // first non-main tab

        private readonly FSprite _darkSprite;
        private readonly SimpleButton _backButton;
        private readonly TabPickerRibbon _ribbon;
        
        public int ActiveTab => _activeTab;
        
        public FinderProcess(ProcessManager manager) : base(manager, Plugin.FinderProcess)
        {
            // Basic setup
            pages.Add(new Page(this, null, "main", 0));
            
            // Pick a background :3
            MenuScene.SceneID backgroundSceneId = MenuScene.SceneID.Landscape_SU;
            if (false && ModManager.MMF)
            {
                backgroundSceneId = manager.rainWorld.options.SubBackground;
            }
            else
            {
                // Pick a random region to display
                List<string> regionList = Region.GetFullRegionOrder(null);
                for (int i = 0; i < 10; i++)
                {
                    backgroundSceneId = Region.GetRegionLandscapeScene(regionList[Random.Range(0, regionList.Count)]);
                    if (backgroundSceneId != MenuScene.SceneID.Empty)
                    {
                        break;
                    }
                }

                if (backgroundSceneId == MenuScene.SceneID.Empty)
                {
                    backgroundSceneId = MenuScene.SceneID.Landscape_SU;
                }
            }
            
            scene = new InteractiveMenuScene(this, pages[0], backgroundSceneId);
            pages[0].subObjects.Add(scene);
            
            // Also dark sprite
            _darkSprite = new FSprite("pixel", true)
            {
                color = new Color(0.01f, 0.01f, 0.01f),
                anchorX = 0f,
                anchorY = 0f,
                scaleX = 1368f,
                scaleY = 770f,
                x = -1f,
                y = -1f,
                alpha = 0.85f
            };
            pages[0].Container.AddChild(_darkSprite);
            
            // Position base calculations
            const float EDGE_PADDING = 20f;
            float buttonWidth = MainMenu.GetButtonWidth(CurrLang);
            var smallestScreenSize = new Vector2(1024f, 768f);
            var headerPos = new Vector2(1366f / 2f - smallestScreenSize.x / 2f + EDGE_PADDING, 768f - EDGE_PADDING - 30f);
            
            // Buttons
            _backButton = new SimpleButton(this, pages[0], Translate("BACK"), "BACK", headerPos,
                new Vector2(buttonWidth, 30f));
            pages[0].subObjects.Add(_backButton);

            selectedObject = _backButton;
            
            // The other pages
            pages.Add(new SearchTab(this, 1));
            pages.Add(new ValuesTab(this, 2));
            pages.Add(new HistoryTab(this, 3));
            
            // Selector
            _ribbon = new TabPickerRibbon(this, pages[0], new Vector2(_backButton.pos.x + _backButton.size.x + 20f, _backButton.pos.y));
            pages[0].subObjects.Add(_ribbon);
            MutualHorizontalButtonBind(_backButton, _ribbon.buttons[0]);
        }

        public override void Singal(MenuObject sender, string message)
        {
            base.Singal(sender, message);
            if (sender == _backButton)
            {
                PlaySound(SoundID.MENU_Switch_Page_Out);
                foreach (Page page in pages)
                {
                    if (page is BaseTab tab)
                    {
                        tab.ClearMemory();
                    }
                }
                manager.RequestMainProcessSwitch(ProcessManager.ProcessID.MainMenu);
            }
        }

        public override void Update()
        {
            base.Update();
            for (int i = 1; i < pages.Count; i++)
            {
                pages[i].pos = new Vector2(1366f * i - 1366f * ActiveTab, 0f);
            }
        }

        private class TabPickerRibbon : RectangularMenuObject
        {
            private FinderProcess Owner => (menu as FinderProcess)!;
            
            public readonly List<TabPickerButton> buttons;
            private readonly float _buttonWidth;
            private readonly RoundedRect _selectedRect;
            
            public TabPickerRibbon(FinderProcess menu, MenuObject owner, Vector2 pos) : base(menu, owner, pos, Vector2.zero)
            {
                _buttonWidth = MainMenu.GetButtonWidth(menu.CurrLang);
                
                // Rect
                var bgRect = new RoundedRect(menu, this, new Vector2(-5f, -5f), Vector2.zero, true)
                {
                    borderColor = MenuColor(MenuColors.MediumGrey)
                };
                subObjects.Add(bgRect);
                
                // The buttons
                buttons = [];
                int buttonCount = 0;
                int activeButton = 0;
                for (int i = 1; i < menu.pages.Count; i++)
                {
                    if (menu.pages[i] is BaseTab tab)
                    {
                        var button = new TabPickerButton(menu, this,
                            new Vector2((_buttonWidth + 10f) * buttonCount, 0f), new Vector2(_buttonWidth, 30f),
                            tab.name, i);
                        buttons.Add(button);
                        subObjects.Add(button);

                        if (Owner.ActiveTab == i)
                        {
                            activeButton = buttonCount;
                        }
                        
                        buttonCount++;
                    }
                }

                for (int i = 0; i < buttons.Count - 1; i++)
                {
                    menu.MutualHorizontalButtonBind(buttons[i], buttons[i + 1]);
                }

                size = new Vector2(_buttonWidth * buttonCount + 10f * (buttonCount - 1), 30f);
                lastSize = size;
                
                bgRect.size = size + Vector2.one * 10f;
                bgRect.lastSize = bgRect.size;
                
                // Selection rect
                _selectedRect = new RoundedRect(menu, this, new Vector2(activeButton * (_buttonWidth + 10f), 0f),
                    new Vector2(_buttonWidth, 30f), false);
                subObjects.Add(_selectedRect);
                
                // Update vertical button binds
                NotifyClick(Owner.ActiveTab);
            }

            private void NotifyClick(int i)
            {
                if (Owner.ActiveTab != i)
                {
                    // todo: better switching
                    Owner._activeTab = i;
                    _selectedRect.pos = new Vector2(buttons.FindIndex(x => x.pageControlling == i) * (_buttonWidth + 10f), 0f);
                }

                var defaultSelectable = (Owner.pages[i] as BaseTab)?.defaultSelectable as MenuObject;
                foreach (TabPickerButton button in buttons)
                {
                    menu.MutualVerticalButtonBind(defaultSelectable ?? button, button);
                }
            }

            public class TabPickerButton : ButtonTemplate
            {
                public readonly int pageControlling;
                private readonly RoundedRect _selectionRect;
                private readonly MenuLabel _menuLabel;

                private readonly HSLColor _labelColor = MenuColor(MenuColors.MediumGrey);
                
                private TabPickerRibbon MyRibbon => (owner as TabPickerRibbon)!;
                
                public TabPickerButton(Menu.Menu menu, TabPickerRibbon owner, Vector2 pos, Vector2 size, string text, int pageControlling) : base(menu, owner, pos, size)
                {
                    this.pageControlling = pageControlling;
                    page.selectables.Add(this);
                    
                    // Decorative part of it
                    subObjects.Add(_menuLabel = new MenuLabel(menu, this, text, Vector2.zero, size, false));
                    subObjects.Add(_selectionRect = new RoundedRect(menu, this, Vector2.zero, size, true) { fillAlpha = 0f });
                }

                public override void Update()
                {
                    base.Update();
                    //buttonBehav.Update();
                }

                public override void GrafUpdate(float timeStacker)
                {
                    base.GrafUpdate(timeStacker);
                    
                    // Update label
                    _menuLabel.label.color = InterpColor(timeStacker, _labelColor);
                    
                    // Update rect
                    float alpha = 0.15f + 0.15f * Mathf.Sin(Mathf.Lerp(buttonBehav.lastSin, buttonBehav.sin, timeStacker) / 30f * 3.1415927f * 2f);
                    alpha *= buttonBehav.sizeBump;
                    for (int i = 0; i < 9; i++)
                    {
                        _selectionRect.sprites[i].color = MenuRGB(MenuColors.White);
                        _selectionRect.sprites[i].alpha = alpha;
                    }
                    for (int i = 9; i < _selectionRect.sprites.Length; i++)
                    {
                        _selectionRect.sprites[i].isVisible = false;
                    }
                }

                public override void Clicked()
                {
                    MyRibbon.NotifyClick(pageControlling);
                    menu.PlaySound(SoundID.MENU_Button_Standard_Button_Pressed);
                }
            }
        }
    }
}