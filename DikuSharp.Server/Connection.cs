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
    /// <summary>
    /// A client connection to the MUD. Handles all the reading/writing of messages
    /// </summary>
    public class Connection
    {
        private NetworkStream _stream;
        private StreamReader _reader;
        private StreamWriter _writer;
        //these act like 'buffers'
        private string inputBuffer;
        private string outputBuffer;

        public Guid ConnectionId { get; set; }
        public ConnectionStatus ConnectionStatus { get; set; }

        public PlayerAccount Account { get; set; }
        public bool UseColors { get; set; }

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
            ConnectionId = Guid.NewGuid( );
            ConnectionStatus = ConnectionStatus.Connected;
            UseColors = false;//start false
        }
        
        public void Start( )
        {
            Task.Run( ( ) => ClientLoop( ) );
        }

        /// <summary>
        /// Saves a line to the internal output buffer
        /// </summary>
        /// <param name="message"></param>
        public void SendLine( string message )
        {
            var colorMessage = Colorizer.Colorize( message, UseColors );
            outputBuffer += colorMessage;
        }

        public void SendLine( string formatMessage, params object[] arg )
        {
            SendLine( string.Format( formatMessage, arg ) );
        }

        /// <summary>
        /// Used in game loop to assign input to buffer
        /// </summary>
        public void Read()
        {
            inputBuffer = _reader.ReadLine();
        }

        /// <summary>
        /// Used in game loop to write output buffer to client
        /// </summary>
        public void Write()
        {
            _writer.WriteLine(outputBuffer);
            _writer.Flush();
        }

        [Obsolete]
        /// This would/should be handled by gameloop
        private void CleanUp()
        {
            _stream.Dispose();
            Mud.I.RemoveConnection(this);
        }

        [Obsolete]
        /// This code is moved to the game loop - connections will handle adding and removing things from their input/output buffers, but that's it.
        /// What shows up on the first connection is/should be handled by game loop
        private void ClientLoop( )
        {
            try
            {
                Mud.I.AddConnection( this );

                //Ask for colors!
                SendLine("Do you want to use ANSI colors? (y/n):");
                var colorResponse = _reader.ReadLine( );
                if ( !string.IsNullOrEmpty(colorResponse) && colorResponse.ToLower( ) == "y") { UseColors = true; }
                //anything else is just no
                else { UseColors = false; }

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
