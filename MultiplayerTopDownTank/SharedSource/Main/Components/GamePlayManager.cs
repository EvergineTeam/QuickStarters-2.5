using MultiplayerTopDownTank.Behaviors;
using System;
using WaveEngine.Framework;

namespace MultiplayerTopDownTank.Components
{
    public class GamePlayManager : Behavior
    {
        private TankBehavior player;

        protected override void Initialize()
        {
            base.Initialize();

            this.player = this.EntityManager.Find(GameConstants.Player).FindComponent<TankBehavior>();
        }

        protected override void Update(TimeSpan gameTime)
        {
          
        }
    }
}