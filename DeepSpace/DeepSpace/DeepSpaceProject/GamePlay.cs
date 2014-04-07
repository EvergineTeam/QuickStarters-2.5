#region Using Statements
using DeepSpaceProject.Behaviors;
using System;
using WaveEngine.Common;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics2D;
using WaveEngine.Framework.Services;
#endregion

namespace DeepSpaceProject
{
    public class GamePlay : Scene
    {
        protected override void CreateScene()
        {
            RenderManager.BackgroundColor = Color.Black;
            System.Random rnd = new System.Random(23);

            CreatePlayer();

            CreateEnemies(rnd);

            CreateStars(rnd);
        }

        private void CreateStars(System.Random rnd)
        {
            for (int i = 0; i < 100; i++)
            {
                Entity star = new Entity()
                    .AddComponent(new Transform2D()
                    {
                        X = rnd.Next((int)WaveServices.ViewportManager.VirtualWidth),
                        Y = rnd.Next((int)WaveServices.ViewportManager.VirtualHeight),
                        DrawOrder = 1
                    })
                    .AddComponent(new StarBehavior((float)rnd.NextDouble()))
                    .AddComponent(new Sprite("Content/Star.wpk")
                    {
                        TintColor = new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble())
                    })
                    .AddComponent(new SpriteRenderer(DefaultLayers.Opaque));

                EntityManager.Add(star);
            }
        }

        private void CreateEnemies(System.Random rnd)
        {
            for (int i = 0; i < 10; i++)
            {
                Entity enemy = new Entity()
                    .AddComponent(new Transform2D()
                    {
                        X = rnd.Next((int)WaveServices.ViewportManager.ScreenWidth),
                        Y = rnd.Next((int)-WaveServices.ViewportManager.ScreenHeight * 5, 100),
                        Origin = new Vector2(0.5f, 0),
                        DrawOrder = 0.5f
                    })
                    .AddComponent(new RectangleCollider())
                    .AddComponent(new EnemyBehavior())
                    .AddComponent(new Sprite("Content/Enemy.wpk"))
                    .AddComponent(new SpriteRenderer(DefaultLayers.Opaque));

                EntityManager.Add(enemy);
            }
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
                .AddComponent(new RectangleCollider())
                .AddComponent(new PlayerBehavior())
                .AddComponent(new Sprite("Content/Player.wpk"))
                .AddComponent(new SpriteRenderer(DefaultLayers.Opaque));
            
            EntityManager.Add(player);

            // Bullet Manager
            BulletManager bulletManager = new BulletManager();
            EntityManager.Add(bulletManager);
        }
    }
}
