using System.Runtime.Serialization;
using SuperSlingshot.Enums;
using WaveEngine.Framework;
using WaveEngine.Components.GameActions;
using System;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.Graphics;

namespace SuperSlingshot.Components
{
    [DataContract]
    public class StarScoreComponent : Component
    {
        private StarScoreEnum score;

        private SwitchComponent star1Component;
        private SwitchComponent star2Component;
        private SwitchComponent star3Component;

        [DataMember]
        public StarScoreEnum Score
        {
            get
            {
                return this.score;
            }

            set
            {
                this.score = value;
                this.SetScore();
            }
        }

        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            var star1entity = this.Owner.FindChild("star1").FindChild("content");
            var star2entity = this.Owner.FindChild("star2").FindChild("content");
            var star3entity = this.Owner.FindChild("star3").FindChild("content");

            if (star1entity != null)
            {
                this.star1Component = star1entity.FindComponent<SwitchComponent>();
            }

            if (star2entity != null)
            {
                this.star2Component = star2entity.FindComponent<SwitchComponent>();
            }

            if (star3entity != null)
            {
                this.star3Component = star3entity.FindComponent<SwitchComponent>();
            }

            this.Score = StarScoreEnum.THREE;
        }

        private void SetScore()
        {
            switch (this.score)
            {
                case StarScoreEnum.NONE:
                    this.SetStates(SwitchEnum.OFF, SwitchEnum.OFF, SwitchEnum.OFF);
                    break;
                case StarScoreEnum.ONE:
                    this.SetStates(SwitchEnum.ON, SwitchEnum.OFF, SwitchEnum.OFF);
                    break;
                case StarScoreEnum.TWO:
                    this.SetStates(SwitchEnum.ON, SwitchEnum.ON, SwitchEnum.OFF);
                    break;
                case StarScoreEnum.THREE:
                    this.SetStates(SwitchEnum.ON, SwitchEnum.ON, SwitchEnum.ON);
                    break;
                default:
                    break;
            }
        }

        private void SetStates(SwitchEnum star1, SwitchEnum star2, SwitchEnum star3)
        {
            float delay = 0;
            float delta = 0.5f;

            IGameAction animations = null;

            if (this.Owner != null && this.Owner.Scene != null)
            {
                animations = this.Owner.Scene.CreateEmptyGameAction();
            }

            if (this.star1Component != null)
            {
                this.star1Component.State = star1;

                if (star1 == SwitchEnum.ON)
                {
                    delay += delta;
                    animations.ContinueWith(this.CreateStarAnimation(this.star1Component.Owner, delay));
                }
            }

            if (this.star2Component != null)
            {
                this.star2Component.State = star2;

                if (star2 == SwitchEnum.ON)
                {
                    delay += delta;
                    animations.ContinueWith(this.CreateStarAnimation(this.star2Component.Owner, delay));
                }
            }

            if (this.star3Component != null)
            {
                this.star3Component.State = star3;

                if (star3 == SwitchEnum.ON)
                {
                    delay += delta;
                    animations.ContinueWith(this.CreateStarAnimation(this.star3Component.Owner, delay));
                }
            }

            if (animations != null)
            {
                animations.Run();
            }
        }

        private IGameAction CreateStarAnimation(Entity entity, float delayTime = 0)
        {
            return this.Owner.Scene.CreateGameActionFromAction(() =>
            {
                var transform = entity.FindComponent<Transform2D>();
                if (transform != null)
                {
                    transform.Scale = Vector2.Zero;
                }
            })
            .Delay(TimeSpan.FromSeconds(delayTime))
            .ContinueWith(new ScaleTo2DGameAction(entity, Vector2.One, TimeSpan.FromSeconds(0.6f), EaseFunction.BounceOutEase, true));
        }
    }
}
