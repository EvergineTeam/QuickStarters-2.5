using P2PTank.Scenes;

namespace P2PTank.Managers.P2PMessages
{
    public class PowerUpMessage
    {
        public string PlayerId { get; set; }
        public PowerUpType PowerUpType { get; set; }
    }
}