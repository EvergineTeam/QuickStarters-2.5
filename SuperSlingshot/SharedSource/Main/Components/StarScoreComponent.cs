using System.Runtime.Serialization;
using SuperSlingshot.Enums;
using WaveEngine.Framework;

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
            if (this.star1Component != null)
            {
                this.star1Component.State = star1;
            }

            if (this.star2Component != null)
            {
                this.star2Component.State = star2;
            }

            if (this.star3Component != null)
            {
                this.star3Component.State = star3;
            }
        }
    }
}
