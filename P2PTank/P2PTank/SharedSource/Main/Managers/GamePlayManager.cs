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
using P2PTank.Services;
using WaveEngine.Framework.Services;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Components.Animation;

namespace P2PTank.Managers
{
    [DataContract]
    public class GamePlayManager : Behavior
    {
        private const int NUMEXPLOSIONS = 5;

        private class BulletState
        {
            public Entity bullet;
            public Vector2 position;
            public Vector2 direction;
            public bool isLocal;
        }

        private CURRENTSCENETYPE gamePlayScene;
        private PoolComponent poolComponent;

        private string playerID;

        private List<Entity> tanksToRemove = new List<Entity>();
        private List<Entity> tanksToAdd = new List<Entity>();
        private List<Entity> bulletsToRemove = new List<Entity>();
        private List<BulletState> bulletsToAdd = new List<BulletState>();
        private Entity[] explosions;
        private int explodeIndex;

        private int ExplodeIndex
        {
            get
            {
                explodeIndex = ++explodeIndex % NUMEXPLOSIONS;
                return explodeIndex;
            }
        }

        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();
            this.gamePlayScene = this.Owner.Scene as CURRENTSCENETYPE;
            this.poolComponent = this.gamePlayScene.EntityManager.FindComponentFromEntityPath<PoolComponent>(GameConstants.ManagerEntityPath);

            this.explosions = new Entity[NUMEXPLOSIONS];

            for (int i = 0; i < NUMEXPLOSIONS; i++)
            {
                this.explosions[i] = this.CreateExplosion();
            }
        }

