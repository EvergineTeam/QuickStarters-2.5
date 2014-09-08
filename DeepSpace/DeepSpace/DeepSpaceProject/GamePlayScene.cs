#region Using Statements
using DeepSpaceProject.Behaviors;
using DeepSpaceProject.Managers;
using System;
using WaveEngine.Common;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Components.Animation;
using WaveEngine.Components.Cameras;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Components.UI;
using WaveEngine.Framework;
using WaveEngine.Framework.Animation;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics2D;
using WaveEngine.Framework.Services;
#endregion

namespace DeepSpaceProject
{
    public class GamePlayScene : Scene
    {
        private GameplayBehavior gameplayBehavior;

        
        private TextBlock scoreText;

        private GameState state;

        public GameState State
        {
            get
            {
                return state;
            }

            set
            {
                state = value;
                this.gameplayBehavior.State = value;
            }
        }

        protected override void CreateScene()
        {
            FixedCamera2D camera2d = new FixedCamera2D("camera");
            camera2d.BackgroundColor = Color.Black;
            EntityManager.Add(camera2d);

            System.Random rnd = new System.Random(23);

            CreateBackground();

            this.gameplayBehavior = new GameplayBehavior();

            this.gameplayBehavior.Scored += gameplayBehavior_Scored;

            this.AddSceneBehavior(this.gameplayBehavior, SceneBehavior.Order.PostUpdate);

            this.CreatePlanet();

            CreatePlayer();

            CreateEnemies(rnd);

            CreateStars();

            CreateHUD();

            this.gameplayBehavior.Reset();
        }

        void gameplayBehavior_Scored(object sender, int e)
        {
            this.scoreText.Text = e.ToString();
        }

        private void CreateHUD()
        {
            Entity scoreBack = new Entity("scoreBack")
                  .AddComponent(new Transform2D()
                  {
                      Origin = new Vector2(0, 1),
                      X = WaveServices.ViewportManager.LeftEdge,
                      Y = WaveServices.ViewportManager.BottomEdge,
                      DrawOrder = 0.1f
                  })
                  .AddComponent(new Sprite("Content/score.wpk"))
                  .AddComponent(new SpriteRenderer(DefaultLayers.Alpha));
            this.EntityManager.Add(scoreBack);

            this.scoreText = new TextBlock()
            {
                Text = "0",
                FontPath = "Content/Space Age.wpk",
                HorizontalAlignment = WaveEngine.Framework.UI.HorizontalAlignment.Left,
                VerticalAlignment = WaveEngine.Framework.UI.VerticalAlignment.Bottom,
                Margin = new WaveEngine.Framework.UI.Thickness(WaveServices.ViewportManager.LeftEdge + 80, 0, 0, 30),
                Foreground = new Color(117, 243, 237, 255)
            };

            this.EntityManager.Add(this.scoreText);
        }

        /// <summary>
        /// Creates a new background entity.
        /// </summary>
        /// <returns></returns>
        public void CreateBackground()
        {
            Entity backEntity = new Entity("back")
               .AddComponent(new Transform2D()
               {
                   Origin = Vector2.Zero,
                   X = WaveServices.ViewportManager.LeftEdge,
                   Y = WaveServices.ViewportManager.TopEdge,
                   XScale = (WaveServices.ViewportManager.ScreenWidth / 768f) / WaveServices.ViewportManager.RatioX,
                   YScale = (WaveServices.ViewportManager.ScreenHeight / 1024f) / WaveServices.ViewportManager.RatioY,
                   DrawOrder = 0.9f
               })
               .AddComponent(new Sprite("Content/bg_space.wpk"))
               .AddComponent(new SpriteRenderer(DefaultLayers.Alpha));
            this.EntityManager.Add(backEntity);
        }

        public void CreatePlanet()
        {
            Entity planet = new Entity()
                    .AddComponent(new Transform2D()
                    {
                        X = WaveServices.ViewportManager.VirtualWidth * 0.7f,
                        Y = 400,
                        DrawOrder = 0.75f,
                        Origin = Vector2.Center
                    })
                    .AddComponent(new StarBehavior(0.2f, 1000))
                    .AddComponent(new Sprite("Content/bg_planets.wpk"))
                    .AddComponent(new SpriteRenderer(DefaultLayers.Alpha));

            EntityManager.Add(planet);
        }

