using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DikuSharp.Server.Characters;
using DikuSharp.Server.Commands;
using DikuSharp.Server.Extensions;
using Jint;
using Jint.Native;
using Jint.Runtime.Interop;

namespace DikuSharp.Server
{
    //Main entry point for the server
    public static class InputParser
    {
        public static void Parse( Connection connection, string line )
        {
            switch ( connection.ConnectionStatus )
            {
                case ConnectionStatus.Connected:
                    //They just connected, so they're putting in their username
                    ParseUsername( connection, line );
                    break;
                case ConnectionStatus.PutInPassword:
                    ParsePassword( connection, line );
                    break;
                case ConnectionStatus.ChooseCharacter:
                    ParseCharacterChoice( connection, line );
                    break;
                case ConnectionStatus.Playing:
                    ParsePlaying( connection, line );
                    break;
                default:
                    if ( line.StartsWith( "connect" ) )
                    {
                        connection.ConnectionStatus = ConnectionStatus.Playing;
                        connection.SendLine("You have connected...");
                    }
                    else
                    {
                        connection.SendLine(line);
                    }
                    break;
            }
        }

        public static void ParseUsername( Connection connection, string line )
        {
            //First line is their user name
            var account = Mud.I.Accounts.FirstOrDefault( a => a.Name.Equals( line, StringComparison.InvariantCultureIgnoreCase ) );
            if ( account == null )
            {
                connection.SendLine( "Account not found, put in a valid account or type new." );
            }
            else
            {
                connection.Account = account;
                connection.ConnectionStatus = ConnectionStatus.PutInPassword;

                //prep them for password
                connection.SendLine( "Enter your password:");
            }
        }

        public static void ParsePassword( Connection connection, string line )
        {
            //compare passwords
            Encoding encoding = Encoding.UTF8;
            SHA256 sha = SHA256.Create( );
            byte[ ] result = sha.ComputeHash( encoding.GetBytes( line ) );
            //now convert the hash to a string
            string password = string.Join( "", result.Select( b => b.ToString( "x2" ) ) );
            //now compare them
            if ( !password.Equals( connection.Account.Password, StringComparison.InvariantCulture))
            {
                connection.SendLine("Invalid password/username combination");
                connection.Account = null;
                connection.ConnectionStatus = ConnectionStatus.Connected; //back to connected!
            }
            else
            {
                connection.ConnectionStatus = ConnectionStatus.ChooseCharacter;
                //prep them for character choice:
                SendCharacterChoices( connection );
            }
        }

        public static void ParseCharacterChoice( Connection connection, string line )
        {
            Dictionary<int, PlayerCharacter> choices = connection.Account.Characters.GetChoices( );
            int choice;
            if ( int.TryParse(line, out choice) && choices.ContainsKey(choice))
            {
                connection.CurrentCharacter = choices[ choice ];
                //find the room they quit in
                var area = Mud.I.Areas.FirstOrDefault( a => a.Rooms.Exists( r => r.Vnum == connection.CurrentCharacter.CurrentRoomVnum ) );
                if ( area == null ) { connection.CurrentCharacter.CurrentRoom = Mud.I.StartingRoom; }
                else {
                    var room = area.Rooms.FirstOrDefault( r => r.Vnum == connection.CurrentCharacter.CurrentRoomVnum );
                    if ( room == null ) { connection.CurrentCharacter.CurrentRoom = Mud.I.StartingRoom; }
                    else { connection.CurrentCharacter.CurrentRoom = room; }
                }


                connection.ConnectionStatus = ConnectionStatus.Playing;
            }
            else
            {
                connection.SendLine( "Invalid choice." );
                SendCharacterChoices( connection );
            }
        }

