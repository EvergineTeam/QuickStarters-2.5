using Match3.Services;
using System;
using System.Runtime.Serialization;
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

        private TimeSpan lastLeftTime; 

        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            this.gameLogic = CustomServices.GameLogic;
        }

        protected override void Update(TimeSpan gameTime)
        {
            if ((int)this.lastLeftTime.TotalSeconds != (int)this.gameLogic.LeftTime.TotalSeconds)
            {
                this.lastLeftTime = this.gameLogic.LeftTime;
                this.textComponent.Text = this.gameLogic.LeftTime.ToString("mm\\:ss");

                if(this.lastLeftTime.TotalSeconds < 10)
                {
                    CustomServices.AudioPlayer.PlaySound(Services.Audio.Sounds.CountDown);
                }
            }
        }
    }
}
