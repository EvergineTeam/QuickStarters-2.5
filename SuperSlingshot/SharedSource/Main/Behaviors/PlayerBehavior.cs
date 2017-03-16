using System;
using System.Runtime.Serialization;
using SuperSlingshot.Enums;
using SuperSlingshot.Managers;
using SuperSlingshot.Scenes;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Math;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics2D;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.Diagnostic;

namespace SuperSlingshot.Behaviors
{
    [DataContract]
    public class PlayerBehavior : Behavior
    {
        private Sprite faceSprite;
        private PlayerState state;
        private TimeSpan timeToDismiss;
        private GamePlayManager gamePlayManager;

        [RequiredComponent]
        private RigidBody2D rigidBody = null;

        [RequiredComponent]
        private Transform2D transform = null;

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
            var gameScene = this.Owner.Scene as GameScene;
            if (gameScene == null)
            {
                return;
            }

            switch (this.PlayerState)
            {
                case PlayerState.Prepared:
                    var anchorEntity = gameScene.SlingshotAnchorEntity;
                    var anchorTransform = anchorEntity.FindComponent<Transform2D>();
                    var anchorPosition = anchorTransform.Position;
                    var impulse = anchorPosition - this.transform.Position;
                    Labels.Add("Impulse", impulse);

                    var showElasticBands = true;
                    if (impulse.X == 0)
                    {
                        impulse = Vector2.Zero;
                        showElasticBands = false;
                    }
                    
                    gameScene.PreviewTrajectory(impulse);
                    gameScene.PreviewElasticBands(showElasticBands, this.transform);
                    break;
                case PlayerState.InTheAir:
                    gameScene.PreviewElasticBands(false, this.transform);
                    // Dead condition: body slept or out of game area
                    if (this.rigidBody.Awake == false
                        || this.rigidBody.LinearVelocity.LengthSquared() <= this.MinimumVelocityToDeclareDead)
                    {
                        this.PlayerState = PlayerState.Dead;
                    }

                    var position = this.transform.Position;
                    if (gamePlayManager.CheckBounds(position))
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
