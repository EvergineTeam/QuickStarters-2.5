#region Using Statements
using SurvivorProject.Commons;
using SurvivorProject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Services; 
#endregion

namespace SurvivorProject.Behaviors
{
    public class EnemyEmiterBehavior : Behavior
    {
        private EnemyEmiter emiter;
        private TimeSpan minCadence;
        private TimeSpan cadence;
        private TimeSpan time;

        private Vector2 upVector, center;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnemyEmiterBehavior" /> class.
        /// </summary>
        /// <param name="emiter">The emiter.</param>
        public EnemyEmiterBehavior(EnemyEmiter emiter)
        {
            this.emiter = emiter;
            this.cadence = TimeSpan.FromSeconds(2);
            this.minCadence = TimeSpan.FromSeconds(0.5f);
            this.time = this.cadence;
            
            this.center = new Vector2(WaveServices.ViewportManager.VirtualWidth / 2, WaveServices.ViewportManager.VirtualHeight / 2);
            this.upVector = this.center + Vector2.UnitY * 500;
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
            this.time -= gameTime;
            if (time <= TimeSpan.Zero)
            {
                float rotation = WaveServices.Random.Next(0,360);
                Vector2 newPosition = Utils.RotateVectorAroundPoint(this.upVector, this.center, rotation);
                this.emiter.AddEnemy(newPosition);

                this.cadence -= TimeSpan.FromMilliseconds(10);
                if (this.cadence <= this.minCadence)
                {
                    this.cadence = this.minCadence;
                }
                this.time = this.cadence;
            }
        }
    }
}
