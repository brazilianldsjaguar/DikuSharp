using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DikuSharp.Server
{
    //Main entry point for the server
    public class Connection
    {
        private TcpClient _tcpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="Connection"/> class.
        /// </summary>
        /// <param name="client">The client.</param>
        public Connection( TcpClient client )
        {
            _tcpClient = client;
        }

        public void Start( )
        {
            if ( _tcpClient == null )
            { throw new Exception( "Cannot start Connection without TcpClient" ); }

            Task.Run( ( ) => ClientLoop( ) );
        }

        private void ClientLoop( )
        {
            try
            {
                while ( true )
                {
                    Mud.I.AddConnection( this );
                }
            }
            finally
            {

            }
        }
    }
}
