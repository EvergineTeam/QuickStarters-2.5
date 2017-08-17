using System;
using System.Collections.Generic;
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
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics2D;

using CURRENTSCENETYPE = P2PTank.Scenes.GamePlayScene;

namespace P2PTank.Managers
{
    [DataContract]
    public class GamePlayManager : Behavior
    {
        private CURRENTSCENETYPE gamePlayScene;
        private PoolComponent poolComponent;

        private string playerID;

        private List<Entity> tanksToRemove = new List<Entity>();
        private List<Entity> tanksToAdd = new List<Entity>();
        private List<Entity> bulletsToRemove = new List<Entity>();
        private List<Entity> bulletsToAdd = new List<Entity>();

        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();
            this.gamePlayScene = this.Owner.Scene as CURRENTSCENETYPE;
            this.poolComponent = this.gamePlayScene.EntityManager.FindComponentFromEntityPath<PoolComponent>(GameConstants.ManagerEntityPath);
        }

        public Entity CreatePlayer(int playerIndex, P2PManager peerManager, string playerID, Vector2 position)
        {
            this.playerID = playerID;

            var category = ColliderCategory2D.Cat1;
            var collidesWith = ColliderCategory2D.Cat3 | ColliderCategory2D.Cat4 | ColliderCategory2D.Cat5;

            var entity = this.CreateBaseTank(playerIndex, category, collidesWith);
            entity.Name = playerID;
            entity.AddComponent(new PlayerInputBehavior(peerManager, playerID))
                 .AddComponent(new RigidBody2D
                 {
                     AngularDamping = 8.0f,
                     LinearDamping = 9.0f,
                 });

            entity.FindComponent<Transform2D>().LocalPosition = position;

            this.tanksToAdd.Add(entity);

            return entity;
        }

        public void CreateFoe(int playerIndex, P2PManager peerManager, string foeID, Vector2 position)
        {
            Labels.Add("foeID", foeID);
            var category = ColliderCategory2D.Cat4;
            var collidesWith = ColliderCategory2D.Cat1 | ColliderCategory2D.Cat2 | ColliderCategory2D.Cat3;

            var entity = this.CreateBaseTank(playerIndex, category, collidesWith);
            entity.Name = foeID;
            entity.AddComponent(new NetworkInputBehavior(peerManager) { PlayerID = foeID });
            entity.FindComponent<Transform2D>().LocalPosition = position;

            this.tanksToAdd.Add(entity);
        }

        public async void ShootPlayerBullet(Vector2 position, Vector2 direction, Color color, P2PManager peerManager)
        {
            var category = ColliderCategory2D.Cat2;
            var collidesWith = ColliderCategory2D.Cat3 | ColliderCategory2D.Cat4;

            var entity = this.CreateBaseBullet(category, collidesWith, color);
            var bulletID = Guid.NewGuid().ToString();
            var behavior = new BulletBehavior(peerManager, bulletID, this.playerID);
            entity.AddComponent(behavior);

            entity.Name = bulletID;

            this.bulletsToAdd.Add(entity);
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
            this.bulletsToAdd.Add(entity);
            return entity;
        }

        public void DestroyTank(Entity tank)
        {
            this.tanksToRemove.Add(tank);
        }

        public async void DestroyBullet(Entity bullet, P2PManager peerManager)
        {
            this.bulletsToRemove.Add(bullet);

            var bulletCollider = bullet.FindComponent<Collider2D>(false);
            if (peerManager != null)
            {
                var destroyMessage = new BulletDestroyMessage()
                {
                    BulletId = bullet.Name,
                };

                await peerManager.SendBroadcastAsync(peerManager.CreateMessage(P2PMessageType.BulletDestroy, destroyMessage));
            }
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

        protected override void Update(TimeSpan gameTime)
        {
            // Removes
            if (this.bulletsToRemove.Count > 0)
            {
                this.poolComponent.FreeBulletEntity(this.bulletsToRemove);
                this.bulletsToRemove.Clear();
            }

            if (this.tanksToRemove.Count > 0)
            {
                foreach (var tank in this.tanksToRemove)
                {
                    this.EntityManager.Remove(tank);
                }

                this.tanksToRemove.Clear();
            }

            // Adds
            if(this.tanksToAdd.Count>0)
            {
                foreach (var tank in this.tanksToAdd)
                {
                    this.EntityManager.Add(tank);
                }

                this.tanksToAdd.Clear();
            }

            if (this.bulletsToAdd.Count > 0)
            {
                foreach (var bullet in this.bulletsToAdd)
                {
                    this.EntityManager.Add(bullet);
                }

                this.bulletsToAdd.Clear();
            }
        }
    }
}
