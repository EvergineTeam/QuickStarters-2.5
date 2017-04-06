using Match3.Services;
using Match3.Services.Audio;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using WaveEngine.Common.Attributes;
using WaveEngine.Components.Gestures;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Framework;

namespace Match3.UI
{
    [DataContract]
    public class ButtonComponent : Component, IDisposable
    {
        [RequiredComponent]
        protected SpriteAtlas spriteAtlas;

        [RequiredComponent]
        protected TouchGestures touchGestures;

        protected bool isPressed;

        [DataMember]
        private string releasedTextureName;

        [DataMember]
        private string pressedTextureName;

        [DataMember]
        public bool IsActive { get; set; }

        [RenderPropertyAsSelector("TextureNames")]
        public string ReleasedTextureName
        {
            get
            {
                return this.releasedTextureName;
            }
            set
            {
                this.releasedTextureName = value;

                this.UpdateTexture();
            }
        }

        [RenderPropertyAsSelector("TextureNames")]
        public string PressedTextureName
        {
            get
            {
                return this.pressedTextureName;
            }
            set
            {
                this.pressedTextureName = value;

                this.UpdateTexture();
            }
        }

        [DontRenderProperty]
        public IEnumerable<string> TextureNames
        {
            get
            {
                return this.spriteAtlas.TextureNames;
            }
        }

        public event EventHandler OnClick;

        protected override void DefaultValues()
        {
            base.DefaultValues();

            this.IsActive = true;
        }

        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            this.touchGestures.TouchPressed -= this.TouchGestures_TouchPressed;
            this.touchGestures.TouchReleased -= this.TouchGestures_TouchReleased;

            this.touchGestures.TouchPressed += this.TouchGestures_TouchPressed;
            this.touchGestures.TouchReleased += this.TouchGestures_TouchReleased;
        }

        protected override void DeleteDependencies()
        {
            this.RemoveCustomDependencies();
            base.DeleteDependencies();
        }

        public void Dispose()
        {
            this.RemoveCustomDependencies();
        }

        private void RemoveCustomDependencies()
        {
            if (this.touchGestures != null)
            {
                this.touchGestures.TouchPressed -= this.TouchGestures_TouchPressed;
                this.touchGestures.TouchReleased -= this.TouchGestures_TouchReleased;
            }
        }

        protected virtual void TouchGestures_TouchReleased(object sender, GestureEventArgs e)
        {
            if (this.IsActive && this.isPressed)
            {
                this.isPressed = false;

                this.UpdateTexture();
                this.PlayButtonSound();

                this.OnClick?.Invoke(this, EventArgs.Empty);
            }
        }

        protected void PlayButtonSound()
        {
            var soundPlayer = CustomServices.AudioPlayer;

            if (soundPlayer != null)
            {
                soundPlayer.PlaySound(Sounds.Button);
            }
        }

        protected virtual void TouchGestures_TouchPressed(object sender, GestureEventArgs e)
        {
            if (this.IsActive && e.GestureSample.IsNew)
            {
                this.isPressed = true;

                this.UpdateTexture();
            }
        }

        protected void UpdateTexture()
        {
            this.spriteAtlas.TextureName = this.isPressed ? this.pressedTextureName : this.releasedTextureName;
        }
    }
}
