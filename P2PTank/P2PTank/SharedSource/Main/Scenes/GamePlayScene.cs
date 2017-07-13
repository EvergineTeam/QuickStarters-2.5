using System;
using System.Collections.Generic;
using System.Text;
using P2PTank.Behaviors;
using P2PTank.Components;
using P2PTank.Managers;
using WaveEngine.Common.Math;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;

namespace P2PTank.Scenes
{
    public class GamePlayScene : Scene
    {
        private GamePlayManager gamePlayManager;

        protected override void CreateScene()
        {
            this.Load(WaveContent.Scenes.GamePlayScene);

            this.gamePlayManager = this.EntityManager.FindComponentFromEntityPath<GamePlayManager>(GameConstants.ManagerEntityPath);

            var player = new Entity()
                .AddComponent(new Transform2D() { Origin = Vector2.Center })
                .AddComponent(new Sprite(WaveContent.Assets.Textures.tankBody_png))
                .AddComponent(new SpriteRenderer())
                .AddComponent(new TankComponent())
                .AddComponent(new PlayerInputBehavior());
            this.EntityManager.Add(player);
        }
    }
}
