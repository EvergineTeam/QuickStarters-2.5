#region File Description
//-----------------------------------------------------------------------------
// Super Squid
//
// Quickstarter for Wave University Tour 2014.
// Author: Wave Engine Team
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using SuperSquidProject.Managers;
using SuperSquidProject.Scenes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Components.Transitions;
using WaveEngine.Framework;
using WaveEngine.Framework.Diagnostic;
using WaveEngine.Framework.Physics2D;
using WaveEngine.Framework.Services; 
#endregion

namespace SuperSquidProject.Entities.Behaviors
{
    public class BlockBuilderBehavior : Behavior
    {
        private readonly int MAX_VISIBLE_BLOCKS = 2;
        private readonly float MIN_SCROLL_VELOCITY = 0.1f;

        private List<RocksBlock> visibleBlocks;
        private List<RocksBlock> avaibleBlocks;
        private Collider2D squidCollider;

        private SoundManager soundManager;

        private GamePlayScene scene;
        private BackgroundScene backScene;

        private float scrollVelocity;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlockBuilderBehavior" /> class.
        /// </summary>
        /// <param name="blocks">The blocks.</param>
        public BlockBuilderBehavior()
        {
            this.visibleBlocks = new List<RocksBlock>();
            this.avaibleBlocks = this.GetAvaibleBlocks();
            this.scrollVelocity = this.MIN_SCROLL_VELOCITY;
        }

        /// <summary>
        /// Performs further custom initialization for this instance.
        /// </summary>
        /// <remarks>
        /// By default this method does nothing.
        /// </remarks>
        protected override void Initialize()
        {
            base.Initialize();

            this.scene = (GamePlayScene)this.Owner.Scene;
            this.backScene = WaveServices.ScreenContextManager.FindContextByName("BackContext")
                                                              .FindScene<BackgroundScene>();

            this.soundManager = WaveServices.GetService<SoundManager>();

            foreach (var block in this.avaibleBlocks)
            {
                this.Owner.AddChild(block.Entity);
                block.Entity.Enabled = false;
            }

            this.Reset();
        }

        /// <summary>
        /// Resolves the dependencies needed for this instance to work.
        /// </summary>
        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            this.squidCollider = this.EntityManager.Find("SquidEntity").FindComponent<Collider2D>(false);
        }

        /// <summary>
        /// Allows this instance to execute custom logic during its <c>Update</c>.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        /// <remarks>
        /// This method will not be executed if the <see cref="T:WaveEngine.Framework.Component" />, or the <see cref="T:WaveEngine.Framework.Entity" />
        /// owning it are not <c>Active</c>.
        /// </remarks>
        protected override void Update(TimeSpan gameTime)
        {
            var virtualScreenHeight = WaveServices.ViewportManager.BottomEdge - WaveServices.ViewportManager.TopEdge;

            foreach (RocksBlock block in this.visibleBlocks.ToList())
            {
                block.Transform2D.Y += scrollVelocity * (float)gameTime.TotalMilliseconds;

                if (block.CheckStarCollision(this.squidCollider))
                {
                    this.soundManager.PlaySound(SoundManager.SOUNDS.Star);
                    this.scene.CurrentScore++;
                }

                bool gameOverDetected = false;

                if (block.CheckRockCollision(this.squidCollider))
                {
                    this.soundManager.PlaySound(SoundManager.SOUNDS.RockCrash);
                    gameOverDetected = true;
                }
                else if(block.CheckJellyCollision(this.squidCollider))
                {
                    this.soundManager.PlaySound(SoundManager.SOUNDS.JellyCrash);
                    gameOverDetected = true;
                }

                if (gameOverDetected)
                {
                    var transition = new ColorFadeTransition(Color.White, TimeSpan.FromSeconds(0.5f));
                    WaveServices.ScreenContextManager.Push(new ScreenContext(new GameOverScene()), transition);
                }

                var diff = block.Transform2D.Y - WaveServices.ViewportManager.BottomEdge;
                if (diff > 0)
                {
                    // Remove this block
                    block.Reset();
                    this.visibleBlocks.Remove(block);
                    block.Entity.Enabled = false;

                    // Add a new block instead
                    var selectedBlock = this.avaibleBlocks[WaveServices.Random.Next(this.avaibleBlocks.Count)];
                    selectedBlock.Transform2D.Y = diff - virtualScreenHeight;
                    this.avaibleBlocks.Remove(selectedBlock);
                    this.visibleBlocks.Add(selectedBlock);
                    selectedBlock.Entity.Enabled = true;

                    // Set removed block as avaible again
                    this.avaibleBlocks.Add(block);
                }
            }

            // Decrease scroll velocity
            if (this.scrollVelocity > this.MIN_SCROLL_VELOCITY)
            {
                this.scrollVelocity -= 0.01f * (float)gameTime.TotalSeconds * 60;
            }

#if DEBUG
            var blockTypesStr = string.Join(", ", this.visibleBlocks.Select(vb => vb.BlockType.ToString()));
            Labels.Add("Block types", blockTypesStr);
#endif
        }

        /// <summary>
        /// Get a list with an instance of all avaible BlockTypes.
        /// </summary>
        /// <returns>A list with an instance of all avaible BlockTypes.</returns>
        private List<RocksBlock> GetAvaibleBlocks()
        {
            List<RocksBlock> avaibleBlocks = new List<RocksBlock>();

            foreach (var blockType in Enum.GetValues(typeof(RocksBlock.BlockTypes)))
            {
                avaibleBlocks.Add(new RocksBlock((RocksBlock.BlockTypes)blockType));
            }

            return avaibleBlocks;
        }

        /// <summary>
        /// Reset all blocks and add new initial blocks
        /// </summary>
        public void Reset()
        {
            // Remove all visible blocks
            foreach (var visibleBlock in this.visibleBlocks)
            {
                visibleBlock.Reset();
                visibleBlock.Entity.Enabled = false;
                this.avaibleBlocks.Add(visibleBlock);
            }
            this.visibleBlocks.Clear();

            var virtualScreenHeight = WaveServices.ViewportManager.BottomEdge - WaveServices.ViewportManager.TopEdge;

            // Add initial blocks
            for (int i = 0; i < this.MAX_VISIBLE_BLOCKS; i++)
            {
                var selectedBlock = this.avaibleBlocks[WaveServices.Random.Next(this.avaibleBlocks.Count)];

                this.avaibleBlocks.Remove(selectedBlock);

                selectedBlock.Transform2D.Y = -virtualScreenHeight * (i + 1);

                this.visibleBlocks.Add(selectedBlock);
                selectedBlock.Entity.Enabled = true;
            }
        }

        /// <summary>
        /// Applies an vertical impulse.
        /// </summary>
        public void ApplyImpulse()
        {
            this.scrollVelocity = 0.5f;
        }
    }
}
