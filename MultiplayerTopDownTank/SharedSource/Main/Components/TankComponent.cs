using MultiplayerTopDownTank.Behaviors;
using System.Runtime.Serialization;
using WaveEngine.Framework;

namespace MultiplayerTopDownTank.Components
{
    [DataContract]
    public class TankComponent : Component
    {
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