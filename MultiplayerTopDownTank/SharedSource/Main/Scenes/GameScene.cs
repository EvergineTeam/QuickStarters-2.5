#region Using Statements
using MultiplayerTopDownTank.Behaviors;
using MultiplayerTopDownTank.Components;
using MultiplayerTopDownTank.Entities;
using WaveEngine.Common.Math;
using WaveEngine.Common.Media;
using WaveEngine.Common.Physics2D;
using WaveEngine.Framework;
using WaveEngine.Framework.Managers;
using WaveEngine.Framework.Physics2D;
using WaveEngine.Framework.Services;
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
            this.CreatePhysicScene();
            this.CreateBackgroundMusic();
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

            CreateHub();
        }

        private void CreateHub()
        {
            // Create Hub
            var hubPanel = new Hub();
            EntityManager.Add(hubPanel);
        }

        private void CreatePhysicScene()
        {
            // Invisible physic walls
            var physicLayer = this.tiledMap.ObjectLayers[GameConstants.PhysicLayer];
            foreach (var physic in physicLayer.Objects)
            {
                var colliderEntity = TiledMapUtils.CollisionEntityFromObject(physic.Name, physic);
                colliderEntity.Tag = GameConstants.TagCollider;
                colliderEntity.AddComponent(new RigidBody2D() { PhysicBodyType = RigidBodyType2D.Static });

                var collider = colliderEntity.FindComponent<Collider2D>(false);
                if (collider != null)
                {
                    collider.CollisionCategories = ColliderCategory2D.Cat3;
                    collider.CollidesWith = ColliderCategory2D.All;
                    collider.Friction = 1.0f;
                    collider.Restitution = 0.2f;
                }

                this.EntityManager.Add(colliderEntity);
            }
        }

        private void CreateBackgroundMusic()
        {
            // Music
            var musicInfo = new MusicInfo(WaveContent.Assets.Sounds.Background_Music_mp3);
            WaveServices.MusicPlayer.Play(musicInfo);
            WaveServices.MusicPlayer.Volume = 0.8f;
            WaveServices.MusicPlayer.IsRepeat = true;
        }
    }
}