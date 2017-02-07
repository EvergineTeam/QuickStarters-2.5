#region Using Statements
using System;
using System.Collections.Generic;
using SuperSlingshot.Behaviors;
using SuperSlingshot.Components;
using SuperSlingshot.Managers;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Common.Physics2D;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics2D;
using WaveEngine.Framework.Services;
using WaveEngine.ImageEffects;
using WaveEngine.TiledMap;
using System.Linq;
using SuperSlingshot.Drawables;
using WaveEngine.Components.Graphics3D;
#endregion

namespace SuperSlingshot.Scenes
{
    public class GameScene : Scene
    {
        private TiledMap tiledMap;
        private string boulderOrder;
        private Trajectory2DDrawable trajectoryDrawable;
        private ElasticBandsDrawable bandDrawable;

        public string Content { get; private set; }

        public int BlockDestroyPoints { get; set; }

        public int BlockHitPoints { get; set; }

        public int GemPoints { get; set; }

        public int NumBreakables { get; set; }

        public int NumGems { get; set; }

        public LevelScore Score { get; set; }

        public Entity SlingshotAnchorEntity { get; set; }

        public string LevelID { get; private set; }

        public Queue<Entity> Boulders { get; private set; }

        public bool HasNextBouder
        {
            get
            {
                return this.Boulders.Count > 0;
            }
        }

        public GameScene(string content) : base()
        {
            this.Content = content;
            this.Boulders = new Queue<Entity>();
            this.Score = new LevelScore();
        }

        protected override void CreateScene()
        {
            this.Load(this.Content);
        }

        protected override void Start()
        {
            base.Start();

            this.InitializeScene();
            this.SetCameraBounds();
            this.CreatePhysicScene();
            this.CreateCratesScene();
            this.GetLevelProperties();
            this.CreateBoulderEntities();
            this.CreateGemsScene();
            this.CreateTrajectory(this.SlingshotAnchorEntity);
            this.CreateElasticBands(this.SlingshotAnchorEntity);

            this.NumBreakables = this.EntityManager.FindAllByTag(GameConstants.TAGBREAKABLE).Count();
            this.NumGems = this.EntityManager.FindAllByTag(GameConstants.TAGBONUS).Count();

            // Prepare to Play
            WaveServices.GetService<GamePlayManager>().NextBoulder();

            // TODO: Workaround, remove when fixed (do not store LocalDrawOrder of layers in WaveEditor)
            this.EntityManager.Find(GameConstants.ENTITYTILEDMAP).FindChild(GameConstants.LAYERMIDDLE).FindComponent<Transform2D>().LocalDrawOrder = 0;
        }

        private void InitializeScene()
        {
            WaveServices.GetService<GamePlayManager>().InitGame();
        }

        private void CreateCratesScene()
        {
            // crates of the layer
            if (this.tiledMap.ObjectLayers.ContainsKey(GameConstants.LAYERBOXPHYSICLAYER))
            {
                var physicLayer = this.tiledMap.ObjectLayers[GameConstants.LAYERBOXPHYSICLAYER];
                foreach (var physic in physicLayer.Objects)
                {
                    var colliderEntity = TiledMapUtils.CollisionEntityFromObject(physic.Name, physic);
                    colliderEntity.Tag = GameConstants.TAGCOLLIDER;
                    colliderEntity.AddComponent(new RigidBody2D());

                    var collider = colliderEntity.FindComponent<Collider2D>(false);
                    if (collider != null)
                    {
                        collider.CollisionCategories = ColliderCategory2D.Cat4;
                        collider.CollidesWith = ColliderCategory2D.All;
                        collider.Friction = 1.0f;
                        collider.Restitution = 0.4f;
                    }

                    Sprite sprite = new Sprite(WaveContent.Assets.Other.crate_png);
                    SpriteRenderer spriteRenderer = new SpriteRenderer(DefaultLayers.Alpha, AddressMode.PointWrap);

                    colliderEntity.AddComponent(sprite);
                    colliderEntity.AddComponent(spriteRenderer);

                    this.EntityManager.Add(colliderEntity);
                }
            }
        }

