using MultiplayerTopDownTank.Entities;
using MultiplayerTopDownTank.Managers;
using System.Runtime.Serialization;
using WaveEngine.Common.Math;
using WaveEngine.Framework;

namespace MultiplayerTopDownTank.Components
{
    [DataContract(Namespace = "MultiplayerTopDownTank.Components")]
    public class BulletEmitter : Component
    {
        private BulletManager bulletManager;

        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            bulletManager = EntityManager
                .Find(GameConstants.Manager)
                .FindComponent<BulletManager>();
        }

        /// <summary>
        /// Shoots the specified position.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="direction">The direction.</param>
        public void Shoot(Vector2 position, Vector2 direction)
        {
            Bullet bullet = this.bulletManager.Retrieve();
            bullet.Shoot(position, direction);
        }

        /// <summary>
        /// Destry the Bullet
        /// </summary>
        /// <param name="bullet">The Bullet</param>
        /// <param name="destroy">Indicates if the bullet is destroyed</param>
        public void Destroy(Bullet bullet, bool destroy)
        {
            bullet.IsBulletActive(destroy);
        }
    }
}
