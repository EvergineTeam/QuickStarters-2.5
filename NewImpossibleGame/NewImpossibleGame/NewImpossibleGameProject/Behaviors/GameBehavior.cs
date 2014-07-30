using NewImpossibleGameProject.Enums;
using NewImpossibleGameProject.GameModels;
using NewImpossibleGameProject.GameServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Components.Graphics3D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics3D;

namespace NewImpossibleGameProject.Behaviors
{
    /// <summary>
    /// 
    /// </summary>
    public class GameBehavior : SceneBehavior
    {
        /// <summary>
        /// The column collection
        /// </summary>
        private List<Entity> ColumnCollection = new List<Entity>();

        /// <summary>
        /// The player velocity
        /// </summary>
        public float PlayerVelocity = 15f;

        /// <summary>
        /// The boost velocity
        /// </summary>
        private float boostVelocity = 0.0f;

        /// <summary>
        /// The decelration
        /// </summary>
        private const float DECELERATION = 7f;

        /// <summary>
        /// The acceleration
        /// </summary>
        private const float ACCELERATION = 10f;

        /// <summary>
        /// The underdeadlevel. Player dies under this falling value
        /// </summary>
        private const float UNDERDEADLEVEL = -7.0f;

        /// <summary>
        /// The block buffer size
        /// </summary>
        private const int BLOCKBUFFERSIZE = 23;

        /// <summary>
        /// The passedbyblockstodiscard
        /// </summary>
        private const int PASSEDBYBLOCKSTODISCARDCOLUMN = 5;

        /// <summary>
        /// The camera player position
        /// </summary>
        private Vector3 cameraPlayerPosition = new Vector3(-10, 5, -5);

        /// <summary>
        /// The camera player lookat
        /// </summary>
        private Vector3 cameraPlayerLookat = new Vector3(0, 3, 2);

        /// <summary>
        /// The game state
        /// </summary>
        public GameState GameState = GameState.INIT;

        /// <summary>
        /// The player bounding box
        /// </summary>
        private BoundingBox playerBoundingBox;

        /// <summary>
        /// The play scene
        /// </summary>
        private PlayScene playScene;

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
        private BoxCollider playerCollider;

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
        /// Initializes a new instance of the <see cref="GameBehavior"/> class.
        /// </summary>
        public GameBehavior()
            : base("GameBehavior")
        {
        }

