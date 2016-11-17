using System;
using System.Runtime.Serialization;
using SuperSlingshot;
using SuperSlingshot.Enums;
using SuperSlingshot.Managers;
using WaveEngine.Common.Attributes;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics2D;
using WaveEngine.Framework.Services;

namespace SuperSlingshot.Behaviors
{
    [DataContract]
    public class PlayerBehavior : Behavior
    {
        private PlayerState state;
        private TimeSpan timeToDismiss;
        private GamePlayManager gamePlayManager;

        [RequiredComponent]
        private RigidBody2D rigidBody = null;

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
        [RenderPropertyAsFInput(MinLimit = 0, Tooltip = "millis")]
        public float TimeToDismiss { get; set; }

        [DataMember]
        [RenderPropertyAsFInput(MinLimit = 0, Tooltip = "Velocity vector length squared")]
        public float MinimumVelocityToDeclareDead { get; set; }

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
            this.timeToDismiss = TimeSpan.FromMilliseconds(this.TimeToDismiss);

            this.gamePlayManager = WaveServices.GetService<GamePlayManager>();
        }

        protected override void Initialize()
        {
            base.Initialize();
            this.PlayerState = PlayerState.Prepared;
        }

        protected override void Update(TimeSpan gameTime)
        {
            switch (this.PlayerState)
            {
                case PlayerState.Prepared:
                    break;
                case PlayerState.InTheAir:
                    // Dead condition: Slept or out of game area
                    if (this.rigidBody.Awake == false
                        || this.rigidBody.LinearVelocity.LengthSquared() <= this.MinimumVelocityToDeclareDead)
                    {
                        this.PlayerState = PlayerState.Dead;
                    }
                    break;
                case PlayerState.Stamped:
                    break;
                case PlayerState.Dead:
                    this.timeToDismiss -= gameTime;

                    if (this.timeToDismiss <= TimeSpan.Zero)
                    {
                        this.gamePlayManager.BoulderDead(this.Owner);
                    }
                    break;
                default:
                    break;
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
                case PlayerState.Dead:
                default:
                    break;
            }
        }
    }
}
