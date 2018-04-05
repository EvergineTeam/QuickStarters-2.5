using P2PTank3D.Models;
using System;

namespace P2PTank.Entities.P2PMessages
{
    public class EndGameMessage
    {
        public PlayerScore[] LeaderBoard { get; set; }

        public TimeSpan Time { get; set; }
    }
}