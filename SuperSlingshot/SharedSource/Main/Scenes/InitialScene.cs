#region Using Statements
using SlingshotRampage.Services;
using SuperSlingshot.Components;
using SuperSlingshot.Managers;
using WaveEngine.Common.Input;
using WaveEngine.Framework;
using WaveEngine.Framework.Services;
#endregion

namespace SuperSlingshot.Scenes
{
    public class InitialScene : Scene
    {
        private NavigationManager navigationManager;

        protected override void CreateScene()
        {
            this.Load(WaveContent.Scenes.InitialScene);

            var audioService = WaveServices.GetService<AudioService>();
            audioService.Play(Audio.Music.backgroundmusic_mp3, 1.0f);

            var start = this.EntityManager.Find(GameConstants.ENTITYSTARTBUTTON);

            start.FindComponent<ButtonComponent>().StateChanged += this.StartStateChanged;

            this.navigationManager = WaveServices.GetService<NavigationManager>();
        }

        private void StartStateChanged(object sender, ButtonState currentState, ButtonState lastState)
        {
            if (currentState == ButtonState.Release && lastState == ButtonState.Pressed)
            {
                this.navigationManager.NavigateToLevelSelection();
            }
        }
    }
}
