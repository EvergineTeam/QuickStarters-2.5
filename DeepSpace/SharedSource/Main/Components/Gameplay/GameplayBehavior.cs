using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using DeepSpace.Components.Bullets;
using DeepSpace.Components.Enemies;
using DeepSpace.Components.Player;
using DeepSpace.Managers;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Components;
using WaveEngine.Components.Animation;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Components.Transitions;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Managers;
using WaveEngine.Framework.Physics2D;
using WaveEngine.Framework.Services;

namespace DeepSpace.Components.Gameplay
{
    public enum GameState
    {
        Intro,
        Game,
        GameOver
    }

    [DataContract]
    public class GameplayBehavior : Behavior
    {
        public event Action GameReset;

        private GameStorage gameStorage;

        private VirtualScreenManager virtualScreenManager;

        private GameState state;

        private PerPixelCollider2D playerCollider;

        private PerPixelCollider2D[] enemyColliders;

        private PerPixelCollider2D[] bulletColliders;

        private PerPixelCollider2D[] enemyBulletColliders;

        private Entity player;

        private Entity[] enemies;

        private Entity[] bullets;

        private Entity[] enemyBullets;

        private Entity[] explosions;

        private int score;

        public int Score
        {
            get
            {
                return this.score;
            }


            set
            {
                this.score = value;
                this.Scored(value);
            }
        }

        public GameState State
        {
            get
            {
                return this.state;
            }

            set
            {
                this.state = value;
            }
        }

        private const int NUMEXPLOSIONS = 5;

        private int explodeIndex;

        private int ExplodeIndex
        {
            get
            {
                explodeIndex = ++explodeIndex % NUMEXPLOSIONS;
                return explodeIndex;
            }
        }

        public Entity Player
        {
            get
            {
                return player;
            }

            set
            {
                this.player = value;
                this.playerCollider = this.player.FindComponent<PerPixelCollider2D>();
            }
        }

        public Entity[] Enemies
        {
            get
            {
                return enemies;
            }

            set
            {
                enemies = value;

                this.enemyColliders = new PerPixelCollider2D[value.Length];

                for (int i = 0; i < value.Length; i++)
                {
                    this.enemyColliders[i] = value[i].FindComponent<PerPixelCollider2D>();
                }
            }
        }

        public Entity[] Bullets
        {
            get
            {
                return bullets;
            }

            set
            {
                bullets = value;

                this.bulletColliders = new PerPixelCollider2D[value.Length];

                for (int i = 0; i < value.Length; i++)
                {
                    this.bulletColliders[i] = value[i].FindComponent<PerPixelCollider2D>();
                }
            }
        }

        public Entity[] EnemyBullets
        {
            get
            {
                return enemyBullets;
            }

            set
            {
                enemyBullets = value;

                this.enemyBulletColliders = new PerPixelCollider2D[value.Length];

                for (int i = 0; i < value.Length; i++)
                {
                    this.enemyBulletColliders[i] = value[i].FindComponent<PerPixelCollider2D>();
                }
            }
        }

        private int bulletIndex;

        private int BulletIndex
        {
            get
            {
                bulletIndex = ++bulletIndex % bullets.Length;
                return bulletIndex;
            }
        }

        private int enemyBulletIndex;

        private int EnemyBulletIndex
        {
            get
            {
                enemyBulletIndex = ++enemyBulletIndex % enemyBullets.Length;
                return enemyBulletIndex;
            }
        }

        public GameplayBehavior()
            : base()
        {
        }

        protected override void Initialize()
        {
            base.Initialize();

            this.virtualScreenManager = this.Owner.Scene.VirtualScreenManager;
        }

        protected override void ResolveDependencies()
        {
            this.gameStorage = Catalog.GetItem<GameStorage>();

            this.explosions = new Entity[NUMEXPLOSIONS];

            for (int i = 0; i < NUMEXPLOSIONS; i++)
            {
                this.explosions[i] = this.CreateExplosion();
            }
        }

