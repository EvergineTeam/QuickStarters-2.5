﻿#region Using Statements
using System;
using SurvivorProject.Commons;
using WaveEngine.Common.Math;
using WaveEngine.Components.Gestures;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics2D;
#endregion

namespace SurvivorProject.Entities
{
    public class Joystick : BaseDecorator
    {        
        private const int offset = 40;
        private Vector2 offsetLimits = new Vector2(offset, offset);

        private Transform2D backgroundTransform;
        private Transform2D thumbTransform;
        private Entity background, thumb;
        private Vector2 pressedPosition;
        private Vector2 direction;

        #region Properties

        /// <summary>
        /// Gets or sets the direction.
        /// </summary>
        public Vector2 Direction
        {
            get { return direction; }
            set { direction = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is active.
        /// </summary>
        public bool IsActive
        {
            get { return this.entity.IsActive; }
            set 
            {
                if (!value)
                {
                    this.direction = Vector2.Zero;
                }
                this.entity.IsActive = value;
                this.thumb.IsVisible = false;
                this.background.IsVisible = false;
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Joystick" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="area">The area.</param>
        public Joystick(string name, RectangleF area)
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

            // Background
            this.background = new Entity()
                            .AddComponent(new Transform2D()
                            {
                                Origin = Vector2.Center,
                                DrawOrder = 0.2f,
                            })
                            .AddComponent(new Sprite(Directories.Textures + "joystickBackground.wpk"))
                            .AddComponent(new SpriteRenderer(DefaultLayers.GUI));
            this.background.IsVisible = false;
            this.entity.AddChild(this.background);

            // Thumb
            this.thumb = new Entity()
                            .AddComponent(new Transform2D()
                            {
                                Origin = Vector2.Center,
                                DrawOrder = 0.1f,
                            })
                            .AddComponent(new Sprite(Directories.Textures + "joystickThumb.wpk"))
                            .AddComponent(new SpriteRenderer(DefaultLayers.GUI));
            this.thumb.IsVisible = false;
            this.entity.AddChild(this.thumb);

            // Touch Events
            this.backgroundTransform = this.background.FindComponent<Transform2D>();
            this.thumbTransform = this.thumb.FindComponent<Transform2D>();
            var touch = this.entity.FindComponent<TouchGestures>();
            touch.TouchPressed += (s, o) =>
            {
                this.pressedPosition = o.GestureSample.Position;
                this.backgroundTransform.LocalX = this.pressedPosition.X - area.X;
                this.backgroundTransform.LocalY = this.pressedPosition.Y - area.Y;
                this.background.IsVisible = true;

                this.thumbTransform.LocalX = this.pressedPosition.X - area.X;
                this.thumbTransform.LocalY = this.pressedPosition.Y - area.Y;
                this.thumb.IsVisible = true;

                this.direction = Vector2.Zero;
            };
            touch.TouchMoved += (s, o) =>
            {
                Vector2 deltaTouch = this.pressedPosition - o.GestureSample.Position;
                deltaTouch.X = MathHelper.Clamp(deltaTouch.X, -offset, offset);
                deltaTouch.Y = MathHelper.Clamp(deltaTouch.Y, -offset, offset);
                this.thumbTransform.LocalX = this.pressedPosition.X - deltaTouch.X - area.X;
                this.thumbTransform.LocalY = this.pressedPosition.Y - deltaTouch.Y - area.Y;

                deltaTouch /= this.offsetLimits;
                this.direction = -deltaTouch;
            };
            touch.TouchReleased += (s, o) =>
            {
                this.background.IsVisible = false;
                this.thumb.IsVisible = false;
                this.direction = Vector2.Zero;
            };
        }
    }
}
