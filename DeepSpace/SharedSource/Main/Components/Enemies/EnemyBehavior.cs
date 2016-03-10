using System;
using System.Runtime.Serialization;
using DeepSpace.Components.Gameplay;
using DeepSpace.Managers;
using WaveEngine.Common.Attributes;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Managers;
using WaveEngine.Framework.Services;

namespace DeepSpace.Components.Enemies
{
    public class EnemyBehavior : Behavior
    {
        [RequiredComponent]
        private Transform2D transform;

        private TimeSpan timeRatio;
        private TimeSpan shootRatio;

        private float speed;
        private int difficultyLevel;

        private GameplayBehavior gamePlay;
        private VirtualScreenManager virtualScreenManager;

        [DataMember]
        [RenderPropertyAsEntity(componentsFilter: new string[] { "DeepSpace.Components.Gameplay.GameplayBehavior" })]
        public string GamePlayEntityPath { get; set; }

        public EnemyBehavior()
            : base()
        {
            this.transform = null;
            this.speed = 2;
            this.shootRatio = TimeSpan.FromMilliseconds(WaveServices.Random.Next(1100, 1600));
        }

        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            this.gamePlay = this.EntityManager.Find(GamePlayEntityPath).FindComponent<GameplayBehavior>();
        }

        protected override void Initialize()
        {
            base.Initialize();

            this.virtualScreenManager = this.Owner.Scene.VirtualScreenManager;
        }

        protected override void Update(TimeSpan gameTime)
        {
            if (this.gamePlay.State == GameState.Intro)
            {
                return;
            }

            float time = (float)gameTime.TotalMilliseconds / 10;

            transform.Y += speed * time;

            // Shooting
            if (timeRatio > TimeSpan.Zero)
            {
                timeRatio -= gameTime;
            }
            else
            {
                timeRatio = shootRatio;
                gamePlay.ShootBullet(false, transform.X, transform.Y + 50, 0f, 10f);
            }

            // Reset position
            if (transform.Y > this.virtualScreenManager.VirtualHeight + transform.Rectangle.Height)
            {
                this.Reset();
            }
        }

        public void Reset()
        {
            difficultyLevel++;
            var deepPosition = (int)-this.virtualScreenManager.VirtualHeight * 3;
            transform.Y = deepPosition;
            transform.X = WaveServices.Random.Next((int)this.virtualScreenManager.VirtualWidth);

            speed = speed + (float)WaveServices.Random.NextDouble() / 2;

            if (speed > 7)
            {
                speed = WaveServices.Random.Next(2, 5);
            }
        }

        public void Explode()
        {
            this.Reset();

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