        private void CreateStars()
        {
            for (int i = 0; i < 100; i++)
            {
                Entity star = new Entity()
                    .AddComponent(new Transform2D()
                    {
                        X = WaveServices.Random.Next((int)WaveServices.ViewportManager.VirtualWidth),
                        Y = WaveServices.Random.Next((int)WaveServices.ViewportManager.VirtualHeight),
                        DrawOrder = 0.8f
                    })
                    .AddComponent(new StarBehavior((float)WaveServices.Random.NextDouble() * 10, 0))
                    .AddComponent(new Sprite("Content/Star.wpk")
                    {
                        TintColor = new Color((float)WaveServices.Random.NextDouble() * 0.3f, (float)WaveServices.Random.NextDouble() * 0.3f, (float)WaveServices.Random.NextDouble() * 0.3f)
                    })
                    .AddComponent(new SpriteRenderer(DefaultLayers.Additive));

                EntityManager.Add(star);
            }
        }

        private void CreateEnemies(System.Random rnd)
        {
            var enemies = new Entity[10];

            for (int i = 0; i < enemies.Length; i++)
            {
                Entity enemy = new Entity()
                    .AddComponent(new Transform2D()
                    {
                        X = rnd.Next((int)WaveServices.ViewportManager.VirtualWidth),
                        Y = rnd.Next((int)-WaveServices.ViewportManager.VirtualHeight * 6, -500),
                        Origin = Vector2.Center,
                        DrawOrder = 0.5f
                    })
                    .AddComponent(new PerPixelCollider("Content/EnemyCollider.wpk", 0.5f))
                    .AddComponent(new EnemyBehavior())
                    .AddComponent(new Sprite("Content/Enemy.wpk"))
                    .AddComponent(new SpriteRenderer(DefaultLayers.Alpha));

                enemy.Enabled = false;

                EntityManager.Add(enemy);

                enemies[i] = enemy;
            }

            this.gameplayBehavior.Enemies = enemies;
        }

        private void CreatePlayer()
        {
            Entity player = new Entity("Player")
                .AddComponent(new Transform2D()
                {
                    X = WaveServices.ViewportManager.VirtualWidth / 2f,
                    Y = WaveServices.ViewportManager.VirtualHeight * 0.75f,
                    Origin = Vector2.Center,
                    DrawOrder = 0,
                })
                .AddComponent(new PerPixelCollider("Content/PlayerCollider.wpk", 0.5f))
                .AddComponent(new PlayerBehavior(this.gameplayBehavior))
                .AddComponent(new Sprite("Content/Player.wpk"))
                .AddComponent(new SpriteRenderer(DefaultLayers.Alpha));

            EntityManager.Add(player);

            // Bullet Manager
            BulletManager bulletManager = new BulletManager(this.gameplayBehavior);
            EntityManager.Add(bulletManager);

            this.gameplayBehavior.Player = player;

            // Left Joystick
            RectangleF leftArea = new RectangleF(WaveServices.ViewportManager.LeftEdge,
                                                  WaveServices.ViewportManager.TopEdge,
                                                  WaveServices.ViewportManager.VirtualWidth / 2f  + Math.Abs(WaveServices.ViewportManager.LeftEdge),
                                                  WaveServices.ViewportManager.VirtualHeight + Math.Abs(WaveServices.ViewportManager.TopEdge));
            var leftJoystick = new Joystick("leftJoystick", leftArea);
            EntityManager.Add(leftJoystick);

            // Right Joystick
            RectangleF rightArea = new RectangleF(WaveServices.ViewportManager.VirtualWidth / 2,
                                                  WaveServices.ViewportManager.TopEdge,
                                                  WaveServices.ViewportManager.VirtualWidth / 2f + Math.Abs(WaveServices.ViewportManager.LeftEdge),
                                                  WaveServices.ViewportManager.VirtualHeight + Math.Abs(WaveServices.ViewportManager.TopEdge));
            var fireButton = new FireButton("fireButton", rightArea);
            EntityManager.Add(fireButton);
        }

        protected override void Start()
        {
            base.Start();
            this.Reset();
        }

        public void Reset()
        {
            this.gameplayBehavior.Reset();
        }
    }
}
