using NewImpossibleGame.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using NewImpossibleGame.Models;
using WaveEngine.Common.Input;
using WaveEngine.Common.Math;
using WaveEngine.Components.Graphics3D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics3D;
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
        /// degrees in Radiands. max Angle to land in ground
        /// </summary>
        private const float rad30 = 0.523599f;

        /// <summary>
        /// The killer tag
        /// </summary>
        private const string KILLERTAG = "KILLER";

        /// <summary>
        /// The initialized
        /// </summary>
        private bool initialized = false;

        /// <summary>
        /// The intial position
        /// </summary>
        private Vector3 initialPosition;

        /// <summary>
        /// The block positions
        /// </summary>
        private List<BlockPathPosition> blockPositions;

        /// <summary>
        /// The current vertical velocity
        /// </summary>
        private float currentVerticalVelocity;

        /// <summary>
        /// The ray
        /// </summary>
        private Ray ray;

        /// <summary>
        /// The player transform
        /// </summary>
        [RequiredComponent]
        private Transform3D playerTransform = null;

        /// <summary>
        /// The player spinner behavior
        /// </summary>
        [RequiredComponent]
        private Spinner spinner = null;

        /// <summary>
        /// The player collider
        /// </summary>
        [RequiredComponent]
        private SphereCollider3D playerCollider = null;

        /// <summary>
        /// Gets or sets the player velocity.
        /// </summary>
        /// <value>
        /// The player velocity.
        /// </value>
        [DataMember]
        public float PlayerVelocity { get; set; }

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
        /// The state
        /// </summary>
        private PlayerState state = PlayerState.INITIAL;

        /// <summary>
        /// Updates the specified game time.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        protected override void Update(TimeSpan gameTime)
        {
            float elapsed = (float)gameTime.TotalSeconds;
            Vector3 position = this.playerTransform.Position;
            this.ray.Position = position;

            // Initialization
            if (!initialized)
            {
                this.InitializeAttributes();
                this.initialized = true;
            }

            // Check Collision
            var collidables =
                this.blockPositions.Where(
                    b =>
                        b.ZPosition > this.playerTransform.Position.Z - 1 &&
                        b.ZPosition < this.playerTransform.Position.Z + 1);

            foreach (var blockPathPosition in collidables)
            {
                var collidableEntity = this.EntityManager.Find(blockPathPosition.Path);
                var blockCollider = collidableEntity.FindComponent<BoxCollider3D>().BoundingBox;

                var collisionType = this.CheckCollisionType(this.playerTransform.Position, 0.45f, blockCollider, collidableEntity.Tag);
                if (collisionType == CollisionType.KILLER)
                {
                    position = this.initialPosition;
                    this.state = PlayerState.GROUND;
                    break;
                }
                else if (collisionType == CollisionType.GROUND)
                {
                    this.currentVerticalVelocity = 0;
                    position.Y = (int)Math.Round(position.Y);
                    this.state = PlayerState.GROUND;
                }
            }

            // Check Ground
            var under = this.blockPositions.Where(b => b.ZPosition == (float)Math.Round(this.playerTransform.Position.Z));
            BoxCollider3D nearestBoxCollider = null;
            float minDistance = float.MaxValue;

            foreach (var blockPathPosition in under)
            {
                var collidableEntity = this.EntityManager.Find(blockPathPosition.Path);
                var blockCollider = collidableEntity.FindComponent<BoxCollider3D>();

                var groundDistance = blockCollider.BoundingBox.Intersects(ref this.ray);
                if (groundDistance.HasValue)
                {
                    if (groundDistance.Value < minDistance)
                    {
                        minDistance = groundDistance.Value;
                        nearestBoxCollider = blockCollider;
                    }
                }
            }
            if (nearestBoxCollider != null)
            {
                if (minDistance <= 0.5f)
                {
                    this.state = PlayerState.GROUND;
                }
                else
                {
                    this.state = PlayerState.FALLING;
                }
            }
            else
            {
                this.state = PlayerState.FALLING;
            }

            // Check state
            switch (this.state)
            {
                case PlayerState.GROUND:
                    // Check the game keys
                    this.currentVerticalVelocity = 0;
                    if (WaveServices.Input.KeyboardState.Up == ButtonState.Pressed
                        || WaveServices.Input.KeyboardState.Space == ButtonState.Pressed
                        || WaveServices.Input.TouchPanelState.Count > 0)
                    {
                        this.currentVerticalVelocity = this.JumpSpeed;
                        this.state = PlayerState.JUMPING;
                        break;
                    }
                    break;

                // playing is jumping, ascending!!
                case PlayerState.JUMPING:
                    this.currentVerticalVelocity -= this.Gravity * elapsed;
                    break;

                // player falls down, after a jump of running over an empty block (then should fall anyway)
                case PlayerState.FALLING:
                    this.currentVerticalVelocity -= this.Gravity * elapsed;

                    // Dead level
                    if (position.Y <= -7.0f)
                    {
                        position = this.initialPosition;
                        this.state = PlayerState.GROUND;
                    }
                    break;
            }

            // Move Forward and spin (speed relative)
            var horizontalstep = this.PlayerVelocity * elapsed;
            this.spinner.IncreaseX = this.PlayerVelocity;
            position.Z += horizontalstep;
            position.Y += this.currentVerticalVelocity * elapsed;
            this.playerTransform.Position = position;
        }

        /// <summary>
        /// Initializes the attributes.
        /// </summary>
        private void InitializeAttributes()
        {
            this.state = PlayerState.GROUND;

            this.ray = new Ray();
            ray.Direction = Vector3.Down;

            // get intial position
            this.initialPosition = new Vector3(0, 1, 0);

            // create collidable bloc list
            this.blockPositions = new List<BlockPathPosition>();
            foreach (var block in this.EntityManager.AllEntities.Where(e => e.FindComponent<Collider3D>(false) != null))
            {
                if (block.Name != "Player")
                {
                    var blockTransform = block.FindComponent<Transform3D>();
                    this.blockPositions.Add(new BlockPathPosition()
                    {
                        Path = block.Name,
                        ZPosition = blockTransform.Position.Z
                    });
                }
            }

            // order by ZPosition
            this.blockPositions = this.blockPositions.OrderBy(b => b.ZPosition).ToList();
        }

        /// <summary>
        /// Intersectses the sphere with box.
        /// </summary>
        /// <param name="center">The center.</param>
        /// <param name="radius">The radius.</param>
        /// <param name="boxCollider">The box collider.</param>
        /// <returns>
        /// True if intersects
        /// </returns>
        public CollisionType CheckCollisionType(Vector3 center, float radius, BoundingOrientedBox boxCollider, string tag)
        {
            CollisionType res = CollisionType.NONE;

            bool collides = this.Intersects(center, radius, boxCollider);

            if (collides)
            {
                if (tag.Equals(KILLERTAG))
                {
                    res = CollisionType.KILLER;
                }
                else
                {
                    Vector2 CB = new Vector2(boxCollider.Center.Z - center.Z, boxCollider.Center.Y - center.Y);
                    CB.Normalize();

                    var angle = Vector2.Angle(-Vector2.UnitY, CB);

                    if (angle > -rad30 && angle < rad30)
                    {
                        res = CollisionType.GROUND;
                    }
                    else
                    {
                        res = CollisionType.KILLER;
                    }
                }
            }

            return res;
        }

        /// <summary>
        /// Intersectses this instance.
        /// </summary>
        /// <param name="center">The center.</param>
        /// <param name="radius">The radius.</param>
        /// <param name="boxCollider">The box collider.</param>
        /// <returns>true if intersects</returns>
        private bool Intersects(Vector3 center, float radius, BoundingOrientedBox boxCollider)
        {
            Vector2 circleDistance = Vector2.Zero;
            circleDistance.X = Math.Abs(center.Z - boxCollider.Center.Z);
            circleDistance.Y = Math.Abs(center.Y - boxCollider.Center.Y);

            if (circleDistance.X > (boxCollider.HalfExtent.Z + radius)) { return false; }
            if (circleDistance.Y > (boxCollider.HalfExtent.Y + radius)) { return false; }

            if (circleDistance.X <= (boxCollider.HalfExtent.Z)) { return true; }
            if (circleDistance.Y <= (boxCollider.HalfExtent.Y)) { return true; }

            var cornerDistanceSq = Math.Sqrt(circleDistance.X - boxCollider.HalfExtent.Z) +
                                Math.Sqrt(circleDistance.Y - boxCollider.HalfExtent.Y);

            return (cornerDistanceSq <= (Math.Sqrt(radius)));
        }
    }
}