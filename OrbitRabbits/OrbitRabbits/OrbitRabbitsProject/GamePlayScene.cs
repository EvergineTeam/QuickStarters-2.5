#region File Description
//-----------------------------------------------------------------------------
// OrbitRabbits
//
// Quickstarter for Wave University Tour 2014.
// Author: Wave Engine Team
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using OrbitRabbitsProject.Commons;
using OrbitRabbitsProject.Entities;
using OrbitRabbitsProject.Entities.Behaviors;
using OrbitRabbitsProject.Managers;
using System;
using System.Collections.Generic;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Components.Cameras;
using WaveEngine.Components.Gestures;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Components.UI;
using WaveEngine.Framework;
using WaveEngine.Framework.Animation;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics2D;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.UI;
#endregion

namespace OrbitRabbitsProject
{
    public class GamePlayScene : Scene
    {
        private enum States
        {
            Paused,
            Play,
        };

        private States currentState;
        private Entity tapHand;
        private RabbitEmiter rabbitEmiter;
        private ScorePanel scorePanel;

        /// <summary>
        /// Creates the scene.
        /// </summary>
        /// <remarks>
        /// This method is called before all <see cref="T:WaveEngine.Framework.Entity" /> instances in this instance are initialized.
        /// </remarks>
        protected override void CreateScene()
        {
            FixedCamera2D camera2d = new FixedCamera2D("camera");
            camera2d.BackgroundColor = Color.Black;
            EntityManager.Add(camera2d);

            //Background          
            Entity background = new Entity()
                            .AddComponent(new Transform2D()
                            {
                                DrawOrder = 1,
                            })
                            .AddComponent(new StretchBehavior())
                            .AddComponent(new Sprite(Directories.TexturePath + "background.wpk"))
                            .AddComponent(new SpriteRenderer(DefaultLayers.Opaque));
            EntityManager.Add(background);

            // Logo
            Entity logo = new Entity()
                           .AddComponent(new Transform2D()
                           {
                               X = 15,
                               Y = WaveServices.ViewportManager.TopEdge,
                               DrawOrder = 0.9f,
                           })
                           .AddComponent(new SpriteAtlas(Directories.TexturePath + "game.wpk", "smallLogo"))
                           .AddComponent(new SpriteAtlasRenderer(DefaultLayers.Alpha));
            EntityManager.Add(logo);

            // Gravity
            Entity gravity = new Entity()
                            .AddComponent(new Transform2D()
                            {
                                Origin = Vector2.Center,
                                X = WaveServices.ViewportManager.VirtualWidth / 2,
                                Y = 605,
                                DrawOrder = 0.9f,
                            })
                            .AddComponent(new SpriteAtlas(Directories.TexturePath + "game.wpk", "gravity"))
                            .AddComponent(new SpriteAtlasRenderer(DefaultLayers.Alpha));
            EntityManager.Add(gravity);
            
            // Moon back
            Entity moonBack = new Entity()
                            .AddComponent(new Transform2D()
                            {
                                Origin = Vector2.Center,
                                X = WaveServices.ViewportManager.VirtualWidth / 2,
                                Y = 605,
                                DrawOrder = 0.8f,
                            })
                            .AddComponent(new SpriteAtlas(Directories.TexturePath + "game.wpk", "moonBack"))
                            .AddComponent(new SpriteAtlasRenderer(DefaultLayers.Alpha));
            EntityManager.Add(moonBack);

            // Rabbits
            this.rabbitEmiter = new RabbitEmiter();
            EntityManager.Add(this.rabbitEmiter);          

            // Tap hand
            this.tapHand = new Entity()
                            .AddComponent(new Transform2D()
                            {
                                X = 305,
                                Y = 598,
                                DrawOrder = 0.4f,
                            })
                            .AddComponent(new SpriteAtlas(Directories.TexturePath + "game.wpk", "tapHand"))
                            .AddComponent(new SpriteAtlasRenderer(DefaultLayers.Alpha));
            EntityManager.Add(this.tapHand);

            // Touch Panel
            Entity touchPanel = new Entity()
                                .AddComponent(new Transform2D()
                                {
                                    DrawOrder = 0.1f,
                                    Rectangle = new RectangleF(WaveServices.ViewportManager.LeftEdge, 
                                                               WaveServices.ViewportManager.TopEdge,
                                                               WaveServices.ViewportManager.RightEdge - WaveServices.ViewportManager.LeftEdge,
                                                               WaveServices.ViewportManager.BottomEdge - WaveServices.ViewportManager.TopEdge),
                                })
                                .AddComponent(new RectangleCollider())
                                .AddComponent(new TouchGestures());
            var touch = touchPanel.FindComponent<TouchGestures>();
            touch.TouchPressed += (s, o) =>
            {
                switch (this.currentState)
                {
                    case States.Paused:
                        this.SetNewState(States.Play);
                        break;
                    case States.Play:

                        this.rabbitEmiter.ApplyImpuseToLast();
                        
                        break;                
                }                
            };
            EntityManager.Add(touchPanel);

            // Restart Button
            Button restart = new Button()
            {
                Text = string.Empty,
                IsBorder = false,
                BackgroundImage = Directories.TexturePath + "restartRelease.wpk",
                PressedBackgroundImage = Directories.TexturePath + "restartPressed.wpk",
                Margin = new Thickness(10, WaveServices.ViewportManager.BottomEdge - 120, 0 ,0)
            };
            restart.Click += (s, o) =>
            {
                this.SetNewState(States.Paused);
                SoundsManager.Instance.PlaySound(SoundsManager.SOUNDS.Click);
            };
            EntityManager.Add(restart);

            // Score Panel            
            this.scorePanel = new ScorePanel()
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
            };
            EntityManager.Add(this.scorePanel);

            this.rabbitEmiter.ScoreChanged += rabbitEmiter_scoreChanged;

            // Add scene behaviors
            this.AddSceneBehavior(new DebugSceneBehavior(), SceneBehavior.Order.PostUpdate);
        }

        /// <summary>
        /// Rabbits the emiter_score changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void rabbitEmiter_scoreChanged(object sender, int e)
        {
            this.scorePanel.Scores = e;
        }        

        /// <summary>
        /// Sets the new state.
        /// </summary>
        /// <param name="state">The state.</param>
        private void SetNewState(States state)
        {
            switch (state)
            {
                case States.Paused:

                    this.tapHand.IsVisible = true;
                    this.rabbitEmiter.Emit = false;
                    this.rabbitEmiter.Clear();
                    break;
                case States.Play:

                    this.tapHand.IsVisible = false;
                    this.rabbitEmiter.Emit = true;
                    this.rabbitEmiter.AddRabbit();

                    SoundsManager.Instance.PlaySound(SoundsManager.SOUNDS.Start);

                    break;
            }

            this.currentState = state;
        }       
    }
}
