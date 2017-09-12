using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace P2PTank
{
    public sealed partial class MainPage : Page
    {
        private WaveEngine.Adapter.Application application;

        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += this.OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.application = new GameRenderer(this.SwapChainPanel);
            this.application.Initialize();
        }
    }
}