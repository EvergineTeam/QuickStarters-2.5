#region Using Statements
using BasketKing.Entities;
using BasketKing.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
#endregion

namespace BasketKing.Behaviors
{
    public class BallBehavior : Behavior
    {
        [RequiredComponent]
        public Transform2D transform;

        private Vector2 basketPoint = new Vector2(200,574);
        public bool basketScore = false;

        private ScoreboardPanel scoreboardPanel;

        /// <summary>
        /// Resolves the dependencies needed for this instance to work.
        /// </summary>
        protected override void ResolveDependencies()
        { 
            base.ResolveDependencies();

            this.scoreboardPanel = ((GamePlayScene)this.Owner.Scene).scoreboardPanel;
        }

        /// <summary>
        /// Allows this instance to execute custom logic during its <c>Update</c>.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        /// <remarks>
        /// This method will not be executed if the <see cref="T:WaveEngine.Framework.Component" />, or the <see cref="T:WaveEngine.Framework.Entity" />
        /// owning it are not <c>Active</c>.
        /// </remarks>
        protected override void Update(TimeSpan gameTime)
        {
            if (!this.basketScore)
            {
                Vector2 ballPosition = new Vector2(this.transform.X, this.transform.Y);

                float distance = Vector2.Distance(ballPosition, basketPoint);             
                if (distance < 50)
                {
                    this.basketScore = true;

                    this.scoreboardPanel.NumSuccesses++;
                    this.scoreboardPanel.InARow++;
                    this.scoreboardPanel.Scores += 1*this.scoreboardPanel.Multipler;
                    SoundsManager.Instance.PlaySound(SoundsManager.SOUNDS.Net);
                }

                if (ballPosition.Y > 700 && !this.basketScore)
                {
                    this.scoreboardPanel.InARow = 0;
                }
            }
        }
    }
}
