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
            }
        }

        public static void ParseCharacterChoice( Connection connection, string line )
        {
            connection.SendLine( "Choose a character:");
            connection.SendLine("---------------------------");
            //generate the choices...
            Dictionary<int,PlayerCharacter> choices = connection.Account.Characters
                .OrderBy( c => c.Level )
                .Select( ( c, i ) => new KeyValuePair<int, PlayerCharacter>( i, c ) )
                .ToDictionary( kvp => kvp.Key, kvp => kvp.Value );
            foreach( var choice in choices )
            {
                connection.SendLine($"[{choice.Key}]: {choice.Value.Name}");
            }
            connection.SendLine("Choice: ");
            connection.ConnectionStatus = ConnectionStatus.Playing;
        }

        public static void ParsePlaying( Connection connection, string line )
        {
            //TODO: Make this only write lines to connections in the same "room" or "zone" or whatever
            if ( line.StartsWith( "say " ) )
            {
                foreach ( var c in Mud.I.Connections.Values.Where( c => c.ConnectionStatus == ConnectionStatus.Playing ) )
                {
                    c.SendLine( line.Substring( 4, line.Length - 4 ) );
                }
            }
            else if ( line.StartsWith( "help ") )
            {
                string[] args = line.Split( ' ' ).Skip( 1 ).ToArray();
                var help = Mud.I.Helps.FirstOrDefault( h => h.Keywords.ToLower().Contains( args[ 0 ] ) );
                if ( help != null )
                {
                    connection.SendLine(help.Keywords);
                    connection.SendLine(help.Contents);
                }
                else
                {
                    connection.SendLine("Help not found.");
                }
            }
            else
            {
                connection.SendLine( line );
            }
        }
        
    }
}
