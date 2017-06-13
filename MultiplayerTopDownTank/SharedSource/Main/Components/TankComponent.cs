using MultiplayerTopDownTank.Behaviors;
using System.Runtime.Serialization;
using WaveEngine.Framework;

namespace MultiplayerTopDownTank.Components
{
    [DataContract]
    public class TankComponent : Component
    {
        private int life;

        public int CurrentLive
        {
            get
            {
                return this.life;
            }

            set
            {
                this.life = value;
            }
        }

        protected override void DefaultValues()
        {
            base.DefaultValues();

            this.life = 100;
        }

        public void PrepareTank()
        {
            var entityPath = this.Owner.EntityPath;
            var cameraBehavior = this.RenderManager.ActiveCamera2D.Owner.FindComponent<CameraBehavior>(false);

            if (cameraBehavior != null)
            {
                cameraBehavior.SetTarget(entityPath);
            }
        }
    }
}