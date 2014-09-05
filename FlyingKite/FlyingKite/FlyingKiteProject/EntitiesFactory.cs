#region File Description
//-----------------------------------------------------------------------------
// Flying Kite
//
// Quickstarter for Wave University Tour 2014.
// Author: Wave Engine Team
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using FlyingKiteProject.Behaviors;
using FlyingKiteProject.Drawables;
using FlyingKiteProject.Entities;
using FlyingKiteProject.Layers;
using FlyingKiteProject.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WaveEngine.Common.Math;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Components.UI;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics2D;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.UI;
using WaveEngine.Materials;
#endregion

namespace FlyingKiteProject
{
    public static class EntitiesFactory
    {
        /// <summary>
        /// Creates the kite.
        /// </summary>
        /// <returns></returns>
        public static Kite CreateKite()
        {
            return new Kite();
        }

        /// <summary>
        /// Creates the kite ball.
        /// </summary>
        /// <returns></returns>
        public static Entity CreateKiteBall()
        {
            var ball = new Entity()
                    .AddComponent(new Transform2D()
                        {
                            X = WaveServices.ViewportManager.VirtualWidth / 4 + 112,
                            Y = WaveServices.ViewportManager.VirtualHeight / 2 + 47,
                        })
                    .AddComponent(new SpriteAtlas(Textures.GAME_ATLAS, Textures.GameAtlas.kite_ball.ToString()))
                    .AddComponent(new SpriteAtlasRenderer(DefaultLayers.GUI));

            return ball;
        }

        /// <summary>
        /// Creates the crash effect.
        /// </summary>
        /// <returns></returns>
        public static CrashEffect CreateCrashEffect()
        {
            return new CrashEffect();
        }

        /// <summary>
        /// Creates the linked rope.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="fromOrigin">From origin.</param>
        /// <param name="to">To.</param>
        /// <param name="toOrigin">To origin.</param>
        /// <returns></returns>
        public static Entity CreateLinkedRope(Entity from, Vector2 fromOrigin, Entity to, Vector2 toOrigin)
        {
            var rope = new Entity()
                        .AddComponent(new Material2D(new BasicMaterial2D(Textures.KITE_ROPE, DefaultLayers.Alpha)))
                        .AddComponent(new DrawableCurve2D())
                        .AddComponent(new LinkedRopeBehavior(from, fromOrigin, to, toOrigin));

            return rope;
        }

        /// <summary>
        /// Creates the obstacle pair.
        /// </summary>
        /// <param name="reappearanceX">The reappearance X.</param>
        /// <returns></returns>
        public static ObstaclePair CreateObstaclePair(float reappearanceX)
        {
            return new ObstaclePair(reappearanceX);
        }

        /// <summary>
        /// Creates the game star.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        public static Entity CreateGameStar(float x, float y)
        {
            var star = new Entity()
               .AddComponent(new Transform2D()
               {
                   X = x,
                   Y = y,
                   Origin = Vector2.Center
               })
               .AddComponent(new Sprite(Textures.STAR))
               .AddComponent(new SpriteRenderer(typeof(ObstaclesLayer)));

            return star;
        }

        /// <summary>
        /// Creates a new background entity.
        /// </summary>
        /// <returns></returns>
        public static Entity CreateBackground()
        {
            var background = new Entity()
                .AddComponent(new Transform2D()
                {
                    X = WaveServices.ViewportManager.LeftEdge,
                    Y = WaveServices.ViewportManager.TopEdge,
                    XScale = (WaveServices.ViewportManager.ScreenWidth / 1300) / WaveServices.ViewportManager.RatioX,
                    DrawOrder = 1f,
                })
                .AddComponent(new Sprite(Textures.BG_IMAGE))
                .AddComponent(new SpriteRenderer(DefaultLayers.Opaque));

            return background;
        }

        /// <summary>
        /// Creates a new backgroundCloud entity.
        /// </summary>
        /// <returns></returns>
        public static Entity CreateBackgroundCloud()
        {
            var scrollBehavior = new ScrollBehavior(0.06f);

            var transform = new Transform2D()
            {
                X = WaveServices.ViewportManager.VirtualWidth / 2,
                Y = 197,
            };

            var cloud = new Entity()
                .AddComponent(transform)
                .AddComponent(new SpriteAtlas(Textures.GAME_ATLAS, Textures.GameAtlas.bg_cloud.ToString()))
                .AddComponent(new SpriteAtlasRenderer(DefaultLayers.Alpha))
                .AddComponent(scrollBehavior);

            scrollBehavior.EntityOutOfScreen += (entity) =>
            {
                transform.X = WaveServices.ViewportManager.RightEdge;
            };

            return cloud;
        }

