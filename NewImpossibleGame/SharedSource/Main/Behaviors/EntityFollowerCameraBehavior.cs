using System;
using System.Runtime.Serialization;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;

namespace NewImpossibleGame.Behaviors
{
    /// <summary>
    /// Behavior to follow an Entity
    /// </summary>
    [DataContract]
    public class EntityFollowerCameraBehavior : Behavior
    {
        /// <summary>
        /// The transform
        /// </summary>
        [RequiredComponent]
        private Transform3D transform = null;

        /// <summary>
        /// The Transform3D entity from FollowedEntity
        /// </summary>
        private Transform3D followedEntityTransform;

        /// <summary>
        /// The Camera Offset Vector
        /// </summary>
        /// <value>
        /// The camera offset.
        /// </value>
        [DataMember]
        public Vector3 CameraOffset
        {
            get;
            set;
        }

        /// <summary>
        /// The Camera Lookat Offset
        /// </summary>
        /// <value>
        /// The camera lookat offset.
        /// </value>
        [DataMember]
        public Vector3 CameraLookatOffset
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the followed Entity.
        /// </summary>
        /// <value>
        /// The followed Entity.
        /// </value>
        [DataMember]
        [RenderPropertyAsEntity]
        public string Followed
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the followe Entity Transform. Initializes the transform on first call.
        /// </summary>
        /// <value>
        /// The followed entity transform.
        /// </value>
        public Transform3D FollowedEntityTransform
        {
            get
            {
                if (this.followedEntityTransform == null)
                {
                    this.followedEntityTransform = this.EntityManager.Find(this.Followed).FindComponent<Transform3D>();
                }

                return this.followedEntityTransform;
            }
        }

        /// <summary>
        /// Sets default values for properties and variables, those values will be override by the WaveEditor.
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();

            this.CameraOffset = new Vector3(-10, 5, -5);
            this.CameraLookatOffset = new Vector3(0, 3, 2);
        }

        /// <summary>
        /// Updates the specified game time.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        protected override void Update(TimeSpan gameTime)
        {
            this.transform.Position = this.FollowedEntityTransform.Position + this.CameraOffset;
            this.transform.LookAt(this.FollowedEntityTransform.Position + this.CameraLookatOffset);
        }
    }
}
