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
using WaveEngine.Framework.Services;
#endregion

namespace DeepSpaceProject.Managers
{
    public class BulletManager : BaseDecorator
    {
        private readonly int numBullets = 20;
        private readonly int numEnemyBullets = 100;
        private int bulletIndex;

        private int BulletIndex
        {
            get
            {
                bulletIndex = ++bulletIndex % numBullets;
                return bulletIndex;
            }
        }

        private int EnemyBulletIndex
        {
            get
            {
                bulletIndex = ++bulletIndex % numEnemyBullets;
                return bulletIndex + numBullets;
            }
        }

        public BulletManager(GameplayBehavior behavior)
            : base()
        {
            this.entity = new Entity("BulletManager");

            var bullets = new Entity[numBullets];

            for (int i = 0; i < numBullets; i++)
            {
                Entity bullet = CreateBullet(true);
                bullet.Enabled = false;
                this.entity.AddChild(bullet);

                bullets[i] = bullet;
            }

            var enemyBullets = new Entity[numEnemyBullets];

            for (int i = 0; i < numEnemyBullets; i++)
            {
                Entity bullet = CreateBullet(false);
                bullet.Enabled = false;
                this.entity.AddChild(bullet);

                enemyBullets[i] = bullet;
            }

            behavior.Bullets = bullets;
            behavior.EnemyBullets = enemyBullets;
        }

        public void ShootBullet(bool player, float initX, float initY, float velocityX, float velocityY)
        {
            Entity bullet = this.entity.ChildEntities.ElementAt(player ? BulletIndex : EnemyBulletIndex);
            var bulletTransform = bullet.FindComponent<Transform2D>();
            bulletTransform.X = initX;
            bulletTransform.Y = initY;

            var bulletBehavior = bullet.FindComponent<BulletBehavior>();
            bulletBehavior.SpeedX = velocityX;
            bulletBehavior.SpeedY = velocityY;

            bullet.Enabled = true;

            if (player)
            {
                switch (WaveServices.Random.Next(0, 3))
                {
                    case 0:
                        WaveServices.GetService<SoundManager>().PlaySound(SoundManager.SOUNDS.Blast0);
                        break;
                    case 1:
                        WaveServices.GetService<SoundManager>().PlaySound(SoundManager.SOUNDS.Blast1);
                        break;
                    case 2:
                        WaveServices.GetService<SoundManager>().PlaySound(SoundManager.SOUNDS.Blast2);
                        break;
                }
            }
        }

        private Entity CreateBullet(bool player)
        {
            return new Entity()
                .AddComponent(new Transform2D()
                {
                    Origin = Vector2.Center,
                    DrawOrder = 0.6f,
                })
                .AddComponent(new PerPixelCollider(player ? "Content/BulletCollider.wpk" : "Content/EnemyBulletCollider.wpk", 0.5f))
                .AddComponent(new Sprite(player ? "Content/Bullet.wpk" : "Content/EnemyBullet.wpk"))
                .AddComponent(new BulletBehavior())
                .AddComponent(new SpriteRenderer(DefaultLayers.Alpha));
        }
    }
}
