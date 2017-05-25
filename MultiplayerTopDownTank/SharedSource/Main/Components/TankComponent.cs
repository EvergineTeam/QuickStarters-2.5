using MultiplayerTopDownTank.Behaviors;
using MultiplayerTopDownTank.Managers;
using System.Runtime.Serialization;
using WaveEngine.Framework;
using WaveEngine.Framework.Services;
using WaveEngine.Networking;

namespace MultiplayerTopDownTank.Components
{
    [DataContract]
    public class TankComponent : Component
    {
        private const int BulletDamage = 25;

        private int life;

        private NetworkService networkService;

        public bool IsLocal { get; set; }

        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            this.networkService = WaveServices.GetService<NetworkService>();
        }

        protected override void DefaultValues()
        {
            base.DefaultValues();

            this.IsLocal = false;
            this.life = 100;
        }

        public void PrepareTank()
        {
            var entityPath = this.Owner.EntityPath;
            var cameraBehavior = this.RenderManager.ActiveCamera2D.Owner.FindComponent<CameraBehavior>(false);

            if (cameraBehavior != null)
            {
                cameraBehavior.SetTarget(entityPath);
            }
        }

        public void Damage()
        {
            this.life = this.life - BulletDamage;

            if (this.life <= 0)
            {
                this.Destroy();
            }
        }

        private void Destroy()
        {
            var gameScene = this.Owner.Scene as GameScene;

            if (gameScene != null)
            {
                var tank = this.Owner;
                gameScene.DestroyTank(tank);
            }

            if (this.IsLocal)
            {
                // TODO: 
            }
        }
    }
}