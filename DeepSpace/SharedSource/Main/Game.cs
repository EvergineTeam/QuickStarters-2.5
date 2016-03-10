using DeepSpace.Managers;
using WaveEngine.Common;
using WaveEngine.Framework.Services;

namespace DeepSpace
{
    public class Game : WaveEngine.Framework.Game
    {
        public override void Initialize(IApplication application)
        {
            base.Initialize(application);
            
            this.RegisterServices();
            
            var gameContext = new ScreenContext("GamePlayContext", new GamePlayScene())
            {
                Behavior = ScreenContextBehaviors.DrawInBackground | ScreenContextBehaviors.UpdateInBackground
            };
            var introContext = new ScreenContext(new IntroScene());
            WaveServices.ScreenContextManager.Push(gameContext);
            WaveServices.ScreenContextManager.Push(introContext);

            // Play background music
            //WaveServices.MusicPlayer.Play(new MusicInfo("Content/music.mp3"));
            //WaveServices.MusicPlayer.Volume = 0.3f;
            //WaveServices.MusicPlayer.IsRepeat = true;
        }

        private void RegisterServices()
        {
            WaveServices.RegisterService(new SoundManager());
            WaveServices.RegisterService(new ScoreManager());
        }
    }
}
