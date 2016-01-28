using SurvivorNinja.Commons;
using SurvivorNinja.Entities;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Services;

namespace SurvivorNinja.Components
{
    [DataContract(Namespace = "SurvivorNinja.Components")]
    public class EnemyEmitter : Behavior
    {
        private int enemyMax;
        private Enemy[] enemies;
        private int enemyIndex;

        private TimeSpan minCadence;
        private TimeSpan cadence;
        private TimeSpan time;

        private Vector2 upVector, center;

        protected override void DefaultValues()
        {
            base.DefaultValues();
            this.enemyMax = 15;
        }

        protected override void Initialize()
        {
            base.Initialize();

            this.enemyIndex = 0;
            this.cadence = TimeSpan.FromSeconds(2);
            this.minCadence = TimeSpan.FromSeconds(0.5f);
            this.time = this.cadence;

            var virtualScreenManager = this.Owner.Scene.VirtualScreenManager;                       
            this.center = new Vector2(virtualScreenManager.VirtualWidth / 2, virtualScreenManager.VirtualHeight / 2);
            
            this.upVector = this.center + Vector2.UnitY * 500;
        }

        protected override void Update(TimeSpan gameTime)
        {
            this.time -= gameTime;

            if(this.time < TimeSpan.Zero)
            {
                this.SpawnEnemy();

                this.cadence -= TimeSpan.FromMilliseconds(10);
                if (this.cadence <= this.minCadence)
                {
                    this.cadence = this.minCadence;
                }

                this.time = this.cadence;
            }
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        public void Reset()
        {
            if(this.enemies == null)
            {
                return;
            }

            for (int i = 0; i < this.enemies.Length; i++)
            {
                this.enemies[i].IsVisible = false;
            }
        }

        /// <summary>
        /// Adds the enemy.
        /// </summary>
        /// <param name="position">The position.</param>
        public void AddEnemy(Vector2 position)
        {
            Enemy enemy = this.enemies[this.enemyIndex];
            enemy.Position = position;

            this.enemyIndex = (this.enemyIndex + 1) % this.enemyMax;
        }

        /// <summary>
        /// Spawn a new enemy
        /// </summary>
        private void SpawnEnemy()
        {
            if(this.enemies == null)
            {
                this.InitEnemyPool();
            }

            float rotation = WaveServices.Random.Next(0, 360);
            Vector2 newPosition = Utils.RotateVectorAroundPoint(this.upVector, this.center, rotation);
            this.AddEnemy(newPosition);
        }

        /// <summary>
        /// Init enemy pool
        /// </summary>
        private void InitEnemyPool()
        {
            this.enemies = new Enemy[this.enemyMax];
            for (int i = 0; i < this.enemyMax; i++)
            {
                Enemy enemy = new Enemy();
                this.enemies[i] = enemy;
                this.Owner.AddChild(enemy.Entity);
            }
        }
    }
}
