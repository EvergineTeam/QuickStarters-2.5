#region File Description
//-----------------------------------------------------------------------------
// Super Squid
//
// Quickstarter for Wave University Tour 2014.
// Author: Wave Engine Team
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using SuperSquidProject.Commons;
using SuperSquidProject.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Components.Particles;
using WaveEngine.Components.UI;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
using WaveEngine.Materials; 
#endregion

namespace SuperSquidProject.Scenes
{
    public class BackgroundScene : Scene
    {
        private readonly Color backgroundColor = new Color(0 / 255f, 31 / 255f, 39 / 255f);

        protected override void CreateScene()
        {
            // Background color
            RenderManager.BackgroundColor = this.backgroundColor;

            // Water particles
            Entity waterParticles = new Entity("waterParticles")
                                        .AddComponent(new Transform2D()
                                        {
                                            X = WaveServices.ViewportManager.VirtualWidth / 2,
                                            Y = WaveServices.ViewportManager.VirtualHeight / 2,                          
                                        })
                                        .AddComponent(ParticleFactory.CreateWaterParticles())
                                        .AddComponent(new Material2D(new BasicMaterial2D(Directories.TexturePath + "waterParticle.wpk")))
                                        .AddComponent(new ParticleSystemRenderer2D("waterParticles", DefaultLayers.Additive));
            EntityManager.Add(waterParticles);

            Entity waterParticles2 = new Entity("waterParticles2")
                                        .AddComponent(new Transform2D()
                                        {
                                            X = WaveServices.ViewportManager.VirtualWidth / 2,
                                            Y = WaveServices.ViewportManager.VirtualHeight / 2,
                                        })
                                        .AddComponent(ParticleFactory.CreateWaterParticles())
                                        .AddComponent(new Material2D(new BasicMaterial2D(Directories.TexturePath + "waterParticle2.wpk")))
                                        .AddComponent(new ParticleSystemRenderer2D("waterParticles2", DefaultLayers.Additive));
            EntityManager.Add(waterParticles2);
        }
    }
}
