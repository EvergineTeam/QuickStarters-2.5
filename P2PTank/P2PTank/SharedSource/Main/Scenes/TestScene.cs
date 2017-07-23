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
        }

        private void ConfigurePhysics()
        {
            this.PhysicsManager.Simulation2D.Gravity = Vector2.Zero;
        }

        private void CreateBorders(TiledMap tiledMap, ColliderCategory2D category, ColliderCategory2D collidesWith)
        {
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

                this.EntityManager.Add(colliderEntity);
            }
        }

        protected override void Start()
        {
            base.Start();

            ///// Doing this code here cause in CreateScene doesnt load tiledMap file still
            var tiledMap = this.EntityManager.FindComponentFromEntityPath<TiledMap>("map");
            this.ConfigurePhysics();
            this.CreateBorders(tiledMap, ColliderCategory2D.Cat3, ColliderCategory2D.All);
            //////

            var gameplayManager = this.EntityManager.FindComponentFromEntityPath<GamePlayManager>(GameConstants.ManagerEntityPath);
            var player = gameplayManager.CreatePlayer();
            this.EntityManager.Add(player);

            var targetCameraBehavior = new TargetCameraBehavior();
            targetCameraBehavior.SetTarget(player.FindComponent<Transform2D>());
            targetCameraBehavior.Follow = true;
            targetCameraBehavior.Speed = 5;
            this.RenderManager.ActiveCamera2D.Owner.AddComponent(targetCameraBehavior);
        }
    }
}
