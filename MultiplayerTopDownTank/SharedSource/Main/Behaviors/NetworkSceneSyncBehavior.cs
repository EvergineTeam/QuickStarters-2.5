using System;
using System.Collections.Generic;
using System.Text;
using MultiplayerTopDownTank.Components;
using MultiplayerTopDownTank.Managers;
using MultiplayerTopDownTank.Messages;
using WaveEngine.Framework;
using WaveEngine.Framework.Services;
using WaveEngine.Networking;
using WaveEngine.Networking.Messages;

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

        private bool finishGame = false;

        public void DamageTank(Entity tank)
        {
            var tankComponent = tank.FindComponent<TankComponent>();
            if (tankComponent != null)
            {
                tankComponent.CurrentLive -= GameConstants.BulletDamage;

                if (tankComponent.CurrentLive <= 0)
                {
                    this.toRemove.Add(tank);
                    // TODO: Add decal, explosion, smoke, etc...
                    this.SendDieMessage(tankComponent.Name);
                }
            }
        }

        private void SendDieMessage(string name)
        {
            var networkService = WaveServices.GetService<NetworkService>();

            OutgoingMessage message = networkService.CreateClientMessage(MessageType.Data);
            message.Write(name);

            networkService.SendToServer(message, DeliveryMethod.ReliableUnordered);
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
