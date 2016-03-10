using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace DeepSpace
{
    public sealed partial class MainPage : Page
    {
        private WaveEngine.Adapter.Application application;

        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += this.OnLoaded;
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait | DisplayOrientations.PortraitFlipped;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.application = new GameRenderer(this.SwapChainPanel);
            this.application.Initialize();
            this.application.Adapter.SupportedOrientations = WaveEngine.Common.Input.DisplayOrientation.Portrait;
        }
    }
}
