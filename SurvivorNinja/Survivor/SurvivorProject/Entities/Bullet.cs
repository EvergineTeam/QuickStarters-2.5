#region Using Statements
using SurvivorProject.Behaviors;
using SurvivorProject.Commons;
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

namespace SurvivorProject.Entities
{
    public class Bullet : BaseDecorator
    {
        private Vector2 position;
        private Vector2 direction;

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
            }
        }

        /// <summary>
        /// Gets or sets the direction.
        /// </summary>
        public Vector2 Direction
        {
            get { return direction; }
            set 
            {
                direction = value;                
                this.entity.IsVisible = true;
            }
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Bullet" /> class.
        /// </summary>
        public Bullet()
        {
            this.entity = new Entity() { Tag = "bullet" }
                                .AddComponent(new Transform2D()
                                {
                                    Origin = Vector2.Center,
                                    X = 0,
                                    Y = 0,
                                    DrawOrder = 0.6f,
                                })
                                .AddComponent(new CircleCollider())
                                .AddComponent(new BulletBehavior(this))
                                .AddComponent(new Sprite(Directories.Textures + "bullet.wpk"))
                                .AddComponent(new SpriteRenderer(DefaultLayers.Alpha));
            this.direction = Vector2.Zero;
            this.position = Vector2.Zero;
            
            this.entity.IsVisible = false;
        }
    }
}
