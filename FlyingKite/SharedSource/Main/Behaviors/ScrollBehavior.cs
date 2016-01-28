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
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using WaveEngine.Common.Attributes;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Managers;
using WaveEngine.Framework.Services;
#endregion

namespace FlyingKite.Behaviors
{
    [DataContract]
    public class ScrollBehavior : Behavior
    {
        [RequiredComponent]
        private Transform2D transform2D = null;

        private VirtualScreenManager virtualScreenManager;

        /// <summary>
        /// Gets or sets the scroll velocity expressed in pixels/second.
        /// </summary>
        /// <value>
        /// The scroll velocity.
        /// </value>
        [DataMember]
        [RenderPropertyAsFInput]
        public float ScrollVelocity
        {
            get;
            set;
        }

        [DataMember]
        public bool DisplaceWhenOutOfScreen
        {
            get;
            set;
        }

        public event EventHandler<Entity> OnEntityOutOfScreen;

        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            this.virtualScreenManager = this.Owner.Scene.VirtualScreenManager;
        }

        protected override void DefaultValues()
        {
            base.DefaultValues();

            this.ScrollVelocity = 10;
        }

        protected override void Update(TimeSpan gameTime)
        {
            this.transform2D.X -= this.ScrollVelocity * (float)gameTime.TotalSeconds;

            var trueWidth = this.transform2D.Rectangle.Width * this.transform2D.XScale;

            var rightSize = this.transform2D.X + trueWidth;

            if (rightSize < this.virtualScreenManager.LeftEdge)
            {
                if (this.DisplaceWhenOutOfScreen)
                {
                    this.transform2D.X = this.virtualScreenManager.RightEdge + this.virtualScreenManager.ScreenWidth;
                }

                this.RaiseOnEntityOutOfScreen(this.Owner);
            }
        }

        private void RaiseOnEntityOutOfScreen(Entity entity)
        {
            if (this.OnEntityOutOfScreen != null)
            {
                this.OnEntityOutOfScreen(this, entity);
            }
        }
    }
}
