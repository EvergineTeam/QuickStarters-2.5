#region File Description
//-----------------------------------------------------------------------------
// Super Squid
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
using WaveEngine.Framework.Services;
#endregion

namespace SuperSquidProject.Managers
{
    public static class ParticleFactory
    {
        public static ParticleSystem2D CreateWaterParticles()
        {
            return new ParticleSystem2D()
            {
                NumParticles = 200,
                EmitRate = 20,
                MinLife = TimeSpan.FromSeconds(3),
                MaxLife = TimeSpan.FromSeconds(10),
                LocalVelocity = new Vector2(0f, 0.8f),
                RandomVelocity = new Vector2(0.2f, 0.2f),
                MinSize = 4,
                MaxSize = 7,
                EndDeltaScale = 1f,
                LinearColorEnabled = true,
                InterpolationColors = new List<Color>() { Color.White, Color.Transparent },
                EmitterSize = new Vector2(WaveServices.ViewportManager.VirtualWidth, 
                                          WaveServices.ViewportManager.VirtualHeight),
                EmitterShape = ParticleSystem2D.Shape.FillRectangle,
            };
        }

        public static ParticleSystem2D CreateBubbleParticles()
        {
            return new ParticleSystem2D()
            {
                NumParticles = 60,
                EmitRate = 500,
                MinLife = TimeSpan.FromSeconds(1f),
                MaxLife = TimeSpan.FromSeconds(1.3f),
                LocalVelocity = new Vector2(0.2f, 6f),
                RandomVelocity = new Vector2(3f, 1f),
                MinSize = 5,
                MaxSize = 8,                     
                EmitterSize = new Vector2(80,20),
                EmitterShape = ParticleSystem2D.Shape.FillRectangle,
                Emit = true
            };
        }
    }
}
