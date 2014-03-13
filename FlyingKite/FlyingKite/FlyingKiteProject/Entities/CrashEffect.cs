#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Components.UI;
using WaveEngine.Framework;
using WaveEngine.Framework.UI;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.Graphics;
using WaveEngine.Common.Graphics;
using WaveEngine.Framework.Animation;
#endregion

namespace FlyingKiteProject.Entities
{
    public class CrashEffect : BaseDecorator
    {
        private SingleAnimation fadeOutAnimation;

        public CrashEffect()
        {
            this.entity = new Entity()
                                .AddComponent(new Transform2D()
                                {
                                    X = WaveServices.ViewportManager.LeftEdge,
                                    Y = WaveServices.ViewportManager.TopEdge,
                                    XScale = 1 / WaveServices.ViewportManager.RatioX,
                                    YScale = 1 / WaveServices.ViewportManager.RatioY,
                                    Opacity = 0
                                })
                                .AddComponent(new ImageControl(
                                    Color.White,
                                    (int)WaveServices.ViewportManager.ScreenWidth,
                                    (int)WaveServices.ViewportManager.ScreenHeight))
                                .AddComponent(new ImageControlRenderer(DefaultLayers.GUI))
                                .AddComponent(new AnimationUI());

            this.fadeOutAnimation = new SingleAnimation(1, 0, TimeSpan.FromMilliseconds(200));
        }

        public void DoEffect()
        {
            this.entity.FindComponent<AnimationUI>().BeginAnimation(Transform2D.OpacityProperty, this.fadeOutAnimation);
        }
    }
}
