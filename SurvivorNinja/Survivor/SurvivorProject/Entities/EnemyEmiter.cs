#region Using Statements
using SurvivorProject.Behaviors;
using SurvivorProject.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
#endregion

namespace SurvivorProject.Entities
{
    public class EnemyEmiter : BaseDecorator
    {
        private int enemyMax = 15;
        private Enemy[] enemies;
        private int enemyIndex;

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this instance is active.
        /// </summary>
        public bool IsActive
        {
            get { return this.entity.IsActive; }
            set { this.entity.IsActive = value; }
        } 

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="EnemyEmiter" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public EnemyEmiter(string name)
        {
            this.entity = new Entity(name)
                            .AddComponent(new Transform2D())
                            .AddComponent(new EnemyEmiterBehavior(this));

            this.enemyIndex = 0;

            this.enemies = new Enemy[this.enemyMax];
            for (int i = 0; i < this.enemyMax; i++)
            {
                Enemy enemy = new Enemy();
                this.enemies[i] = enemy;
                this.entity.AddChild(enemy.Entity);
            }
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        public void Reset()
        {
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
    }
}
