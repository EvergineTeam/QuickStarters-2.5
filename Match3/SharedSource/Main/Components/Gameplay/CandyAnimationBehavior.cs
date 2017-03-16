using System;
using System.Runtime.Serialization;
using Match3.Gameboard;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using Match3.Helpers;
using Match3.Services;
using WaveEngine.Components.Animation;
using System.Linq;
using WaveEngine.Framework.Animation;

namespace Match3.Components.Gameplay
{
    [DataContract]
    public class CandyAnimationBehavior : Behavior
    {
        private enum AnimationStates
        {
            None,
            Moving,
            Appear,
            Disappear
        }

        [RequiredComponent]
        protected Transform2D transform2D;

        [RequiredComponent]
        protected CandyAttributesComponent attributes;

        private Animation2D explisionAnimation2D;

        private Vector2 destinationPosition;

        private Vector2 destinationScale;

        private AnimationStates animationState;

        public bool IsAnimating
        {
            get
            {
                return this.animationState != AnimationStates.None;
            }
        }

        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            this.explisionAnimation2D = this.Owner.FindChild("Particles")
                      .FindComponent<Animation2D>();
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void Update(TimeSpan gameTime)
        {
            var timeFactor = (float)gameTime.TotalSeconds;
            var amount = MathHelper.Clamp(timeFactor * 33, 0, 1);

            if (this.animationState == AnimationStates.Moving)
            {
                this.transform2D.LocalPosition = Vector2.Lerp(this.transform2D.LocalPosition, this.destinationPosition, amount);

                if (this.destinationPosition == this.transform2D.LocalPosition)
                {
                    this.animationState = AnimationStates.None;
                }
            }
            else if (this.animationState == AnimationStates.Appear)
            {
                this.transform2D.LocalScale = Vector2.Lerp(this.transform2D.LocalScale, this.destinationScale, amount); ;

                if (this.destinationScale == this.transform2D.LocalScale)
                {
                    this.animationState = AnimationStates.None;
                }
            }
            else if (this.animationState == AnimationStates.Disappear)
            {
                if (this.explisionAnimation2D.State == AnimationState.Stopped)
                {
                    this.animationState = AnimationStates.None;
                }
            }
        }

        public void RefreshPositionAnimation()
        {
            var gameLogic = CustomServices.GameLogic;
            this.destinationPosition = gameLogic.CalculateCandyPostion(this.attributes.Coordinate);
            this.animationState = AnimationStates.Moving;
        }

        public void SetAppearAnimation()
        {
            this.transform2D.LocalScale = Vector2.Zero;
            this.destinationScale = Vector2.One;
            this.animationState = AnimationStates.Appear;
        }

        public void SetDisappearAnimation()
        {
            var animationName = this.explisionAnimation2D.CurrentAnimation;
            this.Owner.FindComponent<Drawable2D>(false).IsVisible = false;
            this.explisionAnimation2D.Owner.IsVisible = true;
            this.explisionAnimation2D.PlayAnimation(animationName, loop: false);
            this.animationState = AnimationStates.Disappear;
        }

        public void AnimateToShuffle()
        {
            this.destinationPosition = Vector2.Zero;
            this.animationState = AnimationStates.Moving;
        }

        public void AnimateFromShuffle()
        {
            this.transform2D.LocalPosition = Vector2.Zero;
            this.RefreshPositionAnimation();
        }
    }
}
