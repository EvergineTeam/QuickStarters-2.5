#region Using Statements
using System;
using System.Diagnostics;
using System.Windows.Forms;
using WaveEngine.Adapter; 
#endregion

namespace Mangomaco
{
    static class Program
    {
        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
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

