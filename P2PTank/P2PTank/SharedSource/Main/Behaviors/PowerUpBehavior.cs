using P2PTank.Managers;
using P2PTank.Scenes;
using System;
using WaveEngine.Framework;

namespace P2PTank.Behaviors
{
    public class PowerUpBehavior : Behavior
    {
        private const int LifeTime = 10;

        private PowerUpManager powerUpManager;
        private TimeSpan currentLifeTime;

        public PowerUpType PowerUpType { get; set; }

        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            this.powerUpManager = this.Owner.Scene.EntityManager.FindComponentFromEntityPath<PowerUpManager>(GameConstants.ManagerEntityPath);

            this.currentLifeTime = TimeSpan.FromSeconds(LifeTime);
        }

        protected override void Update(TimeSpan gameTime)
        {
            this.currentLifeTime = this.currentLifeTime - gameTime;

            if (this.currentLifeTime <= TimeSpan.Zero)
            {
                this.powerUpManager.SendDestroyPowerUpMessage(this.Owner.Name);
            }
        }
    }
}