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
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
#endregion

namespace FlyingKite.Behaviors
{
    [DataContract]
    public class Follower2DBehavior : Behavior
    {
        public enum FollowTypes
        {
            X,
            Y,
            XY
        }

        [RequiredComponent]
        private Transform2D transform2D = null;

        private string entityToFollowPath;
        private Transform2D followedTranform;

        private Vector2 lastFollowPosition;

        [DataMember]
        public FollowTypes FollowType
        {
            get;
            set;
        }

        [DataMember]
        [RenderPropertyAsEntity(new String[] { "WaveEngine.Framework.Graphics.Transform2D" })]
        public string EntityToFollow
        {
            get
            {
                return this.entityToFollowPath;
            }

            set
            {
                this.entityToFollowPath = value;

                if (this.isInitialized)
                {
                    this.RefreshEntities();
                }
            }
        }

        public Follower2DBehavior()
        {
            this.lastFollowPosition = Vector2.Zero;
        }

        protected override void Initialize()
        {
            base.Initialize();

            this.RefreshEntities();

            this.lastFollowPosition = new Vector2(this.followedTranform.X, this.followedTranform.Y);
        }

        protected override void Update(TimeSpan gameTime)
        {
            var followedPosition = new Vector2(this.followedTranform.X, this.followedTranform.Y);
            var increment = (followedPosition - this.lastFollowPosition);
            this.lastFollowPosition = followedPosition;

            if (this.FollowType == FollowTypes.X || this.FollowType == FollowTypes.XY)
            {
                this.transform2D.X += increment.X;
            }

            if (this.FollowType == FollowTypes.Y || this.FollowType == FollowTypes.XY)
            {
                this.transform2D.Y += increment.Y;
            }
        }

        private void RefreshEntities()
        {
            var entityToFollow = this.EntityManager.Find(this.entityToFollowPath);

            if (entityToFollow != null)
            {
                this.followedTranform = entityToFollow.FindComponent<Transform2D>();
            }
        }
    }
}
