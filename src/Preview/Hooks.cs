using System;
using System.Collections.Generic;
using System.Linq;
using Menu;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using UnityEngine;

namespace FinderMod.Preview
{
    internal static class Hooks
    {
        public static void Apply()
        {
            try
            {
                // Process hooks
                On.ProcessManager.Update += ProcessManager_Update;

                // Camera hooks
                IL.RoomCamera.DrawUpdate += RoomCamera_DrawUpdate;

                // Don't show level in menu
                On.MultiplayerUnlocks.IsLevelUnlocked += MultiplayerUnlocks_IsLevelUnlocked;
            }
            catch (Exception ex)
            {
                Plugin.logger.LogError(ex);
            }
        }

        private static void ProcessManager_Update(On.ProcessManager.orig_Update orig, ProcessManager self, float deltaTime)
        {
            orig(self, deltaTime);
            if (PreviewManager.Initialized)
            {
                if ((self.currentMainLoop?.ID) == ProcessManager.ProcessID.ModdingMenu)
                {
                    PreviewManager.Update(deltaTime);
                }
                else
                {
                    PreviewManager.Uninitialize();
                }
            }
        }

        private static void RoomCamera_DrawUpdate(ILContext il)
        {
            // Replace camera position with ours
            var c = new ILCursor(il);
            c.GotoNext(x => x.MatchLdsfld<RoomCamera>(nameof(RoomCamera.doubleZoomMode)));
            c.GotoNext(MoveType.AfterLabel, x => x.MatchStloc(out _));

            c.Emit(OpCodes.Ldarg_0);
            c.Emit(OpCodes.Ldarg_1);
            c.EmitDelegate((Vector2 origValue, RoomCamera self, float timeStacker) =>
            {
                if (self is PreviewCamera preview)
                {
                    return preview.GetActualPosition(timeStacker);
                }
                return origValue;
            });
        }

        private static bool MultiplayerUnlocks_IsLevelUnlocked(On.MultiplayerUnlocks.orig_IsLevelUnlocked orig, MultiplayerUnlocks self, string levelName)
        {
            return levelName != "id_finder" && orig(self, levelName);
        }
    }
}
