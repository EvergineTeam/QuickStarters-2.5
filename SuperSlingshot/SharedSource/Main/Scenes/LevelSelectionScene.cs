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
    public class LevelSelectionScene : Scene
    {
        private NavigationManager navigationManager;

        protected override void CreateScene()
        {
            this.Load(WaveContent.Scenes.LevelSelectionScene);

            var level1 = this.EntityManager.Find(GameConstants.ENTITYLEVEL1BUTTON);
            var level2 = this.EntityManager.Find(GameConstants.ENTITYLEVEL2BUTTON);
            var level3 = this.EntityManager.Find(GameConstants.ENTITYLEVEL3BUTTON);

            level1.FindComponent<ButtonComponent>().StateChanged += this.Level1StateChanged;
            level2.FindComponent<ButtonComponent>().StateChanged += this.Level2StateChanged;
            level3.FindComponent<ButtonComponent>().StateChanged += this.Level3StateChanged;

            this.navigationManager = WaveServices.GetService<NavigationManager>();
        }

        private void Level1StateChanged(object sender, ButtonState currentState, ButtonState lastState)
        {
            if (currentState == ButtonState.Release && lastState == ButtonState.Pressed)
            {
                this.navigationManager.NavigateToGameLevel(WaveContent.Scenes.Levels.Level1);
            }
        }
        private void Level2StateChanged(object sender, ButtonState currentState, ButtonState lastState)
        {
            if (currentState == ButtonState.Release && lastState == ButtonState.Pressed)
            {
                this.navigationManager.NavigateToGameLevel(WaveContent.Scenes.Levels.Level2);
            }
        }

        private void Level3StateChanged(object sender, ButtonState currentState, ButtonState lastState)
        {
            if (currentState == ButtonState.Release && lastState == ButtonState.Pressed)
            {
                this.navigationManager.NavigateToGameLevel(WaveContent.Scenes.Levels.Level3);
            }
        }
    }
}
