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
using DikuSharp.Server.Repositories;
using Newtonsoft.Json;
using Jint;

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
            Repo = new MudRepository();
        }
        #endregion

        #region Properties
        public MudRepository Repo { get; set; }
        public MudConfig Config { get; set; }
        public ConcurrentDictionary<Guid,Connection> Connections { get; private set; }
        public List<CommandMetaData> Commands { get; private set; }
        public List<Area> Areas { get; private set; }
        public List<Room> AllRooms
        {
            get
            {
                if ( _allRooms == null && Areas != null && Areas.All(a=>a.Rooms != null))
                {
                    _allRooms = Areas.SelectMany( a => a.Rooms ).ToList( );
                }
                return _allRooms;
            }
        }
        private List<Room> _allRooms = null;
        public List<PlayerAccount> Accounts { get; private set; }
        public List<Help> Helps { get; private set; }
        public Engine Engine { get; private set; }
        public Room StartingRoom { get; private set; }
        public PlayerCharacter[] AllPlayers {  get { return Connections.Select(c => c.Value.CurrentCharacter).ToArray(); } }

        private Task<TcpClient> holderClientTask;
        #endregion

        public void StartServer()
        {
            //load the config first...
            Config = _getMudConfigFromFile();


            //get things started
            //load up the areas...
            Console.WriteLine("Loading areas...");
            Areas = Repo.LoadAreas( Config );

            Console.WriteLine("Loading accounts...");
            Accounts = Repo.LoadAccounts( Config );

            Console.WriteLine("Loading help files...");
            Helps = Repo.LoadHelps( Config );

            Console.WriteLine("Loading command files...");
            Commands = Repo.LoadCommands( Config );

            Console.WriteLine("Prepping JInt environment...");
            Engine = new Engine(cfg => {
                cfg.DebugMode();
               // cfg.AddObjectConverter(new PlayerCharacterConverter()); //From Character to JsValue
                });
            Engine.SetValue("HELPS", Helps.ToArray());
            Engine.SetValue("DO_COMMAND", new Action<PlayerCharacter, string>(InputParser.ParsePlaying));
            Engine.SetValue("__log", new Action<object>(Console.WriteLine));
            Engine.SetValue("ADMIN_RESTART", new Action(StartServer));
            Engine.SetValue("MUD", this);

            //Calculate this just once...
            StartingRoom = Areas.First( a => a.Rooms.Exists( r => r.Vnum == Config.RoomVnumForNewPlayers ) )
                    .Rooms.First( r => r.Vnum == Config.RoomVnumForNewPlayers );
            
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
        /// <summary>
        /// Official starting point of the game.
        /// </summary>
        /// <param name="listener"></param>
        public async Task StartGame(TcpListener listener)
        {
            await GameLoop(listener);
        }

        private Task GameLoop(TcpListener listener)
        {
            bool mudRunning = true;
            
            try
            {
                while (mudRunning)
                {
                    HandleNewClient(listener);

                    foreach (var conn in Mud.I.Connections.Values)
                    {
                        conn.Read();
                        InputParser.Parse(conn);
                    }

                    foreach (var conn in Mud.I.Connections.Values)
                    {
                        OutputParser.Parse(conn);
                        conn.Write();
                    }
                }

                return Task.CompletedTask;
            }
            catch( Exception ex)
            {
                return Task.FromException(ex);
            }
        }

        private void HandleNewClient(TcpListener listener)
        {
            if ( holderClientTask == null )
            {
                holderClientTask = listener.AcceptTcpClientAsync();
            }
            
            if ( holderClientTask.IsCompleted )
            {
                var connection = new Connection(holderClientTask.Result);
                Mud.I.AddConnection(connection);
                connection.SendWelcomeMessage();

                //listen for a new one
                holderClientTask = listener.AcceptTcpClientAsync();
            }
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
