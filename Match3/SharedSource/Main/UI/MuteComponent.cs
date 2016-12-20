using Match3.Services;
using System.Runtime.Serialization;
using WaveEngine.Framework;
using Match3.Services.Audio;

namespace Match3.UI
{
    [DataContract]
    public class MuteComponent : Component
    {
        [RequiredComponent]
        protected ToggleButtonComponent toggleButtonComponent;

        private AudioPlayer audioService;

        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            this.audioService = CustomServices.AudioPlayer;

            this.toggleButtonComponent.IsOn = this.audioService.IsMuted;

            this.toggleButtonComponent.OnStateChanged -= this.ToggleButtonComponentOnStateChanged;
            this.toggleButtonComponent.OnStateChanged += this.ToggleButtonComponentOnStateChanged;
        }

        private void ToggleButtonComponentOnStateChanged(object sender, bool isChecked)
        {
            this.audioService.IsMuted = isChecked;
        }
    }
}
