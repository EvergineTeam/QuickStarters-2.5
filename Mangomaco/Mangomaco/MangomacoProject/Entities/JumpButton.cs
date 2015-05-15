﻿#region Using Statements
using WaveEngine.Common.Math;
using WaveEngine.Components.Gestures;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics2D;

#endregion

namespace MangomacoProject.Entities
{
    /// <summary>
    /// Jump Button entity class
    /// </summary>
    public class JumpButton : BaseDecorator
    {
        private Entity thumb;
        private Transform2D thumbTransform;
        private Vector2 pressedPosition;

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this instance is active.
        /// </summary>
        public bool IsActive
        {
            get { return this.entity.IsActive; }
            set
            {
                this.entity.IsActive = value;
            }
        }

        /// <summary>
        /// The is shooting
        /// </summary>
        public bool IsShooting;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="JumpButton" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="area">The area.</param>
        public JumpButton(string name, RectangleF area)
        {
            // Touch area
            this.entity = new Entity(name)
                                .AddComponent(new Transform2D()
                                {
                                    X = area.X,
                                    Y = area.Y,
                                    Rectangle = new RectangleF(0, 0, area.Width, area.Height),
                                })
                                .AddComponent(new RectangleCollider())
                                .AddComponent(new TouchGestures());

           // Thumb
            this.thumb = new Entity()
                            .AddComponent(new Transform2D()
                            {
                                Origin = Vector2.Center,
                                DrawOrder = 0.1f,
                            })
                            .AddComponent(new Sprite("Content/joystickThumb.wpk"))
                            .AddComponent(new SpriteRenderer(DefaultLayers.GUI));
            this.thumb.IsVisible = false;
            this.thumbTransform = this.thumb.FindComponent<Transform2D>();
            this.entity.AddChild(this.thumb);

            // Touch Events
            var touch = this.entity.FindComponent<TouchGestures>();
            touch.TouchPressed += (s, o) =>
            {
                this.pressedPosition = o.GestureSample.Position;

                this.thumbTransform.X = this.pressedPosition.X;
                this.thumbTransform.Y = this.pressedPosition.Y;
                this.thumb.IsVisible = true;

                this.IsShooting = true;
            };
            touch.TouchMoved += (s, o) =>
            {
                Vector2 deltaTouch = this.pressedPosition - o.GestureSample.Position;

                this.thumbTransform.X = this.pressedPosition.X - deltaTouch.X;
                this.thumbTransform.Y = this.pressedPosition.Y - deltaTouch.Y;
            };
            touch.TouchReleased += (s, o) =>
            {
                this.thumb.IsVisible = false;
                this.IsShooting = false;
            };
        }
    }
}
