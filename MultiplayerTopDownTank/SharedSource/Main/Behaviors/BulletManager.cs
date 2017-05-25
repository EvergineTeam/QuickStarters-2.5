using System.Collections.Generic;
using System.Runtime.Serialization;
using WaveEngine.Framework;
using System;
using System.Collections.Concurrent;
using WaveEngine.Framework.Physics2D;
using WaveEngine.Framework.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Networking;
using MultiplayerTopDownTank.Components;
using WaveEngine.Framework.Diagnostic;

namespace MultiplayerTopDownTank.Managers
{
    [DataContract]
    public class BulletManager : Behavior
    {
        private Queue<Entity> bulletPool;
        private ConcurrentQueue<Entity> toRemoveBulletPool;

        private GameScene gameScene;

        [DataMember]
        public int BulletPoolSize { get; set; }

        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            this.gameScene = Owner.Scene as GameScene;
            this.bulletPool = new Queue<Entity>();
            this.toRemoveBulletPool = new ConcurrentQueue<Entity>();

            this.InitBulletPool();
        }

        private void InitBulletPool()
        {
            this.bulletPool.Clear();
            for (int i = 0; i < this.BulletPoolSize; i++)
            {
                Entity bullet = CreateBullet();
                AddToPool(bullet);
            }
        }

        private Entity CreateBullet()
        {
            var bullet = new Entity() { Tag = GameConstants.BulletTag }
                .AddComponent(new Transform2D
                {
                    Origin = Vector2.Center,
                    X = 0,
                    Y = 0,
                    DrawOrder = 0.6f,
                })
                .AddComponent(new RigidBody2D
                {
                    PhysicBodyType = WaveEngine.Common.Physics2D.RigidBodyType2D.Dynamic,
                    IsBullet = true,
                    LinearDamping = 0
                })
                .AddComponent(new CircleCollider2D
                {
                    CollisionCategories = WaveEngine.Common.Physics2D.ColliderCategory2D.Cat2,
                    CollidesWith =
                        WaveEngine.Common.Physics2D.ColliderCategory2D.Cat1 |
                        WaveEngine.Common.Physics2D.ColliderCategory2D.Cat3 |
                        WaveEngine.Common.Physics2D.ColliderCategory2D.Cat4
                })
                .AddComponent(new Sprite(WaveContent.Assets.Textures.Bullets.rounded_bulletBeige_outline_png))
                .AddComponent(new SpriteRenderer(DefaultLayers.Alpha))
                .AddComponent(new BulletComponent())
                .AddComponent(new NetworkBehavior())
                .AddComponent(new BulletNetworkSyncComponent());

            return bullet;
        }

        private void AddToPool(Entity bullet)
        {
            bullet.Tag = GameConstants.BulletTag;
            bulletPool.Enqueue(bullet);
        }

        public Entity Retrieve()
        {
            Entity bullet;

            if (bulletPool.Count > 0)
            {
                bullet = bulletPool.Dequeue();
            }
            else
            {
                bullet = CreateBullet();
            }

            return bullet;
        }

        public void FreeBulletEntity(IEnumerable<Entity> collection)
        {
            this.FreeEntity(collection);
        }

        private void FreeEntity(IEnumerable<Entity> collection)
        {
            foreach (var bullet in collection)
            {
                toRemoveBulletPool.Enqueue(bullet);
            }
        }

        protected override void Update(TimeSpan gameTime)
        {
            foreach (var bullet in toRemoveBulletPool)
            {
                EntityManager.Detach(bullet);
                bulletPool.Enqueue(bullet);
            }

            Entity entity;
            while (!toRemoveBulletPool.IsEmpty)
            {
                toRemoveBulletPool.TryDequeue(out entity);
            }

            Labels.Add("Bullets", bulletPool.Count);
        }
    }
}
