using MultiplayerTopDownTank.Behaviors;
using MultiplayerTopDownTank.Components;
using WaveEngine.Common.Math;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics2D;
using WaveEngine.Networking;

namespace MultiplayerTopDownTank.Entities
{
    public class Bullet : BaseDecorator
    {
        private Vector2 position;
        private Vector2 direction;
        private RigidBody2D rigidBody;
        private float velocity = 5f;

        #region Properties

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        public Vector2 Position
        {
            get { return position; }
            set
            {
                position = value;
                Transform2D transform = this.entity.FindComponent<Transform2D>();
                transform.X = position.X;
                transform.Y = position.Y;
            }
        }

        /// <summary>
        /// Gets or sets the direction.
        /// </summary>
        public Vector2 Direction
        {
            get { return direction; }
            set
            {
                direction = value;
                this.entity.IsVisible = true;
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Bullet" /> class.
        /// </summary>
        public Bullet()
        {
            this.rigidBody = new RigidBody2D
            {
                PhysicBodyType = WaveEngine.Common.Physics2D.RigidBodyType2D.Dynamic,
                IsBullet = true,
                LinearDamping = 0
            };

            this.entity = new Entity() { Tag = "bullet" }
                .AddComponent(new Transform2D()
                {
                    Origin = Vector2.Center,
                    X = 0,
                    Y = 0,
                    DrawOrder = 0.6f,
                })
                .AddComponent(this.rigidBody)
                .AddComponent(new RectangleCollider2D())
                .AddComponent(new BulletBehavior(this))
                .AddComponent(new Sprite(WaveContent.Assets.Textures.Bullets.rounded_bulletBeige_outline_png))
                .AddComponent(new SpriteRenderer(DefaultLayers.Alpha))
                .AddComponent(new NetworkBehavior())
                .AddComponent(new BulletNetworkSyncComponent());

            this.direction = Vector2.Zero;
            this.position = Vector2.Zero;
            this.entity.IsVisible = false;
        }

        public void Shoot(Vector2 position, Vector2 direction)
        {
            this.rigidBody.ResetPosition(position);
            this.Direction = direction;
            this.IsBulletActive(true);

            Vector2 impulse = direction * this.velocity;
            this.rigidBody.ApplyLinearImpulse(impulse, position);
        }

        public void IsBulletActive(bool isActive)
        {
            var bulletBehavior = this.entity.FindComponent<BulletBehavior>();
            bulletBehavior.IsActive = isActive;
            this.IsVisible = isActive;
            this.entity.IsVisible = isActive;
        }
    }
}
