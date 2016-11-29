#region Using Statements
using System;
using SuperSlingshot.Components;
using SuperSlingshot.Managers;
using WaveEngine.Common;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Input;
using WaveEngine.Common.Math;
using WaveEngine.Components.Cameras;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Components.Graphics3D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Resources;
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
