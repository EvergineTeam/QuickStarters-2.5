using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using WaveEngine.Framework;
using WaveEngine.Framework.Animation;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics2D;

namespace SuperSquid.Components
{
    [DataContract(Namespace = "SuperSquid.Components")]
    public class JellyFishController : Component
    {
        [RequiredComponent]
        private AnimationUI animation = null;

        [RequiredComponent]
        private Transform2D transform = null;

        [RequiredComponent(false)]
        public Collider2D Collider = null;

        private SingleAnimation leftAnim, rightAnim;
        
        [DataMember]
        public bool RightAnimation
        {
            get;
            set;
        }

        protected override void Initialize()
        {
            base.Initialize();

            var position = this.transform.Position;

            // Animations
            float offset = 80;
            this.leftAnim = new SingleAnimation(position.X + offset, position.X, TimeSpan.FromSeconds(3));
            this.leftAnim.Completed += (s, o) =>
            {
                this.animation.BeginAnimation(Transform2D.XProperty, this.rightAnim);
            };
            this.rightAnim = new SingleAnimation(position.X, position.X + offset, TimeSpan.FromSeconds(3));
            this.rightAnim.Completed += (s, o) =>
            {
                this.animation.BeginAnimation(Transform2D.XProperty, this.leftAnim);
            };

            if (this.RightAnimation)
            {
                this.animation.BeginAnimation(Transform2D.XProperty, this.rightAnim);
            }
            else
            {
                this.animation.BeginAnimation(Transform2D.XProperty, this.leftAnim);
            }

        }
    }
}
