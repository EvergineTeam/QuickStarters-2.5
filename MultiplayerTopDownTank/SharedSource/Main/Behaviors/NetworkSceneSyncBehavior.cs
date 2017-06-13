using System;
using System.Collections.Generic;
using System.Text;
using WaveEngine.Framework;
using WaveEngine.Networking;

namespace MultiplayerTopDownTank.Behaviors
{
    public class NetworkSceneSyncBehavior : SceneBehavior
    {
        private List<Entity> toRemove = new List<Entity>();

        private NetworkManager networkManager;

        protected override void ResolveDependencies()
        {
        }

        public void AddEntityToRemove(Entity entity)
        {
            this.toRemove.Add(entity);
        }

        public void DamageTank(Entity tank)
        {
            var behavior = tank.FindComponent<TankBehavior>();
            if (behavior != null)
            {
                behavior.CurrentLive -= GameConstants.BulletDamage;

                if (behavior.CurrentLive <= 0)
                {
                    this.toRemove.Add(tank);
                    
                    // TODO: Add decal, explosion, smoke, etc...
                }
            }
        }

        protected override void Update(TimeSpan gameTime)
        {
            if (this.networkManager == null)
            {
                var gameScene = this.Scene as GameScene;

                if (gameScene != null)
                {
                    this.networkManager = gameScene.NetworkManager;
                }
            }

            while (toRemove.Count > 0)
            {
                this.networkManager.RemoveEntity(toRemove[0]);
                toRemove.RemoveAt(0);
            }

        }
    }
}
