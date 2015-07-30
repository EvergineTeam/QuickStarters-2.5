#region Using Statements
using SurvivorNinja.Behaviors;
using SurvivorNinja.Commons;
using SurvivorNinja.Components;
using SurvivorNinja.Entities;
using SurvivorNinja.Managers;
using System;
using WaveEngine.Common;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Common.Media;
using WaveEngine.Components;
using WaveEngine.Components.Cameras;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Components.Graphics3D;
using WaveEngine.Components.UI;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Resources;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.UI;
#endregion

namespace SurvivorNinja
{
    public class GamePlayScene : Scene
    {
        public PlayerBehavior Player
        {
            get
            {
                return this.EntityManager.Find("Player").FindComponent<PlayerBehavior>();
            }
        }

        public EnemyEmitter EnemyEmitter
        {
            get
            {
                return this.EntityManager.Find("EnemyEmitter").FindComponent<EnemyEmitter>();
            }
        }

        protected override void CreateScene()
        {
            this.Load(@"Content/Scenes/GamePlayScene.wscene");
            this.EntityManager.Find("defaultCamera2D").FindComponent<Camera2D>().CenterScreen();

            this.CreateUI();

            // Music
            var musicInfo = new MusicInfo(Directories.Sounds + "ninjaMusic.mp3");
            WaveServices.MusicPlayer.Play(musicInfo);
            WaveServices.MusicPlayer.Volume = 0.8f;
            WaveServices.MusicPlayer.IsRepeat = true;
        }

        private void CreateUI()
        {
            // Left Joystick
            RectangleF leftArea = new RectangleF(0,
                                                  0,
                                                  WaveServices.ViewportManager.VirtualWidth / 2f,
                                                  WaveServices.ViewportManager.VirtualHeight);
            var leftJoystick = new Joystick("leftJoystick", leftArea);
            EntityManager.Add(leftJoystick);

            // Right Joystick
            RectangleF rightArea = new RectangleF(WaveServices.ViewportManager.VirtualWidth / 2,
                                                  0,
                                                  WaveServices.ViewportManager.VirtualWidth / 2f,
                                                  WaveServices.ViewportManager.VirtualHeight);
            var rightJoystick = new Joystick("rightJoystick", rightArea);
            EntityManager.Add(rightJoystick);

            // BestScores
            var bestScores = new TextBlock("BestScores")
            {
                Width = 300,
                FontPath = Directories.Fonts + "Coalition_16.wpk",
                Text = string.Format("BestScores: {0}", 0),
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Bottom,
                TextAlignment = TextAlignment.Right,
                Margin = new Thickness(10),
            };

            EntityManager.Add(bestScores);

            // CreateHUBUI
            var hubPanel = new HubPanel();
            EntityManager.Add(hubPanel);
        }
    }
}
