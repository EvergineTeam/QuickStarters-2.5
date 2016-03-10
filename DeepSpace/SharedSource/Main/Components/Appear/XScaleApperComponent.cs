using System.Runtime.Serialization;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;

namespace DeepSpace.Components.Appear
{
    [DataContract]
    public class XScaleApperComponent : BaseAppearAnimationComponent
    {
        protected override DependencyProperty AnimationProperty
        {
            get
            {
                return Transform2D.XScaleProperty;
            }
        }

        public XScaleApperComponent() : base("XScaleApperComponent")
        {

        }
    }
}
