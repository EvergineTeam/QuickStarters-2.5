using System;
using System.Runtime.Serialization;
using DeepSpace.Components.Gameplay;
using DeepSpace.Components.Joystick;
using DeepSpace.Managers;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Input;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Managers;
using WaveEngine.Framework.Services;

namespace DeepSpace.Components.Player
{
    [DataContract]
    public class PlayerBehavior : Behavior
    {
        public static readonly int NumBullets = 20;

        public const float MAXSPEED = 0.5f;

        public static readonly Vector2 ACCELERATION = new Vector2(0.01f);

        private JoystickComponent leftJoystick;

        [DataMember]
        [RenderPropertyAsEntity(componentsFilter: new string[] { "DeepSpace.Components.Joystick.JoystickComponent" })]
        public string LeftJoystickEntityPath { get; set; }

        private JoystickComponent fireButton;

        [DataMember]
        [RenderPropertyAsEntity(componentsFilter: new string[] { "DeepSpace.Components.Joystick.JoystickComponent" })]
        public string FireButtonEntityPath { get; set; }

        private GameplayBehavior gamePlay;

        [DataMember]
        [RenderPropertyAsEntity(componentsFilter: new string[] { "DeepSpace.Components.Gameplay.GameplayBehavior" })]
        public string GamePlayEntityPath { get; set; }

        [RequiredComponent]
        public Transform2D Transform;

        private Vector2 speed;
        private TimeSpan timeRatio;
        private TimeSpan shootRatio;
        private VirtualScreenManager virtualScreenManager;

        public PlayerBehavior()
            : base("PlayerBehavior")
        {
        }

        protected override void DefaultValues()
        {
            this.speed = Vector2.Zero;
            this.shootRatio = TimeSpan.FromMilliseconds(250);
        }

        protected override void Initialize()
        {
            base.Initialize();

            this.virtualScreenManager = this.Owner.Scene.VirtualScreenManager;
        }

        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            var leftJoystickEntity = this.EntityManager.Find(LeftJoystickEntityPath);
            if (leftJoystickEntity != null)
            {
                this.leftJoystick = leftJoystickEntity.FindComponent<JoystickComponent>();
            }
            var fireButtonEntity = this.EntityManager.Find(FireButtonEntityPath);
            if (fireButtonEntity != null)
            {
                this.fireButton = this.EntityManager.Find(FireButtonEntityPath).FindComponent<JoystickComponent>();
            }

            var gamePlayEntity = this.EntityManager.Find(GamePlayEntityPath);
            if (gamePlayEntity != null)
            {
                this.gamePlay = this.EntityManager.Find(GamePlayEntityPath).FindComponent<GameplayBehavior>();
                this.gamePlay.Player = this.Owner;
                this.gamePlay.GameReset += this.GamePlayGameReset;
            }
        }

        protected override void DeleteDependencies()
        {
            base.DeleteDependencies();
            this.gamePlay.GameReset -= this.GamePlayGameReset;
        }

        private void GamePlayGameReset()
        {
            this.Transform.X = this.virtualScreenManager.VirtualWidth / 2;
            this.Transform.Y = this.virtualScreenManager.VirtualHeight * 0.75f;
            this.Owner.Enabled = true;
        }

        public override void ActiveNotification(bool active)
        {
            base.ActiveNotification(active);
            if (this.leftJoystick != null)
            {
                this.leftJoystick.IsDisable = !active;
            }

            if (this.fireButton != null)
            {
                this.fireButton.IsDisable = !active;
            }
        }

        protected override void Update(TimeSpan gameTime)
        {
            var keyboardState = WaveServices.Input.KeyboardState;
            float time = (float)gameTime.TotalMilliseconds;

            this.speed *= 0.85f;

            if (keyboardState.IsKeyPressed(Keys.Left))
            {
                this.speed.X -= ACCELERATION.X * time;
            }

            if (keyboardState.IsKeyPressed(Keys.Right))
            {
                this.speed.X += ACCELERATION.X * time;
            }

            if (keyboardState.IsKeyPressed(Keys.Up))
            {
                this.speed.Y -= ACCELERATION.Y * time;
            }

            if (keyboardState.IsKeyPressed(Keys.Down))
            {
                this.speed.Y += ACCELERATION.Y * time;
            }

            if (this.leftJoystick.IsPressed)
            {
                this.speed += this.leftJoystick.Direction * time * 0.01f;
            }

            if (this.speed.X > MAXSPEED)
            {
                this.speed.X = MAXSPEED;
                this.Transform.Effect = SpriteEffects.FlipHorizontally;
            }
            else if (this.speed.X < -MAXSPEED)
            {
                this.speed.X = -MAXSPEED;
                this.Transform.Effect = SpriteEffects.None;
            }

            if (this.speed.Y > MAXSPEED)
            {
                this.speed.Y = MAXSPEED;
            }
            else if (this.speed.Y < -MAXSPEED)
            {
                this.speed.Y = -MAXSPEED;
            }

            // Shooting
            if (timeRatio > TimeSpan.Zero)
            {
                timeRatio -= gameTime;
            }

            this.Transform.X += this.speed.X * time;
            this.Transform.Y += this.speed.Y * time;

            this.Transform.XScale = 1 - ((float)Math.Abs(this.speed.X * 0.4f));

            var shooting = keyboardState.IsKeyPressed(Keys.Space) || this.fireButton.IsPressed;

            if (shooting && timeRatio <= TimeSpan.Zero)
            {
                timeRatio = shootRatio;
                gamePlay.ShootBullet(true, Transform.X, Transform.Y, 0f, -15f);
            }

            // Bounds check
            if (Transform.X < 0)
            {
                Transform.X = 0;
                this.speed.X = 0;
            }

            if (Transform.Y < 0)
            {
                Transform.Y = 0;
                this.speed.Y = 0;
            }

            if (Transform.X > this.virtualScreenManager.VirtualWidth)
            {
                this.speed.X = 0;
                Transform.X = this.virtualScreenManager.VirtualWidth;
            }

            if (Transform.Y > this.virtualScreenManager.VirtualHeight)
            {
                this.speed.Y = 0;
                Transform.Y = this.virtualScreenManager.VirtualHeight;
            }
        }

        public void Explode()
        {
            this.Owner.Enabled = false;

            switch (WaveServices.Random.Next(0, 3))
            {
                case 0:
                    WaveServices.GetService<SoundManager>().PlaySound(SoundManager.SOUNDS.Explode0);
                    break;
                case 1:
                    WaveServices.GetService<SoundManager>().PlaySound(SoundManager.SOUNDS.Explode1);
                    break;
                case 2:
                    WaveServices.GetService<SoundManager>().PlaySound(SoundManager.SOUNDS.Explode2);
                    break;
            }
        }
    }
}
