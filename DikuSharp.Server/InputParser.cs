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
using DikuSharp.Server.Models;
using Newtonsoft.Json;
using DikuSharp.Server.Enums;

namespace DikuSharp.Server
{
    //Main entry point for the server
    public static class InputParser
    {
        public static void Parse(Connection connection)
        {
            if ( string.IsNullOrEmpty(connection.InputBuffer) ) { return; }
            
            var line = connection.InputBuffer;
            switch ( connection.ConnectionStatus )
            {
                case ConnectionStatus.Connected:
                    ParseColorChoice(connection, line);
                    break;
                case ConnectionStatus.PutInUsername:
                    ParseUsername(connection, line);
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

        public static void ParseColorChoice(Connection connection, string line )
        {   
            if (!string.IsNullOrEmpty(line) && line.ToLower() == "y") { connection.UseColors = true; }
            //anything else is just no
            else { connection.UseColors = false; }

            //Send the welcome message
            var welcome = Mud.I.Helps.First(h => h.Keywords.ToLower().Contains("welcome"));
            connection.SendLine(welcome.Contents);

            connection.ConnectionStatus = ConnectionStatus.PutInUsername;
        }

        public static void ParseUsername( Connection connection, string line )
        {
            //First line is their user name
            var account = Mud.I.Accounts.FirstOrDefault( a => a.Name.Equals( line, StringComparison.OrdinalIgnoreCase ) );
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
            if ( !password.Equals( connection.Account.Password, StringComparison.Ordinal))
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
                Room room = Mud.I.StartingRoom;
                if ( area != null )
                {
                    room = area.Rooms.FirstOrDefault( r => r.Vnum == connection.CurrentCharacter.CurrentRoomVnum );
                    if ( room == null ) { room = Mud.I.StartingRoom; }
                }
                //assign the room to the player...
                connection.CurrentCharacter.CurrentRoom = room;
                //and the player to the room... (this will be removed when the player quits or moves)
                room.AddCharacter(connection.CurrentCharacter);

                connection.ConnectionStatus = ConnectionStatus.Playing;
                //force the user to look
                ParsePlaying(connection, "look");
            }
            else
            {
                connection.SendLine( "Invalid choice." );
                SendCharacterChoices( connection );
            }
        }

        public static void ParsePlaying( Connection connection, string line )
        {
            ParsePlaying(connection.CurrentCharacter, line);
        }

        public static void ParsePlaying(PlayerCharacter character, string line )
        {
            if ( string.IsNullOrWhiteSpace(line))
            {
                return;
            }

            //split the line to find a command and all arguments
            var cmdAndArgs = _getCommandAndArgsFromInput( line );
            var cmd = cmdAndArgs.Item1.ToLower();
            
            //attempt to find the command
            var command = Mud.I.Commands.FirstOrDefault(c => c.Name.ToLower().StartsWith(cmd))
                    ?? Mud.I.Commands.FirstOrDefault(c => c.Aliases?.FirstOrDefault(a => a.ToLower().StartsWith(cmd)) != null);

            if ( command == null )
            {
                character.SendLine("Huh?");
            }
            else if ( command.Level > character.Level )
            {
                character.SendLine( "Huh?" );
            }
            else
            {
                try
                {
                    //now parse the args, honoring quotes if necessary
                    var args = _parseArgs(cmdAndArgs.Item2, command.PreserveQuotes, ' ').ToArray();

                    //do it!
                    var engine = Mud.I.Engine;                    
                    engine.Execute( command.RawJavascript );
                    var jsCmd = engine.GetValue( command.Name );
                    var arg1 = JsValue.FromObject(engine, character);
                    var arg2 = JsValue.FromObject(engine, args);
                    var result = jsCmd.Invoke(arg1, arg2);
                    
                    //TODO
                    //Do something better here - maybe something returned from the command to signal if another will need to be executed?
                    //or another way to force another command to happen
                    //if (command.CommandType == CommandType.Exit)
                    //{
                    //    ParsePlaying(connection, "look");
                    //}
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
        
        private static Tuple<string, string> _getCommandAndArgsFromInput(string line)
        {

            string cmd = null;
            string args = null;
            //We need to handle some special aliases (e.g. 'hello = "say hello")
            if (line[0] == '\'' || line[0] == ']')
            {
                cmd = line[0].ToString();
                args = new string(line.Skip(1).ToArray());
            }
            else
            {
                //now handle space
                var parts = line.Split(' ');
                cmd = parts[0];
                args = string.Join(" ", parts.Skip(1)); //this preserves the args for later processing
            }

            return new Tuple<string, string>(cmd, args);
        }
        private static List<string> _parseArgs( string args, bool preserveQuotes, params char[] delimiters )
        {
            /*
             * Parse '' quotes (i.e. cast 'magic missile' <person>) - used for multi-word arguments
             *
             * This was adapted from Richard Shepherd's version found here:
             * http://stackoverflow.com/questions/554013/regular-expression-to-split-on-spaces-unless-in-quotes
             */
            List<string> results = new List<string>( );
            
            //check to see if there's an even number of quotes.
            bool inQuote = false;
            StringBuilder currentToken = new StringBuilder( );
            for ( int index = 0; index < args.Length; ++index )
            {
                char currentCharacter = args[ index ];
                if ( currentCharacter == '\'' && !preserveQuotes )
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
