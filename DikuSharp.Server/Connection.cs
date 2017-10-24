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
        private Task<string> inputTask;
        private string outputBuffer;
        private Task outputTask;

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

        public string InputBuffer { get => inputBuffer; }
        public string OutputBuffer { get => outputBuffer; }

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

            //starts the input task - will complete when the user has typed something in.
            inputTask = _reader.ReadLineAsync();
        }
        
        /// <summary>
        /// Saves a line to the internal output buffer
        /// </summary>
        /// <param name="message"></param>
        public void SendLine( string message, bool sendNewLine = true )
        {
            var colorMessage = Colorizer.Colorize( message, UseColors );
            outputBuffer += colorMessage;
            if ( sendNewLine ) { outputBuffer += "\r\n"; }
        }

        /// <summary>
        /// Saves a formated line to the internal output buffer.
        /// </summary>
        /// <param name="formatMessage"></param>
        /// <param name="arg"></param>
        public void SendLine( string formatMessage, bool sendNewLine, params object[] arg )
        {
            SendLine( string.Format( formatMessage, arg ), sendNewLine);
        }

        /// <summary>
        /// Used in game loop to assign input to buffer
        /// </summary>
        public void Read()
        {
            try {
                if (inputTask.IsCompleted)
                {
                    //they've typed something in, so grab it and listen again
                    inputBuffer = inputTask.Result;
                    inputTask = _reader.ReadLineAsync();
                }
                else
                {
                    //null this out so we make sure we never repeat a command
                    inputBuffer = null;
                }
            }
            catch(IOException io)
            {
                //if this failed we'll just assume the client needs to be removed
                Console.WriteLine(io.Message);
                Mud.I.RemoveConnection(this);
            }
            catch (Exception) { throw; } //throw everything else
            
        }
        
        /// <summary>
        /// Used in game loop to write output buffer to client
        /// </summary>
        public void Write()
        {
            if (string.IsNullOrEmpty(outputBuffer)) { return; }

            if (outputTask == null)
            {
                outputTask = _writer.WriteLineAsync(outputBuffer);
            }

            if ( outputTask.IsCompleted )
            {
                _writer.Flush();

                //clear the buffer
                outputBuffer = string.Empty;
                outputTask = null;
            }
        }

        [Obsolete]        
        private void CleanUp()
        {
            _stream.Dispose();
            Mud.I.RemoveConnection(this);
        }
        
        /// This code is moved to the game loop - connections will handle adding and removing things from their input/output buffers, but that's it.
        /// What shows up on the first connection is/should be handled by game loop
        public void SendWelcomeMessage()
        {
            //Ask for colors!
            SendLine("Do you want to use ANSI colors? (y/n):");            
        }
        
    }
}
