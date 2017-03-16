#region Using Statements
using System;
using SlingshotRampage.Services;
using SuperSlingshot.Managers;
using SuperSlingshot.Scenes;
using WaveEngine.Common;
using WaveEngine.Common.Graphics;
using WaveEngine.Framework;
using WaveEngine.Framework.Services;
#endregion

namespace SuperSlingshot
{
    public class Game : WaveEngine.Framework.Game
    {
        public override void Initialize(IApplication application)
        {
            SerializerFactory.DefaultSerializationType = SerializationType.DATACONTRACT;
            base.Initialize(application);

            var navigationManager = new NavigationManager();

            WaveServices.RegisterService(new StorageService());
            WaveServices.RegisterService(new AnimationService());
            WaveServices.RegisterService(new AudioService());
            WaveServices.RegisterService(new GamePlayManager());
            WaveServices.RegisterService(navigationManager);

            navigationManager.InitialNavigation();
        }
    }
}
