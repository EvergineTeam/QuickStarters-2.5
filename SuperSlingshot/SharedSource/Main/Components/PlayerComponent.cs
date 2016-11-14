using System.Runtime.Serialization;
using SuperSlingshot.Behaviors;
using SuperSlingshot.Enums;
using SuperSlingshot.Scenes;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics2D;

namespace SuperSlingshot.Components
{
    [DataContract]
    public class PlayerComponent : Component
    {
        [RequiredComponent]
        private PlayerBehavior behavior = null;

        [RequiredComponent]
        private Transform2D transform = null;

        [RequiredComponent]
        private RigidBody2D rigidBody = null;

        public void PrepareToLaunch()
        {
            // Entity RigidBosy Restitution
            // 0.858 golf ball
            // 0.804 billiard ball
            // 0.712 tennis ball
            // 0.658 glass marble
            // 0.597 steel ball bearing

            var entityPath = this.Owner.EntityPath;
            this.behavior.PlayerState = PlayerState.Prepared;

            var joint = new DistanceJoint2D()
            {
                Distance = 0.01f,
                ConnectedEntityPath = entityPath,
                FrequencyHz = 7.0f,
                DampingRatio = 1f,
                CollideConnected = false,
            };

            // Creates join and update position
            var anchorEntity = ((GameScene)this.Owner.Scene).SlingshotAnchorEntity;
            var anchorTransform = anchorEntity.FindComponent<Transform2D>();
            this.transform.Position = anchorTransform.Position;
            anchorEntity.AddComponent(joint);

            var cameraBehavior = this.RenderManager.ActiveCamera2D.Owner.FindComponent<CameraBehavior>(false);
            if (cameraBehavior != null)
            {
                cameraBehavior.SetTarget(entityPath);
            }

            this.Owner.FindComponent<MouseBehavior>().IsActive = true;
        }

        public void Launch()
        {
            var scene = (GameScene)this.Owner.Scene;
            var anchorEntity = scene.SlingshotAnchorEntity;
            var anchorTransform = anchorEntity.FindComponent<Transform2D>();
            var anchorPosition = anchorTransform.Position;

            var vector = (anchorPosition - this.transform.Position) / 15;

            var joint = anchorEntity.FindComponent<DistanceJoint2D>();
            anchorEntity.RemoveComponent<DistanceJoint2D>();

            this.rigidBody.ApplyLinearImpulse(vector, Vector2.Center);
            this.behavior.PlayerState = PlayerState.InTheAir;

            this.Owner.FindComponent<MouseBehavior>().IsActive = false;
        }
    }
}
