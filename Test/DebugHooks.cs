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
        private readonly int SPAWN_START_ID = UnityEngine.Random.Range(1000,10000);
        private readonly int SPAWN_QUANTITY = 20;
        private readonly CreatureTemplate.Type SPAWN_TYPE = CreatureTemplate.Type.Scavenger;

        public void ApplyDebugHooks()
        {
            // Debug thing for ids and stuff
            On.RainWorldGame.Update += RainWorldGame_Update;

            // Lizard things
            On.LizardGraphics.ctor += LizardGraphics_ctor;
            On.LizardGraphics.AddCosmetic += LizardGraphics_AddCosmetic;
            
            LizardTests.TestAllLizards();

            // Scav things
            On.ScavengerGraphics.ctor += ScavengerGraphics_ctor;
            On.ScavengerCosmetic.HardBackSpikes.ctor += HardBackSpikes_ctor;
            On.ScavengerCosmetic.WobblyBackTufts.ctor += WobblyBackTufts_ctor;

            ScavTests.TestAllScavs();
        }

        private void HardBackSpikes_ctor(On.ScavengerCosmetic.HardBackSpikes.orig_ctor orig, ScavengerCosmetic.HardBackSpikes self, ScavengerGraphics owner, int firstSprite)
        {
            orig(self, owner, firstSprite);
            Logger.LogDebug($"{owner.scavenger.abstractCreature.ID.number}: HardBackSpikes (p: {self.pattern}, n: {self.positions.Length}, t: {self.top}f, b: {self.bottom}f, s: {self.generalSize}f)");
        }

        private void WobblyBackTufts_ctor(On.ScavengerCosmetic.WobblyBackTufts.orig_ctor orig, ScavengerCosmetic.WobblyBackTufts self, ScavengerGraphics owner, int firstSprite)
        {
            orig(self, owner, firstSprite);
            Logger.LogDebug($"{owner.scavenger.abstractCreature.ID.number}: WobblyBackTufts (p: {self.pattern}, n: {self.positions.Length}, t: {self.top}f, b: {self.bottom}f, s: {self.generalSize}f)");
        }

        private void ScavengerGraphics_ctor(On.ScavengerGraphics.orig_ctor orig, ScavengerGraphics self, PhysicalObject ow)
        {
            orig(self, ow);
            int id = self.scavenger.abstractCreature.ID.number;
            Logger.LogDebug($"{id}: body color hsl({self.bodyColor.hue}f, {self.bodyColor.saturation}f, {self.bodyColor.lightness}f)");
            Logger.LogDebug($"{id}: head color hsl({self.headColor.hue}f, {self.headColor.saturation}f, {self.headColor.lightness}f)");
            Logger.LogDebug($"{id}: deco color hsl({self.decorationColor.hue}f, {self.decorationColor.saturation}f, {self.decorationColor.lightness}f)");
            Logger.LogDebug($"{id}: eye color hsl({self.eyeColor.hue}f, {self.eyeColor.saturation}f, {self.eyeColor.lightness}f)");
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
    }
}
