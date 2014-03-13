using WaveEngine.Adapter;
using Windows.Graphics.Display;
using Windows.UI.Xaml.Controls;
namespace LauncherWindowsStore
{
    public partial class MainPage : SwapChainBackgroundPanel
    {
        private Application application;

        public MainPage()
        {
            InitializeComponent();

            application = new GameRenderer(this);
            application.Initialize();
        }
    }
}