        public static void ParsePlaying( Connection connection, string line )
        {
            if ( string.IsNullOrWhiteSpace(line))
            {
                return;
            }

            var split = _getArgsFromInput( line, ' ' );
            //break out spaces first
            var cmd = split[ 0 ].ToLower();
            object[] args = split.Skip( 1 ).Select( s => (object)s ).ToArray();

            //find similar commands
            var command = Mud.I.Commands.FirstOrDefault( c => c.Name.ToLower( ).StartsWith( cmd ) );

            if ( command == null )
            {
                connection.SendLine("Huh?");
            }
            else if ( command.Level > connection.CurrentCharacter.Level )
            {
                connection.SendLine( "Huh?" );
            }
            else
            {
                try
                {
                    //do it!
                    var engine = new Engine( );
                    engine.SetValue( "__log", new Action<object>( Console.WriteLine ) );
                    engine.SetValue( "HELPS", Mud.I.Helps.ToArray( ) );
                    engine.SetValue( "EXITS", connection.CurrentCharacter.CurrentRoom.Exits.Select( x => new { name = x.Key, vnum = x.Value.DestinationVnum } ).ToArray() );
                    //engine.SetValue( "CONNECTIONS", Mud.I.Connections );
                    engine.Execute( command.RawJavascript );
                    var jsCmd = engine.GetValue( command.Name );
                    var result = jsCmd.Invoke( JsValue.FromObject( engine, connection.CurrentCharacter ), JsValue.FromObject( engine, args ) );
                    //engine.Invoke( command.Name, connection, args );
                }
                catch( Exception ex )
                {
                    throw ex;
                }


            }
            //TODO: Make this into a general command interpretter!
            //if ( line.StartsWith( "say " ) )
            //{
            //    foreach ( var c in Mud.I.Connections.Values.Where( c => c.ConnectionStatus == ConnectionStatus.Playing ) )
            //    {
            //        c.SendLine( line.Substring( 4, line.Length - 4 ) );
            //    }
            //}
            //else if ( line.StartsWith( "help ") )
            //{
            //    string[] args = line.Split( ' ' ).Skip( 1 ).ToArray();
            //    var help = Mud.I.Helps.FirstOrDefault( h => h.Keywords.ToLower().Contains( args[ 0 ] ) );
            //    if ( help != null )
            //    {
            //        connection.SendLine(help.Keywords);
            //        connection.SendLine(help.Contents);
            //    }
            //    else
            //    {
            //        connection.SendLine("Help not found.");
            //    }
            //}
            //else
            //{
            //    connection.SendLine( line );
            //}
        }
        
        private static List<string> _getArgsFromInput( string stringToSplit, params char[] delimiters )
        {
            /*
             * Parse '' quotes (i.e. cast 'magic missile' <person>) - used for multi-word arguments
             *
             * This was adapted from Richard Shepherd's version found here:
             * http://stackoverflow.com/questions/554013/regular-expression-to-split-on-spaces-unless-in-quotes
             */
            List<string> results = new List<string>( );

            bool inQuote = false;
            StringBuilder currentToken = new StringBuilder( );
            for ( int index = 0; index < stringToSplit.Length; ++index )
            {
                char currentCharacter = stringToSplit[ index ];
                if ( currentCharacter == '\'' )
                {
                    // When we see a ", we need to decide whether we are
                    // at the start or send of a quoted section...
                    inQuote = !inQuote;
                }
                else if ( delimiters.Contains( currentCharacter ) && inQuote == false )
                {
                    // We've come to the end of a token, so we find the token,
                    // trim it and add it to the collection of results...
                    string result = currentToken.ToString( ).Trim( );
                    if ( result != "" )
                        results.Add( result );

                    // We start a new token...
                    currentToken = new StringBuilder( );
                }
                else
                {
                    // We've got a 'normal' character, so we add it to
                    // the curent token...
                    currentToken.Append( currentCharacter );
                }
            }

            // We've come to the end of the string, so we add the last token...
            string lastResult = currentToken.ToString( ).Trim( );
            if ( lastResult != "" )
                results.Add( lastResult );

            return results;
        }

        public static void SendCharacterChoices( Connection connection )
        {
            connection.SendLine( "Choose a character:" );
            connection.SendLine( "---------------------------" );
            //generate the choices...
            Dictionary<int, PlayerCharacter> choices = connection.Account.Characters.GetChoices( );
            foreach ( var choice in choices )
            {
                connection.SendLine( $"[{choice.Key}]: {choice.Value.Name}" );
            }
            connection.SendLine( "Choice: " );
        }
    }
}
