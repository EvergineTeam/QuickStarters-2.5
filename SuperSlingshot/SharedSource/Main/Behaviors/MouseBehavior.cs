#region Using Statements
using System;
using System.Runtime.Serialization;
using SlingshotRampage.Services;
using SuperSlingshot.Components;
using WaveEngine.Common;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Input;
using WaveEngine.Common.Math;
using WaveEngine.Components.Cameras;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Components.Graphics3D;
using WaveEngine.Framework;
using WaveEngine.Framework.Diagnostic;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Managers;
using WaveEngine.Framework.Physics2D;
using WaveEngine.Framework.Resources;
using WaveEngine.Framework.Services;
#endregion

namespace SuperSlingshot.Behaviors
{
    /// <summary>
    /// Touch contol behavior
    /// </summary>
    [DataContract]
    public class MouseBehavior : Behavior
    {
        private Input input;
        private VirtualScreenManager vsm;
        private TouchPanelState touchState;
        
        public Vector2 TouchPosition;
        public Vector2 WorldPosition;
        public Entity ConnectedEntity;
        public MouseJoint2D mouseJoint;

        /// <summary>
        /// Dafault values
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();

            this.TouchPosition = Vector2.Zero;
            this.WorldPosition = Vector2.Zero;
        }

        /// <summary>
        /// Resolve dependencies
        /// </summary>
        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();
            this.vsm = this.Owner.Scene.VirtualScreenManager;
        }

        /// <summary>
        /// Update method
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Update(TimeSpan gameTime)
        {
            this.input = WaveServices.Input;

            var activeCamera2D = this.RenderManager.ActiveCamera2D;

            if (this.input.TouchPanelState.IsConnected)
            {
                this.touchState = this.input.TouchPanelState;

                if (this.touchState.Count > 0 && this.mouseJoint == null)
                {
                    // Gets the virtual screen touch position
                    this.TouchPosition = this.touchState[0].Position;
                    this.vsm.ToVirtualPosition(ref this.TouchPosition);

                    // Launchs a ray from the virtual screen position
                    Ray r;
                    activeCamera2D.CalculateRay(ref this.TouchPosition, out r);
                    this.WorldPosition = r.IntersectionZPlane(0).ToVector2();

                    /// check collision with DRAGGABLE entities only
                    foreach (Entity entity in this.Owner.Scene.EntityManager.FindAllByTag(GameConstants.TAGDRAGGABLE))
                    {
                        Collider2D collider = entity.FindComponent<Collider2D>(false);
                        if (collider != null)
                        {
                            if (collider.Contain(this.WorldPosition))
                            {
                                RigidBody2D rigidBody = entity.FindComponent<RigidBody2D>();
                                if (rigidBody != null)
                                {
                                    if (rigidBody.PhysicBodyType == WaveEngine.Common.Physics2D.RigidBodyType2D.Dynamic)
                                    {
                                        this.ConnectedEntity = entity;

                                        //Create Joint
                                        this.mouseJoint = new MouseJoint2D()
                                        {
                                            Target = this.WorldPosition,
                                        };

                                        this.ConnectedEntity.AddComponent(mouseJoint);

                                        var audioService = WaveServices.GetService<AudioService>();
                                        audioService.Play(Audio.Sfx.Rubber_wav);

                                        break;
                                    }
                                }
                            }
                        }
                    }
                }

                // Touch just released, remove joint
                if (this.touchState.Count == 0 && this.mouseJoint != null)
                {
                    if (!this.ConnectedEntity.IsDisposed)
                    {
                        this.ConnectedEntity.RemoveComponent(this.mouseJoint);

                        var playerComponent = this.ConnectedEntity.FindComponent<PlayerComponent>();
                        playerComponent.Launch();
                    }

                    this.mouseJoint = null;
                }

                // update touch positions
                if (this.mouseJoint != null)
                {
                    this.TouchPosition = this.touchState[0].Position;
                    this.vsm.ToVirtualPosition(ref this.TouchPosition);

                    Ray r;
                    activeCamera2D.CalculateRay(ref this.TouchPosition, out r);
                    this.WorldPosition = r.IntersectionZPlane(0).ToVector2();
                    this.mouseJoint.Target = this.WorldPosition;
                }
            }
        }
    }
}