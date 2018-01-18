using P2PTank.Scenes;
using WaveEngine.Common.Math;

namespace P2PTank.Managers.P2PMessages
{
    public class CreatePowerUpMessage
    {
        public string PowerUpId { get; set; }
        public PowerUpType PowerUpType { get; set; }
        public Vector2 SpawnPosition { get; set; }
    }
}