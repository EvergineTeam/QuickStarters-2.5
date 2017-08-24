using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;

namespace P2PTank.Entities.P2PMessages
{
    public class CreatePlayerMessage
    {
        public string IpAddress { get; set; }
        public string PlayerId { get; set; }
        public Color PlayerColor { get; set; }
        public Vector2 SpawnPosition { get; set; }
    }
}