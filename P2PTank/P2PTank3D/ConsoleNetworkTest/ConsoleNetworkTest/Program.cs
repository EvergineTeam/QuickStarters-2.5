using Sockets.Plugin;
using System;

namespace ConsoleNetworkTest
{
    class Program
    {
        static void Main(string[] args)
        {
            int defaultPort = 8080;

            var iface = SelectInterface();
            var port = SelectPort(defaultPort);

            DoLoop(port, iface, false);
        }

        private static void DoLoop(int port, CommsInterface iface, bool tcpOnly)
        {

        }

        private static int SelectPort(int defaultPort)
        {
            var port = defaultPort;
            while (true)
            {
                Console.WriteLine();
                Console.Write($"Select port {port}: ");
                var stringport = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(stringport))
                {
                    return port;
                }

                try
                {
                    var newPort = int.Parse(stringport);

                    if (newPort > 0 && newPort < 65535)
                    {
                        port = newPort;
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return port;
        }

        private static CommsInterface SelectInterface()
        {
            CommsInterface res = null;

            var interfaces = CommsInterface.GetAllInterfacesAsync().Result;
            int selectedIndex = -1;

            do
            {
                int i = 0;
                foreach (var iface in interfaces)
                {
                    Console.WriteLine($"{i} - {iface.Name}");
                    i++;
                }
                Console.WriteLine("-------------------");

                var key = Console.ReadKey();
                Console.WriteLine();

                try
                {
                    selectedIndex = int.Parse(key.KeyChar.ToString());
                }
                catch
                {
                    selectedIndex = -1;
                }
            } while (selectedIndex < 0 || selectedIndex >= interfaces.Count);

            res = interfaces[selectedIndex];

            return res;
        }
    }
}