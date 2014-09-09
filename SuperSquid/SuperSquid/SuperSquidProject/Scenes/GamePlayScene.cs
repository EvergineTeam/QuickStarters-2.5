#region File Description
//-----------------------------------------------------------------------------
// Super Squid
//
// Quickstarter for Wave University Tour 2014.
// Author: Wave Engine Team
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using SuperSquidProject.Commons;
using SuperSquidProject.Entities;
using SuperSquidProject.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Common.Media;
using WaveEngine.Components.Cameras;
using WaveEngine.Components.Gestures;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Components.Transitions;
using WaveEngine.Components.UI;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics2D;
using WaveEngine.Framework.Services;
using WaveEngine.Materials;
#endregion

namespace SuperSquidProject.Scenes
{
    public class GamePlayScene : Scene
    {
        private Squid squid;
        private BlockBuilder blockBuilder;

        private ScorePanel scorePanel;

        private BackgroundScene backScene;

        /// <summary>
        /// Gets or sets the current score.
        /// </summary>
        /// <value>
        /// The current score.
        /// </value>
        public int CurrentScore
        {
            get
            {
                return this.scorePanel.Score;
            }

            set
            {
                this.scorePanel.Score = value;
            }
        } 

        /// <summary>
        /// Creates the scene.
        /// </summary>
        /// <remarks>
        /// This method is called before all <see cref="T:WaveEngine.Framework.Entity" /> instances in this instance are initialized.
        /// </remarks>
        protected override void CreateScene()
        {
            var camera2D = new FixedCamera2D("Camera2D") { ClearFlags = ClearFlags.DepthAndStencil };
            EntityManager.Add(camera2D);

            //RenderManager.BackgroundColor = new Color(0 / 255f, 31 / 255f, 39 / 255f);

            //Backscene 
            this.backScene = WaveServices.ScreenContextManager.FindContextByName("BackContext")
                                                              .FindScene<BackgroundScene>();

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

            // Squid
            this.squid = new Squid(WaveServices.ViewportManager.VirtualHeight - 300);
            EntityManager.Add(this.squid);

            // Rocks
            this.blockBuilder = new BlockBuilder();
            EntityManager.Add(this.blockBuilder);

            // ScorePanel
            this.scorePanel = new ScorePanel();
            EntityManager.Add(scorePanel);

            // Scene Behaviors
            this.AddSceneBehavior(new DebugSceneBehavior(), SceneBehavior.Order.PostUpdate);
        }

        /// <summary>
        /// Allows to perform custom code when this instance is started.
        /// </summary>
        /// <remarks>
        /// This base method perfoms a layout pass.
        /// </remarks>
        protected override void Start()
        {
            base.Start();

            this.squid.Appear();
        }

        public void Reset()
        {
            this.CurrentScore = 0;

            // Replay background music
            WaveServices.MusicPlayer.Play(new MusicInfo(Directories.SoundsPath + "bg_music.mp3"));
            WaveServices.MusicPlayer.Volume = 1.0f;
            WaveServices.MusicPlayer.IsRepeat = true;

            //Resume back particles
            this.backScene.Resume();

            this.blockBuilder.Reset();
            this.squid.Appear();
        }

        public void OpenGameOver()
        {
            //Pause back particles
            this.backScene.Pause();

            var transition = new ColorFadeTransition(Color.White, TimeSpan.FromSeconds(0.5f));
            WaveServices.ScreenContextManager.Push(new ScreenContext(new GameOverScene()), transition);
        }
    }
}
