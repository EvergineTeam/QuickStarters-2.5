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
using System.Text;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics; 
#endregion

namespace FlyingKiteProject.Behaviors
{
    public class Follower2DBehavior : Behavior
    {
        public enum FollowTypes
        {
            X,
            Y,
            XY
        }

        [RequiredComponent]
        private Transform2D transform2D;

        private Transform2D followedTranform;
        private FollowTypes followType;

        private Vector2 lastFollowPosition;

        public Follower2DBehavior(Entity entity, FollowTypes followType)
        {
            this.followedTranform = entity.FindComponent<Transform2D>();
            this.followType = followType;

            this.lastFollowPosition = Vector2.Zero;

            if (this.lastFollowPosition == null)
            {
                throw new NotImplementedException("The Transform2D component must be used by the entity to follow");
            }
        }

        protected override void Initialize()
        {
            base.Initialize();

            this.lastFollowPosition = new Vector2(this.followedTranform.X, this.followedTranform.Y);
        }

        protected override void Update(TimeSpan gameTime)
        {
            var followedPosition = new Vector2(this.followedTranform.X, this.followedTranform.Y);
            var increment = (followedPosition - this.lastFollowPosition);
            this.lastFollowPosition = followedPosition;

            if (this.followType == FollowTypes.X || this.followType == FollowTypes.XY)
            {
                this.transform2D.X += increment.X;
            }

            if (this.followType == FollowTypes.Y || this.followType == FollowTypes.XY)
            {
                this.transform2D.Y += increment.Y;
            }
        }
    }
}
