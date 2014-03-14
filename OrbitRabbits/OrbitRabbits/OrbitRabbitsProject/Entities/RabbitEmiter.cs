#region File Description
//-----------------------------------------------------------------------------
// OrbitRabbits
//
// Quickstarter for Wave University Tour 2014.
// Author: Wave Engine Team
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using OrbitRabbitsProject.Entities.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Managers;
#endregion

namespace OrbitRabbitsProject.Entities
{
    public class RabbitEmiter : BaseDecorator
    {
        private List<Rabbit> rabbits;
        private RabbitEmiterBehavior rabbitEmiterBehavior;

        public event EventHandler<int> ScoreChanged
        {
            add { this.rabbitEmiterBehavior.scoreChanged += value; }
            remove { this.rabbitEmiterBehavior.scoreChanged -= value; }
        }

        #region Properties
        public bool Emit
        {
            get
            {
                return this.rabbitEmiterBehavior.IsActive;
            }

            set
            {
                this.rabbitEmiterBehavior.IsActive = value;
            }
        }

        public int NumberOfRabbits
        {
            get { return this.rabbits.Count; }
        } 
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="RabbitEmiter" /> class.
        /// </summary>
        public RabbitEmiter()
        {
            this.rabbits = new List<Rabbit>();

            this.entity = new Entity()
                            .AddComponent(new RabbitEmiterBehavior(this.rabbits));

            // Cached
            this.rabbitEmiterBehavior = this.entity.FindComponent<RabbitEmiterBehavior>();

            this.Emit = false;
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            foreach (Rabbit rabbit in this.rabbits)
            {
                this.entity.RemoveChild(rabbit.Name);
            }
            this.rabbits.Clear();
        }

        /// <summary>
        /// Adds the rabbit.
        /// </summary>
        public void AddRabbit()
        {
            this.rabbitEmiterBehavior.AddRabbit();
        }

        /// <summary>
        /// Applies the impuse to last.
        /// </summary>
        public void ApplyImpuseToLast()
        {
            this.rabbitEmiterBehavior.ApplyImpulse();
        }
    }
}
