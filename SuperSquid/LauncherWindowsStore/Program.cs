using System;

namespace LauncherWindowsStore
{
    public static class Program
    {
        [MTAThread]
        private static void Main()
        {
			using (App game = new App())
            {
                game.Run();
            }
        }
    }
}

