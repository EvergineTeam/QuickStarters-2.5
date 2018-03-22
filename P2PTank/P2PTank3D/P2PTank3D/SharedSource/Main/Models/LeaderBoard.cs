using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text;
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

        public void Clear()
        {
            var childs = this.Owner.ChildEntities;

            foreach (var child in childs)
            {
                this.Owner.RemoveChild(child);
            }
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

            Debug.WriteLine(this.ToString());

            return playerScore;
        }

        public void Score(string playerID)
        {
            PlayerScoreComponent playerScore = null;
            this.Board.TryGetValue(playerID, out playerScore);
            playerScore?.Loose();
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

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine("-- LeaderBoard Start --");
            foreach (var tuple in this.Board)
            {
                sb.Append(tuple.Value.PlayerID);
                sb.Append(" - ");
                sb.Append(tuple.Value.Color.R);
                sb.Append(",");
                sb.Append(tuple.Value.Color.G);
                sb.Append(",");
                sb.Append(tuple.Value.Color.B);
                sb.AppendLine();
            }
            sb.AppendLine("-- LeaderBoard End --");

            return sb.ToString();
        }

        public PlayerScore[] GetGamePlayScore()
        {
            PlayerScore[] gamePlayScore = new PlayerScore[Board.Count];
            var enumerator = Board.Keys.GetEnumerator();
            enumerator.MoveNext();

            for (int i = 0; i < Board.Count; i++)
            {
                var score = Board[enumerator.Current];

                gamePlayScore[i] = new PlayerScore
                {
                    PlayerID = score.PlayerID,
                    Color = score.Color,
                    Deads = score.Deads,
                    Kills = score.Kills
                };

                enumerator.MoveNext();
            }

            return gamePlayScore;
        }
    }
}