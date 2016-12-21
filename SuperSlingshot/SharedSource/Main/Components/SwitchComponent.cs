using System.Runtime.Serialization;
using SuperSlingshot.Enums;
using WaveEngine.Common.Attributes;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Framework;

namespace SuperSlingshot.Components
{
    [DataContract]
    public class SwitchComponent : Component
    {
        private SwitchEnum state;

        [RequiredComponent]
        private Sprite sprite { get; set; }

        [RequiredComponent]
        private SpriteRenderer spriteRenderer { get; set; }

        [DataMember]
        public SwitchEnum State
        {
            get
            {
                return this.state;
            }

            set
            {
                this.state = value;
                this.SetState();
            }
        }

        [DataMember]
        [RenderPropertyAsAsset(AssetType.Texture)]
        public string UnCheckedTexturePath { get; set; }

        [DataMember]
        [RenderPropertyAsAsset(AssetType.Texture)]
        public string CheckedTexturePath { get; set; }

        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            this.State = SwitchEnum.OFF;
        }

        private void SetState()
        {
            if (this.sprite == null || this.spriteRenderer == null)
            {
                return;
            }

            this.spriteRenderer.IsVisible = true;

            switch (this.state)
            {
                case SwitchEnum.OFF:
                    if (!string.IsNullOrEmpty(this.UnCheckedTexturePath))
                    {
                        this.sprite.TexturePath = this.UnCheckedTexturePath;
                    }
                    else
                    {
                        this.State = SwitchEnum.INDETERNINATE;
                    }
                    break;
                case SwitchEnum.ON:
                    if (!string.IsNullOrEmpty(this.CheckedTexturePath))
                    {
                        this.sprite.TexturePath = this.CheckedTexturePath;
                    }
                    else
                    {
                        this.State = SwitchEnum.INDETERNINATE;
                    }
                    break;
                case SwitchEnum.INDETERNINATE:
                    this.spriteRenderer.IsVisible = false;
                    break;
                default:
                    break;
            }
        }
    }
}
