#region Using Statements
using SurvivorProject.Commons;
using SurvivorProject.Entities;
using SurvivorProject.Managers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Input;
using WaveEngine.Common.Math;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
#endregion

namespace SurvivorProject.Behaviors
{
    public class PlayerBehavior : Behavior
    {
        [RequiredComponent]
        private Transform2D transform;

        private Joystick leftJoystick, rightJoystick;
        private float velocity = 5;
        private Vector2 textureDirection = new Vector2(0, -1);
        private TimeSpan shootCadence;
        private TimeSpan time;
        private BulletEmiter bulletEmiter;
        private int borderMargin = 100;        

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerBehavior" /> class.
        /// </summary>
        public PlayerBehavior()
        {
            this.shootCadence = TimeSpan.FromMilliseconds(300);
            this.time = this.shootCadence;
        }

        /// <summary>
        /// Resolves the dependencies needed for this instance to work.
        /// </summary>
        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            this.leftJoystick = this.EntityManager.Find<Joystick>("leftJoystick");
            this.rightJoystick = this.EntityManager.Find<Joystick>("rightJoystick");
            this.bulletEmiter = this.EntityManager.Find<BulletEmiter>("bulletEmiter");
        }

        /// <summary>
        /// Allows this instance to execute custom logic during its <c>Update</c>.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        /// <remarks>
        /// This method will not be executed if the <see cref="T:WaveEngine.Framework.Component" />, or the <see cref="T:WaveEngine.Framework.Entity" />
        /// owning it are not <c>Active</c>.
        /// </remarks>
        protected override void Update(TimeSpan gameTime)
        {
            Input input = WaveServices.Input;                        

            // Player Move
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
                this.transform.X = MathHelper.Clamp(x, WaveServices.ViewportManager.LeftEdge + this.borderMargin, WaveServices.ViewportManager.RightEdge - borderMargin);

                float y = this.transform.Y + (moveDirection.Y * velocity * 60 * (float)gameTime.TotalSeconds);
                this.transform.Y = MathHelper.Clamp(y, WaveServices.ViewportManager.TopEdge + this.borderMargin, WaveServices.ViewportManager.BottomEdge - borderMargin);

                float rotation = Vector2.Angle(moveDirection, this.textureDirection);
                if (float.IsNaN(rotation))
                {
                    this.transform.Rotation = 0;
                }
                else
                {
                    this.transform.Rotation = rotation;
                }
            }

            // Player shoot           
            this.time -= gameTime;
            if (this.time <= TimeSpan.Zero)
            {
                Vector2 rightJoyDirection = this.rightJoystick.Direction;

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

                Vector2 shootDirection = new Vector2((int)rightJoyDirection.X, (int)rightJoyDirection.Y);

                // Create bullet
                if (shootDirection != Vector2.Zero)
                {
                    Vector2 ninjaPosition = new Vector2(this.transform.X, this.transform.Y);
                    this.bulletEmiter.shoot(ninjaPosition, rightJoyDirection);
                    SoundsManager.Instance.PlaySound(SoundsManager.SOUNDS.Shoot);
                }

                this.time = this.shootCadence;
            }
        }
    }
}
