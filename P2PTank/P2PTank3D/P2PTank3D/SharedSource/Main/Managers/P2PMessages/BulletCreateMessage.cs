using WaveEngine.Common.Graphics;

namespace P2PTank.Entities.P2PMessages
{
    public class BulletCreateMessage
    {
        public string BulletID { get; set; }

        public string PlayerID { get; set; }

        public Color Color { get; set; }
    }
}
