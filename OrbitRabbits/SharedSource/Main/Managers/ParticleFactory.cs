#region File Description
//-----------------------------------------------------------------------------
// OrbitRabbits
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
using System.Threading.Tasks;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Components.Particles; 
#endregion

namespace OrbitRabbits.Entities.Particles
{
    public static class ParticleFactory
    {
        /// <summary>
        /// Creates the stars particle.
        /// </summary>
        /// <returns></returns>
        public static ParticleSystem2D CreateStarsParticle()
        {
            return new ParticleSystem2D()
            {
                NumParticles = 75,
                EmitRate = 130,
                MinLife = 0.5f,
                MaxLife = 1.5f,
                LocalVelocity = new Vector2(0.1f, 2f),
                RandomVelocity = new Vector2(1f, 0.2f),
                MinSize = 12,
                MaxSize = 24,
                MinRotateSpeed = 0.1f,
                MaxRotateSpeed = -0.1f,
                EndDeltaScale = 0.0f,
                EmitterSize = new Vector3(5),
                EmitterShape = ParticleSystem2D.Shape.FillCircle,
                LinearColorEnabled = true,
                InterpolationColors = new List<WaveEngine.Common.Graphics.Color>() { Color.White, Color.Black },
                Emit = false
            };
        }
    }
}
