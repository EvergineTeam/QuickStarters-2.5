#region Using Statements
using BasketKing.Behaviors;
using BasketKing.Commons;
using BasketKing.Drawables;
using BasketKing.Entities;
using BasketKing.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics2D;
using WaveEngine.Framework.Services;
using WaveEngine.Materials;
#endregion

namespace BasketKing
{
    public class GamePlaySceneBehavior : SceneBehavior
    {
        private static int ball_instances;
        private static string[] ballTextures = { WaveContent.Assets.Textures.ball_PNG, WaveContent.Assets.Textures.ball2_PNG };

        private const int FORCE = 80;

        private GamePlayScene gamePlayScene;
        private GamePlayScene.States lastState;
        private ScoreboardPanel scoreboardPanel;
        private Entity ball, target, start;
        private Transform2D targetTransform, startTransform, ballTransform;
        private RigidBody2D rigidBodyBall;
        private Sprite ballSprite;
        private Vector2 touchPosition;
        private bool launch;
        private TimeSpan lastTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="GamePlaySceneBehavior" /> class.
        /// </summary>
        public GamePlaySceneBehavior()
        {
            this.launch = false;
        }

        /// <summary>
        /// Resolves the dependencies needed for this instance to work.
        /// </summary>
        protected override void ResolveDependencies()
        {
            this.gamePlayScene = this.Scene as GamePlayScene;
            this.scoreboardPanel = this.gamePlayScene.EntityManager.Find<ScoreboardPanel>("scoreboardPanel");
            this.target = this.gamePlayScene.EntityManager.Find<Entity>("BallTarget");
            this.targetTransform = this.target.FindComponent<Transform2D>();

            this.start = this.gamePlayScene.EntityManager.Find<Entity>("BallStart");
            this.startTransform = this.start.FindComponent<Transform2D>();
        }

        /// <summary>
        /// Creates the ball.
        /// </summary>
        private void CreateBall()
        {
            int index = WaveServices.Random.NextBool() ? 1 : 0;
            string textureName = ballTextures[index];

            this.ball = new Entity("ball" + ball_instances++)
            {
                Tag = "ball"
            }
            .AddComponent(new Transform2D()
            {
                Origin = Vector2.Center,
                X = WaveServices.Random.Next(400, 900),
                Y = WaveServices.Random.Next(370, 580),
                DrawOrder = 0.4f,
            })
            .AddComponent(new CircleCollider2D())
            .AddComponent(new Sprite(textureName)
            {
                //TintColor = Color.Yellow,
            })
            .AddComponent(new BallBehavior())
            .AddComponent(new RigidBody2D()
            {
                PhysicBodyType = PhysicBodyType.Static,
                CollisionCategories = Physic2DCategory.Cat2,
                CollidesWith = Physic2DCategory.None,
                Restitution = 0.6f,
            })
            .AddComponent(new SpriteRenderer(DefaultLayers.Alpha));

            this.gamePlayScene.EntityManager.Add(this.ball);

            this.ballTransform = this.ball.FindComponent<Transform2D>();
            this.rigidBodyBall = this.ball.FindComponent<RigidBody2D>();
            this.ballSprite = this.ball.FindComponent<Sprite>();

            // Collision
            this.rigidBodyBall.OnPhysic2DCollision += (s, o) =>
            {
                if (o.Body2DB.Owner.Tag == "Border")
                {
                    this.gamePlayScene.EntityManager.Remove(o.Body2DA.Owner);

                }
            };

            // Start Position
            this.startTransform.X = this.ballTransform.X;
            this.startTransform.Y = this.ballTransform.Y;            
        }

        /// <summary>
        /// Resets the match.
        /// </summary>
        private void ResetMatch()
        {

            var balls = this.gamePlayScene.EntityManager.FindAllByTag("ball");
            foreach (Entity ball in balls.ToList())
            {
                this.gamePlayScene.EntityManager.Remove(ball);
            }
        }

        /// <summary>
        /// Allows this instance to execute custom logic during its <c>Update</c>.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        /// <remarks>
        /// This method will not be executed if it are not <c>Active</c>.
        /// </remarks>
        protected override void Update(TimeSpan gameTime)
        {
            Input input = WaveServices.Input;

            if (this.gamePlayScene.CurrentState == GamePlayScene.States.TapToStart)
            {
                if (input.TouchPanelState.Count > 0)
                {
                    this.gamePlayScene.CurrentState = GamePlayScene.States.HurryUp;
                    SoundsManager.Instance.PlaySound(SoundsManager.SOUNDS.Button);
                }
            }
            else if (this.gamePlayScene.CurrentState == GamePlayScene.States.Ready)
            {
                if (this.lastState != GamePlayScene.States.Ready)
                {
                    this.ResetMatch();
                }
            }
            else if (this.gamePlayScene.CurrentState == GamePlayScene.States.GamePlay)
            {
                if (this.lastState != GamePlayScene.States.GamePlay)
                {
                    this.scoreboardPanel.Reset();
                    this.CreateBall();
                }
                else
                {
                    Vector2 ballPosition = new Vector2(ballTransform.X, ballTransform.Y);

                    // Update time
                    this.scoreboardPanel.Time -= gameTime;
                    if (this.scoreboardPanel.Time < TimeSpan.FromSeconds(10) &&
                        this.lastTime.Seconds > this.scoreboardPanel.Time.Seconds)
                    {
                        SoundsManager.Instance.PlaySound(SoundsManager.SOUNDS.Digit);
                    }
                    else if (this.scoreboardPanel.Time < TimeSpan.Zero)
                    {
                        this.scoreboardPanel.Time = TimeSpan.Zero;
                        this.gamePlayScene.CurrentState = GamePlayScene.States.TimeOut;
                    }

                    this.lastTime = this.scoreboardPanel.Time;

                    // Launch ball                            
                    if (input.TouchPanelState.Count > 0 && input.TouchPanelState[0].Position.Y > 80)
                    {
                        // Target                   
                        this.touchPosition = input.TouchPanelState[0].Position;
                        WaveServices.ViewportManager.RecoverPosition(ref this.touchPosition);
                        this.targetTransform.X = this.touchPosition.X;
                        this.targetTransform.Y = this.touchPosition.Y;
                        this.target.IsVisible = true;

                        this.launch = true;
                    }
                    else
                    {
                        this.target.IsVisible = false;

                        if (this.launch)
                        {
                            this.launch = false;

                            // launch ball      
                            this.rigidBodyBall.CollidesWith = Physic2DCategory.All;
                            this.rigidBodyBall.PhysicBodyType = PhysicBodyType.Dynamic;
                            this.ballSprite.TintColor = Color.White;
                            Vector2 direction = (this.touchPosition - ballPosition) / FORCE;
                            this.rigidBodyBall.ApplyLinearImpulse(direction - Vector2.UnitY * 1f);
                            this.rigidBodyBall.ApplyAngularImpulse((float)(WaveServices.Random.NextDouble() - 0.5f) * 0.4f);
                            this.gamePlayScene.scoreboardPanel.NumLaunches++;

                            // Create new ball
                            this.CreateBall();
                        }
                    }
                }
            }

            this.lastState = this.gamePlayScene.CurrentState;
        }
    }
}
