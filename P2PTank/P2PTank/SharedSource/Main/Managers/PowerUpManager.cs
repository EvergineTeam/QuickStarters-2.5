﻿using P2PTank.Entities.P2PMessages;
using P2PTank.Managers.P2PMessages;
using P2PTank.Scenes;
using System;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;

namespace P2PTank.Managers
{
    public class PowerUpManager : Behavior
    {
        private const int MinimumSpawnTime = 20;
        private const int MaximumSpawnTime = 40;

        private P2PManager peerManager;
        private GamePlayManager gamePlayManager;
        private TimeSpan currentSpawnTime;

        public PowerUpManager(P2PManager peerManager)
        {
            this.peerManager = peerManager;
        }

        protected override void Update(TimeSpan gameTime)
        {
            this.currentSpawnTime = this.currentSpawnTime - gameTime;

            if(this.currentSpawnTime <= TimeSpan.Zero)
            {
                this.InitPowerUp();
                this.SendCreatePowerUpMessage();
            }
        }

        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            this.gamePlayManager = this.Owner.Scene.EntityManager.FindComponentFromEntityPath<GamePlayManager>(GameConstants.ManagerEntityPath);
        }

        public void InitPowerUp()
        {
            this.currentSpawnTime = TimeSpan.FromSeconds(WaveServices.Random.Next(MinimumSpawnTime, MaximumSpawnTime));
        }

        public async void SendCreatePowerUpMessage()
        {
            // New power up identifier
            var powerUpId = Guid.NewGuid().ToString();

            // Get random power up type
            var powerUpType = WaveServices.Random.Next(0, Enum.GetNames(typeof(PowerUpType)).Length + 1);

            // Get a random spawn point to initialize the player
            var spawnIndex = WaveServices.Random.Next(0, 4);
            var spawnPosition = this.GetSpawnPoint(spawnIndex);

            var createPowerUpMessage = new CreatePowerUpMessage()
            {
                PowerUpId = powerUpId, 
                PowerUpType = (PowerUpType)powerUpType,
                SpawnPosition = spawnPosition
            };

            await this.peerManager.SendBroadcastAsync(this.peerManager.CreateMessage(P2PMessageType.CreatePowerUp, createPowerUpMessage));

            var removePowerUpMessage = new RemovePowerUpMessage();

            await this.peerManager.SendBroadcastAsync(this.peerManager.CreateMessage(P2PMessageType.RemovePowerUp, removePowerUpMessage));
        }

        public async void SendDestroyPowerUpMessage(string powerUpId)
        {
            var message = new DestroyPowerUpMessage()
            {
                 PowerUpId = powerUpId
            };

            await this.peerManager.SendBroadcastAsync(this.peerManager.CreateMessage(P2PMessageType.DestroyPowerUp, message));
        }

        private Vector2 GetSpawnPoint(int index)
        {
            Vector2 res = Vector2.Zero;
            var entity = this.EntityManager.Find(string.Format(GameConstants.SpawnPointPathFormat, index));

            if (entity != null)
            {
                res = entity.FindComponent<Transform2D>().LocalPosition;
            }

            return res;
        }
    }
}