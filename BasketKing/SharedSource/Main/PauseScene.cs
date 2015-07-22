#region Using Statements
using BasketKing.Commons;
using BasketKing.Entities;
using BasketKing.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Graphics;
using WaveEngine.Components.Cameras;
using WaveEngine.Components.UI;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.UI;
#endregion

namespace BasketKing
{
    public class PauseScene : Scene
    {
        private GamePlayScene gameplayScene;

        /// <summary>
        /// Initializes a new instance of the <see cref="PauseScene" /> class.
        /// </summary>
        /// <param name="gameplayScene">The gameplay scene.</param>
        public PauseScene(GamePlayScene gameplayScene)
        {
            this.gameplayScene = gameplayScene;
        }

        /// <summary>
        /// Creates the scene.
        /// </summary>
        /// <remarks>
        /// This method is called before all <see cref="T:WaveEngine.Framework.Entity" /> instances in this instance are initialized.
        /// </remarks>
        protected override void CreateScene()
        {
            FixedCamera2D camera2d = new FixedCamera2D("camera2d")
            {
                ClearFlags = ClearFlags.DepthAndStencil,
            };
            EntityManager.Add(camera2d);

            // Dark background
            Entity dark = new Entity()
                                .AddComponent(new Transform2D()
                                {
                                    X = WaveServices.ViewportManager.LeftEdge,
                                    Y = WaveServices.ViewportManager.TopEdge,
                                    XScale = 1 / WaveServices.ViewportManager.RatioX,
                                    YScale = 1 / WaveServices.ViewportManager.RatioY,
                                    Opacity = 0.4f,
                                    DrawOrder = 2f, 
                                })
                                .AddComponent(new ImageControl(Color.Black, (int)WaveServices.ViewportManager.ScreenWidth, (int)WaveServices.ViewportManager.ScreenHeight))
                                .AddComponent(new ImageControlRenderer(DefaultLayers.GUI));
            EntityManager.Add(dark);

            // Pause text
            Image i_pausetext = new Image(Directories.Textures + "pause_text.wpk")
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0,132,0,0),
            };
            EntityManager.Add(i_pausetext);

            // Panel
            Grid gridPanel = new Grid()
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Width = 475,
                Height = 418,
            };
            EntityManager.Add(gridPanel.Entity);

            gridPanel.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Proportional) });
            gridPanel.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Proportional) });
            gridPanel.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Proportional) });

            gridPanel.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Proportional) });           

            // Resume option
            ButtonOption b_resume = new ButtonOption("resume", ()=>
            {
                WaveServices.ScreenContextManager.Pop();
                SoundsManager.Instance.PlaySound(SoundsManager.SOUNDS.Button);
            });

            b_resume.Grid.SetValue(GridControl.RowProperty, 0);
            b_resume.Grid.SetValue(GridControl.ColumnProperty, 0);
            gridPanel.Add(b_resume.Grid);

            // Restart option
            ButtonOption b_restart = new ButtonOption("restart",() =>
            {
                gameplayScene.CurrentState = GamePlayScene.States.HurryUp;
                WaveServices.ScreenContextManager.Pop();
                SoundsManager.Instance.PlaySound(SoundsManager.SOUNDS.Button);
            });
            b_restart.Grid.SetValue(GridControl.RowProperty, 1);
            b_restart.Grid.SetValue(GridControl.ColumnProperty, 0);
            gridPanel.Add(b_restart.Grid);

            // Exit option
            ButtonOption b_exit = new ButtonOption("exit", ()=>
            {
                gameplayScene.CurrentState = GamePlayScene.States.TapToStart;
                WaveServices.ScreenContextManager.Pop();

                SoundsManager.Instance.PlaySound(SoundsManager.SOUNDS.Button);
            });
            b_exit.Grid.SetValue(GridControl.RowProperty, 2);
            b_exit.Grid.SetValue(GridControl.ColumnProperty, 0);
            gridPanel.Add(b_exit.Grid);

            // Scene behavior
            this.AddSceneBehavior(new DebugSceneBehavior(), SceneBehavior.Order.PostUpdate);
        }
    }
}