        private Entity CreateExplosion()
        {
            var explode = new Entity()
                    .AddComponent(new Transform2D() { Origin = Vector2.Center, XScale = 2, YScale = 2 })
                    .AddComponent(new SpriteAtlas(WaveContent.Assets.ExplodeSprite_spritesheet))
                    .AddComponent(new SpriteAtlasRenderer() { LayerType = DefaultLayers.Additive })
                    .AddComponent(new Animation2D() { CurrentAnimation = "explosion", PlayAutomatically = false });

            explode.Enabled = false;

            var anim2D = explode.FindComponent<Animation2D>();
            this.Owner.AddChild(explode);

            return explode;
        }

        public void ShootBullet(bool player, float initX, float initY, float velocityX, float velocityY)
        {
            Entity bullet = player ? this.Bullets[BulletIndex] : this.EnemyBullets[EnemyBulletIndex];
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

        public void Explode(Transform2D transform)
        {
            var explode = this.explosions[this.ExplodeIndex];
            var transf = explode.FindComponent<Transform2D>();
            transf.X = transform.X;
            transf.Y = transform.Y;
            transf.Rotation = (float)WaveServices.Random.NextDouble();
            explode.IsActive = explode.IsVisible = true;
            var anim2D = explode.FindComponent<Animation2D>();
            anim2D.PlayAnimation("explosion", false);
        }

        protected override void Update(TimeSpan gameTime)
        {
            for (int i = 0; i < this.explosions.Length; i++)
            {
                if (this.explosions[i].FindComponent<Animation2D>().State == WaveEngine.Framework.Animation.AnimationState.Stopped)
                {
                    this.explosions[i].IsActive = this.explosions[i].IsVisible = false;
                }
            }

            if (this.state != GameState.Game)
            {
                if (this.player.IsActive)
                {
                    this.player.IsActive = false;
                }

                return;
            }

            if (!this.player.IsActive)
            {
                this.player.IsActive = true;
            }

            for (int i = 0; i < this.enemyColliders.Length; i++)
            {
                var enemy = this.enemies[i];

                if ((enemy.Enabled) && (enemy.FindComponent<Transform2D>().Y > (-this.virtualScreenManager.TopEdge - 200)))
                {
                    var enemyCollider = this.enemyColliders[i];

                    if (this.playerCollider.Intersects(enemyCollider))
                    {
                        enemy.FindComponent<EnemyBehavior>().Explode();
                        this.GameOver();
                        break;
                    }

                    for (int j = 0; j < this.bulletColliders.Length; j++)
                    {
                        var bullet = this.bullets[j];

                        if (bullet.Enabled)
                        {
                            var bulletCollider = this.bulletColliders[j];

                            if (bulletCollider.Intersects(enemyCollider))
                            {
                                this.Explode(enemy.FindComponent<Transform2D>());

                                enemy.FindComponent<EnemyBehavior>().Explode();

                                this.Score++;

                                bullet.Enabled = false;
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < this.enemyBulletColliders.Length; i++)
            {
                var enemyBullet = this.enemyBullets[i];
                if ((enemyBullet.Enabled) && (enemyBullet.FindComponent<Transform2D>().Y > (-this.virtualScreenManager.TopEdge - 200)))
                {
                    var enemyBulletCollider = this.enemyBulletColliders[i];

                    if (this.playerCollider.Intersects(enemyBulletCollider))
                    {
                        enemyBullet.Enabled = false;
                        this.GameOver();
                        break;
                    }
                }
            }
        }

        private void GameOver()
        {
            this.State = GameState.GameOver;

            this.Explode(this.player.FindComponent<Transform2D>());

            this.player.FindComponent<PlayerBehavior>().Explode();

            if (this.score > this.gameStorage.BestScore)
            {
                this.gameStorage.BestScore = this.score;

                WaveServices.Storage.Write(this.gameStorage);
            }
            
            WaveServices.ScreenContextManager.Push(new ScreenContext(new GameOverScene()), new ColorFadeTransition(Color.White, TimeSpan.FromSeconds(0.5f)));
        }

        private void Scored(int value)
        {
            WaveServices.GetService<ScoreManager>().CurrentScore = value;
        }

        public void Reset()
        {
            this.Score = 0;
            var handler = this.GameReset;
            if (handler != null)
            {
                handler();
            }
        }
    }
}
