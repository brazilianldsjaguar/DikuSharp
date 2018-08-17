using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DikuSharp.Server
{
    //Main entry point for the server
    class Server
    {
        private static TcpListener _listener;

        /// <summary>
        /// Entry point of the server.
        /// </summary>
        /// <param name="args">The arguments.</param>
        static void Main(string[] args)
        {

            //Start up the Mud...
            Mud.I.StartServer();
            //Listen on the network...
            _listener = new TcpListener(IPAddress.Any, Mud.I.Config.PortNumber);
            _listener.Start();

            try
            {
                Task.Run(async () =>
                {
                    await Mud.I.StartGame(_listener);
                }).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            //if we're here, we're done and we need to stop listening
            _listener.Stop();
        }
    }
}
