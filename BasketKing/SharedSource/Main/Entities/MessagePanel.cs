#region Using Statements
using BasketKing.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Components.UI;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.UI;
#endregion

namespace BasketKing.Entities
{
    public class MessagePanel : BaseDecorator
    {
        public enum MessageType
        {
            Hide,
            HurryUp,
            Ready,            
            Timeout,
        };

        private MessageType type;
        private Image ready, hurryUp, timeOut;

        #region Properties

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public MessageType Type
        {
            get
            {
                return type;
            }

            set
            {
                this.type = value;

                this.IsVisible = true;
                this.ready.IsVisible = false;
                this.hurryUp.IsVisible = false;
                this.timeOut.IsVisible = false;

                switch (type)
                {
                    case MessageType.Hide:
                        this.IsVisible = false;
                        break;
                    case MessageType.Ready:
                        this.ready.IsVisible = true;
                        break;
                    case MessageType.HurryUp:
                        this.hurryUp.IsVisible = true;
                        break;
                    case MessageType.Timeout:
                        this.timeOut.IsVisible = true;
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Gets or sets the horizontal alignment.
        /// </summary>
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
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagePanel" /> class.
        /// </summary>
        /// <param name="type">The type.</param>
        public MessagePanel(MessageType type)
        {
            this.entity = new Entity("MessagePanel")
                               .AddComponent(new Transform2D())
                               .AddComponent(new PanelControl(1024, 320))
                               .AddComponent(new PanelControlRenderer())
                               .AddChild(new Image(Directories.Textures + "bg_panel.wpk")
                               {
                                   DrawOrder = 0.6f,
                               }.Entity);

            // HurryUp
            this.hurryUp = new Image(Directories.Textures + "betheking.wpk")
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 165, 0, 0),
            };
            this.entity.AddChild(hurryUp.Entity);

            // Ready
            this.ready = new Image(Directories.Textures + "ready.wpk")
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 165, 0, 0),
            };
            this.entity.AddChild(ready.Entity);            

            // Timeout
            this.timeOut = new Image(Directories.Textures + "timeover.wpk")
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 165, 0, 0),
            };
            this.entity.AddChild(timeOut.Entity);

            this.Type = type;
        }
    }
}
