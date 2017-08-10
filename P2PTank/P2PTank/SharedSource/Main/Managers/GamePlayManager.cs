using System;
using System.Linq;
using System.Runtime.Serialization;
using P2PTank.Behaviors;
using P2PTank.Components;
using P2PTank.Entities.P2PMessages;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Common.Physics2D;
using WaveEngine.Framework;
using WaveEngine.Framework.Diagnostic;
using WaveEngine.Framework.Physics2D;

using CURRENTSCENETYPE = P2PTank.Scenes.GamePlayScene;

namespace P2PTank.Managers
{
    [DataContract]
    public class GamePlayManager : Component
    {
        private CURRENTSCENETYPE gamePlayScene;
        private PoolComponent poolComponent;

        private string playerID;

        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();
            this.gamePlayScene = this.Owner.Scene as CURRENTSCENETYPE;
            this.poolComponent = this.gamePlayScene.EntityManager.FindComponentFromEntityPath<PoolComponent>(GameConstants.ManagerEntityPath);
        }

        public Entity CreatePlayer(int playerIndex, P2PManager peerManager, string playerID)
        {
            this.playerID = playerID;

            var category = ColliderCategory2D.Cat1;
            var collidesWith = ColliderCategory2D.Cat3 | ColliderCategory2D.Cat4 | ColliderCategory2D.Cat5;

            var entity = this.CreateBaseTank(playerIndex, category, collidesWith);
            entity.Name = playerID;
            entity.AddComponent(new PlayerInputBehavior(peerManager, playerID))
                 .AddComponent(new RigidBody2D
                 {
                     AngularDamping = 5.0f,
                     LinearDamping = 10.0f,
                 });
            return entity;
        }

        public Entity CreateFoe(int playerIndex, P2PManager peerManager, string foeID)
        {
            Labels.Add("foeID", foeID);
            var category = ColliderCategory2D.Cat4;
            var collidesWith = ColliderCategory2D.Cat1 | ColliderCategory2D.Cat2 | ColliderCategory2D.Cat3;

            var entity = this.CreateBaseTank(playerIndex, category, collidesWith);
            entity.Name = foeID;
            entity.AddComponent(new NetworkInputBehavior(peerManager) { PlayerID = foeID });
            return entity;
        }

        public async void ShootPlayerBullet(Vector2 position, Vector2 direction, Color color, P2PManager peerManager)
        {
            var category = ColliderCategory2D.Cat2;
            var collidesWith = ColliderCategory2D.Cat3 | ColliderCategory2D.Cat4;
            
            var entity = this.CreateBaseBullet(category, collidesWith, color);
            var bulletID = Guid.NewGuid().ToString();
            var behavior = new BulletBehavior(peerManager, bulletID, this.playerID);
            entity.AddComponent(behavior);

            this.gamePlayScene.EntityManager.Add(entity);
            behavior.Shoot(position, direction);

            this.gamePlayScene.AddActiveBullet(bulletID);

            if (peerManager != null)
            {
                var createBulletMessage = new BulletCreateMessage()
                {
                    BulletID = bulletID,
                    PlayerID = this.playerID,
                    Color = color,
                };

                await peerManager.SendBroadcastAsync(peerManager.CreateMessage(P2PMessageType.BulletCreate, createBulletMessage));
            }
        }

        public Entity CreateFoeBullet(Color color, string playerID, string bulletID, P2PManager peerManager)
        {
            var category = ColliderCategory2D.Cat5;
            var collidesWith = ColliderCategory2D.Cat1 | ColliderCategory2D.Cat3;

            var entity = this.CreateBaseBullet(category, collidesWith, color);

            entity.Name = bulletID;

            entity.AddComponent(new BulletNetworkBehavior(peerManager, bulletID, playerID));
            this.EntityManager.Add(entity);
            return entity;
        }

        public void DestroyTank(Entity tank)
        {
            this.EntityManager.Remove(tank);
        }

        public async void DestroyBullet(Entity bullet, P2PManager peerManager)
        {
            this.poolComponent.FreeBulletEntity(new Entity[] { bullet });

            var destroyMessage = new BulletDestroyMessage()
            {
                BulletId = bullet.Name,
            };

            await peerManager.SendBroadcastAsync(peerManager.CreateMessage(P2PMessageType.BulletDestroy, destroyMessage));
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

        private Entity CreateBaseBullet(ColliderCategory2D category, ColliderCategory2D collidesWith, Color color)
        {
            var entity = this.poolComponent.RetrieveBulletEntity();

            var component = entity.FindComponent<BulletComponent>();
            component.Color = color;

            var colliders = entity.FindComponentsInChildren<Collider2D>(false);
            var collider = colliders.FirstOrDefault();

            if (collider != null)
            {
                collider.CollisionCategories = category;
                collider.CollidesWith = collidesWith;
            }

            entity.AddComponent(new RigidBody2D
            {
                PhysicBodyType = RigidBodyType2D.Dynamic,
                IsBullet = true,

                FixedRotation = true,
                AngularDamping = 0,
                LinearDamping = 0
            });

            return entity;
        }
    }
}
