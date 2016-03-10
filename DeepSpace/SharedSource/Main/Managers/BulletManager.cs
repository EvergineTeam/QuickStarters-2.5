using DeepSpace.Components.Enemies;
using WaveEngine.Common;
using WaveEngine.Common.Math;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics2D;
using WaveEngine.Framework.Services;

namespace DeepSpace.Managers
{
    public class BulletManager : Service
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

        public Entity ShootBullet(bool player, float initX, float initY, float velocityX, float velocityY)
        {
            Entity bullet = this.CreateBullet(player);
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

            return bullet;
        }

        private Entity CreateBullet(bool player)
        {
            return new Entity()
                .AddComponent(new Transform2D()
                {
                    Origin = Vector2.Center,
                    DrawOrder = 0.6f,
                })
                .AddComponent(new PerPixelCollider2D(player ? WaveContent.Assets.BulletCollider_png : WaveContent.Assets.EnemyBulletCollider_png, 0.5f))
                .AddComponent(new Sprite(player ? WaveContent.Assets.Bullet_png : WaveContent.Assets.EnemyBullet_png))
                .AddComponent(new BulletBehavior())
                .AddComponent(new SpriteRenderer(DefaultLayers.Alpha));
        }

        protected override void Initialize()
        {
        }

        protected override void Terminate()
        {
        }
    }
}
