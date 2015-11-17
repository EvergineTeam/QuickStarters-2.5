using NewImpossibleGame.Enums;
using System;
using System.Runtime.Serialization;
using NewImpossibleGame.Services;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Input;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;

namespace NewImpossibleGame.Behaviors
{
    /// <summary>
    /// Player behavior
    /// </summary>
    [DataContract]
    public class PlayerBehavior : Behavior
    {
        /// <summary>
        /// The player transform
        /// </summary>
        [RequiredComponent]
        private Transform3D playerTransform = null;

        /// <summary>
        /// The game behavior
        /// </summary>
        [RequiredComponent]
        private GameBehavior gameBehavior = null;

        /// <summary>
        /// The player state
        /// </summary>
        private PlayerState playerState = PlayerState.INITIAL;

        /// <summary>
        /// The current vertical velocity
        /// </summary>
        private float currentVerticalVelocity;

        /// <summary>
        /// The boost velocity
        /// </summary>
        public float BoostVelocity;

        /// <summary>
        /// Gets or sets the game controller entity.
        /// </summary>
        /// <value>
        /// The game controller entity.
        /// </value>
        [DataMember]
        [RenderPropertyAsEntity]
        public string GameControllerEntitySource
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the player velocity.
        /// </summary>
        /// <value>
        /// The player velocity.
        /// </value>
        [DataMember]
        public float PlayerVelocity
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the jump speed.
        /// </summary>
        /// <value>
        /// The jump speed.
        /// </value>
        [DataMember]
        public float JumpSpeed
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the gravity.
        /// </summary>
        /// <value>
        /// The gravity.
        /// </value>
        [DataMember]
        public float Gravity
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the boost deceleration.
        /// </summary>
        /// <value>
        /// The boost deceleration.
        /// </value>
        [DataMember]
        public float BoostDeceleration
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the boost acceleration.
        /// </summary>
        /// <value>
        /// The boost acceleration.
        /// </value>
        [DataMember]
        public float BoostAcceleration
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the player initial position.
        /// </summary>
        /// <value>
        /// The player initial position.
        /// </value>
        [DataMember]
        public Vector3 PlayerInitialPosition
        {
            get;
            set;
        }

        /// <summary>
        /// The player bounding box
        /// </summary>
        public BoundingBox PlayerBoundingBox;

        /// <summary>
        /// Default Values, those will be override by WaveEditor (in case any of them were setted).
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();

            this.JumpSpeed = 20.0f;
            this.Gravity = -70.0f;
            this.PlayerVelocity = 15.0f;
            this.BoostDeceleration = 7.0f;
            this.BoostAcceleration = 10.0f;
            this.PlayerBoundingBox = new BoundingBox();
        }

