using System.Security;
using Microsoft.Phone.Controls;
using System.Windows.Controls;
using System.Windows.Media;
using GoogleAds;
using WaveEngine.Adapter;
using System.Diagnostics;

namespace LauncherWP
{
    public partial class MainPage : PhoneApplicationPage
    {
        private Application application;

        [SecuritySafeCritical]
        public MainPage()
        {
            InitializeComponent();

            MediaElement media = new MediaElement();
            DrawingSurface.Children.Add(media);

            application = new GameRenderer(new Windows.Foundation.Size(App.Current.Host.Content.ActualWidth, App.Current.Host.Content.ActualHeight), 100, media);

            DrawingSurface.SetBackgroundContentProvider(application.ContentProvider);
            DrawingSurface.SetBackgroundManipulationHandler(application.ManipulationHandler);

            // Admob
            AdRequest adRequest = new AdRequest();

            adRequest.ForceTesting = true;
            this.admob.LoadAd(adRequest);
        }

        protected override void OnOrientationChanged(OrientationChangedEventArgs e)
        {
            base.OnOrientationChanged(e);
        }

        #region Admob Methods

        private void OnAdReceived(object sender, AdEventArgs e)
        {
            Debug.WriteLine("Received ad successfully");
        }

        private void OnFailedToReceiveAd(object sender, AdErrorEventArgs errorCode)
        {
            Debug.WriteLine("Failed to receive ad with error " + errorCode.ErrorCode);
        }

        #endregion
    }
}