using System;
using System.Runtime.Serialization;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics2D;

namespace SuperSlingshot.Behaviors
{
    /// <summary>
    /// Auto throwing behavior
    /// </summary>
    [DataContract]
    public class AutoThrowBehavior : Behavior
    {
        private TimeSpan currentTime;
        private bool slept;

        [DataMember]
        public float CountdownTime { get; set; }

        [DataMember]
        public float SleepTime { get; set; }

        [DataMember]
        public Vector2 InitialPosition { get; set; }

        [DataMember]
        public Vector2 Impulse { get; set; }

        [RequiredComponent]
        private Transform2D transform { get; set; }

        [RequiredComponent]
        private RigidBody2D rigidBody { get; set; }

        /// <summary>
        /// Resolve dependencies
        /// </summary>
        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();
            this.currentTime = TimeSpan.FromSeconds(this.CountdownTime);
        }

        /// <summary>
        /// Update method. Counts the time to next throwing
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Update(TimeSpan gameTime)
        {
            this.currentTime -= gameTime;

            if (this.currentTime < TimeSpan.Zero)
            {
                this.slept = false;
                this.ReSpawn();
            }

            if(this.currentTime < TimeSpan.FromSeconds(this.SleepTime) && !this.slept)
            {
                this.slept = true;
                this.Sleep();
            }
        }

        /// <summary>
        /// Respawn method
        /// </summary>
        private void ReSpawn()
        {
            this.currentTime = TimeSpan.FromSeconds(this.CountdownTime);
            this.rigidBody.ResetPosition(this.InitialPosition);
            this.rigidBody.ApplyLinearImpulse(this.Impulse, Vector2.Zero, true);
        }

        /// <summary>
        /// Sleeps the rigid body
        /// </summary>
        private void Sleep()
        {
            this.rigidBody.Awake = false;
        }
    }
}
