using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
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
        private GamePlayScene gamePlayScene;

        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            this.gamePlayScene = this.Owner.Scene as GamePlayScene;
        }

        public Entity CreatePlayer()
        {
            var entity = new Entity("Player");
            return entity;
        }

        public Entity CreateFoe()
        {
            var entity = new Entity();
            return entity;
        }
    }
}
