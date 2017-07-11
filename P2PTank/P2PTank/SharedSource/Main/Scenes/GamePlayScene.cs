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
            this.Load(WaveContent.Scenes.GamePlayScene);

            this.gamePlayManager = this.EntityManager.FindComponentFromEntityPath<GamePlayManager>(GameConstants.ManagerEntityPath);
        }
    }
}
