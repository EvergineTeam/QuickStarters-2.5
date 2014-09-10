using NewImpossibleGameProject.Behaviors;
using NewImpossibleGameProject.Enums;
using NewImpossibleGameProject.GameModels;
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
using WaveEngine.Materials;

namespace NewImpossibleGameProject.GameServices
{
    /// <summary>
    /// Model Factory with entity pool
    /// </summary>
    public class ModelFactoryService
    {
        // Singleton instance object
        private static ModelFactoryService instance;

        /// <summary>
        /// The columntag
        /// </summary>
        public const string COLUMNTAG = "Column";

        /// <summary>
        /// the block pool
        /// </summary>
        private Dictionary<BlockTypeEnum, Stack<Entity>> blockPool;

        /// <summary>
        /// The scale
        /// </summary>
        public Vector3 Scale;

        /// <summary>
        /// Gets the instance of the Singleton.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static ModelFactoryService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ModelFactoryService();
                }
                return instance;
            }
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="ModelFactoryService" /> class from being created.
        /// </summary>
        private ModelFactoryService()
        {
            // Initializes BlockPool
            this.blockPool = new Dictionary<BlockTypeEnum, Stack<Entity>>();
            foreach (var type in Enum.GetValues(typeof(BlockTypeEnum)))
            {
                this.blockPool.Add((BlockTypeEnum)type, new Stack<Entity>());
            }
        }

        /// <summary>
        /// Clears the pool.
        /// </summary>
        public void ClearPool()
        {
            // DO NOT REMOVE THE STACKS, ONLY THEIRS VALUES
            //this.blockPool.Clear();

            foreach (var entry in this.blockPool)
            {
                entry.Value.Clear();
            }
        }

        /// <summary>
        /// Creates a new column with recycled components.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <returns></returns>
        public Entity CreateColumn(Column column, float zPosition)
        {
            // Empty column (the container)
            Entity entityColumn = new Entity() { Tag = COLUMNTAG }
                .AddComponent(new Transform3D());

            // column element iteration to block entity Creation
            for (int i = 0; i < column.Count; i++)
            {
                var position = new Vector3(0, i * this.Scale.Z, zPosition);
                Color color = Color.Black;

                switch (column[i])
                {
                    case BlockTypeEnum.GROUND:
                        color = Color.WhiteSmoke;
                        break;
                    case BlockTypeEnum.BOX:
                        color = Color.BlueViolet;
                        break;
                    case BlockTypeEnum.PYRAMID:
                        color = Color.Red;
                        break;
                    case BlockTypeEnum.SPEEDERBLOCK:
                        color = Color.Yellow;
                        break;
                    case BlockTypeEnum.EMPTY:
                    default:
                        break;
                }

                // add child to column
                if (column[i] != BlockTypeEnum.EMPTY)
                {
                    entityColumn.AddChild(this.CreateBlock(position, color, this.Scale, column[i]));
                }
            }

            return entityColumn;
        }

        /// <summary>
        /// Frees the column from parent. Don't remove the element cause it calls Dispose, and we need recycle the object at the pool
        /// </summary>
        /// <param name="entityColumn">The entity column.</param>
        public void FreeColumn(Entity entityColumn)
        {
            // Creates a list (to allow iterate)
            var tempChildrenCollection = entityColumn.ChildEntities.ToList();

            foreach (Entity sub in tempChildrenCollection)
            {
                // Detach the entity from the parent
                entityColumn.DetachChild(sub.Name);

                // add the free block to the pool
                BlockTypeEnum blockType = BlockTypeEnum.EMPTY;
                if (Enum.TryParse<BlockTypeEnum>(sub.Tag, out blockType))
                {
                    this.blockPool[blockType].Push(sub);
                }

                sub.IsVisible = false;
                sub.IsActive = false;
            }
        }

        /// <summary>
        /// Creates the Block.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="color">The color.</param>
        /// <param name="scale">The scale.</param>
        /// <param name="entityType">Type of the entity.</param>
        /// <returns></returns>
        public Entity CreateBlock(Vector3 position, Color color, Vector3 scale, BlockTypeEnum entityType)
        {
            Entity entity = null;

            // Check if there are any block free in the pool of the type we need
            if (this.blockPool[entityType].Count > 0)
            {
                // pop from pool
                entity = this.blockPool[entityType].Pop();

                // Reset visibility and active property
                entity.IsVisible = true;
                entity.IsActive = true;

                // reset position
                Transform3D transform = entity.FindComponent<Transform3D>();
                transform.Position = position;
                transform.Scale = scale;

                // IMPORTANT: DO NOT REMOVE!! We need to Refresh boundbox of the model to locate it at right position.
                entity.FindComponent<Model>(false).BoundingBoxRefreshed = true;

                // change color map if needed
                MaterialsMap materialMap = entity.FindComponent<MaterialsMap>();
                BasicMaterial basicMaterial = (BasicMaterial)materialMap.DefaultMaterial;
                basicMaterial.DiffuseColor = color;
            }
            else
            {
                // If there are not entities in the pool of the type we need, then we should create one
                Model entityModel = null;

                // It will be better to use FBX models than primitives to optimize (primitives do not share vertexmodel)
                // but we use primitives here
                switch (entityType)
                {
                    case BlockTypeEnum.GROUND:
                    case BlockTypeEnum.BOX:
                    case BlockTypeEnum.SPEEDERBLOCK:
                        entityModel = Model.CreateCube();
                        break;
                    case BlockTypeEnum.PYRAMID:
                        entityModel = Model.CreateCone();
                        break;
                    default:
                    case BlockTypeEnum.EMPTY:
                        // SHOULD NOT HAPPEND, this method NEVER will be called if BlockType is EMPTY... but in case we forgotten this then it's controlled
                        break;
                }

                // Creates the new entity
                if (entityModel != null)
                {
                    entity = new Entity()
                        .AddComponent(new Transform3D() { Position = position, Scale = scale })
                        .AddComponent(new MaterialsMap(new BasicMaterial(color) { LightingEnabled = true, AmbientLightColor = Color.White * 0.25f }))
                        .AddComponent(entityModel)
                        .AddComponent(new BoxCollider())
                        .AddComponent(new ModelRenderer());
                    entity.Tag = entityType.ToString();

                    // DO NOT push in pool here!!, only add to cache on remove!
                    // this.blockPool[entityType].Push(entity);
                }
            }
            return entity;
        }

        /// <summary>
        /// Creates the player.
        /// </summary>
        /// <param name="gameBehavior">The game behavior.</param>
        /// <returns></returns>
        public Entity CreatePlayer(GameBehavior gameBehavior)
        {
            Entity entity = null;

            entity = new Entity("player")
                .AddComponent(new Transform3D() { })//UpdateOrder = 0 })
                .AddComponent(Model.CreateSphere())
                .AddComponent(new BoxCollider() { Size = new Vector3(this.Scale.X, this.Scale.Y, this.Scale.Z), UpdateOrder = 0.5f })
                .AddComponent(new MaterialsMap())
                .AddComponent(new ModelRenderer())
                .AddComponent(new PlayerBehavior(gameBehavior));

            return entity;
        }
    }
}
