#region File Description
//-----------------------------------------------------------------------------
// Flying Kite
//
// Quickstarter for Wave University Tour 2014.
// Author: Wave Engine Team
//-----------------------------------------------------------------------------
#endregion

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
        /// <summary>
        /// Initializes a new instance of the <see cref="ObstaclesLayer" /> class.
        /// </summary>
        /// <param name="renderManager">The render manager.</param>
        public ObstaclesLayer(RenderManager renderManager)
            : base(renderManager)
        {
        }
    }
}
