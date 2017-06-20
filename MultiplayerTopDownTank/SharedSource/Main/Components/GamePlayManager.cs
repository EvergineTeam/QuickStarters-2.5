using MultiplayerTopDownTank.Behaviors;
using System;
using WaveEngine.Framework;
using System.Runtime.Serialization;
using MultiplayerTopDownTank.Managers;
using WaveEngine.Framework.Services;

namespace MultiplayerTopDownTank.Components
{
    [DataContract]
    public class GamePlayManager : Behavior
    {
        private TankBehavior player;
        private NavigationManager navigationManager;

        protected override void Initialize()
        {
            base.Initialize();

            this.player = this.EntityManager.Find(GameConstants.Player).FindComponent<TankBehavior>();
            this.navigationManager = WaveServices.GetService<NavigationManager>();
        }

        protected override void Update(TimeSpan gameTime)
        {
        }
    }
}