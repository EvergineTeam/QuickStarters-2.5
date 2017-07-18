using System;
using System.Collections.Generic;
using System.Text;
using P2PTank.Behaviors;
using P2PTank.Components;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;

namespace P2PTank.Scenes
{
    public class TestScene : Scene
    {
        private static int currentTankIndex = 0;

        protected override void CreateScene()
        {
            this.Load(WaveContent.Scenes.GamePlayScene);

            this.CreatePlayer();
        }

        private void CreateFoe()
        {
            var entity = this.CreateBaseTank(currentTankIndex++);
            this.EntityManager.Add(entity);
        }

        private void CreatePlayer()
        {
            var entity = this.CreateBaseTank(currentTankIndex++);
            entity.AddComponent(new PlayerInputBehavior());
            this.EntityManager.Add(entity);
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
