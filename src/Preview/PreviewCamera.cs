using UnityEngine;

namespace FinderMod.Preview
{
    internal class PreviewCamera : RoomCamera
    {
        private Vector2 lastGoodPosition = Vector2.zero;
        private PreviewGame Game => (game as PreviewGame)!;

        public PreviewCamera(RainWorldGame game, int cameraNumber) : base(game, cameraNumber)
        {
            levelGraphic.RemoveFromContainer();
            backgroundGraphic.RemoveFromContainer();
        }

        public Vector2 GetActualPosition(float timeStacker)
        {
            if (Game.focusCreature?.realizedCreature?.room != null)
            {
                Vector2 creaturePos = AvgChunkPos(Game.focusCreature.realizedCreature.bodyChunks, timeStacker);
                Vector2 camPos = CamPos(currentCameraPosition);

                Vector2 position = PreviewManager.MiddleOfWindow - (creaturePos - camPos) - new Vector2(0.02f, 0.02f);

                lastGoodPosition = position;
                return position;
            }
            return lastGoodPosition;
        }

        private Vector2 AvgChunkPos(BodyChunk[] bodyChunks, float timeStacker)
        {
            Vector2 sum = Vector2.zero;
            int counter = 0;
            foreach (BodyChunk chunk in bodyChunks)
            {
                if (chunk != null)
                {
                    sum += Vector2.Lerp(chunk.lastPos, chunk.pos, timeStacker);
                    counter++;
                }
            }
            return counter != 0 ? sum / counter : Vector2.zero;
        }
    }
}
