using System;
using System.Runtime.Serialization;
using WaveEngine.Framework;
using WaveEngine.Framework.Animation;

namespace DeepSpace.Components.Appear
{
    [DataContract]
    public abstract class BaseAppearAnimationComponent : Component
    {
        [DataMember]
        public float From { get; set; }

        [DataMember]
        public float To { get; set; }

        [DataMember]
        public float DurationSeconds { get; set; }

        [DataMember]
        public EasingFunctions Function { get; set; }

        [RequiredComponent]
        protected AnimationUI animationUI;

        protected abstract DependencyProperty AnimationProperty { get; }

        protected BaseAppearAnimationComponent(string name) : base(name)
        {
        }

        protected override void DefaultValues()
        {
            base.DefaultValues();

            this.From = 0.0f;
            this.To = 1.0f;
            this.DurationSeconds = 1.0f;
        }

        protected override void Initialize()
        {
            base.Initialize();
            this.Owner.EntityInitialized += this.OwnerEntityInitialized;
        }

        protected override void DeleteDependencies()
        {
            base.DeleteDependencies();
            this.Owner.EntityInitialized -= this.OwnerEntityInitialized;
        }

        private void OwnerEntityInitialized(object sender, EventArgs e)
        {
            this.Owner.EntityInitialized -= this.OwnerEntityInitialized;

            var animation = new SingleAnimation(this.From, this.To, TimeSpan.FromSeconds(this.DurationSeconds), EasingFunctions.Cubic);
            this.animationUI.BeginAnimation(this.AnimationProperty, animation);
        }
    }
}
