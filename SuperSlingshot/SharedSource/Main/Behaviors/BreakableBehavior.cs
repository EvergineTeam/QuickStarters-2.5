using System;
using System.Runtime.Serialization;
using SuperSlingshot.Enums;
using SuperSlingshot.Managers;
using SuperSlingshot.Scenes;
using WaveEngine.Common.Attributes;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Components.Particles;
using WaveEngine.Framework;
using WaveEngine.Framework.Physics2D;

namespace SuperSlingshot.Behaviors
{
    [DataContract]
    public class BreakableBehavior : Behavior
    {
        private ParticleSystem2D childParticleSystem = null;
        private BreakableState state;
        private BreakableState lastState;
        private TimeSpan timeToRemove;
        private TimeSpan timeToEmit;
        private bool firstUpdated;
        private LevelScore score;
        private GameScene scene;

        [RequiredComponent]
        private PolygonCollider2D collider = null;

        [RequiredComponent]
        private Sprite sprite { get; set; }

        [RequiredComponent]
        private SpriteRenderer spriteRenderer { get; set; }

        [RequiredComponent]
        private RigidBody2D rigidBody = null;

        [DataMember]
        [RenderPropertyAsAsset(AssetType.Texture)]
        public string NormalTexture { get; set; }

        [DataMember]
        [RenderPropertyAsAsset(AssetType.Texture)]
        public string BrokenTexture { get; set; }

        [DontRenderProperty]
        public BreakableState State
        {
            get { return this.state; }
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            this.childParticleSystem = this.Owner.FindChild(GameConstants.ENTITYCHILDPARTICLES).FindComponent<ParticleSystem2D>();

            this.timeToEmit = TimeSpan.FromSeconds(0.5);
            this.timeToRemove = TimeSpan.FromSeconds(5);

            var scene = this.Owner.Scene as GameScene;
            if (scene != null)
            {
                this.scene = scene;
                this.score = scene.Score;
            }

            // TODO: Workaround, remove when fixed (do not store EndDeltaScale in WaveEditor)
            this.childParticleSystem.EndDeltaScale = 1.0f;
        }

        public void SetState(BreakableState state)
        {
            if (lastState == BreakableState.DEAD)
            {
                return;
            }

            switch (state)
            {
                case BreakableState.NORMAL:
                    this.sprite.TexturePath = this.NormalTexture;
                    this.childParticleSystem.Emit = false;
                    break;
                case BreakableState.DAMAGED:
                    this.sprite.TexturePath = this.BrokenTexture;
                    this.childParticleSystem.Emit = false;
                    break;
                case BreakableState.DEAD:
                    this.childParticleSystem.Emit = true;

                    this.score.Points += this.scene.BlockDestroyPoints;
                    break;
                default:
                    break;
            }

            this.lastState = this.state;
            this.state = state;
        }

        protected override void Update(TimeSpan gameTime)
        {
            //Labels.Add(this.childVisual.Name + " position", this.childVisual.FindComponent<Transform2D>().Position);
            //Labels.Add(this.Owner.Name + " position", this.Owner.FindComponent<Transform2D>().Position);

            if (!this.firstUpdated)
            {
                this.collider.TexturePath = this.sprite.TexturePath;
                this.firstUpdated = true;
            }

            if (this.state == BreakableState.DEAD)
            {
                this.timeToEmit -= gameTime;
                this.timeToRemove -= gameTime;

                // only an update loop
                if (this.lastState != this.state)
                {
                    this.rigidBody.IsActive = false;
                    this.Owner.RemoveComponent(this.rigidBody);
                    this.spriteRenderer.IsVisible = false;

                    this.lastState = this.state;
                }

                if (this.timeToEmit <= TimeSpan.Zero)
                {
                    this.childParticleSystem.Emit = false;
                }

                if (this.timeToRemove <= TimeSpan.Zero)
                {
                    if (this.Owner.Parent != null)
                    {
                        this.Owner.Parent.RemoveChild(this.Owner.Name);
                    }
                }
            }
        }
    }
}
