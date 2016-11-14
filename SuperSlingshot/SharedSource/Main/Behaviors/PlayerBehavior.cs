using System;
using System.Runtime.Serialization;
using SuperSlingshot;
using SuperSlingshot.Enums;
using WaveEngine.Common.Attributes;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Framework;

namespace SuperSlingshot.Behaviors
{
    [DataContract]
    public class PlayerBehavior : Behavior
    {
        private PlayerState state;

        [RequiredComponent]
        private Sprite bodySprite { get; set; }

        [DataMember]
        [RenderPropertyAsAsset(AssetType.Texture)]
        public string BodyTexturePath { get; set; }

        [DataMember]
        [RenderPropertyAsAsset(AssetType.Texture)]
        public string FacePreparedTexturePath { get; set; }

        [DataMember]
        [RenderPropertyAsAsset(AssetType.Texture)]
        public string FaceInTheAirTexturePath { get; set; }

        [DataMember]
        [RenderPropertyAsAsset(AssetType.Texture)]
        public string FaceStampedTexturePath { get; set; }

        [DataMember]
        public PlayerState PlayerState
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

        private Sprite faceSprite;

        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            var childFace = this.Owner.FindChild(GameConstants.ENTITYFACE);
            this.faceSprite = childFace.FindComponent<Sprite>();
        }

        protected override void Initialize()
        {
            base.Initialize();
            this.PlayerState = PlayerState.Prepared;
        }

        protected override void Update(TimeSpan gameTime)
        {
            if (PlayerState == PlayerState.Prepared)
            {
            }
            else if (PlayerState == PlayerState.InTheAir)
            {

            }
            else
            {

            }
        }

        private void SetState()
        {
            switch (this.state)
            {
                case PlayerState.Prepared:
                    if (!string.IsNullOrEmpty(this.BodyTexturePath) && this.bodySprite != null)
                    {
                        this.bodySprite.TexturePath = this.BodyTexturePath;
                    }

                    if (!string.IsNullOrEmpty(this.FacePreparedTexturePath) && this.faceSprite != null)
                    {
                        this.faceSprite.TexturePath = this.FacePreparedTexturePath;
                    }
                    break;
                case PlayerState.InTheAir:
                    if (!string.IsNullOrEmpty(this.FaceInTheAirTexturePath) && this.faceSprite != null)
                    {
                        this.faceSprite.TexturePath = this.FaceInTheAirTexturePath;
                    }
                    break;
                case PlayerState.Stamped:
                    if (!string.IsNullOrEmpty(this.FaceStampedTexturePath) && this.faceSprite != null)
                    {
                        this.faceSprite.TexturePath = this.FaceStampedTexturePath;
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
