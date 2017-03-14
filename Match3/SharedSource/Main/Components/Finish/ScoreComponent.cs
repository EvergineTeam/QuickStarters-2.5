using Match3.Services;
using System.Runtime.Serialization;
using WaveEngine.Components.Toolkit;
using WaveEngine.Framework;

namespace Match3.Components.Finish
{
    [DataContract]
    public class ScoreComponent : Component
    {
        [RequiredComponent]
        protected TextComponent textComponent;

        protected override void Initialize()
        {
            base.Initialize();

            this.textComponent.Text = CustomServices.GameLogic.CurrentScore.ToString();
        }
    }
}
