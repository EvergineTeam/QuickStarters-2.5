#region Using Statements
using NewImpossibleGameProject.Behaviors;
using NewImpossibleGameProject.GameModels;
using NewImpossibleGameProject.GameServices;
using System;
using WaveEngine.Common;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Components.Cameras;
using WaveEngine.Components.Graphics3D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics3D;
using WaveEngine.Framework.Services;
using WaveEngine.Materials;
#endregion

namespace NewImpossibleGameProject
{
    public class PlayScene : Scene
    {
        /// <summary>
        /// The model factory service
        /// </summary>
        private ModelFactoryService modelFactoryService = ModelFactoryService.Instance;

        /// <summary>
        /// The player
        /// </summary>
        public Entity Player;

        /// <summary>
        /// The game camera
        /// </summary>
        public Camera3D GameCamera;

        protected override void CreateScene()
        {
            // Lights
            DirectionalLight light = new DirectionalLight("light", new Vector3(-10f, 7f, -5f));
            EntityManager.Add(light);
                        
            RenderManager.FrustumCullingEnabled = false;

            // Game Behavior
            GameBehavior gameBehavior = new GameBehavior();

            // Create Player
            this.Player = this.modelFactoryService.CreatePlayer(gameBehavior);
            EntityManager.Add(this.Player);

            // Create Camera
            var camera = new FixedCamera("mainCamera", Vector3.Zero, Vector3.Zero); // Setted in GameBehavior Init
            camera.BackgroundColor = Color.CornflowerBlue;
            this.GameCamera = camera.Entity.FindComponent<Camera3D>();
            EntityManager.Add(camera);

            // Add Scene Behavior
            this.AddSceneBehavior(gameBehavior, SceneBehavior.Order.PostUpdate);
        }
    }
}
