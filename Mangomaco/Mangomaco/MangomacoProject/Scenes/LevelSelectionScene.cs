#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MangomacoProject.Factories;
using MangomacoProject.Services;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Components.Cameras;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Components.UI;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.UI;
#endregion

namespace MangomacoProject.Scenes
{
    /// <summary>
    /// Level Selection Scene
    /// </summary>
    public class LevelSelectionScene : Scene
    {
        /// <summary>
        /// Creates the scene.
        /// </summary>
        /// <remarks>
        /// This method is called before all <see cref="T:WaveEngine.Framework.Entity" /> instances in this instance are initialized.
        /// </remarks>
        protected override void CreateScene()
        {
            // Camera
            var camera = new FixedCamera2D("Camera2D")
                {
                    BackgroundColor = Resources.GameplayGBColor
                };
            EntityManager.Add(camera);

            // Background
            Entity background = new Entity()
                        .AddComponent(new Transform2D()
                        {
                            Origin = Vector2.Center,
                            X = WaveServices.ViewportManager.VirtualWidth / 2,
                            Y = WaveServices.ViewportManager.VirtualHeight / 2,
                            DrawOrder = 10,
                        })
                        .AddComponent(new Sprite(Resources.MenuBackground))
                        .AddComponent(new SpriteRenderer(DefaultLayers.Alpha));
            EntityManager.Add(background);

            // Panel
            var backPanel = ControlsFactory.CreateSelectionBgPanel();
            EntityManager.Add(backPanel);

            Grid gridPanel = new Grid()
            {
                DrawOrder = -1,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 20, 0, 0),
                Width = 670,
                Height = 418,
            };
            EntityManager.Add(gridPanel.Entity);

            gridPanel.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Proportional) });
            gridPanel.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(2, GridUnitType.Proportional) });
            gridPanel.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(2, GridUnitType.Proportional) });
            gridPanel.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Proportional) });

            gridPanel.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Proportional) });
            gridPanel.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Proportional) });
            gridPanel.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Proportional) });
            gridPanel.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Proportional) });
            gridPanel.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Proportional) });
            gridPanel.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Proportional) });
            gridPanel.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Proportional) });

            // Close option
            var resumeBtn = ControlsFactory.CreateCloseButton();
            resumeBtn.SetValue(GridControl.RowProperty, 0);
            resumeBtn.SetValue(GridControl.ColumnProperty, 6);
            gridPanel.Add(resumeBtn);

            var levels = WaveServices.GetService<LevelInfoService>().AvailableLevels;

            int levelIndex = 0;
            for (int row = 0; row < 2; row++)
            {
                for (int column = 0; column < 5; column++)
                {
                    Button levelBtn;

                    if (levels.Count() > levelIndex)
                    {
                        levelBtn = ControlsFactory.CreateLevelButton(levelIndex, false, levels.ElementAt(levelIndex));
                    }
                    else
                    {
                        levelBtn = ControlsFactory.CreateLevelButton(levelIndex, true, null);
                    }

                    levelBtn.SetValue(GridControl.RowProperty, 1 + row);
                    levelBtn.SetValue(GridControl.ColumnProperty, 1 + column);
                    gridPanel.Add(levelBtn);

                    levelIndex++;
                }
            }

#if DEBUG
            this.AddSceneBehavior(new DebugSceneBehavior(), SceneBehavior.Order.PostUpdate);
#endif
        }
    }
}
