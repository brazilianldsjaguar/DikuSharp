using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

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
            Connections = new List<Connection>();
        }
        #endregion

        #region Properties
        public List<Connection> Connections { get; private set; }
        #endregion

        /// <summary>
        /// Adds a connection to the mud
        /// </summary>
        /// <param name="connection">The connection.</param>
        public void AddConnection( Connection connection )
        {
            Connections.Add(connection);
        }
    }
}
