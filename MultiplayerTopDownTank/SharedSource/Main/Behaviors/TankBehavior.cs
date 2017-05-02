using MultiplayerTopDownTank.Components;
using MultiplayerTopDownTank.Entities;
using MultiplayerTopDownTank.Managers;
using System;
using System.Runtime.Serialization;
using WaveEngine.Common.Input;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Managers;
using WaveEngine.Framework.Services;

namespace MultiplayerTopDownTank.Behaviors
{
    [DataContract]
    public class TankBehavior : Behavior
    {
        private Joystick leftJoystick, rightJoystick;
        private int life;
        private float velocity;
        private Vector2 textureDirection;
        private TimeSpan time;
        private TimeSpan shootCadence;
        private VirtualScreenManager virtualScreenManager;
        private Transform2D barrelTransform = null;

        [RequiredComponent]
        private Transform2D transform = null;

        [RequiredComponent]
        private BulletEmitter bulletEmitter = null;

        /// <summary>
        /// Sets the default values
        /// </summary>
        protected override void DefaultValues()
        {
            this.shootCadence = TimeSpan.FromMilliseconds(150);
            this.time = this.shootCadence;
            this.life = 100;
            this.velocity = 2;
            this.textureDirection = new Vector2(0, -1);

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
            this.virtualScreenManager = this.Owner.Scene.VirtualScreenManager;

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

            if (!float.IsNaN(moveDirection.X) && !float.IsNaN(moveDirection.Y))
            {
                float x = this.transform.X + (moveDirection.X * velocity * 60 * (float)gameTime.TotalSeconds);
                this.transform.X = MathHelper.Clamp(x, this.virtualScreenManager.LeftEdge, this.virtualScreenManager.RightEdge);

                float y = this.transform.Y + (moveDirection.Y * velocity * 60 * (float)gameTime.TotalSeconds);
                this.transform.Y = MathHelper.Clamp(y, this.virtualScreenManager.TopEdge, this.virtualScreenManager.BottomEdge);

                float rotation = Vector2.Angle(moveDirection, this.textureDirection);

                if (float.IsNaN(rotation))
                {
                    // Tank does not change its rotation when player do not move
                    // this.transform.Rotation = 0;
                }
                else
                {
                    this.transform.Rotation = rotation;
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
                        this.bulletEmitter.Shoot(tankPosition, rightJoyDirection);
                        SoundsManager.Instance.PlaySound(SoundsManager.SOUNDS.Shoot);
                    }

                    this.time = this.shootCadence;
                }
            }
        }
    }
}