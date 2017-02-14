using System.Runtime.Serialization;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Input;
using WaveEngine.Components.Gestures;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Components.Toolkit;
using WaveEngine.Framework;

namespace SuperSlingshot.Components
{
    [DataContract]
    public class ButtonComponent : Component
    {
        private TextComponent childTextComponent;
        private ButtonState state;

        public event StateChangedEventArgs StateChanged;
        public delegate void StateChangedEventArgs(object sender, ButtonState currentState, ButtonState lastState);

        [RequiredComponent]
        private TouchGestures touchGestures { get; set; }

        [RequiredComponent]
        private Sprite sprite { get; set; }

        [DataMember]
        [RenderPropertyAsAsset(AssetType.Texture)]
        public string NormalButtonPath { get; set; }

        [DataMember]
        [RenderPropertyAsAsset(AssetType.Texture)]
        public string HoverButtonPath { get; set; }

        [DataMember]
        [RenderPropertyAsAsset(AssetType.Texture)]
        public string PressedButtonPath { get; set; }

        [DataMember]
        [RenderPropertyAsAsset(AssetType.Texture)]
        public string BlockedButtonPath { get; set; }

        [DataMember]
        public string Text { get; set; }

        [DataMember]
        public bool IsBlocked { get; set; }

        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            var textEntity = this.Owner.FindChild(GameConstants.ENTITYCHILDBUTTONTEXT);
            if (textEntity != null)
            {
                this.childTextComponent = textEntity.FindComponent<TextComponent>();
                if (childTextComponent != null)
                {
                    this.childTextComponent.Text = this.Text;
                }
            }

            this.touchGestures.TouchPressed += this.TouchGesturesTouchPressed;
            this.touchGestures.TouchReleased += this.TouchGesturesTouchReleased;
            this.SetState(ButtonState.Release);
        }

        private void TouchGesturesTouchReleased(object sender, GestureEventArgs e)
        {
            this.SetState(ButtonState.Release);
        }

        private void TouchGesturesTouchPressed(object sender, GestureEventArgs e)
        {
            this.SetState(ButtonState.Pressed);
        }

        private void SetState(ButtonState state)
        {
            var lastState = this.state;
            this.state = state;

            if (!this.IsBlocked)
            {
                switch (this.state)
                {
                    case ButtonState.Release:
                        this.sprite.TexturePath = this.NormalButtonPath;
                        break;
                    case ButtonState.Pressed:
                        this.sprite.TexturePath = this.PressedButtonPath;
                        break;
                    default:
                        break;
                }

                // TODO: C#6 
                // this.StateChanged?.Invoke(this, this.state, lastState);
                if (this.StateChanged != null)
                {
                    this.StateChanged.Invoke(this, this.state, lastState);
                }
            }
            else
            {
                this.sprite.TexturePath = this.BlockedButtonPath;
            }
        }
    }
}
