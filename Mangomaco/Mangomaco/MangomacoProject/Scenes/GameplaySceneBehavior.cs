#region Using Statements
using System;
using System.Collections.Generic;
using MangomacoProject.Components;
using MangomacoProject.Services;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Diagnostic;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics2D;
using WaveEngine.Framework.Services; 
#endregion

namespace MangomacoProject.Scenes
{
    /// <summary>
    /// Gameplay Scene Behavior
    /// </summary>
    public class GameplaySceneBehavior : SceneBehavior
    {
        private List<Entity> crates;
        private List<Entity> coins;
        private Entity player;

        private Vector2 initPlayerPosition;
        private List<Vector2> initCratePositions;
        private List<Vector2> initCoinPositions;

        private List<Collider2D> coinColliders;
        private List<Collider2D> trapColliders;
        private Collider2D endCollider;

        private SimpleSoundService soundManager;

        private PlayerController playerController;

        bool isInitialized = false;

        /// <summary>
        /// Resolves the dependencies needed for this instance to work.
        /// </summary>
        protected override void ResolveDependencies()
        {

        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        private void Initialize()
        {
            this.soundManager = WaveServices.GetService<SimpleSoundService>();

            this.InitPlayer();
            this.InitCoins();
            this.InitTraps();
            this.InitCrates();
            this.InitEnd();
        }

        /// <summary>
        /// Initializes the player.
        /// </summary>
        private void InitPlayer()
        {
            this.player = this.Scene.EntityManager.Find("Player");
            this.playerController = this.player.FindComponent<PlayerController>();
            this.initPlayerPosition = this.player.FindComponent<Transform2D>().Position;
        }

        /// <summary>
        /// Initializes the end.
        /// </summary>
        private void InitEnd()
        {
            this.endCollider = this.Scene.EntityManager.Find("Finish").FindComponent<Collider2D>(false);
        }

        /// <summary>
        /// Initializes the coins.
        /// </summary>
        private void InitCoins()
        {
            this.coins = new List<Entity>();
            this.coinColliders = new List<Collider2D>();
            this.initCoinPositions = new List<Vector2>();
            var result = this.Scene.EntityManager.FindAllByTag("coin");
            foreach (var coin in result)
            {
                Entity coinEntity = coin as Entity;

                this.coins.Add(coinEntity);
                this.coinColliders.Add(coinEntity.FindComponent<Collider2D>(false));
                this.initCoinPositions.Add(coinEntity.FindComponent<Transform2D>().Position);
            }
        }

        /// <summary>
        /// Initializes the crates.
        /// </summary>
        private void InitCrates()
        {
            this.crates = new List<Entity>();
            this.initCratePositions = new List<Vector2>();
            var result = this.Scene.EntityManager.FindAllByTag("crate");
            foreach (var crate in result)
            {
                Entity crateEntity = crate as Entity;
                this.crates.Add(crateEntity);
                this.initCratePositions.Add(crateEntity.FindComponent<Transform2D>().Position);
            }

            WaveServices.TimerFactory.CreateTimer("cratesounds", TimeSpan.FromSeconds(2), () =>
                {
                    foreach (var crate in result)
                    {
                        Entity crateEntity = crate as Entity;
                        var rigidBody = crateEntity.FindComponent<RigidBody2D>();
                        rigidBody.OnPhysic2DCollision += this.OnCrateCollision;
                    }

                }, false);
        }

        /// <summary>
        /// Initializes the traps.
        /// </summary>
        private void InitTraps()
        {
            this.trapColliders = new List<Collider2D>();
            var traps = this.Scene.EntityManager.FindAllByTag("trap");
            foreach (var trap in traps)
            {
                Entity trapEntity = trap as Entity;
                this.trapColliders.Add(trapEntity.FindComponent<Collider2D>(false));
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
            if (!this.isInitialized)
            {
                this.Initialize();
                this.isInitialized = true;
            }

            this.CheckCoins();
            this.CheckTraps();
            this.CheckEnd();
        }

        /// <summary>
        /// Checks the end.
        /// </summary>
        private void CheckEnd()
        {
            if (this.playerController.Collider.Intersects(this.endCollider))
            {
                this.Win();
            }
        }

        /// <summary>
        /// Checks the coins.
        /// </summary>
        private void CheckCoins()
        {
            for (int i = coinColliders.Count - 1; i >= 0; i--)
            {
                Collider2D coinCollider = this.coinColliders[i];
                if (coinCollider.Owner.Enabled && this.playerController.Collider.Intersects(coinCollider))
                {
                    coinCollider.Owner.Enabled = false;
                    this.soundManager.PlaySound(SimpleSoundService.SoundType.Coin);
                }
            }
        }

        /// <summary>
        /// Checks the traps.
        /// </summary>
        private void CheckTraps()
        {
            for (int i = trapColliders.Count - 1; i >= 0; i--)
            {
                Collider2D trapCollider = this.trapColliders[i];
                if (this.playerController.Collider.Intersects(trapCollider))
                {
                    this.Defeat();
                }
            }
        }

        /// <summary>
        /// Wins this instance.
        /// </summary>
        private void Win()
        {
            this.soundManager.PlaySound(SimpleSoundService.SoundType.Victory);
            this.ResetGame();
        }

        /// <summary>
        /// Defeats this instance.
        /// </summary>
        private void Defeat()
        {
            this.soundManager.PlaySound(SimpleSoundService.SoundType.Crash);
            this.ResetGame();
        }

        /// <summary>
        /// Called when [crate collision].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="Physic2DCollisionEventArgs"/> instance containing the event data.</param>
        private void OnCrateCollision(object sender, Physic2DCollisionEventArgs args)
        {
            var velocity = args.Body2DB.LinearVelocity;
            float length = velocity.Length();
            float volume = Math.Min(1, length/ 5);
            Labels.Add("Volume", volume);

            var instance = this.soundManager.PlaySound(SimpleSoundService.SoundType.CrateDrop, volume);
        }

        /// <summary>
        /// Resets the game.
        /// </summary>
        public void ResetGame()
        {
            this.playerController.Reset();

            foreach (var coin in this.coins)
            {
                coin.Enabled = true;
            }

            for (int i = 0; i < this.crates.Count; i++)
            {
                var crate = this.crates[i];

                var crateBody = crate.FindComponent<RigidBody2D>();
                crateBody.ResetPosition(this.initCratePositions[i]);
                crateBody.Rotation = 0;
            }
        }
    }
}
