using P2PTank3D.Models;

namespace P2PTank.Entities.P2PMessages
{
    public class EndGameMessage
    {
        public PlayerScore[] LeaderBoard { get; set; }
    }
}