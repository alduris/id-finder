using System;
using System.Collections.Generic;
using UnityEngine;

namespace FinderMod.Preview
{
    internal static class PreviewManager
    {
        private static ProcessManager processManager = null!;
        private static PreviewGame gameInstance = null!;
        private static Vector2 windowOffset, windowSize;
        public static bool Initialized { get; private set; }
        private static readonly HashSet<string> loggedExceptions = [];

        public static void Initialize(ProcessManager manager, Vector2 offset, Vector2 size)
        {
            if (Initialized) Uninitialize();
            Initialized = true;
            loggedExceptions.Clear();

            processManager = manager;
            try
            {
                gameInstance = new PreviewGame(processManager);
            }
            catch (Exception ex)
            {
                gameInstance = null!;
                Plugin.logger.LogError(ex);
            }
            windowOffset = offset;
            windowSize = size;
        }

        public static void Update(float dt)
        {
            try
            {
                gameInstance.RawUpdate(dt);
            }
            catch (Exception ex)
            {
                if (!loggedExceptions.Contains(ex.Message))
                {
                    Plugin.logger.LogError(ex);
                    loggedExceptions.Add(ex.Message);
                }
            }
        }

        public static void Uninitialize()
        {
            Initialized = false;
            gameInstance?.ShutDownProcess();
            gameInstance = null!;
        }

        public static PreviewGame GameInstance => gameInstance;

        public static Vector2 MiddleOfWindow => windowOffset + windowSize / 2;
    }
}
