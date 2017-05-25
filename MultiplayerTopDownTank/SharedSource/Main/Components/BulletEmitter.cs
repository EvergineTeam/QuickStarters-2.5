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
            var bullet = this.bulletManager.Retrieve();
            bullet.FindComponent<BulletComponent>().Shoot(position, direction);
        }
    }
}
