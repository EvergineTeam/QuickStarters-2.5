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
    [DataContract]
    public class MouseBehavior : Behavior
    {
        private Input input;
        private TouchPanelState touchState;
        public MouseJoint2D mouseJoint;
        private VirtualScreenManager vsm;

        public Entity ConnectedEntity;
        public Vector2 TouchPosition;
        public Vector2 WorldPosition;

        protected override void DefaultValues()
        {
            base.DefaultValues();

            this.TouchPosition = Vector2.Zero;
            this.WorldPosition = Vector2.Zero;
        }

        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            this.vsm = this.Owner.Scene.VirtualScreenManager;
        }

        protected override void Update(TimeSpan gameTime)
        {
            this.input = WaveServices.Input;

            if (this.input.TouchPanelState.IsConnected)
            {
                this.touchState = this.input.TouchPanelState;

                if (this.touchState.Count > 0 && this.mouseJoint == null)
                {
                    this.TouchPosition = this.touchState[0].Position;
                    this.vsm.ToVirtualPosition(ref this.TouchPosition);
                    //this.RenderManager.ActiveCamera2D.Unproject(ref this.TouchPosition, out this.WorldPosition);
                    Ray r;
                    this.RenderManager.ActiveCamera2D.CalculateRay(ref this.TouchPosition, out r);
                    this.WorldPosition = r.IntersectionZPlane(0).ToVector2();

                    //Labels.Add("worldPosition", this.WorldPosition);

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
                                            //MaxForce = 100,
                                            //DampingRatio = 0.5f,
                                            //FrequencyHz = 2000,
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

                if (this.mouseJoint != null)
                {
                    this.TouchPosition = this.touchState[0].Position;
                    this.vsm.ToVirtualPosition(ref this.TouchPosition);
                    //this.RenderManager.ActiveCamera2D.Unproject(ref this.TouchPosition, out this.WorldPosition);
                    Ray r;
                    this.RenderManager.ActiveCamera2D.CalculateRay(ref this.TouchPosition, out r);
                    this.WorldPosition = r.IntersectionZPlane(0).ToVector2();
                    this.mouseJoint.Target = this.WorldPosition;

                    Labels.Add("worldPosition", this.WorldPosition);
                }
            }
        }
    }
}