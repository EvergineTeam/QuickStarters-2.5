using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using P2PTank.Behaviors;
using P2PTank.Components;
using P2PTank.Scenes;
using WaveEngine.Common.Physics2D;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics2D;
using WaveEngine.Framework.Services;

namespace P2PTank.Managers
{
    [DataContract]
    public class GamePlayManager : Component
    {
        private static int currentTankIndex = 0;
        private GamePlayScene gamePlayScene;

        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            this.gamePlayScene = this.Owner.Scene as GamePlayScene;
        }

        public Entity CreatePlayer(int playerIndex, ColliderCategory2D category, ColliderCategory2D collidesWith)
        {
            var entity = this.CreateBaseTank(playerIndex, category, collidesWith);
            entity.AddComponent(new PlayerInputBehavior())
                 .AddComponent(new RigidBody2D
                 {
                     AngularDamping = 5.0f,
                     LinearDamping = 10.0f,
                 });
            return entity;
        }

        public Entity CreateFoe(int playerIndex, ColliderCategory2D category, ColliderCategory2D collidesWith)
        {
            var entity = this.CreateBaseTank(playerIndex, category, collidesWith);
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
            
            if(collider!=null)
            {
                collider.CollisionCategories = category;
                collider.CollidesWith = collidesWith;
            }
            return entity;
        }
    }
}
