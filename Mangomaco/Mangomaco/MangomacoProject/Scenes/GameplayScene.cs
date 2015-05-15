#region Using Statements
using System;
using System.Linq;
using MangomacoProject.Components;
using MangomacoProject.Factories;
using MangomacoProject.Services;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Common.Media;
using WaveEngine.Components.Animation;
using WaveEngine.Components.Cameras;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Components.Transitions;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics2D;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.UI;
using WaveEngine.TiledMap;
#endregion

namespace MangomacoProject.Scenes
{
    /// <summary>
    /// Level Play Scene
    /// </summary>
    public class GameplayScene : Scene
    {
        /// <summary>
        /// The level definition
        /// </summary>
        private LevelDefinition levelDefinition;

        /// <summary>
        /// The tiled map
        /// </summary>
        private TiledMap tiledmap;

        /// <summary>
        /// The camera
        /// </summary>
        private FixedCamera2D camera;

        /// <summary>
        /// The gameplay scene behavior
        /// </summary>
        private GameplaySceneBehavior gameplaySceneBehavior;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameplayScene"/> class.
        /// </summary>
        /// <param name="levelDefinition">The level definition.</param>
        public GameplayScene(LevelDefinition levelDefinition)
        {
            this.levelDefinition = levelDefinition;
        }

        /// <summary>
        /// Creates the scene.
        /// </summary>
        /// <remarks>
        /// This method is called before all <see cref="T:WaveEngine.Framework.Entity" /> instances in this instance are initialized.
        /// </remarks>
        protected override void CreateScene()
        {
            // Camera
            this.camera = GameEntitiesFactory.CreateCamera();
            EntityManager.Add(this.camera);

            // Load TiledMap
            this.CreateTiledMap();

            // Create UI
            this.CreateMenuOverlay();
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

            this.AddCoins();
            this.AddCrates();
            this.AddTraps();
            this.AddSceneColliders();
            this.AddFinish();
            this.AddCharacter();            

            this.gameplaySceneBehavior = new GameplaySceneBehavior();
            this.AddSceneBehavior(this.gameplaySceneBehavior, SceneBehavior.Order.PostUpdate);

#if DEBUG
            this.AddSceneBehavior(new DebugSceneBehavior(), SceneBehavior.Order.PostUpdate);
#endif
        }

        /// <summary>
        /// Creates the tiled map.
        /// </summary>
        private void CreateTiledMap()
        {
            var map = new Entity("map")
                .AddComponent(new Transform2D())
                .AddComponent(this.tiledmap = new TiledMap(this.levelDefinition.TileMapPath)
                {
                    MinLayerDrawOrder = -10,
                    MaxLayerDrawOrder = 0
                });

            this.EntityManager.Add(map);
        }

        /// <summary>
        /// Creates the menu overlay.
        /// </summary>
        private void CreateMenuOverlay()
        {
            var pauseButton = ControlsFactory.CreatePauseButton();
            EntityManager.Add(pauseButton);
        }       

        /// <summary>
        /// Add Level Finish
        /// </summary>
        private void AddFinish()
        {
            var checkPointsLayer = this.tiledmap.ObjectLayers["CheckPoints"];
            var endObj = checkPointsLayer.Objects.First(obj => obj.Type == "Finish"); ;
            var endEntity = TiledMapUtils.CollisionEntityFromObject("Finish", endObj);

            this.EntityManager.Add(endEntity);
        }

        /// <summary>
        /// Add character
        /// </summary>
        private void AddCharacter()
        {
            var checkPointsLayer = this.tiledmap.ObjectLayers["CheckPoints"];
            var startObj = checkPointsLayer.Objects.First(obj => obj.Type == "Start");

            var tileRectangle = new RectangleF(0, 0, this.tiledmap.Width * this.tiledmap.TileWidth, this.tiledmap.Height * this.tiledmap.TileHeight);

            Entity character = GameEntitiesFactory.CreatePlayer(startObj);
            this.EntityManager.Add(character);

            this.camera.Entity.AddComponent(new CameraBehavior(character, tileRectangle));
        }

        /// <summary>
        /// Add coins
        /// </summary> 
        private void AddCoins()
        {
            var coinsLayer = this.tiledmap.ObjectLayers["Coins"];

            int i = 0;
            foreach (var obj in coinsLayer.Objects)
            {
                Entity coinEntity = GameEntitiesFactory.CreateCoin("coin_" + (i++), obj);

                this.EntityManager.Add(coinEntity);
            }
        }

        /// <summary>
        /// Add physics crates
        /// </summary>
        private void AddCrates()
        {
            var cratesLayer = this.tiledmap.ObjectLayers["Crates"];
            var sceneTileSet = this.tiledmap.Tilesets.First(ts => ts.Name == "sceneTiles");
            
            int i = 0;
            foreach (var obj in cratesLayer.Objects)
            {
                Entity crateEntity = GameEntitiesFactory.CreateCrate("crate_" + (i++), obj, sceneTileSet, 190);

                this.EntityManager.Add(crateEntity);
            }
        }

        /// <summary>
        /// Add traps
        /// </summary>
        private void AddTraps()
        {
            var collisionLayer = this.tiledmap.ObjectLayers["Traps"];

            int i = 0;
            foreach (var obj in collisionLayer.Objects)
            {
                var colliderEntity = TiledMapUtils.CollisionEntityFromObject("trap_" + (i++), obj);
                colliderEntity.Tag = "trap";

                this.EntityManager.Add(colliderEntity);
            }
        }

        /// <summary>
        /// Add Collision entities from TMX object layer
        /// </summary>
        private void AddSceneColliders()
        {
            var collisionLayer = this.tiledmap.ObjectLayers["Collisions"];

            int i = 0;
            foreach (var obj in collisionLayer.Objects)
            {
                var colliderEntity = TiledMapUtils.CollisionEntityFromObject("collider_" + (i++), obj);
                colliderEntity.Tag = "collider";

                colliderEntity.AddComponent(new RigidBody2D() 
                { 
                    PhysicBodyType = PhysicBodyType.Static
                });

                this.EntityManager.Add(colliderEntity);
            }
        }

        /// <summary>
        /// Resets the game.
        /// </summary>
        public void ResetGame()
        {
            this.gameplaySceneBehavior.ResetGame();
        }
    }
}
