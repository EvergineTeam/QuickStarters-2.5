using System;
using System.Runtime.Serialization;
using DeepSpace.Components.Gameplay;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Math;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics2D;

namespace DeepSpace.Components.Bullets
{
    [DataContract]
    public class BulletGeneratorBehavior : Behavior
    {
        private int numBullets;
        private int numEnemyBullets;
        private bool bulletsGenerated;
        private GameplayBehavior gamePlay;

        [DataMember]
        [RenderPropertyAsEntity(componentsFilter: new string[] { "DeepSpace.Components.Gameplay.GameplayBehavior" })]
        public string GamePlayEntityPath { get; set; }

        protected override void DefaultValues()
        {
            base.DefaultValues();
            this.numBullets = 20;
            this.numEnemyBullets = 100;
        }
        
        protected override void Initialize()
        {
            base.Initialize();

            var gamePlayEntity = this.EntityManager.Find(GamePlayEntityPath);
            if (gamePlayEntity != null)
            {
                this.gamePlay = this.EntityManager.Find(GamePlayEntityPath).FindComponent<GameplayBehavior>();
            }
        }

        protected override void Update(TimeSpan gameTime)
        {
            if(!this.bulletsGenerated)
            {
                this.GenerateBullets();
                this.bulletsGenerated = true;
            }
        }

        private void GenerateBullets()
        {
            var bullets = new Entity[numBullets];

            for (int i = 0; i < numBullets; i++)
            {
                Entity bullet = CreateBullet(true);
                bullet.Enabled = false;
                this.Owner.AddChild(bullet);

                bullets[i] = bullet;
            }

            var enemyBullets = new Entity[numEnemyBullets];

            for (int i = 0; i < numEnemyBullets; i++)
            {
                Entity bullet = CreateBullet(false);
                bullet.Enabled = false;
                this.Owner.AddChild(bullet);

                enemyBullets[i] = bullet;
            }

            this.gamePlay.Bullets = bullets;
            this.gamePlay.EnemyBullets = enemyBullets;
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
    }
}
