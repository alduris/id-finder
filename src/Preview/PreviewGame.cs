using System;
using System.Collections.Generic;
using System.Linq;
using ArenaBehaviors;
using RWCustom;
using Random = UnityEngine.Random;

namespace FinderMod.Preview
{
    internal class PreviewGame : RainWorldGame
    {
        public AbstractCreature focusCreature = null!;

        public PreviewGame(ProcessManager manager) : base(manager)
        {
            startingRoom = "id_finder";
            shortcuts = new ShortcutHandler(this);
            globalRain = new GlobalRain(this);

            cameras = [new PreviewCamera(this, 0)];
            grafUpdateMenus = [];
            wasAnArtificerDream = false;
            RainWorld.lockGameTimer = true;
            ModManager.CoopAvailable = false;
            devToolsActive = false;

            manager.arenaSetup = new ArenaSetup(manager);
            var gameSetup = manager.arenaSetup.GetOrInitiateGameTypeSetup(ArenaSetup.GameTypeID.Sandbox);
            manager.arenaSitting = new ArenaSitting(gameSetup, new MultiplayerUnlocks(rainWorld.progression, ["id_finder"]));
            session = new SandboxGameSession(this);
            GetArenaGameSession.AddBehavior(new NoRain(GetArenaGameSession));

            overWorld = new OverWorld(this);
            pathfinderResourceDivider = new PathfinderResourceDivider(this);

            int roomIndex = 0;
            if (world.GetAbstractRoom(overWorld.FIRSTROOM) != null)
            {
                roomIndex = world.GetAbstractRoom(overWorld.FIRSTROOM).index;
            }
            if (!world.IsRoomInRegion(roomIndex))
            {
                roomIndex = world.region.firstRoomIndex;
            }
            world.ActivateRoom(roomIndex);
            for (int camIndex = 0; camIndex < cameras.Length; camIndex++)
            {
                cameras[camIndex].MoveCamera(world.activeRooms[0], 0);
            }
        }

        public override void Update()
        {
            base.Update();
            devToolsActive = false;
            var room = cameras[0].room;
            if (room != null)
            {
                room.roomRain = null;
                room.roomSettings.DangerType = RoomRain.DangerType.None;
            }
        }

        public void AddCreatureToRoom(CreatureTemplate.Type type, int id)
        {
            var room = cameras[0].room;
            if (room == null) return;

            // Remove old creature and any objects it might have spawned
            if (focusCreature != null)
            {
                focusCreature.realizedCreature?.Destroy();
                focusCreature.Destroy();
                focusCreature = null!;
            }

            foreach (var layer in room.physicalObjects)
            {
                for (int i = layer.Count - 1; i >= 0; i--)
                {
                    layer[i].Destroy();
                }
            }
            
            // Determine spawn point of new creature
            var template = StaticWorld.GetCreatureTemplate(type);
            SpawnMode spawnMode = SpawnMode.Ground;
            if (template.waterRelationship == CreatureTemplate.WaterRelationship.Amphibious
                || template.waterRelationship == CreatureTemplate.WaterRelationship.WaterOnly
                || template.TopAncestor().waterRelationship == CreatureTemplate.WaterRelationship.Amphibious
                || template.TopAncestor().waterRelationship == CreatureTemplate.WaterRelationship.WaterOnly)
            {
                spawnMode = SpawnMode.Water;
            }
            else if (template.TopAncestor().type == CreatureTemplate.Type.GarbageWorm)
            {
                spawnMode = SpawnMode.GarbageWorm;
            }
            else if (!template.AccessibilityResistance(AItile.Accessibility.Air).Allowed)
            {
                spawnMode = SpawnMode.Den;
            }
            else if (ModManager.DLCShared && template.TopAncestor().type == DLCSharedEnums.CreatureTemplateType.StowawayBug)
            {
                spawnMode = SpawnMode.Ceiling;
            }

            var coordinate = GoodSpawnPosition(spawnMode);

            // Spawn the creature
            var abstrCrit = new AbstractCreature(world, template, null, coordinate, new EntityID(-1, id));
            room.abstractRoom.AddEntity(abstrCrit);
        }

        public WorldCoordinate GoodSpawnPosition(SpawnMode mode)
        {
            var room = cameras[0].room;
            if (mode == SpawnMode.Den)
            {
                int randomDen = RandomFrom([.. room.abstractRoom.nodes.Where(x => x.type == AbstractRoomNode.Type.Den).Select((x, i) => i)]);
                return room.LocalCoordinateOfNode(randomDen);
            }
            else if (mode == SpawnMode.GarbageWorm)
            {
                int randomDen = RandomFrom([.. room.abstractRoom.nodes.Where(x => x.type == AbstractRoomNode.Type.GarbageHoles).Select((x, i) => i)]);
                return room.LocalCoordinateOfNode(randomDen);
            }

            while (true)
            {
                var tilePos = room.RandomTile();
                var tile = room.GetTile(tilePos);
                if (tile.Solid) continue;

                if (tile.AnyWater && mode == SpawnMode.Water)
                {
                    // Water
                    return room.GetWorldCoordinate(tilePos);
                }
                else
                {
                    // Search for a solid tile not in water
                    IntVector2 nextTile = tilePos;
                    do
                    {
                        tilePos = nextTile;
                        nextTile += new IntVector2(0, mode == SpawnMode.Ceiling ? 1 : -1);
                    }
                    while (!room.GetTile(nextTile).Solid);
                    if (!room.GetTile(nextTile).AnyWater)
                    {
                        return room.GetWorldCoordinate(tilePos);
                    }
                }
            }

            static int RandomFrom(params List<int> ints) => ints[Random.Range(0, ints.Count)];
        }
    }
}
