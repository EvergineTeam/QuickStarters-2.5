using System;
using System.Collections.Generic;
using System.Text;
using P2PTank.Managers;
using WaveEngine.Framework;

namespace P2PTank.Scenes
{
    public class GamePlayScene : Scene
    {
        private GamePlayManager gamePlayManager;

        protected override void CreateScene()
        {
            this.gamePlayManager = new GamePlayManager();

            // TODO: Create this Entity with component by Wave Editor
            Entity managersEntity = new Entity(GameConstants.ManagerEntityName)
                .AddComponent(this.gamePlayManager);
        }
    }
}
