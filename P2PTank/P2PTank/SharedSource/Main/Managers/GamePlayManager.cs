using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using P2PTank.Behaviors;
using P2PTank.Components;
using P2PTank.Scenes;
using WaveEngine.Common.Math;
using WaveEngine.Common.Physics2D;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics2D;
using WaveEngine.Framework.Services;

using CURRENTSCENETYPE = P2PTank.Scenes.TestScene;

namespace P2PTank.Managers
{
    [DataContract]
    public class GamePlayManager : Component
    {
        private CURRENTSCENETYPE gamePlayScene;
        private PoolComponent poolComponent;

        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();
            this.gamePlayScene = this.Owner.Scene as CURRENTSCENETYPE;
            this.poolComponent = this.gamePlayScene.EntityManager.FindComponentFromEntityPath<PoolComponent>(GameConstants.ManagerEntityPath);
        }

        public Entity CreatePlayer(int playerIndex)
        {
            var category = ColliderCategory2D.Cat2;
            var collidesWith = ColliderCategory2D.Cat3 | ColliderCategory2D.Cat4 | ColliderCategory2D.Cat5;

            var entity = this.CreateBaseTank(playerIndex, category, collidesWith);
            entity.AddComponent(new PlayerInputBehavior())
                 .AddComponent(new RigidBody2D
                 {
                     AngularDamping = 5.0f,
                     LinearDamping = 10.0f,
                 });
            return entity;
        }

        public Entity CreateFoe(int playerIndex)
        {
            var category = ColliderCategory2D.Cat4;
            var collidesWith = ColliderCategory2D.Cat1 | ColliderCategory2D.Cat3 | ColliderCategory2D.Cat4 | ColliderCategory2D.Cat5;

            var entity = this.CreateBaseTank(playerIndex, category, collidesWith);
            entity.AddComponent(new NetworkInputBehavior());
            return entity;
        }

        public void ShootPlayerBullet(Vector2 position, Vector2 direction)
        {
            var category = ColliderCategory2D.Cat2;
            var collidesWith = ColliderCategory2D.Cat3 | ColliderCategory2D.Cat4;

            var entity = this.CreateBaseBullet(category, collidesWith);
            var behavior = new BulletBehavior();
            entity.AddComponent(behavior);
            entity.AddComponent(new RigidBody2D
            {
                PhysicBodyType = RigidBodyType2D.Dynamic,
                IsBullet = true,
                LinearDamping = 0
            });

            this.gamePlayScene.EntityManager.Add(entity);
            behavior.Shoot(position, direction);
            // return entity;
        }

        public Entity CreateFoeBullet()
        {
            var category = ColliderCategory2D.Cat5;
            var collidesWith = ColliderCategory2D.Cat1 | ColliderCategory2D.Cat3;

            var entity = this.CreateBaseBullet(category, collidesWith);
            entity.AddComponent(new BulletNetworkBehavior());
            return entity;
        }

        private Entity CreateBaseTank(int playerIndex, ColliderCategory2D category, ColliderCategory2D collidesWith)
        {
            var entity = this.EntityManager.Instantiate(WaveContent.Assets.Prefabs.tankPrefab);

            entity.Name += playerIndex;

            var tankComponent = entity.FindComponent<TankComponent>();
            tankComponent.Color = GameConstants.Palette[playerIndex];

            var colliders = entity.FindComponentsInChildren<Collider2D>(false);
            var collider = colliders.FirstOrDefault();

            if (collider != null)
            {
                collider.CollisionCategories = category;
                collider.CollidesWith = collidesWith;
            }
            return entity;
        }

        private Entity CreateBaseBullet(ColliderCategory2D category, ColliderCategory2D collidesWith)
        {
            var entity = this.poolComponent.RetrieveBulletEntity();
            var colliders = entity.FindComponentsInChildren<Collider2D>(false);
            var collider = colliders.FirstOrDefault();

            if (collider != null)
            {
                collider.CollisionCategories = category;
                collider.CollidesWith = collidesWith;
            }

            return entity;
        }
    }
}
