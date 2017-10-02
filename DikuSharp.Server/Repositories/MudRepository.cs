using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DikuSharp.Server.Characters;
using DikuSharp.Server.Commands;
using DikuSharp.Server.Config;
using DikuSharp.Server.Helps;
using DikuSharp.Server.Models;
using DikuSharp.Server.Extensions;
using Newtonsoft.Json;

namespace DikuSharp.Server.Repositories
{
    /// <summary>
    /// Responsible for loading and saving data to/from the file system.
    /// </summary>
    public class MudRepository
    {
        /// <summary>
        /// Loads all areas from the area-files list in mud.json
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public List<Area> LoadAreas( MudConfig config )
        {
            var areas = new List<Area>( );
            foreach ( var areaFile in config.AreaFiles )
            {
                var rawJson = File.ReadAllText( areaFile );
                var area = JsonConvert.DeserializeObject<Area>( rawJson );
                area.Rooms.ForEach(r => r.Exits = r.Exits.OrderBy(kv => kv.Key).ToDictionary(kv => kv.Key, kv => kv.Value));
                areas.Add( area );
            }
            return areas;
        }

        /// <summary>
        /// Searches a configurable root directory for all .account files.
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public List<PlayerAccount> LoadAccounts( MudConfig config )
        {
            var accounts = new List<PlayerAccount>( );
            var root = config.AccountFileRootDirectory;

            //make sure this exists
            if ( !Directory.Exists( root ) )
            {
                Directory.CreateDirectory( root );
            }

            //make sure each of the letter directories exist, and load accounts from it
            IEnumerable<string> alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray( ).Select( c => c.ToString( ) );
            foreach ( var a in alphabet )
            {
                var path = Path.Combine( root, a );
                if ( !Directory.Exists( path ) )
                {
                    Directory.CreateDirectory( path );
                }

                foreach ( string accountFile in Directory.GetFiles( path, "*.account" ) )
                {
                    var rawJson = File.ReadAllText( accountFile );
                    var player = JsonConvert.DeserializeObject<PlayerAccount>( rawJson );
                    accounts.Add( player );
                }
            }

            return accounts;
        }

        /// <summary>
        /// Saves many accounts.
        /// </summary>
        /// <param name="config"></param>
        /// <param name="accounts"></param>
        public void SaveAccounts( MudConfig config, List<PlayerAccount> accounts )
        {
            //don't need to check for dirs because we always LOAD first!
            foreach ( var account in accounts )
            {
                SaveAccount( config, account );
            }
        }

        /// <summary>
        /// Saves an account to a file.
        /// </summary>
        /// <param name="config"></param>
        /// <param name="account"></param>
        public void SaveAccount( MudConfig config, PlayerAccount account )
        {
            //don't need to check for dirs because we always LOAD first!
            var root = config.AccountFileRootDirectory;
            var fileName = $"{account.Name.ToTitleCase()}.account";
            var a = account.Name[0].ToString().ToUpper();
            var path = Path.Combine( root, a, fileName );
            var rawJson = JsonConvert.SerializeObject( account, new JsonSerializerSettings( ) { Formatting = Formatting.Indented } );
            File.WriteAllText( path, rawJson );
        }

        /// <summary>
        /// Loads all the help files from the file system.
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public List<Help> LoadHelps( MudConfig config )
        {
            var helps = new List<Help>( );
            var root = config.HelpFileDirectory;
            //check if it exists
            if ( !Directory.Exists( root ) )
            {
                Directory.CreateDirectory( root );
            }

            var helpFiles = Directory.GetFiles( root, "*.help" );
            foreach ( var file in helpFiles )
            {
                var fileName = new FileInfo( file ).Name;
                var rawHelp = File.ReadAllText( file );
                var helpParts = rawHelp.Split( '\n' );
                var keyword = helpParts[ 0 ].Replace( "#", "" ).Replace( "\r", "" ).Replace( "\n", "" );
                var contents = string.Join( "\n", helpParts, 1, helpParts.Length - 1 ).Replace( "\r", "" );
                var help = new Help( )
                {
                    FileName = fileName,
                    Keywords = keyword,
                    Contents = contents
                };
                helps.Add( help );
            }
            return helps;
        }

        /// <summary>
        /// Saves helps.
        /// </summary>
        /// <param name="config"></param>
        /// <param name="helps"></param>
        public void SaveHelps( MudConfig config, List<Help> helps )
        {
            foreach(var help in helps)
            {
                SaveHelp(config, help );
            }
        }

        /// <summary>
        /// Save helps.
        /// </summary>
        /// <param name="config"></param>
        /// <param name="help"></param>
        public void SaveHelp( MudConfig config, Help help )
        {
            var root = config.HelpFileDirectory;
            var path = Path.Combine( root, help.FileName );
            var contents = help.Contents.Replace( "\r", "" ).Replace( "\n", Environment.NewLine );
            var rawHelp = string.Concat( help.Keywords, Environment.NewLine, contents );
            File.WriteAllText( path, rawHelp );
        }

        public List<CommandMetaData> LoadCommands(MudConfig config)
        {
            var metaDatas = new List<CommandMetaData>( );

            var root = config.CommandDirectory;
            if ( !Directory.Exists(root))
            {
                Directory.CreateDirectory( root );
            }

            foreach( var commandMeta in config.Commands )
            {
                var path = Path.Combine( root, commandMeta.FileName );
                if ( File.Exists(path))
                {
                    var rawJs = File.ReadAllText( path );
                    commandMeta.RawJavascript = rawJs;
                    metaDatas.Add(commandMeta);
                }
            }

            return metaDatas.OrderBy( md => md.Priority ).ToList();
        }
    }
}
