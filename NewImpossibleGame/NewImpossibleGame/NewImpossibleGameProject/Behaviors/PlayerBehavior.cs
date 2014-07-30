using NewImpossibleGameProject.Enums;
using NewImpossibleGameProject.GameModels;
using NewImpossibleGameProject.GameServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Input;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;

namespace NewImpossibleGameProject.Behaviors
{
    /// <summary>
    /// Player behavior
    /// </summary>
    public class PlayerBehavior : Behavior
    {
        /// <summary>
        /// The jumpspeed
        /// </summary>
        private const float JUMPSPEED = 20f;

        /// <summary>
        /// The gravity
        /// </summary>
        private float gravity = -70f;

        /// <summary>
        /// The player state
        /// </summary>
        private PlayerState playerState = PlayerState.INITIAL;

        /// <summary>
        /// The current vertical velocity
        /// </summary>
        private float currentVerticalVelocity;

        /// <summary>
        /// The player
        /// </summary>
        private Entity player;

        /// <summary>
        /// The player transform
        /// </summary>
        private Transform3D playerTransform;

        /// <summary>
        /// The game behavior
        /// </summary>
        private GameBehavior gameBehavior;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerBehavior"/> class.
        /// </summary>
        /// <param name="gameBehavior">The game behavior.</param>
        public PlayerBehavior(GameBehavior gameBehavior)
            : base("PlayerBehavior")
        {
            this.gameBehavior = gameBehavior;
        }

        /// <summary>
        /// Resolves the dependencies needed for this instance to work.
        /// </summary>
        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            // fast references for players and its transform
            this.player = this.Owner;
            this.playerTransform = this.player.FindComponent<Transform3D>();
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
            // get keys
            var keys = WaveServices.Input.KeyboardState;

            // convert here the gametime in float, multiple uses in this method
            float elapsedSeconds = (float)gameTime.TotalSeconds;
            this.playerTransform.Rotation.X += this.gameBehavior.PlayerVelocity * elapsedSeconds;

            // 4 game states machine
            switch (this.playerState)
            {
                // Initial State: Initial player configuration, simply set to Ground state
                case PlayerState.INITIAL:
                    this.playerState = PlayerState.GROUND;
                    this.playerTransform.Rotation.Z = 0f;
                    this.currentVerticalVelocity = 0f;
                    break;

                // Player is touching ground (plane where allow to walk)
                case PlayerState.GROUND:

                    // floor level is under player then fall
                    if (this.gameBehavior.CurrentGroundLevel + ModelFactoryService.Instance.Scale.Y / 2 < this.playerTransform.Position.Y)
                    {
                        this.playerState = PlayerState.FALLING;
                    }

                    // Check the ONLY game key
                    if (keys.Up == ButtonState.Pressed)
                    {
                        this.currentVerticalVelocity = JUMPSPEED;
                        this.playerState = PlayerState.JUMPING;
                        break;
                    }
                    break;

                // playing is jumping, ascending!!
                case PlayerState.JUMPING:
                    // vertical acceleration and rotation (elapsed time relative, care with this)
                    this.playerTransform.Position.Y += this.currentVerticalVelocity * elapsedSeconds;
                    //this.playerTransform.Rotation.X += this.rotationSpeed * elapsedSeconds;

                    // update vertical velocity with the gravity acceleration
                    this.currentVerticalVelocity += this.gravity * elapsedSeconds;

                    // if finish jumping (negative vertical velocity) then we are falling!!
                    if (this.currentVerticalVelocity < 0)
                    {
                        this.playerState = PlayerState.FALLING;
                    }
                    break;

                // player falls down, after a jump of running over an empty block (then should fall anyway)
                case PlayerState.FALLING:
                    // its a free fall acceleration: v(t)=a*t
                    this.currentVerticalVelocity += this.gravity * elapsedSeconds;
                    this.playerTransform.Position.Y += this.currentVerticalVelocity * elapsedSeconds;

                    // no rotation, just fall
                    //this.playerTransform.Rotation.X = 0;

                    // check if we touch the ground of current
                    if (this.playerTransform.Position.Y <= this.gameBehavior.CurrentGroundLevel + ModelFactoryService.Instance.Scale.Y / 2)
                    {
                        this.playerTransform.Position.Y = this.gameBehavior.CurrentGroundLevel + ModelFactoryService.Instance.Scale.Y / 2;
                        this.playerState = PlayerState.GROUND;
                    }

                    // check if we touch the ground of next
                    if (this.playerTransform.Position.Y <= this.gameBehavior.NextGroundLevel + ModelFactoryService.Instance.Scale.Y / 2)
                    {
                        this.playerTransform.Position.Y = this.gameBehavior.NextGroundLevel + ModelFactoryService.Instance.Scale.Y / 2;
                        this.playerState = PlayerState.GROUND;
                    }
                    break;

                // Player DIE! no special treatment here
                case PlayerState.DIE:
                    break;
            }
        }

        /// <summary>
        /// Restarts this instance.
        /// </summary>
        public void Restart()
        {
            this.playerState = PlayerState.INITIAL;
            this.playerTransform.Rotation.X = 0f;
        }
    }
}
