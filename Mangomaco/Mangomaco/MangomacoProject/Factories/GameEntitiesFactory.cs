#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MangomacoProject.Components;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Components.Animation;
using WaveEngine.Components.Cameras;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics2D;
using WaveEngine.TiledMap;
#endregion

namespace MangomacoProject.Factories
{
    /// <summary>
    /// Game Entities Factory class
    /// </summary>
    public class GameEntitiesFactory
    {
        /// <summary>
        /// Creates the camera.
        /// </summary>
        /// <returns>Camera 2D</returns>
        public static FixedCamera2D CreateCamera()
        {
            var camera = new FixedCamera2D("Camera2D")
            {
                BackgroundColor = Resources.GameplayGBColor,
            };

            var camera2DComponent = camera.Entity.FindComponent<Camera2D>();
            camera2DComponent.Zoom = Vector2.One / 2.5f;

            return camera;
        }

        /// <summary>
        /// Creates the player.
        /// </summary>
        /// <param name="mapObject">The map object.</param>
        /// <returns>Player entity</returns>
        public static Entity CreatePlayer(TiledMapObject mapObject)
        {
            return GameEntitiesFactory.CreateMapObjectEntity("Player", "player", mapObject)
                .AddComponent(new Sprite(Resources.Ball))
                .AddComponent(new SpriteRenderer(DefaultLayers.Alpha, AddressMode.PointWrap))
                .AddComponent(new CircleCollider())
                .AddComponent(new RigidBody2D()
                {
                    AllowSleep = false
                })
                .AddComponent(new PlayerController());
        }

        /// <summary>
        /// Creates the coin.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="mapObject">The map object.</param>
        /// <returns>Coin entity</returns>
        public static Entity CreateCoin(string name, TiledMapObject mapObject)
        {
            Entity coinEntity = GameEntitiesFactory.CreateMapObjectEntity(name, "coin", mapObject)
                .AddComponent(new Sprite(Resources.Coin))
                .AddComponent(Animation2D.Create<TexturePackerGenericXml>(Resources.CoinTP))
                .AddComponent(new AnimatedSpriteRenderer(DefaultLayers.Alpha, AddressMode.PointWrap))
                .AddComponent(new CircleCollider());

            var anim = coinEntity.FindComponent<Animation2D>();
            anim.Add("flip", new SpriteSheetAnimationSequence() { First = 1, Length = 8, FramesPerSecond = 12 });
            anim.CurrentAnimation = "flip";
            anim.Play(true);

            return coinEntity;
        }

        /// <summary>
        /// Creates the crate.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="mapObject">The map object.</param>
        /// <param name="sceneTileSet">The scene tile set.</param>
        /// <param name="tileId">The tile identifier.</param>
        /// <returns>Crate entity</returns>
        public static Entity CreateCrate(string name, TiledMapObject mapObject, Tileset sceneTileSet, int tileId)
        {
            var crateRectangle = GameEntitiesFactory.GetRectangleTileByID(sceneTileSet, 190);

            return GameEntitiesFactory.CreateMapObjectEntity(name, "crate", mapObject)
                    .AddComponent(new Sprite(sceneTileSet.Image.AssetPath)
                    {
                        SourceRectangle = crateRectangle
                    })
                    .AddComponent(new SpriteRenderer(DefaultLayers.Alpha, AddressMode.PointWrap))
                    .AddComponent(new RectangleCollider())
                    .AddComponent(new RigidBody2D()
                    {
                        Mass = 0.003f
                    });
        }

        /// <summary>
        /// Creates the map object entity.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="mapObject">The map object.</param>
        /// <returns></returns>
        private static Entity CreateMapObjectEntity(string name, string tag, TiledMapObject mapObject)
        {
            return new Entity(name)
                   {
                       Tag = tag,
                       IsVisible = mapObject.Visible
                   }
                   .AddComponent(new Transform2D()
                   {
                       LocalPosition = new Vector2(mapObject.X, mapObject.Y),
                       Rotation = (float)mapObject.Rotation,
                       DrawOrder = -9
                   });
        }

        /// <summary>
        /// Gets the rectangle tile by identifier.
        /// </summary>
        /// <param name="tileset">The tileset.</param>
        /// <param name="id">The identifier.</param>
        /// <returns>Tile rectangle</returns>
        private static Rectangle GetRectangleTileByID(Tileset tileset, int id)
        {
            int row = id / tileset.YTilesCount;
            int column = id % tileset.YTilesCount;
            int x = tileset.Margin + (tileset.TileWidth + tileset.Spacing) * column;
            int y = tileset.Margin + (tileset.TileHeight + tileset.Spacing) * row;

            return new Rectangle(x, y, tileset.TileWidth, tileset.TileHeight);
        }
    }
}
