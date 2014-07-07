using DeepSpaceProject.Behaviors;
using DeepSpaceProject.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Common.Media;
using WaveEngine.Components;
using WaveEngine.Components.Animation;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Components.Transitions;
using WaveEngine.Components.UI;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics2D;
using WaveEngine.Framework.Services;

namespace DeepSpaceProject.Behaviors
{
    public enum GameState
    {
        Intro,
        Game,
        GameOver
    }

    public class GameplayBehavior : SceneBehavior
    {
        private GameStorage gameStorage;

        private GameState state;

        private PerPixelCollider playerCollider;

        private PerPixelCollider[] enemyColliders;

        private PerPixelCollider[] bulletColliders;

        private PerPixelCollider[] enemyBulletColliders;

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

                if (this.Scored != null)
                {
                    this.Scored(this, value);
                }
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

                switch (value)
                {
                    case GameState.Intro:
                        break;
                    case GameState.Game:
                        foreach (var enemy in this.enemies)
                        {
                            enemy.Enabled = true;
                        }
                        break;
                    case GameState.GameOver:
                        break;
                    default:
                        break;
                }
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


        public event EventHandler<int> Scored;

        public Entity Player
        {
            get
            {
                return player;
            }

            set
            {
                this.player = value;
                this.playerCollider = this.player.FindComponent<PerPixelCollider>();
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

                this.enemyColliders = new PerPixelCollider[value.Length];

                for (int i = 0; i < value.Length; i++)
                {
                    this.enemyColliders[i] = value[i].FindComponent<PerPixelCollider>();
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

                this.bulletColliders = new PerPixelCollider[value.Length];

                for (int i = 0; i < value.Length; i++)
                {
                    this.bulletColliders[i] = value[i].FindComponent<PerPixelCollider>();
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

                this.enemyBulletColliders = new PerPixelCollider[value.Length];

                for (int i = 0; i < value.Length; i++)
                {
                    this.enemyBulletColliders[i] = value[i].FindComponent<PerPixelCollider>();
                }
            }
        }
        public GameplayBehavior()
            : base()
        {
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
                    .AddComponent(new Sprite("Content/explodeSprite.wpk"))
                    .AddComponent(Animation2D.Create<TexturePackerGenericXml>("Content/explodeSprite.xml")
                        .Add("explosion", new SpriteSheetAnimationSequence() { First = 0, Length = 10, FramesPerSecond = 60 }))

                    .AddComponent(new AnimatedSpriteRenderer(DefaultLayers.Additive));

            explode.Enabled = false;

            var anim2D = explode.FindComponent<Animation2D>();
            this.Scene.EntityManager.Add(explode);

            return explode;
        }

        public void Explode(Transform2D transform)
        {
            var explode = this.explosions[this.ExplodeIndex];
            var transf = explode.FindComponent<Transform2D>();
            transf.X = transform.X;
            transf.Y = transform.Y;
            transf.Rotation = (float)WaveServices.Random.NextDouble();
            explode.Enabled = true;
            var anim2D = explode.FindComponent<Animation2D>();

            anim2D.CurrentAnimation = "explosion";
            anim2D.Play(false);
        }

        protected override void Update(TimeSpan gameTime)
        {
            if (this.state != GameState.Game)
            {
                return;
            }

            for (int i = 0; i < this.enemyColliders.Length; i++)
            {
                var enemy = this.enemies[i];

                if ((enemy.Enabled) && (enemy.FindComponent<Transform2D>().Y > (-WaveServices.ViewportManager.TopEdge - 200)))
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

                                if (this.Scored != null)
                                {
                                    this.Scored(this, this.Score);
                                }

                                bullet.Enabled = false;
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < this.enemyBulletColliders.Length; i++)
            {
                var enemyBullet = this.enemyBullets[i];
                if ((enemyBullet.Enabled) && (enemyBullet.FindComponent<Transform2D>().Y > (-WaveServices.ViewportManager.TopEdge - 200)))
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

                WaveServices.Storage.Write<GameStorage>(this.gameStorage);
            }

            WaveServices.ScreenContextManager.Push(new ScreenContext(new GameOverScene()), new ColorFadeTransition(Color.White, TimeSpan.FromSeconds(0.2f)));
        }

        public void Reset()
        {
            System.Random rnd = new System.Random(23);

            this.Score = 0;

            if (this.Scored != null)
            {
                this.Scored(this, this.Score);
            }

            this.player.Enabled = true;

            for (int i = 0; i < enemies.Length; i++)
            {
                var enemy = this.enemies[i];
                var transform = enemy.FindComponent<Transform2D>();
                transform.X = rnd.Next((int)WaveServices.ViewportManager.VirtualWidth);
                transform.Y = rnd.Next((int)-WaveServices.ViewportManager.VirtualHeight * 6, -500);

            }

            var playerTransform = this.player.FindComponent<Transform2D>();
            playerTransform.X = WaveServices.ViewportManager.VirtualWidth / 2;
            playerTransform.Y = WaveServices.ViewportManager.VirtualHeight * 0.75f;

            for (int i = 0; i < this.bullets.Length; i++)
            {
                bullets[i].Enabled = false;
            }

            for (int i = 0; i < this.enemyBullets.Length; i++)
            {
                enemyBullets[i].Enabled = false;
            }
        }
    }
}
