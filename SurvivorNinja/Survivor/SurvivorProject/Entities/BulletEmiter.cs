#region Using Statements
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
    public class BulletEmiter : BaseDecorator
    {
        private int bulletMax = 50;
        private Bullet[] bullets;
        private int bulletIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="BulletEmiter" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public BulletEmiter(string name)
        {
            this.entity = new Entity(name)
                                .AddComponent(new Transform2D());

            this.bullets = new Bullet[this.bulletMax];
            this.bulletIndex = 0;

            for (int i = 0; i < bulletMax; i++)
            {
                Bullet bullet = new Bullet();
                this.bullets[i] = bullet;
                this.entity.AddChild(bullet.Entity);
            }
        }

        /// <summary>
        /// Shoots the specified position.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="direction">The direction.</param>
        public void shoot(Vector2 position, Vector2 direction)
        {
            Bullet bullet = this.bullets[this.bulletIndex];
            bullet.Position = position;
            bullet.Direction = direction;

            this.bulletIndex = (this.bulletIndex + 1) % this.bulletMax;
        }        
    }
}
