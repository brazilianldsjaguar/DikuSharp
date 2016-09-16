using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DikuSharp.Server.Characters;
using DikuSharp.Server.Commands;
using DikuSharp.Server.Config;
using DikuSharp.Server.Helps;
using DikuSharp.Server.Models;
using Newtonsoft.Json;

namespace DikuSharp.Server
{
    //Main entry point for the server
    public sealed class Mud
    {
        #region Singleton stuff

        /// <summary>
        /// The lazy-loaded Mud singleton class
        /// </summary>
        private static readonly Lazy<Mud> Lazy = new Lazy<Mud>( ( ) => new Mud( ) );

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static Mud Instance => Lazy.Value;
        /// <summary>
        /// Gets the instance. Synonym for Instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static Mud I => Instance;

        #endregion

        #region Constructors & Deconstructors
        /// <summary>
        /// Prevents a default instance of the <see cref="Mud"/> class from being created.
        /// </summary>
        private Mud( )
        {
            Connections = new ConcurrentDictionary<Guid,Connection>();
            Commands = new List<IMudCommand>( );
            Config = _getMudConfigFromFile( );
            GameLoop( );
        }
        #endregion

        #region Properties
        public MudConfig Config { get; set; }
        public ConcurrentDictionary<Guid,Connection> Connections { get; private set; }
        public List<IMudCommand> Commands { get; private set; }
        public List<Area> Areas { get; private set; }
        public List<PlayerAccount> Accounts { get; private set; }
        public List<Help> Helps { get; private set; }
        #endregion

        public void Start()
        {
            //get things started
            //load up the areas...
            Console.WriteLine("Loading areas...");
            Areas = Config.LoadAreas( );

            Console.WriteLine("Loading accounts...");
            Accounts = Config.LoadAccounts( );

            Console.WriteLine("Loading help files...");
            Helps = Config.LoadHelps( );
        }
        /// <summary>
        /// Adds a connection to the mud
        /// </summary>
        /// <param name="connection">The connection.</param>
        public void AddConnection( Connection connection )
        {
            Connections.TryAdd( connection.ConnectionId, connection );
        }

        public void RemoveConnection( Connection connection )
        {
            Connection value;
            Connections.TryRemove( connection.ConnectionId, out value );
        }

        #region Game Loop

        private void GameLoop()
        {
            Timer timer = new Timer((state) =>
            {
                var playingConnections = Connections.Values.Where( c => c.ConnectionStatus == ConnectionStatus.Playing );
                foreach ( var c in playingConnections )
                {
                    c.SendLine( "A big haboob blows by..." );
                }
            }, new { t = false }, 0, 3600 );

        }

        #endregion

        #region Reading from files

        
        public MudConfig _getMudConfigFromFile()
        {
            var rawJson = File.ReadAllText( "mud.json" );
            var mudConfig = JsonConvert.DeserializeObject<MudConfig>( rawJson );
            return mudConfig;
        }

        #endregion
    }
}
