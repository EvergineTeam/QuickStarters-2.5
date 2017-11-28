using System.IO;
using WaveEngine.Common.Math;
using WaveEngine.Common.Physics2D;
using WaveEngine.Components.Graphics3D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Managers;
using WaveEngine.Framework.Physics2D;
using WaveEngine.Framework.Services;
using WaveEngine.Materials;

namespace P2PTank.Tools
{
    public class MapLoader
    {
        private EntityManager entityManager;
        private Entity map;

        public async void Load(string filePath, Material material, EntityManager entityManager)
        {
            this.entityManager = entityManager;
            this.map = new Entity()
                .AddComponent(new Transform2D());

            var storage = WaveServices.Storage;

            if (!storage.ExistsContentFile(filePath))
            {
                throw new FileNotFoundException(filePath);
            }

            using (var stream = new StreamReader(WaveServices.Storage.OpenContentFile(filePath)))
            {
                int currentX = 0;
                int currentY = 0;

                while (!stream.EndOfStream)
                {
                    var line = await stream.ReadLineAsync();

                    foreach (var character in line)
                    {
                        if (character == '0')
                        {
                            this.CreateCube(currentX, currentY, 0, material);
                        }
                        else if (character == '1')
                        {
                            this.CreateCube(currentX, currentY, 0, material);
                            var cube = this.CreateCube(currentX, currentY, 1, material);
                            this.CreateMapCollider(currentX, currentY, cube);
                        }
                        else if (character == '9')
                        {
                            // Spawn
                            this.CreateCube(currentX, currentY, 0, material);
                        }

                        currentX++;
                    }

                    currentY++;
                    currentX = 0;
                }
            }

            this.entityManager.Add(map);
        }

        private void CreateMapCollider(int x, int y, Entity cube)
        {
            float size = 46;
            float midSize = size / 2.0f;
            var colliderEntity = new Entity()
              .AddComponent(new Transform2D()
              {
                  LocalPosition = new Vector2(x * size, y * size),
              })
              .AddComponent(new EdgeCollider2D()
              {
                  Vertices = new Vector2[]
                  {
                        new Vector2(- midSize, - midSize),
                        new Vector2(+ midSize, - midSize),
                        new Vector2(+ midSize, + midSize),
                        new Vector2(- midSize, + midSize),
                        new Vector2(- midSize, - midSize)
                  }
              });

            colliderEntity.Tag = GameConstants.TagCollider;
            colliderEntity.AddComponent(new RigidBody2D() { PhysicBodyType = RigidBodyType2D.Static });

            var collider = colliderEntity.FindComponent<Collider2D>(false);

            if (collider != null)
            {
                collider.CollisionCategories = ColliderCategory2D.Cat3;
                collider.CollidesWith = ColliderCategory2D.All;
                collider.Friction = 1.0f;
                collider.Restitution = 0.2f;
            }

            cube.AddChild(colliderEntity);
        }
        
        private Entity CreateCube(int currentX, int currentY, int z, Material material)
        {
            var cube = new Entity()
                .AddComponent(new Transform3D() { LocalPosition = new Vector3(currentX, z - 1, currentY) })
                .AddComponent(new CubeMesh())
                .AddComponent(new MeshRenderer())
               .AddComponent(new MaterialComponent { Material = material });

            this.map.AddChild(cube);

            return cube;
        }
    }
}

