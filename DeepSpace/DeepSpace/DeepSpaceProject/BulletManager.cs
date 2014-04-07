#region Usings Statements
using DeepSpaceProject.Behaviors;
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

namespace DeepSpaceProject
{
    public class BulletManager : BaseDecorator
    {
        private readonly int numBullets = 20;
        private int bulletIndex;

        private int BulletIndex
        {
            get
            {
                bulletIndex = ++bulletIndex % numBullets;
                return bulletIndex;
            }
        }

        public BulletManager()
            : base()
        {
            this.entity = new Entity("BulletManager");

            for (int i = 0; i < numBullets; i++)
            {
                Entity bullet = CreateBullet();
                bullet.Enabled = false;
                this.entity.AddChild(bullet);
            }
        }

        public void ShootBullet(float initX, float initY, float velocityX, float velocityY)
        {
            Entity bullet = this.entity.ChildEntities.ElementAt(BulletIndex);
            var bulletTransform = bullet.FindComponent<Transform2D>();
            bulletTransform.X = initX;
            bulletTransform.Y = initY;

            var bulletBehavior = bullet.FindComponent<BulletBehavior>();
            bulletBehavior.SpeedX = velocityX;
            bulletBehavior.SpeedY = velocityY;

            bullet.Enabled = true;
        }

        private Entity CreateBullet()
        {
            return new Entity()
                .AddComponent(new Transform2D()
                {
                    Origin = Vector2.Center,
                    DrawOrder = 0.6f,
                })
                .AddComponent(new RectangleCollider())
                .AddComponent(new Sprite("Content/Bullet.wpk"))
                .AddComponent(new BulletBehavior())
                .AddComponent(new SpriteRenderer(DefaultLayers.Opaque));
        }
    }
}