        /// <summary>
        /// Creates a new backgroundPlane entity.
        /// </summary>
        /// <returns></returns>
        public static Entity CreateBackgroundPlane()
        {
            var scrollBehavior = new ScrollBehavior(0.08f);

            var transform = new Transform2D()
            {
                X = WaveServices.ViewportManager.RightEdge - 150,
                Y = 273,
                DrawOrder = 0.85f
            };

            var cloud = new Entity()
                .AddComponent(transform)
                .AddComponent(new SpriteAtlas(Textures.GAME_ATLAS, Textures.GameAtlas.bg_plane.ToString()))
                .AddComponent(new SpriteAtlasRenderer(DefaultLayers.Alpha))
                .AddComponent(scrollBehavior);

            scrollBehavior.EntityOutOfScreen += (entity) =>
            {
                transform.X = WaveServices.ViewportManager.RightEdge + WaveServices.ViewportManager.ScreenWidth;
            };

            return cloud;
        }

        /// <summary>
        /// Creates a new background kite.
        /// </summary>
        /// <param name="initialX">The initial X of the new element.</param>
        /// <returns></returns>
        public static BackgroundKite CreateBackgroundKite(float initialX)
        {
            return new BackgroundKite(initialX);
        }

        /// <summary>
        /// Creates a new BestScore panel.
        /// </summary>
        /// <param name="y">The y of the new element.</param>
        /// <param name="score">The score of the new element.</param>
        /// <returns></returns>
        public static StackPanel CreateBestScore(int y, int score)
        {
            var stackPanelScore = new StackPanel()
            {
                HorizontalAlignment = WaveEngine.Framework.UI.HorizontalAlignment.Center,
                Orientation = WaveEngine.Components.UI.Orientation.Horizontal,
                Margin = new Thickness(
                            0,
                            y,
                            0,
                            0)
            };

            var bestScoreText = new Image(Textures.BEST_SCORE); 

            var bestScoreNumber = new TextBlock()
            {
                FontPath = Fonts.SMALL_SCORE,
                Text = score.ToString(),
                TextAlignment = WaveEngine.Components.UI.TextAlignment.Right,
                Width = 50,
            };

            stackPanelScore.Add(bestScoreText);
            stackPanelScore.Add(bestScoreNumber);

            return stackPanelScore;
        }

        /// <summary>
        /// Creates a new CurrentScore TextBlock.
        /// </summary>
        /// <param name="y">The y of the new element.</param>
        /// <returns></returns>
        public static TextBlock CreateCurrentScore(int y)
        {
            var scoreNumber = new TextBlock()
            {
                FontPath = Fonts.BIG_SCORE,
                HorizontalAlignment = WaveEngine.Framework.UI.HorizontalAlignment.Center,
                TextAlignment = WaveEngine.Components.UI.TextAlignment.Center,
                Margin = new Thickness(0, y, 0, 0),
                Width = WaveServices.ViewportManager.VirtualWidth,
                Height = 117
            };

            return scoreNumber;
        }

        /// <summary>
        /// Creates a new logo.
        /// </summary>
        /// <returns></returns>
        public static Image CreateLogo()
        {
            var logo = new Image(Textures.LOGO)
            {
                HorizontalAlignment = WaveEngine.Framework.UI.HorizontalAlignment.Center,
                Margin = new Thickness(0, 65, 0, 0),
            };

            return logo;
        }

        /// <summary>
        /// Creates a new play button.
        /// </summary>
        /// <param name="x">The x of the new button.</param>
        /// <param name="y">The y of the new button.</param>
        /// <returns></returns>
        public static Button CreatePlayButton(float x, float y)
        {
            var button = new Button()
            {
                Margin = new Thickness(0, y, 0, 0),
                HorizontalAlignment = WaveEngine.Framework.UI.HorizontalAlignment.Center,
                Text = string.Empty,
                IsBorder = false,
                BackgroundImage = Textures.PLAY_BUTTON,
                PressedBackgroundImage = Textures.PLAY_BUTTON_PRESSED
                
            };

            return button;
        }

        /// <summary>
        /// Creates a new GameOverText Image.
        /// </summary>
        /// <returns></returns>
        public static Image CreateGameOverText()
        {
            var gameOverImage = new Image(Textures.GAME_OVER)
            {
                HorizontalAlignment = WaveEngine.Framework.UI.HorizontalAlignment.Center,
                Margin = new Thickness(0, 123, 0, 0),
            };

            return gameOverImage;
        }

        /// <summary>
        /// Creates a new gameOverBackground.
        /// </summary>
        /// <returns></returns>
        public static Entity CreateGameOverBackground()
        {
            var background = new Entity()
                .AddComponent(new Transform2D()
                {
                    X = WaveServices.ViewportManager.LeftEdge,
                    Y = 161,
                    XScale = WaveServices.ViewportManager.ScreenWidth / WaveServices.ViewportManager.RatioX,
                    DrawOrder = 1f
                })
                .AddComponent(new Sprite(Textures.BG_GAME_OVER))
                .AddComponent(new SpriteRenderer(DefaultLayers.GUI));

            return background;
        }
    }
}
