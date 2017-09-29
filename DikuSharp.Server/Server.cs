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
        static void Main( string[ ] args )
        {
            //Start up the Mud...
            Mud.I.Start( );
            //Listen on the network...
            _listener = new TcpListener(IPAddress.Any, 4000); //TODO: Make this configurable
            _listener.Start();
            
            //handle new connections
            while( true )
            {
                Console.WriteLine("Awaiting new connection...");
                TcpClient client = _listener.AcceptTcpClientAsync( ).Result;
                var connection = new Connection(client);
                Console.WriteLine( $"Connection: {client.Client.RemoteEndPoint}" );
                connection.Start();

                //start the client loop

            }
        }
    }
}
