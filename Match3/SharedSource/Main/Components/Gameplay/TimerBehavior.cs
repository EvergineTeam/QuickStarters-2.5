using Match3.Services;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using WaveEngine.Components.Toolkit;
using WaveEngine.Framework;

namespace Match3.Components.Gameplay
{
    [DataContract]
    public class TimerBehavior : Behavior
    {
        private GameLogic gameLogic;

        [RequiredComponent]
        protected TextComponent textComponent;

        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            this.gameLogic = CustomServices.GameLogic;
        }

        protected override void Update(TimeSpan gameTime)
        {
            this.textComponent.Text = this.gameLogic.LeftTime.ToString("mm\\:ss");
        }
    }
}
