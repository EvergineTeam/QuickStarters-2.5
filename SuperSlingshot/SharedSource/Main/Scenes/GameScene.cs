#region Using Statements
using System;
using SuperSlingshot.Behaviors;
using WaveEngine.Common.Math;
using WaveEngine.Common.Physics2D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics2D;
using WaveEngine.TiledMap;
#endregion

namespace SuperSlingshot.Scenes
{
    public class GameScene : Scene
    {
        private readonly string content;

        private TiledMap tiledMap;

        private Entity slingshotAnchorEntity;

        public GameScene(string content) : base()
        {
            this.content = content;
        }

        protected override void CreateScene()
        {
            this.Load(this.content);
        }

        protected override void Start()
        {
            base.Start();
            this.SetCameraBounds();
            this.CreatePhysicScene();

            this.PrepareToLaunch(this.EntityManager.Find("sprite"));
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

        private void CreatePhysicScene()
        {
            //Entity sample = new Entity()
            //    .AddComponent(new Transform2D() { X = 200, Y = 1200 })
            //    .AddComponent(new EdgeCollider2D()
            //    {
            //        Vertices = new Vector2[] { new Vector2(0, 0), new Vector2(100, 0), new Vector2(0, 100) },
            //    })
            //    .AddComponent(new RigidBody2D());
            //this.EntityManager.Add(sample);


            // invisible slingshot anchor entity
            this.slingshotAnchorEntity = new Entity()
                .AddComponent(new Transform2D()
                {
                    Position = this.GetAnchorPosition(GameConstants.ANCHORSLINGSHOT),
                })
                .AddComponent(new RigidBody2D()
                {
                    PhysicBodyType = RigidBodyType2D.Static
                });
            this.EntityManager.Add(this.slingshotAnchorEntity);

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
                }

                this.EntityManager.Add(colliderEntity);
            }
        }

        private void PrepareToLaunch(Entity entity)
        {
            // Entity RigidBosy Restitution
            // 0.858 golf ball
            // 0.804 billiard ball
            // 0.712 tennis ball
            // 0.658 glass marble
            // 0.597 steel ball bearing

            var entityPath = entity.EntityPath;
            var joint = new DistanceJoint2D()
            {
                Distance = 0.01f,
                ConnectedEntityPath = entityPath,
                FrequencyHz = 7.0f,
                DampingRatio = 1f,
                CollideConnected = false,
            };

            //entity.FindComponent<Transform2D>().Position = this.slingshotAnchorEntity.FindComponent<Transform2D>().Position;
            //this.slingshotAnchorEntity.AddComponent(joint);

            var cameraBehavior = this.RenderManager.ActiveCamera2D.Owner.FindComponent<CameraBehavior>(false);
            if (cameraBehavior != null)
            {
                cameraBehavior.SetTarget(entityPath);
            }
        }
    }
}
