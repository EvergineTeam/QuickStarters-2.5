#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MangomacoProject.Factories;
using MangomacoProject.Services;
using WaveEngine.Common.Graphics;
using WaveEngine.Components.Cameras;
using WaveEngine.Components.UI;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.UI;
#endregion

namespace MangomacoProject.Scenes
{
    /// <summary>
    /// Pause Scene
    /// </summary>
    public class PauseScene : Scene
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PauseScene" /> class.
        /// </summary>
        /// <param name="gameplayScene">The gameplay scene.</param>
        public PauseScene()
        {
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
                                    DrawOrder = 1f,
                                })
                                .AddComponent(new ImageControl(Color.Black, (int)WaveServices.ViewportManager.ScreenWidth, (int)WaveServices.ViewportManager.ScreenHeight))
                                .AddComponent(new ImageControlRenderer(DefaultLayers.GUI));
            EntityManager.Add(dark);

            // Panel
            var backPanel = ControlsFactory.CreatePauseBgPanel();
            EntityManager.Add(backPanel);

            Grid gridPanel = new Grid()
            {
                DrawOrder = -1,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 20, 0, 0),
                Width = 300,
                Height = 100,
            };
            EntityManager.Add(gridPanel.Entity);

            gridPanel.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Proportional) });

            gridPanel.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Proportional) });
            gridPanel.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Proportional) });
            gridPanel.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Proportional) });  

            // Resume option
            var resumeBtn = ControlsFactory.CreateResumeButton();
            resumeBtn.SetValue(GridControl.RowProperty, 0);
            resumeBtn.SetValue(GridControl.ColumnProperty, 0);
            gridPanel.Add(resumeBtn);

            // Restart option
            var restartBtn = ControlsFactory.CreateRestartButton();
            restartBtn.SetValue(GridControl.RowProperty, 0);
            restartBtn.SetValue(GridControl.ColumnProperty, 1);
            gridPanel.Add(restartBtn);

            // Selection option
            var selectionBtn = ControlsFactory.CreateSelectionButton();
            selectionBtn.SetValue(GridControl.RowProperty, 0);
            selectionBtn.SetValue(GridControl.ColumnProperty, 2);
            gridPanel.Add(selectionBtn);

            // Scene behavior
            this.AddSceneBehavior(new DebugSceneBehavior(), SceneBehavior.Order.PostUpdate);
        }
    }
}
