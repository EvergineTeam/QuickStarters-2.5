using System;
using System.Runtime.Serialization;
using DeepSpace.Components.Gameplay;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Math;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics2D;
using WaveEngine.Framework.Services;

namespace DeepSpace.Components.Enemies
{
    [DataContract]
    public class EnemiesGeneratorBehavior : Behavior
    {
        private bool isEnemiesGenerated;
        private GameplayBehavior gamePlay;

        [DataMember]
        [RenderPropertyAsEntity(componentsFilter: new string[] { "DeepSpace.Components.Gameplay.GameplayBehavior" })]
        public string GamePlayEntityPath { get; set; }

        public EnemiesGeneratorBehavior() : base("EnemiesGeneratorBehavior")
        {
        }

        protected override void DefaultValues()
        {
            base.DefaultValues();
        }

        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            var gamePlayEntity = this.EntityManager.Find(GamePlayEntityPath);
            if (gamePlayEntity != null)
            {
                this.gamePlay = this.EntityManager.Find(GamePlayEntityPath).FindComponent<GameplayBehavior>();
                this.gamePlay.GameReset += this.GamePlayGameReset;
            }
        }

        protected override void DeleteDependencies()
        {
            base.DeleteDependencies();
            this.gamePlay.GameReset -= this.GamePlayGameReset;
        }

        private void GamePlayGameReset()
        {
            var virtualScreenManager = this.Owner.Scene.VirtualScreenManager;
            if (this.isEnemiesGenerated)
            {
                foreach (var child in this.Owner.ChildEntities)
                {
                    var transform = child.FindComponent<Transform2D>();
                    transform.X = WaveServices.Random.Next((int)virtualScreenManager.VirtualWidth);
                    transform.Y = WaveServices.Random.Next((int)-virtualScreenManager.VirtualHeight * 6, -500);
                }
            }
        }

        protected override void Update(TimeSpan gameTime)
        {
            if (!this.isEnemiesGenerated)
            {
                this.GenerateEnemies();
            }
        }

        private void GenerateEnemies()
        {
            var enemiesList = new Entity[10];
            for (int i = 0; i < enemiesList.Length; i++)
            {
                Entity enemy = new Entity()
                           .AddComponent(new Transform2D()
                           {
                               Origin = Vector2.Center,
                               DrawOrder = 0.5f
                           })
                           .AddComponent(new PerPixelCollider2D(WaveContent.Assets.EnemyCollider_PNG, 0.5f))
                           .AddComponent(new EnemyBehavior() { GamePlayEntityPath = GamePlayEntityPath })
                           .AddComponent(new Sprite(WaveContent.Assets.Enemy_PNG))
                           .AddComponent(new SpriteRenderer(DefaultLayers.Alpha));

                enemiesList[i] = enemy;
                this.Owner.AddChild(enemy);
            }

            this.gamePlay.Enemies = enemiesList;
            this.isEnemiesGenerated = true;

            this.GamePlayGameReset();
        }
    }
}
