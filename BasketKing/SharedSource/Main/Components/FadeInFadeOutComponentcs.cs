using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using WaveEngine.Framework;
using WaveEngine.Framework.Animation;
using WaveEngine.Framework.Graphics;

namespace BasketKing.Components
{
    [DataContract(Namespace = "BasketKing.Components")]
    public class FadeInFadeOutComponentcs : Component
    {
        [RequiredComponent]
        private Transform2D transform;

        [RequiredComponent]
        private AnimationUI animationUI;

        private float speed;
        private SingleAnimation fadeIn;
        private SingleAnimation fadeOut;

        [DataMember]
        public float Speed
        {
            get
            {
                return speed;
            }
            set
            {
                this.speed = value;

                if(this.isInitialized)
                {
                    this.RefreshAnimations();
                }
            }
        }

        protected override void DefaultValues()
        {
            base.DefaultValues();
            this.speed = 1;
        }

        protected override void Initialize()
        {
            base.Initialize();
            this.RefreshAnimations();
        }

        private void RefreshAnimations()
        {
            if(this.fadeIn != null)
            {
                this.fadeIn.Completed -= this.FadeIn_Completed;
            }

            if (this.fadeOut != null)
            {
                this.fadeOut.Completed -= this.FadeOut_Completed;
            }

            Duration duration = TimeSpan.FromSeconds(this.Speed);
            this.fadeIn = new SingleAnimation(0, 1, duration);
            this.fadeOut = new SingleAnimation(1, 0, duration);

            this.fadeIn.Completed += this.FadeIn_Completed;
            this.fadeOut.Completed += this.FadeOut_Completed;

            this.animationUI.BeginAnimation(Transform2D.OpacityProperty, this.fadeIn);
        }

        private void FadeOut_Completed(object sender, EventArgs e)
        {
            this.animationUI.BeginAnimation(Transform2D.OpacityProperty, this.fadeIn);
        }

        private void FadeIn_Completed(object sender, EventArgs e)
        {
            this.animationUI.BeginAnimation(Transform2D.OpacityProperty, this.fadeOut);
        }

    }
}
