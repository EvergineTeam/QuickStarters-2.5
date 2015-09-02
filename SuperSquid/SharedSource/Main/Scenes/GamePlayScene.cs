#region Using Statements
using SuperSquid.Components;
using SuperSquid.Entities;
using System;
using System.Collections.Generic;
using WaveEngine.Common;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Components.Cameras;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Components.Graphics3D;
using WaveEngine.Components.Particles;
using WaveEngine.Components.UI;
using WaveEngine.Framework;
using WaveEngine.Framework.Animation;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Resources;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.UI;
#endregion

namespace SuperSquid.Scenes
{
    public class GamePlayScene : Scene
    {
        private BackgroundScene backScene;

        public GamePlayManager GamePlayManager
        {
            get
            {
                return this.EntityManager.Find("GamePlayManager").FindComponent<GamePlayManager>();
            }
        }

        protected override void CreateScene()
        {
            this.Load(WaveContent.Scenes.GamePlayScene);
            
            this.EntityManager.Find("defaultCamera2D").FindComponent<Camera2D>().CenterScreen();

            //Backscene 
            this.backScene = WaveServices.ScreenContextManager.FindContextByName("BackContext")
                                                              .FindScene<BackgroundScene>();

            this.CreateUI();
        }

        private void CreateUI()
        {
            // Side black panels
            Entity rightBlackpanel = new Entity()
                    .AddComponent(new Transform2D()
                    {
                        DrawOrder = 1f,
                        X = WaveServices.ViewportManager.LeftEdge
                    })
                    .AddComponent(new ImageControl(
                        Color.Black,
                        (int)-WaveServices.ViewportManager.LeftEdge,
                        (int)WaveServices.ViewportManager.VirtualHeight))
                    .AddComponent(new ImageControlRenderer(DefaultLayers.GUI));
            EntityManager.Add(rightBlackpanel);

            Entity leftBlackpanel = new Entity()
                    .AddComponent(new Transform2D()
                    {
                        DrawOrder = 1f,
                        X = WaveServices.ViewportManager.VirtualWidth
                    })
                    .AddComponent(new ImageControl(
                        Color.Black,
                        (int)-WaveServices.ViewportManager.LeftEdge,
                        (int)WaveServices.ViewportManager.VirtualHeight))
                    .AddComponent(new ImageControlRenderer(DefaultLayers.GUI));
            EntityManager.Add(leftBlackpanel);

            // ScorePanel
            ScorePanel scorePanel = new ScorePanel("ScorePanel");
            this.EntityManager.Add(scorePanel);
        }

        protected override void Start()
        {
            base.Start();

            this.Reset();
        }

        public void Reset()
        {
            this.GamePlayManager.StartGame(); 
        }

        public int CurrentScore 
        { 
            get
            {
                return this.GamePlayManager.CurrentScore;
            }
        }
    }
}
