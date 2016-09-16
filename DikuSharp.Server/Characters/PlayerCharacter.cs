using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DikuSharp.Server.Characters
{
    public class PlayerCharacter : Character
    {
        [JsonIgnore]
        public Connection CurrentConnection { get; set; }

        /// <summary>
        /// Helper method to send a message to the player via its PC object instead of its connection. Useful inside COMMANDS
        /// </summary>
        /// <param name="message">The message.</param>
        public void SendLine( string message )
        {
            CurrentConnection.SendLine( message );
        }

    }
}
