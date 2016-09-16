using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DikuSharp.Server.Characters;
using DikuSharp.Server.Colors;

namespace DikuSharp.Server
{
    //Main entry point for the server
    public class Connection
    {
        private NetworkStream _stream;
        private StreamReader _reader;
        private StreamWriter _writer;

        public Guid ConnectionId { get; set; }
        public ConnectionStatus ConnectionStatus { get; set; }

        public PlayerAccount Account { get; set; }

        /// <summary>
        /// Gets or sets the current character. Represents the character the account has chosen
        /// </summary>
        /// <value>
        /// The current character.
        /// </value>
        public PlayerCharacter CurrentCharacter {
            get { return _currentCharacter; }
            set { _currentCharacter = value; _currentCharacter.CurrentConnection = this; }
        }
        private PlayerCharacter _currentCharacter;

        /// <summary>
        /// Initializes a new instance of the <see cref="Connection"/> class.
        /// </summary>
        /// <param name="client">The client.</param>
        public Connection( TcpClient client )
        {
            _stream = client.GetStream( );
            _reader = new StreamReader( _stream );
            _writer = new StreamWriter( _stream );
            _writer.AutoFlush = true;
            ConnectionId = Guid.NewGuid( );
            ConnectionStatus = ConnectionStatus.Connected;
        }
        
        public void Start( )
        {
            Task.Run( ( ) => ClientLoop( ) );
        }

        public void SendLine( string message )
        {
            try
            {
                var colorMessage = Colorizer.Colorize( message, true );
                _writer.WriteLine( colorMessage );
            }
            catch( IOException io )
            {
                CleanUp( );
            }
        }

        public void SendLine( string formatMessage, params object[] arg )
        {
            SendLine( string.Format( formatMessage, arg ) );
        }

        private void CleanUp()
        {
            _stream.Close( );
            Mud.I.RemoveConnection( this );
        }
        private void ClientLoop( )
        {
            try
            {
                Mud.I.AddConnection( this );

                //Send the welcome message
                var welcome = Mud.I.Helps.First( h => h.Keywords.ToLower( ).Contains( "welcome" ) );
                SendLine( welcome.Contents );

                while ( true )
                {
                    string line = _reader.ReadLine( );
                    
                    InputParser.Parse(this, line);
                }
            }
            finally
            {
                CleanUp();
            }
        }
        
    }
}
