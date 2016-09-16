using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DikuSharp.Server.Characters;
using DikuSharp.Server.Helps;
using DikuSharp.Server.Models;
using Newtonsoft.Json;

namespace DikuSharp.Server.Config
{
    public static class MudConfigExtensions
    {
        public static List<Area> LoadAreas(this MudConfig config )
        {
            var areas = new List<Area>( );
            foreach ( var areaFile in config.AreaFiles )
            {
                var rawJson = File.ReadAllText( areaFile );
                var area = JsonConvert.DeserializeObject<Area>( rawJson );
                areas.Add( area );
            }
            return areas;
        }

        public static List<PlayerAccount> LoadAccounts( this MudConfig config )
        {
            var accounts = new List<PlayerAccount>( );
            var root = config.AccountFileRootDirectory;

            //make sure this exists
            if ( !Directory.Exists(root) )
            {
                Directory.CreateDirectory( root );
            }

            //make sure each of the letter directories exist, and load accounts from it
            IEnumerable<string> alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray().Select(c=>c.ToString());
            foreach( var a in alphabet )
            {
                var path = Path.Combine( root, a );
                if ( !Directory.Exists(path) )
                {
                    Directory.CreateDirectory( path );
                }

                foreach( string accountFile in Directory.GetFiles(path, "*.account"))
                {
                    var rawJson = File.ReadAllText( accountFile );
                    var player = JsonConvert.DeserializeObject<PlayerAccount>( rawJson );
                    accounts.Add( player );
                }
            }

            return accounts;
        }

        public static void SaveAccounts( this MudConfig config, List<PlayerAccount> accounts  )
        {
            //don't need to check for dirs because we always LOAD first!
            foreach( var account in accounts )
            {
                config.SaveAccount( account );
            }
        }

        public static void SaveAccount( this MudConfig config, PlayerAccount account )
        {
            //don't need to check for dirs because we always LOAD first!
            var root = config.AccountFileRootDirectory;
            var fileName = account.Name[ 0 ].ToString( ).ToUpper( ) + account.Name.Substring( 1, account.Name.Length - 1 ).ToLower( ) + ".account";
            var a = account.Name[ 0 ].ToString( ).ToUpper( );
            var path = Path.Combine( root, a, fileName );
            var rawJson = JsonConvert.SerializeObject( account, new JsonSerializerSettings( ) { Formatting = Formatting.Indented }  );
            File.WriteAllText( path, rawJson );
        }

        public static List<Help> LoadHelps(this MudConfig config )
        {
            var helps = new List<Help>( );
            var root = config.HelpFileDirectory;
            //check if it exists
            if ( !Directory.Exists( root ))
            {
                Directory.CreateDirectory( root );
            }

            var helpFiles = Directory.GetFiles( root, "*.help" );
            foreach( var file in helpFiles )
            {
                var rawHelp = File.ReadAllText( file );
                var helpParts = rawHelp.Split( '\n' );
                var keyword = helpParts[ 0 ].Replace( "#", "" ).Replace( "\r", "" ).Replace( "\n", "" );
                var contents = string.Join( "\n", helpParts, 1, helpParts.Length - 1 ).Replace("\r", "");
                var help = new Help( )
                {
                    Keywords = keyword,
                    Contents = contents
                };
                helps.Add(help);
            }
            return helps;
        }
    }
}
