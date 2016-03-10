using System;
using System.Runtime.Serialization;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Math;
using WaveEngine.Components.Gestures;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Managers;
using WaveEngine.Framework.Physics2D;
using WaveEngine.Framework.Services;

namespace DeepSpace.Components.Joystick
{
    [DataContract]
    public class JoystickComponent : Behavior
    {
        private const int offset = 40;
        private Vector2 offsetLimits;

        private VirtualScreenManager virtualScreenManager;

        private Vector4 actionableArea;
        private RectangleF viewportActionableArea;
        private string backgroundEntityPath;
        private string thumbEntityPath;
        private Entity backgroundEntity;
        private Entity thumbEntity;
        private Transform2D backgroundTransform;
        private Transform2D thumbTransform;
        private Vector2 pressedPosition;
        private Vector2 direction;

        public Vector2 Direction
        {
            get
            {
                return this.direction;
            }
        }

        public bool IsPressed { get; private set; }

        public bool IsDisable { get; set; }

        [DataMember]
        public Vector4 ActionableArea
        {
            get { return this.actionableArea; }
            set
            {
                this.actionableArea = value;
            }
        }

        [DataMember]
        [RenderPropertyAsEntity(componentsFilter: new string[] { "WaveEngine.Framework.Graphics.Transform2D" })]
        public string BackgroundEntityPath
        {
            get { return this.backgroundEntityPath; }
            set
            {
                this.backgroundEntityPath = value;
            }
        }

        [DataMember]
        [RenderPropertyAsEntity(componentsFilter: new string[] { "WaveEngine.Framework.Graphics.Transform2D" })]
        public string ThumbEntityPath
        {
            get { return this.thumbEntityPath; }
            set
            {
                this.thumbEntityPath = value;
            }
        }

        public JoystickComponent() : base("JoystickComponent")
        {
        }

        protected override void DefaultValues()
        {
            this.offsetLimits = new Vector2(offset, offset);
        }

        protected override void Initialize()
        {
            base.Initialize();
            this.virtualScreenManager = this.Owner.Scene.VirtualScreenManager;
        }

        protected override void Update(TimeSpan gameTime)
        {
            var totalWidth = this.virtualScreenManager.RightEdge - this.virtualScreenManager.LeftEdge;
            var totalHeight = this.virtualScreenManager.BottomEdge - this.virtualScreenManager.TopEdge;
            this.viewportActionableArea = new RectangleF(
                (this.actionableArea.X * totalWidth) + this.virtualScreenManager.LeftEdge,
                (this.actionableArea.Y * totalHeight) + this.virtualScreenManager.TopEdge,
                (this.actionableArea.Z * totalWidth),
                (this.actionableArea.W * totalHeight)
            );

            if (this.backgroundEntityPath != null)
            {
                this.backgroundEntity = this.EntityManager.Find(this.backgroundEntityPath);
                this.backgroundEntity.IsVisible = false;
                if (backgroundEntity != null)
                {
                    this.backgroundTransform = this.backgroundEntity.FindComponent<Transform2D>();
                }
            }

            if (this.thumbEntityPath != null)
            {
                this.thumbEntity = this.EntityManager.Find(this.thumbEntityPath);
                this.thumbEntity.IsVisible = false;
                if (thumbEntity != null)
                {
                    this.thumbTransform = this.thumbEntity.FindComponent<Transform2D>();
                }
            }

            var actionableTouchGestures = new TouchGestures();
            var actionableEntity = new Entity()
                .AddComponent(new Transform2D()
                {
                    X = this.viewportActionableArea.X,
                    Y = this.viewportActionableArea.Y,
                    Rectangle = new RectangleF(0, 0, this.viewportActionableArea.Width, this.viewportActionableArea.Height)
                })
                .AddComponent(new RectangleCollider2D())
                .AddComponent(actionableTouchGestures);
            actionableTouchGestures.TouchPressed += this.ActionableTouchGesturesTouchPressed;
            actionableTouchGestures.TouchMoved += this.ActionableTouchGesturesTouchMoved;
            actionableTouchGestures.TouchReleased += this.ActionableTouchGesturesTouchReleased;

            this.EntityManager.Add(actionableEntity);

            this.IsActive = false;
        }

        private void ActionableTouchGesturesTouchPressed(object sender, GestureEventArgs e)
        {
            if (!this.IsDisable)
            {
                this.pressedPosition = e.GestureSample.Position;
                if (this.backgroundEntity != null)
                {
                    this.backgroundTransform.X = this.pressedPosition.X;
                    this.backgroundTransform.Y = this.pressedPosition.Y;
                    this.backgroundEntity.IsVisible = true;
                }

                this.thumbTransform.X = this.pressedPosition.X;
                this.thumbTransform.Y = this.pressedPosition.Y;
                this.thumbEntity.IsVisible = true;
                this.IsPressed = true;

                this.direction = Vector2.Zero;
            }
        }

        private void ActionableTouchGesturesTouchMoved(object sender, GestureEventArgs e)
        {
            if (!this.IsDisable)
            {
                Vector2 deltaTouch = this.pressedPosition - e.GestureSample.Position;
                deltaTouch.X = MathHelper.Clamp(deltaTouch.X, -offset, offset);
                deltaTouch.Y = MathHelper.Clamp(deltaTouch.Y, -offset, offset);
                this.thumbTransform.X = this.pressedPosition.X - deltaTouch.X;
                this.thumbTransform.Y = this.pressedPosition.Y - deltaTouch.Y;

                deltaTouch /= this.offsetLimits;
                this.direction = -deltaTouch;
            }
        }

        private void ActionableTouchGesturesTouchReleased(object sender, GestureEventArgs e)
        {
            if (this.backgroundEntity != null)
            {
                this.backgroundEntity.IsVisible = false;
            }

            this.thumbEntity.IsVisible = false;
            this.direction = Vector2.Zero;
            this.IsPressed = false;
        }
    }
}