        private void CreateGemsScene()
        {
            // crates of the layer
            if (this.tiledMap.ObjectLayers.ContainsKey(GameConstants.LAYERBONUS))
            {
                var bonusLayer = this.tiledMap.ObjectLayers[GameConstants.LAYERBONUS];
                foreach (var gem in bonusLayer.Objects)
                {
                    var colliderEntity = TiledMapUtils.CollisionEntityFromObject(gem.Name, gem);
                    colliderEntity.Tag = GameConstants.TAGBONUS;
                    colliderEntity.AddComponent(new RigidBody2D()
                    {
                        PhysicBodyType = RigidBodyType2D.Static
                    });

                    var collider = colliderEntity.FindComponent<Collider2D>(false);
                    if (collider != null)
                    {
                        collider.IsSensor = true;
                        collider.CollisionCategories = ColliderCategory2D.Cat5;
                        collider.CollidesWith = ColliderCategory2D.All;
                    }

                    Sprite sprite = new Sprite(WaveContent.Assets.Gui.gem_png);
                    SpriteRenderer spriteRenderer = new SpriteRenderer(DefaultLayers.Alpha, AddressMode.PointWrap);

                    colliderEntity.AddComponent(sprite);
                    colliderEntity.AddComponent(spriteRenderer);
                    colliderEntity.AddComponent(new BonusComponent());

                    this.EntityManager.Add(colliderEntity);
                }
            }
        }

        public bool HasBreakables()
        {
            var breakables = this.EntityManager.FindAllByTag(GameConstants.TAGBREAKABLE).OfType<Entity>().Select(b => b.FindComponent<BreakableBehavior>());
            var count = breakables.Count(b => b.State != Enums.BreakableState.DEAD);
            return count > 0;
        }

        public void PrepareNextBoulder()
        {
            var boulder = this.Boulders.Dequeue();
            this.EntityManager.Add(boulder);

            var playerComponent = boulder.FindComponent<PlayerComponent>();
            playerComponent.PrepareToLaunch();
        }

        public override void Pause()
        {
            base.Pause();
            this.ConfigureLens(true);
        }

        public override void Resume()
        {
            base.Resume();
            this.ConfigureLens(false);
        }

        private void ConfigureLens(bool enable)
        {
            this.SetLens<VignetteLens>(enable);
            this.SetLens<GaussianBlurLens>(enable);
        }

        private void SetLens<T>(bool enable) where T : Lens
        {
            var camera = this.RenderManager.ActiveCamera2D.Owner;
            var lens = camera.FindComponent<T>();
            if (lens != null)
            {
                lens.Active = enable;
            }
        }

        private void SetCameraBounds()
        {
            var cameraBehavior = this.RenderManager.ActiveCamera2D.Owner.FindComponent<CameraBehavior>(false);
            if (cameraBehavior != null)
            {
                cameraBehavior.SetLimits(this.GetAnchorPosition(GameConstants.ANCHORTOPLEFT),
                    this.GetAnchorPosition(GameConstants.ANCHORBOTTOMRIGHT));
            }
        }

        private Vector2 GetAnchorPosition(string anchorName)
        {
            this.tiledMap = this.EntityManager.Find(GameConstants.ENTITYMAP).FindComponent<TiledMap>();
            var anchorsLayer = this.tiledMap.ObjectLayers[GameConstants.LAYERANCHOR];
            var anchorObject = anchorsLayer.Objects.Find(o => o.Name == anchorName);
            var anchorposition = new Vector2(anchorObject.X, anchorObject.Y);
            return anchorposition;
        }

        private void CreateTrajectory(Entity host)
        {
            this.trajectoryDrawable = new Trajectory2DDrawable()
            {
                IsVisible = false,
                NumberOfPoints = 70,
                CurveWith = 6.0f,
                VisibilityPercentage = 1,
            };


            var matsMap = new MaterialsMap() { DefaultMaterialPath = WaveContent.Materials.TrajectoryMaterial };
            matsMap.MaterialsPath.Add("elastic", WaveContent.Materials.ElasticMaterial);
            host.AddComponent(matsMap);

            host.AddComponent(this.trajectoryDrawable);
        }

