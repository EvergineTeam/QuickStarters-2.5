using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using WaveEngine.Framework;
using WaveEngine.Framework.Services;

namespace P2PTank.Components
{
    [DataContract]
    public class PoolComponent : Component
    {
        private static int bulletIndex = 0;
        private string[] bulletPrefabPaths;

        private Queue<Entity> bulletPool;

        [DataMember]
        public int BulletPoolSize { get; set; }

        protected override void DefaultValues()
        {
            base.DefaultValues();

            this.BulletPoolSize = 50;
        }

        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            this.bulletPool = new Queue<Entity>();

            this.bulletPrefabPaths = new string[]
            {
                WaveContent.Assets.Prefabs.bulletPrefab,
            };

            this.FillPool();
        }

        public void FillPool()
        {
            for (int i = 0; i < this.BulletPoolSize; i++)
            {
                var prefab = this.bulletPrefabPaths[0];
                this.AddToPool(prefab, this.bulletPool);
            }
        }

        private void AddToPool(string prefab, Queue<Entity> pool)
        {
            var entity = this.InstantiatePrefab(prefab, ref bulletIndex);

            pool.Enqueue(entity);
        }

        public Entity RetrieveBulletEntity()
        {
            return this.Retrieve(this.bulletPool, this.bulletPrefabPaths);
        }

        public void FreeBulletEntity(IEnumerable<Entity> collection)
        {
            this.FreeEntity(collection, this.bulletPool);
        }

        private Entity Retrieve(Queue<Entity> pool, string[] prefabs)
        {
            Entity entity;

            if (pool.Count > 0)
            {
                entity = pool.Dequeue();
            }
            else
            {
                // if there are more prefabs use a Random:
                // var prefab = prefabs[WaveServices.Random.Next(prefabs.Length)];
                var prefab = prefabs[0];
                entity = this.InstantiatePrefab(prefab, ref bulletIndex);
            }

            return entity;
        }

        private void FreeEntity(IEnumerable<Entity> collection, Queue<Entity> pool)
        {
            foreach (var entity in collection)
            {
                if (entity.Parent != null)
                {
                    entity.Parent.DetachChild(entity.Name);
                    pool.Enqueue(entity);
                }
            }
        }

        private Entity InstantiatePrefab(string prefab, ref int indexer)
        {
            var entity = this.EntityManager.Instantiate(prefab);
            entity.Name += indexer++;
            return entity;
        }
    }
}
