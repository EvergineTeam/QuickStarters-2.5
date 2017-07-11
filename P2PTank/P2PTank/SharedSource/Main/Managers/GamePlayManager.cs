using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using P2PTank.Scenes;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;

namespace P2PTank.Managers
{
    [DataContract]
    public class GamePlayManager : Component
    {
        private GamePlayScene gamePlayScene;

        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            this.gamePlayScene = this.Owner.Scene as GamePlayScene;
        }

        public void CreatePlayer()
        {
        }

        public void CreateFoe()
        {
        }
    }
}