        /// <summary>
        /// Allows this instance to execute custom logic during its
        /// <c>Update</c>.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        /// <remarks>
        /// This method will not be executed if the
        /// <see cref="T:WaveEngine.Framework.Component" />, or the
        /// <see cref="T:WaveEngine.Framework.Entity" />
        /// owning it are not
        /// <c>Active</c>.
        /// </remarks>
        protected override void Update(TimeSpan gameTime)
        {
            // convert here the gametime in float, multiple uses in this method
            float elapsedSeconds = (float)gameTime.TotalSeconds;
            Vector3 position = this.playerTransform.Position;

            // Calculate Position
            position.Z += this.CalculateCurrentVelocity(elapsedSeconds);

            // update player bounding box
            this.PlayerBoundingBox.Min.Y = this.playerTransform.Position.Y - ModelFactoryService.Instance.Scale.Y / 2;
            this.PlayerBoundingBox.Max.Y = this.playerTransform.Position.Y + ModelFactoryService.Instance.Scale.Y / 2;
            this.PlayerBoundingBox.Min.Z = this.playerTransform.Position.Z - ModelFactoryService.Instance.Scale.Z / 2;
            this.PlayerBoundingBox.Max.Z = this.playerTransform.Position.Z + ModelFactoryService.Instance.Scale.Z / 2;

            // 4 game states machine
            switch (this.playerState)
            {
                // Initial State: Initial player configuration
                case PlayerState.INITIAL:
                    position = this.PlayerInitialPosition;
                    this.BoostVelocity = 0.0f;
                    this.currentVerticalVelocity = 0f;
                    this.playerState = PlayerState.GROUND;
                    break;

                // Player is touching ground (plane where allow to walk)
                case PlayerState.GROUND:
                    // floor level is under player then fall
                    if (this.gameBehavior.CurrentGroundLevel + ModelFactoryService.Instance.Scale.Y / 2 < this.playerTransform.Position.Y)
                    {
                        this.playerState = PlayerState.FALLING;
                    }

                    // Check the game keys
                    if (WaveServices.Input.KeyboardState.Up == ButtonState.Pressed
                        || WaveServices.Input.KeyboardState.Space == ButtonState.Pressed
                        || WaveServices.Input.TouchPanelState.Count > 0)
                    {
                        this.currentVerticalVelocity = this.JumpSpeed;
                        this.playerState = PlayerState.JUMPING;
                        break;
                    }
                    break;

                // playing is jumping, ascending!!
                case PlayerState.JUMPING:
                    // vertical acceleration and rotation (elapsed time relative, care with this)
                    position.Y += this.currentVerticalVelocity * elapsedSeconds;

                    // update vertical velocity with the gravity acceleration
                    this.currentVerticalVelocity += this.Gravity * elapsedSeconds;

                    // if finish jumping (negative vertical velocity) then we are falling!!
                    if (this.currentVerticalVelocity < 0)
                    {
                        this.playerState = PlayerState.FALLING;
                    }
                    break;

                // player falls down, after a jump of running over an empty block (then should fall anyway)
                case PlayerState.FALLING:
                    // its a free fall acceleration: v(t)=a*t
                    this.currentVerticalVelocity += this.Gravity * elapsedSeconds;
                    position.Y += this.currentVerticalVelocity * elapsedSeconds;

                    // check if we touch the ground of current
                    if (this.playerTransform.Position.Y <= this.gameBehavior.CurrentGroundLevel + ModelFactoryService.Instance.Scale.Y * 0.7)
                    {
                        position.Y = this.gameBehavior.CurrentGroundLevel + ModelFactoryService.Instance.Scale.Y / 2;
                        this.playerState = PlayerState.GROUND;
                    }

                    // check if we touch the ground of next
                    if (this.playerTransform.Position.Y <= this.gameBehavior.NextGroundLevel + ModelFactoryService.Instance.Scale.Y * 0.7)
                    {
                        position.Y = this.gameBehavior.NextGroundLevel + ModelFactoryService.Instance.Scale.Y / 2;
                        this.playerState = PlayerState.GROUND;
                    }
                    break;

                // Player DIE! no special treatment here
                case PlayerState.DIE:
                    break;
            }

            this.playerTransform.Position = position;
        }

        /// <summary>
        /// Restarts this instance.
        /// </summary>
        public void Restart()
        {
            this.playerState = PlayerState.INITIAL;
        }

        /// <summary>
        /// Calculates the player velocity.
        /// </summary>
        /// <param name="elapsedGameTime">The elapsed game time.</param>
        private float CalculateCurrentVelocity(float elapsedGameTime)
        {
            // New Player Position
            // delta movement using boostvelocity
            var delta = (this.PlayerVelocity * elapsedGameTime) + this.BoostVelocity;

            // check boost velocity to decelerate
            if (this.BoostVelocity > 0.0)
            {
                this.BoostVelocity -= BoostDeceleration * elapsedGameTime;

                // minimum boost is 0.0
                if (this.BoostVelocity < 0.0f)
                {
                    this.BoostVelocity = 0.0f;
                }
            }

            return delta;
        }

        /// <summary>
        /// Accelerates the player.
        /// </summary>
        /// <param name="elapsedSeconds">The p.</param>
        public void Accelerate(float elapsedSeconds)
        {
            this.BoostVelocity += this.BoostAcceleration * elapsedSeconds;

            if (this.BoostVelocity > this.PlayerVelocity + this.BoostAcceleration)
            {
                this.BoostVelocity = this.PlayerVelocity + this.BoostAcceleration;
            }
        }

        /// <summary>
        /// Collideses the specified block transform.
        /// </summary>
        /// <param name="blockTransform">The block transform.</param>
        /// <returns></returns>
        public bool Collides(Transform3D blockTransform)
        {
            bool res =(this.playerTransform.Position.Y - ModelFactoryService.Instance.Scale.Y / 2 < blockTransform.Position.Y);
            return res;
        }
    }
}