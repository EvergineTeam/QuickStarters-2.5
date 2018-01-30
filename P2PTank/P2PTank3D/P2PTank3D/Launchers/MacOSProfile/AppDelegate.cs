using Foundation;
using AppKit;
using P2PTank3D.Services;
using WaveEngine.Framework.Services;

namespace P2PTank3D
{
	public partial class AppDelegate : NSApplicationDelegate
	{
		MainWindowController mainWindowController;

		public override void DidFinishLaunching (NSNotification notification)
		{
			mainWindowController = new MainWindowController ();
			mainWindowController.Window.MakeKeyAndOrderFront (this);

            var localhostService = new LocalhostService
            {
                Localhost = new LocalhostImplementation()
            };
            WaveServices.RegisterService(localhostService);
        }

		public override bool ApplicationShouldTerminateAfterLastWindowClosed(NSApplication sender)
		{
			return true;
		}
	}
}