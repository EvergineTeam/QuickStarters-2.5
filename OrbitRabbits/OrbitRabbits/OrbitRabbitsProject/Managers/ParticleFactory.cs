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

namespace OrbitRabbitsProject.Entities.Particles
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
                NumParticles = 150,
                EmitRate = 130,
                MinLife = TimeSpan.FromSeconds(1f),
                MaxLife = TimeSpan.FromSeconds(3f),
                LocalVelocity = new Vector2(0.2f, 3f),
                RandomVelocity = new Vector2(2f, 0.4f),
                MinSize = 12,
                MaxSize = 24,
                MinRotateSpeed = 0.1f,
                MaxRotateSpeed = -0.1f,
                EndDeltaScale = 0.0f,
                EmitterSize = new Vector3(5),
                EmitterShape = ParticleSystem2D.Shape.FillCircle,
                Emit = false
            };
        }

        /// <summary>
        /// Creates the explosion.
        /// </summary>
        /// <returns></returns>
        public static ParticleSystem2D CreateExplosion()
        {
            ParticleSystem2D explosionParticle = new ParticleSystem2D()
            {
                NumParticles = 100,
                EmitRate = 1500,
                MinLife = TimeSpan.FromSeconds(1f),
                MaxLife = TimeSpan.FromSeconds(3f),
                LocalVelocity = new Vector2(0f, 0f),
                RandomVelocity = new Vector2(3f, 2.5f),
                MinSize = 15,
                MaxSize = 40,
                MinRotateSpeed = 0.03f,
                MaxRotateSpeed = -0.03f,
                EndDeltaScale = 0f,
                EmitterSize = new Vector3(30),
                Gravity = new Vector2(0, 0.03f),
                EmitterShape = ParticleSystem2D.Shape.FillCircle,
                Emit = false,
            };

            return explosionParticle;
        }
    }
}
