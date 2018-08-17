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
using DikuSharp.Server.Events;

namespace DikuSharp.Server
{
    //Main entry point for the server
    public sealed class Mud
    {
        #region Singleton stuff

        /// <summary>
        /// The lazy-loaded Mud singleton class
        /// </summary>
        private static readonly Lazy<Mud> Lazy = new Lazy<Mud>(() => new Mud());

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
        private Mud()
        {
            Connections = new ConcurrentDictionary<Guid, Connection>();
            Repo = new MudRepository();
            EventManager = new EventManager();
        }
        #endregion

        #region Properties
        public MudRepository Repo { get; set; }
        public MudConfig Config { get; set; }
        public ConcurrentDictionary<Guid, Connection> Connections { get; private set; }
        public List<CommandMetaData> Commands { get; private set; }
        public List<Area> Areas { get; private set; }
        public List<Room> AllRooms
        {
            get
            {
                if (_allRooms == null && Areas != null && Areas.All(a => a.Rooms != null))
                {
                    _allRooms = Areas.SelectMany(a => a.Rooms).ToList();
                }
                return _allRooms;
            }
        }
        private List<Room> _allRooms = null;
        public List<PlayerAccount> Accounts { get; private set; }
        public List<Help> Helps { get; private set; }
        public Engine Engine { get; private set; }
        public Room StartingRoom { get; private set; }
        public PlayerCharacter[] AllPlayers { get { return Connections.Select(c => c.Value.CurrentCharacter).ToArray(); } }

        /// <summary>
        /// Core event manager for the MUD.
        /// </summary>
        public EventManager EventManager { get; }
        /// <summary>
        /// Alias for EventManager property.
        /// </summary>
        public EventManager Events => EventManager; //alias for the manager

        private Task<TcpClient> newClientTask;
        private Random tickRandom;
        #endregion

        public const int PULSE_PER_SECOND = 4;
        const int PULSE_TICK = 30 * PULSE_PER_SECOND;
        const int PULSE_TRACK = 40 * PULSE_PER_SECOND;

        public void StartServer()
        {
            //load the config first...
            Config = _getMudConfigFromFile();


            //get things started
            //load up the areas...
            Console.WriteLine("Loading areas...");
            Areas = Repo.LoadAreas(Config);

            Console.WriteLine("Loading accounts...");
            Accounts = Repo.LoadAccounts(Config);

            Console.WriteLine("Loading help files...");
            Helps = Repo.LoadHelps(Config);

            Console.WriteLine("Loading command files...");
            Commands = Repo.LoadCommands(Config);

            Console.WriteLine("Prepping JInt environment...");
            Engine = new Engine(cfg =>
            {
                cfg.DebugMode();
                // cfg.AddObjectConverter(new PlayerCharacterConverter()); //From Character to JsValue
            });
            Engine.SetValue("HELPS", Helps.ToArray());
            Engine.SetValue("DO_COMMAND", new Action<PlayerCharacter, string>(InputParser.ParsePlaying));
            Engine.SetValue("__log", new Action<object>(Console.WriteLine));
            Engine.SetValue("ADMIN_RESTART", new Action(StartServer));
            Engine.SetValue("MUD", this);

            Console.WriteLine("Enqueing core events...");
            EventManager.Initialize();

            //Calculate this just once...
            StartingRoom = Areas.First(a => a.Rooms.Exists(r => r.Vnum == Config.RoomVnumForNewPlayers))
                    .Rooms.First(r => r.Vnum == Config.RoomVnumForNewPlayers);

        }


        /// <summary>
        /// Adds a connection to the mud
        /// </summary>
        /// <param name="connection">The connection.</param>
        public void AddConnection(Connection connection)
        {
            Connections.TryAdd(connection.ConnectionId, connection);
        }

        public void RemoveConnection(Connection connection)
        {
            Connection value;
            Connections.TryRemove(connection.ConnectionId, out value);
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

            DateTime lastTime = DateTime.Now;

            tickRandom = new Random();

            try
            {
                while (mudRunning)
                {
                    try
                    {
                        //New connections
                        HandleNewClient(listener);

                        //Clean up
                        foreach (var conn in Mud.I.Connections.Values)
                        {
                            if (!conn.IsConnected)
                            {
                                Mud.I.RemoveConnection(conn);
                            }
                        }

                        //Input
                        foreach (var conn in Mud.I.Connections.Values)
                        {
                            conn.Read();
                            InputParser.Parse(conn);
                        }

                        //Autonomous game stuff
                        Events.Heartbeat();

                        //Output
                        foreach (var conn in Mud.I.Connections.Values)
                        {
                            OutputParser.Parse(conn);
                            conn.Write();
                        }

                    }
                    catch (IOException)
                    {
                    }

                    //synchornize with the clock
                    DateTime now = DateTime.Now;
                    int msDelta = lastTime.Millisecond - now.Millisecond + 1000 / PULSE_PER_SECOND;
                    int secDelta = lastTime.Second - now.Second;

                    while (msDelta < 0)
                    {
                        msDelta += 1000;
                        secDelta -= 1;
                    }
                    while (msDelta >= 1000)
                    {
                        msDelta -= 1000;
                        secDelta += 1;
                    }
                    if (secDelta > 0 || (secDelta == 0 && msDelta > 0))
                    {
                        Thread.Sleep(new TimeSpan(0, 0, 0, secDelta, msDelta));
                    }

                    lastTime = DateTime.Now;
                }

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                return Task.FromException(ex);
            }
        }

        private void HandleNewClient(TcpListener listener)
        {
            if (newClientTask == null)
            {
                newClientTask = listener.AcceptTcpClientAsync();
            }

            if (newClientTask.IsCompleted)
            {
                var connection = new Connection(newClientTask.Result);
                Mud.I.AddConnection(connection);
                connection.SendWelcomeMessage();

                //listen for a new one
                newClientTask = listener.AcceptTcpClientAsync();
            }
        }
        #endregion

        #region Reading from files


        public MudConfig _getMudConfigFromFile()
        {
            var rawJson = File.ReadAllText("mud.json");
            var mudConfig = JsonConvert.DeserializeObject<MudConfig>(rawJson);
            return mudConfig;
        }

        #endregion
    }
}
