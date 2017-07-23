using System;
using System.Collections.Generic;
using System.Text;
using P2PTank.Behaviors;
using P2PTank.Behaviors.Cameras;
using P2PTank.Components;
using P2PTank.Managers;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Common.Physics2D;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics2D;
using WaveEngine.TiledMap;

namespace P2PTank.Scenes
{
    public class TestScene : Scene
    {
        private string contentPath;

        public TestScene(string contentPath)
        {
            this.contentPath = contentPath;
        }

        protected override void CreateScene()
        {
            this.Load(this.contentPath);

#if DEBUG
            var debugEntity = new Entity()
                .AddComponent(new DebugBehavior());
            this.EntityManager.Add(debugEntity);
#endif
        }

        private void ConfigurePhysics()
        {
            this.PhysicsManager.Simulation2D.Gravity = Vector2.Zero;
        }

        private void CreateBorders(Entity tiledEntity, ColliderCategory2D category, ColliderCategory2D collidesWith)
        {
            var tiledMap = tiledEntity.FindComponent<TiledMap>();
            var borders = tiledMap.ObjectLayers[GameConstants.TiledMapBordersLayerName];
            foreach (var border in borders.Objects)
            {
                var colliderEntity = TiledMapUtils.CollisionEntityFromObject(border.Name, border);
                colliderEntity.Tag = GameConstants.TagCollider;
                colliderEntity.AddComponent(new RigidBody2D() { PhysicBodyType = RigidBodyType2D.Static });

                var collider = colliderEntity.FindComponent<Collider2D>(false);
                if (collider != null)
                {
                    collider.CollisionCategories = category;
                    collider.CollidesWith = collidesWith;
                    collider.Friction = 1.0f;
                    collider.Restitution = 0.2f;
                }

                tiledEntity.AddChild(colliderEntity);
            }
        }

        protected override void Start()
        {
            base.Start();

            var gameplayManager = this.EntityManager.FindComponentFromEntityPath<GamePlayManager>(GameConstants.ManagerEntityPath);

            ///// Doing this code here cause in CreateScene doesnt load tiledMap file still
            
            var tiledEntity = this.EntityManager.Find(GameConstants.MapEntityPath);

            this.ConfigurePhysics();
            this.CreateBorders(tiledEntity, ColliderCategory2D.Cat3, ColliderCategory2D.All);
            /////

            /// Create Player
            var player = gameplayManager.CreatePlayer(0, ColliderCategory2D.Cat1, ColliderCategory2D.All);
            player.FindComponent<Transform2D>().LocalPosition = this.GetSpawnPoint(0);
            this.EntityManager.Add(player);

            var foe1 = gameplayManager.CreateFoe(1, ColliderCategory2D.Cat1, ColliderCategory2D.All);
            var foe2 = gameplayManager.CreateFoe(2, ColliderCategory2D.Cat1, ColliderCategory2D.All);
            var foe3 = gameplayManager.CreateFoe(3, ColliderCategory2D.Cat1, ColliderCategory2D.All);
            foe1.FindComponent<Transform2D>().LocalPosition = this.GetSpawnPoint(1);
            foe2.FindComponent<Transform2D>().LocalPosition = this.GetSpawnPoint(2);
            foe3.FindComponent<Transform2D>().LocalPosition = this.GetSpawnPoint(3);
            this.EntityManager.Add(foe1);
            this.EntityManager.Add(foe2);
            this.EntityManager.Add(foe3);

            /// Set camera to follow player
            var targetCameraBehavior = new TargetCameraBehavior();
            targetCameraBehavior.SetTarget(player.FindComponent<Transform2D>());
            targetCameraBehavior.Follow = true;
            targetCameraBehavior.Speed = 5;
            this.RenderManager.ActiveCamera2D.Owner.AddComponent(targetCameraBehavior);
        }

        private Vector2 GetSpawnPoint(int index)
        {
            Vector2 res = Vector2.Zero;
            var entity = this.EntityManager.Find(string.Format(GameConstants.SpawnPointPathFormat, index));

            if(entity!=null)
            {
                res = entity.FindComponent<Transform2D>().LocalPosition;
            }

            return res;
        }
    }
}