        private void CreateElasticBands(Entity host)
        {
            this.bandDrawable = new ElasticBandsDrawable()
            {
                IsVisible = false,
                FixedPointATop = this.GetAnchorPosition(GameConstants.ANCHORFRONTBANDTOP),
                FixedPointABottom = this.GetAnchorPosition(GameConstants.ANCHORFRONTBANDBOTTOM),
                FixedPointBTop = this.GetAnchorPosition(GameConstants.ANCHORBACKBANDTOP),
                FixedPointBBottom = this.GetAnchorPosition(GameConstants.ANCHORBACKBANDBOTTOM),
                ZOrderA = -100,
                ZOrderB = 100
            };

            // host.AddComponent(new MaterialsMap() { DefaultMaterialPath = WaveContent.Materials.ElasticMaterial });
            host.AddComponent(this.bandDrawable);
        }

        public void PreviewTrajectory(Vector2 shotImpulse)
        {
            this.trajectoryDrawable.DesiredVelocity = shotImpulse;
            if (shotImpulse == Vector2.Zero)
            {
                this.trajectoryDrawable.IsVisible = false;
            }
            else
            {
                this.trajectoryDrawable.IsVisible = true;
            }
        }

        public void PreviewElasticBands(bool visible, Transform2D target)
        {
            this.bandDrawable.IsVisible = visible;

            this.bandDrawable.TargetTransform = target;
        }

        private void CreatePhysicScene()
        {
            // invisible slingshot anchor entity
            this.SlingshotAnchorEntity = new Entity()
                .AddComponent(new Transform2D()
                {
                    Position = this.GetAnchorPosition(GameConstants.ANCHORSLINGSHOT),
                })
                .AddComponent(new RigidBody2D()
                {
                    PhysicBodyType = RigidBodyType2D.Static
                });
            this.EntityManager.Add(this.SlingshotAnchorEntity);

            // invisible physic ground and walls
            var physicLayer = this.tiledMap.ObjectLayers[GameConstants.LAYERPHYSIC];
            foreach (var physic in physicLayer.Objects)
            {
                var colliderEntity = TiledMapUtils.CollisionEntityFromObject(physic.Name, physic);
                colliderEntity.Tag = GameConstants.TAGCOLLIDER;
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

        private void GetLevelProperties()
        {
            var props = this.tiledMap.Properties;
            this.LevelID = props[GameConstants.PROPERTYLEVELID];
            this.boulderOrder = props[GameConstants.PROPERTYBOULDERS];
            this.BlockHitPoints = Convert.ToInt32(props[GameConstants.BLOCKHITPOINTS]);
            this.BlockDestroyPoints = Convert.ToInt32(props[GameConstants.BLOCKDESTROYPOINTS]);
            this.GemPoints = Convert.ToInt32(props[GameConstants.GEMPOINTS]);
        }

        private void CreateBoulderEntities()
        {
            int counter = 0;
            foreach (var character in this.boulderOrder)
            {
                string assetPath = string.Empty;
                switch (character)
                {
                    case GameConstants.BOULDERBARNEY:
                        assetPath = WaveContent.Prefabs.Boulders.Barney;
                        break;
                    case GameConstants.BOULDERFARLEY:
                        assetPath = WaveContent.Prefabs.Boulders.Farney;
                        break;
                    case GameConstants.BOULDERPUDDIN:
                        assetPath = WaveContent.Prefabs.Boulders.Puddin;
                        break;
                    default:
                        assetPath = WaveContent.Prefabs.Boulders.Barney;
                        break;
                }


                if (!string.IsNullOrEmpty(assetPath))
                {
                    var boulder = this.EntityManager.Instantiate(assetPath);

                    boulder.FindComponent<Transform2D>().DrawOrder = 0;

                    // Must not collide names
                    boulder.Name += counter++.ToString();

                    this.Boulders.Enqueue(boulder);
                }
            }
        }
    }
}