        private Entity CreateExplosion()
        {
            var explode = new Entity() { IsSerializable = false }
                    .AddComponent(new Transform2D() { Origin = Vector2.Center, XScale = 2, YScale = 2 })
                    .AddComponent(new SpriteAtlas(WaveContent.Assets.Textures.ExplodeSprite_spritesheet))
                    .AddComponent(new SpriteAtlasRenderer() { LayerType = DefaultLayers.Additive })
                    .AddComponent(new Animation2D() { CurrentAnimation = "explosion", PlayAutomatically = false });

            explode.Enabled = false;

            var anim2D = explode.FindComponent<Animation2D>();
            this.Owner.AddChild(explode);

            return explode;
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

        public void CreateFoe(int playerIndex, P2PManager peerManager, string foeID, Color color, Vector2 position)
        {
            Labels.Add("foeID", foeID);
            var category = ColliderCategory2D.Cat4;
            var collidesWith = ColliderCategory2D.Cat1 | ColliderCategory2D.Cat2 | ColliderCategory2D.Cat3;

            var entity = this.CreateBaseTank(playerIndex, category, collidesWith);
            entity.Name = foeID;
            entity.AddComponent(new NetworkInputBehavior(peerManager) { PlayerID = foeID });
            entity.FindComponent<Transform2D>().LocalPosition = position;

            entity.FindComponent<TankComponent>().Color = color;

            this.tanksToAdd.Add(entity);
        }

        public async void ShootPlayerBullet(Vector2 position, Vector2 direction, Color color, P2PManager peerManager)
        {
            var category = ColliderCategory2D.Cat2;
            var collidesWith = ColliderCategory2D.Cat3 | ColliderCategory2D.Cat4;

            var entity = this.CreateBaseBullet(category, collidesWith, color);
            var bulletID = Guid.NewGuid().ToString();

            // Player Bullet Behavior should activate
            var bulletBehavior = entity.FindComponent<BulletBehavior>();
            if (bulletBehavior == null)
            {
                bulletBehavior = new BulletBehavior(peerManager, bulletID, this.playerID);
                entity.AddComponent(bulletBehavior);
            }
            else
            {
                bulletBehavior.BulletID = bulletID;
                bulletBehavior.PlayerID = this.playerID;
            }
            bulletBehavior.IsActive = true;

            // Deactivate network behavior for this bullet
            var bulletNetworkBehavior = entity.FindComponent<BulletNetworkBehavior>();
            if (bulletNetworkBehavior != null)
            {
                bulletNetworkBehavior.IsActive = false;
            }


            entity.Name = bulletID;

            this.bulletsToAdd.Add(new BulletState() { bullet = entity, direction = direction, position = position, isLocal = true });

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

            // Activate network behavior for this bullet
            var bulletNetworkBehavior = entity.FindComponent<BulletNetworkBehavior>();
            if (bulletNetworkBehavior == null)
            {
                bulletNetworkBehavior = new BulletNetworkBehavior(peerManager, bulletID, playerID);
                entity.AddComponent(bulletNetworkBehavior);
            }
            else
            {
                bulletNetworkBehavior.BulletID = bulletID;
                bulletNetworkBehavior.PlayerID = this.playerID;
            }
            bulletNetworkBehavior.IsActive = true;

            // deactivate player behavior for this bullet
            var bulletBehavior = entity.FindComponent<BulletBehavior>();
            if (bulletBehavior != null)
            {
                bulletBehavior.IsActive = false;
            }

            this.bulletsToAdd.Add(new BulletState() { bullet = entity, isLocal = false });

            var audioService = WaveServices.GetService<AudioService>();
            audioService.Play(Audio.Sfx.Gun_wav);

            return entity;
        }

        public void DestroyTank(Entity tank)
        {
            var transform = tank.FindComponent<Transform2D>();
            this.ExplodeTank(transform);
            this.tanksToRemove.Add(tank);
        }

        public void ExplodeTank(Transform2D transform)
        {
            var explode = this.explosions[this.ExplodeIndex];
            var transf = explode.FindComponent<Transform2D>();
            transf.X = transform.X;
            transf.Y = transform.Y;
            transf.Rotation = (float)WaveServices.Random.NextDouble();
            explode.IsActive = explode.IsVisible = true;
            var anim2D = explode.FindComponent<Animation2D>();
            anim2D.PlayAnimation("explosion", false);
        }

        public async void DestroyBullet(Entity bullet, P2PManager peerManager)
        {
            if (bullet == null)
            {
                return;
            }

            // Clean Bullet Entity
            bullet.RemoveComponent<BulletBehavior>();
            bullet.RemoveComponent<BulletNetworkBehavior>();

            this.bulletsToRemove.Add(bullet);

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

            var index = WaveServices.Random.Next(0, GameConstants.Palette.Count());
            tankComponent.Color = GameConstants.Palette[index];

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

            var rigidBody = entity.FindComponent<RigidBody2D>();

            if (rigidBody == null)
            {
                entity.AddComponent(new RigidBody2D
                {
                    PhysicBodyType = RigidBodyType2D.Dynamic,
                    IsBullet = true,

                    FixedRotation = true,
                    AngularDamping = 0,
                    LinearDamping = 0
                });
            }

            return entity;
        }

        protected override void Update(TimeSpan gameTime)
        {
            var audioService = WaveServices.GetService<AudioService>();

            // Removes
            if (this.bulletsToRemove.Count > 0)
            {
                this.poolComponent.FreeBulletEntity(this.bulletsToRemove);
                this.bulletsToRemove.Clear();
                audioService.Play(Audio.Sfx.BulletCollision_wav);
            }

            if (this.tanksToRemove.Count > 0)
            {
                foreach (var tank in this.tanksToRemove)
                {
                    this.EntityManager.Remove(tank);
                }

                audioService.Play(Audio.Sfx.Explosion_wav);

                this.tanksToRemove.Clear();
            }

            // Adds
            if (this.tanksToAdd.Count > 0)
            {
                foreach (var tank in this.tanksToAdd)
                {
                    if (!this.EntityManager.Contains(tank))
                    {
                        this.EntityManager.Add(tank);
                    }
                }

                this.tanksToAdd.Clear();
            }

            if (this.bulletsToAdd.Count > 0)
            {
                foreach (var bullet in this.bulletsToAdd)
                {
                    var entity = bullet.bullet;

                    if (!this.EntityManager.Contains(entity))
                    {
                        this.EntityManager.Add(entity);

                        if (bullet.isLocal)
                        {
                            entity.FindComponent<BulletBehavior>().Shoot(bullet.position, bullet.direction);
                        }
                    }
                }

                this.bulletsToAdd.Clear();
            }

            for (int i = 0; i < this.explosions.Length; i++)
            {
                if (this.explosions[i].FindComponent<Animation2D>().State == WaveEngine.Framework.Animation.AnimationState.Stopped)
                {
                    this.explosions[i].IsActive = this.explosions[i].IsVisible = false;
                }
            }
        }
    }
}
