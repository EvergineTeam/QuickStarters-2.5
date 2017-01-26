using Match3.Components.Gameplay;
using Match3.Gameboard;
using Match3.Services;
using WaveEngine.Framework;

namespace Match3.Scenes
{
    public class Gameplay : Scene
    {
        private GameboardContent gameboardContent;

        protected override void CreateScene()
        {
            this.Load(WaveContent.Scenes.Gameplay);
            
            var gamePanel = this.EntityManager.Find("Panel.Content");
            this.gameboardContent = gamePanel.FindComponent<GameboardContent>();
        }

        protected override void Start()
        {
            base.Start();

            var board = new Board(6, 6);
            this.gameboardContent.RegenerateGameboard(board);
        }
    }
}
