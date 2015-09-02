#region Using Statements
using System;
using System.Collections.Generic;
using WaveEngine.Common;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Components.Cameras;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Components.Graphics3D;
using WaveEngine.Components.Particles;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Resources;
using WaveEngine.Framework.Services;
#endregion

namespace SuperSquid.Scenes
{
    public class BackgroundScene : Scene
    {
        protected override void CreateScene()
        {
            this.Load(WaveContent.Scenes.BackgroundScene);
            
            this.EntityManager.Find("defaultCamera2D").FindComponent<Camera2D>().CenterScreen();

            var interpolationColors = new List<Color>() { Color.Transparent, Color.White, Color.White, Color.Transparent };
            this.EntityManager.Find("waterParticles").FindComponent<ParticleSystem2D>().InterpolationColors = interpolationColors;
            this.EntityManager.Find("waterParticles2").FindComponent<ParticleSystem2D>().InterpolationColors = interpolationColors;
        }
    }
}
