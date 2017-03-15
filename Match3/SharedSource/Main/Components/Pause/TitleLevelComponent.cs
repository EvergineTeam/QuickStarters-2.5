using Match3.Services;
using System.Runtime.Serialization;
using WaveEngine.Components.Toolkit;
using WaveEngine.Framework;

namespace Match3.Components.Pause
{
    [DataContract]
    public class TitleLevelComponent : Component
    {
        [RequiredComponent]
        protected TextComponent textComponent;

        [DataMember]
        public string Format { get; set; }

        protected override void DefaultValues()
        {
            base.DefaultValues();
            this.Format = "level {0}";
        }

        protected override void Initialize()
        {
            base.Initialize();

            this.textComponent.Text = string.Format(this.Format, CustomServices.GameLogic.CurrentLevel);
        }
    }
}
