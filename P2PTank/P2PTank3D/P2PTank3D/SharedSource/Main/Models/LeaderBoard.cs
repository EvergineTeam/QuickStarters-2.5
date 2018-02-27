using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using WaveEngine.Common.Graphics;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;

namespace P2PTank3D.Models
{
    [DataContract]
    public class LeaderBoard : Component
    {
        public Dictionary<string, PlayerScoreComponent> Board { get; private set; } = new Dictionary<string, PlayerScoreComponent>();

        protected override void Initialize()
        {
            base.Initialize();
        }
        
        public PlayerScoreComponent AddOrUpdatePlayerIfNotExtist(string playerID, Color color)
        {
            PlayerScoreComponent playerScore = null;

            if (!this.Board.ContainsKey(playerID))
            {
                var entity = this.EntityManager.Instantiate(WaveContent.Assets.Prefabs.ScoreRowPrefab);
                entity.Name = Guid.NewGuid().ToString();
                playerScore = entity.FindComponent<PlayerScoreComponent>();
                playerScore.PlayerID = playerID;
                playerScore.Color = color;

                // position
                var transform = entity.FindComponent<Transform2D>();
                var position = transform.LocalPosition;
                position.Y = 100 * this.Board.Count;
                transform.LocalPosition = position;

                this.Owner.AddChild(entity);
                this.Board.Add(playerID, playerScore);
}
            else
            {
                playerScore = this.Board[playerID];
                playerScore.Color = color;
            }

            return playerScore;
        }

        public void Killed(string playerID)
        {
            PlayerScoreComponent playerScore = null;
            this.Board.TryGetValue(playerID, out playerScore);
            playerScore?.Loose();
        }

        public void Victory(string playerID)
        {
            PlayerScoreComponent playerScore = null;
            this.Board.TryGetValue(playerID, out playerScore);
            playerScore?.Victory();
        }
    }
}
