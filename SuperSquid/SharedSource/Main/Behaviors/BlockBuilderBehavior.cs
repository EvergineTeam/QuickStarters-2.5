#region File Description
//-----------------------------------------------------------------------------
// Super Squid
//
// Quickstarter for Wave University Tour 2014.
// Author: Wave Engine Team
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using SuperSquid.Components;
using SuperSquid.Managers;
using SuperSquid.Scenes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Components.Transitions;
using WaveEngine.Framework;
using WaveEngine.Framework.Diagnostic;
using WaveEngine.Framework.Physics2D;
using WaveEngine.Framework.Services;
#endregion

namespace SuperSquid.Entities.Behaviors
{
    [DataContract(Namespace = "SuperSquid.Entities.Behaviors")] 
    public class BlockBuilderBehavior : Behavior
    {
        private const int MAX_VISIBLE_BLOCKS = 2;
        private const float MIN_SCROLL_VELOCITY = 0.1f;

        private List<RocksBlock> visibleBlocks;
        private List<RocksBlock> avaibleBlocks;
        private Collider2D squidCollider;

        private SoundManager soundManager;

        private GamePlayManager gamePlayManager;        
        private float scrollVelocity;

        public event EventHandler OnCollision;
        public event EventHandler OnStarCollected;

        protected override void DefaultValues()
        {
            base.DefaultValues();
            this.scrollVelocity = MIN_SCROLL_VELOCITY;
        }

        /// <summary>
        /// Resolves the dependencies needed for this instance to work.
        /// </summary>
        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            this.visibleBlocks = new List<RocksBlock>();
            this.avaibleBlocks = this.Owner.ChildEntities.Select(c => c.FindComponent<RocksBlock>()).ToList();

            var squid = this.EntityManager.Find("Squid");
            if (squid != null)
            {
                this.squidCollider = squid.FindComponent<Collider2D>(false);
            }

            var gamePlayEntity = this.EntityManager.Find("GamePlayManager");
            if (gamePlayEntity != null)
            {
                this.gamePlayManager = gamePlayEntity.FindComponent<GamePlayManager>();
            }
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

            ////this.scene = (GamePlayScene)this.Owner.Scene;
            ////this.backScene = WaveServices.ScreenContextManager.FindContextByName("BackContext")
            ////                                                  .FindScene<BackgroundScene>();

            this.soundManager = WaveServices.GetService<SoundManager>();
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
            float virtualScreenHeight = WaveServices.ViewportManager.BottomEdge - WaveServices.ViewportManager.TopEdge;

            foreach (RocksBlock block in this.visibleBlocks.ToList())
            {
                block.Transform2D.Y += scrollVelocity * (float)gameTime.TotalMilliseconds;

                if (block.CheckStarCollision(this.squidCollider))
                {
                    this.soundManager.PlaySound(SoundManager.SOUNDS.Star);                    
                    if(this.OnStarCollected != null)
                    {
                        this.OnStarCollected(this, null);
                    }
                }

                bool gameOverDetected = false;

                if (block.CheckRockCollision(this.squidCollider))
                {
                    this.soundManager.PlaySound(SoundManager.SOUNDS.RockCrash);
                    gameOverDetected = true;
                }
                else if (block.CheckJellyFishCollision(this.squidCollider))
                {
                    this.soundManager.PlaySound(SoundManager.SOUNDS.JellyCrash);
                    gameOverDetected = true;
                }

                if (gameOverDetected)
                {
                    if (this.OnCollision != null)
                    {
                        this.OnCollision(this, null);
                    }

                    break; 
                }

                var diff = block.Transform2D.Y - WaveServices.ViewportManager.BottomEdge;
                if (diff > 0)
                {
                    // Remove this block
                    block.Reset();
                    this.visibleBlocks.Remove(block);
                    block.Owner.Enabled = false;

                    // Add a new block instead
                    var selectedBlock = this.avaibleBlocks[WaveServices.Random.Next(this.avaibleBlocks.Count)];
                    selectedBlock.Transform2D.Y = diff - virtualScreenHeight;
                    this.avaibleBlocks.Remove(selectedBlock);
                    this.visibleBlocks.Add(selectedBlock);
                    selectedBlock.Owner.Enabled = true;

                    // Set removed block as avaible again
                    this.avaibleBlocks.Add(block);
                }
            }

            // Decrease scroll velocity
            if (this.scrollVelocity > MIN_SCROLL_VELOCITY)
            {
                this.scrollVelocity -= 0.01f * (float)gameTime.TotalSeconds * 60;
            }
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
                visibleBlock.Owner.Enabled = false;
                this.avaibleBlocks.Add(visibleBlock);
            }

            this.visibleBlocks.Clear();

            float screenHeight;

            if(WaveServices.ViewportManager.IsActivated)
            {
                screenHeight = WaveServices.ViewportManager.BottomEdge - WaveServices.ViewportManager.TopEdge;
            }
            else
            {
                screenHeight = WaveServices.Platform.ScreenHeight;
            }

            // Add initial blocks
            for (int i = 0; i < MAX_VISIBLE_BLOCKS; i++)
            {
                var selectedBlock = this.avaibleBlocks[WaveServices.Random.Next(this.avaibleBlocks.Count)];

                this.avaibleBlocks.Remove(selectedBlock);

                selectedBlock.Transform2D.Y = -screenHeight * (i + 1);

                this.visibleBlocks.Add(selectedBlock);
                selectedBlock.Owner.Enabled = true;
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
