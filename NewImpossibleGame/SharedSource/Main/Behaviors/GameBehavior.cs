using NewImpossibleGame.Enums;
using NewImpossibleGame.Models;
using NewImpossibleGame.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics3D;

namespace NewImpossibleGame.Behaviors
{
    /// <summary>
    /// Game Bahevior
    /// </summary>
    [DataContract]
    public class GameBehavior : Behavior
    {
        /// <summary>
        /// The column collection
        /// </summary>
        private List<Entity> ColumnCollection;

        /// <summary>
        /// The underdeadlevel. Player dies under this falling value
        /// </summary>
        public float UnderDeadLevel
        {
            get;
            set;
        }

        /// <summary>
        /// The block buffer size
        /// </summary>
        [DataMember]
        public int BlockBufferSize
        {
            get;
            set;
        }

        /// <summary>
        /// The passedbyblockstodiscard
        /// </summary>
        [DataMember]
        public int PasseByBlocksToDiscardColumn
        {
            get;
            set;
        }

        [DataMember]
        [RenderPropertyAsEntity]
        public string PlayerSource
        {
            get;
            set;
        }

        /// <summary>
        /// The game state
        /// </summary>
        public GameState GameState = GameState.INIT;

        /// <summary>
        /// The play scene
        /// </summary>
        private MyScene playScene;

        /// <summary>
        /// The model factory service
        /// </summary>
        private ModelFactoryService modelFactoryService;

        /// <summary>
        /// The player transform
        /// </summary>
        private Transform3D playerTransform;

        /// <summary>
        /// The player collider
        /// </summary>
        private BoxCollider3D playerCollider;

        /// <summary>
        /// The temporary collider
        /// </summary>
        private BoundingBox tempBoundingBox;

        /// <summary>
        /// The current column
        /// </summary>
        public Entity CurrentColumn;

        /// <summary>
        /// The current column
        /// </summary>
        public Entity NextColumn;

        /// <summary>
        /// The current ground level
        /// </summary>
        public float CurrentGroundLevel;

        /// <summary>
        /// The next ground level
        /// </summary>
        public float NextGroundLevel;

        /// <summary>
        /// The level model
        /// </summary>
        public LevelModel LevelModel;

        /// <summary>
        /// The player
        /// </summary>
        private Entity player;

        /// <summary>
        /// The player behavior
        /// </summary>
        private PlayerBehavior playerBehavior;

        /// <summary>
        /// The initialized
        /// </summary>
        private bool initialized = false;

        /// <summary>
        /// Defaults the values.
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();

            this.UnderDeadLevel = -7.0f;
            this.BlockBufferSize = 23;
            this.PasseByBlocksToDiscardColumn = 5;

            this.ColumnCollection = new List<Entity>();
        }

