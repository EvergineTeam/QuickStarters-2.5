using NewImpossibleGame.Enums;
using NewImpossibleGame.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Components.Graphics3D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Managers;
using WaveEngine.Framework.Physics3D;
using WaveEngine.Materials;

namespace NewImpossibleGame.Services
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
        /// The column cache
        /// </summary>
        private Queue<Entity> columnCache;

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

            this.columnCache = new Queue<Entity>();
        }

        /// <summary>
        /// Creates a new column with recycled components.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <param name="zPosition">The z position.</param>
        /// <returns></returns>
        public Entity CreateColumn(Column column, float zPosition, EntityManager eManager)
        {
            Entity entityColumn = null;

            // Column Cache
            if (this.columnCache.Count > 0)
            {
                entityColumn = this.columnCache.Dequeue();
            }
            else
            {
                entityColumn = new Entity()
                    {
                        Tag = COLUMNTAG
                    }
                    .AddComponent(new Transform3D());

                eManager.Add(entityColumn);
            }

            var transform = entityColumn.FindComponent<Transform3D>();
            Vector3 positiontemp = transform.Position;
            positiontemp.Z = zPosition;
            transform.Position = positiontemp;
            entityColumn.IsVisible = true;

            // column element iteration to block entity Creation
            for (int i = 0; i < column.Count; i++)
            {
                var position = new Vector3(0, i * this.Scale.Z, 0);
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

            this.columnCache.Enqueue(entityColumn);
            entityColumn.IsVisible = false;
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
                var modelRenderer = entity.FindComponent<ModelRenderer>();
                modelRenderer.IsVisible = true;
                entity.IsActive = true;

                // reset position
                Transform3D transform = entity.FindComponent<Transform3D>();
                transform.LocalPosition = position;
                transform.Scale = scale;

                // IMPORTANT: DO NOT REMOVE!! We need to Refresh boundbox of the model to locate it at right position.
                entity.FindComponent<Model>(false).BoundingBoxRefreshed = true;

                // change color map if needed
                MaterialsMap materialMap = entity.FindComponent<MaterialsMap>();
                StandardMaterial basicMaterial = (StandardMaterial)materialMap.DefaultMaterial;
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
                        .AddComponent(new MaterialsMap(new StandardMaterial(color, DefaultLayers.Opaque) { LightingEnabled = true, AmbientColor = Color.White * 0.25f }))
                        .AddComponent(entityModel)
                        .AddComponent(new BoxCollider3D())
                        .AddComponent(new ModelRenderer());
                    entity.Tag = entityType.ToString();

                    // DO NOT push in pool here!!, only add to cache on remove!
                    // this.blockPool[entityType].Push(entity);
                }
            }
            return entity;
        }
    }
}
