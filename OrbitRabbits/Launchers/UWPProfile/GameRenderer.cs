using System;
using WaveEngine.Adapter;
using WaveEngine.Common.Input;
using Windows.System.Display;
using Windows.UI.Xaml.Controls;

namespace OrbitRabbits
{
    public class GameRenderer : Application
    {
        private DisplayRequest displayRequest;

        private OrbitRabbits.Game game;

        public GameRenderer(SwapChainPanel panel)
            : base(panel)
        {
            this.FullScreen = true;
        }

        public override void Update(TimeSpan gameTime)
        {  
            this.game.UpdateFrame(gameTime);
        }

        public override void Draw(TimeSpan gameTime)
        {
            this.game.DrawFrame(gameTime);
        }

        public override void Initialize()
        {
            base.Initialize();

            this.Adapter.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;

            this.displayRequest = new DisplayRequest();
            this.displayRequest.RequestActive();

            this.game = new OrbitRabbits.Game();
            this.game.Initialize(this);
        }

        public override void OnResuming()
        {
            base.OnResuming();
            this.game.OnActivated();
        }

        public override void OnSuspending()
        {
            base.OnSuspending();
            this.game.OnDeactivated();
        }
    }
}
