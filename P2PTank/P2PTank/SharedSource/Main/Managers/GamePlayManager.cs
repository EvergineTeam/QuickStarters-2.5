using System.Runtime.Serialization;
using P2PTank.Scenes;
using WaveEngine.Framework;

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
