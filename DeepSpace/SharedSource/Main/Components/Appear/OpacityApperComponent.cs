using System.Runtime.Serialization;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;

namespace DeepSpace.Components.Appear
{
    [DataContract]
    public class OpacityApperComponent : BaseAppearAnimationComponent
    {
        protected override DependencyProperty AnimationProperty
        {
            get
            {
                return Transform2D.OpacityProperty;
            }
        }

        public OpacityApperComponent() : base("OpacityApperComponent")
        {

        }
    }
}
