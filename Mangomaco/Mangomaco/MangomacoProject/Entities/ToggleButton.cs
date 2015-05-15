#region Using Statements
using System;
using WaveEngine.Common.Helpers;
using WaveEngine.Common.Math;
using WaveEngine.Components.Gestures;
using WaveEngine.Components.UI;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics2D;
using WaveEngine.Framework.UI; 
#endregion

namespace MangomacoProject.Entities
{
    /// <summary>
    /// Toggle Button UI class
    /// </summary>
    public class ToggleButton : UIBase
    {
        private static int instances;

        #region Constants
        /// <summary>
        /// The default margin
        /// </summary>
        private readonly Thickness defaultMargin = new Thickness(5);

        /// <summary>
        /// The default width
        /// </summary>
        private const int DefaultWidth = 100;

        /// <summary>
        /// The default height
        /// </summary>
        private const int DefaultHeight = 40;
        #endregion

        private ToggleButtonBehavior toggleButtonBehavior;

        /// <summary>
        /// The size define by user
        /// </summary>
        private bool sizeDefineByUser;

        #region Events
        public event EventHandler<BoolEventArgs> Checked;
        #endregion

        #region Properties
        public bool IsChecked
        {
            get
            {
                return this.toggleButtonBehavior.IsChecked;
            }

            set
            {
                this.toggleButtonBehavior.IsChecked = value;
            }
        }

        /// <summary>
        /// Gets or sets the margin.
        /// </summary>
        /// <value>
        /// The margin.
        /// </value>
        public Thickness Margin
        {
            get
            {
                return this.entity.FindComponent<PanelControl>().Margin;
            }

            set
            {
                this.entity.FindComponent<PanelControl>().Margin = value;
            }
        }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        public float Width
        {
            get
            {
                return this.entity.FindComponent<PanelControl>().Width;
            }

            set
            {
                this.entity.FindComponent<PanelControl>().Width = value;
                this.sizeDefineByUser = true;
            }
        }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        public float Height
        {
            get
            {
                return this.entity.FindComponent<PanelControl>().Height;
            }

            set
            {
                this.entity.FindComponent<PanelControl>().Height = value;
                this.sizeDefineByUser = true;
            }
        }

        /// <summary>
        /// Gets or sets the horizontal alignment.
        /// </summary>
        /// <value>
        /// The horizontal alignment.
        /// </value>
        public HorizontalAlignment HorizontalAlignment
        {
            get
            {
                return this.entity.FindComponent<PanelControl>().HorizontalAlignment;
            }

            set
            {
                this.entity.FindComponent<PanelControl>().HorizontalAlignment = value;
            }
        }

        /// <summary>
        /// Gets or sets the vertical alignment.
        /// </summary>
        /// <value>
        /// The vertical alignment.
        /// </value>
        public VerticalAlignment VerticalAlignment
        {
            get
            {
                return this.entity.FindComponent<PanelControl>().VerticalAlignment;
            }

            set
            {
                this.entity.FindComponent<PanelControl>().VerticalAlignment = value;
            }
        }

        public string UncheckedImage
        {
            set
            {
                this.ChangeBackgroundEntity("UncheckedImage", value);
            }
        }

        public string UncheckedPressedImage
        {
            set
            {
                this.ChangeBackgroundEntity("UncheckedPressedImage", value);
            }
        }

        public string CheckedImage
        {
            set
            {
                this.ChangeBackgroundEntity("CheckedImage", value);
            }
        }

        public string CheckedPressedImage
        {
            set
            {
                this.ChangeBackgroundEntity("CheckedPressedImage", value);
            }
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ToggleButton" /> class.
        /// </summary>
        public ToggleButton()
            : this("ToggleButton" + instances++, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ToggleButton" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public ToggleButton(string name, bool initialChecked)
        {
            this.entity = new Entity(name)
                               .AddComponent(new Transform2D())
                               .AddComponent(new RectangleCollider())
                               .AddComponent(new TouchGestures(false))
                               .AddComponent(new ToggleButtonBehavior(initialChecked))
                               .AddComponent(new PanelControl(DefaultWidth, DefaultHeight))
                               .AddComponent(new PanelControlRenderer())
                               .AddComponent(new BorderRenderer());

            // Cached
            this.toggleButtonBehavior = this.entity.FindComponent<ToggleButtonBehavior>();

            this.toggleButtonBehavior.CheckedChanged -= toggleButtonBehavior_CheckedChanged;
            this.toggleButtonBehavior.CheckedChanged += toggleButtonBehavior_CheckedChanged;

            this.entity.EntityInitialized += this.Entity_EntityInitialized;
        }

        /// <summary>
        /// Handles the EntityInitialized event of the entity control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void Entity_EntityInitialized(object sender, EventArgs e)
        {
            Entity imageEntity = this.entity.FindChild("UncheckedImage");

            if (imageEntity != null && !this.sizeDefineByUser)
            {
                ImageControl ic = imageEntity.FindComponent<ImageControl>();
                PanelControl panel = this.entity.FindComponent<PanelControl>();

                panel.Width = ic.Width;
                panel.Height = ic.Height;
            }
        }

        /// <summary>
        /// Handles the CheckedChanged event of the toggleButtonBehavior control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void toggleButtonBehavior_CheckedChanged(object sender, EventArgs e)
        {
            if (this.Checked != null)
            {
                this.Checked(this, new BoolEventArgs(this.toggleButtonBehavior.IsChecked));
            }
        }

        /// <summary>
        /// Modifies the background image with the new asset path.
        /// </summary>
        /// <param name="imagePath">Path to the background image</param>
        private void ChangeBackgroundEntity(string entityName, string imagePath)
        {
            Entity imageEntity = this.entity.FindChild(entityName);
            ImageControl newImageControl = new ImageControl(imagePath)
            {
                Stretch = Stretch.Fill,
            };

            if (imageEntity != null)
            {
                Transform2D transform = imageEntity.FindComponent<Transform2D>();
                RectangleF rectangle = transform.Rectangle;

                rectangle.Offset(-rectangle.Width * transform.Origin.X, -rectangle.Height * transform.Origin.Y);

                // If imageEntity exist                
                imageEntity.RemoveComponent<ImageControl>();
                imageEntity.AddComponent(newImageControl);
                imageEntity.RefreshDependencies();

                newImageControl.Arrange(rectangle);
            }
            else
            {
                // If imageEntity doesn't exist
                this.entity.AddChild(new Entity(entityName)
                    .AddComponent(new Transform2D()
                    {
                        DrawOrder = 0.5f
                    })
                    .AddComponent(newImageControl)
                    .AddComponent(new ImageControlRenderer()));

                this.entity.RefreshDependencies();
            }
        }
    }
}