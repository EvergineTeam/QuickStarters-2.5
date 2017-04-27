#region Using Statements
using MultiplayerTopDownTank.Behaviors;
using MultiplayerTopDownTank.Components;
using MultiplayerTopDownTank.Entities;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Managers;
using WaveEngine.TiledMap;
#endregion

namespace MultiplayerTopDownTank
{
    public class GameSceneScene : Scene
    {
        private TiledMap tiledMap;

        protected override void CreateScene()
        {
            this.Load(WaveContent.Scenes.GameScene);

            this.CreateUI();
        }

        protected override void Start()
        {
            base.Start();

            this.InitializePlayer();
            this.SetCameraBounds();
        }

        private void InitializePlayer()
        {
            var player = this.EntityManager.Find(GameConstants.Player);
            var playerComponent = player.FindComponent<TankComponent>();
            playerComponent.PrepareTank();
        }

        private void SetCameraBounds()
        {
            var cameraBehavior = this.RenderManager.ActiveCamera2D.Owner.FindComponent<CameraBehavior>(false);
            if (cameraBehavior != null)
            {
                this.tiledMap = this.EntityManager.Find(GameConstants.EntityMap).FindComponent<TiledMap>();
                cameraBehavior.SetLimits(
                    new Vector2(0, 0),
                    new Vector2(tiledMap.Width * tiledMap.TileWidth, tiledMap.Height * tiledMap.TileHeight));
            }
        }
        
        private void CreateUI()
        {
            // Left Joystick
            RectangleF leftArea = new RectangleF(
                0,             
                0,
                this.VirtualScreenManager.VirtualWidth / 2f,
                this.VirtualScreenManager.VirtualHeight);
            var leftJoystick = new Joystick("leftJoystick", leftArea);
            EntityManager.Add(leftJoystick);

            // Right Joystick
            RectangleF rightArea = new RectangleF(
                this.VirtualScreenManager.VirtualWidth / 2,
                0,
                this.VirtualScreenManager.VirtualWidth / 2f,
                this.VirtualScreenManager.VirtualHeight);
            var rightJoystick = new Joystick("rightJoystick", rightArea);
            EntityManager.Add(rightJoystick);
        }
    }
}