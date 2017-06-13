using MultiplayerTopDownTank.Entities;
using MultiplayerTopDownTank.Managers;
using System;
using System.Runtime.Serialization;
using WaveEngine.Common.Input;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.Physics2D;
using WaveEngine.Framework.Diagnostic;

namespace MultiplayerTopDownTank.Behaviors
{
    [DataContract]
    public class TankBehavior : Behavior
    {
        private Joystick leftJoystick, rightJoystick;
        private float moveVelocity;
        private float rotateVelocity;
        private Vector2 textureDirection;
        private TimeSpan time;
        private TimeSpan shootCadence;
        private Transform2D barrelTransform = null;
        private int life;

        [RequiredComponent]
        private Transform2D transform = null;

        [RequiredComponent]
        private RigidBody2D rigidBody = null;

        public int CurrentLive
        {
            get
            {
                return this.life;
            }

            set
            {
                this.life = value;
            }
        }

        /// <summary>
        /// Sets the default values
        /// </summary>
        protected override void DefaultValues()
        {
            this.shootCadence = TimeSpan.FromMilliseconds(150);
            this.time = this.shootCadence;
            this.moveVelocity = 15;
            this.rotateVelocity = 20;
            this.textureDirection = new Vector2(0, -1);
            this.life = 100;

            base.DefaultValues();
        }

        /// <summary>
        /// Resolves the dependencies needed for this instance to work.
        /// </summary>
        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            this.leftJoystick = this.EntityManager.Find<Joystick>("leftJoystick");
            this.rightJoystick = this.EntityManager.Find<Joystick>("rightJoystick");

            var barrelEntity = this.Owner.FindChild(GameConstants.PlayerBarrel);
            this.barrelTransform = barrelEntity.FindComponent<Transform2D>();
        }

        protected override void Update(TimeSpan gameTime)
        {
            Input input = WaveServices.Input;

            if (this.leftJoystick == null || this.rightJoystick == null)
            {
                return;
            }

            // Tank Move
            Vector2 moveDirection = this.leftJoystick.Direction;

            #region Keyboard move

            if (input.KeyboardState.IsConnected)
            {
                if (input.KeyboardState.IsKeyPressed(Keys.Up))
                    moveDirection += -Vector2.UnitY;
                if (input.KeyboardState.IsKeyPressed(Keys.Down))
                    moveDirection += Vector2.UnitY;
                if (input.KeyboardState.IsKeyPressed(Keys.Left))
                    moveDirection += -Vector2.UnitX;
                if (input.KeyboardState.IsKeyPressed(Keys.Right))
                    moveDirection += Vector2.UnitX;
            }

            #endregion

            if (!float.IsNaN(moveDirection.Y))
            {
                Vector2 impulse = moveDirection * this.moveVelocity * (float)gameTime.TotalSeconds;
                impulse.X = 0;

                float rotation = this.transform.Rotation;

                if (!float.IsNaN(rotation) && impulse.Y != 0)
                {
                    var result = Vector2.Rotate(impulse, this.transform.Rotation);
                    this.rigidBody.ApplyLinearImpulse(result, this.transform.Position);
                }
            }


            if (!float.IsNaN(moveDirection.X))
            {
                Vector2 impulse = moveDirection * this.rotateVelocity * (float)gameTime.TotalSeconds;
                impulse.Y = 0;

                float rotation = this.transform.Rotation;

                if (!float.IsNaN(rotation) && impulse.X != 0)
                {
                    var result = Vector2.Rotate(impulse, MathHelper.ToRadians(rotation));
                    this.rigidBody.ApplyTorque(result.X);
                }
            }

            // Tank shoot   
            Vector2 rightJoyDirection = this.rightJoystick.Direction;

            this.time -= gameTime;

            #region Keyboard move

            if (input.KeyboardState.IsConnected)
            {
                if (input.KeyboardState.IsKeyPressed(Keys.W))
                    rightJoyDirection += -Vector2.UnitY;
                if (input.KeyboardState.IsKeyPressed(Keys.S))
                    rightJoyDirection += Vector2.UnitY;
                if (input.KeyboardState.IsKeyPressed(Keys.A))
                    rightJoyDirection += -Vector2.UnitX;
                if (input.KeyboardState.IsKeyPressed(Keys.D))
                    rightJoyDirection += Vector2.UnitX;
            }

            #endregion

            if (!float.IsNaN(rightJoyDirection.X) && !float.IsNaN(rightJoyDirection.Y))
            {
                float rotation = Vector2.Angle(rightJoyDirection, this.textureDirection);

                if (!float.IsNaN(rotation))
                {
                    this.barrelTransform.Rotation = rotation;

                    if (this.time <= TimeSpan.Zero)
                    {
                        // Create bullet
                        Vector2 tankPosition = new Vector2(this.transform.X, this.transform.Y);

                        var gameScene = this.Owner.Scene as GameScene;

                        if (gameScene != null)
                        {
                            gameScene.Shoot(tankPosition, rightJoyDirection);
                        }

                        SoundsManager.Instance.PlaySound(SoundsManager.SOUNDS.Shoot);
                    }

                    this.time = this.shootCadence;
                }
            }
        }
    }
}