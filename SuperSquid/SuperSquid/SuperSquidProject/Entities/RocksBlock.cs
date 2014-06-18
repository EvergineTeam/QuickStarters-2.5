#region File Description
//-----------------------------------------------------------------------------
// Super Squid
//
// Quickstarter for Wave University Tour 2014.
// Author: Wave Engine Team
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using SuperSquidProject.Entities.Behaviors;
using SuperSquidProject.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics2D;
using WaveEngine.Framework.Services;
#endregion

namespace SuperSquidProject.Entities
{
    public class RocksBlock : BaseDecorator
    {
        public enum BlockTypes
        {
            One,
            Two,
            Three,
            Four,
            Five,
            Six,
            Seven
        };

        private List<Rock> rocks;
        private List<JellyFish> jellyFishs;
        private List<StarFish> stars;

        public Transform2D Transform2D { get; set; }

        public BlockTypes BlockType { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RocksBlock" /> class.
        /// </summary>
        /// <param name="blockType">Type of the block.</param>
        public RocksBlock(BlockTypes blockType)
        {
            this.BlockType = blockType;

            this.entity = new Entity()
                        .AddComponent(new Transform2D());

            // Cached
            this.Transform2D = this.entity.FindComponent<Transform2D>();

            this.rocks = new List<Rock>();
            this.jellyFishs = new List<JellyFish>();
            this.stars = new List<StarFish>();

            switch (blockType)
            {
                case BlockTypes.One:
                    this.jellyFishs.Add(new JellyFish(JellyFish.JellyFishType.Little, new Vector2(176, 154)));
                    this.rocks.Add(new Rock(Rock.RockType.Left, new Vector2(0, 616)));
                    this.rocks.Add(new Rock(Rock.RockType.Right, new Vector2(WaveServices.ViewportManager.VirtualWidth, 163)));
                    this.stars.Add(new StarFish(new Vector2(711, 640)));
                    this.stars.Add(new StarFish(new Vector2(49, 460)));
                    break;
                case BlockTypes.Two:
                    this.rocks.Add(new Rock(Rock.RockType.Center, new Vector2(231, 233)));
                    this.rocks.Add(new Rock(Rock.RockType.Center, new Vector2(546, 739)));
                    this.stars.Add(new StarFish(new Vector2(178, 664)));
                    this.stars.Add(new StarFish(new Vector2(571, 209)));
                    break;
                case BlockTypes.Three:
                    this.jellyFishs.Add(new JellyFish(JellyFish.JellyFishType.Big, new Vector2(170, 212)));
                    this.rocks.Add(new Rock(Rock.RockType.Center, new Vector2(570, 512)));
                    this.stars.Add(new StarFish(new Vector2(625, 857)));
                    this.stars.Add(new StarFish(new Vector2(91, 506)));
                    this.stars.Add(new StarFish(new Vector2(380, 198)));
                    break;
                case BlockTypes.Four:
                    this.jellyFishs.Add(new JellyFish(JellyFish.JellyFishType.Little, new Vector2(170, 807)));
                    this.rocks.Add(new Rock(Rock.RockType.Left, new Vector2(0, 163)));
                    this.rocks.Add(new Rock(Rock.RockType.Right, new Vector2(WaveServices.ViewportManager.VirtualWidth + 60, 616)));
                    this.stars.Add(new StarFish(new Vector2(130, 580)));
                    this.stars.Add(new StarFish(new Vector2(573, 478)));
                    this.stars.Add(new StarFish(new Vector2(110, 68)));
                    break;
                case BlockTypes.Five:
                    this.jellyFishs.Add(new JellyFish(JellyFish.JellyFishType.Little, new Vector2(165, 769)));
                    this.jellyFishs.Add(new JellyFish(JellyFish.JellyFishType.Little, new Vector2(620, 912), false));
                    this.jellyFishs.Add(new JellyFish(JellyFish.JellyFishType.Big, new Vector2(194, 208)));
                    this.stars.Add(new StarFish(new Vector2(86, 326)));
                    this.stars.Add(new StarFish(new Vector2(195, 630)));
                    this.stars.Add(new StarFish(new Vector2(648, 423)));
                    break;
                case BlockTypes.Six:
                    this.rocks.Add(new Rock(Rock.RockType.Right, new Vector2(WaveServices.ViewportManager.VirtualWidth, 171)));
                    this.jellyFishs.Add(new JellyFish(JellyFish.JellyFishType.Little, new Vector2(98, 418)));
                    this.jellyFishs.Add(new JellyFish(JellyFish.JellyFishType.Little, new Vector2(165, 807)));
                    this.jellyFishs.Add(new JellyFish(JellyFish.JellyFishType.Big, new Vector2(553, 835), false));
                    this.stars.Add(new StarFish(new Vector2(243, 174)));
                    this.stars.Add(new StarFish(new Vector2(203, 616)));
                    break;
                case BlockTypes.Seven:
                    this.rocks.Add(new Rock(Rock.RockType.Left, new Vector2(-193, 469)));
                    this.rocks.Add(new Rock(Rock.RockType.Right, new Vector2(WaveServices.ViewportManager.VirtualWidth + 119, 74)));
                    this.jellyFishs.Add(new JellyFish(JellyFish.JellyFishType.Big, new Vector2(152, 235)));
                    this.jellyFishs.Add(new JellyFish(JellyFish.JellyFishType.Little, new Vector2(444, 433), false));
                    this.jellyFishs.Add(new JellyFish(JellyFish.JellyFishType.Little, new Vector2(316, 835)));
                    this.stars.Add(new StarFish(new Vector2(379, 232)));
                    this.stars.Add(new StarFish(new Vector2(379, 616)));
                    break;
            }

            foreach (Rock rock in this.rocks)
            {
                this.entity.AddChild(rock.Entity);
            }

            foreach (JellyFish jellyFish in this.jellyFishs)
            {
                this.entity.AddChild(jellyFish.Entity);
            }

            foreach (StarFish star in this.stars)
            {
                this.entity.AddChild(star.Entity);
            }
        }

        /// <summary>
        /// Check if any rock element collides with the specified collider.
        /// </summary>
        /// <param name="collider">The collider.</param>
        /// <returns></returns>
        public bool CheckRockCollision(Collider2D collider)
        {
            bool collision = false;

            foreach (Rock rock in this.rocks)
            {
                if (collider.Intersects(rock.Collider))
                {
                    collision = true;

                    break;
                }
            }

            return collision;
        }

        /// <summary>
        /// Check if any jelly element collides with the specified collider.
        /// </summary>
        /// <param name="collider">The collider.</param>
        /// <returns></returns>
        public bool CheckJellyCollision(Collider2D collider)
        {
            bool collision = false;

            foreach (JellyFish jellyFish in this.jellyFishs)
            {
                if (collider.Intersects(jellyFish.Collider))
                {
                    collision = true;

                    break;
                }
            }

            return collision;
        }

        /// <summary>
        /// Check if any star element collides with the specified collider.
        /// </summary>
        /// <param name="collider">The collider.</param>
        /// <returns></returns>
        public bool CheckStarCollision(Collider2D collider)
        {
            bool collision = false;

            foreach (StarFish star in this.stars)
            {
                if (star.IsVisible && collider.Intersects(star.collider))
                {
                    star.IsVisible = false;
                    collision = true;
                }
            }

            return collision;
        }

        /// <summary>
        /// Reset this instance
        /// </summary>
        public void Reset()
        {
            foreach (StarFish star in this.stars)
            {
                star.IsVisible = true;
            }
        }
    }
}
