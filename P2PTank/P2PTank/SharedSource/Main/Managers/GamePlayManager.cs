using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using P2PTank.Behaviors;
using P2PTank.Components;
using P2PTank.Scenes;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
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

        public Entity CreatePlayer()
        {
            var entity = this.CreateBaseTank(currentTankIndex++);
            entity.AddComponent(new PlayerInputBehavior());
            return entity;
        }

        private Entity CreateFoe()
        {
            var entity = this.CreateBaseTank(currentTankIndex++);
            return entity;
        }

        private Entity CreateBaseTank(int playerIndex)
        {
            var entity = this.EntityManager.Instantiate(WaveContent.Assets.Prefabs.tankPrefab);
            var component = entity.FindComponent<TankComponent>();
            component.Color = GameConstants.Palette[currentTankIndex];
            return entity;
        }
    }
}