        /// <summary>
        /// Allows this instance to execute custom logic during its 
        /// <c>Update</c>.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        /// <remarks>
        /// This method will not be executed if it are not 
        /// <c>Active</c>.
        /// </remarks>
        protected override void Update(TimeSpan gameTime)
        {
            if (!this.initialized)
            {
                this.modelFactoryService = ModelFactoryService.Instance;
                this.playScene = this.Owner.Scene as MyScene;
                this.player = this.EntityManager.Find(this.PlayerSource);
                this.playerTransform = this.player.FindComponent<Transform3D>();
                this.playerCollider = this.player.FindComponent<BoxCollider3D>();
                this.playerBehavior = this.player.FindComponent<PlayerBehavior>();
                this.tempBoundingBox = new BoundingBox();

                this.initialized = true;
            }

            // Game Tri-state machine
            switch (GameState)
            {
                // Initial level state
                case GameState.INIT:
                    // Loads the level
                    // it can be done in everywhere we want, in scene too, but then we need to call a Reset method or similar
                    this.LevelModel = ImportService.Instance.ImportLevel(WaveContent.Assets.Levels.level1A_level);

                    // Restart Camera, Player states and positions
                    this.player.FindComponent<PlayerBehavior>().Restart();

                    // put player over the first ground block
                    // FIRST BLOCK OF LEVEL SHOULD BE A GROUND, logically
                    this.playerTransform.Position = new Vector3(0f, this.modelFactoryService.Scale.Y, 0.0f);

                    // fills the intiial Buffer, we need to load some elements prior playing
                    float currentZ = 0;
                    for (int i = 0; i < BlockBufferSize; i++)
                    {
                        // Create Column
                        this.CreateNextColumnEntity(currentZ);
                        currentZ += this.modelFactoryService.Scale.Z;
                    }
                    GameState = GameState.PLAY;
                    break;

                // Playing State of game:
                case GameState.PLAY:
                    // new player position, using acceleration, falling, etc... where applicable
                    //// this.UpdatePlayerPosition(elapsedGameTime);

                    // Check if dead by level platform falling down
                    if (this.playerTransform.Position.Y <= UnderDeadLevel)
                    {
                        this.GameState = Enums.GameState.DIE;
                        break;
                    }

                    // Selects the current and next column, selects the columns to free too.
                    // the current column and next column are used to check collisions, only with those two columns, others are ignored
                    for (int i = 0; i < this.ColumnCollection.Count - 1; i++)
                    {
                        Entity column = this.ColumnCollection[i];
                        var columnTransform = column.FindComponent<Transform3D>();

                        // Remove passedby columns by distance
                        if (columnTransform.Position.Z < this.playerTransform.Position.Z - PasseByBlocksToDiscardColumn * this.modelFactoryService.Scale.Z)
                        {
                            ////// removes column
                            this.modelFactoryService.FreeColumn(column);
                            this.ColumnCollection.RemoveAt(i);

                            ////// Create new column at the end
                            this.CreateNextColumnEntity(columnTransform.Position.Z + BlockBufferSize * this.modelFactoryService.Scale.Z);
                        }
                        // check if player is over this column and sets current and next column
                        else if (this.playerTransform.Position.Z < columnTransform.Position.Z + this.modelFactoryService.Scale.Z
                           && this.playerTransform.Position.Z >= columnTransform.Position.Z)
                        {
                            this.CurrentColumn = column;
                            this.NextColumn = this.ColumnCollection[i + 1];

                            // update the ground level for current and next columns
                            this.CheckGroundPosition();
                            break;
                        }
                    }

                    // if there are a current column checks the collision in current and next column
                    if (this.CurrentColumn != null && !this.CurrentColumn.IsDisposed && this.NextColumn!=null && !this.NextColumn.IsDisposed)
                    {
                        // creates the union of each column entities
                        List<Entity> collidableCollection = this.CurrentColumn.ChildEntities.ToList();
                        collidableCollection.AddRange(this.NextColumn.ChildEntities.ToList());

                        // check if collides
                        foreach (Entity block in collidableCollection)
                        {
                            var blockTransform = block.FindComponent<Transform3D>();
                            var blockCollider = block.FindComponent<BoxCollider3D>();

                            // updates the block boundingbox to check collision
                            this.tempBoundingBox.Max = blockTransform.Position + blockCollider.Size / 2;
                            this.tempBoundingBox.Min = blockTransform.Position - blockCollider.Size / 2;

                            // we use intersects of boundboxes cause collider class has not a Intersects with boundingboxes
                            if (this.tempBoundingBox.Intersects(this.playerBehavior.PlayerBoundingBox))
                            {
                                BlockTypeEnum blockType = BlockTypeEnum.EMPTY;
                                Enum.TryParse<BlockTypeEnum>(block.Tag, out blockType);

                                // if player colliders with the block, we must to check the block effect:
                                switch (blockType)
                                {
                                    case BlockTypeEnum.EMPTY:
                                        break;
                                    // ground and box obstacles can walk over they, but if crash horizontally player dies
                                    case BlockTypeEnum.GROUND:
                                    case BlockTypeEnum.BOX:
                                        if (this.playerBehavior.Collides(blockTransform))
                                        {
                                            this.GameState = Enums.GameState.DIE;
                                        }
                                        break;
                                    case BlockTypeEnum.PYRAMID:
                                        // pyramid collision dies player
                                        this.GameState = Enums.GameState.DIE;
                                        break;
                                    case BlockTypeEnum.SPEEDERBLOCK:
                                        // if collide with speeder then player accelerates
                                        this.playerBehavior.Accelerate((float)gameTime.TotalSeconds);
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }
                    break;

                // player die state
                case GameState.DIE:
                    // free every column entity and remove containers from entitymanager
                    foreach (Entity column in this.ColumnCollection)
                    {
                        this.modelFactoryService.FreeColumn(column);
                    }

                    // clears the collection and colums
                    this.ColumnCollection.Clear();
                    this.CurrentColumn = null;
                    this.NextColumn = null;

                    // just init
                    GameState = GameState.INIT;
                    break;
            }
        }

        /// <summary>
        /// Creates the next column entity by Z position.
        /// </summary>
        /// <param name="currentZ">The current z.</param>
        private void CreateNextColumnEntity(float currentZ)
        {
            // load next column from model and create the entities at next position
            var modelColumn = this.LevelModel.GetNextColumn();
            var entityColumn = this.modelFactoryService.CreateColumn(modelColumn, currentZ, this.EntityManager);

            // this.playScene.EntityManager.Add(entityColumn);
            this.ColumnCollection.Add(entityColumn);
        }

        /// <summary>
        /// Checks the ground position.
        /// </summary>
        private void CheckGroundPosition()
        {
            // Updates the ground levels by current and next columns
            float current = this.GetGroundLevel(this.CurrentColumn);
            this.NextGroundLevel = this.GetGroundLevel(this.NextColumn);

            if (this.NextGroundLevel > current && this.CurrentGroundLevel <= current)
            {
                this.CurrentGroundLevel = this.NextGroundLevel;
            }
            else
            {
                this.CurrentGroundLevel = current;
            }
        }

        /// <summary>
        /// Gets the ground level of a column entity.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <returns>The ground level</returns>
        private float GetGroundLevel(Entity column)
        {
            // calculates the ground level of the column by player distance to the entities

            // falling to infinite!!
            float res = float.MinValue;

            float minDistance = float.MaxValue;
            Transform3D minBlockTransform = null;

            if (column != null)
            {
                foreach (Entity element in column.ChildEntities)
                {
                    // get the type of every block... only GROUND and BOX can walk over
                    BlockTypeEnum blockType = BlockTypeEnum.EMPTY;
                    Enum.TryParse<BlockTypeEnum>(element.Tag, out blockType);

                    // only the blocks you can walk over used to calculate grounds
                    if (blockType == BlockTypeEnum.GROUND || blockType == BlockTypeEnum.BOX || blockType == BlockTypeEnum.SPEEDERBLOCK)
                    {
                        Transform3D blockTransform = element.FindComponent<Transform3D>();

                        // use the nearest under player block as ground
                        if (this.playerTransform.Position.Y >= blockTransform.Position.Y + this.modelFactoryService.Scale.Y)
                        {
                            var distance = Vector3.Distance(this.playerTransform.Position, blockTransform.Position);
                            // updates the lower block
                            if (distance < minDistance)
                            {
                                minDistance = distance;
                                minBlockTransform = blockTransform;
                            }
                        }
                    }
                }

                // If there are any nearest block update ground level, otherwise then level is float.Minimum
                if (minBlockTransform != null)
                {
                    res = minBlockTransform.Position.Y + this.modelFactoryService.Scale.Y / 2;
                }
            }
            return res;
        }
    }
}
