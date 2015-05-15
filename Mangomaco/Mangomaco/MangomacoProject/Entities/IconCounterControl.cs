#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Math;
using WaveEngine.Components.UI;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
#endregion

namespace MangomacoProject.Entities
{
    /// <summary>
    /// Icon Counter Control UI
    /// </summary>
    public class IconCounterControl : UIBase
    {
        /// <summary>
        /// The empty icon image
        /// </summary>
        private string emptyIconImage;

        /// <summary>
        /// The filled icon image
        /// </summary>
        private string filledIconImage;

        /// <summary>
        /// The maximum
        /// </summary>
        private int maximum;

        /// <summary>
        /// The value
        /// </summary>
        private int value;

        #region Properties

        /// <summary>
        /// Gets or sets the empty icon image.
        /// </summary>
        public string EmptyIconImage
        {
            get
            {
                return this.emptyIconImage;
            }

            set
            {
                this.emptyIconImage = value;
                this.Refresh();
            }
        }

        /// <summary>
        /// Gets or sets the filled icon image.
        /// </summary>
        public string FilledIconImage
        {
            get
            {
                return this.filledIconImage;
            }

            set
            {
                this.filledIconImage = value;
                this.Refresh();
            }
        }

        /// <summary>
        /// Gets or sets the maximum.
        /// </summary>
        public int Maximum
        {
            get
            {
                return this.maximum;
            }

            set
            {
                this.maximum = value;
                this.Refresh();
            }
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public int Value
        {
            get
            {
                return this.value;
            }

            set
            {
                this.value = value;
                this.Refresh();
            }
        } 

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="IconCounterControl"/> class.
        /// </summary>
        public IconCounterControl()
        {
            this.entity = new Entity()
                               .AddComponent(new Transform2D())
                               .AddComponent(new PanelControl(10, 10))
                               .AddComponent(new PanelControlRenderer())
                               .AddComponent(new BorderRenderer());
        }

        /// <summary>
        /// Refreshes this instance.
        /// </summary>
        private void Refresh()
        {
            for (int i = 0; i < this.maximum; i++)
            {
                if(i < this.value && this.filledIconImage != string.Empty)
                {
                    this.ChangeBackgroundEntity("Icon_" + i, this.filledIconImage);
                }
                else if (this.emptyIconImage != string.Empty)
                {
                    this.ChangeBackgroundEntity("Icon_" + i, this.emptyIconImage);
                }
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