        /// <summary>
        /// Resolves the dependencies needed for this instance to work.
        /// </summary>
        protected override void ResolveDependencies()
        {
            this.modelFactoryService = ModelFactoryService.Instance;
            this.playScene = this.Scene as PlayScene;
            this.playerTransform = this.playScene.Player.FindComponent<Transform3D>();
            this.playerCollider = this.playScene.Player.FindComponent<BoxCollider>();
            this.playerBoundingBox = new BoundingBox(this.playerTransform.Position - this.playerCollider.Size / 2, this.playerTransform.Position + this.playerCollider.Size / 2);
            this.tempBoundingBox = new BoundingBox();
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
            float elapsedGameTime = (float)gameTime.TotalSeconds;

            // Game Tri-state machine
            switch (GameState)
            {
                // Initial level state
                case GameState.INIT:
                    // Loads the level, reloads every time player dead to reset the collection and column order... 
                    // it can be done in everywhere we want, in scene too, but then we need to call a Reset method or similar
                    this.LevelModel = ImportService.Instance.ImportLevel("Content/Levels/level1A.level");

                    // Restart Camera, Player states and positions
                    this.playScene.Player.FindComponent<PlayerBehavior>().Restart();
                    this.boostVelocity = 0.0f;

                    // put player over the first ground block
                    // FIRST BLOCK OF LEVEL SHOULD BE A GROUND, logically
                    this.playerTransform.Position = new Vector3(0f, this.modelFactoryService.Scale.Y, 0.0f);
                    this.playScene.GameCamera.Position = cameraPlayerPosition;
                    this.playScene.GameCamera.LookAt = cameraPlayerLookat;

                    // fills the intiial Buffer, we need to load some elements prior playing
                    float currentZ = 0;
                    for (int i = 0; i < BLOCKBUFFERSIZE; i++)
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
                    this.UpdatePlayerPosition(elapsedGameTime);

                    // Check if dead by level platform falling down
                    if (this.playerTransform.Position.Y <= UNDERDEADLEVEL)
                    {
                        this.GameState = Enums.GameState.DIE;
                    }

                    // used for find the block limits.
                    var middleSize = this.modelFactoryService.Scale.Z / 2;

                    // Selects the current and next column, selects the columns to free too.
                    // the current column and next column are used to check collisions, only with those two columns, others are ignored
                    for (int i = 0; i < this.ColumnCollection.Count - 1; i++)
                    {
                        Entity column = this.ColumnCollection[i];
                        var columnTransform = column.FindComponent<Transform3D>();

                        // Remove passedby columns by distance
                        if (columnTransform.Position.Z < this.playerTransform.Position.Z - PASSEDBYBLOCKSTODISCARDCOLUMN * this.modelFactoryService.Scale.Z)
                        {
                            // removes column
                            this.modelFactoryService.FreeColumn(column);
                            this.playScene.EntityManager.Remove(column);
                            this.ColumnCollection.RemoveAt(i);

                            // Create new column at the end
                            this.CreateNextColumnEntity(columnTransform.Position.Z + BLOCKBUFFERSIZE * this.modelFactoryService.Scale.Z);
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
                    if (this.CurrentColumn != null)
                    {
                        // creates the union of each column entities
                        List<Entity> collidableCollection = this.CurrentColumn.ChildEntities.ToList();
                        collidableCollection.AddRange(this.NextColumn.ChildEntities.ToList());

                        // check if collides
                        foreach (Entity block in collidableCollection)
                        {
                            var blockTransform = block.FindComponent<Transform3D>();
                            var blockCollider = block.FindComponent<BoxCollider>();
                            // updates the block boundingbox to check collision
                            this.tempBoundingBox.Max = blockTransform.Position + blockCollider.Size / 2;
                            this.tempBoundingBox.Min = blockTransform.Position - blockCollider.Size / 2;

                            // we use intersects of boundboxes cause collider class has not a Intersects with boundingboxes
                            if (this.tempBoundingBox.Intersects(this.playerBoundingBox))
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
                                        if (this.playerTransform.Position.Y - this.modelFactoryService.Scale.Y / 2 < blockTransform.Position.Y)
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
                                        this.boostVelocity += ACCELERATION * elapsedGameTime;
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
                        this.playScene.EntityManager.Remove(column);
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
        /// Creates the next column entity.
        /// </summary>
        /// <param name="currentZ">The current z.</param>
        private void CreateNextColumnEntity(float currentZ)
        {
            // load next column from model and create the entities at next position
            var modelColumn = this.LevelModel.GetNextColumn();
            var entityColumn = this.modelFactoryService.CreateColumn(modelColumn, currentZ);
            var entityTransform = entityColumn.FindComponent<Transform3D>();
            entityTransform.Position.Z = currentZ;
            this.playScene.EntityManager.Add(entityColumn);
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
        /// Gets the ground level.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Updates the player position.
        /// </summary>
        /// <param name="elapsedGameTime">The elapsed game time.</param>
        private void UpdatePlayerPosition(float elapsedGameTime)
        {
            // New Player Position
            // delta movement using boostvelocity
            var delta = (this.PlayerVelocity + this.boostVelocity) * elapsedGameTime;

            // check boost velocity to decelerate
            if (this.boostVelocity > 0.0)
            {
                this.boostVelocity -= DECELERATION * elapsedGameTime;

                // minimum boost is 0.0
                if (this.boostVelocity < 0.0f)
                {
                    this.boostVelocity = 0.0f;
                }
            }

            // update transforms position for player and camera
            this.playerTransform.Position.Z += delta;
            this.playScene.GameCamera.Position.Z = this.playerTransform.Position.Z + this.cameraPlayerPosition.Z;
            this.playScene.GameCamera.LookAt.Z = this.playerTransform.Position.Z + this.cameraPlayerLookat.Z;

            // update player bounding box
            this.playerBoundingBox.Min.Y = this.playerTransform.Position.Y - this.modelFactoryService.Scale.Y / 2;
            this.playerBoundingBox.Max.Y = this.playerTransform.Position.Y + this.modelFactoryService.Scale.Y / 2;
            this.playerBoundingBox.Min.Z = this.playerTransform.Position.Z - this.modelFactoryService.Scale.Z / 2;
            this.playerBoundingBox.Max.Z = this.playerTransform.Position.Z + this.modelFactoryService.Scale.Z / 2;
        }
    }
}
