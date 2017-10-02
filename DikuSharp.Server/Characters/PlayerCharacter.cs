using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Jint.Runtime.Interop;
using Jint.Native;

namespace DikuSharp.Server.Characters
{
    public class PlayerCharacter : Character
    {
        [JsonIgnore]
        public Connection CurrentConnection { get; set; }

        [JsonProperty("experiencePoints")]
        public int ExperiencePoints { get; set; }
        [JsonProperty("prompt")]
        public Prompt Prompt { get; set; }

        /// <summary>
        /// Gets or sets the xp. Alias for <see cref="ExperiencePoints"/>.
        /// </summary>
        /// <value>
        /// The xp.
        /// </value>
        [JsonIgnore]
        public int Xp { get {  return ExperiencePoints; } set { ExperiencePoints = value; } }


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
