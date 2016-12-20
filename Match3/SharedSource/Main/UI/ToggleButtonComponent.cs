#region Using Statements
using Match3.Services;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using WaveEngine.Common.Attributes;
using WaveEngine.Components.Gestures;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Framework;
#endregion

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
            if (this.OnStateChanged != null)
            {
                this.OnStateChanged(this, this.isPressed);
            }
        }

        protected override void TouchGestures_TouchPressed(object sender, GestureEventArgs e)
        {
        }

        protected override void TouchGestures_TouchReleased(object sender, GestureEventArgs e)
        {
            if (this.IsActive
             && e.GestureSample.IsNew)
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
