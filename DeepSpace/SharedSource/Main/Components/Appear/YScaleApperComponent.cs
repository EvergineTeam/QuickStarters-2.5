using System.Runtime.Serialization;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;

namespace DeepSpace.Components.Appear
{
    [DataContract]
    public class YScaleApperComponent : BaseAppearAnimationComponent
    {
        protected override DependencyProperty AnimationProperty
        {
            get
            {
                return Transform2D.YScaleProperty;
            }
        }

        public YScaleApperComponent() : base("YScaleApperComponent")
        {

        }
    }
}
