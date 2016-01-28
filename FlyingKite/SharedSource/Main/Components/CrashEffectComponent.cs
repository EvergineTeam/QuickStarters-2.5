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
using System.Runtime.Serialization;
using System.Text;
using WaveEngine.Framework;
using WaveEngine.Framework.Animation;
using WaveEngine.Framework.Graphics;
#endregion

namespace FlyingKite.Components
{
    [DataContract]
    public class CrashEffectComponent : Component
    {
        [RequiredComponent]
        private AnimationUI animationUI = null;

        private SingleAnimation fadeOutAnimation;
        
        protected override void DefaultValues()
        {
            base.DefaultValues();

            this.fadeOutAnimation = new SingleAnimation(1, 0, TimeSpan.FromMilliseconds(200));
        }

        /// <summary>
        /// Does the crash effect.
        /// </summary>
        public void DoEffect()
        {
            this.animationUI.BeginAnimation(Transform2D.OpacityProperty, this.fadeOutAnimation);
        }
    }
}
