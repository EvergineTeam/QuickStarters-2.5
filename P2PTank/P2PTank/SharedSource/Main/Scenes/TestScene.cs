using System;
using System.Collections.Generic;
using System.Text;
using P2PTank.Behaviors;
using P2PTank.Behaviors.Cameras;
using P2PTank.Components;
using P2PTank.Managers;
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
        }

        protected override void Start()
        {
            base.Start();

            var gameplayManager = this.EntityManager.FindComponentFromEntityPath<GamePlayManager>(GameConstants.ManagerEntityPath);
            var player = gameplayManager.CreatePlayer();
            this.EntityManager.Add(player);

            var targetCameraBehavior = new TargetCameraBehavior();
            targetCameraBehavior.SetTarget(player.FindComponent<Transform2D>());
            targetCameraBehavior.Follow = true;
            targetCameraBehavior.Speed = 5;
            this.RenderManager.ActiveCamera2D.Owner.AddComponent(targetCameraBehavior);
        }
    }
}
