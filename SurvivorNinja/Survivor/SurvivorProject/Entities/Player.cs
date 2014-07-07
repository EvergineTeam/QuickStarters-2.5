#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SurvivorProject.Commons;
using WaveEngine.Common.Math;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using SurvivorProject.Behaviors;
using WaveEngine.Framework.Physics2D;
using WaveEngine.Framework.Services;
#endregion

namespace SurvivorProject.Entities
{
    public class Player : BaseDecorator
    {
        private Transform2D transform;

        private HubPanel hubPanel;
        private int life;
        private GamePlayScene gamePlayScene;
        private Vector2 initialPosition;

        #region Properties

        /// <summary>
        /// Gets or sets the life.
        /// </summary>
        public int Life
        {
            get { return life; }
            set
            {
                life = value;
                this.hubPanel.Playerlife = value;

                if (value <= 0)
                {
                    this.gamePlayScene.CurrentState = GamePlayScene.States.GameOver;
                }               
            }
        }

        /// <summary>
        /// Sets the direction.
        /// </summary>
        public Vector2 Direction
        {
            set
            {
                this.transform.X += value.X;
                this.transform.Y += value.Y;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is active.
        /// </summary>
        public bool IsActive
        {
            get { return this.entity.IsActive; }
            set { this.entity.IsActive = value; }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Player" /> class.
        /// </summary>
        /// <param name="position">The position.</param>
        public Player(string name, Vector2 position)
        {
            this.initialPosition = position;

            this.entity = new Entity(name)
                            .AddComponent(new Transform2D()
                            {
                                Origin = Vector2.Center,
                                X = position.X,
                                Y = position.Y,
                                DrawOrder = 0.5f,
                            })
                            .AddComponent(new CircleCollider() { Radius = 0.3f })
                            .AddComponent(new PlayerBehavior())
                            .AddComponent(new Sprite(Directories.Textures + "player.wpk"))
                            .AddComponent(new SpriteRenderer(DefaultLayers.Alpha));

            // Cached
            this.transform = this.entity.FindComponent<Transform2D>();

            this.entity.EntityInitialized += entity_EntityInitialized;
        }

        /// <summary>
        /// Handles the EntityInitialized event of the entity control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void entity_EntityInitialized(object sender, EventArgs e)
        {
            this.gamePlayScene = this.entity.Scene as GamePlayScene;
            this.hubPanel = this.entity.Scene.EntityManager.Find<HubPanel>("HubPanel");
            this.Life = 100;            
        }


        /// <summary>
        /// Resets this instance.
        /// </summary>
        public void Reset()
        {
            this.transform.X = this.initialPosition.X;
            this.transform.Y = this.initialPosition.Y;
            this.life = 100;            
        }
    }
}
