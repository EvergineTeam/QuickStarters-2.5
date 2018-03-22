using Newtonsoft.Json;
using WaveEngine.Common.Graphics;

namespace P2PTank3D.Models
{
    public class PlayerScore
    {
        public string PlayerID { get; set; }
        public Color Color { get; set; }
        public int Kills { get; set; }
        public int Deads { get; set; }

        [JsonIgnore]
        public float Score { get; set; }
    }
}