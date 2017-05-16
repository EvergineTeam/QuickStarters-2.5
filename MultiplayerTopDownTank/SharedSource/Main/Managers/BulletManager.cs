using MultiplayerTopDownTank.Entities;
using System.Collections.Generic;
using System.Runtime.Serialization;
using WaveEngine.Framework;

namespace MultiplayerTopDownTank.Managers
{
    [DataContract]
    public class BulletManager : Component
    {
        private Queue<Bullet> bulletPool;

        [DataMember]
        public int BulletPoolSize { get; set; }

        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            this.bulletPool = new Queue<Bullet>();

            this.InitBulletPool();
        }

        private void InitBulletPool()
        {
            this.bulletPool.Clear();
            for (int i = 0; i < this.BulletPoolSize; i++)
            {
                Bullet bullet = new Bullet();
                AddToPool(bullet);
            }
        }

        private void AddToPool(Bullet bullet)
        {
            bullet.Tag = GameConstants.BulletTag;
            bulletPool.Enqueue(bullet);

            bullet.IsBulletActive(false);
            EntityManager.Add(bullet);
        }

        public Bullet Retrieve()
        {
            Bullet bullet;

            if (bulletPool.Count > 0)
            {
                bullet = bulletPool.Dequeue();
            }
            else
            {
                bullet = new Bullet();
                bullet.Tag = GameConstants.BulletTag;
            }

            return bullet;
        }

        public void FreeBulletEntity(IEnumerable<Bullet> collection)
        {
            this.FreeEntity(collection);
        }

        private void FreeEntity(IEnumerable<Bullet> collection)
        {
            foreach (var bullet in collection)
            {
                if (bullet.Entity.Parent != null)
                {
                    bullet.Entity.Parent.DetachChild(bullet.Entity.Name);
                    bulletPool.Enqueue(bullet);
                }
            }
        }
    }
}
