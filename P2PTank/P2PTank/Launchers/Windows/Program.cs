using System;

namespace P2PTank
{
    static class Program
	{
		[STAThread]
		static void Main()
		{
			using (App game = new App())
			{
				game.Run();
			}
		}
	}
}

