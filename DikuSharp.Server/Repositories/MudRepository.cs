using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DikuSharp.Server.Config;
using DikuSharp.Server.Helps;

namespace DikuSharp.Server.Repositories
{
    public class MudRepository
    {
        public List<Help> GetHelps( MudConfig config )
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
                var rawHelp = File.ReadAllText( file );
                var helpParts = rawHelp.Split( '\n' );
                var keyword = helpParts[ 0 ].Replace( "#", "" ).Replace( "\r", "" ).Replace( "\n", "" );
                var contents = string.Join( "\n", helpParts, 1, helpParts.Length - 1 ).Replace( "\r", "" );
                var help = new Help( )
                {
                    Keywords = keyword,
                    Contents = contents
                };
                helps.Add( help );
            }
            return helps;
        }
    }
}
