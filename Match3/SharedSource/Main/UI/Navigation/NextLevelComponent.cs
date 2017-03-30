using Match3.Services;
using System;
using System.Runtime.Serialization;
using WaveEngine.Framework;

namespace Match3.UI.Navigation
{
    [DataContract]
    public class NextLevelComponent : Component
    {
        [RequiredComponent]
        protected ButtonComponent buttonComponent;

        private GameLogic gameLogic;

        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            this.gameLogic = CustomServices.GameLogic;
        }

        protected override void Initialize()
        {
            base.Initialize();

            var nextLevelAvailable = this.gameLogic.IsLevelUnlocked(this.gameLogic.CurrentLevel + 1);
            this.Owner.IsActive = nextLevelAvailable;
            if (!nextLevelAvailable)
            {
                this.buttonComponent.ReleasedTextureName = this.buttonComponent.PressedTextureName;
            }

            this.buttonComponent.OnClick += this.ButtonComponentOnClick;
        }

        private void ButtonComponentOnClick(object sender, EventArgs e)
        {
            this.gameLogic.SelectLevel(this.gameLogic.CurrentLevel + 1);
        }
    }
}
