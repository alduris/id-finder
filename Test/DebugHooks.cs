using System;
using BepInEx.Logging;
using FinderMod.Test;
using LizardCosmetics;
using RWCustom;
using UnityEngine;

namespace FinderMod
{
    internal partial class FinderPlugin
    {
        private readonly int SPAWN_START_ID = 6307;
        private readonly int SPAWN_QUANTITY = 20;
        private readonly CreatureTemplate.Type SPAWN_TYPE = MoreSlugcats.MoreSlugcatsEnums.CreatureTemplateType.TrainLizard;

        public void ApplyDebugHooks()
        {
            // Debug thing for ids and stuff
            On.RainWorldGame.Update += RainWorldGame_Update;

            // Lizard things
            On.LizardGraphics.ctor += LizardGraphics_ctor;
            On.LizardGraphics.AddCosmetic += LizardGraphics_AddCosmetic;
            try
            {
                LizardTests.TestAllLizards();
            }
            catch (Exception ex)
            {
                Logger.LogDebug(ex.Message);
            }
        }

        private void RainWorldGame_Update(On.RainWorldGame.orig_Update orig, RainWorldGame self)
        {
            orig(self);
            if(self.devToolsActive && self.IsArenaSession && Input.GetKeyDown(KeyCode.Backslash))
            {
                Room room = self.GetArenaGameSession.room;
                World world = room.world;
                WorldCoordinate mousePos = self.GetArenaGameSession.room.GetWorldCoordinate(Futile.mousePosition);
                for(int i = SPAWN_START_ID; i < SPAWN_START_ID + SPAWN_QUANTITY; i++)
                {
                    CreatureTemplate template = StaticWorld.GetCreatureTemplate(SPAWN_TYPE);
                    var ac = new AbstractCreature(world, template, null, mousePos, new EntityID(-1, i));
                    room.abstractRoom.AddEntity(ac);
                    ac.RealizeInRoom();
                }
            }
        }

        private void LizardGraphics_ctor(On.LizardGraphics.orig_ctor orig, LizardGraphics self, PhysicalObject ow)
        {
            orig(self, ow);
            if (self.lizard.abstractCreature.creatureTemplate.type == CreatureTemplate.Type.Salamander)
            {
                Logger.LogDebug(self.lizard.abstractCreature.ID.RandomSeed + ": " + (self.blackSalamander ? "Dark" : "Light"));
            }
            else if (self.lizard.abstractCreature.creatureTemplate.type == MoreSlugcats.MoreSlugcatsEnums.CreatureTemplateType.SpitLizard)
            {
                Vector3 effect = Custom.RGB2HSL(self.lizard.effectColor);
                Vector3 ivar = Custom.RGB2HSL(self.ivarBodyColor);
                Logger.LogDebug($"{self.lizard.abstractCreature.ID.RandomSeed}: effect({effect.x}f, {effect.y}f, {effect.z}f), ivarbody({ivar.x}f, {ivar.y}f, {ivar.z}f)");
            }
        }

        private int LizardGraphics_AddCosmetic(On.LizardGraphics.orig_AddCosmetic orig, LizardGraphics self, int spriteIndex, LizardCosmetics.Template cosmetic)
        {
            int ret = orig(self, spriteIndex, cosmetic);
            string debugStr = self.lizard.abstractCreature.ID.RandomSeed + ": " + cosmetic.GetType().Name;
            if (cosmetic is Antennae) debugStr += $" (l: {(cosmetic as Antennae).length}, a: {(cosmetic as Antennae).alpha})";
            else if (cosmetic is Whiskers) debugStr += $" (num: {(cosmetic as Whiskers).amount})";
            else if (cosmetic is TailGeckoScales) debugStr += $" (r: {(cosmetic as TailGeckoScales).rows}, l: {(cosmetic as TailGeckoScales).lines})";
            Logger.LogDebug(debugStr);
            return ret;
        }
    }
}
