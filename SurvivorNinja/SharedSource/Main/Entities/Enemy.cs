#region Using Statements
using SurvivorNinja.Behaviors;
using SurvivorNinja.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Math;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics2D;
#endregion

namespace SurvivorNinja.Entities
{
    public class Enemy : BaseDecorator
    {
        private Vector2 position;

        #region Properties

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        public Vector2 Position
        {
            get { return position; }
            set 
            {
                position = value;
                Transform2D transform = this.entity.FindComponent<Transform2D>();
                transform.X = position.X;
                transform.Y = position.Y;

                this.IsVisible = true;
            }
        } 

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Enemy" /> class.
        /// </summary>
        /// <param name="position">The position.</param>
        public Enemy()
        {
            this.entity = new Entity() { Tag = "enemy" }
                                .AddComponent(new Transform2D()
                                {
                                    Origin = Vector2.Center,
                                    X = -100,
                                    Y = -100,
                                    DrawOrder = 0.5f,
                                })
                                .AddComponent(new CircleCollider2D() { Radius = 0.3f})
                                .AddComponent(new EnemyBehavior())
                                .AddComponent(new Sprite(Directories.Textures + "enemy.png"))
                                .AddComponent(new SpriteRenderer(DefaultLayers.Alpha));

            this.IsVisible = false;
        }
    }
}
