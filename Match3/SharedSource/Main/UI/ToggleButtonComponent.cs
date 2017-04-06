using System;
using System.Runtime.Serialization;
using WaveEngine.Components.Gestures;

namespace Match3.UI
{
    [DataContract]
    public class ToggleButtonComponent : ButtonComponent, IDisposable
    {
        [DataMember]
        public bool IsOn
        {
            get { return this.isPressed; }
            set
            {
                if (this.isPressed != value)
                {
                    this.isPressed = value;
                    this.RaiseStateChanged();
                    this.UpdateTexture();
                }
            }
        }

        public event EventHandler<bool> OnStateChanged;

        private void RaiseStateChanged()
        {
            this.OnStateChanged?.Invoke(this, this.isPressed);
        }

        protected override void TouchGestures_TouchPressed(object sender, GestureEventArgs e)
        {
            // Intentionally blank
        }

        protected override void TouchGestures_TouchReleased(object sender, GestureEventArgs e)
        {
            if (this.IsActive && e.GestureSample.IsNew)
            {
                this.SwitchValue();
            }
        }

        private void SwitchValue()
        {
            this.IsOn = !this.IsOn;
            this.PlayButtonSound();
        }
    }
}
