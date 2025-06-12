using System;
using Menu.Remix.MixedUI;
using UnityEngine;

namespace FinderMod.Preview
{
    internal class OpGamePreview(Vector2 pos, Vector2 size) : OpRect(pos, size, 0.8f)
    {
        private bool initialized = false;
        public void Initialize(ProcessManager processManager)
        {
            if (initialized) return;
            initialized = true;

            try
            {
                PreviewManager.Initialize(processManager, ScreenPos, size);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                Plugin.logger.LogError(ex);
            }
        }

        public void UpdateColor(float f)
        {
            colorFill = Color.Lerp(Color.black, Color.white, f);
        }

        public void SpawnCreature(CreatureTemplate.Type type, int id)
        {
            PreviewManager.GameInstance.AddCreatureToRoom(type, id);
        }
    }
}
