#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Managers; 
#endregion

namespace FlyingKiteProject.Layers
{
    public class ObstaclesLayer : AlphaLayer
    {
        public ObstaclesLayer(RenderManager renderManager)
            : base(renderManager)
        {
        }
    }
}
