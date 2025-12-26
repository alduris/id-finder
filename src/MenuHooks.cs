using System;
using Menu;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using UnityEngine;

namespace FinderMod
{
    internal static class MenuHooks
    {
        private const string FinderButtonSignal = "IDFINDER";
        
        public static void Apply()
        {
            try
            {
                // For the actual id finder menu itself
                On.Menu.MainMenu.ctor += MainMenu_ctor;
                On.ProcessManager.PostSwitchMainProcess += ProcessManager_PostSwitchMainProcess;
                IL.Music.MusicPlayer.UpdateMusicContext += MusicPlayer_UpdateMusicContext;

                // Purely for scrollbox changes
                IL.Menu.Remix.MixedUI.OpScrollBox.ctor_Vector2_Vector2_float_bool_bool_bool +=
                    OpScrollBox_UnhardcodeWidth;
                IL.Menu.Remix.MixedUI.OpScrollBox.Change += OpScrollBox_UnhardcodeWidth;
                
                // Debug stuff
                On.Menu.Remix.MixedUI.OpComboBox._CloseList += (orig, self) =>
                {
                    orig(self);
                    Plugin.logger.LogDebug(Environment.StackTrace);
                };
            }
            catch (Exception e)
            {
                Plugin.logger.LogError(e);
            }
        }

        private static void OpScrollBox_UnhardcodeWidth(ILContext il)
        {
            // Un-hardcode the width cap for scroll boxes
            var c = new ILCursor(il);
            c.GotoNext(MoveType.After, x => x.MatchLdcR4(800f));
            c.EmitDelegate((float val) => Mathf.Max(val, 1400f));
        }

        private static void MusicPlayer_UpdateMusicContext(ILContext il)
        {
            // Make us use arena music context as well by creating the following condition:
            // if (currentProcess.ID == ProcessManager.ProcessID.MultiplayerMenu || currentProcess.ID == Plugin.FinderProcess)
            var c = new ILCursor(il);
            c.GotoNext(x => x.MatchLdsfld<ProcessManager.ProcessID>(nameof(ProcessManager.ProcessID.MultiplayerMenu)));
            c.GotoNext(MoveType.AfterLabel, x => x.MatchBrfalse(out _));

            c.Emit(OpCodes.Ldarg_1);
            c.EmitDelegate((bool origValue, MainLoopProcess currentProcess) =>
            {
                return origValue || currentProcess.ID == Plugin.FinderProcess;
            });
        }

        private static void ProcessManager_PostSwitchMainProcess(On.ProcessManager.orig_PostSwitchMainProcess orig, ProcessManager self, ProcessManager.ProcessID id)
        {
            if (id == Plugin.FinderProcess)
            {
                self.currentMainLoop = new FinderProcess(self);
            }
            orig(self, id);
        }

        private static void MainMenu_ctor(On.Menu.MainMenu.orig_ctor orig, MainMenu self, ProcessManager manager, bool showRegionSpecificBkg)
        {
            orig(self, manager, showRegionSpecificBkg);

            float buttonWidth = MainMenu.GetButtonWidth(self.CurrLang);
            var pos = new Vector2(683f - buttonWidth / 2f, 0f);
            var size = new Vector2(buttonWidth, 30f);
            var btn = new SimpleButton(self, self.pages[0], self.Translate("ID FINDER"), FinderButtonSignal, pos, size);
            self.AddMainMenuButton(btn, FinderButtonCallback, self.mainMenuButtons.Count - 1 - self.mainMenuButtons.FindIndex(x => x.signalText == "ARENA"));
            return;
            
            void FinderButtonCallback()
            {
                self.manager.RequestMainProcessSwitch(Plugin.FinderProcess);
                self.PlaySound(SoundID.MENU_Switch_Page_In);
            }
        }
    }
}